using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control;
using HwpLib.Object.Etc;
using HwpLib.Reader.BodyText.Control.Eqed;
using HwpLib.Reader.BodyText.Control.Gso.Part;

namespace HwpLib.Reader.BodyText.Control;

/// <summary>
/// 수식 컨트롤을 읽기 위한 객체
/// </summary>
public class ForControlEquation
{
    /// <summary>
    /// 수식 컨트롤
    /// </summary>
    private ControlEquation? _eqed;

    /// <summary>
    /// 스트림 리더
    /// </summary>
    private CompoundStreamReader? _sr;

    /// <summary>
    /// 컨트롤 헤더 레코드의 레벨
    /// </summary>
    private int _ctrlHeaderLevel;

    /// <summary>
    /// 생성자
    /// </summary>
    public ForControlEquation()
    {
    }

    /// <summary>
    /// 수식 컨트롤을 읽는다.
    /// </summary>
    /// <param name="eqed">수식 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public void Read(ControlEquation eqed, CompoundStreamReader sr)
    {
        _eqed = eqed;
        _sr = sr;
        _ctrlHeaderLevel = sr.CurrentRecordHeader!.Level;

        CtrlHeader();
        Caption();

        while (!sr.IsEndOfStream())
        {
            if (!sr.IsImmediatelyAfterReadingHeader)
            {
                sr.ReadRecordHeader();
            }

            if (_ctrlHeaderLevel >= sr.CurrentRecordHeader!.Level)
            {
                break;
            }
            ReadBody();
        }
    }

    /// <summary>
    /// 수식 컨트롤의 컨트롤 헤더 레코드를 읽는다.
    /// </summary>
    private void CtrlHeader()
    {
        ForCtrlHeaderGso.Read(_eqed!.GetHeader()!, _sr!);
    }

    /// <summary>
    /// 캡션 정보를 읽는다.
    /// </summary>
    private void Caption()
    {
        _sr!.ReadRecordHeader();
        if (_sr.CurrentRecordHeader?.TagId != HWPTag.ListHeader) return;

        _eqed!.CreateCaption();
        ForCaption.Read(_eqed.Caption!, _sr);
    }

    /// <summary>
    /// 이미 읽은 레코드 헤더에 따른 레코드 내용을 읽는다.
    /// </summary>
    private void ReadBody()
    {
        if (_sr!.CurrentRecordHeader?.TagId == HWPTag.EqEdit)
        {
            EqEdit();
        }
        else
        {
            _sr.SkipToEndRecord();
        }
    }

    /// <summary>
    /// 수식 정보 레코드를 읽는다.
    /// </summary>
    private void EqEdit()
    {
        ForEQEdit.Read(_eqed!.EQEdit, _sr!);
    }
}
