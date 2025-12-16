using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 글자 겹침 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlOverlappingLetter
{
    /// <summary>
    /// 글자 겹침 컨트롤을 읽는다.
    /// </summary>
    /// <param name="tcps">글자 겹침 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlOverlappingLetter tcps, CompoundStreamReader sr)
    {
        CtrlHeader(tcps.GetHeader()!, sr);
    }

    /// <summary>
    /// 글자 겹침 컨트롤의 컨트롤 헤더 레코드을 읽는다.
    /// </summary>
    /// <param name="header">글자 겹침 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderOverlappingLetter header, CompoundStreamReader sr)
    {
        OverlappingLetters(header, sr);

        if (sr.IsEndOfRecord()) return;

        header.BorderType = sr.ReadUInt1();
        header.InternalFontSize = (byte)sr.ReadSInt1();
        header.ExpendInsideLetter = sr.ReadUInt1();

        CharShapeIds(header, sr);
    }

    /// <summary>
    /// 겹침 글자 부분을 읽는다.
    /// </summary>
    /// <param name="header">글자 겹침 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void OverlappingLetters(CtrlHeaderOverlappingLetter header, CompoundStreamReader sr)
    {
        int count = sr.ReadUInt2();
        for (int index = 0; index < count; index++)
        {
            var letter = new HWPString();
            letter.Bytes = sr.ReadWChar();
            header.AddOverlappingLetter(letter);
        }
    }

    /// <summary>
    /// 글자 모양 부분을 읽는다.
    /// </summary>
    /// <param name="header">글자 겹침 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CharShapeIds(CtrlHeaderOverlappingLetter header, CompoundStreamReader sr)
    {
        short count = sr.ReadUInt1();
        for (short i = 0; i < count; i++)
        {
            uint charShapeId = sr.ReadUInt4();
            header.AddCharShapeId(charShapeId);
        }
    }
}
