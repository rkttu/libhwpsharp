using HwpLib.Object;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText;
using HwpLib.Reader;
using HwpLib.Tool.ObjectFinder;

namespace HwpLibSharp.Test;

[TestClass]
public class FindingControlTest
{
    private class MyControlFilter : IControlFilter
    {
        public bool IsMatched(Control control, Paragraph paragraph, Section section)
        {
            if (control.Type == ControlType.Table)
            {
                var table = (ControlTable)control;
                var firstRow = table.RowList[0];
                var firstCell = firstRow.CellList[0];
                
                var normalString = firstCell.ParagraphList.GetNormalString();
                if (normalString?.StartsWith("A") == true)
                {
                    return true;
                }
            }
            return false;
        }
    }

    [TestMethod]
    public void FindControl_WithFilter_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("finding-control.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Act
        Assert.IsNotNull(hwpFile);
        var myFilter = new MyControlFilter();
        var result = ControlFinder.Find(hwpFile, myFilter);
        
        // Assert
        Assert.IsNotNull(result);
        Console.WriteLine($"found {result.Count} tables.");
        Assert.IsTrue(result.Count > 0, "테이블을 찾았습니다.");
    }

    [TestMethod]
    public void FindAllTables_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetBasicSamplePath("표.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        // Act
        Assert.IsNotNull(hwpFile);
        var tableFilter = new TableControlFilter();
        var result = ControlFinder.Find(hwpFile, tableFilter);
        
        // Assert
        Assert.IsNotNull(result);
        Console.WriteLine($"found {result.Count} tables.");
    }

    private class TableControlFilter : IControlFilter
    {
        public bool IsMatched(Control control, Paragraph paragraph, Section section)
        {
            return control.Type == ControlType.Table;
        }
    }
}
