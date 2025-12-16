using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Curve;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 곡선 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlCurve
{
    /// <summary>
    /// 곡선 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="curve">곡선 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlCurve curve, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            curve.CreateTextBox();
            ForTextBox.Read(curve.TextBox!, sr);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentCurve)
        {
            ShapeComponentCurve(curve.ShapeComponentCurve, sr);
        }
    }

    /// <summary>
    /// 곡선 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scc">곡선 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentCurve(ShapeComponentCurve scc, CompoundStreamReader sr)
    {
        int positionCount = sr.ReadSInt4();
        for (int index = 0; index < positionCount; index++)
        {
            var p = scc.AddNewPosition();
            p.X = (uint)sr.ReadSInt4();
            p.Y = (uint)sr.ReadSInt4();
        }
        for (int index = 0; index < positionCount - 1; index++)
        {
            var cst = CurveSegmentTypeExtensions.FromValue(sr.ReadUInt1());
            scc.AddCurveSegmentType(cst);
        }
        sr.Skip(4);
    }
}
