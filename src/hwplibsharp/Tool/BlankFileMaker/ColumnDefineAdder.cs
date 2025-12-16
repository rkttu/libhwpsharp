using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.DocInfo.BorderFill;

namespace HwpLib.Tool.BlankFileMaker;

/// <summary>
/// 빈 HWP 파일 생성 시 단 정의 컨트롤을 추가하는 클래스
/// </summary>
public static class ColumnDefineAdder
{
    /// <summary>
    /// 단 정의 컨트롤을 추가한다.
    /// </summary>
    /// <param name="paragraph">문단</param>
    public static void Add(Paragraph paragraph)
    {
        var columnDefine = paragraph.AddNewControl(ControlType.ColumnDefine) as ControlColumnDefine;
        var header = columnDefine?.GetHeader();
        if (header != null)
        {
            Header(header);
        }
    }

    private static void Header(CtrlHeaderColumnDefine header)
    {
        header.Property.Value = 4100;
        header.GapBetweenColumn = 0;
        header.Property2 = 0;
        header.DivideLine.Type = BorderType.None;
        header.DivideLine.Thickness = BorderThickness.MM0_1;
        header.DivideLine.Color.Value = 0;
    }
}
