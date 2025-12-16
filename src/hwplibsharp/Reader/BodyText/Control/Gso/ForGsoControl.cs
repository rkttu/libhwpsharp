using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Bookmark;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.Caption;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 그리기 개체 컨트롤들을 읽는다.
/// </summary>
public class ForGsoControl
{
    /// <summary>
    /// 문단 객체
    /// </summary>
    private Object.BodyText.Paragraph.Paragraph? _paragraph;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 생성된 그리기 개체 컨트롤
    /// </summary>
    private GsoControl? _gsoControl;

    private CtrlHeaderGso? _header;
    private Caption? _caption;
    private CtrlData? _ctrlData;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForGsoControl()
    {
    }

    /// <summary>
    /// 그리기 개체 컨트롤을 읽는다.
    /// </summary>
    /// <param name="paragraph">문단 객체</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(Object.BodyText.Paragraph.Paragraph paragraph, CompoundStreamReader sr)
    {
        _paragraph = paragraph;
        _sr = sr;

        CtrlHeader();
        CaptionAndCtrlData();

        long gsoId = GsoIDFromShapeComponent();
        _gsoControl = CreateGsoControl(gsoId);
        RestPartOfShapeComponent();
        RestPartOfControl();
    }

    /// <summary>
    /// 그리기 개체의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        _header = new CtrlHeaderGso();
        ForCtrlHeaderGso.Read(_header, _sr!);
    }

    /// <summary>
    /// 캡션과 컨트롤 데이터를 읽는다.
    /// </summary>
    private void CaptionAndCtrlData()
    {
        _caption = null;
        _ctrlData = null;

        _sr!.ReadRecordHeader();
        while (_sr.CurrentRecordHeader?.TagId != HWPTag.ShapeComponent)
        {
            if (_sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
            {
                _caption = new Caption();
                ForCaption.Read(_caption, _sr);
                if (!_sr.IsImmediatelyAfterReadingHeader)
                {
                    _sr.ReadRecordHeader();
                }
            }
            else if (_sr.CurrentRecordHeader?.TagId == HWPTag.CtrlData)
            {
                _ctrlData = ForCtrlData.Read(_sr);
                if (!_sr.IsImmediatelyAfterReadingHeader)
                {
                    _sr.ReadRecordHeader();
                }
            }
        }
    }

    /// <summary>
    /// 객체 공통 속성 레코드로부터 그리기 개체의 id를 읽는다.
    /// </summary>
    /// <returns>그리기 개체의 id</returns>
    private long GsoIDFromShapeComponent()
    {
        if (!_sr!.IsImmediatelyAfterReadingHeader)
        {
            _sr.ReadRecordHeader();
        }
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponent)
        {
            long id = _sr.ReadUInt4();
            _sr.Skip(4); // id2
            return id;
        }
        else
        {
            throw new InvalidOperationException("Shape Component must come after CtrlHeader for gso control.");
        }
    }

    /// <summary>
    /// 그리기 개체 컨트롤을 생성한다.
    /// </summary>
    /// <param name="gsoId">그리기 개체 아이디</param>
    /// <returns>생성된 그리기 개체 컨트롤</returns>
    private GsoControl CreateGsoControl(long gsoId)
    {
        var gc = _paragraph!.AddNewGsoControl((uint)gsoId, _header!);
        gc.SetCaption(_caption);
        gc.SetCtrlData(_ctrlData);
        return gc;
    }

    /// <summary>
    /// 객체 공통 속성 레코드의 나머지 부분을 읽는다.
    /// </summary>
    private void RestPartOfShapeComponent()
    {
        ForShapeComponent.Read(_gsoControl!, _sr!);
    }

    /// <summary>
    /// 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    private void RestPartOfControl()
    {
        switch (_gsoControl!.GsoType)
        {
            case GsoControlType.Line:
                ForControlLine.ReadRest((ControlLine)_gsoControl, _sr!);
                break;
            case GsoControlType.Rectangle:
                ForControlRectangle.ReadRest((ControlRectangle)_gsoControl, _sr!);
                break;
            case GsoControlType.Ellipse:
                ForControlEllipse.ReadRest((ControlEllipse)_gsoControl, _sr!);
                break;
            case GsoControlType.Arc:
                ForControlArc.ReadRest((ControlArc)_gsoControl, _sr!);
                break;
            case GsoControlType.Polygon:
                ForControlPolygon.ReadRest((ControlPolygon)_gsoControl, _sr!);
                break;
            case GsoControlType.Curve:
                ForControlCurve.ReadRest((ControlCurve)_gsoControl, _sr!);
                break;
            case GsoControlType.Picture:
                ForControlPicture.ReadRest((ControlPicture)_gsoControl, _sr!);
                break;
            case GsoControlType.OLE:
                ForControlOLE.ReadRest((ControlOLE)_gsoControl, _sr!);
                break;
            case GsoControlType.Container:
                ForControlContainer.ReadRest((ControlContainer)_gsoControl, _sr!);
                break;
            case GsoControlType.ObjectLinkLine:
                ForControlObjectLinkLine.ReadRest((ControlObjectLinkLine)_gsoControl, _sr!);
                break;
            case GsoControlType.TextArt:
                ForControlTextArt.ReadRest((ControlTextArt)_gsoControl, _sr!);
                break;
        }
    }

    /// <summary>
    /// 묶음 컨트롤 안에 포함된 컨트롤을 읽는다.
    /// </summary>
    /// <param name="sr">스트림 리더</param>
    /// <returns>묶음 컨트롤 안에 포함된 컨트롤</returns>
    public GsoControl ReadInContainer(CompoundStreamReader sr)
    {
        _sr = sr;
        ShapeComponentInContainer();
        RestPartOfControl();
        return _gsoControl!;
    }

    /// <summary>
    /// 묶음 컨트롤 안에 포함된 컨트롤을 위한 그리기 개체 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void ShapeComponentInContainer()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponent)
        {
            long id = _sr.ReadUInt4();
            _gsoControl = FactoryForControl.CreateGso((uint)id, null!);
            ForShapeComponent.Read(_gsoControl!, _sr);
        }
        else
        {
            throw new InvalidOperationException("Shape Component must come after CtrlHeader for gso control.");
        }
    }
}
