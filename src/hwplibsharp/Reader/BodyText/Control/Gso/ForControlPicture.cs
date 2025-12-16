using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Picture;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Gso.Part;
using HwpLib.Reader.DocInfo.BorderFill;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 그림 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlPicture
{
    /// <summary>
    /// 그림 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="picture">그림 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlPicture picture, CompoundStreamReader sr)
    {
        sr.ReadRecordHeader();
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.CtrlData)
        {
            picture.CreateCtrlData();
            var ctrlData = ForCtrlData.Read(sr);
            picture.SetCtrlData(ctrlData);

            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }
        }
        
        if (sr.CurrentRecordHeader?.TagId == HWPTag.ShapeComponentPicture)
        {
            ShapeComponentPicture(picture.ShapeComponentPicture, sr);
        }
    }

    /// <summary>
    /// 그림 개체 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scp">그림 개체 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentPicture(ShapeComponentPicture scp, CompoundStreamReader sr)
    {
        scp.BorderColor.Value = sr.ReadUInt4();
        scp.BorderThickness = sr.ReadSInt4();
        scp.BorderProperty.Value = sr.ReadUInt4();
        scp.LeftTop.X = (uint)sr.ReadSInt4();
        scp.LeftTop.Y = (uint)sr.ReadSInt4();
        scp.RightTop.X = (uint)sr.ReadSInt4();
        scp.RightTop.Y = (uint)sr.ReadSInt4();
        scp.RightBottom.X = (uint)sr.ReadSInt4();
        scp.RightBottom.Y = (uint)sr.ReadSInt4();
        scp.LeftBottom.X = (uint)sr.ReadSInt4();
        scp.LeftBottom.Y = (uint)sr.ReadSInt4();
        scp.LeftAfterCutting = sr.ReadSInt4();
        scp.TopAfterCutting = sr.ReadSInt4();
        scp.RightAfterCutting = sr.ReadSInt4();
        scp.BottomAfterCutting = sr.ReadSInt4();
        InnerMargin(scp.InnerMargin, sr);
        ForFillInfo.ReadPictureInfo(scp.PictureInfo, sr);

        if (sr.IsEndOfRecord()) return;

        scp.BorderTransparency = sr.ReadUInt1();

        if (sr.IsEndOfRecord()) return;

        scp.InstanceId = sr.ReadUInt4();

        if (sr.IsEndOfRecord()) return;

        ForPictureEffect.Read(scp.PictureEffect, sr);

        if (sr.IsEndOfRecord()) return;

        scp.ImageWidth = sr.ReadUInt4();
        scp.ImageHeight = sr.ReadUInt4();

        if (sr.IsEndOfRecord()) return;

        sr.SkipToEndRecord();
    }

    /// <summary>
    /// 그림 개체 속성 레코드의 내부 여백 부분을 읽는다.
    /// </summary>
    /// <param name="im">내부 여백을 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void InnerMargin(InnerMargin im, CompoundStreamReader sr)
    {
        im.Left = sr.ReadUInt2();
        im.Right = sr.ReadUInt2();
        im.Top = sr.ReadUInt2();
        im.Bottom = sr.ReadUInt2();
    }
}
