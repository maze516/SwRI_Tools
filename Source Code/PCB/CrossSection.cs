using DXP;
using PCB;
using System;
using EDP;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using NLog;

public partial class CrossSection : Form//ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetCurrentClassLogger();
    public const string PanelName = "CrossSection";
    public const string PanelCaption = "Cross Section Calc";
    Dictionary<string, LayerValues> Nets;
    Dictionary<string, RuleValues> Rules;
    public CrossSection()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
        Nets = new Dictionary<string, LayerValues>();
        Rules = new Dictionary<string, RuleValues>();
        LoadData();
        Loadrules();
    }
    double Solve(double width, double height, int spokes)
    {
        return (width * height) * spokes;
    }

    //void GetNets()
    //{
    //    try
    //    {
    //        IPCB_Board Board;
    //        IPCB_BoardIterator BoardIterator;
    //        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
    //        IPCB_Net Net;
    //        Board = Util.GetCurrentPCB();
    //        if (Board == null)
    //            return;

    //        //Iterate theough all nets on the board.
    //        BoardIterator = Board.BoardIterator_Create();
    //        PCB.TObjectSet FilterSet = new PCB.TObjectSet();

    //        //Filter for nets only.
    //        FilterSet.Add(PCB.TObjectId.eNetObject);
    //        BoardIterator.AddFilter_ObjectSet(FilterSet);
    //        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
    //        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

    //        cboNets.Items.Clear();
    //        Net = (IPCB_Net)BoardIterator.FirstPCBObject();
    //        while (Net != null)
    //        {
    //            //Net.GetState_ReliefConductorWidth()
    //            //    Net.GetState_LayerUsed
    //            //Net.GetState_ReliefEntries()
    //            string netname = Net.GetState_Name();
    //            int cnt = 0;
    //            cboNets.Items.Add(Net.GetState_Name());
    //            foreach (TV6_Layer layer in PCBConstant.V6InternalPlanes)
    //            {
    //                if (Net.GetState_LayerUsed(layer))
    //                    cnt++;
    //            }
    //            Net = (IPCB_Net)BoardIterator.NextPCBObject();
    //        }
    //        //Iterator clean-up
    //        Board.BoardIterator_Destroy(ref BoardIterator);
    //    }
    //    catch (Exception ex)
    //    {
    //        ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);

    //    }

    //}

    void LoadData()
    {
        IPCB_Board Board = Util.GetCurrentPCB();
        if (Board == null)
            return;

        try
        {
            //PCB.IPCB_BoardHelper.GetState_InternalPlaneNetName(this PCB.IPCB_Board, PCB.V7_LayerBase)
            IPCB_LayerStack_V7 tmpLayerstack = Board.GetState_LayerStack_V7();
            IPCB_LayerObject_V7 objLayer;
            objLayer = tmpLayerstack.FirstLayer();
            IPCB_InternalPlane IPlane;
            while (objLayer != null)
            {
                if (objLayer.LayerID().ToString().Contains("InternalPlane"))
                {
                    IPlane = (IPCB_InternalPlane)objLayer;

                    //if (IPlane.GetState_NetName() == "(Multiple Nets)")
                    //{

                    //}
                    //else
                    //{
                        if (!Nets.ContainsKey(IPlane.GetState_NetName()))
                            Nets.Add(IPlane.GetState_NetName(), new LayerValues(1, EDP.Utils.CoordToMils(objLayer.GetState_CopperThickness())));
                        else
                        {
                            cboNets.Items.Add(IPlane.GetState_NetName());
                            Nets[IPlane.GetState_NetName()].Thickness += EDP.Utils.CoordToMils(objLayer.GetState_CopperThickness());
                            Nets[IPlane.GetState_NetName()].Count++;
                        }
                    //}
                }
                objLayer = tmpLayerstack.NextLayer(objLayer);
            }


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


    void Loadrules()
    {
        IPCB_Board Board;
        IPCB_BoardIterator BoardIterator;
        Board = Util.GetCurrentPCB();
        if (Board == null)
            return;
        BoardIterator = Board.BoardIterator_Create();
        try
        {
            IPCB_Rule Rule;
            IClient Client = DXP.GlobalVars.Client;
            IPCB_PowerPlaneConnectStyleRule ConnStyleRule;
            //Iterate through all rules
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            FilterSet.Add(PCB.TObjectId.eRuleObject); //Filter for rules only
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
            Rule = (IPCB_Rule)BoardIterator.FirstPCBObject();
            //Step through all rules.
            while (Rule != null)
            {
                if (Rule.GetState_RuleKind() == TRuleKind.eRule_PowerPlaneConnectStyle)
                {
                    ConnStyleRule = (IPCB_PowerPlaneConnectStyleRule)Rule;
                    if (Rules.ContainsKey(ConnStyleRule.GetState_Name()))
                        DXP.Utils.ShowError("Multiple rules of the same name.", "Rule Error");
                    else
                    {
                        Rules.Add(ConnStyleRule.GetState_Name(), new RuleValues(EDP.Utils.CoordToMils(ConnStyleRule.GetState_ReliefConductorWidth()), ConnStyleRule.GetState_ReliefEntries()));
                        cboRules.Items.Add(ConnStyleRule.GetState_Name());
                    }

                    cboRules.Items.Add(ConnStyleRule.GetState_Name());
                }
                Rule = (IPCB_Rule)BoardIterator.NextPCBObject();
            }


            //Generate report.
            //File.WriteAllLines(Util.ProjPath() + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "-Rules.do", (string[])Report.ToArray(typeof(string)));
            //Client.ShowDocument(Client.OpenDocument("Text", Util.ProjPath() + "\\" + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "-Rules.do"));
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
        }

    }


}

class RuleValues
{
    public double Width;
    public int Spokes;
    public RuleValues(double width, int spokes)
    {
        Width = width;
        Spokes = spokes;
    }

}
class LayerValues
{
    public int Count;
    public double Thickness;
    public LayerValues(int cnt, double thick)
    {
        Count = cnt;
        Thickness = thick;
    }
}