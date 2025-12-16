using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.SectionDefine;

namespace HwpLib.Reader.BodyText.Control.Secd;

/// <summary>
/// 쪽 테두리/배경 레코드를 읽기 위한 객체
/// </summary>
public static class ForPageBorderFill
{
    /// <summary>
    /// 쪽 테두리/배경 레코드를 읽는다.
    /// </summary>
    /// <param name="pbf">쪽 테두리/배경 레코드</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(PageBorderFill pbf, CompoundStreamReader sr)
    {
        pbf.Property.Value = sr.ReadUInt4();
        pbf.LeftGap = sr.ReadUInt2();
        pbf.RightGap = sr.ReadUInt2();
        pbf.TopGap = sr.ReadUInt2();
        pbf.BottomGap = sr.ReadUInt2();
        pbf.BorderFillId = sr.ReadUInt2();
    }
}
