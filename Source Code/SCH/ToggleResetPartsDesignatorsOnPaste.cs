using DXP;


class ToggleResetPartsDesignatorsOnPaste
{
    /// <summary>
    /// Toggles the "Reset Part Refdes on Paste" preference.
    /// </summary>
    public void ToggleResetPartsRefDesOnPaste()
    {
        bool CurrentState;

        IOptionsReader optionsReader = DXP.Utils.ServerOptionsReader("SCH");
        IOptionsWriter optionsWriter = DXP.Utils.ServerOptionsWriter("SCH");

        CurrentState = optionsReader.ReadBoolean("Schematic Preferences", "ResetPartsDesignatorsOnPaste", true); //Get current preference state.
        optionsWriter.WriteBoolean("Schematic Preferences", "ResetPartsDesignatorsOnPaste", !CurrentState); //Invert current preference state.

        //Update changed preferences.
        var schServer = DXP.GlobalVars.Client.GetServerModuleByName("SCH");
        if (schServer != null)
            (schServer as IServerOptions).ReloadOptions();
    }
}

