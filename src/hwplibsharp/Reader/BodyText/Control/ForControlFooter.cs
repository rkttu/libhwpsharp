using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader.Header;
using HwpLib.Object.BodyText.Control.HeaderFooter;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 꼬리말 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlFooter
{
    /// <summary>
    /// 꼬리말 컨트롤
    /// </summary>
    private ControlFooter? _foot;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlFooter()
    {
    }

    /// <summary>
    /// 꼬리말 컨트롤을 읽는다.
    /// </summary>
    /// <param name="foot">꼬리말 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlFooter foot, CompoundStreamReader sr)
    {
        _foot = foot;
        _sr = sr;

        CtrlHeader();
        ListHeader();
        ParagraphList();
    }

    /// <summary>
    /// 꼬리말 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        _foot!.Header.ApplyPage = HeaderFooterApplyPageExtensions.FromValue((byte)_sr!.ReadUInt4());
        
        if (_sr.CurrentRecordHeader!.Size > (_sr.CurrentPosition - _sr.CurrentPositionAfterHeader))
        {
            _foot.Header.CreateIndex = _sr.ReadSInt4();
        }
    }

    /// <summary>
    /// 꼬리말 컨트롤의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    private void ListHeader()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            ListHeaderForHeaderFooter lh = _foot!.ListHeader;
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
        ForParagraphList.Read(_foot!.ParagraphList, _sr!);
    }
}
