using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 감추기 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlPageHide
{
    /// <summary>
    /// 감추기 컨트롤을 읽는다.
    /// </summary>
    /// <param name="pghd">감추기 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlPageHide pghd, CompoundStreamReader sr)
    {
        pghd.GetHeader()!.Property.Value = sr.ReadUInt4();
    }
}
