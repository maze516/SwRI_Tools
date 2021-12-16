using NLog;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


public partial class frmEmbededResistorRpt : Form
{
    private bool updating = false; //Used to stop methods from being triggered while filling the Data Grid View.
    private SortedDictionary<string, int> LayerList;
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public frmEmbededResistorRpt()
    {
        _Log.Debug("frmEmbededResistorRpt");

        updating = true;
        InitializeComponent();
        UI.ApplyADUITheme(this);
        updating = false;
    }


    private void dataGridView1_ContentClicked(object sender, DataGridViewCellEventArgs e)
    {
        _Log.Debug("dataGridView1_ContentClicked");

        if (updating) return;
        updating = true;
        if (e.ColumnIndex == 2)
            dgvLayers.CurrentCell.Value = !(bool)dgvLayers.CurrentCell.Value;

        int i = ToolsPreferences.FirstResistorLayer;
        foreach (DataGridViewRow item in dgvLayers.Rows)
        {
            if (item.Cells["colEnable"].Value.Equals(true))
            {
                item.Cells["colDest"].Value = "Mechanical " + i;
                i++;
            }
            else
                item.Cells["colDest"].Value = "";
        }
        
        dgvLayers.CurrentCell = null;
        updating = false;
    }

    /// <summary>
    /// fills the DGV with used layers.
    /// </summary>
    /// <param name="Layers">Dictionary of used layers and qty of components on each.</param>
    public void FillList(SortedDictionary<string, int> Layers)
    {
        _Log.Debug("FillList");

        updating = true;
        int Pos;
        LayerList = Layers;
        foreach (KeyValuePair<string, int> item in Layers)
        {
            Pos = dgvLayers.Rows.Add();
            dgvLayers.Rows[Pos].Cells["colSource"].Value = item.Value + " components on layer: " + item.Key; //Source Column
            dgvLayers.Rows[Pos].Cells["colDest"].Value = "Mechanical " + (ToolsPreferences.FirstResistorLayer + Pos); //Destination Column
            dgvLayers.Rows[Pos].Cells["colEnable"].Value = true; //Enable Column
        }
        updating = false;
    }

    /// <summary>
    /// Gets the list of layers used based on which are enabled.
    /// </summary>
    /// <returns>Updated list of layers used.</returns>
    public SortedDictionary<string, int> GetSelection()
    {
        _Log.Debug("GetSelection");

        string tmpKey;
        int tmpLoc;
        foreach (DataGridViewRow item in dgvLayers.Rows)
        {
            if (item.Cells["colEnable"].Value.Equals(false))
            {
                tmpLoc = item.Cells["colSource"].Value.ToString().IndexOf(": ");
                tmpKey = item.Cells["colSource"].Value.ToString().Substring(tmpLoc + 2);
                LayerList.Remove(tmpKey);
            }
        }
        return LayerList;
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnCancel_Click");

    }
}