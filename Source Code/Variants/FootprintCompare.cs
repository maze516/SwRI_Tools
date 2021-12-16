using DXP;
using EDP;
using NLog;
using SCH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class FootprintCompare
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    List<Var_Type> AllVariants = new List<Var_Type>();

    public void CompareFootprints()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //purge existing classname data


        //Here’s what we’d like to use:
        //Flight parts ClassName = “Stencil_Flt”
        //Engineering parts ClassName = “Stencil_Eng”
        //all parts ClassName = “Stencil_Base”


        GetBaseVariants();
        MessageBox.Show("Process complete.");
    }


    /// <summary>
    /// Get parameter data from the base design.
    /// </summary>
    /// <param name="VarList">Reference to the class that will store the gathered parameter data.</param>
    public void GetBaseVariants()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {

            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            int LogicalDocumentCount;
            int LoopIterator;
            Dictionary<string, IComponentVariation> FltCompVar, EngCompVar;
            IDXPDocument CurrentSheet;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();
            ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
            IClient Client = DXP.GlobalVars.Client;
            IServerDocument ServerDoc;
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.
            VarParam<string, string> Parameters = new VarParam<string, string>();
            string RefDes;

            IParameterVariation TempVar;
            int Matches;
            bool DocOpened = false;

            FltCompVar = Get_Variants("VAR_FLT");
            EngCompVar = Get_Variants("VAR_ENG");

            if (FltCompVar == null || EngCompVar == null)
                return;

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

                    ISch_Iterator SchIterator, PIterator;
                    ISch_Component Component;
                    ISch_Implementation Footprint;
                    ISch_Parameter Param;
                    if (SchDoc == null)
                        return;
                    //Iterate theough all components on the schematic.
                    SchIterator = SchDoc.SchIterator_Create();
                    SchIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = SchIterator.FirstSchObject() as ISch_Component;



                    while (Component != null)
                    {
                        Matches = 0;
                        if (Component.GetState_SchDesignator().GetState_Text().Contains("?"))
                        {
                            MessageBox.Show("Detected and un-annotated refdes. Please Annotate the project and try again.");
                            return;
                        }
                        RefDes = Component.GetState_SchDesignator().GetState_Text();



                        //Iterate theough all parameters in the component.
                        PIterator = Component.SchIterator_Create();
                        PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eImplementation));

                        Footprint = PIterator.FirstSchObject() as ISch_Implementation;
                        if (FltCompVar.ContainsKey(RefDes))
                            if (FltCompVar[RefDes].DM_AlternateLibraryLink().DM_Footprint() != Footprint.GetState_ModelName())
                            {
                                TempVar = FltCompVar[RefDes].DM_FindParameterVariation("ClassName");
                                if (TempVar != null)
                                    TempVar.DM_SetVariedValue("Stencil_Flt");
                                Matches++;
                            }

                        if (EngCompVar.ContainsKey(RefDes))
                            if (EngCompVar[RefDes].DM_AlternateLibraryLink().DM_Footprint() != Footprint.GetState_ModelName())
                            {
                                TempVar = EngCompVar[RefDes].DM_FindParameterVariation("ClassName");
                                if (TempVar != null)
                                    TempVar.DM_SetVariedValue("Stencil_Eng");
                                Matches++;
                            }

                        //?Param.GetState_ModelName()
                        //"FIDUCIAL_SMD"

                        //Iterate theough all parameters in the component.
                        PIterator = Component.SchIterator_Create();
                        PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

                        Param = PIterator.FirstSchObject() as ISch_Parameter;
                        while (Param != null)
                        {
                            if (Param.GetState_Name() == "ClassName")
                            {
                                if (Matches == 2)
                                    Param.SetState_Text("Stencil_Base");
                                else
                                    Param.SetState_Text("");

                                Component.UpdatePart_PostProcess();
                                break;
                            }
                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }

                        Component = SchIterator.NextSchObject() as ISch_Component;
                    }

                    //if (ServerDoc.GetModified())
                    //    ServerDoc.DoFileSave("");

                    //Close opend documents.
                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);

                    ServerDoc = null;

                }
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
    /// 
    /// </summary>
    /// <param name="VariantName"></param>
    /// <returns></returns>
    public Dictionary<string, IComponentVariation> Get_Variants(string VariantName)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            Dictionary<string, IComponentVariation> Output = new Dictionary<string, IComponentVariation>();
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;
            IParameterVariation ParamVariant;
            //IParameterVariation ParamVariant;

            //VarParam<string, string> Parameters = new VarParam<string, string>();
            string RefDes;
            int l = 0;

            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
            {
                l++;
                Variant = project.DM_ProjectVariants(i);

                //Find the variant that matches the one provided.
                if (project.DM_ProjectVariants(i).DM_Description().ToUpper() == VariantName)
                {
                    for (int j = 0; j < Variant.DM_VariationCount(); j++)
                    {

                        CompVariant = Variant.DM_Variations(j);
                        RefDes = CompVariant.DM_PhysicalDesignator().ToUpper();

                        //checking to make sure all components have a refdes assigned.
                        if (RefDes.Contains("?"))
                        {
                            MessageBox.Show("Detected an un-annotated refdes. Please Annotate the project and try again.");
                            return null;
                        }
                        ParamVariant = CompVariant.DM_FindParameterVariation("ClassName");
                        if (ParamVariant != null)
                            if (ParamVariant.DM_VariedValue() != null)
                                ParamVariant.DM_SetVariedValue("");

                        if (CompVariant.DM_VariationKind() == TVariationKind.eVariation_Alternate)
                            Output.Add(RefDes, CompVariant);

                        // Parameters = new VarParam<string, string>();
                        ////Iterate through all parameters for current component variant.
                        //for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                        //{

                        //        //ParamVariant = CompVariant.DM_Variations(k);
                        //        //Get values of matching parameters.
                        //        //if ("PE_ENG" == ParamVariant.DM_ParameterName().ToUpper() || ParamVariant.DM_ParameterName().ToUpper() == "PE_FLT")
                        //        //{
                        //        //    string tmpVarValue = ParamVariant.DM_VariedValue() == null ? "x" : ParamVariant.DM_VariedValue();
                        //        //    Parameters.Add(ParamVariant.DM_ParameterName().ToUpper(), tmpVarValue);
                        //        //    if (!tmpVarValue.EndsWith("_$"))
                        //        //        ParamVariant.DM_SetVariedValue(tmpVarValue + "_$");
                        //        //}
                        //    }


                        //l++;


                    }
                }
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


}

