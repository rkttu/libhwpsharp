using HwpLib.Object.BodyText;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Table;
using HwpLib.Object.BodyText.Paragraph;
using HwpLib.Object.BodyText.Paragraph.LineSeg;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Object.Etc;
using HwpLib.Tool.ObjectFinder.ForField.GetText;
using HwpLib.Tool.ObjectFinder.ForField.SetText;
using HwpLib.Tool.ParagraphAdder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Para = HwpLib.Object.BodyText.Paragraph.Paragraph;

namespace HwpLib.Tool.ObjectFinder.ForField
{
    /// <summary>
    /// 문단 리스트에 대해 필드 객체를 찾거나 텍스트를 가져오거나 설정하는 기능을 포함한 클래스
    /// </summary>
    public class ForParagraphList
    {
        /// <summary>
        /// 찾기 위치 정보
        /// </summary>
        public class FindPosition
        {
            public int StartParaIndex { get; set; }
            public int StartCharIndex { get; set; }
            public int EndParaIndex { get; set; }
            public int EndCharIndex { get; set; }

            public FindPosition()
            {
                StartParaIndex = -1;
                StartCharIndex = -1;
                EndParaIndex = -1;
                EndCharIndex = -1;
            }

            public bool FindStartPosition => StartParaIndex != -1 && StartCharIndex != -1;
            public bool FindEndPosition => EndParaIndex != -1 && EndCharIndex != -1;
        }

        /// <summary>
        /// 문단 리스트에서 필드 객체를 찾아 텍스트를 반환한다.
        /// </summary>
        /// <param name="paraList">문단 리스트</param>
        /// <param name="fieldType">필드 타입</param>
        /// <param name="fieldName">필드 이름</param>
        /// <returns>필드 텍스트</returns>
        public static string? GetFieldText(IParagraphList paraList, ControlType fieldType, string fieldName)
        {
            StringBuilder sb = new StringBuilder();

            if (paraList == null)
                return null;

            FindPosition findPosition = new FindPosition();

            int paraCount = paraList.ParagraphCount;
            for (int paraIndex = 0; paraIndex < paraCount; paraIndex++)
            {
                Para? para = paraList.GetParagraph(paraIndex);
                if (para == null) continue;
                
                if (!findPosition.FindStartPosition)
                {
                    FindStartPosition(para, fieldType, fieldName, findPosition, paraIndex);
                }

                if (findPosition.FindStartPosition)
                {
                    FindEndPosition(para, findPosition, paraIndex);

                    if (findPosition.FindEndPosition)
                    {
                        AppendText(paraList, sb, findPosition);
                        findPosition = new FindPosition();
                    }
                }

                ForControlGetText.GetFieldText(para.ControlList?.ToList(), fieldType, fieldName, sb);
            }

            return sb.Length > 0 ? sb.ToString() : null;
        }

        /// <summary>
        /// 시작 위치를 찾는다.
        /// </summary>
        private static void FindStartPosition(Paragraph para, ControlType fieldType, string fieldName, FindPosition findPosition, int paraIndex)
        {
            ParaText? pt = para.Text;
            if (pt == null)
                return;

            var charList = pt.CharList;
            int controlIndex = 0;
            for (int charIndex = 0; charIndex < charList.Count; charIndex++)
            {
                var hwpChar = charList[charIndex];
                if (hwpChar.Type == HWPCharType.ControlExtend)
                {
                    var controlList = para.ControlList;
                    if (controlList != null && controlIndex < controlList.Count)
                    {
                        Control ctrl = controlList[controlIndex];
                        if (ctrl.Type == fieldType)
                        {
                            if (IsMatchFieldName(ctrl, fieldName))
                            {
                                findPosition.StartParaIndex = paraIndex;
                                findPosition.StartCharIndex = charIndex;
                                return;
                            }
                        }
                    }
                    controlIndex++;
                }
            }
        }

        /// <summary>
        /// 필드 이름이 일치하는지 확인한다.
        /// </summary>
        private static bool IsMatchFieldName(Control ctrl, string fieldName)
        {
            if (ctrl is ControlField field)
            {
                string? ctrlFieldName = field.GetName();
                return fieldName.Equals(ctrlFieldName, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }

        /// <summary>
        /// 끝 위치를 찾는다.
        /// </summary>
        private static void FindEndPosition(Paragraph para, FindPosition findPosition, int paraIndex)
        {
            ParaText? pt = para.Text;
            if (pt == null)
                return;

            var charList = pt.CharList;
            int depth = 0;
            int startCharIndex = (paraIndex == findPosition.StartParaIndex) ? findPosition.StartCharIndex + 1 : 0;
            
            for (int charIndex = startCharIndex; charIndex < charList.Count; charIndex++)
            {
                var hwpChar = charList[charIndex];
                int code = hwpChar.Code;
                if (code == 0x3) // field start
                {
                    depth++;
                }
                else if (code == 0x4) // field end
                {
                    if (depth == 0)
                    {
                        findPosition.EndParaIndex = paraIndex;
                        findPosition.EndCharIndex = charIndex;
                        return;
                    }
                    else
                    {
                        depth--;
                    }
                }
            }
        }

        /// <summary>
        /// 필드의 텍스트를 추가한다.
        /// </summary>
        private static void AppendText(IParagraphList paraList, StringBuilder sb, FindPosition findPosition)
        {
            for (int paraIndex = findPosition.StartParaIndex; paraIndex <= findPosition.EndParaIndex; paraIndex++)
            {
                Para para = paraList.GetParagraph(paraIndex);
                ParaText? pt = para.Text;
                if (pt == null)
                    continue;

                var charList = pt.CharList;
                int startIndex = (paraIndex == findPosition.StartParaIndex) ? findPosition.StartCharIndex + 1 : 0;
                int endIndex = (paraIndex == findPosition.EndParaIndex) ? findPosition.EndCharIndex : charList.Count;

                for (int charIndex = startIndex; charIndex < endIndex; charIndex++)
                {
                    var hwpChar = charList[charIndex];
                    if (hwpChar is HWPCharNormal normal)
                    {
                        int code = normal.Code;
                        if (code >= 32) // 일반 문자
                        {
                            sb.Append((char)code);
                        }
                        else if (code == 10) // 줄바꿈
                        {
                            sb.Append('\n');
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 문단 리스트에서 모든 필드 객체를 찾아 텍스트를 반환한다.
        /// </summary>
        public static void GetAllFieldText(IParagraphList paraList, ControlType fieldType, string fieldName, List<string> results)
        {
            if (paraList == null)
                return;

            FindPosition findPosition = new FindPosition();

            int paraCount = paraList.ParagraphCount;
            for (int paraIndex = 0; paraIndex < paraCount; paraIndex++)
            {
                Para? para = paraList.GetParagraph(paraIndex);
                if (para == null) continue;
                
                if (!findPosition.FindStartPosition)
                {
                    FindStartPosition(para, fieldType, fieldName, findPosition, paraIndex);
                }

                if (findPosition.FindStartPosition)
                {
                    FindEndPosition(para, findPosition, paraIndex);

                    if (findPosition.FindEndPosition)
                    {
                        StringBuilder sb = new StringBuilder();
                        AppendText(paraList, sb, findPosition);
                        if (sb.Length > 0)
                        {
                            results.Add(sb.ToString());
                        }
                        findPosition = new FindPosition();
                    }
                }

                ForControlWithAllField.GetFieldText(para.ControlList?.ToList(), fieldType, fieldName, results);
            }
        }

        /// <summary>
        /// 문단 리스트에서 필드 객체를 찾아 텍스트를 설정한다.
        /// </summary>
        public static SetFieldResult SetFieldText(IParagraphList paraList, ControlType fieldType, string fieldName, TextBuffer textBuffer)
        {
            if (paraList == null)
                return SetFieldResult.InProcess;

            FindPosition findPosition = new FindPosition();

            int paraCount = paraList.ParagraphCount;
            for (int paraIndex = 0; paraIndex < paraCount; paraIndex++)
            {
                Para para = paraList.GetParagraph(paraIndex);
                if (!findPosition.FindStartPosition)
                {
                    FindStartPosition(para, fieldType, fieldName, findPosition, paraIndex);
                }

                if (findPosition.FindStartPosition)
                {
                    FindEndPosition(para, findPosition, paraIndex);

                    if (findPosition.FindEndPosition)
                    {
                        int deletedCount;
                        if (!ChangeText(paraList, findPosition, textBuffer, out deletedCount))
                        {
                            return SetFieldResult.NotEnoughText;
                        }
                        // 삭제된 문단 수만큼 인덱스와 카운트를 조정
                        paraIndex -= deletedCount;
                        paraCount -= deletedCount;
                        findPosition = new FindPosition();
                    }
                }

                SetFieldResult result = ForControlSetText.SetFieldText(para.ControlList?.ToList(), fieldType, fieldName, textBuffer);
                if (result == SetFieldResult.NotEnoughText)
                {
                    return SetFieldResult.NotEnoughText;
                }
            }

            return SetFieldResult.InProcess;
        }

        /// <summary>
        /// 텍스트를 변경한다.
        /// </summary>
        private static bool ChangeText(IParagraphList paraList, FindPosition findPosition, TextBuffer textBuffer, out int deletedCount)
        {
            deletedCount = 0;

            string? text = textBuffer.NextText();
            if (text == null)
            {
                return false;
            }

            if (findPosition.StartParaIndex != findPosition.EndParaIndex)
            {
                // 다른 문단인 경우
                Para startPara = paraList.GetParagraph(findPosition.StartParaIndex);
                ParaTextSetter.DeleteParaTextFrom(startPara, findPosition.StartCharIndex + 1);
                startPara.Text?.AddString(text);

                Para endPara = paraList.GetParagraph(findPosition.EndParaIndex);
                ParaTextSetter.DeleteParaTextTo(endPara, findPosition.EndCharIndex - 1);

                ParaTextSetter.MergeParagraph(startPara, endPara);

                paraList.DeleteParagraph(findPosition.EndParaIndex);
                deletedCount++;

                if (findPosition.EndParaIndex - findPosition.StartParaIndex >= 2)
                {
                    for (int deleteIndex = 0; deleteIndex < findPosition.EndParaIndex - findPosition.StartParaIndex - 1; deleteIndex++)
                    {
                        paraList.DeleteParagraph(findPosition.StartParaIndex + 1);
                        deletedCount++;
                    }
                }
            }
            else
            {
                // 같은 문단인 경우
                Para para = paraList.GetParagraph(findPosition.StartParaIndex);
                ParaTextSetter.ChangeText(para, findPosition.StartCharIndex + 1, findPosition.EndCharIndex - 1, text);
            }

            // LineSeg 삭제
            DeleteLineSeg(paraList, findPosition);

            return true;
        }

        /// <summary>
        /// 줄 세그먼트를 삭제한다.
        /// </summary>
        private static void DeleteLineSeg(IParagraphList paraList, FindPosition findPosition)
        {
            int endParaIndex = Math.Min(findPosition.EndParaIndex, paraList.ParagraphCount - 1);
            for (int paraIndex = findPosition.StartParaIndex; paraIndex <= endParaIndex; paraIndex++)
            {
                paraList.GetParagraph(paraIndex).DeleteLineSeg();
            }
        }
    }
}
