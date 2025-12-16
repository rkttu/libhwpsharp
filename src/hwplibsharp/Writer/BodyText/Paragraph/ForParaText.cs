using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Paragraph.Text;
using HwpLib.Object.Etc;

namespace HwpLib.Writer.BodyText.Paragraph;

/// <summary>
/// 문단의 텍스트 레코드를 쓰기 위한 객체
/// </summary>
public static class ForParaText
{
    /// <summary>
    /// 문단의 텍스트 레코드를 쓴다.
    /// </summary>
    /// <param name="p">문단</param>
    /// <param name="sw">스트림 라이터</param>
    public static void Write(Object.BodyText.Paragraph.Paragraph p, CompoundStreamWriter sw)
    {
        if (EmptyText(p))
        {
            return;
        }

        RecordHeader(p, sw);

        foreach (var hc in p.Text!.CharList)
        {
            HwpChar(hc, sw);
        }
    }

    private static bool EmptyText(Object.BodyText.Paragraph.Paragraph p)
    {
        if (p.Header.CharacterCount <= 1)
        {
            var paraText = p.Text;
            if (paraText == null)
            {
                return true;
            }

            if (paraText.CharList.Count == 0)
            {
                return true;
            }

            if (paraText.CharList.Count == 1)
            {
                var hwpChar = paraText.CharList[0];
                if (hwpChar.Code == 0x0d)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 문단의 텍스트 레코드의 레코드 헤더를 쓴다.
    /// </summary>
    private static void RecordHeader(Object.BodyText.Paragraph.Paragraph p, CompoundStreamWriter sw)
    {
        long size = p.Header.CharacterCount * 2;
        sw.WriteRecordHeader(HWPTag.ParaText, (int)size);
    }

    /// <summary>
    /// Character을 쓴다.
    /// </summary>
    private static void HwpChar(HWPChar hc, CompoundStreamWriter sw)
    {
        switch (hc.Type)
        {
            case HWPCharType.Normal:
                Normal((HWPCharNormal)hc, sw);
                break;
            case HWPCharType.ControlChar:
                ControlChar((HWPCharControlChar)hc, sw);
                break;
            case HWPCharType.ControlInline:
                ControlInline((HWPCharControlInline)hc, sw);
                break;
            case HWPCharType.ControlExtend:
                ControlExtend((HWPCharControlExtend)hc, sw);
                break;
        }
    }

    /// <summary>
    /// 일반 Character를 쓴다.
    /// </summary>
    private static void Normal(HWPCharNormal hc, CompoundStreamWriter sw)
    {
        sw.WriteUInt2(hc.Code);
    }

    /// <summary>
    /// 문자 컨트롤 Character를 쓴다.
    /// </summary>
    private static void ControlChar(HWPCharControlChar hc, CompoundStreamWriter sw)
    {
        sw.WriteUInt2(hc.Code);
    }

    /// <summary>
    /// 인라인 컨트롤 character을 쓴다.
    /// </summary>
    private static void ControlInline(HWPCharControlInline hc, CompoundStreamWriter sw)
    {
        sw.WriteUInt2(hc.Code);
        sw.WriteBytes(hc.Addition ?? Array.Empty<byte>());
        sw.WriteUInt2(hc.Code);
    }

    /// <summary>
    /// 확장 컨트롤 Character를 쓴다.
    /// </summary>
    private static void ControlExtend(HWPCharControlExtend hc, CompoundStreamWriter sw)
    {
        sw.WriteUInt2(hc.Code);
        sw.WriteBytes(hc.Addition ?? Array.Empty<byte>());
        sw.WriteUInt2(hc.Code);
    }
}
