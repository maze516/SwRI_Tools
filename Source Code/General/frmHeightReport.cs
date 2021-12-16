using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class frmHeightReport : Form
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    #region Windows Form Controls

    public frmHeightReport()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
    }

    private void lnkSelectAll_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        for (int i = 0; i < lstComponents.Items.Count; i++)
        {
            lstComponents.SetItemChecked(i, true);
        }
    }

    private void lnkSelectNone_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        for (int i = 0; i < lstComponents.Items.Count; i++)
        {
            lstComponents.SetItemChecked(i, false);
        }
    }

    #endregion

    /// <summary>
    /// Will populate the list box with data from HeightList.
    /// </summary>
    /// <param name="HeightList"></param>
    public void FillList(Dictionary<string, Heights> HeightList)
    {
        _Log.Debug("FillList");

        foreach (KeyValuePair<string, Heights> item in HeightList)
        {
            if (item.Value.BodyHeight != -1 && item.Value.ParameterHeight != -1)
                if (item.Value.BodyHeight != item.Value.ParameterHeight)
                    lstComponents.Items.Add(item.ToString().Replace("[", "").Replace("]", ""), true);
        }

    }
    /// <summary>
    /// 
    /// </summary>
    /// <returns>Returns a list of components selected in the list.</returns>
    public List<string> GetSelectedComponents()
    {
        _Log.Debug("GetSelectedComponents");

        List<string> tmpComponents = new List<string>();
        foreach (string item in lstComponents.CheckedItems)
        {
            tmpComponents.Add(item.Substring(0, item.IndexOf(",")));
        }
        return tmpComponents;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnUpdate_Click");

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnCancel_Click");

        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
    public int ListCount()
    {
        _Log.Debug("ListCount");

        return lstComponents.Items.Count;
    }
}


