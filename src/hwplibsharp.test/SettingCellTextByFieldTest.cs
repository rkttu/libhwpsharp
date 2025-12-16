using HwpLib.Object;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Reader;
using HwpLib.Tool.ObjectFinder;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 필드 이름으로 표의 셀을 찾아서 텍스트를 설정하는 테스트
/// </summary>
[TestClass]
public class SettingCellTextByFieldTest
{
    [TestMethod]
    public void SetCellTextByField_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("setting-celltext-by-field.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        SetCellTextByField(hwpFile, "필드명1", "ABCD");
        SetCellTextByField(hwpFile, "필드명2", "가나다라");
        SetCellTextByField(hwpFile, "필드명3", "1234");
        
        var writePath = TestHelper.GetResultPath("result-setting-celltext-by-field.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "필드로 셀 텍스트 설정 성공");
    }

    private static void SetCellTextByField(HWPFile hwpFile, string fieldName, string fieldText)
    {
        var cellList = CellFinder.FindAll(hwpFile, fieldName);
        foreach (var c in cellList)
        {
            Paragraph? firstPara = c.ParagraphList.GetParagraph(0);
            if (firstPara == null) continue;
            
            var paraText = firstPara.Text;
            if (paraText == null)
            {
                firstPara.CreateText();
                paraText = firstPara.Text;
            }

            paraText?.AddString(fieldText);
        }
    }
}
