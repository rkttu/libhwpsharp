using HwpLib.Object;
using HwpLib.Reader;

namespace HwpLibSharp.Test;

/// <summary>
/// URL로부터 파일을 읽는 테스트
/// </summary>
[TestClass]
public class ReadingHwpFromUrlTest
{
    [TestMethod]
    public void ReadHwpFromUrl_ShouldSucceed()
    {
        // Arrange
        var url = "https://raw.githubusercontent.com/rkttu/libhwpsharp/refs/heads/main/sample_hwp/big_file.hwp";
        
        // Act
        HWPFile hwpFile;
        try
        {
            hwpFile = HWPReader.FromUrl(url);
        }
        catch (HttpRequestException ex)
        {
            // 네트워크 접근 불가 시 테스트 스킵
            Assert.Inconclusive($"네트워크 접근 불가로 테스트 스킵: {ex.Message}");
            return;
        }
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0, $"{url} 읽기 성공 !!");
        Console.WriteLine($"{url} 읽기 성공 !!");
    }

    [TestMethod]
    public void ReadHwpFromUrl_WithLocalFile_ShouldSucceed()
    {
        // Arrange - 로컬 파일을 Stream으로 읽어서 FromStream 테스트
        var filePath = TestHelper.GetBasicSamplePath("blank.hwp");
        
        // Act - FromStream이 정상 동작하는지 확인 (FromUrl의 핵심 로직)
        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var memoryStream = new MemoryStream();
        stream.CopyTo(memoryStream);
        memoryStream.Position = 0;
        var hwpFile = HWPReader.FromStream(memoryStream);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Console.WriteLine("FromStream (FromUrl의 핵심 로직) 테스트 성공!");
    }
}
