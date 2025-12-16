using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.CtrlHeader.AdditionalText;
using HwpLib.Object.DocInfo.ParaShape;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 덧말 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlAdditionalText
{
    /// <summary>
    /// 덧말 컨트롤을 읽는다.
    /// </summary>
    /// <param name="at">덧말 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlAdditionalText at, CompoundStreamReader sr)
    {
        CtrlHeader(at.GetHeader()!, sr);
    }

    /// <summary>
    /// 덧말 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="h">덧말 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderAdditionalText h, CompoundStreamReader sr)
    {
        h.MainText.Bytes = sr.ReadHWPString();
        h.SubText.Bytes = sr.ReadHWPString();
        h.Position = AdditionalTextPositionExtensions.FromValue((byte)sr.ReadUInt4());
        h.Fsizeratio = sr.ReadUInt4();
        h.Option = sr.ReadUInt4();
        h.StyleId = sr.ReadUInt4();
        h.Alignment = AlignmentExtensions.FromValue((byte)sr.ReadUInt4());
    }
}
