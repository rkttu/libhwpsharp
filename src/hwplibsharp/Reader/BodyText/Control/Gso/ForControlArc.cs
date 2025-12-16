using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Arc;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 호 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlArc
{
    /// <summary>
    /// 호 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="arc">호 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlArc arc, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            arc.CreateTextBox();
            ForTextBox.Read(arc.TextBox!, sr);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentArc)
        {
            ShapeComponentArc(arc.ShapeComponentArc, sr);
        }
    }

    /// <summary>
    /// 호 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="sca">호 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentArc(ShapeComponentArc sca, CompoundStreamReader sr)
    {
        sca.ArcType = ArcTypeExtensions.FromValue(sr.ReadUInt1());
        sca.CenterX = sr.ReadSInt4();
        sca.CenterY = sr.ReadSInt4();
        sca.Axis1X = sr.ReadSInt4();
        sca.Axis1Y = sr.ReadSInt4();
        sca.Axis2X = sr.ReadSInt4();
        sca.Axis2Y = sr.ReadSInt4();
    }
}
