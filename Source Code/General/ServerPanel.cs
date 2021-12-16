using DXP;
using NLog;

public class ServerPanel : ServerPanelView
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public ServerPanel(ServerPanelForm argFrm, string argPanelName, string argPanelCaption)
        : base(argFrm, argPanelName, argPanelCaption)
    {
        _Log.Debug("ServerPanel");

        if (argPanelName == "")
            return;
        if (argPanelCaption == "")
            return;
    }

}
