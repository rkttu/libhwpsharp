using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.SectionDefine;

namespace HwpLib.Reader.BodyText.Control.Secd;

/// <summary>
/// 용지 설정 레코드를 읽기 위한 객체
/// </summary>
public static class ForPageDef
{
    /// <summary>
    /// 용지 설정 레코드를 읽는다.
    /// </summary>
    /// <param name="pd">용지 설정 레코드</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(PageDef pd, CompoundStreamReader sr)
    {
        pd.PaperWidth = sr.ReadUInt4();
        pd.PaperHeight = sr.ReadUInt4();
        pd.LeftMargin = sr.ReadUInt4();
        pd.RightMargin = sr.ReadUInt4();
        pd.TopMargin = sr.ReadUInt4();
        pd.BottomMargin = sr.ReadUInt4();
        pd.HeaderMargin = sr.ReadUInt4();
        pd.FooterMargin = sr.ReadUInt4();
        pd.GutterMargin = sr.ReadUInt4();
        pd.Property.Value = sr.ReadUInt4();
    }
}
