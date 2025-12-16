using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 선 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlLine
{
    /// <summary>
    /// 선 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="line">선 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlLine line, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentLine)
        {
            ShapeComponentLine(line.ShapeComponentLine, sr);
        }
    }

    /// <summary>
    /// 선 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scl">선 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentLine(ShapeComponentLine scl, CompoundStreamReader sr)
    {
        scl.StartX = sr.ReadSInt4();
        scl.StartY = sr.ReadSInt4();
        scl.EndX = sr.ReadSInt4();
        scl.EndY = sr.ReadSInt4();
        int temp = sr.ReadSInt4();
        scl.IsStartedRightOrBottom = (temp == 1);
    }
}
