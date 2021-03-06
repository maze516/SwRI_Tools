using DXP;
using NLog;
using System;
using System.IO;

public partial class General_Options : OptionsForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);


    //Batch outjob generator option
    private bool ODB_HideRefDes;

    //Fix embeded resistor option
    private int FirstResistorLayer;
    private int LayerCount;

    //Error email options
    private bool SMTP_Enable;
    private string ToAddress;
    private string FromAddress;
    private string ClientHost;

    //Open Ext File Config
    private string ExtFileConfig = "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini";

    //Logger level
    public static NLog.LogLevel LoggerLevel = NLog.LogLevel.Info;

    public General_Options() : base()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        InitializeComponent();
        UI.ApplyADUITheme(this);
    }

    /// <summary>
    /// Gets settings from values in the form.
    /// Called by AD.
    /// </summary>
    protected override void GetStateControlsImpl()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        int intLayer;
        if (Int32.TryParse(txtEmbededResLayer.Text, out intLayer))
            ToolsPreferences.FirstResistorLayer = intLayer;
        else
            ToolsPreferences.FirstResistorLayer = 20;

        int intCount;
        if (Int32.TryParse(txtLayerCount.Text, out intCount))
            ToolsPreferences.LayerCount = intCount;
        else
            ToolsPreferences.LayerCount = 5;

        ToolsPreferences.ClientHost = txtSMTPHost.Text;
        ToolsPreferences.FromAddress = txtFromAddress.Text;
        ToolsPreferences.ToAddress = txtToAddress.Text;
        ToolsPreferences.SMTP_Enable = chkErrorEnable.Checked;

        ToolsPreferences.ODB_HideRefDes = chkBatchRefHide.Checked;

        ToolsPreferences.ExtFileConfig = txtExtFileConfig.Text;

        switch (cboLogLevel.SelectedItem)
        {
            case "Fatal":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Fatal;
                break;

            case "Error":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Error;
                break;

            case "Warn":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Warn;
                break;

            case "Info":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Info;
                break;

            case "Debug":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Debug;
                break;

            case "Trace":
                ToolsPreferences.LoggerLevel = NLog.LogLevel.Trace;
                break;
        }

    }

    /// <summary>
    /// Clears local modified settings to match the form values.
    /// </summary>
    protected override void ClearModified()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        int intLayer;
        if (int.TryParse(txtEmbededResLayer.Text, out intLayer))
            FirstResistorLayer = intLayer;
        else
            FirstResistorLayer = 20;

        int intCount;
        if (int.TryParse(txtLayerCount.Text, out intCount))
            LayerCount = intCount;
        else
            LayerCount = 5;

        ClientHost = txtSMTPHost.Text;
        FromAddress = txtFromAddress.Text;
        ToAddress = txtToAddress.Text;
        SMTP_Enable = chkErrorEnable.Checked;

        ODB_HideRefDes = chkBatchRefHide.Checked;

        ExtFileConfig = txtExtFileConfig.Text;

        switch (cboLogLevel.SelectedItem)
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
    }

    /// <summary>
    /// Updates the form values with current settings.
    /// Called by AD.
    /// </summary>
    protected override void SetStateControlsImpl()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        txtEmbededResLayer.Text = ToolsPreferences.FirstResistorLayer.ToString();
        txtLayerCount.Text = ToolsPreferences.LayerCount.ToString();

        txtSMTPHost.Text = ToolsPreferences.ClientHost;
        txtFromAddress.Text = ToolsPreferences.FromAddress;
        txtToAddress.Text = ToolsPreferences.ToAddress;
        chkErrorEnable.Checked = ToolsPreferences.SMTP_Enable;

        chkBatchRefHide.Checked = ToolsPreferences.ODB_HideRefDes;

        txtExtFileConfig.Text = ToolsPreferences.ExtFileConfig;

        cboLogLevel.Text = ToolsPreferences.LoggerLevel.Name;

        ClearModified();
    }

    /// <summary>
    /// Sets default values on the form.
    /// Called by AD.
    /// </summary>
    protected override void SetDefaultStateImpl()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //Batch outjob generator option
        chkBatchRefHide.Checked = true;

        //Fix embeded resistor option
        txtEmbededResLayer.Text = "20";
        txtLayerCount.Text = "5";

        //Error email options
        chkErrorEnable.Checked = true;
        txtFromAddress.Text = "SwRI15_Error_Report@swri.org";
        txtToAddress.Text = "rlyne@swri.org";
        txtSMTPHost.Text = "smtp.swri.org";

        //Open Ext File config path
        if (System.IO.Directory.Exists("G:\\CADTOOLS\\"))
            txtExtFileConfig.Text = "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini";
        else
            txtExtFileConfig.Text = "G:\\ElectronicCAD\\Templates\\Altium Project Templates\\SwRI_Tools Paths.ini";
    }

    /// <summary>
    /// Checks if values in the form have been modified. Enables/disables the "Apply" button in DXP>Preferences.
    /// Called by AD.
    /// </summary>
    /// <returns>True if form values have been modified.</returns>
    protected override bool GetModifiedImpl()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        return (txtEmbededResLayer.Text != ToolsPreferences.FirstResistorLayer.ToString()) ||
            (txtLayerCount.Text != ToolsPreferences.LayerCount.ToString()) ||
            (txtSMTPHost.Text != ToolsPreferences.ClientHost) ||
            (txtFromAddress.Text != ToolsPreferences.FromAddress) ||
            (txtToAddress.Text != ToolsPreferences.ToAddress) ||
            (chkErrorEnable.Checked != ToolsPreferences.SMTP_Enable) ||
            (chkBatchRefHide.Checked != ToolsPreferences.ODB_HideRefDes) ||
            (txtExtFileConfig.Text != ToolsPreferences.ExtFileConfig) ||
            (cboLogLevel.Text != ToolsPreferences.LoggerLevel.Name);
    }

    /// <summary>
    /// Lets AD get the unique ID of our extension for use in notifications.
    /// Called by AD.
    /// </summary>
    /// <returns></returns>
    protected override int GetNotificationCodeImpl()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        return ToolsPreferences.NotificationCodePreferencesChanged;
    }

    /// <summary>
    /// Toggles the text fields on the form for SMTP error reporting based on if reporting is enabled..
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkErrorEnable_CheckedChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        bool boolEnabled = chkErrorEnable.Checked;

        txtFromAddress.Enabled = boolEnabled;
        txtSMTPHost.Enabled = boolEnabled;
        txtToAddress.Enabled = boolEnabled;

    }

    private void txtExtFileConfig_TextChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (!File.Exists(txtExtFileConfig.Text))
        {
            txtExtFileConfig.BackColor = System.Drawing.Color.Red;
        }
        else
        {
            txtExtFileConfig.BackColor = DxpThemeManager.ColorPanelBody;
        }
    }
}

