using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// OLE 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlOLE
{
    /// <summary>
    /// OLE 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="ole">OLE 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlOLE ole, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentOle)
        {
            ShapeComponentOLE(ole.ShapeComponentOLE, sr);
        }
    }

    /// <summary>
    /// OLE 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="sco">OLE 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentOLE(ShapeComponentOLE sco, CompoundStreamReader sr)
    {
        sco.Property.Value = sr.ReadUInt4();
        sco.ExtentWidth = sr.ReadSInt4();
        sco.ExtentHeight = sr.ReadSInt4();
        sco.BinDataId = sr.ReadUInt2();
        sco.BorderColor.Value = sr.ReadUInt4();
        sco.BorderThickness = sr.ReadSInt4();
        sco.BorderProperty.Value = sr.ReadUInt4();
        UnknownData(sco, sr);
    }

    /// <summary>
    /// 알 수 없는 데이터 블럭을 읽는다.
    /// </summary>
    /// <param name="sco">OLE 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void UnknownData(ShapeComponentOLE sco, CompoundStreamReader sr)
    {
        int unknownSize = (int)(sr.CurrentRecordHeader!.Size - (sr.CurrentPosition - sr.CurrentPositionAfterHeader));
        if (unknownSize > 0)
        {
            byte[] unknown = sr.ReadBytes(unknownSize);
            sco.Unknown = unknown;
        }
    }
}
