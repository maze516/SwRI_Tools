using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCB;
using DXP;
using NLog;

public class LayerStackTable
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    IPCB_ServerInterface PCBServer;
    int OffsetX = 0, OffsetY = 0;
    TV6_Layer ActiveLayer;
    List<IPCB_ElectricalLayer> Layers = new List<IPCB_ElectricalLayer>();

    public void PlaceLayerStackTable()
    {

        PCBServer = PCB.GlobalVars.PCBServer;
        List<IPCB_Primitive> lstLayerStackPrims = new List<IPCB_Primitive>();

        //DXP.Utils.RunCommand("PCB:ManageLayerSets", "SetIndex=0");

        IPCB_Board pcbBoard = Util.GetCurrentPCB();

        if (pcbBoard == null)
            return;

        if (!pcbBoard.ChooseLocation(ref OffsetX, ref OffsetY, "Select placement location"))
            return;

        IPCB_MasterLayerStack LStack = pcbBoard.GetState_MasterStack();

        //IPCB_Text test = pcbBoard.SelectedObjectsCount

        ActiveLayer = TV6_Layer.eV6_DrillDrawing;

        IPCB_LayerObject tmp;

        tmp = LStack.First(TLayerClassID.eLayerClass_Electrical);

        do
        {
            Layers.Add(tmp as IPCB_ElectricalLayer);

            //GetState_CopperThickness
            //GetState_LayerName
            tmp = LStack.Next(TLayerClassID.eLayerClass_Electrical, tmp);
        } while (tmp != null);



        // [Page Number, Layer Num, Ext, Name, POS/NEG, Weight]
        List<TableData> TableText = new List<TableData>();

        string CopperWeight;
        TableText.Add(new TableData(new string[6] { "1", null, "GD1", "FABRICATION DRAWING", "POS", null }, TV6_Layer.eV6_DrillDrawing));
        TableText.Add(new TableData(new string[6] { "2", null, "GTO", "Top Overlay", "POS", null }, TV6_Layer.eV6_TopOverlay));
        TableText.Add(new TableData(new string[6] { "3", null, "GTS", "Top Solder", "NEG", null }, TV6_Layer.eV6_TopSolder));
        int i = 4;
        int Sig = 1, Plane = 1;
        string Ext;
        foreach (IPCB_ElectricalLayer item in Layers)
        {
            _Log.Debug(item.GetState_CopperThickness());
            _Log.Debug(item.GetState_LayerDisplayName(0));
            _Log.Debug(item.GetState_LayerDisplayName(1));
            _Log.Debug(item.GetState_LayerDisplayName(2));
            //14000
            //7000
            switch (item.GetState_CopperThickness())
            {
                case 7090:
                    CopperWeight = ".5 oz";
                    break;
                case 7000:
                    CopperWeight = ".5 oz";
                    break;
                case 7087:
                    CopperWeight = ".5 oz";
                    break;
                case 13780:
                    CopperWeight = "1 oz";
                    break;
                case 14000:
                    CopperWeight = "1 oz";
                    break;
                case 13779:
                    CopperWeight = "1 oz";
                    break;
                case 28000:
                    CopperWeight = "2 oz";
                    break;
                case 27560:
                    CopperWeight = "2 oz";
                    break;
                default:
                    CopperWeight = EDP.Utils.CoordToMils(item.GetState_CopperThickness()).ToString() + "mils";
                    break;
            }

            if (item.GetState_LayerDisplayName(0).Contains("P"))
            {
                Ext = "GP" + Plane.ToString();
                Plane++;
            }
            else if (item.GetState_LayerDisplayName(0).Contains("T"))
                Ext = "G" + item.GetState_LayerDisplayName(0);
            else if (item.GetState_LayerDisplayName(0).Contains("B"))
                Ext = "G" + item.GetState_LayerDisplayName(0);
            else
            {
                Ext = "G" + Sig.ToString();
                Sig++;
            }
            //TV6_Layer tempLayer = (TV6_Layer)item;
            TableText.Add(new TableData(new string[6] { i.ToString(),
                (i - 3).ToString(),
                Ext,
                item.GetState_LayerName(),
                item.GetState_LayerDisplayName(0).Contains("P") ?"NEG":"POS",
                CopperWeight }, item.V6_LayerID()));
            i++;
        }

        TableText.Add(new TableData(new string[6] { (LStack.Count_1(3) + 4).ToString(), null, "GBS", "Bottom Solder", "NEG", null }, TV6_Layer.eV6_BottomSolder));
        TableText.Add(new TableData(new string[6] { (LStack.Count_1(3) + 5).ToString(), null, "GBO", "Bottom Overlay", "POS", null }, TV6_Layer.eV6_BottomOverlay));
        TableText.Add(new TableData(new string[6] { "", null, "TXT", "NC DRILL FILE", null, null }, TV6_Layer.eV6_DrillDrawing));

        double pos = -815;
        foreach (TableData item in TableText)
        {// [Page Number, Layer Num, Ext, Name, POS/NEG, Weight]
            if (item.Text[0] != "") lstLayerStackPrims.Add(CreateString(161, pos, item.Text[0], ActiveLayer)); //Page Number
            if (item.Text[1] != "") lstLayerStackPrims.Add(CreateString(661, pos, item.Text[1], ActiveLayer)); //Layer Num
            if (item.Text[2] != "") lstLayerStackPrims.Add(CreateString(1061, pos, item.Text[2], ActiveLayer)); //Ext
            if (item.Text[3] != "") lstLayerStackPrims.Add(CreateString(1516, pos, item.Text[3], ActiveLayer)); //Name
            if (item.Text[4] != "") lstLayerStackPrims.Add(CreateString(3361, pos, item.Text[4], ActiveLayer)); //POS/NEG
            if (item.Text[5] != "") lstLayerStackPrims.Add(CreateString(3861, pos, item.Text[5], ActiveLayer)); //Weight

            if (item.Text[0] != "1")
            {
                if (item.Text[0] != "") lstLayerStackPrims.Add(CreateString(4485, pos, ".Layer_Name", item.CurrentLayer)); //Layer name
                if (item.Text[0] != "") lstLayerStackPrims.Add(CreateString(5985, pos, item.Text[0], item.CurrentLayer)); //Page Number
            }
            pos -= 200;
        }


        tmp = LStack.First(TLayerClassID.eLayerClass_Dielectric);
        double thickness;
        pos = -1515;
        do
        {
            _Log.Debug((tmp as IPCB_DielectricLayer).GetState_DielectricType());
            if ((tmp as IPCB_DielectricLayer).GetState_DielectricType() != TDielectricType.eSurfaceMaterial)
            {
                thickness = EDP.Utils.CoordToMils((tmp as IPCB_DielectricLayer).GetState_DielectricHeight()) / 1000;
                thickness = Math.Round(thickness, 4);
                lstLayerStackPrims.Add(CreateString(-737, pos, thickness.ToString() + "\"", ActiveLayer));
                lstLayerStackPrims.Add(CreateTrack(-238, pos + 50, -50, pos + 50, ActiveLayer));

                pos -= 200;
            }
            tmp = LStack.Next(TLayerClassID.eLayerClass_Dielectric, tmp);
        } while (tmp != null);



        PCBServer.PreProcess();

        lstLayerStackPrims.AddRange(CreateTemplate(LStack.Count_1(3)));



        //tmpPrim = PCBServer.PCBObjectFactory(TObjectId.eTrackObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default);

        //if (tmpPrim == null)
        //    return;

        //IPCB_Track track = tmpPrim as IPCB_Track;




        foreach (IPCB_Primitive prim in lstLayerStackPrims)
        {
            if (prim != null)
                pcbBoard.AddPCBObject(prim);
        }

        PCBServer.PostProcess();

    }
    struct TableData
    {
        public string[] Text;
        public TV6_Layer CurrentLayer;

        public TableData(string[] Data, TV6_Layer Layer)
        {
            Text = Data;
            CurrentLayer = Layer;
        }
    }

    List<IPCB_Primitive> CreateTemplate(int LayerCount)
    {

        List<IPCB_Primitive> lstWorkingSet = new List<IPCB_Primitive>();

        //Top
        lstWorkingSet.Add(CreateTrack(0, 0, 4285, 0, ActiveLayer));
        //Right
        lstWorkingSet.Add(CreateTrack(4285, 0, 4285, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Left
        lstWorkingSet.Add(CreateTrack(0, 0, 0, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Bottom
        lstWorkingSet.Add(CreateTrack(0, -(2015 + (LayerCount * 200)), 4285, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Sheet Col
        lstWorkingSet.Add(CreateTrack(456, -400, 456, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Layer Col
        lstWorkingSet.Add(CreateTrack(961, -400, 961, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Ext Col
        lstWorkingSet.Add(CreateTrack(1411, -400, 1411, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Layer Name Col
        lstWorkingSet.Add(CreateTrack(3224, -400, 3224, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Image Col
        lstWorkingSet.Add(CreateTrack(3749, -400, 3749, -(2015 + (LayerCount * 200)), ActiveLayer));
        //Bot Title
        lstWorkingSet.Add(CreateTrack(0, -400, 4285, -400, ActiveLayer));
        //Bot Label
        lstWorkingSet.Add(CreateTrack(0, -590, 4285, -590, ActiveLayer));

        //lstWorkingSet.Add(CreateTRack(OffsetX + 0, OffsetY + 0, OffsetX + 0, OffsetY + 0, Layer));

        lstWorkingSet.Add(CreateString(1662, -165, "ARTWORK SCHEDULE", ActiveLayer));
        lstWorkingSet.Add(CreateString(1537, -340, "FILM INDEX AND STACKUP", ActiveLayer));
        lstWorkingSet.Add(CreateString(45, -555, "SHEET", ActiveLayer));
        lstWorkingSet.Add(CreateString(511, -555, "LAYER", ActiveLayer));
        lstWorkingSet.Add(CreateString(1061, -555, "EXT", ActiveLayer));
        lstWorkingSet.Add(CreateString(1811, -555, "LAYER NAME", ActiveLayer));
        lstWorkingSet.Add(CreateString(3311, -555, "IMAGE", ActiveLayer));
        lstWorkingSet.Add(CreateString(3811, -555, "CU WT", ActiveLayer));



        lstWorkingSet.Add(CreateString(-838, -815, "SUGGESTED", ActiveLayer));
        lstWorkingSet.Add(CreateString(-838, -965, "DIELECTRIC", ActiveLayer));
        lstWorkingSet.Add(CreateString(-838, -1115, "THICKNESS", ActiveLayer));

        //Dielectric Line
        lstWorkingSet.Add(CreateTrack(-838, -1165, -138, -1165, ActiveLayer));

        return lstWorkingSet;
    }

    IPCB_Primitive CreateTrack(double x1, double y1, double x2, double y2, TV6_Layer Layer)
    {
        IPCB_Primitive tmpPrim = PCBServer.PCBObjectFactory(TObjectId.eTrackObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default);
        IPCB_Track track = tmpPrim as IPCB_Track;

        track.SetState_X1(OffsetX + EDP.Utils.MilsToCoord(x1 - 4285));
        track.SetState_Y1(OffsetY + EDP.Utils.MilsToCoord(y1));
        track.SetState_X2(OffsetX + EDP.Utils.MilsToCoord(x2 - 4285));
        track.SetState_Y2(OffsetY + EDP.Utils.MilsToCoord(y2));

        track.SetState_Layer(Layer);
        track.SetState_Width(EDP.Utils.MilsToCoord(20));

        return tmpPrim;
    }

    IPCB_Primitive CreateString(double x1, double y1, string Text, TV6_Layer Layer)
    {
        if (Text == null) return null;
        IPCB_Primitive tmpPrim = PCBServer.PCBObjectFactory(TObjectId.eTextObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default);
        IPCB_Text text = tmpPrim as IPCB_Text;

        text.SetState_XLocation(OffsetX + EDP.Utils.MilsToCoord(x1 - 4285));
        text.SetState_YLocation(OffsetY + EDP.Utils.MilsToCoord(y1));
        text.SetState_Text(Text);
        text.SetState_FontID(2);
        text.SetState_Width(100000);
        text.SetState_Size(950000);

        text.SetState_Layer(Layer);

        return tmpPrim;
    }
    //EDP.Utils.CoordToMils((tmp as IPCB_ElectricalLayer).GetState_CopperThickness())
    //0.709 1/2 oz
    //1.378 1 oz

    //int x=0, y=0;
    //pcbBoard.ChooseLocation(ref x,ref  y, "Select placement location");
    //return;
    private static IPCB_Component Place(IPCB_Component component)
    {
        DXP.Utils.RunCommand("PCB:DeSelect", "Scope = All");

        var pcbServer = PCB.GlobalVars.PCBServer;

        var pcbBoard = pcbServer?.GetCurrentPCBBoard();

        if (pcbBoard == null)

            return null;



        component.SetState_Board(pcbBoard);

        pcbBoard.AddPCBObject(component);

        component.SetState_Selected(true);



        DXP.Utils.RunCommand("PCB:PlaceComponent", "RepositionSelected=True");




        if (component.GetState_XLocation().Equals(-1) || component.GetState_YLocation().Equals(-1))

        {

            pcbBoard.RemovePCBObject(component);

            component = null;

        }

        else

        {

            component.SetState_Selected(false);

            component.ResetDisplacement();

            pcbBoard.GraphicalView_ZoomRedraw();

        }

        return component;

    }
}
//.Layer_Name