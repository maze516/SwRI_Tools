using DXP;
using NLog;
using PCB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


public partial class FootprintSelect : ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public const string PanelName = "VarFootprintSelect";
    public const string PanelCaption = "Variant Footprint Select";
    List<string> Report;
    public FootprintSelect()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        InitializeComponent();
        UI.ApplyADUITheme(this);
        //ListFootprints();
    }

    /// <summary>
    /// Fills treeview with variant component info.
    /// </summary>
    void ListFootprints()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            Report = new List<string>();
            Report.Add("Header:\t");
            Report.Add("Date:\t" + DateTime.Today.ToShortDateString());
            Report.Add("Time:\t	" + DateTime.Today.ToShortTimeString());
            tvList.Nodes.Clear();
            IPCB_BoardIterator BoardIterator;
            IPCB_Component Component;
            string RefDes, Varriant, Footprint, CompID;
            bool varr = false;
            IPCB_Board Board = Util.GetCurrentPCB();
            int ErrorCount = 0;
            Dictionary<string, IPCB_Component> VarrCompLayers = new Dictionary<string, IPCB_Component>();
            if (Board == null)
                return;

            Report.Add("Filename:\t" + Board.GetState_FileName());

            //Get origin values for determining coordinates
            double OriginX = EDP.Utils.CoordToMils(Board.GetState_XOrigin());
            double OriginY = EDP.Utils.CoordToMils(Board.GetState_YOrigin());

            #region First Pass
            //Iterate theough all components on the board.
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            //Filter for components only.
            FilterSet.Add(PCB.TObjectId.eComponentObject);
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            Component = (IPCB_Component)BoardIterator.FirstPCBObject();

            while (Component != null)
            {
                RefDes = Component.GetState_Name().GetState_Text();
                //Determines if component is a variant.
                //MessageBox.Show("X: " + (EDP.Utils.CoordToMils(Component.GetState_XLocation()) - OriginX) + " Y: " + (EDP.Utils.CoordToMils(Component.GetState_YLocation()) - OriginY));
                //MessageBox.Show("X: " + (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Lx) - OriginX) + " Y: " + (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Ly) - OriginY));
                if (Component.GetState_SourceUniqueId() == null)
                {
                    ErrorCount++;
                    varr = false;
                }
                else
                {
                    varr = Component.GetState_SourceUniqueId().Contains("@"); //Verify its a variant.
                    if (!varr)
                        varr = RefDes.Contains("EM");
                }

                if (varr)
                {
                    CompID = Component.GetState_SourceUniqueId();
                    if (Component.GetState_SourceUniqueId().Contains("@"))
                        Varriant = CompID.Substring(CompID.IndexOf("@") + 1); //Get variant name.
                    else
                        Varriant = "EM/FM";
                    Footprint = Component.GetState_Pattern(); //Get footprint name.
                    if (!VarrCompLayers.ContainsKey(RefDes))
                        VarrCompLayers.Add(RefDes, Component); //Collecting layer data for later comparison.
                    else
                    {
                        MessageBox.Show("There are duplicate " + RefDes + ". Please fix and try again.");
                        return;
                    }
                    if (!tvList.Nodes.ContainsKey(Varriant)) //Creates variant node if it doesnt already exist.
                        tvList.Nodes.Add(Varriant, Varriant);
                    if (!tvList.Nodes[Varriant].Nodes.ContainsKey(Footprint))//Creates footprint node if it doesnt already exist.
                        tvList.Nodes[Varriant].Nodes.Add(Footprint, Footprint);
                    tvList.Nodes[Varriant].Nodes[Footprint].Nodes.Add(RefDes); //Creates refdes node for component.
                }

                Component = (IPCB_Component)BoardIterator.NextPCBObject();
                varr = false;
            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
            #endregion

            #region Second Pass

            //Iterate theough all components on the board.
            BoardIterator = Board.BoardIterator_Create();
            FilterSet = new PCB.TObjectSet();
            //Filter for components only.
            FilterSet.Add(PCB.TObjectId.eComponentObject);
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            Component = (IPCB_Component)BoardIterator.FirstPCBObject();
            //Section 1:
            //Parts with Duplicate RefDes on Top &Bottom
            //Side Refdes  Variant FootPrint            PartNumber CentX    CentY Rotation
            Report.Add("\nSection 1:");
            Report.Add("Parts with Duplicate RefDes on Top & Bottom");
            while (Component != null)
            {
                RefDes = Component.GetState_Name().GetState_Text();
                if (VarrCompLayers.ContainsKey(RefDes) || VarrCompLayers.ContainsKey(RefDes.Replace("FM", "EM")))
                {
                    //Determines if component is a variant.
                    if (Component.GetState_SourceUniqueId() == null)
                    {
                        ErrorCount++;
                        varr = false;
                    }
                    else
                    {
                        varr = Component.GetState_SourceUniqueId().Contains("@");
                        if (!varr)
                            varr = RefDes.Contains("FM") || RefDes.Contains("EM");
                    }

                    if (!varr)
                    {
                        if (VarrCompLayers[RefDes].GetState_Layer() != Component.GetState_Layer())
                        {
                            CompID = VarrCompLayers[RefDes].GetState_SourceUniqueId();
                            if (CompID.Contains("@"))
                                Varriant = CompID.Substring(CompID.IndexOf("@") + 1); //Get variant name.
                            else
                                Varriant = "EM/FM";
                            Report.Add(RefDes + "\t" + Varriant + "\t" + VarrCompLayers[RefDes].GetState_Pattern() + "\t" + Util.GetLayerName(Board, VarrCompLayers[RefDes].GetState_V7Layer()));
                            Report.Add(RefDes + "\t" + "Base" + "\t" + Component.GetState_Pattern() + "\t" + Util.GetLayerName(Board, Component.GetState_V7Layer()));

                            for (int i = 0; i < tvList.Nodes.Count; i++)
                            {
                                for (int j = 0; j < tvList.Nodes[i].Nodes.Count; j++)
                                {
                                    foreach (TreeNode node in tvList.Nodes[i].Nodes[j].Nodes)
                                    {
                                        if (node.Text == RefDes)
                                        {
                                            //tvList.Nodes[i].Nodes[j].Nodes[RefDes].BackColor = Color.Red;
                                            node.BackColor = Color.Red;

                                            break;
                                        }

                                    }

                                }
                            }
                        }
                    }
                }
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
                varr = false;
            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
            #endregion

            Board.GraphicalView_ZoomRedraw();
            //Board.GraphicallyInvalidate();
            if (ErrorCount > 0)
                MessageBox.Show("There are " + ErrorCount + " parts missing unique ID's.", "Missing ID's", MessageBoxButtons.OK);
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

    /// <summary>
    /// Selects components matching selected treeview node.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tvList_AfterSelect(object sender, TreeViewEventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        List<string> Components = new List<string>();
        TreeNode Selected = tvList.SelectedNode;
        typeSelected SelectionType;
        //Determines whats type of selection will be processed
        if (Selected.Nodes.Count == 0) //No child nodes in this node so it is a reference designator.
            SelectionType = typeSelected.RefDes;
        else if (Selected.Parent == null) //Node does not have a parent so it is the top level node.
            SelectionType = typeSelected.Var;
        else //Node is in the middle.
            SelectionType = typeSelected.Footprint;

        //Reset mask if applied.
        string process = "PCB:RunQuery";
        string parameters = "Clear=True";
        DXP.Utils.RunCommand(process, parameters);

        IPCB_BoardIterator BoardIterator;
        IPCB_Component Component;
        string RefDes, Varriant, Footprint, CompID;
        bool varr = false;
        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        Component = (IPCB_Component)BoardIterator.FirstPCBObject();

        while (Component != null)
        {
            RefDes = Component.GetState_Name().GetState_Text();
            //Deselect existing selected components.
            Component.SetState_Selected(false);

            //Determine if component is in a variant.
            if (Component.GetState_SourceUniqueId() == null)
                varr = false;
            else
            {
                varr = Component.GetState_SourceUniqueId().Contains("@");

                if (!varr)
                    varr = RefDes.Contains("EM") || RefDes.Contains("FM");
            }

            if (varr)
            {

                CompID = Component.GetState_SourceUniqueId();
                if (Component.GetState_SourceUniqueId().Contains("@"))
                    Varriant = CompID.Substring(CompID.IndexOf("@") + 1); //Get variant name.
                else
                    Varriant = "EM/FM";
                Footprint = Component.GetState_Pattern(); //Get footprint name.

                //Add components to list based on selection type.
                switch (SelectionType)
                {
                    case typeSelected.Var:
                        if (Varriant == Selected.Text)
                        {
                            Components.Add(RefDes);
                        }
                        break;
                    case typeSelected.Footprint:
                        if (Footprint == Selected.Text)
                        {
                            Components.Add(RefDes);
                        }
                        break;
                    case typeSelected.RefDes:
                        if (RefDes == Selected.Text)
                        {
                            Components.Add(RefDes);
                        }
                        //else if(RefDes == Selected.Text.Replace("EM","FM"))
                        //{
                        //    Components.Add(RefDes);
                        //}
                        break;
                    default:
                        break;
                }
            }
            Component = (IPCB_Component)BoardIterator.NextPCBObject();
            varr = false;
        }

        Component = (IPCB_Component)BoardIterator.FirstPCBObject();
        while (Component != null)
        {
            RefDes = Component.GetState_Name().GetState_Text();
            //Deselect existing selected components.
            //Component.SetState_Selected(false);
            //_Log.Debug(RefDes);
            //Determine if component is in a variant.
            if (Component.GetState_SourceUniqueId() == null)
                varr = false;
            else
            {
                varr = Component.GetState_SourceUniqueId().Contains("@");

                if (!varr)
                    varr = RefDes.Contains("EM");
            }

            switch (cbSelectBase.Checked)
            {
                case true:
                    if (varr == false & Components.Contains(RefDes.Replace("FM", "EM")))
                    {
                        Component.SetState_Selected(true);
                        if (chkMask.Checked) //Add component to list of masked items.
                            Board.AddObjectToHighlightObjectList(Component);       //Iterator clean-up
                    }
                    break;
                case false:
                    if (varr == true & Components.Contains(RefDes))
                    {
                        Component.SetState_Selected(true);
                        if (chkMask.Checked) //Add component to list of masked items.
                            Board.AddObjectToHighlightObjectList(Component);       //Iterator clean-up
                    }
                    break;
                default:
                    break;
            }


            Component = (IPCB_Component)BoardIterator.NextPCBObject();
        }
        //Component.SetState_Selected(true);
        //if (chkMask.Checked) //Add component to list of masked items.
        //    Board.AddObjectToHighlightObjectList(Component);       //Iterator clean-up


        Board.BoardIterator_Destroy(ref BoardIterator);

        Board.GraphicalView_ZoomRedraw();
        Board.GraphicallyInvalidate();

        //Apply component mask.
        if (chkMask.Checked)
            Board.SetState_Navigate_HighlightObjectList(new EDP.THighlightMethodSet(EDP.THighlightMethod.eHighlight_Filter), false);
    }
    private enum typeSelected
    {
        Var,
        Footprint,
        RefDes
    };

    private void btnZoom_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        DXP.Utils.RunCommand("PCB:Zoom", "Action= Selected");
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        ListFootprints();
    }

    private void btnMask_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        DXP.Utils.RunCommand("PCB:Mask", "Action= Selected");
    }

    private void btnAlignSelected_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        new AlignVariants().AlignSelectedVariants();
    }

    private void btnReport_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        string strPath = Util.ProjPath();
        //"C:\\Users\\rlyne\\AppData\\Local"
        if (!Directory.Exists(strPath))
            Directory.CreateDirectory(strPath);
        StreamWriter LogWriter = new StreamWriter(strPath + "VarLog.txt", false);
        LogWriter.WriteLine("");
        LogWriter.WriteLine("");
        foreach (var item in Report)
        {
            LogWriter.WriteLine(item);

        }
        LogWriter.Close();
        LogWriter.Dispose();
    }
}

//Header:
//Date:                     2/21/2018
//Time:                     1:15:12 PM
//Filename:            N:\22668_Lucy_L_Ralph_MEB\Designs\Eng\Backplane\sch-_pwb-\LRalph_BP_Eng_pwb-.PcbDoc

//Variant Pattern Report:

//Section 1:
//Parts with Duplicate RefDes on Top & Bottom
//Side       Refdes Variant                  FootPrint PartNumber       CentX CentY    Rotation


//Section 2:
//Variants with offset Centroid locations
//Side       Refdes Variant                  FootPrint PartNumber       CentX CentY    Rotation


//Section 3:
//Aligned Variant parts
//Side       Refdes Variant                  FootPrint PartNumber       CentX CentY    Rotation
