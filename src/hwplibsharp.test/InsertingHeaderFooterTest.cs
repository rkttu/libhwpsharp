using HwpLib.Object;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader.Header;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Tool.BlankFileMaker;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 머리글/꼬리글 삽입 테스트
/// </summary>
[TestClass]
public class InsertingHeaderFooterTest
{
    [TestMethod]
    public void InsertHeaderFooter_ShouldSucceed()
    {
        // Arrange
        var hwpFile = BlankFileMaker.Make();
        Assert.IsNotNull(hwpFile);
        
        // Act
        var firstPara = hwpFile.BodyText.SectionList[0].GetParagraph(0);
        InsertHeader(firstPara);
        InsertFooter(firstPara);
        
        var writePath = TestHelper.GetResultPath("result-inserting-headerfooter.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "머리글/꼬리글 삽입 성공");
    }

    private static void InsertHeader(Paragraph? para)
    {
        if (para?.Text == null) return;
        
        para.Text.AddExtendCharForHeader();

        var header = (ControlHeader?)para.AddNewControl(ControlType.Header);
        if (header == null) return;
        
        header.Header.CreateIndex = 1;
        header.Header.ApplyPage = HeaderFooterApplyPage.BothPage;
        header.ListHeader.ParaCount = 1;
        header.ListHeader.TextWidth = 42520;
        header.ListHeader.TextHeight = 4252;

        var paragraph = header.ParagraphList.AddNewParagraph();
        paragraph.Header.ParaShapeId = 1;
        paragraph.Header.StyleId = 1;
        paragraph.CreateText();
        paragraph.Text?.AddString("머리글 입니다.");
        paragraph.CreateCharShape();
        paragraph.CharShape?.AddParaCharShape(0, 2);
    }

    private static void InsertFooter(Paragraph? para)
    {
        if (para?.Text == null) return;
        
        para.Text.AddExtendCharForFooter();

        var footer = (ControlFooter?)para.AddNewControl(ControlType.Footer);
        if (footer == null) return;
        
        footer.Header.CreateIndex = 1;
        footer.Header.ApplyPage = HeaderFooterApplyPage.BothPage;
        footer.ListHeader.ParaCount = 1;
        footer.ListHeader.TextWidth = 42520;
        footer.ListHeader.TextHeight = 4252;

        var paragraph = footer.ParagraphList.AddNewParagraph();
        paragraph.Header.ParaShapeId = 1;
        paragraph.Header.StyleId = 1;
        paragraph.CreateText();
        paragraph.Text?.AddString("꼬리글 입니다.");
        paragraph.CreateCharShape();
        paragraph.CharShape?.AddParaCharShape(0, 2);
    }
}
