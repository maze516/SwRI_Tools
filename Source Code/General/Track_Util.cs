using DXP;



public class Track_Util : ServerPanelView
{
    public const string PanelName = "TrackUtil";
    public const string PanelCaption = "Track Util";
    public Track_Util(ServerPanelForm argFrm, string argPanelName, string argPanelCaption)
        : base(argFrm, argPanelName, argPanelCaption)
    {
        if (argPanelName == "")
            return;
        if (argPanelCaption == "")
            return;
    }

}
