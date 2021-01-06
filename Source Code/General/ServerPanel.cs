using DXP;

public class ServerPanel : ServerPanelView
{
   
    public ServerPanel(ServerPanelForm argFrm, string argPanelName, string argPanelCaption)
        : base(argFrm, argPanelName, argPanelCaption)
    {
        if (argPanelName == "")
            return;
        if (argPanelCaption == "")
            return;
    }

}
