using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader.Header;
using HwpLib.Object.BodyText.Control.HeaderFooter;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 머리말 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlHeader
{
    /// <summary>
    /// 머리말 컨트롤
    /// </summary>
    private ControlHeader? _head;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlHeader()
    {
    }

    /// <summary>
    /// 머리말 컨트롤을 읽는다.
    /// </summary>
    /// <param name="head">머리말 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlHeader head, CompoundStreamReader sr)
    {
        _head = head;
        _sr = sr;

        CtrlHeader();
        ListHeader();
        ParagraphList();
    }

    /// <summary>
    /// 머리말 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        _head!.Header.ApplyPage = HeaderFooterApplyPageExtensions.FromValue((byte)_sr!.ReadUInt4());

        if (_sr.IsEndOfRecord()) return;

        _head.Header.CreateIndex = _sr.ReadSInt4();
    }

    /// <summary>
    /// 머리말 컨트롤의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    private void ListHeader()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            ListHeaderForHeaderFooter lh = _head!.ListHeader;
            lh.ParaCount = _sr.ReadSInt4();
            lh.Property.Value = _sr.ReadUInt4();
            lh.TextWidth = _sr.ReadUInt4();
            lh.TextHeight = _sr.ReadUInt4();
            _sr.SkipToEndRecord();
        }
        else
        {
            throw new InvalidOperationException("List header must be located.");
        }
    }

    /// <summary>
    /// 문단 리스트를 읽는다.
    /// </summary>
    private void ParagraphList()
    {
        ForParagraphList.Read(_head!.ParagraphList, _sr!);
    }
}
