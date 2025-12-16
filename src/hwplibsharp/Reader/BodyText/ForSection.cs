using HwpLib.CompoundFile;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control;
using HwpLib.Reader.BodyText.Control.Gso;
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
        // 수식 컨트롤인 경우
        else if (ctrlId == ControlType.Equation.GetCtrlId())
        {
            var eqed = new ControlEquation();
            var fce = new ForControlEquation();
            fce.Read(eqed, _sr);
            _currentParagraph.AddControl(eqed);
            _lastControl = eqed;
        }
        // Gso 컨트롤인 경우
        else if (ctrlId == ControlType.Gso.GetCtrlId())
        {
            var fgc = new ForGsoControl();
            fgc.Read(_currentParagraph, _sr);
            // ForGsoControl.Read() 메서드 내부에서 _currentParagraph.AddNewGsoControl()을 호출하여
            // 컨트롤을 추가하므로 여기서는 별도로 AddControl()을 호출하지 않습니다.
            // ControlList의 마지막 항목을 _lastControl로 설정합니다.
            if (_currentParagraph.ControlList != null && _currentParagraph.ControlList.Count > 0)
            {
                _lastControl = _currentParagraph.ControlList[^1];
            }
            else
            {
                _lastControl = null;
            }
        }
        // Form 컨트롤인 경우 (아직 지원되지 않음 - 스킵)
        else if (ctrlId == ControlType.Form.GetCtrlId())
        {
            SkipControlWithSubRecords();
            _lastControl = null;
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
        // 머리말 컨트롤인 경우
        else if (ctrlId == ControlType.Header.GetCtrlId())
        {
            var head = new ControlHeader();
            var fch = new ForControlHeader();
            fch.Read(head, _sr);
            _currentParagraph.AddControl(head);
            _lastControl = head;
        }
        // 꼬리말 컨트롤인 경우
        else if (ctrlId == ControlType.Footer.GetCtrlId())
        {
            var foot = new ControlFooter();
            var fcf = new ForControlFooter();
            fcf.Read(foot, _sr);
            _currentParagraph.AddControl(foot);
            _lastControl = foot;
        }
        // 각주 컨트롤인 경우
        else if (ctrlId == ControlType.Footnote.GetCtrlId())
        {
            var fn = new ControlFootnote();
            var fcfn = new ForControlFootnote();
            fcfn.Read(fn, _sr);
            _currentParagraph.AddControl(fn);
            _lastControl = fn;
        }
        // 미주 컨트롤인 경우
        else if (ctrlId == ControlType.Endnote.GetCtrlId())
        {
            var en = new ControlEndnote();
            var fcen = new ForControlEndnote();
            fcen.Read(en, _sr);
            _currentParagraph.AddControl(en);
            _lastControl = en;
        }
        // 자동 번호 컨트롤인 경우
        else if (ctrlId == ControlType.AutoNumber.GetCtrlId())
        {
            var an = new ControlAutoNumber();
            ForControlAutoNumber.Read(an, _sr);
            _currentParagraph.AddControl(an);
            _lastControl = an;
        }
        // 새 번호 지정 컨트롤인 경우
        else if (ctrlId == ControlType.NewNumber.GetCtrlId())
        {
            var nwno = new ControlNewNumber();
            ForControlNewNumber.Read(nwno, _sr);
            _currentParagraph.AddControl(nwno);
            _lastControl = nwno;
        }
        // 감추기 컨트롤인 경우
        else if (ctrlId == ControlType.PageHide.GetCtrlId())
        {
            var pghd = new ControlPageHide();
            ForControlPageHide.Read(pghd, _sr);
            _currentParagraph.AddControl(pghd);
            _lastControl = pghd;
        }
        // 홀/짝수 조정 컨트롤인 경우
        else if (ctrlId == ControlType.PageOddEvenAdjust.GetCtrlId())
        {
            var pgoea = new ControlPageOddEvenAdjust();
            ForControlPageOddEvenAdjust.Read(pgoea, _sr);
            _currentParagraph.AddControl(pgoea);
            _lastControl = pgoea;
        }
        // 쪽 번호 위치 컨트롤인 경우
        else if (ctrlId == ControlType.PageNumberPosition.GetCtrlId())
        {
            var pgnp = new ControlPageNumberPosition();
            ForControlPageNumberPosition.Read(pgnp, _sr);
            _currentParagraph.AddControl(pgnp);
            _lastControl = pgnp;
        }
        // 찾아보기 표식 컨트롤인 경우
        else if (ctrlId == ControlType.IndexMark.GetCtrlId())
        {
            var idxm = new ControlIndexMark();
            ForControlIndexMark.Read(idxm, _sr);
            _currentParagraph.AddControl(idxm);
            _lastControl = idxm;
        }
        // 책갈피 컨트롤인 경우
        else if (ctrlId == ControlType.Bookmark.GetCtrlId())
        {
            var bkmk = new ControlBookmark();
            ForControlBookmark.Read(bkmk, _sr);
            _currentParagraph.AddControl(bkmk);
            _lastControl = bkmk;
        }
        // 글자 겹침 컨트롤인 경우
        else if (ctrlId == ControlType.OverlappingLetter.GetCtrlId())
        {
            var tcps = new ControlOverlappingLetter();
            ForControlOverlappingLetter.Read(tcps, _sr);
            _currentParagraph.AddControl(tcps);
            _lastControl = tcps;
        }
        // 덧말 컨트롤인 경우
        else if (ctrlId == ControlType.AdditionalText.GetCtrlId())
        {
            var at = new ControlAdditionalText();
            ForControlAdditionalText.Read(at, _sr);
            _currentParagraph.AddControl(at);
            _lastControl = at;
        }
        // 숨은 설명 컨트롤인 경우
        else if (ctrlId == ControlType.HiddenComment.GetCtrlId())
        {
            var tcmt = new ControlHiddenComment();
            var fchc = new ForControlHiddenComment();
            fchc.Read(tcmt, _sr);
            _currentParagraph.AddControl(tcmt);
            _lastControl = tcmt;
        }
        else
        {
            // 다른 종류의 컨트롤은 아직 구현되지 않음
            _sr.SkipToEndRecord();
            _lastControl = null;
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
                if (!_sr.ReadRecordHeader())
                    break;
            }
            if (ctrlHeaderLevel >= _sr.CurrentRecordHeader!.Level)
            {
                break;
            }
            _sr.SkipToEndRecord();
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
