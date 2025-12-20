using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Paragraph;

namespace HwpLib.Tool.ObjectFinder.FieldForm;

/// <summary>
/// 필드와 폼을 찾는 클래스
/// </summary>
public static class FieldFormFinder
{
    /// <summary>
    /// 모든 필드와 폼을 찾는다.
    /// </summary>
    /// <param name="hwpFile">한글 파일 객체</param>
    /// <returns>검색 결과</returns>
    public static Result FindAll(HWPFile hwpFile)
    {
        return Find(hwpFile, new Option(null, false, true, true, true, true));
    }

    /// <summary>
    /// 조건에 맞는 필드와 폼을 찾는다.
    /// </summary>
    /// <param name="hwpFile">한글 파일 객체</param>
    /// <param name="option">검색 옵션</param>
    /// <returns>검색 결과</returns>
    public static Result Find(HWPFile hwpFile, Option option)
    {
        var result = new Result();

        foreach (var section in hwpFile.BodyText.SectionList)
        {
            try
            {
                FindInParagraphList(section, result, option);
            }
            catch (StopFindException)
            {
                break;
            }
        }

        return result;
    }

    /// <summary>
    /// 문단 리스트에서 필드를 찾는다.
    /// </summary>
    /// <param name="paragraphList">문단 리스트</param>
    /// <param name="result">결과 객체</param>
    /// <param name="option">검색 옵션</param>
    public static void FindInParagraphList(IParagraphList paragraphList, Result result, Option option)
    {
        StopFindException? exception = null;

        if (option.NameToFind != null && option.OnlyFirst && result.Added)
        {
            throw new StopFindException();
        }

        if (option.FindField)
        {
            var resultInParagraphList = new List<FieldData>();
            try
            {
                GetFieldStartPosition(paragraphList, resultInParagraphList, option);
            }
            catch (StopFindException e)
            {
                exception = e;
            }
            foreach (var fieldData in resultInParagraphList)
            {
                GetFieldEndPosition(paragraphList, fieldData);
            }
            result.AddAllFieldData(resultInParagraphList);
        }

        if (exception != null)
        {
            throw exception;
        }

        foreach (var paragraph in paragraphList)
        {
            if (paragraph.ControlList != null)
            {
                ForControlInFieldForm.FindInControlList(paragraph.ControlList.ToList(), result, option);
            }
        }
    }

    private static void GetFieldStartPosition(IParagraphList paragraphList, List<FieldData> result, Option option)
    {
        int paraCount = paragraphList.ParagraphCount;
        for (int paraIndex = 0; paraIndex < paraCount; paraIndex++)
        {
            FindStartingField(paragraphList, paraIndex, result, option);
        }
    }

    private static void FindStartingField(IParagraphList paragraphList, int paraIndex, List<FieldData> results, Option option)
    {
        var p = paragraphList.GetParagraph(paraIndex);
        if (p.ControlList == null)
        {
            return;
        }

        int ctrlCount = p.ControlList.Count;
        for (int ctrlIndex = 0; ctrlIndex < ctrlCount; ctrlIndex++)
        {
            var c = p.ControlList[ctrlIndex];
            if (c.IsField)
            {
                var field = (ControlField)c;
                if (option.NameToFind != null)
                {
                    if (option.NameToFind.Equals(field.GetName()))
                    {
                        AddStartingField(paragraphList, paraIndex, results, p, ctrlIndex, field);
                        if (option.OnlyFirst)
                        {
                            throw new StopFindException();
                        }
                    }
                }
                else
                {
                    AddStartingField(paragraphList, paraIndex, results, p, ctrlIndex, field);
                }
            }
        }
    }

    private static void AddStartingField(IParagraphList paragraphList, int paraIndex, List<FieldData> results, Paragraph p, int ctrlIndex, ControlField field)
    {
        var fieldData = new FieldData(
            field.GetName() ?? string.Empty,
            (field.Type == ControlType.FIELD_CLICKHERE) ? FieldType.ClickHere : FieldType.ETC,
            null,
            paragraphList);
        fieldData.SetStartPosition(paraIndex, p.Text!.GetCharIndexFromExtendCharIndex(ctrlIndex));

        results.Add(fieldData);
    }

    private static void GetFieldEndPosition(IParagraphList paragraphList, FieldData fieldData)
    {
        int depth = 0;
        int paraCount = paragraphList.ParagraphCount;
        for (int paraIndex = fieldData.StartParaIndex; paraIndex < paraCount; paraIndex++)
        {
            var p = paragraphList.GetParagraph(paraIndex);
            if (p.Text != null)
            {
                int startIndex = (paraIndex == fieldData.StartParaIndex) ? fieldData.StartCharIndex + 1 : 0;
                int charCount = p.Text.CharList.Count;
                for (int charIndex = startIndex; charIndex < charCount; charIndex++)
                {
                    var hwpChar = p.Text.CharList[charIndex];
                    if (hwpChar.Code == 0x3 /* field start */)
                    {
                        depth++;
                    }
                    else if (hwpChar.Code == 0x4 /* field end */)
                    {
                        if (depth == 0)
                        {
                            fieldData.SetEndPosition(paraIndex, charIndex);
                            return;
                        }
                        else
                        {
                            depth--;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 검색 결과
    /// </summary>
    public class Result
    {
        private readonly List<FieldData> _fieldDataList;
        private readonly List<FormData> _formDataList;

        public Result()
        {
            _fieldDataList = new List<FieldData>();
            _formDataList = new List<FormData>();
            Added = false;
        }

        public void AddFieldData(FieldData? fieldData)
        {
            if (fieldData != null)
            {
                _fieldDataList.Add(fieldData);
                Added = true;
            }
        }

        public void AddAllFieldData(List<FieldData> fieldDataList)
        {
            _fieldDataList.AddRange(fieldDataList);
            if (fieldDataList.Count > 0)
            {
                Added = true;
            }
        }

        public List<FieldData> FieldDataList => _fieldDataList;

        public void AddFormData(FormData? formData)
        {
            if (formData != null)
            {
                _formDataList.Add(formData);
                Added = true;
            }
        }

        public List<FormData> FormDataList => _formDataList;

        public bool Added { get; private set; }
    }

    /// <summary>
    /// 검색 옵션
    /// </summary>
    public class Option
    {
        public string? NameToFind { get; set; }
        public bool OnlyFirst { get; set; }
        public bool FindField { get; set; }
        public bool FindGso { get; set; }
        public bool FindCell { get; set; }
        public bool FindForm { get; set; }

        public Option(string? nameToFind, bool onlyFirst, bool findField, bool findGso, bool findCell, bool findForm)
        {
            NameToFind = nameToFind;
            OnlyFirst = onlyFirst;
            FindField = findField;
            FindGso = findGso;
            FindCell = findCell;
            FindForm = findForm;
        }
    }

    /// <summary>
    /// 검색 중단 예외
    /// </summary>
    public class StopFindException : Exception
    {
    }
}
