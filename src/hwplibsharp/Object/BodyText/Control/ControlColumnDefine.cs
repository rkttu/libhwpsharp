namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 단 정의 컨트롤에 대한 객체
/// </summary>
public class ControlColumnDefine : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlColumnDefine()
        : base(new CtrlHeaderColumnDefine())
    {
    }

    /// <summary>
    /// 단 정의용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>단 정의용 컨트롤 헤더</returns>
    public new CtrlHeaderColumnDefine? GetHeader() => Header as CtrlHeaderColumnDefine;

    public override Control Clone()
    {
        ControlColumnDefine cloned = new ControlColumnDefine();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
