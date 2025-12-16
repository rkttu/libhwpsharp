using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent;
using HwpLib.Object.DocInfo.BorderFill.FillInfo;
using HwpLib.Tool.ParagraphAdder.DocInfo;

namespace HwpLib.Tool.ParagraphAdder.Control
{
    /// <summary>
    /// 그리기 개체의 공통 부분을 복사하는 클래스
    /// </summary>
    public class GsoCommonPartCopier
    {
        public static void Copy(GsoControl source, GsoControl target, DocInfoAdder? docInfoAdder)
        {
            // in container == null
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }

            CtrlDataCopier.Copy(source, target, docInfoAdder);
            CopyCaption(source, target, docInfoAdder);

            if (source.GsoType == GsoControlType.Container)
            {
                var sourceComp = source.ShapeComponent as ShapeComponentContainer;
                var targetComp = target.ShapeComponent as ShapeComponentContainer;
                if (sourceComp != null)
                {
                    targetComp?.Copy(sourceComp);
                }
            }
            else
            {
                var sourceComp = source.ShapeComponent as ShapeComponentNormal;
                var targetComp = target.ShapeComponent as ShapeComponentNormal;
                CopyShapeComponentNormal(sourceComp, targetComp, docInfoAdder);
            }
        }

        private static void CopyCaption(GsoControl source, GsoControl target, DocInfoAdder? docInfoAdder)
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

        private static void CopyShapeComponentNormal(ShapeComponentNormal? source, ShapeComponentNormal? target, DocInfoAdder? docInfoAdder)
        {
            if (source == null || target == null) return;

            target.Copy(source);

            if (source.LineInfo != null)
            {
                target.CreateLineInfo();
                target.LineInfo?.Copy(source.LineInfo);
            }

            if (source.FillInfo != null)
            {
                target.CreateFillInfo();
                CopyFillInfo(source.FillInfo!, target.FillInfo!, docInfoAdder);
            }

            if (source.ShadowInfo != null)
            {
                target.CreateShadowInfo();
                target.ShadowInfo?.Copy(source.ShadowInfo);
            }
        }

        private static void CopyFillInfo(FillInfo source, FillInfo target, DocInfoAdder? docInfoAdder)
        {
            if (source.Type != null && target.Type != null)
            {
                target.Type.Value = source.Type.Value;
            }

            if (source.Type?.HasPatternFill == true && source.PatternFill != null)
            {
                target.CreatePatternFill();
                target.PatternFill?.Copy(source.PatternFill);
            }

            if (source.Type?.HasGradientFill == true && source.GradientFill != null)
            {
                target.CreateGradientFill();
                target.GradientFill?.Copy(source.GradientFill);
            }

            if (source.Type?.HasImageFill == true)
            {
                target.CreateImageFill();
                var sourceIF = source.ImageFill;
                var targetIF = target.ImageFill;

                if (sourceIF != null && targetIF != null)
                {
                    targetIF.ImageFillType = sourceIF.ImageFillType;
                    CopyPictureInfo(sourceIF.PictureInfo, targetIF.PictureInfo, docInfoAdder);
                }
            }
        }

        public static void CopyPictureInfo(PictureInfo? source, PictureInfo? target, DocInfoAdder? docInfoAdder)
        {
            if (source == null || target == null) return;

            target.Brightness = source.Brightness;
            target.Contrast = source.Contrast;
            target.Effect = source.Effect;
            target.BinItemID = docInfoAdder == null ? source.BinItemID : docInfoAdder.ForBinData().ProcessById(source.BinItemID);
        }
    }
}
