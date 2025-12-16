namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 홀/짝수 조정(페이지 번호 제어) 컨트롤
/// </summary>
public class ControlPageOddEvenAdjust : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlPageOddEvenAdjust()
        : base(new CtrlHeaderPageOddEvenAdjust())
    {
    }

    /// <summary>
    /// 홀/짝수 조정(페이지 번호 제어) 컨트롤용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>홀/짝수 조정(페이지 번호 제어) 컨트롤 헤더</returns>
    public new CtrlHeaderPageOddEvenAdjust? GetHeader() => Header as CtrlHeaderPageOddEvenAdjust;

    public override Control Clone()
    {
        ControlPageOddEvenAdjust cloned = new ControlPageOddEvenAdjust();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
