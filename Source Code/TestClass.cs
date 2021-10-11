using NLog;
using DXP;
using EDP;
using PCB;
using SCH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Reflection;
using System.Text;

class TestClass
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    string[] LogFile = new string[100000];
    public void ScanDocuments()//ref Dictionary<string, Heights> report)
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            int LogicalDocumentCount;
            int LoopIterator;
            IDXPDocument CurrentSheet;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();


            ISch_Document SchDoc;
            IClient Client = DXP.GlobalVars.Client;
            IServerDocument ServerDoc;
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

            DXP.Utils.PercentInit("Scanning Documents", LogicalDocumentCount);

            bool DocOpened = false;
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                //if (CurrentSheet.DM_DocumentKind() == "PCB")
                //    if (BoardCount == 0)
                //    {
                //        BoardCount += 1;
                //        BoardName = CurrentSheet.DM_FileName();
                //    }
                //    else
                //    {

                //        dlgResult = MessageBox.Show("There are multiple boards in this project. PCB outjobs will only run on the first board :" + BoardName + "\n Do you wish to continue?", "Multiple PCBs", MessageBoxButtons.YesNo);
                //        if (dlgResult == DialogResult.No)
                //        {
                //            report = null;
                //            return;
                //        }
                //    }
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    DocOpened = false;
                    SchDoc = CurrentSheet as ISch_Document;
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

                    Client.ShowDocument(ServerDoc);
                    //GetParamHeights(ref report);

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                    //if (report == null) return;
                }
                DocOpened = false;


                DXP.Utils.PercentUpdate();
            }

            DXP.Utils.PercentFinish();
            return;
            //DXP.Utils.PercentInit("Scanning Documents", LogicalDocumentCount);

            //for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            //{
            //    CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
            //    if (CurrentSheet.DM_DocumentKind() == "PCB")
            //    {
            //        if (BoardName == CurrentSheet.DM_FileName())
            //        {
            //            DocOpened = false;
            //            IPCB_Board PCBDoc = CurrentSheet as IPCB_Board;
            //            if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
            //            {
            //                ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
            //                DocOpened = true;
            //            }
            //            else
            //                ServerDoc = Client.OpenDocument("PCB", CurrentSheet.DM_FullPath());

            //            Client.ShowDocument(ServerDoc);
            //            GetBodyHeights(ref report);

            //            if (!DocOpened)
            //                Client.CloseDocument(ServerDoc);

            //            ServerDoc = null;
            //        }
            //    }
            //    DXP.Utils.PercentUpdate();
            //}
            //Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));

            //DXP.Utils.PercentFinish();

            //return;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    public void LibraryTest()
    {
        //Library testing
        ArrayList report = new ArrayList();

        IIntegratedLibraryManager integratedLibraryManager = EDP.Utils.LoadIntegratedLibraryManager();
        int count = integratedLibraryManager.InstalledLibraryCount();
        report.Add("Installed Libray Count : " + count);
        for (int i = 0; i < count; i++)
        {
            string libPath = integratedLibraryManager.InstalledLibraryPath(i);
            int componentCount = integratedLibraryManager.GetComponentCount(libPath);

            DXP.Utils.PercentInit("Generating Library Report: Library " + i + " of " + count, componentCount);

            report.Add("=============================================");
            report.Add("Installed Libray Path : " + libPath);
            report.Add("Component Count : " + componentCount);
            for (int j = 0; j < componentCount; j++)
            {
                string componentName = integratedLibraryManager.GetComponentName(libPath, j);
                int paramCount = integratedLibraryManager.GetParameterCount(libPath, j);
                report.Add(" ------------------------------------");
                report.Add(" Component Name : " + componentName);
                report.Add(" Parameter Count : " + paramCount);
                //report.Add(" Footprint : " + integratedLibraryManager.GetModelName(libPath, j, 0));
                for (int k = 0; k < paramCount; k++)
                {
                    report.Add(" " + integratedLibraryManager.GetParameterName(libPath, j, k) + " = " + integratedLibraryManager.GetParameterValue(libPath, j, k));
                }
                DXP.Utils.PercentUpdate();
            }
        }

        string fileName = "C:\\InstalledLibraries.txt";
        File.WriteAllLines(fileName, (string[])report.ToArray(typeof(string)));

        IServerDocument reportDocument = DXP.GlobalVars.Client.OpenDocument("Text", fileName);
        if (reportDocument != null)
            DXP.GlobalVars.Client.ShowDocument(reportDocument);
    }

    public void Variants()
    {
        //IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();
        //EDP.IWorkspace wrokspace = edp
        IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
        IProjectVariant Variant;
        IComponentVariation CompVariant;
        IParameterVariation ParamVariant;


        int l = 0;
        //MessageBox.Show(project.DM_ProjectVariantCount().ToString());
        for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
        {
            LogFile[l] = project.DM_ProjectVariants(i).DM_Description();
            l++;
            Variant = project.DM_ProjectVariants(i);
            for (int j = 0; j < Variant.DM_VariationCount(); j++)
            {
                CompVariant = Variant.DM_Variations(j);
                LogFile[l] = CompVariant.DM_PhysicalDesignator() + " Param Count: " + CompVariant.DM_ParameterCount() + " Var Count: " + CompVariant.DM_VariationCount() + "\n";
                for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                {
                    ParamVariant = CompVariant.DM_Variations(k);
                    LogFile[l] += "Var Kind: " + CompVariant.DM_VariationKind().ToString() + " Desc: " + ParamVariant.DM_ShortDescriptorString() + "\n";

                }

                l++;
            }
        }
        _Log.Debug(LogFile);
        //degub:
        //EDP.IOutputer test;
        //for (int x = 0; x < OutJobDoc.GetState_OutputerCount(); x++)
        //    MessageBox.Show(OutJobDoc.GetState_Outputer(x).DM_ViewName());

        //if (project.DM_ProjectVariantCount() > 0)
        //{
        //    //cboVariant.Enabled = true;
        //    //cboVariant.Items.Add("[No Variations]");

        //    //for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
        //    //    cboVariant.Items.Add(project.DM_ProjectVariants(i).DM_Description());
        //    //cboVariant.SelectedIndex = 0;
        //}
        //else
        //cboVariant.Enabled = false;
    }

    public void GetCompData()
    {
        try
        {
            IPCB_Board Board;
            IPCB_BoardIterator BoardIterator;
            IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
            IPCB_Net Net;
            Board = Util.GetCurrentPCB();
            if (Board == null)
                return;

            //Iterate theough all components on the board.
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            //Filter for components only.
            FilterSet.Add(PCB.TObjectId.eNetObject);
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            //Component.GetState_ObjectID().ToString()
            //Component.GetState_ObjectIDString()
            int l = 0;
            Net = (IPCB_Net)BoardIterator.FirstPCBObject();
            //Component = (IPCB_Component)BoardIterator.FirstPCBObject();
            while (Net != null)
            {
                LogFile[l] = Net.GetState_ObjectIDString() + " " + Net.GetState_ObjectID().ToString();
                l++;
                //Component.BeginModify();
                //Component.SetState_NameOn(NameOn); //Show or hide refdes.
                //Component.EndModify();

                Net = (IPCB_Net)BoardIterator.NextPCBObject();
                //Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
            //Board.GraphicalView_ZoomRedraw();
            _Log.Debug(LogFile);
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

    public PCB.TObjectSet cstmPCBObject = new PCB.TObjectSet(new PCB.TObjectId[]
    {
        PCB.TObjectId.eArcObject,           //1
        PCB.TObjectId.ePadObject,           //2
        PCB.TObjectId.eViaObject,           //3
        PCB.TObjectId.eTrackObject,         //4
        PCB.TObjectId.eTextObject,          //5
        PCB.TObjectId.eFillObject,          //6
        PCB.TObjectId.eConnectionObject,    //7
        PCB.TObjectId.eNetObject,           //8
        PCB.TObjectId.eComponentObject,     //9
        PCB.TObjectId.ePolyObject,          //10
        PCB.TObjectId.eRegionObject,        //11
        PCB.TObjectId.eCoordinateObject,    //14
        PCB.TObjectId.eFromToObject,        //17
        PCB.TObjectId.eDifferentialPairObject, //18
        PCB.TObjectId.eEmbeddedObject,      //20
        PCB.TObjectId.eEmbeddedBoardObject, //21
        PCB.TObjectId.eSplitPlaneObject,    //22
        PCB.TObjectId.eTraceObject,         //23
        PCB.TObjectId.eSpareViaObject,      //24
        PCB.TObjectId.eBoardObject,         //25
    });


    public void Command_GetxSignalInfo()//Const View : IServerDocumentView; Var Parameters : WideString)
    {
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        IPCB_Board Board;
        IPCB_BoardIterator Iterator;
        IPCB_ObjectClass2 ObjectClass;
        IPCB_PinPairsManager PinPairsManager;
        IPCB_PinPair PinPair;
        //int I;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        IPCB_Primitive Prim;
        IPCB_Pad Pad;
        if (PCBServer == null)
            return;

        Board = PCBServer.GetCurrentPCBBoard();
        if (Board == null)
            return;

        PinPairsManager = Board.GetState_PinPairsManager();
        if (PinPairsManager == null)
            return;

        PinPairsManager.InvalidateAll();

        Iterator = Board.BoardIterator_Create();
        try
        {
            Iterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
            Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);

            ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
            while (ObjectClass != null)
            {
                if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
                {
                    Report.Add("xSignal Class : " + ObjectClass.GetState_DisplayName());

                    for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
                    {

                        PinPair = PinPairsManager.GetState_PinPairs(I);

                        if (ObjectClass.IsMember(PinPair.GetState_Name()))
                        {
                            //need primitive info
                            //refdes and pin number

                            Report.Add(String.Format("    xSignal : {0}, Node Count : {1}, Signal Length : {2}mils, Routed Length : {3}mils, Unrouted Length : {4}mils, Primitive Count : {5}",

                                               PinPair.GetState_Name(),
                                        PinPair.GetState_NodeCount(),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_Length()),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_RoutedLength()),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_UnroutedLength()),
                                        PinPair.GetState_PrimitivesCount()));

                            for (int j = 0; j <= PinPair.GetState_PrimitivesCount() - 1; j++)
                            {
                                Prim = PinPair.GetPrimitives(j);
                                if (Prim.GetState_DescriptorString().StartsWith("Pad")) //need refdes, pin number, net name
                                {
                                    Pad = (IPCB_Pad)Prim; //Pad.GetState_PinDescriptorString() = "U6-C17"
                                    Report.Add("        Pin: " + Pad.GetState_PinDescriptorString() + ", Net: " + Prim.GetState_Net().GetState_Name());
                                    //Report.Add("        Primative Desc: " + Prim.GetState_DescriptorString() + ", Net : " + Prim.GetState_Net().GetState_Name());
                                }
                            }
                        }
                    }
                }

                ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
            }

            File.WriteAllLines("C:\\xSignals.txt", (string[])Report.ToArray(typeof(string)));

            //Client.OpenDocument("Text", "C:\\xSignals.txt");
        }
        finally
        {
            Board.BoardIterator_Destroy(ref Iterator);
            //FreeAndnull(Report);
        }
    }


    public void CollectNetClasses()
    {

        int i;
        IPCB_Board Board;
        IPCB_BoardIterator ClassIterator;
        IPCB_ObjectClass NetClass;
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;

        Board = PCBServer.GetCurrentPCBBoard();
        ClassIterator = Board.BoardIterator_Create();
        ClassIterator.SetState_FilterAll();
        ClassIterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
        NetClass = (IPCB_ObjectClass)ClassIterator.FirstPCBObject();
        while (NetClass != null)
        {
            i = 0;
            if (NetClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Net)
            {
                Report.Add("NetClass Name: " + NetClass.GetState_Name());
                while (NetClass.GetState_MemberName(i) != null)
                {
                    Report.Add("    Member: " + NetClass.GetState_MemberName(i));
                    i++;
                }

            }
            NetClass = (IPCB_ObjectClass)ClassIterator.NextPCBObject();
        }
        Board.BoardIterator_Destroy(ref ClassIterator);
        File.WriteAllLines("C:\\NetClass.txt", (string[])Report.ToArray(typeof(string)));
        //Client.OpenDocument("Text", "C:\\NetClass.txt");
    }

    public void GetRules()
    {

        IPCB_Board Board;
        IPCB_Rule Rule;
        IPCB_BoardIterator BoardIterator;
        ArrayList Report = new ArrayList();

        Board = Util.GetCurrentPCB();
        if (Board == null)
            return;

        //Iterate through all rules
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        FilterSet.Add(PCB.TObjectId.eRuleObject); //Filter for rules only
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
        Rule = (IPCB_Rule)BoardIterator.FirstPCBObject();
        //Step through all rules to find one matching RuleName.
        while (Rule != null)
        {
            Report.Add("Rule : " + Rule.GetState_Name() + ", Desc : " + Rule.GetState_DescriptorString() + ", Detail : " + Rule.GetState_DetailString());
            Rule = (IPCB_Rule)BoardIterator.NextPCBObject();
        }

        Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
        File.WriteAllLines("C:\\Rules.txt", (string[])Report.ToArray(typeof(string)));
        return; //No match found.

    }

    public void ModelTest()
    {
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;


        IPCB_ComponentBody STEPmodel = (IPCB_ComponentBody)PCBServer.PCBObjectFactory(PCB.TObjectId.eComponentBodyObject, TDimensionKind.eNoDimension, PCB.TObjectCreationMode.eCreate_Default);

        IPCB_Model Model = STEPmodel.ModelFactory_FromFilename("C:\\test.step", false);

        STEPmodel.SetState_FromModel();

        Model.SetState(90, 100, 110, 12000);

        double RotX;
        double RotY;
        double RotZ;
        int StandOff;
        Model.GetState(out RotX, out RotY, out RotZ, out StandOff); //here occurs the error!!!

        STEPmodel.SetModel(Model);

        IPCB_Component Component = null;
        Component.AddPCBObject(STEPmodel);



        ////This code produces the same error:


        //CIter:= Component.GroupIterator_Create;

        //    CIter.AddFilter_ObjectSet(MkSet(eComponentBodyObject));

        //STEPmodel:= CIter.FirstPCBObject;

        //    While(STEPmodel <> nil) do

        //        begin


        //   StepModel.GetModel.GetState(RotX, RotY, RotZ, StandOff); //here occurs the error!!!

        //STEPmodel:= CIter.NextPCBObject;

        //    end;
    }

    public void BoundingBoxTest()
    {
        IPCB_BoardIterator BoardIterator;
        IPCB_Component Component;
        string RefDes;//, Varriant, Footprint, CompID;
        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        double OriginX = EDP.Utils.CoordToMils(Board.GetState_XOrigin());
        double OriginY = EDP.Utils.CoordToMils(Board.GetState_YOrigin());

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
            if (RefDes == "U7")
            {
                while (true)
                {
                    MessageBox.Show("X: " + Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Right) - OriginX) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Left) - OriginX)) + " Y: " + Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Top) - OriginX) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Bottom) - OriginX)));

                    //MessageBox.Show("X: " + (EDP.Utils.CoordToMils(Component.BoundingRectangle().Lx) - OriginX) + " Y: " + (EDP.Utils.CoordToMils(Component.BoundingRectangle().Ly) - OriginY));
                    //MessageBox.Show("X: " + (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Lx) - OriginX) + " Y: " + (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Ly) - OriginY));
                }
            }
            Component = (IPCB_Component)BoardIterator.NextPCBObject();
        }
        //Iterator clean-up
        Board.BoardIterator_Destroy(ref BoardIterator);

    }


    public void PrimPrimTest()
    {
        IPCB_BoardIterator BoardIterator;
        IPCB_Pad Pad, Selected1 = null, Selected2 = null;
        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        double OriginX = EDP.Utils.CoordToMils(Board.GetState_XOrigin());
        double OriginY = EDP.Utils.CoordToMils(Board.GetState_YOrigin());

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.ePadObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);


        Pad = (IPCB_Pad)BoardIterator.FirstPCBObject();

        while (Pad != null)
        {
            //RefDes = Component.GetState_Name().GetState_Text();
            //Determines if component is a variant.
            if (Selected1 == null && Pad.GetState_Selected() == true)
                Selected1 = Pad;
            else if (Pad.GetState_Selected() == true)
                Selected2 = Pad;
            if (Selected1 != null && Selected2 != null)
                break;


            Pad = (IPCB_Pad)BoardIterator.NextPCBObject();
        }
        if (Selected1 == null || Selected2 == null)
            return;

        bool bot = false, top = false;

        if (Selected1.GetState_Layer() == TV6_Layer.eV6_BottomLayer)
        {
            Selected1.SetState_Layer(TV6_Layer.eV6_TopLayer);
            top = true;
        }

        if (Selected2.GetState_Layer() == TV6_Layer.eV6_BottomLayer)
        {
            Selected2.SetState_Layer(TV6_Layer.eV6_TopLayer);
            bot = true;
        }

        MessageBox.Show(EDP.Utils.CoordToMMs(Board.PrimPrimDistance(Selected1, Selected2)).ToString());

        if (top)
            Selected1.SetState_Layer(TV6_Layer.eV6_BottomLayer);

        if (bot)
            Selected2.SetState_Layer(TV6_Layer.eV6_BottomLayer);

        //Iterator clean-up
        Board.BoardIterator_Destroy(ref BoardIterator);


    }

    public void Command_GetAllComponents(IServerDocumentView view, ref string parameters)
    {
        ComponentList<string, string> CompList = new ModBom_IO().GetComponents();

        IClient client = DXP.GlobalVars.Client;
        IWorkspace workSpace = client.GetDXPWorkspace() as IWorkspace;
        IProject project = workSpace.DM_FocusedProject();
        if (project.DM_NeedsCompile())
            project.DM_Compile();

        List<string> designators = new List<string>();

        IDocument document = project.DM_DocumentFlattened();
        int componentCount = document.DM_ComponentCount();
        for (int i = 0; i < componentCount; i++)
        {
            IComponent component = document.DM_Components(i);
            designators.Add(component.DM_PhysicalDesignator());
            //component.DM_NexusDeviceId()
        }

        return;
    }

    public void RegionBuilder()
    {


        var pcbServer = PCB.GlobalVars.PCBServer;
        if (pcbServer == null)
        {
            return;
        }

        var pcbBoard = pcbServer.GetCurrentPCBBoard();
        if (pcbBoard == null)
        {
            return;
        }

        int OffsetX = 0, OffsetY = 0;
        if (!pcbBoard.ChooseLocation(ref OffsetX, ref OffsetY, "Select placement location"))
            return;

        var layer = new V7_Layer(TV6_Layer.eV6_Mechanical1);

        //GenerateRegion(pcbServer, pcbBoard, layer, OffsetX, OffsetY);
        CommandPlaceRectRegion();




    }


    private static void GenerateRegion(IPCB_ServerInterface pcbServer, IPCB_Board pcbBoard, V7_Layer layer, int OffX, int OffY)
    {
        var segments = new List<IPolySegment> {
        CreateLinearPolySegment(0+OffX, 0+OffY),
        CreateLinearPolySegment(EDP.Utils.MilsToCoord(1000)+OffX, 0+OffY),
        CreateLinearPolySegment(EDP.Utils.MilsToCoord(1000)+OffX, EDP.Utils.MilsToCoord(1000)+OffY),
        CreateLinearPolySegment(0+OffX, EDP.Utils.MilsToCoord(1000)+OffY)
        };

        var polygonObject = (IPCB_Polygon)CreatePcbObject(pcbServer, PCB.TObjectId.ePolyObject);
        polygonObject.SetState_V7Layer(layer);
        polygonObject.SetState_PointCount(segments.Count);
        for (var i = 0; i < segments.Count; i++)
        {
            polygonObject.SetState_Segments(i, segments[i]);
        }

        var GeoPoly = pcbServer.PCBContourMaker().MakeContour_8(polygonObject, 0, layer.SafeV6Layer());

        var region = (IPCB_Region)CreatePcbObject(pcbServer, PCB.TObjectId.eRegionObject);
        region.SetState_V7Layer(layer);
        region.SetState_Kind(TRegionKind.eRegionKind_Copper);
        region.SetGeometricPolygon(GeoPoly);
        pcbBoard.AddPCBObject(region);
    }

    private static PolySegment CreateLinearPolySegment(int vx, int vy)
    {
        return new PolySegment
        {
            Kind = TPolySegmentType.ePolySegmentLine,
            Vx = vx,
            Vy = vy,
            Cx = 100,
            Cy = 100,
            Radius = 0,
            Angle1 = 0.0,
            Angle2 = 0.0
        };
    }

    private static IPCB_Primitive CreatePcbObject(IPCB_ServerInterface pcbServer, PCB.TObjectId objectId, TDimensionKind dimensionKind = TDimensionKind.eNoDimension)
    {
        return pcbServer.PCBObjectFactory(objectId,
            dimensionKind, PCB.TObjectCreationMode.eCreate_Default);
    }

    public void CommandPlaceRectRegion()
    {

        IPCB_ServerInterface pcbServer = PCB.GlobalVars.PCBServer;
        if (pcbServer == null)
            return;

        // retrieve the interface representing the PCB document
        IPCB_Board board = pcbServer.GetCurrentPCBBoard();
        if (board == null)
            return;

        IPCB_Primitive primitive = pcbServer.PCBObjectFactory(PCB.TObjectId.eRegionObject, TDimensionKind.eNoDimension, PCB.TObjectCreationMode.eCreate_Default);
        if (primitive == null)
            return;

        IPCB_Region region = primitive as IPCB_Region;

        region.SetState_Kind(TRegionKind.eRegionKind_Copper);
        region.SetState_Layer(TV6_Layer.eV6_Mechanical1);

        IPCB_Contour Contour = region.GetMainContour().Replicate();

        Contour.SetState_Count(4);
        Contour.SetState_PointX(1, EDP.Utils.MilsToCoord(1000));
        Contour.SetState_PointY(1, EDP.Utils.MilsToCoord(1000));

        Contour.SetState_PointX(2, EDP.Utils.MilsToCoord(1000));
        Contour.SetState_PointY(2, EDP.Utils.MilsToCoord(2000));

        Contour.SetState_PointX(3, EDP.Utils.MilsToCoord(2000));
        Contour.SetState_PointY(3, EDP.Utils.MilsToCoord(2000));

        Contour.SetState_PointX(4, EDP.Utils.MilsToCoord(2000));
        Contour.SetState_PointY(4, EDP.Utils.MilsToCoord(1000));

        region.SetOutlineContour(Contour);
        board.AddPCBObject(region);

    }


    public void loggerTesting()
    {
        //string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        //NLog.LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(assemblyFolder + "\\NLog.config", true);

        Util.UpdateLogger(ToolsPreferences.LoggerLevel);

        try
        {
            _Log.Trace("Trace");
            _Log.Debug("Debug");
            _Log.Info("Info");
            _Log.Warn("Warn");
            _Log.Error("Error");

            System.Console.ReadKey();
        }
        catch (Exception ex)
        {
            _Log.Fatal(ex, "Fatal");
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
        }

        //var sb = new StringBuilder();
        //sb.AppendLine("");
        //sb.AppendLine(@"   ------------ [ ApiClientSettings ] -------------");
        //sb.AppendLine(@"     ClientId            : " + "ClientId");
        //sb.AppendLine(@"     ClientSecret        : " + "ClientSecret");
        //sb.AppendLine(@"     RedirectUri         : " + "RedirectUri");
        //sb.AppendLine(@"     AccessToken         : " + "AccessToken");
        //sb.AppendLine(@"     RefreshToken        : " + "RefreshToken");
        //sb.AppendLine(@"     ExpirationDateTime  : " + "ExpirationDateTime");
        //sb.AppendLine(@"   ---------------------------------------------");
        //_Log.Fatal(sb);
        //return sb.ToString();

    }


}



