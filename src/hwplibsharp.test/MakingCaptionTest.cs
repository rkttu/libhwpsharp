using HwpLib.Object;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.Caption;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.CharShape;
using HwpLib.Object.BodyText.Paragraph.Header;
using HwpLib.Object.BodyText.Paragraph.LineSeg;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 캡션 만들기 테스트
/// </summary>
[TestClass]
public class MakingCaptionTest
{
    /// <summary>
    /// GSO 컨트롤(그리기 객체) Reader가 구현되어 테스트를 실행할 수 있습니다.
    /// </summary>
    [TestMethod]
    public void MakeCaption_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("making_caption.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        var para = hwpFile.BodyText.SectionList[0].GetParagraph(0);
        Assert.IsNotNull(para);
        Assert.IsNotNull(para.ControlList);
        
        var ctrlRect = para.ControlList[2] as ControlRectangle;
        Assert.IsNotNull(ctrlRect);
        
        var caption = ctrlRect.Caption;
        if (caption == null)
        {
            ctrlRect.CreateCaption();
            caption = ctrlRect.Caption;
        }
        Assert.IsNotNull(caption);
        
        SetListHeader(caption.ListHeader);
        var newPara = caption.ParagraphList.AddNewParagraph();
        SetPara(newPara, "이것은 caption 입니다");
        
        var writePath = TestHelper.GetResultPath("result_making_caption.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "캡션 만들기 성공");
    }

    private static void SetListHeader(ListHeaderForCaption listHeader)
    {
        listHeader.CaptionProperty.Direction = CaptionDirection.Bottom;
        listHeader.SpaceBetweenCaptionAndFrame = 850;
    }

    private static void SetPara(Paragraph? emptyPara, string text)
    {
        if (emptyPara == null) return;
        
        SetParaHeader(emptyPara);
        SetParaText(emptyPara, text);
        SetParaCharShape(emptyPara);
    }

    private static void SetParaHeader(Paragraph p)
    {
        var ph = p.Header;
        ph.LastInList = true;
        ph.ParaShapeId = 1;
        ph.StyleId = 1;
        ph.DivideSort.IsDivideSection = false;
        ph.DivideSort.IsDivideMultiColumn = false;
        ph.DivideSort.IsDividePage = false;
        ph.DivideSort.IsDivideColumn = false;
        ph.CharShapeCount = 1;
        ph.RangeTagCount = 0;
        ph.LineAlignCount = 1;
        ph.InstanceID = 0;
        ph.IsMergedByTrack = 0;
    }

    private static void SetParaText(Paragraph p, string text)
    {
        p.CreateText();
        var pt = p.Text;
        pt?.AddString(text);
    }

    private static void SetParaCharShape(Paragraph p)
    {
        p.CreateCharShape();

        var pcs = p.CharShape;
        pcs?.AddParaCharShape(0, 1);
    }

    private static void SetParaLineSeg(Paragraph p)
    {
        p.CreateLineSeg();

        var pls = p.LineSeg;
        var lsi = pls?.AddNewLineSegItem();
        if (lsi == null) return;

        lsi.TextStartPosition = 0;
        lsi.LineVerticalPosition = 0;
        lsi.LineHeight = TestHelper.PtToLineHeight(10.0);
        lsi.TextPartHeight = TestHelper.PtToLineHeight(10.0);
        lsi.DistanceBaseLineToLineVerticalPosition = TestHelper.PtToLineHeight(10.0 * 0.85);
        lsi.LineSpace = TestHelper.PtToLineHeight(6.0);
        lsi.StartPositionFromColumn = 0;
        lsi.SegmentWidth = (int)TestHelper.MmToHwp(50.0);
        lsi.Tag.IsFirstSegmentAtLine = true;
        lsi.Tag.IsLastSegmentAtLine = true;
    }
}
