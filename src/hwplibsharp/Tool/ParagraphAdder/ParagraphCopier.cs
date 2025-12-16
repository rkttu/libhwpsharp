using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.CharShape;
using HwpLib.Object.BodyText.Paragraph.Header;
using HwpLib.Object.BodyText.Paragraph.LineSeg;
using HwpLib.Object.BodyText.Paragraph.RangeTag;
using HwpLib.Tool.ParagraphAdder.Control;
using HwpLib.Tool.ParagraphAdder.DocInfo;
using System;
using Paragraph = HwpLib.Object.BodyText.Paragraph.Paragraph;

namespace HwpLib.Tool.ParagraphAdder
{
    /// <summary>
    /// Paragraph 객체를 복사하는 기능을 포함하는 클래스
    /// </summary>
    public class ParagraphCopier
    {
        private DocInfoAdder? _docInfoAdder;
        private Paragraph? _source;
        private Paragraph? _target;
        private bool _includingSectionInfo;
        private bool _excludedSectionDefine;

        public ParagraphCopier(DocInfoAdder? docInfoAdder)
        {
            _docInfoAdder = docInfoAdder;
        }

        /// <summary>
        /// 문단 리스트를 복사한다.
        /// </summary>
        public static void ListCopy(ParagraphList source, ParagraphList target, DocInfoAdder? docInfoAdder)
        {
            var copier = new ParagraphCopier(docInfoAdder);
            foreach (var p in source)
            {
                try
                {
                    copier.Copy(p, target.AddNewParagraph());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <summary>
        /// 문단을 복사한다.
        /// </summary>
        public void Copy(Paragraph source, Paragraph target)
        {
            _source = source;
            _target = target;
            _includingSectionInfo = false;

            CopyHeader();
            CopyText();
            CopyCharShapeInfo();
            CopyLineSeg();
            CopyRangeTag();
            CopyControlList();
            CopyMemoList();
        }

        /// <summary>
        /// 구역 정보를 포함하여 문단을 복사한다.
        /// </summary>
        public void CopyIncludingSectionInfo(Paragraph source, Paragraph target)
        {
            _source = source;
            _target = target;
            _includingSectionInfo = true;

            CopyHeader();
            CopyText();
            CopyCharShapeInfo();
            CopyLineSeg();
            CopyRangeTag();
            CopyControlList();
            CopyMemoList();
        }

        private void CopyHeader()
        {
            if (_source?.Header == null || _target == null) return;

            var sourceH = _source.Header;
            var targetH = _target.Header;

            if (sourceH == null || targetH == null) return;

            targetH.LastInList = sourceH.LastInList;
            targetH.CharacterCount = sourceH.CharacterCount;
            targetH.ControlMask.Value = sourceH.ControlMask.Value;
            targetH.ParaShapeId = _docInfoAdder == null ? sourceH.ParaShapeId : _docInfoAdder.ForParaShapeInfo().ProcessById(sourceH.ParaShapeId);
            targetH.StyleId = (short)(_docInfoAdder == null ? sourceH.StyleId : _docInfoAdder.ForStyle().ProcessById(sourceH.StyleId));
            targetH.DivideSort.Value = sourceH.DivideSort.Value;
            targetH.CharShapeCount = sourceH.CharShapeCount;
            targetH.RangeTagCount = sourceH.RangeTagCount;
            targetH.LineAlignCount = sourceH.LineAlignCount;
            targetH.InstanceID = 0;
            targetH.IsMergedByTrack = sourceH.IsMergedByTrack;
        }

        private void CopyText()
        {
            if (_source?.Text == null || _target == null) return;

            _target.CreateText();
            _excludedSectionDefine = ParaTextCopier.Copy(_source.Text!, _target.Text!, _includingSectionInfo);
        }

        private void CopyCharShapeInfo()
        {
            if (_source?.CharShape == null || _target == null) return;

            _target.CreateCharShape();

            foreach (var cpsp in _source.CharShape!.PositionShapeIdPairList)
            {
                if (_excludedSectionDefine && cpsp.Position > 0)
                {
                    _target.CharShape!.AddParaCharShape(
                        cpsp.Position - 8,
                        _docInfoAdder == null ? cpsp.ShapeId : _docInfoAdder.ForCharShapeInfo().ProcessById((int)cpsp.ShapeId));
                }
                else
                {
                    _target.CharShape!.AddParaCharShape(
                        cpsp.Position,
                        _docInfoAdder == null ? cpsp.ShapeId : _docInfoAdder.ForCharShapeInfo().ProcessById((int)cpsp.ShapeId));
                }
            }
        }

        private void CopyLineSeg()
        {
            if (_source?.LineSeg == null || _target == null) return;

            _target.CreateLineSeg();
            foreach (var lsi in _source.LineSeg!.LineSegItemList)
            {
                _target.LineSeg!.AddLineSegItem(lsi.Clone());
            }
        }

        private void CopyRangeTag()
        {
            if (_source?.RangeTag == null || _target == null) return;

            _target.CreateRangeTag();
            foreach (var rti in _source.RangeTag!.RangeTagItemList)
            {
                _target.RangeTag!.AddRangeTagItem(rti.Clone());
            }
        }

        private void CopyControlList()
        {
            if (_source?.ControlList == null || _target == null) return;

            foreach (var c in _source.ControlList!)
            {
                switch (c.Type)
                {
                    case ControlType.Table:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Table) as ControlTable;
                            if (targetControl != null)
                                TableCopier.Copy((ControlTable)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Gso:
                        {
                            var targetControl = _target.AddNewGsoControl(((GsoControl)c).GsoType);
                            if (targetControl != null)
                                GsoCopier.Copy((GsoControl)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Equation:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Equation) as ControlEquation;
                            if (targetControl != null)
                                EquationCopier.Copy((ControlEquation)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.SectionDefine:
                        if (_includingSectionInfo)
                        {
                            var targetControl = _target.AddNewControl(ControlType.SectionDefine) as ControlSectionDefine;
                            if (targetControl != null)
                                SectionDefineCopier.Copy((ControlSectionDefine)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.ColumnDefine:
                        {
                            var targetControl = _target.AddNewControl(ControlType.ColumnDefine) as ControlColumnDefine;
                            if (targetControl != null)
                                ETCControlCopier.CopyColumnDefine((ControlColumnDefine)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Header:
                        if (_includingSectionInfo)
                        {
                            var targetControl = _target.AddNewControl(ControlType.Header) as ControlHeader;
                            if (targetControl != null)
                                ETCControlCopier.CopyHeader((ControlHeader)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Footer:
                        if (_includingSectionInfo)
                        {
                            var targetControl = _target.AddNewControl(ControlType.Footer) as ControlFooter;
                            if (targetControl != null)
                                ETCControlCopier.CopyFooter((ControlFooter)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Footnote:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Footnote) as ControlFootnote;
                            if (targetControl != null)
                                ETCControlCopier.CopyFootnote((ControlFootnote)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Endnote:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Endnote) as ControlEndnote;
                            if (targetControl != null)
                                ETCControlCopier.CopyEndnote((ControlEndnote)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.AutoNumber:
                        {
                            var targetControl = _target.AddNewControl(ControlType.AutoNumber) as ControlAutoNumber;
                            if (targetControl != null)
                                ETCControlCopier.CopyAutoNumber((ControlAutoNumber)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.NewNumber:
                        {
                            var targetControl = _target.AddNewControl(ControlType.NewNumber) as ControlNewNumber;
                            if (targetControl != null)
                                ETCControlCopier.CopyNewNumber((ControlNewNumber)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.PageHide:
                        {
                            var targetControl = _target.AddNewControl(ControlType.PageHide) as ControlPageHide;
                            if (targetControl != null)
                                ETCControlCopier.CopyPageHide((ControlPageHide)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.PageOddEvenAdjust:
                        {
                            var targetControl = _target.AddNewControl(ControlType.PageOddEvenAdjust) as ControlPageOddEvenAdjust;
                            if (targetControl != null)
                                ETCControlCopier.CopyPageOddEvenAdjust((ControlPageOddEvenAdjust)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.PageNumberPosition:
                        {
                            var targetControl = _target.AddNewControl(ControlType.PageNumberPosition) as ControlPageNumberPosition;
                            if (targetControl != null)
                                ETCControlCopier.CopyPageNumberPosition((ControlPageNumberPosition)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.IndexMark:
                        {
                            var targetControl = _target.AddNewControl(ControlType.IndexMark) as ControlIndexMark;
                            if (targetControl != null)
                                ETCControlCopier.CopyIndexMark((ControlIndexMark)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Bookmark:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Bookmark) as ControlBookmark;
                            if (targetControl != null)
                                ETCControlCopier.CopyBookmark((ControlBookmark)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.OverlappingLetter:
                        {
                            var targetControl = _target.AddNewControl(ControlType.OverlappingLetter) as ControlOverlappingLetter;
                            if (targetControl != null)
                                OverlappingLetterCopier.Copy((ControlOverlappingLetter)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.AdditionalText:
                        {
                            var targetControl = _target.AddNewControl(ControlType.AdditionalText) as ControlAdditionalText;
                            if (targetControl != null)
                                AdditionalTextCopier.Copy((ControlAdditionalText)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.HiddenComment:
                        {
                            var targetControl = _target.AddNewControl(ControlType.HiddenComment) as ControlHiddenComment;
                            if (targetControl != null)
                                ETCControlCopier.CopyHiddenComment((ControlHiddenComment)c, targetControl, _docInfoAdder);
                        }
                        break;
                    case ControlType.Form:
                        {
                            var targetControl = _target.AddNewControl(ControlType.Form) as ControlForm;
                            if (targetControl != null)
                                ETCControlCopier.CopyForm((ControlForm)c, targetControl, _docInfoAdder);
                        }
                        break;
                    default:
                        // Field 컨트롤 처리
                        if (c.Type.IsField())
                        {
                            var sourceField = (ControlField)c;
                            var targetControl = _target.AddNewControl(sourceField.GetHeader()?.CtrlId ?? 0) as ControlField;
                            if (targetControl != null)
                                ETCControlCopier.CopyField(sourceField, targetControl, _docInfoAdder);
                        }
                        break;
                }
            }
        }

        private void CopyMemoList()
        {
            CopyMemoList(_source, _target, _docInfoAdder);
        }

        public static void CopyMemoList(Paragraph? source, Paragraph? target, DocInfoAdder? docInfoAdder)
        {
            // 메모 리스트 복사 - 추후 구현
        }
    }
}
