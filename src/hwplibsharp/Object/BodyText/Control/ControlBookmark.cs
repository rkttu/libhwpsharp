namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 책갈피 컨트롤에 대한 객체
/// </summary>
public class ControlBookmark : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlBookmark()
        : base(new CtrlHeaderBookmark())
    {
    }

    /// <summary>
    /// 책갈피용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>책갈피용 컨트롤 헤더</returns>
    public new CtrlHeaderBookmark? GetHeader() => Header as CtrlHeaderBookmark;

    public override Control Clone()
    {
        ControlBookmark cloned = new ControlBookmark();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
