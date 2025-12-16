using HwpLib.Object;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.CharShape;
using HwpLib.Object.BodyText.Paragraph.Header;
using HwpLib.Object.BodyText.Paragraph.LineSeg;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Object.DocInfo;
using HwpLib.Object.DocInfo.CharShape;
using HwpLib.Object.DocInfo.FaceName;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 새로운 글자 모양을 추가하고 문단에 글자 모양을 설정하는 테스트
/// </summary>
[TestClass]
public class InsertingCharShapeTest
{
    private HWPFile? _hwpFile;
    private int _charShapeIndexForNormal;
    private int _charShapeIndexForBold;
    private int _faceNameIndexForBatang;

    [TestMethod]
    public void InsertCharShape_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("blank.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        Test(hwpFile);
        
        var writePath = TestHelper.GetResultPath("result-inserting-charshape.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "글자 모양 삽입 성공");
    }

    private void Test(HWPFile hwpFile)
    {
        _hwpFile = hwpFile;

        _faceNameIndexForBatang = CreateFaceNameForBatang();
        _charShapeIndexForNormal = CreateCharShape(false);
        _charShapeIndexForBold = CreateCharShape(true);

        CreateTestParagraph();
    }

    /// <summary>
    /// 바탕 폰트를 위한 FaceName 객체를 생성한다.
    /// </summary>
    private int CreateFaceNameForBatang()
    {
        FaceNameInfo fn;

        // 한글 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile!.DocInfo.AddNewHangulFaceName();
        SetFaceNameForBatang(fn);

        // 영어 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewEnglishFaceName();
        SetFaceNameForBatang(fn);

        // 한자 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewHanjaFaceName();
        SetFaceNameForBatang(fn);

        // 일본어 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewJapaneseFaceName();
        SetFaceNameForBatang(fn);

        // 기타 문자 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewEtcFaceName();
        SetFaceNameForBatang(fn);

        // 기호 문자 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewSymbolFaceName();
        SetFaceNameForBatang(fn);

        // 사용자 정의 문자 부분을 위한 FaceName 객체를 생성한다.
        fn = _hwpFile.DocInfo.AddNewUserFaceName();
        SetFaceNameForBatang(fn);

        return _hwpFile.DocInfo.HangulFaceNameList.Count - 1;
    }

    private static void SetFaceNameForBatang(FaceNameInfo fn)
    {
        string fontName = "바탕";
        fn.Property.HasBaseFont = false;
        fn.Property.HasFontInfo = false;
        fn.Property.HasSubstituteFont = false;
        fn.Name = fontName;
    }

    private int CreateCharShape(bool bold)
    {
        var cs = _hwpFile!.DocInfo.AddNewCharShape();
        // 바탕 폰트를 위한 FaceName 객체를 링크한다.
        cs.FaceNameIds.SetForAll(_faceNameIndexForBatang);

        cs.Ratios.SetForAll(100);
        cs.CharSpaces.SetForAll(0);
        cs.RelativeSizes.SetForAll(100);
        cs.CharOffsets.SetForAll(0);
        cs.BaseSize = TestHelper.PtToBaseSize(11);

        cs.Property.IsItalic = false;
        cs.Property.IsBold = bold;
        cs.Property.UnderLineSort = UnderLineSort.None;
        cs.Property.UnderLineShape = BorderType2.Solid;
        cs.Property.OutterLineSort = OutterLineSort.None;
        cs.Property.ShadowSort = ShadowSort.None;
        cs.Property.IsEmboss = false;
        cs.Property.IsEngrave = false;
        cs.Property.IsSuperScript = false;
        cs.Property.IsSubScript = false;
        cs.Property.IsStrikeLine = false;
        cs.Property.EmphasisSort = EmphasisSort.None;
        cs.Property.IsUsingSpaceAppropriateForFont = false;
        cs.Property.StrikeLineShape = BorderType2.Solid;
        cs.Property.IsKerning = false;

        cs.ShadowGap1 = 0;
        cs.ShadowGap2 = 0;
        cs.CharColor.Value = 0x00000000;
        cs.UnderLineColor.Value = 0x00000000;
        cs.ShadeColor.Value = 0xFFFFFFFF;
        cs.ShadowColor.Value = 0x00b2b2b2;
        cs.BorderFillId = 0;

        return _hwpFile.DocInfo.CharShapeList.Count - 1;
    }

    private void CreateTestParagraph()
    {
        var p = _hwpFile!.BodyText.SectionList[0].AddNewParagraph();
        if (p == null) return;
        
        SetParaHeader(p);
        SetParaText(p, "This is a Paragraph. Bold on. Bold off.");
        SetParaCharShape(p);
        SetParaLineSeg(p);
    }

    private static void SetParaHeader(Paragraph p)
    {
        var ph = p.Header;
        ph.LastInList = true;
        ph.ParaShapeId = 1;
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

    private static void SetParaText(Paragraph p, string text)
    {
        p.CreateText();
        var pt = p.Text;
        pt?.AddString(text);
    }

    private void SetParaCharShape(Paragraph p)
    {
        int paragraphStartPos = 0;
        int boldStartPos = 20;
        int boldEndPos = 28;

        p.CreateCharShape();

        var pcs = p.CharShape;
        pcs?.AddParaCharShape(paragraphStartPos, _charShapeIndexForNormal);
        pcs?.AddParaCharShape(boldStartPos, _charShapeIndexForBold);
        pcs?.AddParaCharShape(boldEndPos, _charShapeIndexForNormal);
    }

    private static void SetParaLineSeg(Paragraph p)
    {
        p.CreateLineSeg();

        var pls = p.LineSeg;
        var lsi = pls?.AddNewLineSegItem();
        if (lsi == null) return;

        lsi.TextStartPosition = 0;
        lsi.LineVerticalPosition = 0;
        lsi.LineHeight = TestHelper.PtToLineHeight(11.0);
        lsi.TextPartHeight = TestHelper.PtToLineHeight(11.0);
        lsi.DistanceBaseLineToLineVerticalPosition = TestHelper.PtToLineHeight(11.0 * 0.85);
        lsi.LineSpace = TestHelper.PtToLineHeight(4.0);
        lsi.StartPositionFromColumn = 0;
        lsi.SegmentWidth = (int)TestHelper.MmToHwp(50.0);
        lsi.Tag.IsFirstSegmentAtLine = true;
        lsi.Tag.IsLastSegmentAtLine = true;
    }
}
