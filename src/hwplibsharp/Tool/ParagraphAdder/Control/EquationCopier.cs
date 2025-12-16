using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Tool.ParagraphAdder.DocInfo;

namespace HwpLib.Tool.ParagraphAdder.Control
{
    /// <summary>
    /// 수식 컨트롤을 복사하는 클래스
    /// </summary>
    public class EquationCopier
    {
        public static void Copy(ControlEquation source, ControlEquation target, DocInfoAdder? docInfoAdder)
        {
            var sourceH = source.GetHeader();
            var targetH = target.GetHeader();
            if (sourceH != null)
            {
                targetH?.Copy(sourceH);
            }

            CtrlDataCopier.Copy(source, target, docInfoAdder);
            CopyCaption(source, target, docInfoAdder);

            var sourceEE = source.EQEdit;
            var targetEE = target.EQEdit;
            targetEE?.Copy(sourceEE);
        }

        private static void CopyCaption(ControlEquation source, ControlEquation target, DocInfoAdder? docInfoAdder)
        {
            if (source.Caption != null)
            {
                target.CreateCaption();
                CaptionCopier.Copy(source.Caption!, target.Caption!, docInfoAdder);
            }
            else
            {
                target.DeleteCaption();
            }
        }
    }
}
