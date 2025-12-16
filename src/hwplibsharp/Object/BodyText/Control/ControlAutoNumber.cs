namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 자동번호 컨트롤에 대한 객체
/// </summary>
public class ControlAutoNumber : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlAutoNumber()
        : base(new CtrlHeaderAutoNumber())
    {
    }

    /// <summary>
    /// 자동번호 컨트롤용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>자동번호 컨트롤용 컨트롤 헤더</returns>
    public new CtrlHeaderAutoNumber? GetHeader() => Header as CtrlHeaderAutoNumber;

    public override Control Clone()
    {
        ControlAutoNumber cloned = new ControlAutoNumber();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
