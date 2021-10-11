using DXP;
using EDP;
using NLog;
using PCB;
using SCH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

public class HeightReport
{
    public static readonly Logger _Log = LogManager.GetCurrentClassLogger();
    /// <summary>
    /// Generates a report comparing schematic height parameter to footprint body height.
    /// </summary>
    /// <param name="Update">Update body heights to schematic height parameter.</param>
    public void GetReport(bool Update = false)
    {
        try
        {

            Dictionary<string, Heights> CompHeights = new Dictionary<string, Heights>();
            DXP.Utils.StatusBarSetState(2, "Scanning Documents");
            ScanDocuments(ref CompHeights);
            if (CompHeights != null)
            {
                DXP.Utils.StatusBarSetState(2, "Generating CSV");
                GenerateCSV(ref CompHeights);
                DXP.Utils.StatusBarSetState(2, "Generating Report");
                GenerateReport(ref CompHeights);

                if (Update)
                    UpdateBodies(CompHeights);
            }
            DXP.Utils.PercentFinish();
            DXP.Utils.StatusBarSetStateDefault();
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


    /// <summary>
    /// Get the height parameter value from all schematic components.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    void GetParamHeights(ref Dictionary<string, Heights> argHeights)
    {
        try
        {
            ISch_ServerInterface schServer = SCH.GlobalVars.SchServer;
            if (schServer == null)
                return;
            ISch_Document currentSheet = schServer.GetCurrentSchDocument();

            SCH.TObjectSet objectSet = new SCH.TObjectSet();
            objectSet.Add(SCH.TObjectId.eSchComponent);
            ISch_Iterator iterator = currentSheet.SchIterator_Create();
            iterator.AddFilter_ObjectSet(objectSet);

            ISch_Component schComponent = iterator.FirstSchObject() as ISch_Component;
            while (schComponent != null)
            {

                ObtainParamHeight(ref argHeights, schComponent);
                if (argHeights == null) return;
                schComponent = iterator.NextSchObject() as ISch_Component;
            }
            return;
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

    /// <summary>
    /// Get the body heights of all the footprints on the PCB.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    void GetBodyHeights(ref Dictionary<string, Heights> argHeights)
    {
        try
        {
            IPCB_Component Component; // component object
            IPCB_BoardIterator BoardIterator;
            IPCB_Board Board; // document board object

            Board = Util.GetCurrentPCB();
            if (Board == null)
                return;
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            //Filter for components only
            FilterSet.Add(PCB.TObjectId.eComponentObject);
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            //Iterate through all components looking for components.
            Component = (IPCB_Component)BoardIterator.FirstPCBObject();
            while (Component != null)
            {
                if (Component.GetState_SourceDesignator() == "U18")
                    _Log.Debug(Component.GetState_SourceDesignator());
                ObtainBodyHeight(ref argHeights, Component);
                if (argHeights == null) return;
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }

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

    /// <summary>
    /// Gets height parameter of specified component.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    /// <param name="argComponent">Component to get height from.</param>
    private void ObtainParamHeight(ref Dictionary<string, Heights> argHeights, ISch_Component argComponent)
    {
        try
        {
            ISch_Designator designator = argComponent.GetState_SchDesignator();
            ISch_Iterator paramIterator = argComponent.SchIterator_Create();
            paramIterator.SetState_IterationDepth((int)SCH.TIterationDepth.eIterateAllLevels);
            SCH.TObjectSet objectSet = new SCH.TObjectSet();
            objectSet.Add(SCH.TObjectId.eParameter);
            paramIterator.AddFilter_ObjectSet(objectSet);

            if (argComponent.GetState_CurrentPartID() > 1) return;

            //Make sure component is already logged.
            if (!argHeights.ContainsKey(designator.GetState_Text()))
                argHeights.Add(designator.GetState_Text(), new Heights());
            else
            {
                argHeights = null;
                DXP.Utils.ShowWarning("A duplicate refdes detected. Opeartion will stop. Please correct this issue.");
                return;
            }

            argHeights[designator.GetState_Text()].Library = argComponent.GetState_LibraryIdentifier() + "/" + argComponent.GetState_DesignItemId();

            //Go through all parameters looking for component height.
            ISch_Parameter param = paramIterator.FirstSchObject() as ISch_Parameter;
            while (param != null)
            {
                if (param.GetState_Name() == "ComponentHeight")
                {
                    if (param.GetState_Text() == null)
                    {
                        argComponent.SchIterator_Destroy(ref paramIterator);
                        return;
                    }
                    int height;
                    if (Int32.TryParse(param.GetState_Text(), out height))
                        argHeights[designator.GetState_Text()].ParameterHeight = height;
                    argComponent.SchIterator_Destroy(ref paramIterator);
                    return;
                }
                param = paramIterator.NextSchObject() as ISch_Parameter;
            }
            argComponent.SchIterator_Destroy(ref paramIterator);
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


    /// <summary>
    /// Gets body height of specified footprint.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    /// <param name="argComponent">Footprint to get height from.</param>
    private void ObtainBodyHeight(ref Dictionary<string, Heights> argHeights, IPCB_Component argComponent)
    {
        try
        {
            IPCB_GroupIterator CompIterator;
            IPCB_Primitive CompItem;
            IPCB_ComponentBody Body;
            bool FoundBody = false;

            CompIterator = argComponent.GroupIterator_Create();
            CompIterator.AddFilter_LayerSet_2(PCBConstant.V7AllLayersSet);
            CompItem = CompIterator.FirstPCBObject();
            if (argComponent.GetState_SourceDesignator() == null)
            {
                DialogResult result = MessageBox.Show("Footprint " + argComponent.GetState_Name().GetState_ConvertedString() + " has no source designator. This is cause by a footprint not linked to a schematic symbol.\nDo you wish to continue?", "Unlinked Footprint", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (result == DialogResult.Yes)
                    argHeights.Add(argComponent.GetState_Name().GetState_ConvertedString(), new Heights() { BodyHeight = 0 });
                else
                {
                    argHeights = null;
                    return;
                }
            }
            argHeights[argComponent.GetState_Name().GetState_ConvertedString()].Footprint = argComponent.GetState_Pattern();
            while (CompItem != null)
            {
                if (CompItem.GetState_ObjectIDString() == "ComponentBody")
                {
                    Body = CompItem as IPCB_ComponentBody;
                    if (FoundBody)
                    {
                        //DXP.Utils.ShowError("Multiple bodies.\nDo Something.");
                        if (argHeights[argComponent.GetState_Name().GetState_ConvertedString()].BodyHeight < EDP.Utils.CoordToMils(GetCompHeight(Body)))
                        {
                            argHeights[argComponent.GetState_Name().GetState_ConvertedString()].BodyHeight = EDP.Utils.CoordToMils(GetCompHeight(Body));
                        }
                    }
                    else
                    {
                        if (argComponent.GetState_SourceDesignator() == "JA1" || argComponent.GetState_SourceDesignator() == "JB6" || argComponent.GetState_SourceDesignator() == "U2EM")
                            MessageBox.Show("temp");
                        FoundBody = true;
                        argHeights[argComponent.GetState_Name().GetState_ConvertedString()].BodyHeight = EDP.Utils.CoordToMils(GetCompHeight(Body));
                    }

                }
                CompItem = CompIterator.NextPCBObject();
            }
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

    /// <summary>
    /// Update the body heights of all fooprints with bodies.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    void UpdateBodies(Dictionary<string, Heights> argHeights)
    {
        try
        {
            frmHeightReport HeightForm = new frmHeightReport();
            HeightForm.FillList(argHeights);
            if (HeightForm.ListCount() == 0)
            {
                DXP.Utils.ShowInfo("There are no components to update.");
                return;
            }
            DialogResult tmp = HeightForm.ShowDialog();
            if (tmp == DialogResult.OK)
            {
                List<string> tmpList = HeightForm.GetSelectedComponents();
                SetBodyHeight(argHeights, tmpList);
            }
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    /// <param name="tmpList"></param>
    private void SetBodyHeight(Dictionary<string, Heights> argHeights, List<string> tmpList)
    {

        //?Body.GetState_DescriptorString()
        //"3D Extruded  (Mechanical 2, Bot Assy)  Standoff=0mil  Overall=163mil  (26522.112mil, 20350.482mil)"
        //? Body.GetState_DescriptorString()
        //"3D STEP SW3dPS-74VHC32 - 14lead IC SOIC (Mechanical 2, Bot Assy)  Standoff=-0.5mil  Overall=68.5mil  (26522.107mil, 20347.985mil)"
        //argComponent.GetState_Name().GetState_ConvertedString() "U18EM" string
        //Body.GetModel().GetFileName()   "SW3dPS-74VHC32 - 14lead IC SOIC.STEP"  string

        IPCB_Component Component; // component object
        IPCB_BoardIterator BoardIterator;
        IPCB_Board Board; // document board object
        IPCB_GroupIterator CompIterator;
        IPCB_Primitive CompItem;
        bool FoundBody = false;

        Board = Util.GetCurrentPCB(true);
        if (Board == null)
            return;

        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        //Iterate through all components looking for components.
        Component = (IPCB_Component)BoardIterator.FirstPCBObject();
        while (Component != null)
        {
            if (tmpList.Contains(Component.GetState_SourceDesignator()))
            {
                if (Component.GetState_SourceDesignator() == "JA1" || Component.GetState_SourceDesignator() == "JB6" || Component.GetState_SourceDesignator() == "U2EM")
                    MessageBox.Show("temp");
                //argComponent.GetState_SourceDesignator()
                CompIterator = Component.GroupIterator_Create();
                CompIterator.AddFilter_LayerSet_2(PCBConstant.V7AllLayersSet);
                CompItem = CompIterator.FirstPCBObject();

                IPCB_ComponentBody Body, temp;
                FoundBody = false;
                Body = null;
                while (CompItem != null)
                {
                    if (CompItem.GetState_ObjectIDString() == "ComponentBody")
                    {
                        temp = CompItem as IPCB_ComponentBody;
                        if (!FoundBody)
                        {
                            FoundBody = true;
                            Body = temp;
                        }
                        else
                        {
                            if (GetCompHeight(Body) < GetCompHeight(temp))
                            {
                                Body = temp;
                            }
                        }
                    }
                    CompItem = CompIterator.NextPCBObject();
                }

                Component.BeginModify();
                SetCompHeight(Body, EDP.Utils.MilsToCoord(argHeights[Component.GetState_SourceDesignator()].ParameterHeight));
                Component.EndModify();

                Body = null;
            }
            Component = (IPCB_Component)BoardIterator.NextPCBObject();
        }


        try
        {



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


    /// <summary>
    /// Generate text height report.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    private void GenerateReport(ref Dictionary<string, Heights> argHeights)
    {
        try
        {//Generating Report
            DXP.Utils.PercentInit("Generating Report", argHeights.Count);

            ArrayList report = new ArrayList();
            report.Add("Component Heights Report");
            report.Add("========================");
            foreach (KeyValuePair<string, Heights> item in argHeights)
            {
                report.Add("Component " + item.Key);
                report.Add(item.Value.ToString());
                report.Add("");

                DXP.Utils.PercentUpdate();
            }

            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject = CurrentWorkspace.DM_FocusedProject();

            string fileName = CurrentProject.DM_GetOutputPath() + "\\HeightReport.txt";
            File.WriteAllLines(fileName, (string[])report.ToArray(typeof(string)));

            IServerDocument reportDocument = DXP.GlobalVars.Client.OpenDocument("Text", fileName);
            if (reportDocument != null)
            {
                CurrentProject.DM_AddGeneratedDocument(fileName);
                DXP.GlobalVars.Client.ShowDocument(reportDocument);
            }

            DXP.Utils.PercentFinish();
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

    /// <summary>
    /// Generate CSV height report.
    /// </summary>
    /// <param name="argHeights">Reference to the dict storing report info.</param>
    private void GenerateCSV(ref Dictionary<string, Heights> argHeights)
    {
        try
        {
            DXP.Utils.PercentInit("Generating CSV", argHeights.Count);
            int i = 4;
            ArrayList report = new ArrayList();
            report.Add("Excel Component Heights Report");
            report.Add("========================");
            report.Add("Ref,Footprint,Symbol Height (mils),Body Height (mils),Difference (Sym-Body),Library");
            foreach (KeyValuePair<string, Heights> item in argHeights)
            {
                report.Add(item.Key + "," +
                    item.Value.Footprint + "," +
                    ((item.Value.ParameterHeight < 0) ? "N/A" : item.Value.ParameterHeight.ToString()) + "," +
                    ((item.Value.BodyHeight < 0) ? "N/A" : item.Value.BodyHeight.ToString()) + "," +
                    ("=C" + i + "-D" + i) + "," +
                    item.Value.Library);
                i++;

                DXP.Utils.PercentUpdate();
            }

            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject = CurrentWorkspace.DM_FocusedProject();

            string fileName = CurrentProject.DM_GetOutputPath() + "\\HeightReport.csv";
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            File.WriteAllLines(fileName, (string[])report.ToArray(typeof(string)));
            IServerDocument reportDocument = DXP.GlobalVars.Client.OpenDocument("Text", fileName);
            if (reportDocument != null)
            {
                CurrentProject.DM_AddGeneratedDocument(fileName);
                DXP.GlobalVars.Client.ShowDocument(reportDocument);
            }

            DXP.Utils.PercentFinish();
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="report"></param>
    void ScanDocuments(ref Dictionary<string, Heights> report)
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

            int BoardCount = 0;
            string BoardName = "";
            DialogResult dlgResult;

            DXP.Utils.PercentInit("Scanning Documents", LogicalDocumentCount);

            bool DocOpened = false;
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "PCB")
                    if (BoardCount == 0)
                    {
                        BoardCount += 1;
                        BoardName = CurrentSheet.DM_FileName();
                    }
                    else
                    {

                        dlgResult = MessageBox.Show("There are multiple boards in this project. PCB outjobs will only run on the first board :" + BoardName + "\n Do you wish to continue?", "Multiple PCBs", MessageBoxButtons.YesNo);
                        if (dlgResult == DialogResult.No)
                        {
                            report = null;
                            return;
                        }
                    }
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
                    GetParamHeights(ref report);

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                    if (report == null) return;
                }
                DocOpened = false;

                DXP.Utils.PercentUpdate();
            }

            DXP.Utils.PercentFinish();

            DXP.Utils.PercentInit("Scanning Documents", LogicalDocumentCount);

            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "PCB")
                {
                    if (BoardName == CurrentSheet.DM_FileName())
                    {
                        DocOpened = false;
                        IPCB_Board PCBDoc = CurrentSheet as IPCB_Board;
                        if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                        {
                            ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                            DocOpened = true;
                        }
                        else
                            ServerDoc = Client.OpenDocument("PCB", CurrentSheet.DM_FullPath());

                        Client.ShowDocument(ServerDoc);
                        GetBodyHeights(ref report);

                        if (!DocOpened)
                            Client.CloseDocument(ServerDoc);

                        ServerDoc = null;
                    }
                }
                DXP.Utils.PercentUpdate();
            }
            Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));

            DXP.Utils.PercentFinish();

            return;
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

    /// <summary>
    /// Gets the total height of a body.
    /// </summary>
    /// <param name="Body">Body to adjust</param>
    /// <returns>Height of body in AD coords</returns>
    int GetCompHeight(IPCB_ComponentBody Body)
    {
        double RotX, RotY, RotZ;
        int StandOff;

        if (Body.GetState_DescriptorString().Contains("STEP"))
        {
            Body.GetModel().GetState(out RotX, out RotY, out RotZ, out StandOff);
            Body.SaveModelToFile("C:\\test.step");
            return Body.GetOverallHeight();
        }
        else
            return Body.GetOverallHeight();

    }

    /// <summary>
    /// Sets the body height to the new value supplied.
    /// </summary>
    /// <param name="Body">Body to adjust</param>
    /// <param name="value">Desired height in AD coords</param>
    void SetCompHeight(IPCB_ComponentBody Body, int value)
    {
        double RotX, RotY, RotZ;
        int StandOff;

        Body.BeginModify();

        if (Body.GetState_DescriptorString().Contains("STEP"))
        {//todo: alter offset dont override
            Body.GetModel().GetState(out RotX, out RotY, out RotZ, out StandOff);
            Body.GetModel().SetState(RotX, RotY, RotZ, StandOff + value - Body.GetOverallHeight());//todo: value = coord or mils?
        }
        else
            Body.SetOverallHeight(value);
        Body.EndModify();

    }
}

/// <summary>
/// 
/// </summary>
public class Heights
{
    private double intBodyHeight = -1;
    private double intParamHeight = -1;
    private string strLibrary = "";
    private string strFootprint = "";
    public double BodyHeight { get { return intBodyHeight; } set { intBodyHeight = value; } }
    public double ParameterHeight { get { return intParamHeight; } set { intParamHeight = value; } }
    public string Library { get { return strLibrary; } set { strLibrary = value; } }
    public string Footprint { get { return strFootprint; } set { strFootprint = value; } }
    public override string ToString()
    {
        string output = "";
        output = "Body Height:";
        if (BodyHeight < 0)
            output += "N/A";
        else
            output += BodyHeight + " mils";
        output += ", Parameter Height: ";
        if (ParameterHeight < 0)
            output += "N/A";
        else
            output += ParameterHeight + " mils.";
        return output;
    }
}