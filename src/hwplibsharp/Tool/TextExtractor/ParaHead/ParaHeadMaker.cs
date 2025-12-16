using HwpLib.Object;
using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.Etc;
using HwpLib.Object.DocInfo;
using HwpLib.Object.DocInfo.Numbering;
using HwpLib.Object.DocInfo.ParaShape;
using HwpLib.Util;

namespace HwpLib.Tool.TextExtractor.ParaHead
{
    /// <summary>
    /// 문단 머리 생성 클래스
    /// </summary>
    public class ParaHeadMaker
    {
        private readonly HWPFile _hwpFile;
        private ControlSectionDefine? _sectionDefine;
        private ParaNumber _paraNumberForNumbering;
        private ParaNumber? _paraNumberForOutline;
        private NumberingInfo _defaultNumbering = new NumberingInfo();

        public ParaHeadMaker(HWPFile hwpFile)
        {
            _hwpFile = hwpFile;
            MakeDefaultNumbering();
            var sections = hwpFile.BodyText?.SectionList;
            if (sections != null && sections.Count > 0)
            {
                SetSectionDefine(sections[0]);
            }
            _paraNumberForNumbering = new ParaNumber();
        }

        private void MakeDefaultNumbering()
        {
            _defaultNumbering.StartNumber = 0;

            var levelList = _defaultNumbering.LevelNumberingList;
            if (levelList != null)
            {
                for (int i = 0; i < 10 && i < levelList.Count; i++)
                {
                    var lv = levelList[i];
                    lv.StartNumber = 0;
                    if (lv.ParagraphHeadInfo?.Property != null)
                    {
                        lv.ParagraphHeadInfo.Property.ParagraphNumberFormat = ParagraphNumberFormat.Number;
                    }
                    
                    string format = i == 0 ? "" : GetDefaultFormat(i + 1);
                    lv.NumberFormat?.FromUTF16LEString(format);
                }
            }
        }

        private string GetDefaultFormat(int level)
        {
            var parts = new List<string>();
            for (int i = 2; i <= level; i++)
            {
                parts.Add($"^{i}");
            }
            return string.Join(".", parts) + ".";
        }

        public void StartSection(Section section)
        {
            SetSectionDefine(section);
            _paraNumberForOutline = new ParaNumber();
        }

        public void EndSection()
        {
            _paraNumberForOutline = null;
        }

        private void SetSectionDefine(Section section)
        {
            if (section.ParagraphCount > 0)
            {
                var para = section.GetParagraph(0);
                var controlList = para?.ControlList;
                if (controlList != null && controlList.Count > 0)
                {
                    var firstControl = controlList[0];
                    if (firstControl.Type == ControlType.SectionDefine)
                    {
                        _sectionDefine = (ControlSectionDefine)firstControl;
                    }
                    else if (controlList.Count >= 2)
                    {
                        var secondControl = controlList[1];
                        if (secondControl.Type == ControlType.SectionDefine)
                        {
                            _sectionDefine = (ControlSectionDefine)secondControl;
                        }
                    }
                }
            }
        }

        public string? ParaHeadString(Paragraph paragraph)
        {
            var paraShapeList = _hwpFile.DocInfo?.ParaShapeList;
            var paraShapeId = paragraph.Header?.ParaShapeId ?? 0;
            if (paraShapeList == null || paraShapeId < 0 || paraShapeId >= paraShapeList.Count)
            {
                return "";
            }

            var paraShape = paraShapeList[paraShapeId];
            var headShape = paraShape.Property1?.ParaHeadShape;
            var paraLevel = paraShape.Property1?.ParaLevel ?? 0;

            switch (headShape)
            {
                case ParaHeadShape.None:
                    return "";
                case ParaHeadShape.Outline:
                    return Outline(paragraph.Header?.StyleId ?? 0, (byte)paraLevel);
                case ParaHeadShape.Numbering:
                    return NumberingText(paraShape.ParaHeadId, (byte)paraLevel);
                case ParaHeadShape.Bullet:
                    return BulletText(paraShape.ParaHeadId, (byte)paraLevel);
            }
            return null;
        }

        private string? Outline(int styleID, byte paraLevel)
        {
            var styleList = _hwpFile.DocInfo?.StyleList;
            if (styleList == null || styleID < 0 || styleID >= styleList.Count)
            {
                return null;
            }

            var style = styleList[styleID];
            var paraShapeList = _hwpFile.DocInfo?.ParaShapeList;
            var paraShapeId = style.ParaShapeId;
            if (paraShapeList == null || paraShapeId < 0 || paraShapeId >= paraShapeList.Count)
            {
                return null;
            }

            var outlineParaShapeInfo = paraShapeList[paraShapeId];
            var numbering = GetNumbering(outlineParaShapeInfo.ParaHeadId);

            LevelNumbering? lv = null;
            try
            {
                lv = numbering?.GetLevelNumbering(paraLevel + 1);
            }
            catch { }

            if (lv != null && _paraNumberForOutline != null)
            {
                if (_paraNumberForOutline.ChangedParaHead(outlineParaShapeInfo.ParaHeadId))
                {
                    _paraNumberForOutline.Reset(outlineParaShapeInfo.ParaHeadId, paraLevel, (int)lv.StartNumber);
                }
                else
                {
                    _paraNumberForOutline.Increase(paraLevel);
                }

                return NumberText(lv, _paraNumberForOutline, paraLevel);
            }
            return null;
        }

        private NumberingInfo? GetNumbering(int paraHeadId)
        {
            if (paraHeadId == 0) return _defaultNumbering;
            var list = _hwpFile.DocInfo?.NumberingList;
            if (list == null || paraHeadId - 1 < 0 || paraHeadId - 1 >= list.Count)
            {
                return _defaultNumbering;
            }
            return list[paraHeadId - 1];
        }

        private string? NumberingText(int paraHeadID, byte paraLevel)
        {
            var numbering = GetNumbering(paraHeadID);

            LevelNumbering? lv = null;
            try
            {
                lv = numbering?.GetLevelNumbering(paraLevel + 1);
            }
            catch { }

            if (lv != null)
            {
                if (_paraNumberForNumbering.ChangedParaHead(paraHeadID))
                {
                    _paraNumberForNumbering.Reset(paraHeadID, paraLevel, (int)lv.StartNumber);
                }
                else
                {
                    _paraNumberForNumbering.Increase(paraLevel);
                }

                return NumberText(lv, _paraNumberForNumbering, paraLevel);
            }
            return null;
        }

        private string? NumberText(LevelNumbering lv, ParaNumber paraNumber, int paraLevel)
        {
            var format = lv.NumberFormat?.ToUTF16LEString();
            if (format == null) return null;

            var tokens = new string[10];
            var values = new string[10];
            var headFormat = lv.ParagraphHeadInfo?.Property?.ParagraphNumberFormat ?? ParagraphNumberFormat.Number;

            for (int level = 0; level <= paraLevel; level++)
            {
                tokens[level] = $"^{level + 1}";
                values[level] = ParaHeadNumber.ToString(paraNumber.Value(level), headFormat);
            }

            return StringUtil.ReplaceEach(format, tokens, values);
        }

        private string? BulletText(int paraHeadId, byte paraLevel)
        {
            if (paraHeadId > 0)
            {
                var bulletList = _hwpFile.DocInfo?.BulletList;
                if (bulletList != null && paraHeadId - 1 >= 0 && paraHeadId - 1 < bulletList.Count)
                {
                    var bullet = bulletList[paraHeadId - 1];
                    return bullet.BulletChar?.ToUTF16LEString();
                }
            }
            return "●";
        }
    }
}
