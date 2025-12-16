using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Tool.ParagraphAdder.DocInfo;
using System.Text;

namespace HwpLib.Tool.ParagraphAdder.Control
{
    /// <summary>
    /// 기타 컨트롤을 복사하는 클래스
    /// </summary>
    public class ETCControlCopier
    {
        public static void CopyAutoNumber(ControlAutoNumber source, ControlAutoNumber target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyColumnDefine(ControlColumnDefine source, ControlColumnDefine target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyEndnote(ControlEndnote source, ControlEndnote target, DocInfoAdder? docInfoAdder)
        {
            target.Header.Copy(source.Header);
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        public static void CopyField(ControlField source, ControlField target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);

            if (IsMemo(source) && docInfoAdder != null)
            {
                var sourceFieldHeader = source.GetHeader();
                if (sourceFieldHeader != null)
                {
                    var sourceMemo = GetSourceMemo(sourceFieldHeader.MemoIndex, docInfoAdder);
                    if (sourceMemo != null)
                    {
                        long newMemoIndex = AddMemoToTarget(sourceMemo, docInfoAdder);
                        var targetFieldHeader = target.GetHeader();
                        if (targetFieldHeader != null)
                        {
                            SetNewMemoIndex(targetFieldHeader, newMemoIndex);
                        }
                    }
                }
            }
        }

        private static bool IsMemo(ControlField source)
        {
            var command = source.GetHeader()?.Command;
            if (command?.Bytes == null) return false;
            return command.ToUTF16LEString()?.StartsWith("MEMO") == true;
        }

        private static HwpLib.Object.BodyText.Paragraph.Memo.Memo? GetSourceMemo(int memoIndex, DocInfoAdder docInfoAdder)
        {
            var memoList = docInfoAdder.GetSourceHWPFile()?.BodyText?.MemoList;
            if (memoList == null) return null;

            foreach (var memo in memoList)
            {
                if (memo.MemoList?.MemoIndex == memoIndex)
                {
                    return memo;
                }
            }
            return null;
        }

        private static long AddMemoToTarget(HwpLib.Object.BodyText.Paragraph.Memo.Memo sourceMemo, DocInfoAdder docInfoAdder)
        {
            long maxMemoIndex = 0;
            var memoList = docInfoAdder.GetTargetHWPFile()?.BodyText?.MemoList;
            if (memoList != null)
            {
                foreach (var memo in memoList)
                {
                    var index = memo.MemoList?.MemoIndex ?? 0;
                    if (index > maxMemoIndex) maxMemoIndex = index;
                }
            }
            maxMemoIndex += 1;

            var clonedMemo = sourceMemo.Clone();
            if (clonedMemo.MemoList != null)
                clonedMemo.MemoList.MemoIndex = maxMemoIndex;
            
            var targetMemo = docInfoAdder.GetTargetHWPFile()?.BodyText?.AddNewMemo();
            if (targetMemo != null)
            {
                var clonedMemoList = clonedMemo.MemoList;
                if (clonedMemoList != null)
                {
                    targetMemo.MemoList?.Copy(clonedMemoList);
                }
                ParagraphCopier.ListCopy(clonedMemo.ParagraphList, targetMemo.ParagraphList, docInfoAdder);
            }
            return maxMemoIndex;
        }

        private static void SetNewMemoIndex(CtrlHeaderField targetH, long newMemoIndex)
        {
            var command = targetH.Command;
            if (command?.Bytes == null) return;

            var commandStr = command.ToUTF16LEString();
            if (commandStr == null) return;

            string[] commands = commandStr.Split('/');
            if (commands.Length > 2)
            {
                commands[2] = newMemoIndex.ToString();
                command.Bytes = Encoding.Unicode.GetBytes(string.Join("/", commands));
                targetH.MemoIndex = (int)newMemoIndex;
            }
        }

        public static void CopyFooter(ControlFooter source, ControlFooter target, DocInfoAdder? docInfoAdder)
        {
            target.Header.Copy(source.Header);
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        public static void CopyFootnote(ControlFootnote source, ControlFootnote target, DocInfoAdder? docInfoAdder)
        {
            target.Header.Copy(source.Header);
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        public static void CopyHeader(ControlHeader source, ControlHeader target, DocInfoAdder? docInfoAdder)
        {
            target.Header.Copy(source.Header);
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        public static void CopyIndexMark(ControlIndexMark source, ControlIndexMark target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyNewNumber(ControlNewNumber source, ControlNewNumber target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyPageHide(ControlPageHide source, ControlPageHide target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyPageNumberPosition(ControlPageNumberPosition source, ControlPageNumberPosition target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyPageOddEvenAdjust(ControlPageOddEvenAdjust source, ControlPageOddEvenAdjust target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyBookmark(ControlBookmark source, ControlBookmark target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
        }

        public static void CopyHiddenComment(ControlHiddenComment source, ControlHiddenComment target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            target.ListHeader?.Copy(source.ListHeader);
            ParagraphCopier.ListCopy(source.ParagraphList, target.ParagraphList, docInfoAdder);
        }

        public static void CopyForm(ControlForm source, ControlForm target, DocInfoAdder? docInfoAdder)
        {
            var sourceHeader = source.GetHeader();
            if (sourceHeader != null)
            {
                target.GetHeader()?.Copy(sourceHeader);
            }
            CtrlDataCopier.Copy(source, target, docInfoAdder);
            var sourceFormObject = source.FormObject;
            if (sourceFormObject != null)
            {
                target.FormObject?.Copy(sourceFormObject);
            }
        }
    }
}
