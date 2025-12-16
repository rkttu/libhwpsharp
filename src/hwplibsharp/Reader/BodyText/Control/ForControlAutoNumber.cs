using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 자동 번호 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlAutoNumber
{
    /// <summary>
    /// 자동 번호 컨트롤을 읽는다.
    /// </summary>
    /// <param name="an">자동번호 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlAutoNumber an, CompoundStreamReader sr)
    {
        CtrlHeader(an.GetHeader()!, sr);
    }

    /// <summary>
    /// 자동 번호 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="h">자동 번호 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderAutoNumber h, CompoundStreamReader sr)
    {
        h.Property.Value = sr.ReadUInt4();
        h.Number = sr.ReadUInt2();
        h.UserSymbol.Bytes = sr.ReadWChar();
        h.BeforeDecorationLetter.Bytes = sr.ReadWChar();
        h.AfterDecorationLetter.Bytes = sr.ReadWChar();
    }
}
