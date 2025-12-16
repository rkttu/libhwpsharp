using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Tool.BlankFileMaker;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 하이퍼링크 삽입 테스트
/// </summary>
[TestClass]
public class InsertingHyperLinkTest
{
    [TestMethod]
    public void InsertHyperLink_ShouldSucceed()
    {
        // Arrange
        var hwpFile = BlankFileMaker.Make();
        Assert.IsNotNull(hwpFile);
        
        // Act
        Section s = hwpFile.BodyText.SectionList[0];
        int count = s.ParagraphCount;
        for (int index = 0; index < count; index++)
        {
            InsertHyperLink(hwpFile.BodyText.SectionList[0].GetParagraph(index));
        }
        
        var writePath = TestHelper.GetResultPath("result-inserting-hyperlink.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "하이퍼링크 삽입 성공");
    }

    private static void InsertHyperLink(Paragraph? paragraph)
    {
        if (paragraph?.Text == null) return;
        
        paragraph.Text.AddString("이것은 ");
        paragraph.Text.AddExtendCharForHyperlinkStart();
        paragraph.Text.AddString("다음 사이트");
        paragraph.Text.AddExtendCharForHyperlinkEnd();
        paragraph.Text.AddString("로 가는 링크입니다.");

        var field = (ControlField?)paragraph.AddNewControl(ControlType.FIELD_HYPERLINK.GetCtrlId());
        var header = field?.GetHeader();
        header?.Command.FromUTF16LEString("https\\://www.dogfoot.kr/aaa.jsp\\?aaa=bb&ccc=dd" + ";1;0;0;");
    }
}
