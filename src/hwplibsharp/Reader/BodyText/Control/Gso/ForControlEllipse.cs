using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 타원 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlEllipse
{
    /// <summary>
    /// 타원 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="ellipse">타원 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlEllipse ellipse, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            ellipse.CreateTextBox();
            ForTextBox.Read(ellipse.TextBox!, sr);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentEllipse)
        {
            ShapeComponentEllipse(ellipse.ShapeComponentEllipse, sr);
        }
    }

    /// <summary>
    /// 타원 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="sce">타원 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentEllipse(ShapeComponentEllipse sce, CompoundStreamReader sr)
    {
        sce.Property.Value = sr.ReadUInt4();
        sce.CenterX = sr.ReadSInt4();
        sce.CenterY = sr.ReadSInt4();
        sce.Axis1X = sr.ReadSInt4();
        sce.Axis1Y = sr.ReadSInt4();
        sce.Axis2X = sr.ReadSInt4();
        sce.Axis2Y = sr.ReadSInt4();
        sce.StartX = sr.ReadSInt4();
        sce.StartY = sr.ReadSInt4();
        sce.EndX = sr.ReadSInt4();
        sce.EndY = sr.ReadSInt4();
        sce.StartX2 = sr.ReadSInt4();
        sce.StartY2 = sr.ReadSInt4();
        sce.EndX2 = sr.ReadSInt4();
        sce.EndY2 = sr.ReadSInt4();
    }
}
