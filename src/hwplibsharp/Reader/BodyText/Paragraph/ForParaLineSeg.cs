using HwpLib.CompoundFile;

namespace HwpLib.Reader.BodyText.Paragraph;

/// <summary>
/// 문단 레이아웃 레코드를 읽기 위한 객체
/// </summary>
public static class ForParaLineSeg
{
    /// <summary>
    /// 문단 레이아웃 레코드를 읽는다.
    /// </summary>
    /// <param name="pls">문단 레이아웃</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(HwpLib.Object.BodyText.Paragraph.LineSeg.ParaLineSeg pls, CompoundStreamReader sr)
    {
        // 5.0.2.5 버전 이상부터는 LineSegItem 크기가 다름
        int itemSize = sr.FileVersion.IsOver(5, 0, 2, 5) ? 36 : 32;
        int count = (int)(sr.CurrentRecordHeader!.Size / itemSize);

        for (int i = 0; i < count; i++)
        {
            var item = pls.AddNewLineSegItem();
            
            item.TextStartPosition = sr.ReadUInt4();
            item.LineVerticalPosition = sr.ReadSInt4();
            item.LineHeight = sr.ReadSInt4();
            item.TextPartHeight = sr.ReadSInt4();
            item.DistanceBaseLineToLineVerticalPosition = sr.ReadSInt4();
            item.LineSpace = sr.ReadSInt4();
            item.StartPositionFromColumn = sr.ReadSInt4();
            item.SegmentWidth = sr.ReadSInt4();

            if (sr.FileVersion.IsOver(5, 0, 2, 5))
            {
                item.Tag.Value = sr.ReadUInt4();
            }
        }

        // 남은 바이트가 있으면 건너뛴다
        sr.SkipToEndRecord();
    }
}

