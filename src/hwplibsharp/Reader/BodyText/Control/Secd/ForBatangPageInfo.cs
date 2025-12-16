using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.SectionDefine;

namespace HwpLib.Reader.BodyText.Control.Secd;

/// <summary>
/// 바탕쪽 정보를 읽기 위한 객체
/// </summary>
public static class ForBatangPageInfo
{
    /// <summary>
    /// 바탕쪽 정보를 읽는다.
    /// </summary>
    /// <param name="bpi">바탕쪽 정보</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(BatangPageInfo bpi, CompoundStreamReader sr)
    {
        ListHeader(bpi.ListHeader, sr);
        ForParagraphList.Read(bpi.ParagraphList, sr);
    }

    /// <summary>
    /// 바탕쪽의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="lh">바탕쪽의 문단 리스트 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ListHeader(ListHeaderForBatangPage lh, CompoundStreamReader sr)
    {
        lh.ParaCount = sr.ReadSInt4();
        lh.Property.Value = sr.ReadUInt4();
        lh.TextWidth = sr.ReadUInt4();
        lh.TextHeight = sr.ReadUInt4();
        sr.SkipToEndRecord();
    }
}
