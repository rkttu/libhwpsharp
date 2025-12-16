using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Picture;

namespace HwpLib.Reader.BodyText.Control.Gso.Part;

/// <summary>
/// 그림 개체 속성 레코드의 그림 효과 부분을 읽기 위한 객체
/// </summary>
public static class ForPictureEffect
{
    /// <summary>
    /// 그림 개체 속성 레코드의 그림 효과 부분을 읽는다.
    /// </summary>
    /// <param name="pe">그림 개체 속성 레코드의 그림 효과를 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(PictureEffect pe, CompoundStreamReader sr)
    {
        pe.Property.Value = sr.ReadUInt4();
        if (pe.Property.HasShadowEffect)
        {
            pe.CreateShadowEffect();
            ShadowEffect(pe.ShadowEffect!, sr);
        }
        if (pe.Property.HasNeonEffect)
        {
            pe.CreateNeonEffect();
            NeonEffect(pe.NeonEffect!, sr);
        }
        if (pe.Property.HasSoftBorderEffect)
        {
            pe.CreateSoftEdgeEffect();
            SoftEdgeEffect(pe.SoftEdgeEffect!, sr);
        }
        if (pe.Property.HasReflectionEffect)
        {
            pe.CreateReflectionEffect();
            ReflectionEffect(pe.ReflectionEffect!, sr);
        }
    }

    /// <summary>
    /// 그림자 효과 부분을 읽는다.
    /// </summary>
    /// <param name="se">그림자 효과 부분을 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShadowEffect(ShadowEffect se, CompoundStreamReader sr)
    {
        se.Style = sr.ReadSInt4();
        se.Transparency = sr.ReadFloat();
        se.Cloudy = sr.ReadFloat();
        se.Direction = sr.ReadFloat();
        se.Distance = sr.ReadFloat();
        se.Sort = sr.ReadSInt4();
        se.TiltAngleX = sr.ReadFloat();
        se.TiltAngleY = sr.ReadFloat();
        se.ZoomRateX = sr.ReadFloat();
        se.ZoomRateY = sr.ReadFloat();
        se.RotateWithShape = sr.ReadSInt4();

        ColorProperty(se.Color, sr);
    }

    /// <summary>
    /// 색상 속성 부분을 읽는다.
    /// </summary>
    /// <param name="cp">색상 속성을 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void ColorProperty(ColorWithEffect cp, CompoundStreamReader sr)
    {
        cp.Type = sr.ReadSInt4();
        if (cp.Type == 0)
        {
            byte[] color = sr.ReadBytes(4);
            cp.Color = color;
        }
        else
        {
            throw new InvalidOperationException("not supported color type !!!");
        }
        int colorEffectCount = (int)sr.ReadUInt4();
        for (int index = 0; index < colorEffectCount; index++)
        {
            var ce = cp.AddNewColorEffect();
            ce.Sort = ColorEffectSortExtensions.FromValue(sr.ReadSInt4());
            ce.Value = sr.ReadFloat();
        }
    }

    /// <summary>
    /// 네온 효과 부분을 읽는다.
    /// </summary>
    /// <param name="ne">네온 효과를 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void NeonEffect(NeonEffect ne, CompoundStreamReader sr)
    {
        ne.Transparency = sr.ReadFloat();
        ne.Radius = sr.ReadFloat();
        ColorProperty(ne.Color, sr);
    }

    /// <summary>
    /// 부드러운 가장자리 효과 부분을 읽는다.
    /// </summary>
    /// <param name="see">부드러운 가장자리 효과를 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void SoftEdgeEffect(SoftEdgeEffect see, CompoundStreamReader sr)
    {
        see.Radius = sr.ReadFloat();
    }

    /// <summary>
    /// 반사 효과 부분을 읽는다.
    /// </summary>
    /// <param name="re">반사 효과를 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void ReflectionEffect(ReflectionEffect re, CompoundStreamReader sr)
    {
        re.Style = sr.ReadSInt4();
        re.Radius = sr.ReadFloat();
        re.Direction = sr.ReadFloat();
        re.Distance = sr.ReadFloat();
        re.TiltAngleX = sr.ReadFloat();
        re.TiltAngleY = sr.ReadFloat();
        re.ZoomRateX = sr.ReadFloat();
        re.ZoomRateY = sr.ReadFloat();
        re.RotationStyle = sr.ReadSInt4();
        re.StartTransparency = sr.ReadFloat();
        re.StartPosition = sr.ReadFloat();
        re.EndTransparency = sr.ReadFloat();
        re.EndPosition = sr.ReadFloat();
        re.OffsetDirection = sr.ReadFloat();
    }
}
