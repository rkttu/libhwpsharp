using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.FootnoteEndnote;
using HwpLib.Object.BodyText.Control.SectionDefine;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 각주 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlFootnote
{
    /// <summary>
    /// 각주 컨트롤
    /// </summary>
    private ControlFootnote? _fn;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlFootnote()
    {
    }

    /// <summary>
    /// 각주 컨트롤을 읽는다.
    /// </summary>
    /// <param name="fn">각주 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlFootnote fn, CompoundStreamReader sr)
    {
        _fn = fn;
        _sr = sr;

        CtrlHeader();
        ListHeader();
        ParagraphList();
    }

    /// <summary>
    /// 각주 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        CtrlHeaderFootnote h = _fn!.Header;
        h.Number = _sr!.ReadUInt4();
        h.BeforeDecorationLetter.Bytes = _sr.ReadWChar();
        h.AfterDecorationLetter.Bytes = _sr.ReadWChar();
        h.NumberShape = NumberShapeExtensions.FromValue((short)_sr.ReadUInt4());

        if (_sr.IsEndOfRecord()) return;

        h.InstanceId = _sr.ReadUInt4();
    }

    /// <summary>
    /// 각주 컨트롤의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    private void ListHeader()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            ListHeaderForFootnoteEndnote lh = _fn!.ListHeader;
            lh.ParaCount = _sr.ReadSInt4();
            lh.Property.Value = _sr.ReadUInt4();
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
        ForParagraphList.Read(_fn!.ParagraphList, _sr!);
    }
}
