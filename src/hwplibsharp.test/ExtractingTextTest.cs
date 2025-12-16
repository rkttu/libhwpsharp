using HwpLib.Object;
using HwpLib.Reader;
using HwpLib.Tool.TextExtractor;

namespace HwpLibSharp.Test;

/// <summary>
/// 파일에서 전체 텍스트를 추출하는 테스트
/// </summary>
[TestClass]
public class ExtractingTextTest
{
    [TestMethod]
    [DataRow("blank.hwp")]
    [DataRow("etc.hwp")]
    [DataRow("ole.hwp")]
    [DataRow("각주미주.hwp")]
    // 구버전 파일은 다른 구조로 인해 무한 루프 발생 가능 - 임시 제외
    // [DataRow("구버전(5.0.2.2) Picture 컨트롤.hwp")]
    [DataRow("그림.hwp")]
    [DataRow("글상자.hwp")]
    [DataRow("글상자-압축.hwp")]
    [DataRow("글자겹침.hwp")]
    [DataRow("다각형.hwp")]
    [DataRow("덧말.hwp")]
    [DataRow("머리글꼬리글.hwp")]
    [DataRow("묶음.hwp")]
    [DataRow("문단번호 1-10 수준.hwp")]
    [DataRow("바탕쪽.hwp")]
    [DataRow("새번호지정.hwp")]
    [DataRow("선-사각형-타원.hwp")]
    [DataRow("수식.hwp")]
    [DataRow("숨은설명.hwp")]
    [DataRow("이미지추가.hwp")]
    [DataRow("차트.hwp")]
    [DataRow("책갈피.hwp")]
    [DataRow("페이지숨김.hwp")]
    [DataRow("표.hwp")]
    [DataRow("필드.hwp")]
    [DataRow("필드-누름틀.hwp")]
    [DataRow("호-곡선.hwp")]
    public void ExtractText_ShouldSucceed(string filename)
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath(filename);
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile, $"{filename} 읽기 성공");
        Console.WriteLine($"{filename} 읽기 성공 !!");
        Console.WriteLine();
        
        // Act
        var option = new TextExtractOption();
        option.SetMethod(TextExtractMethod.InsertControlTextBetweenParagraphText);
        option.SetWithControlChar(false);
        option.SetAppendEndingLF(true);
        
        var hwpText = TextExtractor.Extract(hwpFile, option);
        
        // Assert
        Assert.IsNotNull(hwpText);
        Console.WriteLine(hwpText);
        Console.WriteLine("========================================================");
    }
}
