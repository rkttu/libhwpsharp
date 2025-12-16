using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 표의 행을 삭제하는 테스트
/// </summary>
[TestClass]
public class RemovingTableRowTest
{
    [TestMethod]
    public void RemoveTableRow_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("removing-row.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        var table = FindTable(hwpFile);
        Assert.IsNotNull(table);
        
        RemoveSecondRowObject(table);
        AdjustTableRowCount(table);
        RemoveCellCountOfRow(table);
        AdjustCellRowIndex(table);
        
        var writePath = TestHelper.GetResultPath("result-removing-row.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "테이블 행 삭제 성공");
    }

    private static void RemoveSecondRowObject(ControlTable table)
    {
        table.RemoveRow(1);
    }

    private static void AdjustTableRowCount(ControlTable table)
    {
        table.Table.RowCount = table.RowList.Count;
    }

    private static void RemoveCellCountOfRow(ControlTable table)
    {
        table.Table.RemoveCellCountOfRow(1);
    }

    private static void AdjustCellRowIndex(ControlTable table)
    {
        int rowCount = table.RowList.Count;
        for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
        {
            if (rowIndex > 0)
            {
                var row = table.RowList[rowIndex];
                foreach (var cell in row.CellList)
                {
                    cell.ListHeader.RowIndex = cell.ListHeader.RowIndex - 1;
                }
            }
        }
    }

    private static ControlTable? FindTable(HWPFile hwpFile)
    {
        Section s = hwpFile.BodyText.SectionList[0];
        Paragraph firstParagraph = s.GetParagraph(0);
        if (firstParagraph.ControlList[2].Type == ControlType.Table)
        {
            return (ControlTable)firstParagraph.ControlList[2];
        }
        return null;
    }
}
