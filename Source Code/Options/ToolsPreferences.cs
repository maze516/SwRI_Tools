using DXP;


public class ToolsPreferences
{
    public const int NotificationCodePreferencesChanged = 1010; //Unique ID used to identify messages for this Extension from AD.

    //Batch outjob generator option
    public static bool ODB_HideRefDes = true;

    //Fix embeded resistor option
    public static int FirstResistorLayer = 19;
    public static int LayerCount = 6;

    //Error email options
    public static bool SMTP_Enable = true;
    public static string FromAddress = "SwRI_Error_Report@swri.org";
    public static string ToAddress = "rlyne@swri.org";
    public static string ClientHost = "smtp.swri.org";

    //Open Ext File Config
    public static string ExtFileConfig = "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini";

    /// <summary>
    /// Load preference settings from Altium Designer.
    /// </summary>
    public static void Load()
    {
        IOptionsReader optionsReader = Utils.ServerOptionsReader(Util.SERVERNAME);

        string section = Util.SERVERNAME;

        //If there are no preferences saved to AD yet then current defaults are saved.
        if (optionsReader.ReadSection(section) == null)
            Save();

        //Read settings from AD.
        ODB_HideRefDes = optionsReader.ReadBoolean(section, "ODB_HideRefDes", false);

        FirstResistorLayer = optionsReader.ReadInteger(section, "FirstResistorLayer", 20);
        LayerCount = optionsReader.ReadInteger(section, "LayerCount", 5);

        SMTP_Enable = optionsReader.ReadBoolean(section, "SMTP_Enable", false);
        FromAddress = optionsReader.ReadString(section, "FromAddress", "");
        ToAddress = optionsReader.ReadString(section, "ToAddress", "");
        ClientHost = optionsReader.ReadString(section, "ClientHost", "");
        ExtFileConfig = optionsReader.ReadString(section, "ExtFileConfig", "");

    }

    /// <summary>
    /// Save setings to AD.
    /// </summary>
    public static void Save()
    {
        IOptionsWriter optionsWriter = DXP.Utils.ServerOptionsWriter(Util.SERVERNAME);

        string section = Util.SERVERNAME;

        optionsWriter.WriteBoolean(section, "ODB_HideRefDes", ODB_HideRefDes);

        optionsWriter.WriteInteger(section, "FirstResistorLayer", FirstResistorLayer);
        optionsWriter.WriteInteger(section, "LayerCount", LayerCount);

        optionsWriter.WriteBoolean(section, "SMTP_Enable", SMTP_Enable);
        optionsWriter.WriteString(section, "FromAddress", FromAddress);
        optionsWriter.WriteString(section, "ToAddress", ToAddress);
        optionsWriter.WriteString(section, "ClientHost", ClientHost);
        optionsWriter.WriteString(section, "ExtFileConfig", ExtFileConfig);

    }

}

