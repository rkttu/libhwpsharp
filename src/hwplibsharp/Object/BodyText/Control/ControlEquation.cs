namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;
using HwpLib.Object.BodyText.Control.Equation;
using HwpLib.Object.BodyText.Control.Gso.Caption;

/// <summary>
/// 수식 컨트롤
/// </summary>
public class ControlEquation : Control
{
    /// <summary>
    /// 캡션
    /// </summary>
    private Caption? caption;

    /// <summary>
    /// 수식 정보
    /// </summary>
    private EQEdit eqEdit;

    /// <summary>
    /// 생성자
    /// </summary>
    public ControlEquation()
        : base(new CtrlHeaderGso(ControlType.Equation))
    {
        eqEdit = new EQEdit();
    }

    /// <summary>
    /// 그리기 객체용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>그리기 객체용 컨트롤 헤더</returns>
    public new CtrlHeaderGso? GetHeader() => Header as CtrlHeaderGso;

    /// <summary>
    /// 캡션 객체를 생성한다.
    /// </summary>
    public void CreateCaption()
    {
        caption = new Caption();
    }

    /// <summary>
    /// 캡션 객체를 삭제한다.
    /// </summary>
    public void DeleteCaption()
    {
        caption = null;
    }

    /// <summary>
    /// 캡션 객체를 반환한다.
    /// </summary>
    /// <returns>캡션 객체</returns>
    public Caption? Caption => caption;

    /// <summary>
    /// 수식 정보 객체를 반환한다.
    /// </summary>
    /// <returns>수식 정보 객체</returns>
    public EQEdit EQEdit => eqEdit;

    public override Control Clone()
    {
        ControlEquation cloned = new ControlEquation();
        cloned.CopyControlPart(this);

        if (caption != null)
        {
            cloned.CreateCaption();
            cloned.caption!.Copy(caption);
        }
        else
        {
            cloned.caption = null;
        }

        cloned.eqEdit.Copy(eqEdit);

        return cloned;
    }
}
