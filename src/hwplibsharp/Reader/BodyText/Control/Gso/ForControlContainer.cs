using HwpLib.CompoundFile;
using HwpLib.Object.BodyText.Control.Gso;
using HwpLib.Object.BodyText.Control.Gso.ShapeComponent;

namespace HwpLib.Reader.BodyText.Control.Gso;

/// <summary>
/// 묶음 컨트롤의 나머지 부분을 읽기 위한 객체
/// </summary>
public static class ForControlContainer
{
    /// <summary>
    /// 묶음 컨트롤의 나머지 부분을 읽는다.
    /// </summary>
    /// <param name="container">묶음 컨트롤</param>
    /// <param name="sr">스트림 리더</param>
    public static void ReadRest(ControlContainer container, CompoundStreamReader sr)
    {
        var scc = (ShapeComponentContainer)container.ShapeComponent;
        int childCount = scc.ChildControlIdList.Count;
        for (int index = 0; index < childCount; index++)
        {
            var fgc = new ForGsoControl();
            var gc = fgc.ReadInContainer(sr);
            container.AddChildControl(gc);
        }
    }
}
