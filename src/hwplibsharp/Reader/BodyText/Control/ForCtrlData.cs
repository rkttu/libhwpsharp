using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Bookmark;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 임의 데이타 레코드를 읽기 위한 객체
/// </summary>
public static class ForCtrlData
{
    /// <summary>
    /// 임의 데이터 객체를 읽는다.
    /// </summary>
    /// <param name="sr">스트림 리더</param>
    /// <returns>읽은 임의 데이터 객체</returns>
    public static CtrlData Read(CompoundStreamReader sr)
    {
        var ctrlData = new CtrlData();
        ForParameterSet.Read(ctrlData.ParameterSet, sr);
        return ctrlData;
    }
}
