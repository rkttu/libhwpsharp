namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Form;

/// <summary>
/// 양식 개체 컨트롤
/// </summary>
public class ControlForm : Control
{
    private FormObject formObject;

    /// <summary>
    /// 생성자
    /// </summary>
    public ControlForm()
        : this(new CtrlHeaderGso(ControlType.Form))
    {
    }

    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="header">양식 개체를 위한 컨트롤 헤더</param>
    public ControlForm(CtrlHeaderGso header)
        : base(header)
    {
        formObject = new FormObject();
    }

    /// <summary>
    /// 그리기 객체용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>그리기 객체용 컨트롤 헤더</returns>
    public new CtrlHeaderGso? GetHeader() => Header as CtrlHeaderGso;

    /// <summary>
    /// 양식 개체를 반환한다.
    /// </summary>
    /// <returns>양식 개체</returns>
    public FormObject FormObject => formObject;

    public override Control Clone()
    {
        ControlForm cloned = new ControlForm();
        cloned.CopyControlPart(this);

        cloned.formObject.Copy(formObject);
        return cloned;
    }
}
