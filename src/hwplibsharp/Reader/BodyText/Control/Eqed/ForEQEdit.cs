using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Equation;

namespace HwpLib.Reader.BodyText.Control.Eqed;

/// <summary>
/// 수식 정보 레코드를 읽기 위한 객체
/// </summary>
public static class ForEQEdit
{
    /// <summary>
    /// 수식 정보 레코드를 읽는다.
    /// </summary>
    /// <param name="eqEdit">수식 정보 레코드</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(EQEdit eqEdit, CompoundStreamReader sr)
    {
        eqEdit.Property = sr.ReadUInt4();
        eqEdit.Script.Bytes = sr.ReadHWPString();
        eqEdit.LetterSize = sr.ReadUInt4();
        eqEdit.LetterColor.Value = sr.ReadUInt4();

        if (sr.IsEndOfRecord()) return;

        eqEdit.BaseLine = sr.ReadSInt2();

        if (sr.IsEndOfRecord()) return;

        eqEdit.Unknown = sr.ReadUInt2();

        if (sr.IsEndOfRecord()) return;

        eqEdit.VersionInfo.Bytes = sr.ReadHWPString();

        if (sr.IsEndOfRecord()) return;

        eqEdit.FontName.Bytes = sr.ReadHWPString();
    }
}
