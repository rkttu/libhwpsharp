using HwpLib.CompoundFile;

namespace HwpLib.Reader.BodyText.Paragraph;

/// <summary>
/// 문단 헤더 레코드를 읽기 위한 객체
/// </summary>
public static class ForParaHeader
{
    /// <summary>
    /// 문단 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="ph">문단 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(HwpLib.Object.BodyText.Paragraph.Header.ParaHeader ph, CompoundStreamReader sr)
    {
        // 문단 리스트에서 마지막 문단여부와 문자수를 읽는다.
        uint value = sr.ReadUInt4();
        ph.LastInList = (value & 0x80000000) != 0;
        ph.CharacterCount = value & 0x7fffffff;

        ph.ControlMask.Value = sr.ReadUInt4();
        ph.ParaShapeId = sr.ReadUInt2();
        ph.StyleId = sr.ReadUInt1();
        ph.DivideSort.Value = sr.ReadUInt1();
        ph.CharShapeCount = sr.ReadUInt2();
        ph.RangeTagCount = sr.ReadUInt2();
        ph.LineAlignCount = sr.ReadUInt2();
        ph.InstanceID = sr.ReadUInt4();

        if (!sr.IsEndOfRecord() && sr.FileVersion.IsOver(5, 0, 3, 2))
        {
            ph.IsMergedByTrack = sr.ReadUInt2();
        }

        // 남은 바이트가 있으면 건너뛴다
        sr.SkipToEndRecord();
    }
}

