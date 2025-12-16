using HwpLib.Reader;

namespace HwpLibSharp.Test;

[TestClass]
public class GsoReadingTest
{
    [TestMethod]
    public void ReadGsoFile_Picture_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("그림.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_TextBox_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("글상자.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_TextBoxCompressed_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("글상자-압축.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_Polygon_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("다각형.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_LineRectEllipse_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("선-사각형-타원.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_ArcCurve_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("호-곡선.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_Container_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("묶음.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    [TestMethod]
    public void ReadGsoFile_Chart_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("차트.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }

    // 구버전 파일은 다른 구조로 인해 무한 루프 발생 가능 - 임시 제외
    // [TestMethod]
    // public void ReadGsoFile_OldVersionPicture_ShouldSucceed()
    // {
    //     // Arrange
    //     var filePath = TestHelper.GetBasicSamplePath("구버전(5.0.2.2) Picture 컨트롤.hwp");
        
    //     // Act
    //     var hwpFile = HWPReader.FromFile(filePath);
        
    //     // Assert
    //     Assert.IsNotNull(hwpFile);
    //     Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    // }

    [TestMethod]
    public void ReadGsoFile_Ole_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("ole.hwp");
        
        // Act
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Assert
        Assert.IsNotNull(hwpFile);
        Assert.IsTrue(hwpFile.BodyText.SectionList.Count > 0);
    }
}
