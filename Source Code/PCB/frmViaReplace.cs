using DXP;
using NLog;
using PCB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


public partial class frmViaReplace : ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    private IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
    private IPCB_Board Board = null;
    private bool SelectedOnly = false;

    public const string PanelName = "ViaReplace";
    public const string PanelCaption = "Via Replace";

    public frmViaReplace()
    {
        _Log.Debug("frmViaReplace");

        InitializeComponent();
        UI.ApplyADUITheme(this);

    }

    private void LoadLists()
    {
        _Log.Debug("LoadLists");

        PCBServer = PCB.GlobalVars.PCBServer;

        if (PCBServer == null)
        {
            MessageBox.Show("Error opening PCB.");
            return;
        }


        Board = Util.GetCurrentPCB();
        if (Board == null)
            MessageBox.Show("Invalid Board");

        IPCB_DrillLayerPair DrillPair = null;

        lstBefore.Items.Clear();
        lstAfter.Items.Clear();

        for (int i = 0; i < Board.GetState_DrillLayerPairsCount(); i++)
        {
            DrillPair = Board.GetState_LayerPair(i);
            lstBefore.Items.Add(DrillPair.GetState_Description());
            lstAfter.Items.Add(DrillPair.GetState_Description());
        }

    }
    private void btnReplaceAll_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnReplaceAll_Click");

        DXP.Utils.StatusBarSetState(2, "Replacing Vias");
        if (lstAfter.SelectedItems.Count < 1 || lstBefore.SelectedItems.Count < 1)
        {
            MessageBox.Show("Please select an initial via and replacement via.");
            return;
        }

        bool Succeed = false;

        if (lstBefore.SelectedItems.Count == 1)
            Succeed = SingleViaReplace();
        else if (lstBefore.SelectedItems.Count > 1 && lstAfter.SelectedItems.Count == 1)
            Succeed = MultiBeforeViaReplace();
        else if (lstBefore.SelectedItems.Count > 1 && lstAfter.SelectedItems.Count > 1)
        {
            MessageBox.Show("Unable to replace multiple vias with multiple other vias.\r\nPlease only select one drill pair on the Initial Drill Pair list or the New Drill Pair list.");
            return;
        }

        if (Succeed)
            MessageBox.Show("Process Complete");
        else
            MessageBox.Show("Unable to complete. Error occured.");
    }

    private bool SingleViaReplace()
    {
        _Log.Debug("SingleViaReplace");

        if (PCBServer == null)
            return false;

        IPCB_BoardIterator BoardIterator;
        IPCB_Via Via;

        Board = Util.GetCurrentPCB();

        if (Board == null)
            return false;

        BoardIterator = Board.BoardIterator_Create();

        //Iterate theough all components on the board.
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eViaObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        IPCB_DrillLayerPair OldPair = null, DrillPair = null;
        List<IPCB_DrillLayerPair> NewPairs = new List<IPCB_DrillLayerPair>();

        for (int i = 0; i < Board.GetState_DrillLayerPairsCount(); i++)
        {
            DrillPair = Board.GetState_LayerPair(i);
            if (lstBefore.SelectedItem.ToString() == DrillPair.GetState_Description())
            {
                OldPair = DrillPair;
            }
            if (lstAfter.SelectedItems.Contains(DrillPair.GetState_Description()))
            {
                NewPairs.Add(DrillPair);
            }
        }

        Via = BoardIterator.FirstPCBObject() as IPCB_Via;

        Board.BeginModify();

        while (Via != null)
        {
            if (Via.GetState_StartLayer() == OldPair.GetState_StartLayer() && Via.GetState_StopLayer() == OldPair.GetState_StopLayer())
            {
                if (SelectedOnly)
                {
                    if (Via.GetState_Selected())
                        ReplaceVia(Via, NewPairs);
                }
                else
                    ReplaceVia(Via, NewPairs);
            }
            Via = BoardIterator.NextPCBObject() as IPCB_Via;
        }

        Board.EndModify();

        Board.BoardIterator_Destroy(ref BoardIterator);

        return true;
    }

    private bool MultiBeforeViaReplace()
    {
        _Log.Debug("MultiBeforeViaReplace");

        //if (PCBServer == null)
        //    return false;

        IPCB_BoardIterator BoardIterator;
        List<IPCB_Via> BoardVias = new List<IPCB_Via>();
        IPCB_Via Via;

        Board = Util.GetCurrentPCB();

        if (Board == null)
            return false;

        BoardIterator = Board.BoardIterator_Create();

        //Iterate theough all components on the board.
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eViaObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        IPCB_DrillLayerPair DrillPair = null;
        List<IPCB_DrillLayerPair> OldPairs = new List<IPCB_DrillLayerPair>();
        List<IPCB_DrillLayerPair> NewPairs = new List<IPCB_DrillLayerPair>();

        for (int i = 0; i < Board.GetState_DrillLayerPairsCount(); i++)
        {
            DrillPair = Board.GetState_LayerPair(i);
            if (lstBefore.SelectedItems.Contains(DrillPair.GetState_Description()))
            {
                OldPairs.Add(DrillPair);
            }
            if (lstAfter.SelectedItems.Contains(DrillPair.GetState_Description()))
            {
                NewPairs.Add(DrillPair);
            }
        }

        //Collect board vias that meet requirements
        Via = BoardIterator.FirstPCBObject() as IPCB_Via;
        while (Via != null)
        {
            foreach (IPCB_DrillLayerPair OldPair in OldPairs)
            {
                if (Via.GetState_StartLayer() == OldPair.GetState_StartLayer() && Via.GetState_StopLayer() == OldPair.GetState_StopLayer())
                {
                    if (SelectedOnly)
                    {
                        if (Via.GetState_Selected())
                        {
                            BoardVias.Add(Via);
                            break;
                        }
                    }
                    else
                    {
                        BoardVias.Add(Via);
                        break;
                    }
                }
            }
            Via = BoardIterator.NextPCBObject() as IPCB_Via;
        }

        Board.BoardIterator_Destroy(ref BoardIterator);

        DXP.Utils.PercentInit("Replacing Vias", BoardVias.Count);//Progressbar init.
        //Replace vias

        Board.BeginModify();

        IPCB_Net Net = null;
        int X, Y;
        List<IPCB_Via> Replaced = new List<IPCB_Via>();
        while (BoardVias.Count > 0)
        {
            X = BoardVias[0].GetState_XLocation();
            Y = BoardVias[0].GetState_YLocation();
            Net = BoardVias[0].GetState_Net();
            Replaced.Add(BoardVias[0]);

            for (int i = 1; i < BoardVias.Count; i++)
            {
                foreach (IPCB_DrillLayerPair OldPair in OldPairs)
                {
                    if (BoardVias[i].GetState_Net() == Net)
                        if (BoardVias[i].GetState_XLocation() == X)
                            if (BoardVias[i].GetState_YLocation() == Y)
                                if (BoardVias[i].GetState_StartLayer() == OldPair.GetState_StartLayer())
                                    if (BoardVias[i].GetState_StopLayer() == OldPair.GetState_StopLayer())
                                    {
                                        if (SelectedOnly)
                                        {
                                            if (BoardVias[i].GetState_Selected())
                                            {
                                                Replaced.Add(BoardVias[i]);
                                            }
                                        }
                                        else
                                            Replaced.Add(BoardVias[i]);
                                    }
                }
            }
            if (Replaced.Count == lstBefore.SelectedItems.Count)
                ReplaceVia(Replaced[0], NewPairs, false);

            //Remove replaced vias from BoardVias (clean up)
            foreach (IPCB_Via OldVia in Replaced)
            {
                DXP.Utils.PercentUpdate();
                BoardVias.Remove(OldVia);
                if (Replaced.Count == lstBefore.SelectedItems.Count)
                    Board.RemovePCBObject(OldVia);
            }
            Replaced.Clear();



        }

        Board.EndModify();

        DXP.Utils.PercentFinish();
        return true;

    }

    private void ReplaceVia(IPCB_Via OldVia, List<IPCB_DrillLayerPair> NewPairs, bool RemoveOld = true)
    {
        _Log.Debug("ReplaceVia");

        IPCB_Via NewVia;
        PCBServer = PCB.GlobalVars.PCBServer;

        if (PCBServer == null)
            return;

        foreach (IPCB_DrillLayerPair Pair in NewPairs)
        {
            NewVia = PCBServer.PCBObjectFactory(TObjectId.eViaObject, TDimensionKind.eCenterDimension, TObjectCreationMode.eCreate_Default) as IPCB_Via;
            NewVia.SetState_Net(OldVia.GetState_Net());
            NewVia.SetState_XLocation(OldVia.GetState_XLocation());
            NewVia.SetState_YLocation(OldVia.GetState_YLocation());
            //NewVia.SetState_Layer(Via.GetState_Layer());
            NewVia.SetState_HighLayer(Pair.GetState_StartLayer().V7_LayerID());
            NewVia.SetState_LowLayer(Pair.GetState_StopLayer().V7_LayerID());

            if (radMetric.Checked)
            {
                NewVia.SetState_HoleSize(EDP.Utils.MMsToCoord((double)numDrill.Value));
                NewVia.SetState_Size(EDP.Utils.MMsToCoord((double)numPad.Value));
            }
            else
            {
                NewVia.SetState_HoleSize(EDP.Utils.MilsToCoord((double)numDrill.Value));
                NewVia.SetState_Size(EDP.Utils.MilsToCoord((double)numPad.Value));
            }

            Board.AddPCBObject(NewVia as IPCB_Primitive);

        }
        if (RemoveOld)
            Board.RemovePCBObject(OldVia);
    }
    /*
    ?Via.GetState_DescriptorString()
    "Via (7525mil,1070mil) from Top Layer to Bottom Layer"
    ?Via.GetState_DetailString()
    "Net:BANK_1_2_D3 Size: 32mil Hole: 12mil"
    ?Via.GetState_HoleSize()
    120000
    ?Via.GetState_Size()
    320000
    ?Via.GetState_XLocation()
    ?Via.GetState_YLocation()
    ?Via.GetState_Net()
    */



    private void btnSelected_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnSelected_Click");

        SelectedOnly = true;
        btnReplaceAll.PerformClick();
        SelectedOnly = false;
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnSelect_Click");

        if (PCBServer == null)
            return;

        IPCB_BoardIterator BoardIterator;
        IPCB_Via Via;

        Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        BoardIterator = Board.BoardIterator_Create();

        //Iterate theough all components on the board.
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eViaObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        IPCB_DrillLayerPair OldPair = null, DrillPair = null;
        List<IPCB_DrillLayerPair> NewPairs = new List<IPCB_DrillLayerPair>();

        for (int i = 0; i < Board.GetState_DrillLayerPairsCount(); i++)
        {
            DrillPair = Board.GetState_LayerPair(i);
            if (lstBefore.SelectedItem.ToString() == DrillPair.GetState_Description())
            {
                OldPair = DrillPair;
            }
        }

        Via = BoardIterator.FirstPCBObject() as IPCB_Via;

        Board.BeginModify();

        while (Via != null)
        {
            if (Via.GetState_StartLayer() == OldPair.GetState_StartLayer() && Via.GetState_StopLayer() == OldPair.GetState_StopLayer())
            {
                Via.SetState_Selected(true);
            }
            Via = BoardIterator.NextPCBObject() as IPCB_Via;
        }

        Board.EndModify();

        Board.BoardIterator_Destroy(ref BoardIterator);
    }

    private void btnUpdateList_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnUpdateList_Click");

        LoadLists();
    }

    private void btnRemoveDupe_Click(object sender, EventArgs e)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);
        DXP.Utils.StatusBarSetState(2, "Removing Vias");

        //if (PCBServer == null)
        //    return false;

        IPCB_BoardIterator BoardIterator;
        List<IPCB_Via> BoardVias = new List<IPCB_Via>();
        List<IPCB_Via> clnBoardVias = new List<IPCB_Via>();
        IPCB_Via Via;

        Board = Util.GetCurrentPCB();

        if (Board == null)
        {
            MessageBox.Show("Unable to complete. Error occured.");
            return;
        }

        BoardIterator = Board.BoardIterator_Create();

        //Iterate theough all components on the board.
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eViaObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        IPCB_DrillLayerPair DrillPair = null;
        List<IPCB_DrillLayerPair> OldPairs = new List<IPCB_DrillLayerPair>();

        for (int i = 0; i < Board.GetState_DrillLayerPairsCount(); i++)
        {
            DrillPair = Board.GetState_LayerPair(i);
            if (lstBefore.SelectedItems.Contains(DrillPair.GetState_Description()))
                OldPairs.Add(DrillPair);

        }

        //Collect board vias that meet requirements
        Via = BoardIterator.FirstPCBObject() as IPCB_Via;
        while (Via != null)
        {
            foreach (IPCB_DrillLayerPair OldPair in OldPairs)
            {
                if (Via.GetState_StartLayer() == OldPair.GetState_StartLayer() && Via.GetState_StopLayer() == OldPair.GetState_StopLayer())
                {
                    BoardVias.Add(Via);
                    break;
                }
            }
            Via = BoardIterator.NextPCBObject() as IPCB_Via;
        }

        Board.BoardIterator_Destroy(ref BoardIterator);

        DXP.Utils.PercentInit("Removing Vias", BoardVias.Count * OldPairs.Count);//Progressbar init.
        //Replace vias

        Board.BeginModify();

        IPCB_Net Net = null;
        int X, Y;
        List<IPCB_Via> Replaced = new List<IPCB_Via>();
        foreach (IPCB_DrillLayerPair OldPair in OldPairs)
        {
            clnBoardVias.AddRange(BoardVias);

            while (clnBoardVias.Count > 0)
            {
                X = clnBoardVias[0].GetState_XLocation();
                Y = clnBoardVias[0].GetState_YLocation();
                Net = clnBoardVias[0].GetState_Net();
                Replaced.Add(clnBoardVias[0]);

                for (int i = 1; i < clnBoardVias.Count; i++)
                {

                    if (clnBoardVias[i].GetState_Net() == Net)
                        if (clnBoardVias[i].GetState_XLocation() == X)
                            if (clnBoardVias[i].GetState_YLocation() == Y)
                                if (clnBoardVias[i].GetState_StartLayer() == OldPair.GetState_StartLayer())
                                    if (clnBoardVias[i].GetState_StopLayer() == OldPair.GetState_StopLayer())
                                    {
                                        Replaced.Add(clnBoardVias[i]);
                                    }
                }


                if (Replaced.Count > 1)
                    //Remove replaced vias from BoardVias (clean up)
                    for (int i = 1; i < Replaced.Count; i++)
                    {
                        clnBoardVias.Remove(Replaced[i]);
                        Board.RemovePCBObject(Replaced[i]);
                        DXP.Utils.PercentUpdate();
                    }
                clnBoardVias.Remove(Replaced[0]);
                DXP.Utils.PercentUpdate();
                Replaced.Clear();
            }
        }

        Board.EndModify();

        DXP.Utils.PercentFinish();
        MessageBox.Show("Process Complete");


    }
}
//        IPCB_Primitive primitive = PCBServer.PCBObjectFactory(TObjectId.eTrackObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default);
//IPCB_Board pcbBoard = Util.GetCurrentPCB();

//PCB.IPCB_BoardHelper.GetState_PadViaLibrary(this PCB.IPCB_Board)
//PCB.IPCB_PadViaLibrary
//PCB.IPCB_PadViaTemplate
