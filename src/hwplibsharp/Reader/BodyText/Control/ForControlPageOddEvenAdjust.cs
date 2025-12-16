using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 홀/짝수 조정 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlPageOddEvenAdjust
{
    /// <summary>
    /// 홀/짝수 조정 컨트롤을 읽는다.
    /// </summary>
    /// <param name="pgoea">홀/짝수 조정 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlPageOddEvenAdjust pgoea, CompoundStreamReader sr)
    {
        pgoea.GetHeader()!.Property.Value = sr.ReadUInt4();
    }
}
