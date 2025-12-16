using HwpLib.Object;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.CtrlHeader.Gso;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent.LineInfo;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent.ShadowInfo;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.DocInfo;
using HwpLib.Object.DocInfo.BinData;
using HwpLib.Object.DocInfo.BorderFill.FillInfo;
using HwpLib.Reader;
using HwpLib.Tool.ObjectFinder;
using HwpLib.Writer;
using System.Drawing;

namespace HwpLibSharp.Test;

/// <summary>
/// 셀에 이미지를 삽입하는 테스트
/// </summary>
[TestClass]
public class InsertingImageCellTest
{
    private const string ImageFileExt = "jpg";
    private const BinDataCompress CompressMethod = BinDataCompress.ByStorageDefault;

    private int _instanceId = 0x5bb840e1;
    private HWPFile? _hwpFile;
    private string _imageFilePath = "";
    private string _fieldName = "";
    private int _streamIndex;
    private int _binDataId;

    private ControlRectangle? _rectangle;
    private Rectangle _shapePosition = new(0, 0, 20, 20);

    [TestMethod]
    public void InsertImageInCell_ShouldSucceed()
    {
        // Arrange
        var docFilePath = TestHelper.GetSamplePath("inserting_image_cell.hwp");
        var imgFilePath = TestHelper.GetImagePath("sample.jpg");
        var hwpFile = HWPReader.FromFile(docFilePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        InsertShapeWithImage(hwpFile, "필드3", imgFilePath);
        
        var writePath = TestHelper.GetResultPath("result-inserting-image-cell.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "셀에 이미지 삽입 성공");
    }

    private void InsertShapeWithImage(HWPFile hwpFile, string fieldName, string imgFilePath)
    {
        _hwpFile = hwpFile;
        _fieldName = fieldName;
        _imageFilePath = imgFilePath;

        AddBinData();
        _binDataId = AddBinDataInDocInfo(_streamIndex);
        AddGsoControl();
    }

    private void AddBinData()
    {
        _streamIndex = _hwpFile!.BinData.EmbeddedBinaryDataList.Count + 1;
        var streamName = GetStreamName();
        var fileBinary = LoadFile();

        _hwpFile.BinData.AddNewEmbeddedBinaryData(streamName, fileBinary, CompressMethod);
    }

    private string GetStreamName()
    {
        return $"Bin{_streamIndex:X4}.{ImageFileExt}";
    }

    private byte[] LoadFile()
    {
        return File.ReadAllBytes(_imageFilePath);
    }

    private int AddBinDataInDocInfo(int streamIndex)
    {
        var bd = _hwpFile!.DocInfo.AddNewBinData();
        bd.Property.Type = BinDataType.Embedding;
        bd.Property.Compress = CompressMethod;
        bd.Property.State = BinDataState.NotAccess;
        bd.BinDataId = streamIndex;
        bd.ExtensionForEmbedding = ImageFileExt;
        return _hwpFile.DocInfo.BinDataList.Count;
    }

    private void AddGsoControl()
    {
        CreateRectangleControlAtCell();

        if (_rectangle != null)
        {
            SetCtrlHeaderGso();
            SetShapeComponent();
            SetShapeComponentRectangle();
        }
    }

    private void CreateRectangleControlAtCell()
    {
        var cellList = CellFinder.FindAll(_hwpFile!, _fieldName);
        foreach (var c in cellList)
        {
            Paragraph? firstPara = c.ParagraphList.GetParagraph(0);
            if (firstPara == null) continue;
            
            var paraText = firstPara.Text;
            if (paraText == null)
            {
                firstPara.CreateText();
                paraText = firstPara.Text;
            }

            // 문단에서 사각형 컨트롤의 위치를 표현하기 위한 확장 문자를 넣는다.
            paraText?.AddExtendCharForGSO();

            // 문단에 사각형 컨트롤 추가한다.
            _rectangle = (ControlRectangle?)firstPara.AddNewGsoControl(GsoControlType.Rectangle);
            break;
        }
    }

    private void SetCtrlHeaderGso()
    {
        var hdr = _rectangle!.GetHeader()!;
        var prop = hdr.Property;
        prop.SetLikeWord(false);
        prop.SetApplyLineSpace(false);
        prop.SetVertRelTo(VertRelTo.Para);
        prop.SetVertRelativeArrange(RelativeArrange.TopOrLeft);
        prop.SetHorzRelTo(HorzRelTo.Para);
        prop.SetHorzRelativeArrange(RelativeArrange.TopOrLeft);
        prop.SetVertRelToParaLimit(true);
        prop.SetAllowOverlap(true);
        prop.SetWidthCriterion(WidthCriterion.Absolute);
        prop.SetHeightCriterion(HeightCriterion.Absolute);
        prop.SetProtectSize(false);
        prop.SetTextFlowMethod(TextFlowMethod.FitWithText);
        prop.SetTextHorzArrange(TextHorzArrange.BothSides);
        prop.SetObjectNumberSort(ObjectNumberSort.Figure);

        hdr.YOffset = (uint)FromMm(_shapePosition.Y);
        hdr.XOffset = (uint)FromMm(_shapePosition.X);
        hdr.Width = (uint)FromMm(_shapePosition.Width);
        hdr.Height = (uint)FromMm(_shapePosition.Height);
        hdr.ZOrder = 0;
        hdr.OutterMarginLeft = 0;
        hdr.OutterMarginRight = 0;
        hdr.OutterMarginTop = 0;
        hdr.OutterMarginBottom = 0;
        hdr.InstanceId = (uint)_instanceId;
        hdr.PreventPageDivide = false;
        hdr.Explanation.Bytes = null;
    }

    private static int FromMm(int mm)
    {
        if (mm == 0)
        {
            return 1;
        }

        return (int)((double)mm * 72000.0f / 254.0f + 0.5f);
    }

    private void SetShapeComponent()
    {
        var sc = (ShapeComponentNormal)_rectangle!.ShapeComponent;
        sc.OffsetX = 0;
        sc.OffsetY = 0;
        sc.GroupingCount = 0;
        sc.LocalFileVersion = 1;
        sc.WidthAtCreate = FromMm(_shapePosition.Width);
        sc.HeightAtCreate = FromMm(_shapePosition.Height);
        sc.WidthAtCurrent = FromMm(_shapePosition.Width);
        sc.HeightAtCurrent = FromMm(_shapePosition.Height);
        sc.RotateAngle = 0;
        sc.RotateXCenter = FromMm(_shapePosition.Width / 2);
        sc.RotateYCenter = FromMm(_shapePosition.Height / 2);

        sc.CreateLineInfo();
        var li = sc.LineInfo!;
        li.Property.LineEndShape = LineEndShape.Flat;
        li.Property.StartArrowShape = LineArrowShape.None;
        li.Property.StartArrowSize = LineArrowSize.MiddleMiddle;
        li.Property.EndArrowShape = LineArrowShape.None;
        li.Property.EndArrowSize = LineArrowSize.MiddleMiddle;
        li.Property.IsFillStartArrow = true;
        li.Property.IsFillEndArrow = true;
        li.Property.LineType = LineType.None;
        li.OutlineStyle = OutlineStyle.Normal;
        li.Thickness = 0;
        li.Color.Value = 0;

        sc.CreateFillInfo();
        var fi = sc.FillInfo!;
        fi.Type.HasPatternFill = false;
        fi.Type.HasImageFill = true;
        fi.Type.HasGradientFill = false;
        fi.CreateImageFill();
        var imgF = fi.ImageFill!;
        imgF.ImageFillType = ImageFillType.FitSize;
        imgF.PictureInfo.Brightness = 0;
        imgF.PictureInfo.Contrast = 0;
        imgF.PictureInfo.Effect = PictureEffect.RealPicture;
        imgF.PictureInfo.BinItemID = _binDataId;

        sc.CreateShadowInfo();
        var si = sc.ShadowInfo!;
        si.Type = ShadowType.None;
        si.Color.Value = 0xc4c4c4;
        si.OffsetX = 283;
        si.OffsetY = 283;
        si.Transparent = 0;

        sc.SetMatrixsNormal();
    }

    private void SetShapeComponentRectangle()
    {
        var scr = _rectangle!.ShapeComponentRectangle;
        scr.RoundRate = 0;
        scr.X1 = 0;
        scr.Y1 = 0;
        scr.X2 = FromMm(_shapePosition.Width);
        scr.Y2 = 0;
        scr.X3 = FromMm(_shapePosition.Width);
        scr.Y3 = FromMm(_shapePosition.Height);
        scr.X4 = 0;
        scr.Y4 = FromMm(_shapePosition.Height);
    }
}
