using HwpLib.CompoundFile;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso;
using HwpLib.Reader.BodyText.Paragraph;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 문단 리스트를 읽는 객체 (캡션, 표, 머리글/꼬리글, 각주/미주 등등 등)
/// </summary>
public static class ForParagraphList
{
    /// <summary>
    /// 문단 리스트를 읽는다.
    /// </summary>
    /// <param name="pli">문단 리스트 객체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(IParagraphList pli, CompoundStreamReader sr)
    {
        var fp = new ForParagraph();
        sr.ReadRecordHeader();
        
        while (!sr.IsEndOfStream())
        {
            var para = pli.AddNewParagraph();
            fp.Read(para, sr);
            if (para.Header.LastInList)
            {
                break;
            }
        }
    }
}

/// <summary>
/// 하나의 문단을 읽기 위한 객체
/// </summary>
public class ForParagraph
{
    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 문단 헤더의 level
    /// </summary>
    private short _paraHeaderLevel;

    /// <summary>
    /// 문단 객체
    /// </summary>
    private Object.BodyText.Paragraph.Paragraph? _paragraph;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForParagraph()
    {
    }

    /// <summary>
    /// 하나의 문단을 읽는다.
    /// </summary>
    /// <param name="paragraph">문단 객체</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(Object.BodyText.Paragraph.Paragraph paragraph, CompoundStreamReader sr)
    {
        if (sr.CurrentRecordHeader?.TagId != HWPTag.ParaHeader)
        {
            throw new InvalidOperationException("This is not paragraph.");
        }

        _sr = sr;
        _paragraph = paragraph;
        _paraHeaderLevel = (short)sr.CurrentRecordHeader.Level;
        
        ParaHeaderBody();
        ParaText();
        ParaCharShape();
        ParaLineSeg();
        ParaRangeTag();

        while (!sr.IsEndOfStream())
        {
            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
            if (IsOutOfParagraph() || IsFollowLastBatangPageInfo() || IsFollowMemo())
            {
                break;
            }
            if (sr.CurrentRecordHeader?.TagId == HWPTag.CtrlHeader)
            {
                Control();
            }
            else
            {
                SkipETCRecord();
            }
        }
    }

    /// <summary>
    /// 문단 헤더 레코드를 읽는다.
    /// </summary>
    private void ParaHeaderBody()
    {
        ForParaHeader.Read(_paragraph!.Header, _sr!);
    }

    /// <summary>
    /// 문단의 텍스트 레코드를 읽는다.
    /// </summary>
    private void ParaText()
    {
        if (_sr!.IsEndOfStream()) return;

        if (!_sr.IsImmediatelyAfterReadingHeader)
        {
            _sr.ReadRecordHeader();
        }
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ParaText)
        {
            ForParaText.Read(_paragraph!, _sr);
        }
    }

    /// <summary>
    /// 문단의 문자 모양 레코드를 읽는다.
    /// </summary>
    private void ParaCharShape()
    {
        if (_sr!.IsEndOfStream()) return;

        if (!_sr.IsImmediatelyAfterReadingHeader)
        {
            _sr.ReadRecordHeader();
        }
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ParaCharShape)
        {
            if (_paragraph!.CharShape == null) _paragraph.CreateCharShape();
            ForParaCharShape.Read(_paragraph.CharShape!, _sr);
        }
    }

    /// <summary>
    /// 문단의 레이아웃 레코드를 읽는다.
    /// </summary>
    private void ParaLineSeg()
    {
        if (_sr!.IsEndOfStream()) return;

        if (!_sr.IsImmediatelyAfterReadingHeader)
        {
            _sr.ReadRecordHeader();
        }
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ParaLineSeg)
        {
            ForParaLineSeg.Read(_paragraph!, _sr);
        }
    }

    /// <summary>
    /// 문단의 영역 태그 레코드를 읽는다.
    /// </summary>
    private void ParaRangeTag()
    {
        if (_sr!.IsEndOfStream()) return;

        if (!_sr.IsImmediatelyAfterReadingHeader)
        {
            _sr.ReadRecordHeader();
        }
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ParaRangeTag)
        {
            if (_paragraph!.RangeTag == null) _paragraph.CreateRangeTag();
            ForParaRangeTag.Read(_paragraph.RangeTag!, _sr);
        }
    }

    /// <summary>
    /// 읽은 레코드 헤더가 문단 바깥쪽인지 여부를 반환한다.
    /// </summary>
    private bool IsOutOfParagraph()
    {
        return _paraHeaderLevel >= _sr!.CurrentRecordHeader!.Level;
    }

    /// <summary>
    /// 마지막 바탕쪽 정보가 뒤에 붙어 있는지 여부를 반환한다.
    /// </summary>
    private bool IsFollowLastBatangPageInfo()
    {
        return _paraHeaderLevel == 0
            && _sr!.CurrentRecordHeader?.TagId == HWPTag.ListHeader
            && _sr.CurrentRecordHeader.Level == 1;
    }

    /// <summary>
    /// 메모 정보가 뒤에 붙어 있는지 여부를 반환한다.
    /// </summary>
    private bool IsFollowMemo()
    {
        return _paraHeaderLevel == 0
            && _sr!.CurrentRecordHeader?.TagId == HWPTag.MemoList
            && _sr.CurrentRecordHeader.Level == 1;
    }

    /// <summary>
    /// 문단에 포함된 컨트롤을 읽는다.
    /// </summary>
    private void Control()
    {
        uint id = _sr!.ReadUInt4();
        
        // Gso 컨트롤인 경우 - 임시로 스킵
        if (id == ControlType.Gso.GetCtrlId())
        {
            SkipControlWithSubRecords();
            return;
        }
        
        // Form 컨트롤인 경우 (현재 지원하지 않음)
        if (id == ControlType.Form.GetCtrlId())
        {
            SkipControlWithSubRecords();
            return;
        }
        
        // 다른 컨트롤은 ForControl을 통해 읽는다
        var c = _paragraph!.AddNewControl(id);
        if (c != null)
        {
            ForControl.Read(c, _sr);
        }
    }

    /// <summary>
    /// 하위 레코드를 가진 컨트롤을 건너뛴다.
    /// </summary>
    private void SkipControlWithSubRecords()
    {
        var ctrlHeaderLevel = _sr!.CurrentRecordHeader!.Level;
        _sr.SkipToEndRecord();

        while (!_sr.IsEndOfStream())
        {
            if (!_sr.IsImmediatelyAfterReadingHeader)
            {
                _sr.ReadRecordHeader();
            }
            if (ctrlHeaderLevel >= _sr.CurrentRecordHeader!.Level)
            {
                break;
            }
            _sr.SkipToEndRecord();
        }
    }

    /// <summary>
    /// 기타 레코드를 스킵한다.
    /// </summary>
    private void SkipETCRecord()
    {
        _sr!.SkipToEndRecord();
    }
}
