using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 사각형 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlRectangle
{
    /// <summary>
    /// 사각형 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="rectangle">사각형 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlRectangle rectangle, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.CtrlData)
        {
            rectangle.CreateCtrlData();
            var ctrlData = ForCtrlData.Read(sr);
            rectangle.SetCtrlData(ctrlData);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }

        if (sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            rectangle.CreateTextBox();
            ForTextBox.Read(rectangle.TextBox!, sr);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }

        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentRectangle)
        {
            ShapeComponentRectangle(rectangle.ShapeComponentRectangle, sr);
        }
    }

    /// <summary>
    /// 사각형 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scr">사각형 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentRectangle(ShapeComponentRectangle scr, CompoundStreamReader sr)
    {
        scr.RoundRate = sr.ReadUInt1();
        scr.X1 = sr.ReadSInt4();
        scr.Y1 = sr.ReadSInt4();
        scr.X2 = sr.ReadSInt4();
        scr.Y2 = sr.ReadSInt4();
        scr.X3 = sr.ReadSInt4();
        scr.Y3 = sr.ReadSInt4();
        scr.X4 = sr.ReadSInt4();
        scr.Y4 = sr.ReadSInt4();
    }
}
