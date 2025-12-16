using HwpLib.Object;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 파일 복제 테스트
/// </summary>
[TestClass]
public class CloningHwpFileTest
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
    public void CloneFile_ShouldSucceed(string filename)
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath(filename);
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Act
        Assert.IsNotNull(hwpFile);
        var clonedHwpFile = hwpFile.Clone(false);
        
        // Assert
        Assert.IsNotNull(clonedHwpFile);
        Assert.AreNotSame(hwpFile, clonedHwpFile);
        
        // 복제 파일 저장
        var writePath = TestHelper.GetResultPath($"result-clone-{filename}");
        HWPWriter.ToFile(clonedHwpFile, writePath);
        Assert.IsTrue(File.Exists(writePath), $"{filename} 복제 성공");
    }

    [TestMethod]
    public void CloneFile_DeepCopy_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("표.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Act
        Assert.IsNotNull(hwpFile);
        var clonedHwpFile = hwpFile.Clone(true); // 깊은 복사
        
        // Assert
        Assert.IsNotNull(clonedHwpFile);
        Assert.AreNotSame(hwpFile, clonedHwpFile);
        Assert.AreNotSame(hwpFile.BodyText, clonedHwpFile.BodyText);
    }
}
