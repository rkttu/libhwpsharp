using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.CharShape;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Reader;
using HwpLib.Writer;

namespace HwpLibSharp.Test;

/// <summary>
/// 문단 텍스트 변경 테스트
/// </summary>
[TestClass]
public class ChangingParagraphTextTest
{
    private const string Source1 = "안녕하세요.";
    private const string Target1 = "Hello.";
    private const string Source2 = "이것은 샘플입니다.";
    private const string Target2 = "This is Sample.";

    [TestMethod]
    public void ChangeParagraphText_ShouldSucceed()
    {
        // Arrange
        var filePath = TestHelper.GetSamplePath("changing-paragraph-text.hwp");
        var hwpFile = HWPReader.FromFile(filePath);
        
        Assert.IsNotNull(hwpFile);
        
        // Act
        Section s = hwpFile.BodyText.SectionList[0];
        int count = s.ParagraphCount;
        for (int index = 0; index < count; index++)
        {
            ChangeParagraphText(hwpFile.BodyText.SectionList[0].GetParagraph(index));
        }
        
        var writePath = TestHelper.GetResultPath("result-changing-paragraph-text.hwp");
        HWPWriter.ToFile(hwpFile, writePath);
        
        // Assert
        Assert.IsTrue(File.Exists(writePath), "문단 텍스트 변경 성공");
    }

    private static void ChangeParagraphText(Paragraph? paragraph)
    {
        if (paragraph?.Text == null) return;
        
        var newCharList = GetNewCharList(paragraph.Text.CharList.ToList());
        ChangeNewCharList(paragraph, newCharList);
        RemoveLineSeg(paragraph);
        RemoveCharShapeExceptFirstOne(paragraph);
    }

    private static List<HWPChar> GetNewCharList(List<HWPChar> oldList)
    {
        var newList = new List<HWPChar>();
        var listForText = new List<HWPChar>();
        
        foreach (var ch in oldList)
        {
            if (ch.Type == HWPCharType.Normal)
            {
                listForText.Add(ch);
            }
            else
            {
                if (listForText.Count > 0)
                {
                    var text = ToString(listForText);
                    listForText.Clear();
                    var newText = ChangeText(text);
                    
                    if (newText != null)
                    {
                        newList.AddRange(ToHwpCharList(newText));
                    }
                }
                newList.Add(ch);
            }
        }

        if (listForText.Count > 0)
        {
            var text = ToString(listForText);
            listForText.Clear();
            var newText = ChangeText(text);
            
            if (newText != null)
            {
                newList.AddRange(ToHwpCharList(newText));
            }
        }
        
        return newList;
    }

    private static string ToString(List<HWPChar> listForText)
    {
        var sb = new System.Text.StringBuilder();
        foreach (var ch in listForText)
        {
            var chn = (HWPCharNormal)ch;
            sb.Append(chn.Ch);
        }
        return sb.ToString();
    }

    private static string? ChangeText(string text)
    {
        if (Source1 == text)
        {
            return Target1;
        }
        else if (Source2 == text)
        {
            return Target2;
        }
        return null;
    }

    private static List<HWPChar> ToHwpCharList(string text)
    {
        var list = new List<HWPChar>();
        int count = text.Length;
        for (int index = 0; index < count; index++)
        {
            var chn = new HWPCharNormal();
            chn.Code = (short)char.ConvertToUtf32(text, index);
            list.Add(chn);
        }
        return list;
    }

    private static void ChangeNewCharList(Paragraph paragraph, List<HWPChar> newCharList)
    {
        // TODO: ParaText에 Clear() 및 SetCharList() 메서드 추가 필요
        // CharList는 IReadOnlyList이므로 직접 수정 불가
        // 임시 우회: 텍스트 삭제 후 AddString으로 다시 추가
        paragraph.DeleteText();
        paragraph.CreateText();
        
        if (paragraph.Text == null) return;
        
        // 새 문자 리스트의 내용을 문자열로 변환하여 추가
        var sb = new System.Text.StringBuilder();
        foreach (var ch in newCharList)
        {
            if (ch.Type == HWPCharType.Normal)
            {
                sb.Append(((HWPCharNormal)ch).Ch);
            }
        }
        if (sb.Length > 0)
        {
            paragraph.Text.AddString(sb.ToString());
        }
        
        paragraph.Header.CharacterCount = paragraph.Text.CharList.Count;
    }

    private static void RemoveLineSeg(Paragraph paragraph)
    {
        paragraph.DeleteLineSeg();
    }

    private static void RemoveCharShapeExceptFirstOne(Paragraph paragraph)
    {
        if (paragraph.CharShape == null) return;
        
        int size = paragraph.CharShape.PositionShapeIdPairList.Count;
        if (size > 1)
        {
            // PositionShapeIdPairList는 IReadOnlyList이므로 RemoveAt 직접 호출 불가
            // RemoveParaCharShape(pair)를 사용하여 객체로 제거해야 함
            var pairsToRemove = new List<CharPositionShapeIdPair>();
            for (int index = 1; index < size; index++)
            {
                pairsToRemove.Add(paragraph.CharShape.PositionShapeIdPairList[index]);
            }
            foreach (var pair in pairsToRemove)
            {
                paragraph.CharShape.RemoveParaCharShape(pair);
            }
            paragraph.Header.CharShapeCount = 1;
        }
    }
}
