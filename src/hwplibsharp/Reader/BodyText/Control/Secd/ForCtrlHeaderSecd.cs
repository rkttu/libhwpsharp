using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.CtrlHeader;

namespace HwpLib.Reader.BodyText.Control.Secd;

/// <summary>
/// 구역 정의 컨트롤의 컨트롤 헤더 레코드를 읽기 위한 객체
/// </summary>
public static class ForCtrlHeaderSecd
{
    /// <summary>
    /// 구역 정의 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="header">구역 정의 컨트롤의 컨트롤 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(CtrlHeaderSectionDefine header, CompoundStreamReader sr)
    {
        header.Property.Value = sr.ReadUInt4();
        header.ColumnGap = sr.ReadUInt2();
        header.VerticalLineAlign = sr.ReadUInt2();
        header.HorizontalLineAlign = sr.ReadUInt2();
        header.DefaultTabGap = sr.ReadUInt4();
        header.NumberParaShapeId = sr.CorrectParaShapeId(sr.ReadUInt2());
        header.PageStartNumber = sr.ReadUInt2();
        header.ImageStartNumber = sr.ReadUInt2();
        header.TableStartNumber = sr.ReadUInt2();
        header.EquationStartNumber = sr.ReadUInt2();

        if (!sr.IsEndOfRecord() && sr.FileVersion.IsOver(5, 0, 1, 2))
        {
            header.DefaultLanguage = sr.ReadUInt2();
        }

        if (sr.IsEndOfRecord()) return;

        sr.SkipToEndRecord();
    }
}
