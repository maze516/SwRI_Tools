using NLog;
using PCB;
using System;


public class ToggleLockedObjects
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;

    /// <summary>
    /// Togles the 'DXP>Preferences>PCB Editor>General>Protect Locked Objects' setting.
    /// </summary>
    public void ToggleProtectLockedObjects()
    {
        try
        {
            IPCB_SystemOptions PCBSystemOptions;
            bool PLPSetting;
            PCBSystemOptions = PCBServer.GetState_SystemOptions();
            if (PCBSystemOptions == null) return;
            
            PLPSetting = PCBSystemOptions.GetState_ProtectLockedPrimitives();
            PLPSetting = !PLPSetting;
            PCBSystemOptions.SetState_ProtectLockedPrimitives(PLPSetting);

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
    }

    public void ToggleLoopRemoval()
    {
        try
        {
            IPCB_SystemOptions PCBSystemOptions;
            bool PLPSetting;
            PCBSystemOptions = PCBServer.GetState_SystemOptions();
            if (PCBSystemOptions == null) return;

            PLPSetting = PCBSystemOptions.GetState_LoopRemoval();
            PLPSetting = !PLPSetting;
            PCBSystemOptions.SetState_LoopRemoval(PLPSetting);

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
    }

}

