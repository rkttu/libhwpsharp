using HwpLib.CompoundFile;

namespace HwpLib.Reader.BodyText.Paragraph;

/// <summary>
/// 문단 글자 모양 레코드를 읽기 위한 객체
/// </summary>
public static class ForParaCharShape
{
    /// <summary>
    /// 문단 글자 모양 레코드를 읽는다.
    /// </summary>
    /// <param name="pcs">문단 글자 모양</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(HwpLib.Object.BodyText.Paragraph.CharShape.ParaCharShape pcs, CompoundStreamReader sr)
    {
        // 레코드 크기 / 8 = 항목 수 (position 4바이트 + charShapeId 4바이트)
        int count = (int)(sr.CurrentRecordHeader!.Size / 8);

        for (int i = 0; i < count; i++)
        {
            uint position = sr.ReadUInt4();
            uint charShapeId = sr.ReadUInt4();
            pcs.AddParaCharShape(position, charShapeId);
        }

        // 남은 바이트가 있으면 건너뛴다
        sr.SkipToEndRecord();
    }
}

