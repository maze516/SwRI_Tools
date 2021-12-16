using DXP;
using EDP;
using NLog;
using SCH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

public class Variants
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    private string Overwrite;
    public ProgressBar Progress;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from">Variant to pull data from</param>
    /// <param name="to">Variant to update</param>
    /// <param name="_overwrite">Type of Overwrite</param>
    /// <param name="Force">Will force Alternate variants to fitted and apply data.</param>
    public void SyncVariants(string from, string[] Destinations, string _overwrite, bool Force)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        Overwrite = _overwrite;


        foreach (string to in Destinations)
        {
            Var_Type VariantList = new Var_Type();
            //Get variant info based on From variable.
            if (from == "Base")
            {
                VariantList.VarName = from;
                GetBaseVariants(ref VariantList);
            }
            else
            {
                VariantList.VarName = from.ToUpper();
                Get_Variants(ref VariantList);
            }

            //Make sure variant data was loaded.
            if (VariantList == null) return;
            if (VariantList.Components.Count == 0)
            {
                MessageBox.Show("No component variations found.");
                return;
            }
            //Set variant info based on To variable.
            if (to == "Base")
                SetBaseDesign(VariantList);
            else
                SetVariant(VariantList, to.ToUpper(), Force);
        }
        MessageBox.Show("Process Complete.");

    }

    /// <summary>
    /// Add graphical test to the progressbar.
    /// </summary>
    /// <param name="Text">Text to be displayed in the progress bar.</param>
    public void UpdateLabel(string Text)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (Progress.Maximum > 0)
        {
            float strWidth;
            string myString = ((Progress.Value * 100) / Progress.Maximum).ToString();
            myString = Text + " " + myString + "% Done";
            Graphics canvas = Progress.CreateGraphics();
            strWidth = canvas.MeasureString(myString, new Font("Verdana", 8, FontStyle.Bold)).Width;
            canvas.DrawString(myString, new Font("Verdana", 8, FontStyle.Bold), new SolidBrush(Color.Black), (canvas.VisibleClipBounds.Width - strWidth) / 2, 4);
            canvas.Dispose();
        }
    }

    /// <summary>
    /// Get the number of Alternates in the provided variant.
    /// </summary>
    /// <param name="VariantName">Name of variant to check.</param>
    /// <returns>Quantity of Alternates in provided variant.</returns>
    public int Get_VariantAlternates(string VariantName)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;

            VarParam<string, string> Parameters = new VarParam<string, string>();
            int cnt = 0;
            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
            {
                Variant = project.DM_ProjectVariants(i);
                //Find variant with the same name as the one provided.
                if (project.DM_ProjectVariants(i).DM_Description() == VariantName)
                {
                    for (int j = 0; j < Variant.DM_VariationCount(); j++)
                    {
                        CompVariant = Variant.DM_Variations(j);
                        //Count the number of alternates.
                        if (CompVariant.DM_VariationKind() == TVariationKind.eVariation_Alternate)
                        {
                            cnt++;
                        }
                    }
                }
            }
            return cnt;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return -1;
        }
    }

    /// <summary>
    /// Get parameter data from an existing variant.
    /// </summary>
    /// <param name="VarList">Reference to the class that will store the gathered parameter data.</param>
    public void Get_Variants(ref Var_Type VarList)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;
            IParameterVariation ParamVariant;

            VarParam<string, string> Parameters = new VarParam<string, string>();
            string RefDes;
            int l = 0;

            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
            {
                l++;
                Variant = project.DM_ProjectVariants(i);

                //Find the variant that matches the one provided.
                if (project.DM_ProjectVariants(i).DM_Description().ToUpper() == VarList.VarName)
                {
                    Progress.Maximum = Variant.DM_VariationCount();
                    Progress.Value = 0;
                    UpdateLabel("Loading Variants");
                    for (int j = 0; j < Variant.DM_VariationCount(); j++)
                    {

                        CompVariant = Variant.DM_Variations(j);
                        RefDes = CompVariant.DM_PhysicalDesignator();

                        //checking to make sure all components have a refdes assigned.
                        if (RefDes.Contains("?"))
                        {
                            MessageBox.Show("Detected an un-annotated refdes. Please Annotate the project and try again.");
                            VarList = null;
                            return;
                        }
                        Parameters = new VarParam<string, string>();
                        //Iterate through all parameters for current component variant.
                        for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                        {
                            if (CompVariant.DM_VariationKind() != TVariationKind.eVariation_NotFitted)
                            {
                                ParamVariant = CompVariant.DM_Variations(k);
                                //Get values of matching parameters.
                                if ("PE_ENG" == ParamVariant.DM_ParameterName().ToUpper() || ParamVariant.DM_ParameterName().ToUpper() == "PE_FLT")
                                {
                                    string tmpVarValue = ParamVariant.DM_VariedValue() == null ? "x" : ParamVariant.DM_VariedValue();
                                    Parameters.Add(ParamVariant.DM_ParameterName().ToUpper(), tmpVarValue);
                                    if (!tmpVarValue.EndsWith("_$"))
                                        ParamVariant.DM_SetVariedValue(tmpVarValue + "_$");
                                }
                            }
                        }

                        l++;
                        //Save collected data to VarList.
                        if (Parameters.Count > 0)
                            VarList.Components.Add(RefDes, Parameters);
                        Progress.Value += 1;
                        UpdateLabel("Loading Variants");
                    }
                }
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
    /// Get parameter data from the base design.
    /// </summary>
    /// <param name="VarList">Reference to the class that will store the gathered parameter data.</param>
    public void GetBaseVariants(ref Var_Type VarList)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {

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
            VarParam<string, string> Parameters = new VarParam<string, string>();
            string RefDes;

            bool DocOpened = false;
            Progress.Value = 0;
            Progress.Maximum = LogicalDocumentCount;
            UpdateLabel("Loading Variants");
            //iterate through project documents.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                //Check for schematic documents.
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    DocOpened = false;
                    //Open documents
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

                    //Client.ShowDocument(ServerDoc);

                    ISch_Lib SchDoc;
                    SchDoc = SchServer.LoadSchDocumentByPath(CurrentSheet.DM_FullPath()) as ISch_Lib;

                    ISch_Iterator LibraryIterator, PIterator;
                    ISch_Component Component;
                    ISch_Parameter Param;
                    if (SchDoc == null)
                        return;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SchDoc.SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;

                    while (Component != null)
                    {
                        if (Component.GetState_SchDesignator().GetState_Text().Contains("?"))
                        {
                            MessageBox.Show("Detected and un-annotated refdes. Please Annotate the project and try again.");
                            VarList = null;
                            return;
                        }
                        RefDes = Component.GetState_SchDesignator().GetState_Text();
                        Parameters = new VarParam<string, string>();

                        //Iterate theough all parameters in the component.
                        PIterator = Component.SchIterator_Create();
                        PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

                        Param = PIterator.FirstSchObject() as ISch_Parameter;
                        while (Param != null)
                        {
                            if (Param.GetState_Name() != null)
                            {
                                //Store specific parameter data.
                                if ("PE_ENG" == Param.GetState_Name().ToUpper() || Param.GetState_Name().ToUpper() == "PE_FLT")
                                    if (Param.GetState_Text() != "x")
                                        Parameters.Add(Param.GetState_Name().ToUpper(), Param.GetState_CalculatedValueString());
                            }
                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }
                        //Add stored parameter data to VarList.
                        if (Parameters.Count > 0)
                            if (!VarList.Components.ContainsKey(RefDes))
                                VarList.Components.Add(RefDes, Parameters);
                        Component = LibraryIterator.NextSchObject() as ISch_Component;
                    }

                    //if (ServerDoc.GetModified())
                    //    ServerDoc.DoFileSave("");

                    //Close opend documents.
                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                }
                Progress.Value += 1;
                UpdateLabel("Loading Variants");
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

    /// <summary>
    /// Updates base design parameters based on provided data.
    /// </summary>
    /// <param name="VarList">Parameter data</param>
    void SetBaseDesign(Var_Type VarList)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            string RefDes;
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

            Progress.Maximum = LogicalDocumentCount;
            Progress.Value = 0;
            UpdateLabel("Updating Variants");
            bool DocOpened = false;
            //Iterate through all documents looking for scheatic docs.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    //Open document if not already open.
                    DocOpened = false;
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

                    //Client.ShowDocument(ServerDoc);

                    ISch_Lib SchDoc;
                    SchDoc = SchServer.LoadSchDocumentByPath(CurrentSheet.DM_FullPath()) as ISch_Lib;

                    ISch_Iterator LibraryIterator, PIterator;
                    ISch_Component Component;
                    ISch_Parameter Param;
                    VarParam<string, string> CompVars = new VarParam<string, string>();
                    if (SchDoc == null)
                        return;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SchDoc.SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;
                    while (Component != null)
                    {
                        RefDes = Component.GetState_SchDesignator().GetState_Text();
                        if (VarList.Components.ContainsKey(Component.GetState_SchDesignator().GetState_Text()))
                        {
                            if (VarList.Components[Component.GetState_SchDesignator().GetState_Text()].Saved == false)
                            {
                                Component.UpdatePart_PreProcess();
                                CompVars = VarList.Components[Component.GetState_SchDesignator().GetState_Text()];
                                //Iterate theough all parameters in the component.
                                PIterator = Component.SchIterator_Create();
                                PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

                                Param = PIterator.FirstSchObject() as ISch_Parameter;
                                while (Param != null)
                                {
                                    if (Param.GetState_Name() != null)
                                        if (Param.GetState_Name().ToUpper() != null)
                                            //Set parameter data in component if it is in the provided parameter data.
                                            //Param = null;
                                            if (CompVars.ContainsKey(Param.GetState_Name().ToUpper()))
                                            {

                                                if (Param.GetState_Text() == "x")
                                                    Param.SetState_Text(CompVars[Param.GetState_Name().ToUpper()]);
                                                else if (OverwriteValue(Param.GetState_Text().ToUpper(), CompVars[Param.GetState_Name().ToUpper()], Component.GetState_SchDesignator().GetState_Text(), Param.GetState_Name().ToUpper()))
                                                    Param.SetState_Text(CompVars[Param.GetState_Name().ToUpper()]);
                                            }
                                    Param = PIterator.NextSchObject() as ISch_Parameter;
                                }
                                Component.UpdatePart_PostProcess();
                                VarList.Components[Component.GetState_SchDesignator().GetState_Text()].Saved = true;
                            }
                        }
                        Component = LibraryIterator.NextSchObject() as ISch_Component;
                    }

                    if (ServerDoc.GetModified())
                        ServerDoc.DoFileSave("");

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                }
                Progress.Value += 1;
                UpdateLabel("Updating Variants");
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
    /// <summary>
    /// Set parameter data in the provided variant.
    /// </summary>
    /// <param name="VarList">Parameter data.</param>
    /// <param name="VarName">Name of variant to modify.</param>
    /// <param name="Force">Force alternate variants to fitted.</param>
    void SetVariant(Var_Type VarList, string VarName, bool Force)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;
            IParameterVariation ParamVariant;
            if (project == null) return;
            project.DM_BeginUpdate();

            string RefDes;

            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
            {
                Variant = project.DM_ProjectVariants(i);
                //Find variant that matches the one provided.
                if (project.DM_ProjectVariants(i).DM_Description().ToUpper() == VarName)
                {
                    Progress.Maximum = (Variant.DM_VariationCount() + VarList.Components.Count);
                    Progress.Value = 0;
                    UpdateLabel("Updating " + VarName + " Variants");
                    for (int j = 0; j < Variant.DM_VariationCount(); j++)
                    {
                        //Remove alternate variants  if Force is true.
                        if (Variant.DM_Variations(j).DM_VariationKind() == TVariationKind.eVariation_Alternate && Force)
                        {
                            Variant.DM_RemoveComponentVariation(j);
                            j--;
                        }
                        else
                        {
                            CompVariant = Variant.DM_Variations(j);
                            RefDes = CompVariant.DM_PhysicalDesignator();
                            //Make sure there is parameter data for this component variant.
                            if (VarList.Components.ContainsKey(CompVariant.DM_PhysicalDesignator()))
                            {

                                for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                                {
                                    if (CompVariant.DM_VariationKind() != TVariationKind.eVariation_NotFitted)
                                    {
                                        ParamVariant = CompVariant.DM_Variations(k);
                                        //Update parameter data.
                                        if ("PE_ENG" == ParamVariant.DM_ParameterName().ToUpper() || ParamVariant.DM_ParameterName().ToUpper() == "PE_FLT")
                                            if (VarList.Components[CompVariant.DM_PhysicalDesignator()].ContainsKey(ParamVariant.DM_ParameterName().ToUpper()))
                                                if (VarList.Components[CompVariant.DM_PhysicalDesignator()][ParamVariant.DM_ParameterName().ToUpper()] != CompVariant.DM_PhysicalDesignator())
                                                    if (OverwriteValue(ParamVariant.DM_VariedValue(), VarList.Components[CompVariant.DM_PhysicalDesignator()][ParamVariant.DM_ParameterName().ToUpper()], CompVariant.DM_PhysicalDesignator(), ParamVariant.DM_ParameterName().ToUpper()))
                                                    {
                                                        ParamVariant.DM_SetVariedValue(VarList.Components[CompVariant.DM_PhysicalDesignator()][ParamVariant.DM_ParameterName().ToUpper()] + "_$");
                                                        VarList.Components[CompVariant.DM_PhysicalDesignator()].Saved = true;
                                                    }
                                        //else
                                        //{
                                        //    ParamVariant.DM_SetVariedValue(VarList.Components[CompVariant.DM_PhysicalDesignator()][ParamVariant.DM_ParameterName().ToUpper()] + "_$");
                                        //    VarList.Components[CompVariant.DM_PhysicalDesignator()].Saved = true;
                                        //}

                                    }

                                }
                                if (!VarList.Components[CompVariant.DM_PhysicalDesignator()].Saved)
                                {
                                    CreateCompVar(ref CompVariant, VarList.Components[CompVariant.DM_PhysicalDesignator()]);
                                    VarList.Components[CompVariant.DM_PhysicalDesignator()].Saved = true;
                                }

                            }
                        }
                        Progress.Value += 1;
                    }
                    IComponentVariation tmpCompVar;
                    foreach (string CompRef in VarList.Components.Keys)
                    {
                        //Add component variants that havent already been modified.
                        if (!VarList.Components[CompRef].Saved)
                        {
                            tmpCompVar = null;
                            tmpCompVar = project.DM_ProjectVariants(i).DM_AddComponentVariation();
                            tmpCompVar.DM_SetVariationKind(TVariationKind.eVariation_None);
                            CreateCompVar(ref tmpCompVar, VarList.Components[CompRef], CompRef);

                            VarList.Components[CompRef].Saved = true;
                        }
                        Progress.Value += 1;
                        UpdateLabel("Updating " + VarName + " Variants");
                    }
                }
            }
            project.DM_EndUpdate();
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

    //create alternate part. Need to make functional.
    public void Command_CreateComponentVariantion(IServerDocumentView view, ref string parameters)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IWorkspace workspace = DXP.GlobalVars.DXPWorkSpace as IWorkspace;
        IProject project = workspace.DM_FocusedProject();

        if (project.DM_NeedsCompile())
            project.DM_Compile();

        // Get or create "Test" Project Variant
        IProjectVariant projectVariant = null;
        for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
        {
            if (project.DM_ProjectVariants(i).DM_Description().Equals("Test", StringComparison.OrdinalIgnoreCase))
            {
                projectVariant = project.DM_ProjectVariants(i);
                break;
            }
        }

        if (projectVariant == null)
        {
            projectVariant = project.DM_AddProjectVariant();
            projectVariant.DM_SetName("Test");
            projectVariant.DM_SetDescription("Test");
        }

        // Create alternate part for each component in the project, using "Bridge2" in "Miscellaneous Devices.IntLib".
        project.DM_BeginUpdate();
        try
        {
            IDocument documentFlattened = project.DM_DocumentFlattened();
            for (int i = 0; i < documentFlattened.DM_ComponentCount(); i++)
            {
                IComponent component = documentFlattened.DM_Components(i);
                if (component.DM_IsInferredObject())
                    continue;

                IComponentVariation componentVariation = projectVariant.DM_FindComponentVariationByUniqueId(component.DM_UniqueId());
                if (componentVariation == null)
                {
                    componentVariation = projectVariant.DM_AddComponentVariation();
                }

                componentVariation.DM_SetUniqueId(component.DM_UniqueId());
                IComponentLibraryLink componentLibraryLink = componentVariation.DM_AlternateLibraryLink();
                componentLibraryLink.DM_SetUseLibraryName(true);
                componentLibraryLink.DM_SetLibraryIdentifier("Miscellaneous Devices.IntLib");
                componentLibraryLink.DM_SetSourceLibraryName("Miscellaneous Devices.IntLib");
                componentLibraryLink.DM_SetDesignItemID("Bridge2");
                componentVariation.DM_SetVariationKind(TVariationKind.eVariation_Alternate);
            }
        }
        finally
        {
            project.DM_EndUpdate();
        }
    }

    /// <summary>
    /// Create a new component variant.
    /// </summary>
    /// <param name="Dest">Component to create variant for.</param>
    /// <param name="Source">Parameter data.</param>
    /// <param name="RefDes">Refdes of component being modified.</param>
    void CreateCompVar(ref IComponentVariation Dest, VarParam<string, string> Source, string RefDes = "")
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IParameterVariation tmpParam;
        if (RefDes != "")
            Dest.DM_SetPhysicalDesignator(RefDes); //Set refdes
        foreach (string key in Source.Keys) //Set each parameter
        {
            tmpParam = Dest.DM_AddParameterVariation();
            tmpParam.DM_SetParameterName(key);
            tmpParam.DM_SetVariedValue(Source[key] + "_$");

        }
    }

    /// <summary>
    /// Determine if value should be overwritten.
    /// </summary>
    /// <param name="Original">Original value.</param>
    /// <param name="NewValue">New value</param>
    /// <param name="RefDes">Component refdes.</param>
    /// <param name="Parameter">Parameter name.</param>
    /// <returns></returns>
    bool OverwriteValue(string Original, string NewValue, string RefDes, string Parameter)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        DialogResult Result;

        if (Overwrite == "Overwrite")
            return true;
        else if (Overwrite == "Skip")
            return false;
        else //Overwrite = "Prompt"
        {
            Result = MessageBox.Show(RefDes + " already has info for " + Parameter + "\n" + "Do you wish to overwrite " + Original + " with " + NewValue + "?", "Overwrite Existing Info", MessageBoxButtons.YesNo);
            if (Result == DialogResult.Yes)
                return true;
            else
                return false;
        }
    }
}

/// <summary>
/// Contains all variant info.
/// </summary>
public class Var_Type
{
    /// <summary>
    /// Name of variant.
    /// </summary>
    public string VarName = "";

    /// <summary>
    /// List of components with variant changes.
    /// </summary>
    public VarComp<string, VarParam<string, string>> Components = new VarComp<string, VarParam<string, string>>();
}

/// <summary>
/// Dictionary of component variants.
/// </summary>
/// <typeparam name="TKey">Component LibRef</typeparam>
/// <typeparam name="TValue">Component parameter variants.</typeparam>
public class VarComp<TKey, TValue> : IDictionary<string, VarParam<string, string>>
{
    private Dictionary<string, VarParam<string, string>> backingDictionary = new Dictionary<string, VarParam<string, string>>();

    #region Auto-Implemented
    public VarParam<string, string> this[string key]
    {
        get
        {
            return ((IDictionary<string, VarParam<string, string>>)backingDictionary)[key];
        }

        set
        {
            ((IDictionary<string, VarParam<string, string>>)backingDictionary)[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return ((IDictionary<string, VarParam<string, string>>)backingDictionary).IsReadOnly;
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Keys;
        }
    }

    public ICollection<VarParam<string, string>> Values
    {
        get
        {
            return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Values;
        }
    }

    public void Add(KeyValuePair<string, VarParam<string, string>> item)
    {
        ((IDictionary<string, VarParam<string, string>>)backingDictionary).Add(item);
    }

    public void Add(string key, VarParam<string, string> value)
    {
        ((IDictionary<string, VarParam<string, string>>)backingDictionary).Add(key, value);
    }

    public void Clear()
    {
        ((IDictionary<string, VarParam<string, string>>)backingDictionary).Clear();
    }

    public bool Contains(KeyValuePair<string, VarParam<string, string>> item)
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, VarParam<string, string>>[] array, int arrayIndex)
    {
        ((IDictionary<string, VarParam<string, string>>)backingDictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, VarParam<string, string>>> GetEnumerator()
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).GetEnumerator();
    }

    public bool Remove(KeyValuePair<string, VarParam<string, string>> item)
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Remove(item);
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).Remove(key);
    }

    public bool TryGetValue(string key, out VarParam<string, string> value)
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, VarParam<string, string>>)backingDictionary).GetEnumerator();
    }
    #endregion


}

/// <summary>
/// A dictionary of parameters.
/// </summary>
/// <typeparam name="TKey">Parameter name</typeparam>
/// <typeparam name="TValue">Parameter value</typeparam>
public class VarParam<TKey, TValue> : IDictionary<string, string>
{
    private bool _Saved = false;
    private Dictionary<string, string> backingDictionary = new Dictionary<string, string>();
    public bool Saved { get { return _Saved; } set { _Saved = value; } }
    public string this[string key]
    {
        get
        {
            string val = ((IDictionary<string, string>)backingDictionary)[key];
            if (val.EndsWith("_$"))
                return val.Substring(0, val.Length - 2);
            else
                return ((IDictionary<string, string>)backingDictionary)[key];
        }

        set
        {
            ((IDictionary<string, string>)backingDictionary)[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return ((IDictionary<string, string>)backingDictionary).Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return ((IDictionary<string, string>)backingDictionary).IsReadOnly;
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return ((IDictionary<string, string>)backingDictionary).Keys;
        }
    }

    public ICollection<string> Values
    {
        get
        {
            return ((IDictionary<string, string>)backingDictionary).Values;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="KVPair">A key value pair of (Parameter name, Parameter value)</param>
    public void Add(KeyValuePair<string, string> item)
    {
        ((IDictionary<string, string>)backingDictionary).Add(item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">Parameter Name</param>
    /// <param name="value">Parameter Value</param>
    public void Add(string key, string value)
    {
        ((IDictionary<string, string>)backingDictionary).Add(key, value);
    }

    public void Clear()
    {
        ((IDictionary<string, string>)backingDictionary).Clear();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
        return ((IDictionary<string, string>)backingDictionary).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, string>)backingDictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
        ((IDictionary<string, string>)backingDictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
        return ((IDictionary<string, string>)backingDictionary).GetEnumerator();
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
        return ((IDictionary<string, string>)backingDictionary).Remove(item);
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, string>)backingDictionary).Remove(key);
    }

    public bool TryGetValue(string key, out string value)
    {
        return ((IDictionary<string, string>)backingDictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, string>)backingDictionary).GetEnumerator();
    }
}
