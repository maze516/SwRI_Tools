using DXP;
using NLog;

public class ToolsPreferences
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

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

    //Logger level
    public static NLog.LogLevel LoggerLevel = NLog.LogLevel.Info;

    /// <summary>
    /// Load preference settings from Altium Designer.
    /// </summary>
    public static void Load()
    {
        _Log.Debug("Load");

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
        if (System.IO.Directory.Exists("G:\\CADTOOLS\\"))
            ExtFileConfig = optionsReader.ReadString(section, "ExtFileConfig", "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini");
        else
            ExtFileConfig = optionsReader.ReadString(section, "ExtFileConfig", "G:\\ElectronicCAD\\Templates\\Altium Project Templates\\SwRI_Tools Paths.ini");

        switch (optionsReader.ReadString(section, "LoggerLevel", "Warn"))
        {
            case "Fatal":
                LoggerLevel = NLog.LogLevel.Fatal;
                break;

            case "Error":
                LoggerLevel = NLog.LogLevel.Error;
                break;

            case "Warn":
                LoggerLevel = NLog.LogLevel.Warn;
                break;

            case "Info":
                LoggerLevel = NLog.LogLevel.Info;
                break;

            case "Debug":
                LoggerLevel = NLog.LogLevel.Debug;
                break;

            case "Trace":
                LoggerLevel = NLog.LogLevel.Trace;
                break;
        }

        Util.UpdateLogger(LoggerLevel);

    }

    /// <summary>
    /// Save setings to AD.
    /// </summary>
    public static void Save()
    {
        _Log.Debug("Save");

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
        optionsWriter.WriteString(section, "LoggerLevel", LoggerLevel.Name);
        Util.UpdateLogger(LoggerLevel);

    }

}

