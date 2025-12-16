using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Bookmark;
using HwpLib.Object.BodyText.Control.Gso.TextBox;

namespace HwpLib.Reader.BodyText.Control.Gso.Part;

/// <summary>
/// 글상자를 읽기 위한 객체
/// </summary>
public static class ForTextBox
{
    /// <summary>
    /// 글상자를 읽는다.
    /// </summary>
    /// <param name="textBox">글상자</param>
    /// <param name="sr">스트림 리더</param>
    public static void Read(TextBox textBox, CompoundStreamReader sr)
    {
        ListHeader(textBox.ListHeader, sr);
        ForParagraphList.Read(textBox.ParagraphList, sr);
    }

    /// <summary>
    /// 글상자의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    /// <param name="lh">글상자의 문단 리스트 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void ListHeader(ListHeaderForTextBox lh, CompoundStreamReader sr)
    {
        lh.ParaCount = sr.ReadSInt4();
        lh.Property.Value = sr.ReadUInt4();
        lh.LeftMargin = sr.ReadUInt2();
        lh.RightMargin = sr.ReadUInt2();
        lh.TopMargin = sr.ReadUInt2();
        lh.BottomMargin = sr.ReadUInt2();
        lh.TextWidth = sr.ReadUInt4();

        if (sr.IsEndOfRecord()) return;

        sr.Skip(8); // unknown bytes

        if (sr.IsEndOfRecord()) return;

        int temp = sr.ReadSInt4();
        lh.EditableAtFormMode = (temp == 1);

        short flag = sr.ReadUInt1();
        if (flag == 0xff)
        {
            FieldName(lh, sr);
        }
    }

    /// <summary>
    /// 필드 이름을 읽는다.
    /// </summary>
    /// <param name="lh">글상자의 문단 리스트 헤더 레코드</param>
    /// <param name="sr">스트림 리더</param>
    private static void FieldName(ListHeaderForTextBox lh, CompoundStreamReader sr)
    {
        var ps = new ParameterSet();
        ForParameterSet.Read(ps, sr);

        if (ps.Id != 0x21b) return;

        foreach (var pi in ps.ParameterItemList)
        {
            if (pi.Id == 0x4000 && pi.Type == ParameterType.String)
            {
                lh.FieldName = pi.Value_BSTR;
            }
        }
    }
}
