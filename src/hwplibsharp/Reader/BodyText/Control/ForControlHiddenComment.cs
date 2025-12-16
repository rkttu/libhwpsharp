using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 숨은 설명 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlHiddenComment
{
    /// <summary>
    /// 숨은 설명 컨트롤
    /// </summary>
    private ControlHiddenComment? _tcmt;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlHiddenComment()
    {
    }

    /// <summary>
    /// 숨은 설명 컨트롤을 읽는다.
    /// </summary>
    /// <param name="tcmt">숨은 설명 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlHiddenComment tcmt, CompoundStreamReader sr)
    {
        _tcmt = tcmt;
        _sr = sr;

        ListHeader();
        ParagraphList();
    }

    /// <summary>
    /// 숨은 설명 컨트롤의 문단 리스트 헤더 레코드를 읽는다.
    /// </summary>
    private void ListHeader()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId == HWPTag.ListHeader)
        {
            _tcmt!.ListHeader.ParaCount = _sr.ReadSInt4();
            _tcmt.ListHeader.Property.Value = _sr.ReadUInt4();
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
        ForParagraphList.Read(_tcmt!.ParagraphList, _sr!);
    }
}
