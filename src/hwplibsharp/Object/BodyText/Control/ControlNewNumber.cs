namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 새 번호 지정 컨트롤
/// </summary>
public class ControlNewNumber : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlNewNumber()
        : base(new CtrlHeaderNewNumber())
    {
    }

    /// <summary>
    /// 새 번호 지정용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>새 번호 지정용 컨트롤 헤더</returns>
    public new CtrlHeaderNewNumber? GetHeader() => Header as CtrlHeaderNewNumber;

    public override Control Clone()
    {
        ControlNewNumber cloned = new ControlNewNumber();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
