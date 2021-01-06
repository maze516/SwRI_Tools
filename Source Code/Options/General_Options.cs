using DXP;
using System;


public partial class General_Options : OptionsForm
{
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

    public General_Options() : base()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
    }

    /// <summary>
    /// Gets settings from values in the form.
    /// Called by AD.
    /// </summary>
    protected override void GetStateControlsImpl()
    {
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

    }

    /// <summary>
    /// Clears local modified settings to match the form values.
    /// </summary>
    protected override void ClearModified()
    {
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
    }

    /// <summary>
    /// Updates the form values with current settings.
    /// Called by AD.
    /// </summary>
    protected override void SetStateControlsImpl()
    {

        txtEmbededResLayer.Text = ToolsPreferences.FirstResistorLayer.ToString();
        txtLayerCount.Text = ToolsPreferences.LayerCount.ToString();

        txtSMTPHost.Text = ToolsPreferences.ClientHost;
        txtFromAddress.Text = ToolsPreferences.FromAddress;
        txtToAddress.Text = ToolsPreferences.ToAddress;
        chkErrorEnable.Checked = ToolsPreferences.SMTP_Enable;

        chkBatchRefHide.Checked = ToolsPreferences.ODB_HideRefDes;

        txtExtFileConfig.Text = ToolsPreferences.ExtFileConfig;

        ClearModified();
    }

    /// <summary>
    /// Sets default values on the form.
    /// Called by AD.
    /// </summary>
    protected override void SetDefaultStateImpl()
    {
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
        txtExtFileConfig.Text = "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini";
    }

    /// <summary>
    /// Checks if values in the form have been modified. Enables/disables the "Apply" button in DXP>Preferences.
    /// Called by AD.
    /// </summary>
    /// <returns>True if form values have been modified.</returns>
    protected override bool GetModifiedImpl()
    {
        return (txtEmbededResLayer.Text != ToolsPreferences.FirstResistorLayer.ToString()) ||
            (txtLayerCount.Text != ToolsPreferences.LayerCount.ToString()) ||
            (txtSMTPHost.Text != ToolsPreferences.ClientHost) ||
            (txtFromAddress.Text != ToolsPreferences.FromAddress) ||
            (txtToAddress.Text != ToolsPreferences.ToAddress) ||
            (chkErrorEnable.Checked != ToolsPreferences.SMTP_Enable) ||
            (chkBatchRefHide.Checked != ToolsPreferences.ODB_HideRefDes) ||
            (txtExtFileConfig.Text != ToolsPreferences.ExtFileConfig);
    }

    /// <summary>
    /// Lets AD get the unique ID of our extension for use in notifications.
    /// Called by AD.
    /// </summary>
    /// <returns></returns>
    protected override int GetNotificationCodeImpl()
    {
        return ToolsPreferences.NotificationCodePreferencesChanged;
    }

    /// <summary>
    /// Toggles the text fields on the form for SMTP error reporting based on if reporting is enabled..
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void chkErrorEnable_CheckedChanged(object sender, EventArgs e)
    {
        bool boolEnabled = chkErrorEnable.Checked;

        txtFromAddress.Enabled = boolEnabled;
        txtSMTPHost.Enabled = boolEnabled;
        txtToAddress.Enabled = boolEnabled;

    }
}

