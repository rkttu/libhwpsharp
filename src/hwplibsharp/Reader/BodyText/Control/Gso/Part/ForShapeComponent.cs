using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent.LineInfo;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent.RenderingInfo;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent.ShadowInfo;
using HwpLib.Reader.DocInfo.BorderFill;

namespace HwpLib.Reader.BodyText.Control.Gso.Part;

/// <summary>
/// 그리기 개체의 객체 공통 속성 레코드를 읽기 위한 객체
/// </summary>
public static class ForShapeComponent
{
    /// <summary>
    /// 그리기 개체의 객체 공통 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="gsoControl">그리기 개체</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(GsoControl gsoControl, CompoundStreamReader sr)
    {
        if (gsoControl.GsoType == GsoControlType.Container)
        {
            ShapeComponentForContainer((ShapeComponentContainer)gsoControl.ShapeComponent, sr);
        }
        else
        {
            ShapeComponentForNormal((ShapeComponentNormal)gsoControl.ShapeComponent, sr);
        }
    }

    /// <summary>
    /// 일반 컨트롤을 위한 객체 공통 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scn">객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentForNormal(ShapeComponentNormal scn, CompoundStreamReader sr)
    {
        CommonPart(scn, sr);

        if (sr.IsEndOfRecord()) return;

        LineInfo(scn, sr);

        if (sr.IsEndOfRecord()) return;

        FillInfo(scn, sr);

        if (sr.IsEndOfRecord()) return;

        ShadowInfo(scn, sr);

        if (sr.IsEndOfRecord()) return;

        scn.Instid = sr.ReadUInt4();
        sr.Skip(1);
        if (scn.ShadowInfo != null)
        {
            scn.ShadowInfo.Transparent = sr.ReadUInt1();
        }
        else
        {
            sr.Skip(1);
        }
    }

    /// <summary>
    /// 객체 공통 속성 레코드의 공통 부분을 읽는다.
    /// </summary>
    /// <param name="sc">객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void CommonPart(ShapeComponent sc, CompoundStreamReader sr)
    {
        sc.OffsetX = sr.ReadSInt4();
        sc.OffsetY = sr.ReadSInt4();
        sc.GroupingCount = sr.ReadUInt2();
        sc.LocalFileVersion = sr.ReadUInt2();
        sc.WidthAtCreate = sr.ReadSInt4();
        sc.HeightAtCreate = sr.ReadSInt4();
        sc.WidthAtCurrent = sr.ReadSInt4();
        sc.HeightAtCurrent = sr.ReadSInt4();
        sc.Property.Value = sr.ReadUInt4();
        sc.RotateAngle = sr.ReadUInt2();
        sc.RotateXCenter = sr.ReadSInt4();
        sc.RotateYCenter = sr.ReadSInt4();

        RenderingInfo(sc.RenderingInfo, sr);
    }

    /// <summary>
    /// 객체 공통 속성 레코드의 rendering 정보를 읽는다.
    /// </summary>
    /// <param name="ri">rendering 정보를 나타내는 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void RenderingInfo(RenderingInfo ri, CompoundStreamReader sr)
    {
        int scaleRotateMatrixCount = sr.ReadUInt2();
        Matrix(ri.TranslationMatrix, sr);
        for (int index = 0; index < scaleRotateMatrixCount; index++)
        {
            var srmp = ri.AddNewScaleRotateMatrixPair();
            Matrix(srmp.ScaleMatrix, sr);
            Matrix(srmp.RotateMatrix, sr);
        }
    }

    /// <summary>
    /// 변환 행렬을 읽는다.
    /// </summary>
    /// <param name="m">변환 행렬 객체</param>
    /// <param name="sr">스트림 리더</param>
    private static void Matrix(Matrix m, CompoundStreamReader sr)
    {
        for (int index = 0; index < 6; index++)
        {
            m.SetValue(index, sr.ReadDouble());
        }
    }

    /// <summary>
    /// 일반 컨트롤을 위한 객체 공통 속성 레코드의 line 정보를 읽는다.
    /// </summary>
    /// <param name="scn">일반 컨트롤을 위한 객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void LineInfo(ShapeComponentNormal scn, CompoundStreamReader sr)
    {
        scn.CreateLineInfo();
        var li = scn.LineInfo!;
        li.Color.Value = sr.ReadUInt4();
        li.Thickness = sr.ReadSInt4();
        li.Property.Value = sr.ReadUInt4();
        li.OutlineStyle = OutlineStyleExtensions.FromValue((byte)sr.ReadUInt1());
    }

    /// <summary>
    /// 일반 컨트롤을 위한 객체 공통 속성 레코드의 배경 정보를 읽는다.
    /// </summary>
    /// <param name="scn">일반 컨트롤을 위한 객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void FillInfo(ShapeComponentNormal scn, CompoundStreamReader sr)
    {
        scn.CreateFillInfo();
        ForFillInfo.Read(scn.FillInfo!, sr);
    }

    /// <summary>
    /// 일반 컨트롤을 위한 객체 공통 속성 레코드의 그림자 정보를 읽는다.
    /// </summary>
    /// <param name="scn">일반 컨트롤을 위한 객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShadowInfo(ShapeComponentNormal scn, CompoundStreamReader sr)
    {
        scn.CreateShadowInfo();
        var si = scn.ShadowInfo!;
        si.Type = ShadowTypeExtensions.FromValue((byte)sr.ReadUInt4());
        si.Color.Value = sr.ReadUInt4();
        si.OffsetX = sr.ReadSInt4();
        si.OffsetY = sr.ReadSInt4();
    }

    /// <summary>
    /// 묶음 컨트롤을 위한 객체 공통 속성 레코드를 읽는다.
    /// </summary>
    /// <param name="scc">객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ShapeComponentForContainer(ShapeComponentContainer scc, CompoundStreamReader sr)
    {
        CommonPart(scc, sr);
        ChildInfo(scc, sr);

        if (sr.IsEndOfRecord()) return;

        scc.Instid = sr.ReadUInt4();
    }

    /// <summary>
    /// 포함하고 있는 컨트롤에 대한 정보 부분을 읽는다.
    /// </summary>
    /// <param name="scc">묶음 컨트롤의 객체 공통 속성 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ChildInfo(ShapeComponentContainer scc, CompoundStreamReader sr)
    {
        int count = sr.ReadUInt2();
        for (int index = 0; index < count; index++)
        {
            uint childId = sr.ReadUInt4();
            scc.AddChildControlId(childId);
        }
    }
}
