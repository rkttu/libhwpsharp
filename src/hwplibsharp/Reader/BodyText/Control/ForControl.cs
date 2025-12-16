using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Reader.BodyText.Paragraph;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControl
{
    /// <summary>
    /// 컨트롤의 종류에 따라 컨트롤을 읽는다.
    /// </summary>
    /// <param name="c">컨트롤 객체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        if (ControlTypeExtensions.IsField(c.Type.GetCtrlId()))
        {
            Field(c, sr);
            return;
        }

        switch (c.Type)
        {
            case ControlType.Table:
                Table(c, sr);
                break;
            case ControlType.Equation:
                Equation(c, sr);
                break;
            case ControlType.SectionDefine:
                SectionDefine(c, sr);
                break;
            case ControlType.ColumnDefine:
                ColumnDefine(c, sr);
                break;
            case ControlType.Header:
                Header(c, sr);
                break;
            case ControlType.Footer:
                Footer(c, sr);
                break;
            case ControlType.Footnote:
                Footnote(c, sr);
                break;
            case ControlType.Endnote:
                Endnote(c, sr);
                break;
            case ControlType.AutoNumber:
                AutoNumber(c, sr);
                break;
            case ControlType.NewNumber:
                NewNumber(c, sr);
                break;
            case ControlType.PageHide:
                PageHide(c, sr);
                break;
            case ControlType.PageOddEvenAdjust:
                PageOddEvenAdjust(c, sr);
                break;
            case ControlType.PageNumberPosition:
                PageNumberPosition(c, sr);
                break;
            case ControlType.IndexMark:
                IndexMark(c, sr);
                break;
            case ControlType.Bookmark:
                Bookmark(c, sr);
                break;
            case ControlType.OverlappingLetter:
                OverlappingLetter(c, sr);
                break;
            case ControlType.AdditionalText:
                AdditionalText(c, sr);
                break;
            case ControlType.HiddenComment:
                HiddenComment(c, sr);
                break;
            default:
                // 알 수 없는 컨트롤은 건너뛴다
                sr.SkipToEndRecord();
                break;
        }
    }

    private static void Field(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlField.ReadCtrlHeader((ControlField)c, sr);
    }

    private static void Table(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fct = new ForControlTable();
        fct.Read((ControlTable)c, sr);
    }

    private static void Equation(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fce = new ForControlEquation();
        fce.Read((ControlEquation)c, sr);
    }

    private static void SectionDefine(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fcsd = new ForControlSectionDefine();
        fcsd.Read((ControlSectionDefine)c, sr);
    }

    private static void ColumnDefine(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlColumnDefine.Read((ControlColumnDefine)c, sr);
    }

    private static void Header(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fch = new ForControlHeader();
        fch.Read((ControlHeader)c, sr);
    }

    private static void Footer(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fcf = new ForControlFooter();
        fcf.Read((ControlFooter)c, sr);
    }

    private static void Footnote(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fcfn = new ForControlFootnote();
        fcfn.Read((ControlFootnote)c, sr);
    }

    private static void Endnote(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fcen = new ForControlEndnote();
        fcen.Read((ControlEndnote)c, sr);
    }

    private static void AutoNumber(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlAutoNumber.Read((ControlAutoNumber)c, sr);
    }

    private static void NewNumber(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlNewNumber.Read((ControlNewNumber)c, sr);
    }

    private static void PageHide(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlPageHide.Read((ControlPageHide)c, sr);
    }

    private static void PageOddEvenAdjust(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlPageOddEvenAdjust.Read((ControlPageOddEvenAdjust)c, sr);
    }

    private static void PageNumberPosition(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlPageNumberPosition.Read((ControlPageNumberPosition)c, sr);
    }

    private static void IndexMark(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlIndexMark.Read((ControlIndexMark)c, sr);
    }

    private static void Bookmark(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlBookmark.Read((ControlBookmark)c, sr);
    }

    private static void OverlappingLetter(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlOverlappingLetter.Read((ControlOverlappingLetter)c, sr);
    }

    private static void AdditionalText(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        ForControlAdditionalText.Read((ControlAdditionalText)c, sr);
    }

    private static void HiddenComment(Object.BodyText.Control.Control c, CompoundStreamReader sr)
    {
        var fchc = new ForControlHiddenComment();
        fchc.Read((ControlHiddenComment)c, sr);
    }
}
