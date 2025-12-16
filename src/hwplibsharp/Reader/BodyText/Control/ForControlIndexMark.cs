using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 찾아보기 표식 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlIndexMark
{
    /// <summary>
    /// 찾아보기 표식 컨트롤을 읽는다.
    /// </summary>
    /// <param name="idxm">찾아보기 표식 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlIndexMark idxm, CompoundStreamReader sr)
    {
        CtrlHeader(idxm.GetHeader()!, sr);
    }

    /// <summary>
    /// 찾아보기 표시 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="header">찾아보기 표시 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderIndexMark header, CompoundStreamReader sr)
    {
        header.Keyword1.Bytes = sr.ReadHWPString();
        header.Keyword2.Bytes = sr.ReadHWPString();
        if (!sr.IsEndOfRecord())
        {
            sr.SkipToEndRecord();
        }
    }
}
