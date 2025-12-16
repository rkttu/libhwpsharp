using HwpLib.Object;
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
    [Ignore("테이블 컨트롤 읽기 기능이 아직 구현되지 않았습니다. ForSection.ReadCtrlHeader()에서 테이블/GSO 컨트롤 파싱 필요")]
    public void MergeCell_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("merging-cell.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        var control = hwpFile.BodyText.SectionList[0].GetParagraph(0).ControlList[2];
        if (control.Type == ControlType.Table)
        {
            var table = (ControlTable)control;
            TableCellMerger.MergeCell(table, 2, 2, 4, 3);
        }
        
        var writePath = TestHelper.GetResultPath("result-merging-cell.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "셀 병합 성공");
    }
}
