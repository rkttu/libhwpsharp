namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 찾아보기 표식 컨트롤
/// </summary>
public class ControlIndexMark : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlIndexMark()
        : base(new CtrlHeaderIndexMark())
    {
    }

    /// <summary>
    /// 찾아보기 표식용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>찾아보기 표식용 컨트롤 헤더</returns>
    public new CtrlHeaderIndexMark? GetHeader() => Header as CtrlHeaderIndexMark;

    public override Control Clone()
    {
        ControlIndexMark cloned = new ControlIndexMark();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
