using DXP;
using EDP;
using NLog;
using System;
using System.Windows.Forms;

public partial class VariantSync : Form
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    Variants SyncVar = new Variants();
    public VariantSync()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        InitializeComponent();
        UI.ApplyADUITheme(this);
        LoadCombobox(cbSource);
        LoadCombobox(cbDest, "");
    }
    void LoadCombobox(CheckedListBox cbObject, string Omit)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
        //cbObject.Items.Add("Base");
        ////Add variants to combo boxs.
        //for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
        //{

        //    cbObject.Items.Add((project.DM_ProjectVariants(i).DM_Description()));
        //}
        cbObject.Items.Clear();
        foreach (string item in cbSource.Items)
        {
            if (item != Omit)
                cbObject.Items.Add(item);
        }
    }
    void LoadCombobox(ComboBox cbObject)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
        cbObject.Items.Add("Base");
        //Add variants to combo boxs.
        for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
        {
            cbObject.Items.Add((project.DM_ProjectVariants(i).DM_Description()));
        }
    }
    private void btnSync_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        string Overwrite;
        if (rbOverwrite.Checked)
            Overwrite = "Overwrite";
        else if (rbPrompt.Checked)
            Overwrite = "Prompt";
        else
            Overwrite = "Skip";
        if (cbSource.Text != cbDest.Text)
        {
            string[] Destinations = new string[cbDest.CheckedItems.Count];

            SyncVar.Progress = pbProgress; //Pass progress bar to class so it can be updated.

            cbDest.CheckedItems.CopyTo(Destinations, 0);

            SyncVar.SyncVariants(cbSource.Text, Destinations, Overwrite, cbForce.Checked);
            this.Close();
        }
        else
            MessageBox.Show("From and To are the same. Please select different locations.");
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        this.Close();
    }

    private void cbDest_SelectedIndexChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //Gets the number of non-fitted variants to notify the user about what will be changed to fitted.
        int Count = 0;
        cbForce.Checked = false;
        string[] Dest = new string[cbDest.CheckedItems.Count];
        cbDest.CheckedItems.CopyTo(Dest, 0);
        foreach (string item in Dest)
        {
            if (item != "Base")
                Count += SyncVar.Get_VariantAlternates(item);
        }

        cbForce.Text = "Force " + Count + " Alternates to Fitted.";
        if (Count > 0) cbForce.Enabled = true;
        else cbForce.Enabled = false;
    }

    private void cbForce_CheckedChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (cbForce.Checked)
            MessageBox.Show("This will remove all variants set to \"Alternate\".", "Warning", MessageBoxButtons.OK);
    }

    private void cbSource_SelectedIndexChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        LoadCombobox(cbDest, cbSource.Text);
    }
}

