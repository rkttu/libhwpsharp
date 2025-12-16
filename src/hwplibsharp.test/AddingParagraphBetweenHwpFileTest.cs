using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Reader;
using HwpLib.Tool.ParagraphAdder;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 다른 파일에 문단을 복사하여 추가하는 테스트
/// </summary>
[TestClass]
public class AddingParagraphBetweenHwpFileTest
{
    [TestMethod]
    public void AddParagraphBetweenFiles_ShouldSucceed()
    {
        // Arrange
        var sourceFilePath = TestHelper.GetSamplePath("source.hwp");
        var targetFilePath = TestHelper.GetSamplePath("target.hwp");
        
        var sourceHwpFile = HWPReader.FromFile(sourceFilePath);
        var targetHwpFile = HWPReader.FromFile(targetFilePath);
        
        Assert.IsNotNull(sourceHwpFile);
        Assert.IsNotNull(targetHwpFile);
        
        // Act
        IParagraphList targetFirstSection = targetHwpFile.BodyText.SectionList[0];
        
        // 첫 번째 문단 추가
        {
            Paragraph sourceParagraph = sourceHwpFile.BodyText.SectionList[0].GetParagraph(0);
            
            var paraAdder = new ParagraphAdder(targetHwpFile, targetFirstSection);
            paraAdder.Add(sourceHwpFile, sourceParagraph);
        }
        
        // 두 번째 문단 추가
        {
            Paragraph sourceParagraph = sourceHwpFile.BodyText.SectionList[0].GetParagraph(1);
            
            var paraAdder = new ParagraphAdder(targetHwpFile, targetFirstSection);
            paraAdder.Add(sourceHwpFile, sourceParagraph);
        }
        
        // 여섯 번째 문단 추가
        {
            Paragraph sourceParagraph = sourceHwpFile.BodyText.SectionList[0].GetParagraph(5);
            
            var paraAdder = new ParagraphAdder(targetHwpFile, targetFirstSection);
            paraAdder.Add(sourceHwpFile, sourceParagraph);
        }
        
        var writePath = TestHelper.GetResultPath("result-adding-paragraph.hwp");
        HWPWriter.ToFile(targetHwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "문단 추가 성공");
    }
}
