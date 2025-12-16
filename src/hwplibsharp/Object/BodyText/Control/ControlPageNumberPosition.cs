namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 쪽 번호 위치 컨트롤
/// </summary>
public class ControlPageNumberPosition : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlPageNumberPosition()
        : base(new CtrlHeaderPageNumberPosition())
    {
    }

    /// <summary>
    /// 쪽 번호 위치 컨트롤용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>쪽 번호 위치 컨트롤용 컨트롤 헤더</returns>
    public new CtrlHeaderPageNumberPosition? GetHeader() => Header as CtrlHeaderPageNumberPosition;

    public override Control Clone()
    {
        ControlPageNumberPosition cloned = new ControlPageNumberPosition();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
