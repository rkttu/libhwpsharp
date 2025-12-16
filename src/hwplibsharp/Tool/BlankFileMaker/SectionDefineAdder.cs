using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.SectionDefine;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.DocInfo.BorderFill;

namespace HwpLib.Tool.BlankFileMaker;

/// <summary>
/// 빈 HWP 파일 생성 시 구역 정의 컨트롤을 추가하는 클래스
/// </summary>
public static class SectionDefineAdder
{
    /// <summary>
    /// 구역 정의 컨트롤을 추가한다.
    /// </summary>
    /// <param name="paragraph">문단</param>
    public static void Add(Paragraph paragraph)
    {
        var sectionDefine = paragraph.AddNewControl(ControlType.SectionDefine) as ControlSectionDefine;
        if (sectionDefine == null) return;
        
        Header(sectionDefine.Header);
        PageDef(sectionDefine.PageDef);
        FootNoteShape(sectionDefine.FootNoteShape);
        EndNoteShape(sectionDefine.EndNoteShape);
        PageBorderFill(sectionDefine.BothPageBorderFill);
        PageBorderFill(sectionDefine.EvenPageBorderFill);
        PageBorderFill(sectionDefine.OddPageBorderFill);
    }

    private static void Header(CtrlHeaderSectionDefine header)
    {
        header.Property.Value = 0;
        header.ColumnGap = 1134;
        header.VerticalLineAlign = 0;
        header.HorizontalLineAlign = 0;
        header.DefaultTabGap = 8000;
        header.NumberParaShapeId = 1;
        header.PageStartNumber = 0;
        header.ImageStartNumber = 0;
        header.TableStartNumber = 0;
        header.EquationStartNumber = 0;
        header.DefaultLanguage = 0;
    }

    private static void PageDef(PageDef pageDef)
    {
        pageDef.PaperWidth = 59528;
        pageDef.PaperHeight = 84188;
        pageDef.LeftMargin = 8504;
        pageDef.RightMargin = 8504;
        pageDef.TopMargin = 5668;
        pageDef.BottomMargin = 4252;
        pageDef.HeaderMargin = 4252;
        pageDef.FooterMargin = 4252;
        pageDef.GutterMargin = 0;
        pageDef.Property.Value = 0;
    }

    private static void FootNoteShape(FootEndNoteShape footNoteShape)
    {
        footNoteShape.Property.Value = 0;
        footNoteShape.UserSymbol.FromUTF16LEString("\u0000");
        footNoteShape.BeforeDecorativeLetter.FromUTF16LEString("\u0000");
        footNoteShape.AfterDecorativeLetter.FromUTF16LEString(")");
        footNoteShape.StartNumber = 1;
        footNoteShape.DivideLineLength = -1;
        footNoteShape.DivideLineTopMargin = 850;
        footNoteShape.DivideLineBottomMargin = 567;
        footNoteShape.BetweenNotesMargin = 283;
        footNoteShape.DivideLine.Type = BorderType.Solid;
        footNoteShape.DivideLine.Thickness = BorderThickness.MM0_12;
        footNoteShape.DivideLine.Color.Value = 0;
        footNoteShape.Unknown = 0;
    }

    private static void EndNoteShape(FootEndNoteShape endNoteShape)
    {
        endNoteShape.Property.Value = 0;
        endNoteShape.UserSymbol.FromUTF16LEString("\u0000");
        endNoteShape.BeforeDecorativeLetter.FromUTF16LEString("\u0000");
        endNoteShape.AfterDecorativeLetter.FromUTF16LEString(")");
        endNoteShape.StartNumber = 1;
        endNoteShape.DivideLineLength = 14692344;
        endNoteShape.DivideLineTopMargin = 850;
        endNoteShape.DivideLineBottomMargin = 567;
        endNoteShape.BetweenNotesMargin = 0;
        endNoteShape.DivideLine.Type = BorderType.Solid;
        endNoteShape.DivideLine.Thickness = BorderThickness.MM0_12;
        endNoteShape.DivideLine.Color.Value = 0;
        endNoteShape.Unknown = 0;
    }

    private static void PageBorderFill(PageBorderFill pageBorderFillInfo)
    {
        pageBorderFillInfo.Property.Value = 1;
        pageBorderFillInfo.LeftGap = 1417;
        pageBorderFillInfo.RightGap = 1417;
        pageBorderFillInfo.TopGap = 1417;
        pageBorderFillInfo.BottomGap = 1417;
        pageBorderFillInfo.BorderFillId = 1;
    }
}
