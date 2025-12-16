using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 구버전(5.0.2.2) Picture 컨트롤 파일 테스트
/// 
/// 이 테스트 클래스는 구버전 Picture 컨트롤 파일에 대한 집중 테스트를 위해 분리되었습니다.
/// 원본 테스트 클래스들에서는 다른 구조로 인해 무한 루프 발생 가능성으로 인해 주석 처리되어 있습니다.
/// </summary>
[TestClass]
public class OldVersionPictureControlTest
{
    private const string OldVersionPictureFileName = "구버전(5.0.2.2) Picture 컨트롤.hwp";

    /// <summary>
    /// 구버전 Picture 컨트롤 파일 읽기 테스트
    /// 
    /// 원본: ReadingHwpFromFileTest.ReadBasicFile_ShouldSucceed
    /// </summary>
    [TestMethod]
    [Timeout(10000)] // 10초 타임아웃
    public void ReadOldVersionPictureFile_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath(OldVersionPictureFileName);

        // Act
        var hwpFile = HWPReader.FromFile(filePath);

        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0, $"{OldVersionPictureFileName} 읽기 성공");
    }

    /// <summary>
    /// 구버전 Picture 컨트롤 파일 다시 쓰기 테스트
    /// 
    /// 원본: RewritingHwpFileTest.RewriteFile_ShouldSucceed
    /// </summary>
    [TestMethod]
    [Timeout(10000)] // 10초 타임아웃
    public void RewriteOldVersionPictureFile_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath(OldVersionPictureFileName);
        var hwpFile = HWPReader.FromFile(filePath);

        // Act
        Assert.IsNotNull(hwpFile);
        var writePath = TestHelper.GetResultPath($"result-rewrite-{OldVersionPictureFileName}");
        HWPWriter.ToFile(hwpFile, writePath);

        // Assert
        Assert.IsTrue(File.Exists(writePath), $"{OldVersionPictureFileName} 다시 쓰기 성공");

        // 다시 읽어서 검증
        var reloadedFile = HWPReader.FromFile(writePath);
        Assert.IsNotNull(reloadedFile);
        Assert.IsTrue(reloadedFile.BodyText.SectionList.Count > 0);
    }

    /// <summary>
    /// 구버전 Picture 컨트롤 GSO 읽기 테스트
    /// 
    /// 원본: GsoReadingTest.ReadGsoFile_OldVersionPicture_ShouldSucceed
    /// </summary>
    [TestMethod]
    [Timeout(10000)] // 10초 타임아웃
    public void ReadGsoFile_OldVersionPicture_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath(OldVersionPictureFileName);

        // Act
        var hwpFile = HWPReader.FromFile(filePath);

        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }
}
