using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader.Gso;
using HwpLib.Object.BodyText.Control.CtrlHeader.SectionDefine;
using HwpLib.Object.BodyText.Control.Gso.TextBox;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.CharShape;
using HwpLib.Object.BodyText.Paragraph.Header;
using HwpLib.Object.BodyText.Paragraph.LineSeg;
using HwpLib.Object.DocInfo;
using HwpLib.Object.DocInfo.BinData;
using HwpLib.Object.DocInfo.BorderFill;
using HwpLib.Object.DocInfo.BorderFill.FillInfo;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 이미지 배경이 있는 표를 삽입하는 테스트
/// </summary>
[TestClass]
public class InsertingTableWithImageBackTest
{
    private readonly string _imageFilePath = TestHelper.GetImagePath("sample.jpg");
    private const string ImageFileExt = "jpg";
    private const BinDataCompress CompressMethod = BinDataCompress.ByStorageDefault;

    private HWPFile? _hwpFile;
    private ControlTable? _table;
    private Row? _row;
    private Cell? _cell;
    private int _borderFillIdForCell;

    private int _zOrder = 0;

    private int _streamIndex;
    private int _binDataId;

    [TestMethod]
    public void InsertTableWithImageBack_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("blank.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        _hwpFile = hwpFile;

        // Act
        AddBinData();
        AddBinDataInDocInfo();
        MakeTable();
        
        var writePath = TestHelper.GetResultPath("result-inserting-table-with-imageback.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "이미지 배경 표 삽입 성공");
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

    private void AddBinDataInDocInfo()
    {
        var bd = _hwpFile!.DocInfo.AddNewBinData();
        bd.Property.Type = BinDataType.Embedding;
        bd.Property.Compress = CompressMethod;
        bd.Property.State = BinDataState.NotAccess;
        bd.BinDataId = _streamIndex;
        bd.ExtensionForEmbedding = ImageFileExt;
        _binDataId = _hwpFile.DocInfo.BinDataList.Count;
    }

    private void MakeTable()
    {
        CreateTableControlAtFirstParagraph();
        SetCtrlHeaderRecord();
        SetTableRecordFor2By2Cells();
        Add2By2Cell();
    }

    private void CreateTableControlAtFirstParagraph()
    {
        Section firstSection = _hwpFile!.BodyText.SectionList[0];
        Paragraph? firstParagraph = firstSection.GetParagraph(0);
        if (firstParagraph?.Text == null) return;

        // 문단에서 표 컨트롤의 위치를 표현하기 위한 확장 문자를 넣는다.
        firstParagraph.Text.AddExtendCharForTable();

        // 문단에 표 컨트롤 추가한다.
        _table = (ControlTable?)firstParagraph.AddNewControl(ControlType.Table);
    }

    private void SetCtrlHeaderRecord()
    {
        if (_table == null) return;
        
        var ctrlHeader = _table.Header;
        ctrlHeader.Property.SetLikeWord(false);
        ctrlHeader.Property.SetApplyLineSpace(false);
        ctrlHeader.Property.SetVertRelTo(VertRelTo.Para);
        ctrlHeader.Property.SetVertRelativeArrange(RelativeArrange.TopOrLeft);
        ctrlHeader.Property.SetHorzRelTo(HorzRelTo.Para);
        ctrlHeader.Property.SetHorzRelativeArrange(RelativeArrange.TopOrLeft);
        ctrlHeader.Property.SetVertRelToParaLimit(false);
        ctrlHeader.Property.SetAllowOverlap(false);
        ctrlHeader.Property.SetWidthCriterion(WidthCriterion.Absolute);
        ctrlHeader.Property.SetHeightCriterion(HeightCriterion.Absolute);
        ctrlHeader.Property.SetProtectSize(false);
        ctrlHeader.Property.SetTextFlowMethod(TextFlowMethod.FitWithText);
        ctrlHeader.Property.SetTextHorzArrange(TextHorzArrange.BothSides);
        ctrlHeader.Property.SetObjectNumberSort(ObjectNumberSort.Table);
        ctrlHeader.XOffset = (uint)MmToHwp(20.0);
        ctrlHeader.YOffset = (uint)MmToHwp(20.0);
        ctrlHeader.Width = (uint)MmToHwp(100.0);
        ctrlHeader.Height = (uint)MmToHwp(60.0);
        ctrlHeader.ZOrder = _zOrder++;
        ctrlHeader.OutterMarginLeft = 0;
        ctrlHeader.OutterMarginRight = 0;
        ctrlHeader.OutterMarginTop = 0;
        ctrlHeader.OutterMarginBottom = 0;
    }

    private static long MmToHwp(double mm)
    {
        return (long)(mm * 72000.0f / 254.0f + 0.5f);
    }

    private void SetTableRecordFor2By2Cells()
    {
        if (_table == null) return;
        
        var tableRecord = _table.Table;
        tableRecord.Property.DivideAtPageBoundary = DivideAtPageBoundary.DivideByCell;
        tableRecord.Property.AutoRepeatTitleRow = false;
        tableRecord.RowCount = 2;
        tableRecord.ColumnCount = 2;
        tableRecord.CellSpacing = 0;
        tableRecord.LeftInnerMargin = 0;
        tableRecord.RightInnerMargin = 0;
        tableRecord.TopInnerMargin = 0;
        tableRecord.BottomInnerMargin = 0;
        tableRecord.BorderFillId = GetBorderFillIdForTableOutterLine();
        tableRecord.AddCellCountOfRow(2);
        tableRecord.AddCellCountOfRow(2);
    }

    private int GetBorderFillIdForTableOutterLine()
    {
        var bf = _hwpFile!.DocInfo.AddNewBorderFill();
        bf.Property.Is3DEffect = false;
        bf.Property.IsShadowEffect = false;
        bf.Property.SlashDiagonalShape = SlashDiagonalShape.None;
        bf.Property.BackSlashDiagonalShape = BackSlashDiagonalShape.None;
        bf.LeftBorder.Type = BorderType.None;
        bf.LeftBorder.Thickness = BorderThickness.MM0_5;
        bf.LeftBorder.Color.Value = 0x0;
        bf.RightBorder.Type = BorderType.None;
        bf.RightBorder.Thickness = BorderThickness.MM0_5;
        bf.RightBorder.Color.Value = 0x0;
        bf.TopBorder.Type = BorderType.None;
        bf.TopBorder.Thickness = BorderThickness.MM0_5;
        bf.TopBorder.Color.Value = 0x0;
        bf.BottomBorder.Type = BorderType.None;
        bf.BottomBorder.Thickness = BorderThickness.MM0_5;
        bf.BottomBorder.Color.Value = 0x0;
        bf.DiagonalBorder.Type = BorderType.None;
        bf.DiagonalBorder.Thickness = BorderThickness.MM0_5;
        bf.DiagonalBorder.Color.Value = 0x0;

        bf.FillInfo.Type.HasPatternFill = true;
        bf.FillInfo.CreatePatternFill();
        var pf = bf.FillInfo.PatternFill!;
        pf.PatternType = PatternType.None;
        pf.BackColor.Value = unchecked((uint)-1);
        pf.PatternColor.Value = 0;

        return _hwpFile.DocInfo.BorderFillList.Count;
    }

    private void Add2By2Cell()
    {
        _borderFillIdForCell = CreateBorderFillIdForCell();

        AddFirstRow();
        AddSecondRow();
    }

    private int CreateBorderFillIdForCell()
    {
        var bf = _hwpFile!.DocInfo.AddNewBorderFill();
        bf.Property.Is3DEffect = false;
        bf.Property.IsShadowEffect = false;
        bf.Property.SlashDiagonalShape = SlashDiagonalShape.None;
        bf.Property.BackSlashDiagonalShape = BackSlashDiagonalShape.None;
        bf.LeftBorder.Type = BorderType.Solid;
        bf.LeftBorder.Thickness = BorderThickness.MM0_5;
        bf.LeftBorder.Color.Value = 0x0;
        bf.RightBorder.Type = BorderType.Solid;
        bf.RightBorder.Thickness = BorderThickness.MM0_5;
        bf.RightBorder.Color.Value = 0x0;
        bf.TopBorder.Type = BorderType.Solid;
        bf.TopBorder.Thickness = BorderThickness.MM0_5;
        bf.TopBorder.Color.Value = 0x0;
        bf.BottomBorder.Type = BorderType.Solid;
        bf.BottomBorder.Thickness = BorderThickness.MM0_5;
        bf.BottomBorder.Color.Value = 0x0;
        bf.DiagonalBorder.Type = BorderType.None;
        bf.DiagonalBorder.Thickness = BorderThickness.MM0_5;
        bf.DiagonalBorder.Color.Value = 0x0;

        bf.FillInfo.Type.HasPatternFill = true;
        bf.FillInfo.CreatePatternFill();
        var pf = bf.FillInfo.PatternFill!;
        pf.PatternType = PatternType.None;
        pf.BackColor.Value = unchecked((uint)-1);
        pf.PatternColor.Value = 0;

        return _hwpFile.DocInfo.BorderFillList.Count;
    }

    private void AddFirstRow()
    {
        _row = _table?.AddNewRow();
        AddLeftTopCell();
        AddRightTopCell();
    }

    private void AddLeftTopCell()
    {
        _cell = _row?.AddNewCell();
        SetListHeaderForCell(0, 0, true);
        SetParagraphForCell();
    }

    private void SetListHeaderForCell(int colIndex, int rowIndex, bool insertImage)
    {
        if (_cell == null) return;
        
        var lh = _cell.ListHeader;
        lh.ParaCount = 1;
        lh.Property.TextDirection = TextDirection.Horizontal;
        lh.Property.LineChange = LineChange.Normal;
        lh.Property.TextVerticalAlignment = TextVerticalAlignment.Center;
        lh.Property.ProtectCell = false;
        lh.Property.EditableAtFormMode = false;
        lh.ColIndex = colIndex;
        lh.RowIndex = rowIndex;
        lh.ColSpan = 1;
        lh.RowSpan = 1;
        lh.Width = (uint)MmToHwp(50.0);
        lh.Height = (uint)MmToHwp(30.0);
        lh.LeftMargin = 0;
        lh.RightMargin = 0;
        lh.TopMargin = 0;
        lh.BottomMargin = 0;
        if (insertImage)
        {
            lh.BorderFillId = CreateBorderFillIdForCellWithImage(_binDataId);
        }
        else
        {
            lh.BorderFillId = _borderFillIdForCell;
        }
        lh.TextWidth = (uint)MmToHwp(50.0);
        lh.FieldName = "";
    }

    private int CreateBorderFillIdForCellWithImage(int binDataId)
    {
        var bf = _hwpFile!.DocInfo.AddNewBorderFill();
        bf.Property.Is3DEffect = false;
        bf.Property.IsShadowEffect = false;
        bf.Property.SlashDiagonalShape = SlashDiagonalShape.None;
        bf.Property.BackSlashDiagonalShape = BackSlashDiagonalShape.None;
        bf.LeftBorder.Type = BorderType.Solid;
        bf.LeftBorder.Thickness = BorderThickness.MM0_5;
        bf.LeftBorder.Color.Value = 0x0;
        bf.RightBorder.Type = BorderType.Solid;
        bf.RightBorder.Thickness = BorderThickness.MM0_5;
        bf.RightBorder.Color.Value = 0x0;
        bf.TopBorder.Type = BorderType.Solid;
        bf.TopBorder.Thickness = BorderThickness.MM0_5;
        bf.TopBorder.Color.Value = 0x0;
        bf.BottomBorder.Type = BorderType.Solid;
        bf.BottomBorder.Thickness = BorderThickness.MM0_5;
        bf.BottomBorder.Color.Value = 0x0;
        bf.DiagonalBorder.Type = BorderType.None;
        bf.DiagonalBorder.Thickness = BorderThickness.MM0_5;
        bf.DiagonalBorder.Color.Value = 0x0;

        bf.FillInfo.Type.HasImageFill = true;
        bf.FillInfo.CreateImageFill();
        var imgF = bf.FillInfo.ImageFill!;
        imgF.ImageFillType = ImageFillType.FitSize;
        imgF.PictureInfo.BinItemID = binDataId;

        return _hwpFile.DocInfo.BorderFillList.Count;
    }

    private void SetParagraphForCell()
    {
        if (_cell == null) return;
        
        var p = _cell.ParagraphList.AddNewParagraph();
        SetParaHeader(p);
        SetParaCharShape(p);
        SetParaLineSeg(p);
    }

    private void SetParaHeader(Paragraph p)
    {
        var ph = p.Header;
        ph.LastInList = true;
        // 셀의 문단 모양을 이미 만들어진 문단 모양으로 사용함
        ph.ParaShapeId = 1;
        // 셀의 스타일을이미 만들어진 스타일로 사용함
        ph.StyleId = 1;
        ph.DivideSort.IsDivideSection = false;
        ph.DivideSort.IsDivideMultiColumn = false;
        ph.DivideSort.IsDividePage = false;
        ph.DivideSort.IsDivideColumn = false;
        ph.CharShapeCount = 1;
        ph.RangeTagCount = 0;
        ph.LineAlignCount = 1;
        ph.InstanceID = 0;
        ph.IsMergedByTrack = 0;
    }

    private void SetParaCharShape(Paragraph p)
    {
        p.CreateCharShape();

        var pcs = p.CharShape;
        // 셀의 글자 모양을 이미 만들어진 글자 모양으로 사용함
        pcs?.AddParaCharShape(0, 1);
    }

    private void SetParaLineSeg(Paragraph p)
    {
        p.CreateLineSeg();

        var pls = p.LineSeg;
        var lsi = pls?.AddNewLineSegItem();
        if (lsi == null) return;

        lsi.TextStartPosition = 0;
        lsi.LineVerticalPosition = 0;
        lsi.LineHeight = PtToLineHeight(10.0);
        lsi.TextPartHeight = PtToLineHeight(10.0);
        lsi.DistanceBaseLineToLineVerticalPosition = PtToLineHeight(10.0 * 0.85);
        lsi.LineSpace = PtToLineHeight(3.0);
        lsi.StartPositionFromColumn = 0;
        lsi.SegmentWidth = (int)MmToHwp(50.0);
        lsi.Tag.IsFirstSegmentAtLine = true;
        lsi.Tag.IsLastSegmentAtLine = true;
    }

    private static int PtToLineHeight(double pt)
    {
        return (int)(pt * 100.0f);
    }

    private void AddRightTopCell()
    {
        _cell = _row?.AddNewCell();
        SetListHeaderForCell(1, 0, false);
        SetParagraphForCell();
    }

    private void AddSecondRow()
    {
        _row = _table?.AddNewRow();
        AddLeftBottomCell();
        AddRightBottomCell();
    }

    private void AddLeftBottomCell()
    {
        _cell = _row?.AddNewCell();
        SetListHeaderForCell(0, 1, false);
        SetParagraphForCell();
    }

    private void AddRightBottomCell()
    {
        _cell = _row?.AddNewCell();
        SetListHeaderForCell(1, 1, false);
        SetParagraphForCell();
    }
}
