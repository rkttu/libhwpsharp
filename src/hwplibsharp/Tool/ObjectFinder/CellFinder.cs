using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.TextBox;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;

namespace HwpLib.Tool.ObjectFinder;

/// <summary>
/// 필드명이 일치하는 셀을 찾는 기능을 하는 객체
/// </summary>
public class CellFinder
{
    /// <summary>
    /// 결과가 저장되는 셀 리스트
    /// </summary>
    private readonly List<Cell> _cellList;

    /// <summary>
    /// 찾고자하는 필드명
    /// </summary>
    private readonly string _fieldName;

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="fieldName">필드명</param>
    private CellFinder(string fieldName)
    {
        _cellList = new List<Cell>();
        _fieldName = fieldName;
    }

    /// <summary>
    /// 표 안에서 필드명이 일치하는 셀들을 반환한다.
    /// </summary>
    /// <param name="table">표 컨트롤</param>
    /// <param name="fieldName">필드명</param>
    /// <returns>셀 리스트</returns>
    public static List<Cell> FindAll(ControlTable table, string fieldName)
    {
        var finder = new CellFinder(fieldName);
        finder.Find(table);
        return finder._cellList;
    }

    /// <summary>
    /// 파일 내에서 필드명이 일치하는 셀들을 반환한다.
    /// </summary>
    /// <param name="hwpFile">한글 파일 객체</param>
    /// <param name="fieldName">필드명</param>
    /// <returns>셀 리스트</returns>
    public static List<Cell> FindAll(HWPFile hwpFile, string fieldName)
    {
        var finder = new CellFinder(fieldName);
        foreach (var s in hwpFile.BodyText.SectionList)
        {
            finder.ForParagraphList(s);
        }
        return finder._cellList;
    }

    /// <summary>
    /// 표 안에서 필드명이 일치하는 셀들을 찾는다.
    /// </summary>
    /// <param name="table">표 컨트롤</param>
    public void Find(ControlTable table)
    {
        foreach (var row in table.RowList)
        {
            foreach (var cell in row.CellList)
            {
                if (MatchFieldName(cell, _fieldName))
                {
                    _cellList.Add(cell);
                }
                ForParagraphList(cell.ParagraphList);
            }
        }
    }

    /// <summary>
    /// 셀의 필드명이 원하는 필드명과 일치하는지 비교한다.
    /// </summary>
    /// <param name="cell">셀</param>
    /// <param name="fieldName">필드명</param>
    /// <returns>셀의 필드명이 원하는 필드명과 일치하는지 여부</returns>
    private bool MatchFieldName(Cell cell, string fieldName)
    {
        return cell != null
               && cell.ListHeader.FieldName != null
               && cell.ListHeader.FieldName.Equals(fieldName);
    }

    /// <summary>
    /// 문단리스트에서 필드명이 일치하는 셀들을 찾는다.
    /// </summary>
    /// <param name="paragraphs">문단 리스트</param>
    private void ForParagraphList(IParagraphList paragraphs)
    {
        foreach (var p in paragraphs)
        {
            if (p.ControlList != null)
            {
                foreach (var c in p.ControlList)
                {
                    ForControl(c);
                }
            }
        }
    }

    /// <summary>
    /// 다양한 종류의 컨트롤 내에서 필드명이 일치하는 셀들을 찾는다.
    /// </summary>
    /// <param name="c">컨트롤 객체</param>
    private void ForControl(Control c)
    {
        switch (c.Type)
        {
            case ControlType.Table:
                Find((ControlTable)c);
                break;
            case ControlType.Gso:
                ForGso((GsoControl)c);
                break;
            case ControlType.Header:
                {
                    var header = (ControlHeader)c;
                    ForParagraphList(header.ParagraphList);
                }
                break;
            case ControlType.Footer:
                {
                    var footer = (ControlFooter)c;
                    ForParagraphList(footer.ParagraphList);
                }
                break;
            case ControlType.Footnote:
                {
                    var footnote = (ControlFootnote)c;
                    ForParagraphList(footnote.ParagraphList);
                }
                break;
            case ControlType.Endnote:
                {
                    var endnote = (ControlEndnote)c;
                    ForParagraphList(endnote.ParagraphList);
                }
                break;
            case ControlType.HiddenComment:
                {
                    var comment = (ControlHiddenComment)c;
                    ForParagraphList(comment.ParagraphList);
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 다양한 그리기 객체 내에서 필드명이 일치하는 셀들을 찾는다.
    /// </summary>
    /// <param name="gso">그리기 객체</param>
    private void ForGso(GsoControl gso)
    {
        switch (gso.GsoType)
        {
            case GsoControlType.Rectangle:
                {
                    var rectangle = (ControlRectangle)gso;
                    ForTextBox(rectangle.TextBox);
                }
                break;
            case GsoControlType.Ellipse:
                {
                    var ellipse = (ControlEllipse)gso;
                    ForTextBox(ellipse.TextBox);
                }
                break;
            case GsoControlType.Arc:
                {
                    var arc = (ControlArc)gso;
                    ForTextBox(arc.TextBox);
                }
                break;
            case GsoControlType.Polygon:
                {
                    var polygon = (ControlPolygon)gso;
                    ForTextBox(polygon.TextBox);
                }
                break;
            case GsoControlType.Curve:
                {
                    var curve = (ControlCurve)gso;
                    ForTextBox(curve.TextBox);
                }
                break;
            case GsoControlType.Container:
                {
                    var container = (ControlContainer)gso;
                    foreach (var child in container.ChildControlList)
                    {
                        ForControl(child);
                    }
                }
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 그리기 객체의 텍스트 박스에서 필드명이 일치하는 셀들을 찾는다.
    /// </summary>
    /// <param name="textBox">그리기 객체의 텍스트 박스</param>
    private void ForTextBox(TextBox? textBox)
    {
        if (textBox == null)
        {
            return;
        }

        ForParagraphList(textBox.ParagraphList);
    }
}
