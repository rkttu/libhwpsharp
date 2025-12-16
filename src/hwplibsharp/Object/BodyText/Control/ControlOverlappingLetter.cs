namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 글자 겹침 컨트롤
/// </summary>
public class ControlOverlappingLetter : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    public ControlOverlappingLetter()
        : base(new CtrlHeaderOverlappingLetter())
    {
    }

    /// <summary>
    /// 글자 겹침용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>글자 겹침용 컨트롤 헤더</returns>
    public new CtrlHeaderOverlappingLetter? GetHeader() => Header as CtrlHeaderOverlappingLetter;

    public override Control Clone()
    {
        ControlOverlappingLetter cloned = new ControlOverlappingLetter();
        cloned.CopyControlPart(this);
        return cloned;
    }
}
