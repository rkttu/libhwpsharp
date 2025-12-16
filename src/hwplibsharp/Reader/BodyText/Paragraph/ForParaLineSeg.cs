using HwpLib.CompoundFile;

namespace HwpLib.Reader.BodyText.Paragraph;

/// <summary>
/// 문단의 레이아웃 레코드를 읽기 위한 객체
/// </summary>
public static class ForParaLineSeg
{
    /// <summary>
    /// 문단의 레이아웃 레코드를 읽는다.
    /// </summary>
    /// <param name="p">문단 객체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(HwpLib.Object.BodyText.Paragraph.Paragraph p, CompoundStreamReader sr)
    {
        p.CreateLineSeg();
        int count = p.Header.LineAlignCount;
        if (count != 0)
        {
            var pls = p.LineSeg!;
            for (int i = 0; i < count; i++)
            {
                var item = pls.AddNewLineSegItem();
                ParaLineSegItem(item, sr);
            }
        }
        else
        {
            sr.SkipToEndRecord();
        }
    }

    /// <summary>
    /// 한 라인의 레이아웃 정보를 읽는다.
    /// </summary>
    /// <param name="item">한 라인의 레이아웃 정보</param>
    /// <param name="sr">스트림 리더</param>
    private static void ParaLineSegItem(HwpLib.Object.BodyText.Paragraph.LineSeg.LineSegItem item, CompoundStreamReader sr)
    {
        item.TextStartPosition = sr.ReadUInt4();
        item.LineVerticalPosition = sr.ReadSInt4();
        item.LineHeight = sr.ReadSInt4();
        item.TextPartHeight = sr.ReadSInt4();
        item.DistanceBaseLineToLineVerticalPosition = sr.ReadSInt4();
        item.LineSpace = sr.ReadSInt4();
        item.StartPositionFromColumn = sr.ReadSInt4();
        item.SegmentWidth = sr.ReadSInt4();
        item.Tag.Value = sr.ReadUInt4();
    }
}

