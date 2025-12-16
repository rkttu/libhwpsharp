using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 쪽 번호 위치 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlPageNumberPosition
{
    /// <summary>
    /// 쪽 번호 위치 컨트롤을 읽는다.
    /// </summary>
    /// <param name="pgnp">쪽 번호 위치 컨트롤 객체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlPageNumberPosition pgnp, CompoundStreamReader sr)
    {
        CtrlHeader(pgnp.GetHeader()!, sr);
    }

    /// <summary>
    /// 쪽 번호 위치 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="header">쪽 번호 위치 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderPageNumberPosition header, CompoundStreamReader sr)
    {
        header.Property.Value = sr.ReadUInt4();
        header.Number = sr.ReadUInt2();
        header.UserSymbol.Bytes = sr.ReadWChar();
        header.BeforeDecorationLetter.Bytes = sr.ReadWChar();
        header.AfterDecorationLetter.Bytes = sr.ReadWChar();
    }
}
