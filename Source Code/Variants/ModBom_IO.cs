using DXP;
using EDP;
using NLog;
using SCH;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

//todo: comment
public partial class ModBom_IO : Form
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public IWSM_OutputJobDocument OutJobDoc;
    public IOutputMedium OutputMedium;

    public ModBom_IO()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
    }
    public void Export()
    {
        try
        {
            ExportModBOM();

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


    private void btnBrowse_Click(object sender, EventArgs e)
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();

            openFileDialog.InitialDirectory = CurrentProject.DM_GetOutputPath(); //Start file browser in project outputs.
            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
                return;

            txtPath.Text = openFileDialog.FileName;
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
    /// Add text to the progress bar.
    /// </summary>
    /// <param name="Text">Text to display on the progress bar.</param>
    public void UpdateLabel(string Text)
    {
        try
        {
            string myString = "N/A";
            if (pbProgress.Maximum != 0) //Stops a divide by 0 error.
                myString = ((pbProgress.Value * 100) / pbProgress.Maximum).ToString();
            myString = Text + " " + myString + "%";
            Graphics canvas = pbProgress.CreateGraphics();
            canvas.DrawString(myString, new Font("Verdana", 8, FontStyle.Bold), new SolidBrush(Color.Black), 20, 4);
            canvas.Dispose();
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

    private void btnImport_Click(object sender, EventArgs e)
    {
        try
        {
            //TODO: need to support hierarchy 
            string Path = txtPath.Text;
            if (Path == "")
            {
                MessageBox.Show("Missing file path.");
                return;
            }

            Var_Type Variants = GetPEData(Path); //Get data from excel.

            if (Variants == null)
                return;

            //Verify data was collected.
            if (Variants.Components.Count == 0)
            {
                MessageBox.Show("There were no PE_FLT or PE_ENG changes detected.");
                return;
            }

            //Disabled for now.
            #region Set Base Design
            //IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            //IDXPProject CurrentProject;
            //int LogicalDocumentCount;
            //int LoopIterator;
            //IDXPDocument CurrentSheet;
            //CurrentProject = CurrentWorkspace.DM_FocusedProject();
            //LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();
            //ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
            //IClient Client = DXP.GlobalVars.Client;
            //IServerDocument ServerDoc;
            //IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

            //bool DocOpened = false;
            //for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            //{
            //    CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
            //    if (CurrentSheet.DM_DocumentKind() == "SCH")
            //    {
            //        DocOpened = false;
            //        if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
            //        {
            //            ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
            //            DocOpened = true;
            //        }
            //        else
            //            ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

            //        //Client.ShowDocument(ServerDoc);

            //        ISch_Lib SchDoc;
            //        SchDoc = SchServer.LoadSchDocumentByPath(CurrentSheet.DM_FullPath()) as ISch_Lib;

            //        ISch_Iterator LibraryIterator, PIterator;
            //        ISch_Component Component;
            //        ISch_Parameter Param;
            //        VarParam<string, string> CompVars = new VarParam<string, string>();
            //        if (SchDoc == null)
            //            return;
            //        //Iterate theough all components on the schematic.
            //        LibraryIterator = SchDoc.SchIterator_Create();
            //        LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

            //        Component = LibraryIterator.FirstSchObject() as ISch_Component;
            //        while (Component != null)
            //        {
            //            if (Variants.Components.ContainsKey(Component.GetState_DesignItemId()))
            //            {
            //                Component.UpdatePart_PreProcess();
            //                CompVars = Variants.Components[Component.GetState_DesignItemId()];
            //                //Iterate theough all parameters in the component.
            //                PIterator = Component.SchIterator_Create();
            //                PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

            //                Param = PIterator.FirstSchObject() as ISch_Parameter;
            //                while (Param != null)
            //                {
            //                    if (Param.GetState_Name() != null)
            //                        if ("PE_ENG" == Param.GetState_Name() || Param.GetState_Name() == "PE_FLT")
            //                        {

            //                            Param.SetState_Text(CompVars[Param.GetState_Name()]);
            //                        }
            //                    Param = PIterator.NextSchObject() as ISch_Parameter;
            //                }
            //                Component.UpdatePart_PostProcess();
            //            }

            //            Component = LibraryIterator.NextSchObject() as ISch_Component;
            //        }

            //        if (ServerDoc.GetModified())
            //            ServerDoc.DoFileSave("");

            //        if (!DocOpened)
            //            Client.CloseDocument(ServerDoc);

            //        ServerDoc = null;

            //    }

            //}

            //Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));
            #endregion

            #region Set Var_PE
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;
            IParameterVariation ParamVariant;

            CheckParams();

            //Stores the library reference value for each refdes.
            ComponentList<string, string> CompList = GetComponents(); //Get list of components in the design.

            //GetComponents() will return null if there is an error.
            if (CompList == null)
            {
                MessageBox.Show("Error getting a list of components. Please check design and try again.", "Component List Error", MessageBoxButtons.OK);
                return;
            }

            string RefDes;
            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
            {
                Variant = project.DM_ProjectVariants(i);
                //Update Var_PE data only.
                if (project.DM_ProjectVariants(i).DM_Description().ToUpper() == "VAR_PE")
                {

                    pbProgress.Maximum = Variant.DM_VariationCount();
                    pbProgress.Value = 0;
                    UpdateLabel("Updating Existing Variants");
                    //PCB.IPCB_Component.GetState_ChannelOffset()
                    //EDP.IProject.DM_ChannelDesignatorFormat()
                    //EDP.IProject.DM_ChannelRoomLevelSeperator()
                    //EDP.IPart.DM_ChannelOffset()
                    //EDP.IDocument.DM_ChannelIndex()
                    //EDP.IDocument.DM_ChannelPrefix()

                    for (int j = 0; j < Variant.DM_VariationCount(); j++)
                    {

                        CompVariant = Variant.DM_Variations(j);
                        RefDes = CompVariant.DM_PhysicalDesignator();
                        //Check that there is data in the Variants list for current refdes.
                        if (CompList.ContainsKey(RefDes))
                            if (Variants.Components.ContainsKey(CompList[RefDes]))
                            {
                                for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                                {
                                    if (CompVariant.DM_VariationKind() != TVariationKind.eVariation_NotFitted)
                                    {
                                        ParamVariant = CompVariant.DM_Variations(k);
                                        //Update component variant parameter data.
                                        if ("PE_ENG" == ParamVariant.DM_ParameterName().ToUpper() || ParamVariant.DM_ParameterName().ToUpper() == "PE_FLT") //Verify a parameter we want.
                                            if (CompList.ContainsKey(RefDes))
                                                if (Variants.Components.ContainsKey(CompList[RefDes]))
                                                    if (Variants.Components[CompList[RefDes]].ContainsKey(ParamVariant.DM_ParameterName().ToUpper())) //Make sure the parameter value is in our list of data.

                                                        //if (Variants.Components[CompList[RefDes]][ParamVariant.DM_ParameterName()] != CompVariant.DM_PhysicalDesignator()) //I dont know what this is for.
                                                        //if (Variants.Components[CompList[RefDes]][ParamVariant.DM_ParameterName().ToUpper()] != "x") //Make sure the parameter data is not 'x' 
                                                        ParamVariant.DM_SetVariedValue(Variants.Components[CompList[RefDes]][ParamVariant.DM_ParameterName().ToUpper()] + "_$");
                                    }
                                }
                                if (CompList.ContainsKey(RefDes))
                                    CompList.Remove(RefDes);
                            }
                        pbProgress.Value = j;
                        UpdateLabel("Updating Existing Variants");
                    }
                    IComponentVariation tmpCompVar;

                    pbProgress.Maximum = CompList.Count;
                    pbProgress.Value = 0;
                    UpdateLabel("Creating New Variants");
                    project.DM_BeginUpdate();
                    //Create new variants.
                    foreach (string CompRef in CompList.Keys)
                    {
                        if (Variants.Components.ContainsKey(CompList[CompRef]))
                        {
                            tmpCompVar = project.DM_ProjectVariants(i).DM_AddComponentVariation();
                            tmpCompVar.DM_SetVariationKind(TVariationKind.eVariation_None);
                            CreateCompVar(ref tmpCompVar, Variants.Components[CompList[CompRef]], CompRef);
                            Variants.Components[CompList[CompRef]].Saved = true;
                        }
                        pbProgress.Value++;
                        UpdateLabel("Creating New Variants");
                    }
                    project.DM_EndUpdate();
                }
            }
            #endregion

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
        finally
        {
            this.Close();
            MessageBox.Show("Process Complete");
        }
    }

    /// <summary>
    /// Create a new component variant.
    /// </summary>
    /// <param name="Dest">Component to create variant for.</param>
    /// <param name="Source">Parameter data.</param>
    /// <param name="RefDes">Refdes of component being modified.</param>
    void CreateCompVar(ref IComponentVariation Dest, VarParam<string, string> Source, string RefDes)
    {
        try
        {
            IParameterVariation tmpParam;
            Dest.DM_SetPhysicalDesignator(RefDes);
            foreach (string key in Source.Keys)
            {
                tmpParam = Dest.DM_AddParameterVariation();
                tmpParam.DM_SetParameterName(key);
                tmpParam.DM_SetVariedValue(Source[key] + "_$");
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
    /// Load parameter data from excel doc.
    /// </summary>
    /// <param name="Path">Path to excel doc.</param>
    /// <returns></returns>
    Var_Type GetPEData(string Path)
    {

        pbProgress.Value = 0;
        UpdateLabel("Opening Excel Doc");
        Excel.Application xlApp;
        Excel.Workbook xlWorkBook = null;
        Excel.Worksheet xlWorkSheet;
        object misValue = System.Reflection.Missing.Value;

        if (Path == "") return null;

        xlApp = new Excel.Application();
        xlWorkBook = xlApp.Workbooks.Open(Path, 0, true, 5, "", "", true, Microsoft.Office.Interop.Excel.XlPlatform.xlWindows, "\t", false, false, 0, true, 1, 0);
        xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets.get_Item(1);

        Var_Type Output = new Var_Type();
        VarParam<string, string> tmpParams;
        string PE_Flt, PE_Eng, LibRef;

        try
        {
            Output.VarName = "ModBOM";

            pbProgress.Maximum = 3;
            UpdateLabel("Getting Column Headers");

            //Get column letters for specific column names.
            PE_Eng = GetColumn("PE_ENG", ref xlWorkSheet);

            pbProgress.Value = 1;
            UpdateLabel("Getting Column Headers");

            PE_Flt = GetColumn("PE_FLT", ref xlWorkSheet);

            pbProgress.Value = 2;
            UpdateLabel("Getting Column Headers");

            LibRef = GetColumn("LIBREF", ref xlWorkSheet);

            pbProgress.Value = 3;
            UpdateLabel("Getting Column Headers");

            if (PE_Eng == "" || PE_Flt == "" || LibRef == "")
            {
                MessageBox.Show("One or more parameters are missing from the BOM. Please try again with a different BOM.");
                xlWorkBook.Close(false, misValue, misValue);
                xlApp.Quit();


                releaseObject(xlWorkSheet);
                releaseObject(xlWorkBook);
                releaseObject(xlApp);
                return null;
            }

            int Row = 2;
            int cnt = 0;
            while (xlWorkSheet.Range[LibRef + Row].Value2 != null)
            {
                Row++;
                cnt++;
            }
            pbProgress.Maximum = cnt;

            pbProgress.Value = 0;
            UpdateLabel("Reading Data");

            Row = 2;
            //Read parameter data.
            while (xlWorkSheet.Range[LibRef + Row].Value2 != null)
            {
                //if (xlWorkSheet.Range[PE_Flt + Row].Value2.ToString() != "x" || xlWorkSheet.Range[PE_Eng + Row].Value2.ToString() != "x")
                //{
                if (Output.Components.ContainsKey(xlWorkSheet.Range[LibRef + Row].Value2.ToString()))
                    MessageBox.Show("Multiple library references for " + xlWorkSheet.Range[LibRef + Row].Value2.ToString() + " in this BOM. First instance is used.");
                else
                {
                    if (xlWorkSheet.Range[PE_Flt + Row].Value2 == null)
                        xlWorkSheet.Range[PE_Flt + Row].Value2 = "x";
                    if (xlWorkSheet.Range[PE_Eng + Row].Value2 == null)
                        xlWorkSheet.Range[PE_Eng + Row].Value2 = "x";

                    if (xlWorkSheet.Range[PE_Flt + Row].Value2.ToString() == "x")
                        xlWorkSheet.Range[PE_Flt + Row].Value2 = "z";
                    if (xlWorkSheet.Range[PE_Eng + Row].Value2.ToString() == "x")
                        xlWorkSheet.Range[PE_Eng + Row].Value2 = "z";
                    tmpParams = new VarParam<string, string>();
                    tmpParams.Add(new KeyValuePair<string, string>("PE_ENG", xlWorkSheet.Range[PE_Eng + Row].Value2.ToString().Replace("'", "")));
                    tmpParams.Add(new KeyValuePair<string, string>("PE_FLT", xlWorkSheet.Range[PE_Flt + Row].Value2.ToString().Replace("'", "")));
                    Output.Components.Add(new KeyValuePair<string, VarParam<string, string>>(xlWorkSheet.Range[LibRef + Row].Value2.ToString(), tmpParams));
                }
                //}
                Row++;
                pbProgress.Value++;
                UpdateLabel("Reading Data");
            }
            //xlWorkBook.Close(false, misValue, misValue);
            //xlApp.Quit();


            //releaseObject(xlWorkSheet);
            //releaseObject(xlWorkBook);
            //releaseObject(xlApp);


            //return Output;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            Output = null;
        }

        xlWorkBook.Close(false, misValue, misValue);
        xlApp.Quit();


        releaseObject(xlWorkSheet);
        releaseObject(xlWorkBook);
        releaseObject(xlApp);
        return Output;

    }

    /// <summary>
    /// Get component list with library IDs.
    /// </summary>
    /// <returns></returns>
    public void CheckParams()
    {
        try
        {
            ComponentList<string, string> Output = new ComponentList<string, string>();
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            int LogicalDocumentCount;
            int LoopIterator;
            IDXPDocument CurrentSheet;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();
            ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
            IClient Client = DXP.GlobalVars.Client;
            IServerDocument ServerDoc;
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

            pbProgress.Maximum = LogicalDocumentCount;
            pbProgress.Value = 0;
            UpdateLabel("Checking Parameters");

            bool DocOpened = false;
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    DocOpened = false;
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

                    ISch_Document SchDoc;
                    SchDoc = SchServer.LoadSchDocumentByPath(CurrentSheet.DM_FullPath()) as ISch_Document;

                    ISch_Iterator LibraryIterator, PIterator;
                    ISch_Component Component;
                    VarParam<string, string> CompVars = new VarParam<string, string>();
                    ISch_Parameter Param;
                    bool Flt = false, Eng = false;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SchDoc.SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;
                    while (Component != null)
                    {
                        //Iterate theough all parameters in the component.
                        PIterator = Component.SchIterator_Create();
                        PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

                        Param = PIterator.FirstSchObject() as ISch_Parameter;
                        while (Param != null)
                        {
                            if (Param.GetState_Name() != null)
                                if ("PE_ENG" == Param.GetState_Name().ToUpper())
                                    Eng = true;
                                else if (Param.GetState_Name().ToUpper() == "PE_FLT")
                                    Flt = true;

                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }

                        if (!Flt)
                        {
                            Param = Component.AddSchParameter();
                            Param.SetState_Name("PE_FLT");
                            Param.SetState_Text("x");
                        }

                        if (!Eng)
                        {
                            Param = Component.AddSchParameter();
                            Param.SetState_Name("PE_ENG");
                            Param.SetState_Text("x");
                        }
                        Flt = false;
                        Eng = false;
                        Component = LibraryIterator.NextSchObject() as ISch_Component;
                    }

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                }
                pbProgress.Value++;
                UpdateLabel("Checking Parameters");
            }

            Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));

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

    public ComponentList<string, string> GetComponents()
    {
        try
        {

            IClient client = DXP.GlobalVars.Client;
            IWorkspace workSpace = client.GetDXPWorkspace() as IWorkspace;
            IProject project = workSpace.DM_FocusedProject();
            if (project.DM_NeedsCompile())
                project.DM_Compile();

            ComponentList<string, string> Output = new ComponentList<string, string>();

            IDocument document = project.DM_DocumentFlattened();
            int componentCount = document.DM_ComponentCount();

            pbProgress.Maximum = componentCount;
            pbProgress.Value = 0;
            UpdateLabel("Getting Comp List");

            for (int i = 0; i < componentCount; i++)
            {//TODO: what to do when multiple keys
                IComponent component = document.DM_Components(i);
                if (!Output.ContainsKey(component.DM_PhysicalDesignator()))
                    Output.Add(component.DM_PhysicalDesignator(), component.DM_NexusDeviceId());
                pbProgress.Value++;
                UpdateLabel("Getting Comp List");
            }

            return Output;

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return null;
        }
    }

    private void releaseObject(object obj)
    {
        try
        {
            System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
            obj = null;
        }
        catch (Exception ex)
        {
            obj = null;
            MessageBox.Show("Unable to release the Object " + ex.ToString());
        }
        finally
        {
            GC.Collect();
        }
    }

    /// <summary>
    /// Get column letters for specific column names.
    /// </summary>
    /// <param name="Name">Column name</param>
    /// <param name="xlWorksheet">Referene to excel document to search.</param>
    /// <returns></returns>
    string GetColumn(string Name, ref Excel.Worksheet xlWorksheet)
    {
        try
        {
            int Col = 65;

            while (Col < 91)
            {
                if (xlWorksheet.Range[((char)Col).ToString() + "1"].Value2 != null)
                    if (xlWorksheet.Range[((char)Col).ToString() + "1"].Value2.ToUpper() == Name.ToUpper())
                        return ((char)Col).ToString();
                Col++;
                if (Col == 91)
                    return "";

            }
            return "";
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return "";
        }
    }
    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }

    /// <summary>
    /// Run ModBOM outjob
    /// </summary>
    void ExportModBOM()
    {
        try
        {
            string process, parameters;
            IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();
            List<IServerDocument> lstServerDocs = new List<IServerDocument>();
            IClient tmpClient = DXP.GlobalVars.Client;

            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;

            List<string> lstOutjobDocPaths = new frmBatchOutjob().GetOutputJobPath(); //Get a list of all outjob docs in the project

            if (lstOutjobDocPaths == null)
                return;


            //Open the outjob docs
            foreach (string strPath in lstOutjobDocPaths)
                lstServerDocs.Add(tmpClient.OpenDocument("OUTPUTJOB", strPath));

            List<TreeNode> lstTreeNode;
            //Get outjob mediums
            foreach (IServerDocument ServerDoc in lstServerDocs)
            {
                OutJobDoc = (IWSM_OutputJobDocument)ServerDoc;

                lstTreeNode = new List<TreeNode>();
                for (int i = 0; i < OutJobDoc.GetState_OutputMediumCount(); i++)
                {
                    if (OutJobDoc.GetState_OutputMedium(i).GetState_TypeString() == "Generate Files")
                    {
                        OutputMedium = OutJobDoc.GetState_OutputMedium(i);
                        if (OutputMedium.GetState_Name() == "ModBOM")
                        {
                            tmpClient.ShowDocument(ServerDoc);

                            OutJobDoc = (IWSM_OutputJobDocument)ServerDoc;
                            OutJobDoc.SetState_VariantScope(TOutputJobVariantScope.eVariantScope_DefinedForWholeOutputJob);
                            OutJobDoc.SetState_VariantName("Var_PE");

                            //Generate outjob outputs.

                            process = "WorkspaceManager:GenerateReport";
                            parameters = "Action=Run|ObjectKind=OutputBatch" +
                                            "|OutputMedium=" + OutputMedium.GetState_Name() +
                                            "|PromptOverwrite=FALSE|OpenOutput=FALSE";
                            DXP.Utils.RunCommand(process, parameters);

                            foreach (IServerDocument tmpServerDoc in lstServerDocs)
                                tmpClient.CloseDocument(tmpServerDoc);

                            MessageBox.Show("Process Complete");

                            return;
                        }

                    }
                }
            }
            MessageBox.Show("There is no \"ModBOM\" outjob in this project. You must create one to proceed.\nThe outjob must contain parameters \"Var_Eng\", \"Var_Flt\" and \"libref\". The container must be named \"ModBOM\"");
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

}


/// <summary>
/// Will store a list of components and their library reference.
/// </summary>
/// <typeparam name="LibRef">Library reference ID</typeparam>
/// <typeparam name="RefDes">Reference designator</typeparam>
public class ComponentList<RefDes, LibRef> : Dictionary<string, string>
{
    /// <summary>
    /// Gets or sets the associated library reference of the given reference designator.
    /// </summary>
    /// <param name="RefDes">Reference Designator</param>
    /// <returns>Library reference</returns>
    public new string this[string RefDes]
    {
        get
        {
            return base[RefDes];
        }

        set
        {
            base[RefDes] = value;
        }
    }
    /// <summary>
    /// Add new entry.
    /// </summary>
    /// <param name="RefDes">Reference Designator</param>
    /// <param name="LibRef">Library Reference</param>
    public new void Add(string RefDes, string LibRef)
    {
        base.Add(RefDes, LibRef);
    }
}