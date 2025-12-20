using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.TextBox;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;

namespace HwpLib.Tool.ObjectFinder;

/// <summary>
/// 원하는 컨트롤을 찾기 위한 객체
/// </summary>
public class ControlFinder
{
    /// <summary>
    /// 조건 필터
    /// </summary>
    private IControlFilter? _filter;

    /// <summary>
    /// 결과 리스트
    /// </summary>
    private List<Control>? _resultList;

    /// <summary>
    /// 현재 구역 객체
    /// </summary>
    private Section? _currentSection;

    /// <summary>
    /// 현재 문단 객체
    /// </summary>
    private Paragraph? _currentParagraph;

    /// <summary>
    /// 생성자
    /// </summary>
    private ControlFinder()
    {
    }

    /// <summary>
    /// 원하는 조건에 맞는 컨트롤을 찾는다.
    /// </summary>
    /// <param name="hwpFile">한글 파일객체</param>
    /// <param name="filter">조건 필터</param>
    /// <returns>원하는 조건에 맞는 컨트롤 리스트</returns>
    public static List<Control> Find(HWPFile hwpFile, IControlFilter filter)
    {
        var finder = new ControlFinder();
        return finder.Go(hwpFile, filter);
    }

    /// <summary>
    /// 한글 파일 객체에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="hwpFile">한글 파일 객체</param>
    /// <param name="filter">조건 필터</param>
    /// <returns>조건에 맞는 컨트롤 리스트</returns>
    private List<Control> Go(HWPFile hwpFile, IControlFilter filter)
    {
        _resultList = new List<Control>();
        _filter = filter;

        foreach (var s in hwpFile.BodyText.SectionList)
        {
            _currentSection = s;
            ForParagraphList(s);
        }

        return _resultList;
    }

    /// <summary>
    /// 문단 리스트에서 원하는 조건에 맞는 컨트롤를 찾는다
    /// </summary>
    /// <param name="paraList">문단 리스트</param>
    private void ForParagraphList(IParagraphList paraList)
    {
        foreach (var p in paraList)
        {
            _currentParagraph = p;
            ForParagraph(p);
        }
    }

    /// <summary>
    /// 문단에서 원하는 조건에 맞는 컨트롤를 찾는다
    /// </summary>
    /// <param name="p">문단</param>
    private void ForParagraph(Paragraph p)
    {
        if (p.ControlList == null)
        {
            return;
        }
        foreach (var c in p.ControlList)
        {
            if (_filter!.IsMatched(c, _currentParagraph!, _currentSection!))
            {
                _resultList!.Add(c);
            }
            ForParagraphInControl(c);
        }
    }

    /// <summary>
    /// 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="c">컨트롤</param>
    private void ForParagraphInControl(Control c)
    {
        switch (c.Type)
        {
            case ControlType.Table:
                ForTable((ControlTable)c);
                break;
            case ControlType.Gso:
                ForGso((GsoControl)c);
                break;
            case ControlType.Equation:
                break;
            case ControlType.SectionDefine:
                break;
            case ControlType.ColumnDefine:
                break;
            case ControlType.Header:
                ForHeader((ControlHeader)c);
                break;
            case ControlType.Footer:
                ForFooter((ControlFooter)c);
                break;
            case ControlType.Footnote:
                ForFootnote((ControlFootnote)c);
                break;
            case ControlType.Endnote:
                ForEndnote((ControlEndnote)c);
                break;
            case ControlType.AutoNumber:
                break;
            case ControlType.NewNumber:
                break;
            case ControlType.PageHide:
                break;
            case ControlType.PageOddEvenAdjust:
                break;
            case ControlType.PageNumberPosition:
                break;
            case ControlType.IndexMark:
                break;
            case ControlType.Bookmark:
                break;
            case ControlType.OverlappingLetter:
                break;
            case ControlType.AdditionalText:
                break;
            case ControlType.HiddenComment:
                ForHiddenComment((ControlHiddenComment)c);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 표 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="table">표 컨트롤</param>
    private void ForTable(ControlTable table)
    {
        foreach (var r in table.RowList)
        {
            foreach (var c in r.CellList)
            {
                ForParagraphList(c.ParagraphList);
            }
        }
    }

    /// <summary>
    /// 머리글 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="header">머리글 컨트롤</param>
    private void ForHeader(ControlHeader header)
    {
        ForParagraphList(header.ParagraphList);
    }

    /// <summary>
    /// 바닥글 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="footer">바닥글 컨트롤</param>
    private void ForFooter(ControlFooter footer)
    {
        ForParagraphList(footer.ParagraphList);
    }

    /// <summary>
    /// 각주 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="footnote">각주 컨트롤</param>
    private void ForFootnote(ControlFootnote footnote)
    {
        ForParagraphList(footnote.ParagraphList);
    }

    /// <summary>
    /// 미주 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="endnote">미주 컨트롤</param>
    private void ForEndnote(ControlEndnote endnote)
    {
        ForParagraphList(endnote.ParagraphList);
    }

    /// <summary>
    /// 숨은 설명 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="hiddenComment">숨은 설명 컨트롤</param>
    private void ForHiddenComment(ControlHiddenComment hiddenComment)
    {
        ForParagraphList(hiddenComment.ParagraphList);
    }

    /// <summary>
    /// 그리기 객체 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="gc">그리기 객체 컨트롤</param>
    private void ForGso(GsoControl gc)
    {
        switch (gc.GsoType)
        {
            case GsoControlType.Line:
                break;
            case GsoControlType.Rectangle:
                ForRectangle((ControlRectangle)gc);
                break;
            case GsoControlType.Ellipse:
                ForEllipse((ControlEllipse)gc);
                break;
            case GsoControlType.Arc:
                ForArc((ControlArc)gc);
                break;
            case GsoControlType.Polygon:
                ForPolygon((ControlPolygon)gc);
                break;
            case GsoControlType.Curve:
                ForCurve((ControlCurve)gc);
                break;
            case GsoControlType.Picture:
                break;
            case GsoControlType.OLE:
                break;
            case GsoControlType.Container:
                ForContainer((ControlContainer)gc);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 사각형 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="rectangle">사각형 컨트롤</param>
    private void ForRectangle(ControlRectangle rectangle)
    {
        if (rectangle.TextBox != null)
        {
            ForParagraphList(rectangle.TextBox.ParagraphList);
        }
    }

    /// <summary>
    /// 타원 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="ellipse">타원 컨트롤</param>
    private void ForEllipse(ControlEllipse ellipse)
    {
        if (ellipse.TextBox == null)
        {
            return;
        }
        ForParagraphList(ellipse.TextBox.ParagraphList);
    }

    /// <summary>
    /// 호 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="arc">호 컨트롤</param>
    private void ForArc(ControlArc arc)
    {
        if (arc.TextBox == null)
        {
            return;
        }
        ForParagraphList(arc.TextBox.ParagraphList);
    }

    /// <summary>
    /// 다각형 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="polygon">다각형 컨트롤</param>
    private void ForPolygon(ControlPolygon polygon)
    {
        if (polygon.TextBox == null)
        {
            return;
        }
        ForParagraphList(polygon.TextBox.ParagraphList);
    }

    /// <summary>
    /// 곡선 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="curve">곡선 컨트롤</param>
    private void ForCurve(ControlCurve curve)
    {
        if (curve.TextBox == null)
        {
            return;
        }
        ForParagraphList(curve.TextBox.ParagraphList);
    }

    /// <summary>
    /// 묶음 컨트롤 안에 문단에서 원하는 조건에 맞는 컨트롤를 찾는다.
    /// </summary>
    /// <param name="container">묶음 컨트롤</param>
    private void ForContainer(ControlContainer container)
    {
        foreach (var child in container.ChildControlList)
        {
            if (_filter!.IsMatched(child, _currentParagraph!, _currentSection!))
            {
                _resultList!.Add(child);
            }
            ForGso(child);
        }
    }
}
