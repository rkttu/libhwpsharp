using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 책갈피 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlBookmark
{
    /// <summary>
    /// 책갈피 컨트롤을 읽는다.
    /// </summary>
    /// <param name="b">책갈피 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlBookmark b, CompoundStreamReader sr)
    {
        CtrlData(b, sr);
    }

    /// <summary>
    /// 컨트롤 데이터 레코드를 읽는다.
    /// </summary>
    /// <param name="b">책갈피 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlData(ControlBookmark b, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        if (sr.CurrentRecordHeader?.TagId == HWPTag.CtrlData)
        {
            b.CreateCtrlData();
            var ctrlData = ForCtrlData.Read(sr);
            b.SetCtrlData(ctrlData);
        }
    }
}
