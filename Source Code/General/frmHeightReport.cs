using System;
using System.Collections.Generic;
using System.Windows.Forms;

public partial class frmHeightReport : Form
{
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
        List<string> tmpComponents = new List<string>();
        foreach (string item in lstComponents.CheckedItems)
        {
            tmpComponents.Add(item.Substring(0, item.IndexOf(",")));
        }
        return tmpComponents;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
    public int ListCount()
    {
        return lstComponents.Items.Count;
    }
}


