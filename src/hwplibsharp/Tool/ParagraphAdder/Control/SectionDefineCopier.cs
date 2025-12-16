using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.SectionDefine;
using HwpLib.Tool.ParagraphAdder.DocInfo;

namespace HwpLib.Tool.ParagraphAdder.Control
{
    /// <summary>
    /// 구역 정의 컨트롤을 복사하는 클래스
    /// </summary>
    public class SectionDefineCopier
    {
        public static void Copy(ControlSectionDefine source, ControlSectionDefine target, DocInfoAdder? docInfoAdder)
        {
            CopyHeader(source.Header, target.Header, docInfoAdder);
            CtrlDataCopier.Copy(source, target, docInfoAdder);

            target.PageDef?.Copy(source.PageDef);
            CopyFootEndNoteShape(source.FootNoteShape, target.FootNoteShape);
            CopyFootEndNoteShape(source.EndNoteShape, target.EndNoteShape);
            CopyPageBorderFill(source.BothPageBorderFill, target.BothPageBorderFill, docInfoAdder);
            CopyPageBorderFill(source.EvenPageBorderFill, target.EvenPageBorderFill, docInfoAdder);
            CopyPageBorderFill(source.OddPageBorderFill, target.OddPageBorderFill, docInfoAdder);

            foreach (var sourceBatangPageInfo in source.BatangPageInfoList)
            {
                CopyBatangPageInfo(sourceBatangPageInfo, target.AddNewBatangPageInfo(), docInfoAdder);
            }
        }

        private static void CopyHeader(HwpLib.Object.BodyText.Control.CtrlHeader.CtrlHeaderSectionDefine? source, 
                                       HwpLib.Object.BodyText.Control.CtrlHeader.CtrlHeaderSectionDefine? target, 
                                       DocInfoAdder? docInfoAdder)
        {
            if (source == null || target == null) return;

            if (source.Property != null && target.Property != null)
                target.Property.Value = source.Property.Value;
            target.ColumnGap = source.ColumnGap;
            target.VerticalLineAlign = source.VerticalLineAlign;
            target.HorizontalLineAlign = source.HorizontalLineAlign;
            target.DefaultTabGap = source.DefaultTabGap;
            target.NumberParaShapeId = docInfoAdder == null ? source.NumberParaShapeId : docInfoAdder.ForParaShapeInfo().ProcessById(source.NumberParaShapeId);
            target.PageStartNumber = source.PageStartNumber;
            target.ImageStartNumber = source.ImageStartNumber;
            target.TableStartNumber = source.TableStartNumber;
            target.EquationStartNumber = source.EquationStartNumber;
            target.DefaultLanguage = source.DefaultLanguage;
        }

        private static void CopyFootEndNoteShape(FootEndNoteShape? source, FootEndNoteShape? target)
        {
            if (source != null)
            {
                target?.Copy(source);
            }
        }

        private static void CopyPageBorderFill(PageBorderFill? source, PageBorderFill? target, DocInfoAdder? docInfoAdder)
        {
            if (source == null || target == null) return;

            if (source.Property != null && target.Property != null)
                target.Property.Value = source.Property.Value;
            target.LeftGap = source.LeftGap;
            target.RightGap = source.RightGap;
            target.TopGap = source.TopGap;
            target.BottomGap = source.BottomGap;

            if (source.BorderFillId == 0)
            {
                target.BorderFillId = 0;
            }
            else
            {
                target.BorderFillId = docInfoAdder == null ? source.BorderFillId : docInfoAdder.ForBorderFillInfo().ProcessById(source.BorderFillId);
            }
        }

        private static void CopyBatangPageInfo(BatangPageInfo source, BatangPageInfo target, DocInfoAdder? docInfoAdder)
        {
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }
    }
}
