using HwpLib.CompoundFile;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Gso;
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
    /// 현재 건너뛰어야 할 레코드 레벨 (-1이면 건너뛰기 모드 아님)
    /// </summary>
    private int _skipUntilLevel = -1;

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
        _skipUntilLevel = -1;

        while (!sr.IsEndOfStream())
        {
            if (!sr.ReadRecordHeader())
                break;
            
            var currentLevel = sr.CurrentRecordHeader!.Level;
            
            // 건너뛰기 모드인 경우: 현재 레벨이 건너뛰기 시작 레벨보다 높으면 자식 레코드이므로 건너뜀
            if (_skipUntilLevel >= 0)
            {
                if (currentLevel > _skipUntilLevel)
                {
                    // 자식 레코드이므로 건너뜀
                    sr.SkipToEndRecord();
                    continue;
                }
                else
                {
                    // 같은 레벨이거나 더 낮은 레벨이면 건너뛰기 모드 종료
                    // 이 레코드는 정상적으로 처리해야 함
                    _skipUntilLevel = -1;
                }
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

        // 현재 레벨 저장 - 자식 레코드들을 건너뛰기 위함
        int ctrlLevel = _sr.CurrentRecordHeader!.Level;

        // 컨트롤 ID를 읽는다 (4바이트)
        uint ctrlId = _sr.ReadUInt4();

        // 필드 컨트롤인 경우
        if (ControlTypeExtensions.IsField(ctrlId))
        {
            var field = new ControlField(ctrlId);
            ForControlField.ReadCtrlHeader(field, _sr);
            _currentParagraph.AddControl(field);
            _lastControl = field;
            // 필드 컨트롤의 자식 레코드 건너뛰기
            _skipUntilLevel = ctrlLevel;
        }
        // GSO 컨트롤인 경우
        else if (ctrlId == ControlType.Gso.GetCtrlId())
        {
            ReadGsoControl(ctrlLevel);
        }
        // 구역 정의 컨트롤인 경우
        else if (ctrlId == ControlType.SectionDefine.GetCtrlId())
        {
            var sectionDefine = new ControlSectionDefine();
            // 헤더 읽기는 건너뛰고 컨트롤만 추가
            _sr.SkipToEndRecord();
            _currentParagraph.AddControl(sectionDefine);
            _lastControl = sectionDefine;
            // 자식 레코드 건너뛰기
            _skipUntilLevel = ctrlLevel;
        }
        // 단 정의 컨트롤인 경우
        else if (ctrlId == ControlType.ColumnDefine.GetCtrlId())
        {
            var columnDefine = new ControlColumnDefine();
            _sr.SkipToEndRecord();
            _currentParagraph.AddControl(columnDefine);
            _lastControl = columnDefine;
            _skipUntilLevel = ctrlLevel;
        }
        else
        {
            // 다른 종류의 컨트롤은 아직 구현되지 않음
            // 알 수 없는 컨트롤도 건너뛰되 자식 레코드들도 건너뜀
            _skipUntilLevel = ctrlLevel;
            _sr.SkipToEndRecord();
            _lastControl = null;
        }
    }

    /// <summary>
    /// GSO 컨트롤을 읽는다.
    /// </summary>
    private void ReadGsoControl(int gsoLevel)
    {
        if (_currentParagraph == null || _sr == null)
        {
            _sr!.SkipToEndRecord();
            return;
        }

        // GSO 컨트롤 헤더를 읽는다
        var header = new CtrlHeaderGso();
        ForCtrlHeaderGso.Read(header, _sr);

        // 남은 레코드 데이터 건너뛰기
        _sr.SkipToEndRecord();

        // GSO 컨트롤은 하위 레코드에서 gsoId를 읽어서 생성해야 하지만,
        // 현재는 기본적으로 사각형 컨트롤로 생성
        var gsoControl = new ControlRectangle(header);
        _currentParagraph.AddControl(gsoControl);
        _lastControl = gsoControl;

        // GSO 컨트롤의 자식 레코드들을 건너뛰도록 설정
        _skipUntilLevel = gsoLevel;
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
