namespace HwpLib.Object.BodyText.Control;

using HwpLib.Object.BodyText.Control.Bookmark;
using HwpLib.Object.BodyText.Control.CtrlHeader;

/// <summary>
/// 필드 컨트롤
/// </summary>
public class ControlField : Control
{
    /// <summary>
    /// 생성자
    /// </summary>
    /// <param name="ctrlId">ctrl header의 ctrl-id</param>
    public ControlField(uint ctrlId)
        : base(new CtrlHeaderField(ctrlId))
    {
    }

    /// <summary>
    /// 필드용 컨트롤 헤더를 반환한다.
    /// </summary>
    /// <returns>필드용 컨트롤 헤더</returns>
    public new CtrlHeaderField? GetHeader() => Header as CtrlHeaderField;

    /// <summary>
    /// 필드 컨트롤의 이름을 반환한다.
    /// </summary>
    /// <returns>필드 컨트롤의 이름</returns>
    public string? GetName()
    {
        if (CtrlData != null)
        {
            if (CtrlData.ParameterSet.Id == 0x021B)
            {
                ParameterItem? pi = CtrlData.ParameterSet.GetParameterItem(0x4000);
                if (pi != null && pi.Type == ParameterType.String)
                {
                    if (pi.Value_BSTR != null)
                    {
                        return pi.Value_BSTR;
                    }
                }
            }
        }
        return CommandToName(GetHeader()?.Command.ToUTF16LEString());
    }

    private string? CommandToName(string? command)
    {
        if (command == null)
        {
            return null;
        }

        string[] properties = command.Split(' ');
        if (properties != null && properties.Length >= 1)
        {
            string[] token = properties[0].Split(':');
            if (token != null && token.Length >= 1)
            {
                return token[token.Length - 1];
            }
        }
        return null;
    }

    /// <summary>
    /// 필드 컨트롤의 이름을 설정한다.
    /// </summary>
    /// <param name="name">필드 이름</param>
    public void SetName(string name)
    {
        if (CtrlData == null)
        {
            CreateCtrlData();
            CtrlData!.ParameterSet.Id = 0x021B;
        }

        ParameterItem? pi = CtrlData!.ParameterSet.GetParameterItem(0x4000);
        if (pi == null)
        {
            pi = CtrlData.ParameterSet.AddNewParameterItem();
            pi.Id = 0x4000;
        }

        pi.Type = ParameterType.String;
        pi.Value_BSTR = name;
    }

    public override Control Clone()
    {
        ControlField cloned = new ControlField(Header!.CtrlId);
        cloned.CopyControlPart(this);
        return cloned;
    }
}
