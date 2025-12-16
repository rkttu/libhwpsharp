using HwpLib.CompoundFile;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control;
using HwpLib.Reader.BodyText.Paragraph;
using ControlNS = HwpLib.Object.BodyText.Control;

namespace HwpLib.Reader.BodyText;

/// <summary>
/// 구역(Section)을 읽기 위한 객체
/// </summary>
public class ForSection
{
    /// <summary>
    /// 구역 객체
    /// </summary>
    private Section? _section;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 현재 문단
    /// </summary>
    private Object.BodyText.Paragraph.Paragraph? _currentParagraph;

    /// <summary>
    /// 마지막으로 읽은 컨트롤
    /// </summary>
    private ControlNS.Control? _lastControl;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForSection()
    {
    }

    /// <summary>
    /// 구역을 읽는다.
    /// </summary>
    /// <param name="section">구역 객체</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(Section section, CompoundStreamReader sr)
    {
        _section = section;
        _sr = sr;

        while (!sr.IsEndOfStream())
        {
            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                if (!sr.ReadRecordHeader())
                    break;
            }
            ReadRecordBody();
        }
    }

    /// <summary>
    /// 이미 읽은 레코드 헤더에 따른 레코드 내용을 읽는다.
    /// </summary>
    private void ReadRecordBody()
    {
        if (_sr == null || _section == null || _sr.CurrentRecordHeader == null)
            return;

        var tagId = (short)_sr.CurrentRecordHeader.TagId;

        if (tagId == HWPTag.ParaHeader)
        {
            ReadParaHeader();
        }
        else if (tagId == HWPTag.ParaText)
        {
            ReadParaText();
        }
        else if (tagId == HWPTag.ParaCharShape)
        {
            ReadParaCharShape();
        }
        else if (tagId == HWPTag.ParaLineSeg)
        {
            ReadParaLineSeg();
        }
        else if (tagId == HWPTag.ParaRangeTag)
        {
            ReadParaRangeTag();
        }
        else if (tagId == HWPTag.CtrlHeader)
        {
            ReadCtrlHeader();
        }
        else if (tagId == HWPTag.CtrlData)
        {
            ReadCtrlData();
        }
        else
        {
            // 알 수 없는 태그는 건너뛴다
            _sr.SkipToEndRecord();
        }
    }

    /// <summary>
    /// 문단 헤더 레코드를 읽는다.
    /// </summary>
    private void ReadParaHeader()
    {
        _currentParagraph = _section!.AddNewParagraph();
        ForParaHeader.Read(_currentParagraph.Header, _sr!);
    }

    /// <summary>
    /// 문단 텍스트 레코드를 읽는다.
    /// </summary>
    private void ReadParaText()
    {
        if (_currentParagraph == null) return;
        ForParaText.Read(_currentParagraph, _sr!);
    }

    /// <summary>
    /// 문단 글자 모양 레코드를 읽는다.
    /// </summary>
    private void ReadParaCharShape()
    {
        if (_currentParagraph == null) return;
        if (_currentParagraph.CharShape == null)
        {
            _currentParagraph.CreateCharShape();
        }
        ForParaCharShape.Read(_currentParagraph.CharShape!, _sr!);
    }

    /// <summary>
    /// 문단 레이아웃 레코드를 읽는다.
    /// </summary>
    private void ReadParaLineSeg()
    {
        if (_currentParagraph == null) return;
        if (_currentParagraph.LineSeg == null)
        {
            _currentParagraph.CreateLineSeg();
        }
        ForParaLineSeg.Read(_currentParagraph.LineSeg!, _sr!);
    }

    /// <summary>
    /// 문단 영역 태그 레코드를 읽는다.
    /// </summary>
    private void ReadParaRangeTag()
    {
        if (_currentParagraph == null) return;
        if (_currentParagraph.RangeTag == null)
        {
            _currentParagraph.CreateRangeTag();
        }
        ForParaRangeTag.Read(_currentParagraph.RangeTag!, _sr!);
    }

    /// <summary>
    /// 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void ReadCtrlHeader()
    {
        if (_currentParagraph == null || _sr == null)
        {
            _sr!.SkipToEndRecord();
            return;
        }

        // 컨트롤 ID를 읽는다 (4바이트)
        uint ctrlId = _sr.ReadUInt4();

        // 필드 컨트롤인 경우
        if (ControlTypeExtensions.IsField(ctrlId))
        {
            var field = new ControlField(ctrlId);
            ForControlField.ReadCtrlHeader(field, _sr);
            _currentParagraph.AddControl(field);
            _lastControl = field;
        }
        // 표 컨트롤인 경우
        else if (ctrlId == ControlType.Table.GetCtrlId())
        {
            var table = new ControlTable();
            var fct = new ForControlTable();
            fct.Read(table, _sr);
            _currentParagraph.AddControl(table);
            _lastControl = table;
        }
        // 구역 정의 컨트롤인 경우
        else if (ctrlId == ControlType.SectionDefine.GetCtrlId())
        {
            var secd = new ControlSectionDefine();
            var fcsd = new ForControlSectionDefine();
            fcsd.Read(secd, _sr);
            _currentParagraph.AddControl(secd);
            _lastControl = secd;
        }
        // 단 정의 컨트롤인 경우
        else if (ctrlId == ControlType.ColumnDefine.GetCtrlId())
        {
            var cold = new ControlColumnDefine();
            ForControlColumnDefine.Read(cold, _sr);
            _currentParagraph.AddControl(cold);
            _lastControl = cold;
        }
        else
        {
            // 다른 종류의 컨트롤은 아직 구현되지 않음
            _sr.SkipToEndRecord();
            _lastControl = null;
        }
    }

    /// <summary>
    /// 컨트롤 데이터 레코드를 읽는다.
    /// </summary>
    private void ReadCtrlData()
    {
        if (_lastControl == null || _sr == null)
        {
            _sr!.SkipToEndRecord();
            return;
        }

        var ctrlData = ForCtrlData.Read(_sr);
        _lastControl.SetCtrlData(ctrlData);
        _sr.SkipToEndRecord();
    }
}
