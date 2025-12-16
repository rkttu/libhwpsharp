using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Picture;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponentEach.Polygon;
using HwpLib.Object.BodyText.Control.Gso.TextBox;
using HwpLib.Tool.ParagraphAdder.DocInfo;

namespace HwpLib.Tool.ParagraphAdder.Control
{
    /// <summary>
    /// 그리기 개체를 복사하는 클래스
    /// </summary>
    public class GsoCopier
    {
        public static void Copy(GsoControl source, GsoControl target, DocInfoAdder? docInfoAdder)
        {
            GsoCommonPartCopier.Copy(source, target, docInfoAdder);

            switch (source.GsoType)
            {
                case GsoControlType.Line:
                    CopyLine((ControlLine)source, (ControlLine)target, docInfoAdder);
                    break;
                case GsoControlType.Rectangle:
                    CopyRectangle((ControlRectangle)source, (ControlRectangle)target, docInfoAdder);
                    break;
                case GsoControlType.Ellipse:
                    CopyEllipse((ControlEllipse)source, (ControlEllipse)target, docInfoAdder);
                    break;
                case GsoControlType.Arc:
                    CopyArc((ControlArc)source, (ControlArc)target, docInfoAdder);
                    break;
                case GsoControlType.Polygon:
                    CopyPolygon((ControlPolygon)source, (ControlPolygon)target, docInfoAdder);
                    break;
                case GsoControlType.Curve:
                    CopyCurve((ControlCurve)source, (ControlCurve)target, docInfoAdder);
                    break;
                case GsoControlType.Picture:
                    CopyPicture((ControlPicture)source, (ControlPicture)target, docInfoAdder);
                    break;
                case GsoControlType.OLE:
                    CopyOLE((ControlOLE)source, (ControlOLE)target, docInfoAdder);
                    break;
                case GsoControlType.Container:
                    CopyContainer((ControlContainer)source, (ControlContainer)target, docInfoAdder);
                    break;
                case GsoControlType.ObjectLinkLine:
                    CopyObjectLinkLine((ControlObjectLinkLine)source, (ControlObjectLinkLine)target, docInfoAdder);
                    break;
                case GsoControlType.TextArt:
                    CopyTextArt((ControlTextArt)source, (ControlTextArt)target, docInfoAdder);
                    break;
            }
        }

        private static void CopyLine(ControlLine source, ControlLine target, DocInfoAdder? docInfoAdder)
        {
            target.ShapeComponentLine?.Copy(source.ShapeComponentLine);
        }

        private static void CopyRectangle(ControlRectangle source, ControlRectangle target, DocInfoAdder? docInfoAdder)
        {
            if (source.TextBox != null)
            {
                target.CreateTextBox();
                CopyTextBox(source.TextBox!, target.TextBox!, docInfoAdder);
            }

            target.ShapeComponentRectangle?.Copy(source.ShapeComponentRectangle);
        }

        private static void CopyTextBox(TextBox source, TextBox target, DocInfoAdder? docInfoAdder)
        {
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        private static void CopyEllipse(ControlEllipse source, ControlEllipse target, DocInfoAdder? docInfoAdder)
        {
            if (source.TextBox != null)
            {
                target.CreateTextBox();
                CopyTextBox(source.TextBox!, target.TextBox!, docInfoAdder);
            }

            target.ShapeComponentEllipse?.Copy(source.ShapeComponentEllipse);
        }

        private static void CopyArc(ControlArc source, ControlArc target, DocInfoAdder? docInfoAdder)
        {
            if (source.TextBox != null)
            {
                target.CreateTextBox();
                CopyTextBox(source.TextBox!, target.TextBox!, docInfoAdder);
            }

            target.ShapeComponentArc?.Copy(source.ShapeComponentArc);
        }

        private static void CopyPolygon(ControlPolygon source, ControlPolygon target, DocInfoAdder? docInfoAdder)
        {
            if (source.TextBox != null)
            {
                target.CreateTextBox();
                CopyTextBox(source.TextBox!, target.TextBox!, docInfoAdder);
            }

            target.ShapeComponentPolygon?.Copy(source.ShapeComponentPolygon);
        }

        private static void CopyCurve(ControlCurve source, ControlCurve target, DocInfoAdder? docInfoAdder)
        {
            if (source.TextBox != null)
            {
                target.CreateTextBox();
                CopyTextBox(source.TextBox!, target.TextBox!, docInfoAdder);
            }

            target.ShapeComponentCurve?.Copy(source.ShapeComponentCurve);
        }

        private static void CopyPicture(ControlPicture source, ControlPicture target, DocInfoAdder? docInfoAdder)
        {
            var sourceSCP = source.ShapeComponentPicture;
            var targetSCP = target.ShapeComponentPicture;

            if (sourceSCP == null || targetSCP == null) return;

            if (sourceSCP.BorderColor != null && targetSCP.BorderColor != null)
                targetSCP.BorderColor.Value = sourceSCP.BorderColor.Value;
            targetSCP.BorderThickness = sourceSCP.BorderThickness;
            if (sourceSCP.BorderProperty != null && targetSCP.BorderProperty != null)
                targetSCP.BorderProperty.Value = sourceSCP.BorderProperty.Value;
            targetSCP.LeftTop?.Copy(sourceSCP.LeftTop);
            targetSCP.RightTop?.Copy(sourceSCP.RightTop);
            targetSCP.LeftBottom?.Copy(sourceSCP.LeftBottom);
            targetSCP.RightBottom?.Copy(sourceSCP.RightBottom);
            targetSCP.LeftAfterCutting = sourceSCP.LeftAfterCutting;
            targetSCP.TopAfterCutting = sourceSCP.TopAfterCutting;
            targetSCP.RightAfterCutting = sourceSCP.RightAfterCutting;
            targetSCP.BottomAfterCutting = sourceSCP.BottomAfterCutting;
            targetSCP.InnerMargin?.Copy(sourceSCP.InnerMargin);
            GsoCommonPartCopier.CopyPictureInfo(sourceSCP.PictureInfo, targetSCP.PictureInfo, docInfoAdder);
            targetSCP.BorderTransparency = sourceSCP.BorderTransparency;
            targetSCP.InstanceId = sourceSCP.InstanceId;
            targetSCP.PictureEffect?.Copy(sourceSCP.PictureEffect);
            targetSCP.ImageWidth = sourceSCP.ImageWidth;
            targetSCP.ImageHeight = sourceSCP.ImageHeight;
        }

        private static void CopyOLE(ControlOLE source, ControlOLE target, DocInfoAdder? docInfoAdder)
        {
            var sourceSCO = source.ShapeComponentOLE;
            var targetSCO = target.ShapeComponentOLE;

            if (sourceSCO == null || targetSCO == null) return;

            if (sourceSCO.Property != null && targetSCO.Property != null)
                targetSCO.Property.Value = sourceSCO.Property.Value;
            targetSCO.ExtentWidth = sourceSCO.ExtentWidth;
            targetSCO.ExtentHeight = sourceSCO.ExtentHeight;
            targetSCO.BinDataId = docInfoAdder == null ? sourceSCO.BinDataId : docInfoAdder.ForBinData().ProcessById(sourceSCO.BinDataId);
            if (sourceSCO.BorderColor != null && targetSCO.BorderColor != null)
                targetSCO.BorderColor.Value = sourceSCO.BorderColor.Value;
            targetSCO.BorderThickness = sourceSCO.BorderThickness;
            if (sourceSCO.BorderProperty != null && targetSCO.BorderProperty != null)
                targetSCO.BorderProperty.Value = sourceSCO.BorderProperty.Value;
            targetSCO.Unknown = sourceSCO.Unknown;
        }

        private static void CopyContainer(ControlContainer source, ControlContainer target, DocInfoAdder? docInfoAdder)
        {
            foreach (var sourceChild in source.ChildControlList)
            {
                var targetChild = target.AddNewChildControl(sourceChild.GsoType);
                if (targetChild != null)
                {
                    Copy(sourceChild, targetChild, docInfoAdder);
                }
            }
        }

        private static void CopyObjectLinkLine(ControlObjectLinkLine source, ControlObjectLinkLine target, DocInfoAdder? docInfoAdder)
        {
            target.ShapeComponentLine?.Copy(source.ShapeComponentLine);
        }

        private static void CopyTextArt(ControlTextArt source, ControlTextArt target, DocInfoAdder? docInfoAdder)
        {
            target.ShapeComponentTextArt?.Copy(source.ShapeComponentTextArt);
        }
    }
}
