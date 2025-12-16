using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.ObjectLinkLine;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 객체 연결선 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlObjectLinkLine
{
    /// <summary>
    /// 객체 연결선 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="objectLinkLine">객체 연결선 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlObjectLinkLine objectLinkLine, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentLine)
        {
            ShapeComponentLine(objectLinkLine.ShapeComponentLine, sr);
        }
    }

    /// <summary>
    /// 선 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scl">선 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentLine(ShapeComponentLineForObjectLinkLine scl, CompoundStreamReader sr)
    {
        scl.StartX = sr.ReadSInt4();
        scl.StartY = sr.ReadSInt4();
        scl.EndX = sr.ReadSInt4();
        scl.EndY = sr.ReadSInt4();

        scl.Type = LinkLineTypeExtensions.FromValue((byte)sr.ReadUInt4());
        scl.StartSubjectID = sr.ReadUInt4();
        scl.StartSubjectIndex = sr.ReadUInt4();
        scl.EndSubjectID = sr.ReadUInt4();
        scl.EndSubjectIndex = sr.ReadUInt4();

        int countOfCP = (int)sr.ReadUInt4();
        for (int index = 0; index < countOfCP; index++)
        {
            var cp = scl.AddNewControlPoint();
            cp.X = sr.ReadUInt4();
            cp.Y = sr.ReadUInt4();
            cp.Type = sr.ReadUInt2();
        }

        if (sr.IsEndOfRecord()) return;

        sr.SkipToEndRecord();
    }
}
