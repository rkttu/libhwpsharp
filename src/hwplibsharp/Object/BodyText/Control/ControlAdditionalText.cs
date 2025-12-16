namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 덧말 컨트롤
/// </summary>
public class ControlAdditionalText : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlAdditionalText()
        : base(new CtrlHeaderAdditionalText())
    {
    }

    /// <summary>
    /// 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>컨트롤 헤더</returns>
    public new CtrlHeaderAdditionalText? GetHeader() => Header as CtrlHeaderAdditionalText;

    public override Control Clone()
    {
        ControlAdditionalText cloned = new ControlAdditionalText();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
