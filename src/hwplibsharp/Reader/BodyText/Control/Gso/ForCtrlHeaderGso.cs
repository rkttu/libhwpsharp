using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.Etc;
using HwpLib.Util.Binary;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 그리기 개체의 컨트롤 헤더 레코드를 읽기 위한 객체
/// </summary>
public static class ForCtrlHeaderGso
{
    /// <summary>
    /// 그리기 개체의 컨트롤 헤더 레코드를 읽는다.
    /// (ctrlId는 이미 읽은 상태로 호출됨)
    /// </summary>
    public static void Read(CtrlHeaderGso h, CompoundStreamReader sr)
    {
        h.Property.Value = sr.ReadUInt4();
        h.YOffset = sr.ReadUInt4();
        h.XOffset = sr.ReadUInt4();
        h.Width = sr.ReadUInt4();
        h.Height = sr.ReadUInt4();
        h.ZOrder = sr.ReadSInt4();
        h.OutterMarginLeft = sr.ReadUInt2();
        h.OutterMarginRight = sr.ReadUInt2();
        h.OutterMarginTop = sr.ReadUInt2();
        h.OutterMarginBottom = sr.ReadUInt2();
        h.InstanceId = sr.ReadUInt4();
        int temp = sr.ReadSInt4();
        h.PreventPageDivide = BitFlag.Get((uint)temp, 0);
        
        // HWP 문자열 읽기
        byte[] explanationBytes = sr.ReadHWPString();
        h.Explanation.Bytes = explanationBytes;

        // 남은 바이트가 있으면 Unknown으로 읽음
        if (!sr.IsEndOfRecord())
        {
            h.Unknown = sr.ReadToEndRecord();
        }
    }
}
