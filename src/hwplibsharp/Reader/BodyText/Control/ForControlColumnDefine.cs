using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.CtrlHeader.ColumnDefine;
using HwpLib.Object.DocInfo.BorderFill;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 단 정의 컨트롤을 읽기 위한 객체
/// </summary>
public static class ForControlColumnDefine
{
    /// <summary>
    /// 단 정의 컨트롤을 읽는다.
    /// </summary>
    /// <param name="cd">단 정의 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(ControlColumnDefine cd, CompoundStreamReader sr)
    {
        CtrlHeader(cd.GetHeader()!, sr);
    }

    /// <summary>
    /// 단 정의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="h">단 정의 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CtrlHeader(CtrlHeaderColumnDefine h, CompoundStreamReader sr)
    {
        h.Property.Value = sr.ReadUInt2();

        int count = h.Property.GetColumnCount();
        bool sameWidth = h.Property.IsSameWidth();

        if (count < 2 || sameWidth)
        {
            h.GapBetweenColumn = sr.ReadUInt2();
            h.Property2 = sr.ReadUInt2();
        }
        else
        {
            h.Property2 = sr.ReadUInt2();
            ColumnInfos(h, sr);
        }

        h.DivideLine.Type = BorderTypeExtensions.FromValue(sr.ReadUInt1());
        h.DivideLine.Thickness = BorderThicknessExtensions.FromValue(sr.ReadUInt1());
        h.DivideLine.Color.Value = sr.ReadUInt4();

        sr.SkipToEndRecord();
    }

    /// <summary>
    /// 단 정의 컨트롤의 컨트롤 헤더 레코드의 단 정보 부분을 읽는다.
    /// </summary>
    /// <param name="h">단 정의 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ColumnInfos(CtrlHeaderColumnDefine h, CompoundStreamReader sr)
    {
        int count = h.Property.GetColumnCount();
        for (int index = 0; index < count; index++)
        {
            ColumnInfo ci = h.AddNewColumnInfo();
            ci.Width = sr.ReadUInt2();
            ci.Gap = sr.ReadUInt2();
        }
    }
}
