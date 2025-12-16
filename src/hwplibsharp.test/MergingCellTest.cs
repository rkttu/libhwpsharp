using HwpLib.Object.BodyText.Control;
using HwpLib.Reader;
using HwpLib.Tool;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 표의 셀을 병합하는 테스트
/// </summary>
[TestClass]
public class MergingCellTest
{
    [TestMethod]
    public void MergeCell_ShouldSucceed()
    {
        var filePath = TestHelper.GetSamplePath("merging-cell.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        if (hwpFile != null)
        {
            var control = hwpFile.BodyText.SectionList[0].GetParagraph(0).ControlList[2];
            if (control.Type == ControlType.Table)
            {
                var table = (ControlTable)control;
                TableCellMerger.MergeCell(table, 2, 2, 4, 3);
            }
            
            var writePath = TestHelper.GetResultPath("result-merging-cell.hwp");
            HWPWriter.ToFile(hwpFile, writePath);
        }
    }
}
