using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.DocInfo;
using HwpLib.Object.DocInfo.CompatibleDocument;
using HwpLib.Object.DocInfo.DocumentProperties;
using HwpLib.Object.FileHeader;

namespace HwpLib.Tool.BlankFileMaker;

/// <summary>
/// 빈 HWP 파일을 생성하는 클래스
/// </summary>
public static class BlankFileMaker
{
    /// <summary>
    /// 빈 HWP 파일을 생성한다.
    /// </summary>
    /// <returns>생성된 HWP 파일</returns>
    public static HWPFile Make()
    {
        var hwpFile = new HWPFile();
        SetFileHeader(hwpFile.FileHeader);

        var docInfo = hwpFile.DocInfo;
        SetDocumentProperties(docInfo.DocumentProperties);

        FaceNameInfoAdder.Add(docInfo);
        BorderFillInfoAdder.Add(docInfo);
        CharShapeInfoAdder.Add(docInfo);
        TabDefInfoAdder.Add(docInfo);
        NumberingAdder.Add(docInfo);
        ParaShapeInfoAdder.Add(docInfo);
        StyleAdder.Add(docInfo);
        CompatibleDocument(docInfo);
        LayoutCompatibility(docInfo);

        var section = hwpFile.BodyText.AddNewSection();
        EmptyParagraphAdder.Add(section);

        SetScript(hwpFile);
        return hwpFile;
    }

    private static void SetFileHeader(FileHeader fileHeader)
    {
        fileHeader.Version.SetVersion(5, 0, 3, 4);
        fileHeader.Compressed = true;
        fileHeader.HasPassword = false;
        fileHeader.IsDistribution = false;
        fileHeader.SaveScript = false;
        fileHeader.IsDrmDocument = false;
        fileHeader.HasXmlTemplate = false;
        fileHeader.HasDocumentHistory = false;
        fileHeader.HasSignature = false;
        fileHeader.EncryptPublicCertification = false;
        fileHeader.SavePrepareSignature = false;
        fileHeader.IsPublicCertificationDrmDocument = false;
        fileHeader.IsCclDocument = false;
    }

    private static void SetDocumentProperties(DocumentPropertiesInfo documentProperties)
    {
        documentProperties.SectionCount = 1;

        var startNumber = documentProperties.StartNumber;
        startNumber.Page = 1;
        startNumber.Footnote = 1;
        startNumber.Endnote = 1;
        startNumber.Picture = 1;
        startNumber.Table = 1;
        startNumber.Equation = 1;

        var caretPosition = documentProperties.CaretPosition;
        caretPosition.ListID = 0;
        caretPosition.ParagraphID = 0;
        caretPosition.PositionInParagraph = 0;
    }

    private static void CompatibleDocument(DocInfo docInfo)
    {
        docInfo.CreateCompatibleDocument();
        docInfo.CompatibleDocument!.TargetProgram = CompatibleDocumentSort.HWPCurrent;
    }

    private static void LayoutCompatibility(DocInfo docInfo)
    {
        docInfo.CreateLayoutCompatibility();
        var layoutCompatibility = docInfo.LayoutCompatibility!;
        layoutCompatibility.LetterLevelFormat = 0;
        layoutCompatibility.ParagraphLevelFormat = 0;
        layoutCompatibility.SectionLevelFormat = 0;
        layoutCompatibility.ObjectLevelFormat = 0;
        layoutCompatibility.FieldLevelFormat = 0;
    }

    private static void SetScript(HWPFile hwpFile)
    {
        byte[] compressedJsVersion = new byte[] { 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        hwpFile.Scripts.JScriptVersion = compressedJsVersion;

        byte[] compressedDefaultJScript = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF };
        hwpFile.Scripts.DefaultJScript = compressedDefaultJScript;
    }
}
