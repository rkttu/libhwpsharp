using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 새 번호 지정 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlNewNumber
{
    /// <summary>
    /// 새 번호 지정 컨트롤을 읽는다.
    /// </summary>
    /// <param name="nwno">새 번호 지정 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlNewNumber nwno, CompoundStreamReader sr)
    {
        CtrlHeader(nwno.GetHeader()!, sr);
    }

    /// <summary>
    /// 새 번호 지정 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="header">새 번호 지정 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderNewNumber header, CompoundStreamReader sr)
    {
        header.Property.Value = sr.ReadUInt4();
        header.Number = sr.ReadUInt2();
    }
}
