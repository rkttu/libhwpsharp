namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 감추기 컨트롤
/// </summary>
public class ControlPageHide : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlPageHide()
        : base(new CtrlHeaderPageHide())
    {
    }

    /// <summary>
    /// 감추기 컨트롤용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>감추기 컨트롤용 컨트롤 헤더</returns>
    public new CtrlHeaderPageHide? GetHeader() => Header as CtrlHeaderPageHide;

    public override Control Clone()
    {
        ControlPageHide cloned = new ControlPageHide();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
