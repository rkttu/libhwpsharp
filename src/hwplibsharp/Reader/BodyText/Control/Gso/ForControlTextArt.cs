using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Polygon;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.TextArt;
using HwpLib.Object.DocInfo.FaceName;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 글맵시 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlTextArt
{
    /// <summary>
    /// 글맵시 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="textArt">글맵시 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlTextArt textArt, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentTextArt)
        {
            ShapeComponentTextArt(textArt.ShapeComponentTextArt, sr);
        }
    }

    /// <summary>
    /// 글맵시 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scta">글맵시 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentTextArt(ShapeComponentTextArt scta, CompoundStreamReader sr)
    {
        scta.X1 = sr.ReadSInt4();
        scta.Y1 = sr.ReadSInt4();
        scta.X2 = sr.ReadSInt4();
        scta.Y2 = sr.ReadSInt4();
        scta.X3 = sr.ReadSInt4();
        scta.Y3 = sr.ReadSInt4();
        scta.X4 = sr.ReadSInt4();
        scta.Y4 = sr.ReadSInt4();
        scta.Content.Bytes = sr.ReadHWPString();
        scta.FontName.Bytes = sr.ReadHWPString();
        scta.FontStyle.Bytes = sr.ReadHWPString();
        scta.FontType = FontTypeExtensions.FromValue((byte)sr.ReadSInt4());
        scta.TextArtShape = TextArtShapeExtensions.FromValue((byte)sr.ReadSInt4());
        scta.LineSpace = sr.ReadSInt4();
        scta.CharSpace = sr.ReadSInt4();
        scta.ParaAlignment = TextArtAlignExtensions.FromValue((byte)sr.ReadSInt4());
        scta.ShadowType = sr.ReadSInt4();
        scta.ShadowOffsetX = sr.ReadSInt4();
        scta.ShadowOffsetY = sr.ReadSInt4();
        scta.ShadowColor.Value = sr.ReadUInt4();

        int outlinePointCount = sr.ReadSInt4();
        for (int index = 0; index < outlinePointCount; index++)
        {
            var positionXY = scta.AddNewOutlinePoint();
            positionXY.X = (uint)sr.ReadSInt4();
            positionXY.Y = (uint)sr.ReadSInt4();
        }
    }
}
