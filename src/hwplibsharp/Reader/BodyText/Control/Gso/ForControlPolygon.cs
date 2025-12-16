using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 다각형 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlPolygon
{
    /// <summary>
    /// 다각형 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="polygon">다각형 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlPolygon polygon, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            polygon.CreateTextBox();
            ForTextBox.Read(polygon.TextBox!, sr);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentPolygon)
        {
            ShapeComponentPolygon(polygon.ShapeComponentPolygon, sr);
        }
    }

    /// <summary>
    /// 다각형 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scp">다각형 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentPolygon(ShapeComponentPolygon scp, CompoundStreamReader sr)
    {
        int positionCount = sr.ReadSInt4();
        for (int index = 0; index < positionCount; index++)
        {
            var p = scp.AddNewPosition();
            p.X = (uint)sr.ReadSInt4();
            p.Y = (uint)sr.ReadSInt4();
        }
        if (!sr.IsEndOfRecord())
        {
            sr.SkipToEndRecord();
        }
    }
}
