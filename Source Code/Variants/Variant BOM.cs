using DXP;
using EDP;
using SCH;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using NLog;
//PE pn, Comp pn, Refdes, Libref, package, UID(?), variant

class Variant_BOM
{
    public static readonly Logger _Log = LogManager.GetCurrentClassLogger();
    PNCompList PartnumberCompList;
    RefDesCompList RefCompList;
    public void Create_Variant_BOM()
    {
        try
        {
            PartnumberCompList = new PNCompList();
            RefCompList = new RefDesCompList();
            //Load base design data
            if (!GetBaseVariants())
                return;
            //Load variant data (Var_flt, Var_eng)
            if (!Get_Variants("VAR_ENG"))
                return;
            if (!Get_Variants("VAR_FLT"))
                return;
            if (!Get_Variants("VAR_PE"))
                return;

            OutputBOM();
            MessageBox.Show("Process Complete");
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

    void OutputBOM()
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            string BasePath = CurrentProject.DM_GetOutputPath();

            if (!Directory.Exists(BasePath))
                Directory.CreateDirectory(BasePath);

            StreamWriter swOutput = new StreamWriter(BasePath + "\\Partnumber Variant Report " + DateTime.Today.ToString("MM-dd-yyyy") + ".csv");
            swOutput.WriteLine("PE Flt Partnumber, PE Eng Partnumber, Base Partnumber, Base LibRef, RefDes, UniqueID, Var Flt Partnumber, Var Flt LibRef, Var Eng Partnumber, Var Eng LibRef");
            foreach (CompData item in PartnumberCompList.Values)
            {
                swOutput.WriteLine(item.ToString());
            }
            swOutput.Close();

            System.Diagnostics.Process.Start(BasePath + "\\Partnumber Variant Report " + DateTime.Today.ToString("MM-dd-yyyy") + ".csv");

            swOutput = new StreamWriter(BasePath + "\\Refdes Variant Report.csv");
            swOutput.WriteLine("PE Flt Partnumber, PE Eng Partnumber, Base Partnumber, Base LibRef, RefDes, UniqueID, Var Flt Partnumber, Var Flt LibRef, Var Eng Partnumber, Var Eng LibRef");
            foreach (CompData item in RefCompList.Values)
            {
                swOutput.WriteLine(item.ToString());
            }
            swOutput.Close();
            System.Diagnostics.Process.Start(BasePath + "\\Refdes Variant Report.csv");
            swOutput.Dispose();
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
    /// Get parameter data from an existing variant.
    /// </summary>
    /// <param name="VarList">Reference to the class that will store the gathered parameter data.</param>
    bool Get_Variants(string VariantName)
    {
        try
        {
            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;
            IProjectVariant Variant;
            IComponentVariation CompVariant;
            IParameterVariation ParamVariant;
            CompData NewComp;
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
                        NewComp = new CompData("");
                        CompVariant = Variant.DM_Variations(j);
                        RefDes = CompVariant.DM_PhysicalDesignator();
                        NewComp.RefDes = RefDes;
                        NewComp.Base_Partnumber = RefCompList[RefDes].Base_Partnumber;
                        if (CompVariant.DM_VariationKind() == TVariationKind.eVariation_Alternate)
                            if (VariantName.ToUpper() == "VAR_ENG")
                                NewComp.Var_Eng_LibRef = CompVariant.DM_AlternateLibraryLink().DM_DesignItemID();
                            else if (VariantName.ToUpper() == "VAR_FLT")
                                NewComp.Var_Flt_LibRef = CompVariant.DM_AlternateLibraryLink().DM_DesignItemID();
                        //checking to make sure all components have a refdes assigned.
                        if (RefDes.Contains("?"))
                        {
                            MessageBox.Show("Detected and un-annotated refdes. Please Annotate the project and try again.");
                            return false;
                        }
                        //Iterate through all parameters for current component variant.
                        for (int k = 0; k < CompVariant.DM_VariationCount(); k++)
                        {
                            if (CompVariant.DM_VariationKind() != TVariationKind.eVariation_NotFitted)
                            {
                                ParamVariant = CompVariant.DM_Variations(k);
                                if (VariantName.ToUpper() != "VAR_PE")
                                {
                                    //Get values of matching parameters.
                                    if ("PARTNUMBER" == ParamVariant.DM_ParameterName().ToUpper())
                                    {
                                        if (VariantName.ToUpper() == "VAR_ENG")
                                            NewComp.Var_Eng_Partnumber = ParamVariant.DM_VariedValue();
                                        else if (VariantName.ToUpper() == "VAR_FLT")
                                            NewComp.Var_Flt_Partnumber = ParamVariant.DM_VariedValue();
                                    }
                                }
                                else
                                {
                                    if ("PE_FLT" == ParamVariant.DM_ParameterName().ToUpper())
                                        NewComp.PE_FLT_Partnumber = (ParamVariant.DM_VariedValue() == null) ? ParamVariant.DM_VariedValue() : "";
                                    else if ("PE_ENG" == ParamVariant.DM_ParameterName().ToUpper())
                                        NewComp.PE_ENG_Partnumber = (ParamVariant.DM_VariedValue() == null) ? ParamVariant.DM_VariedValue() : "";
                                }
                            }
                        }
                        AddPart(NewComp);
                        l++;
                    }
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return false;
        }
    }

    /// <summary>
    /// Get parameter data from the base design.
    /// </summary>
    /// <param name="VarList">Reference to the class that will store the gathered parameter data.</param>
    bool GetBaseVariants()
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
            ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
            IClient Client = DXP.GlobalVars.Client;
            IServerDocument ServerDoc;
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.
            string RefDes;
            CompData NewComp;

            bool DocOpened = false;
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
                        return false;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SchDoc.SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;

                    while (Component != null)
                    {
                        NewComp = new CompData("");
                        if (Component.GetState_SchDesignator().GetState_Text().Contains("?"))
                        {
                            MessageBox.Show("Detected and un-annotated refdes. Please Annotate the project and try again.");
                            return false;
                        }
                        RefDes = Component.GetState_SchDesignator().GetState_Text();
                        NewComp.RefDes = RefDes;
                        NewComp.Base_LibRef = Component.GetState_DesignItemId();
                        NewComp.UniqueID = Component.GetState_UniqueId();
                        //Iterate theough all parameters in the component.
                        PIterator = Component.SchIterator_Create();
                        PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

                        Param = PIterator.FirstSchObject() as ISch_Parameter;
                        while (Param != null)
                        {
                            if (Param.GetState_Name() != null)
                                //Store specific parameter data.
                                if ("PARTNUMBER" == Param.GetState_Name().ToUpper())
                                    NewComp.Base_Partnumber = Param.GetState_CalculatedValueString();

                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }


                        AddPart(NewComp);

                        Component = LibraryIterator.NextSchObject() as ISch_Component;
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
            return true;

        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return false;
        }
    }


    void AddPart(CompData Component)
    {
        RefCompList.Add(Component.RefDes, Component);
        PartnumberCompList.Add(Component.Base_Partnumber, Component);
        ///add to each dictionary

    }

}
class PNCompList : IDictionary<string, CompData>
{
    public static readonly Logger _Log = LogManager.GetCurrentClassLogger();
    private Dictionary<string, CompData> PNDictionary = new Dictionary<string, CompData>();

    public CompData this[string key]
    {
        get
        {
            return ((IDictionary<string, CompData>)PNDictionary)[key];
        }

        set
        {
            ((IDictionary<string, CompData>)PNDictionary)[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return ((IDictionary<string, CompData>)PNDictionary).Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return ((IDictionary<string, CompData>)PNDictionary).IsReadOnly;
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return ((IDictionary<string, CompData>)PNDictionary).Keys;
        }
    }

    public ICollection<CompData> Values
    {
        get
        {
            return ((IDictionary<string, CompData>)PNDictionary).Values;
        }
    }

    public void Add(KeyValuePair<string, CompData> item)
    {
        ((IDictionary<string, CompData>)PNDictionary).Add(item);
    }

    public void Add(string key, CompData value)
    {
        try
        {
            CompData temp;

            if (!PNDictionary.ContainsKey(key))
                ((IDictionary<string, CompData>)PNDictionary).Add(key, value);
            else
            {
                temp = PNDictionary[key];

                if (!temp.Base_LibRef.Contains(value.Base_LibRef))
                    if (temp.Base_LibRef != "")
                        temp.Base_LibRef += "; " + value.Base_LibRef;
                    else
                        temp.Base_LibRef = value.Base_LibRef;

                if (!temp.RefDes.Contains(value.RefDes))
                    if (temp.RefDes != "")
                        temp.RefDes += "; " + value.RefDes;
                    else
                        temp.RefDes = value.RefDes;

                if (!temp.UniqueID.Contains(value.UniqueID))
                    if (temp.UniqueID != "")
                        temp.UniqueID += "; " + value.UniqueID;
                    else
                        temp.UniqueID = value.UniqueID;

                if (!temp.Var_Eng_LibRef.Contains(value.Var_Eng_LibRef))
                    if (temp.Var_Eng_LibRef != "")
                        temp.Var_Eng_LibRef += "; " + value.Var_Eng_LibRef;
                    else
                        temp.Var_Eng_LibRef = value.Var_Eng_LibRef;

                if (!temp.Var_Eng_Partnumber.Contains(value.Var_Eng_Partnumber))
                    if (temp.Var_Eng_Partnumber != "")
                        temp.Var_Eng_Partnumber += "; " + value.Var_Eng_Partnumber;
                    else
                        temp.Var_Eng_Partnumber = value.Var_Eng_Partnumber;

                if (!temp.Var_Flt_LibRef.Contains(value.Var_Flt_LibRef))
                    if (temp.Var_Flt_LibRef != "")
                        temp.Var_Flt_LibRef += "; " + value.Var_Flt_LibRef;
                    else
                        temp.Var_Flt_LibRef += "; " + value.Var_Flt_LibRef;

                if (!temp.Var_Flt_Partnumber.Contains(value.Var_Flt_Partnumber))
                    if (temp.Var_Flt_Partnumber != "")
                        temp.Var_Flt_Partnumber += "; " + value.Var_Flt_Partnumber;
                    else
                        temp.Var_Flt_Partnumber = value.Var_Flt_Partnumber;

                if (!temp.PE_ENG_Partnumber.Contains(value.PE_ENG_Partnumber))
                    if (temp.PE_ENG_Partnumber != "")
                        temp.PE_ENG_Partnumber += "; " + value.PE_ENG_Partnumber;
                    else
                        temp.PE_ENG_Partnumber = value.PE_ENG_Partnumber;

                if (!temp.PE_FLT_Partnumber.Contains(value.PE_FLT_Partnumber))
                    if (temp.PE_FLT_Partnumber != "")
                        temp.PE_FLT_Partnumber += "; " + value.PE_FLT_Partnumber;
                    else
                        temp.PE_FLT_Partnumber = value.PE_FLT_Partnumber;

                PNDictionary[key] = temp;
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
    public void Clear()
    {
        ((IDictionary<string, CompData>)PNDictionary).Clear();
    }

    public bool Contains(KeyValuePair<string, CompData> item)
    {
        return ((IDictionary<string, CompData>)PNDictionary).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, CompData>)PNDictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, CompData>[] array, int arrayIndex)
    {
        ((IDictionary<string, CompData>)PNDictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, CompData>> GetEnumerator()
    {
        return ((IDictionary<string, CompData>)PNDictionary).GetEnumerator();
    }

    public bool Remove(KeyValuePair<string, CompData> item)
    {
        return ((IDictionary<string, CompData>)PNDictionary).Remove(item);
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, CompData>)PNDictionary).Remove(key);
    }

    public bool TryGetValue(string key, out CompData value)
    {
        return ((IDictionary<string, CompData>)PNDictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, CompData>)PNDictionary).GetEnumerator();
    }
}
class RefDesCompList : IDictionary<string, CompData>
{
    public static readonly Logger _Log = LogManager.GetCurrentClassLogger();
    private Dictionary<string, CompData> RefDesDictionary = new Dictionary<string, CompData>();

    public CompData this[string key]
    {
        get
        {
            return ((IDictionary<string, CompData>)RefDesDictionary)[key];
        }

        set
        {
            ((IDictionary<string, CompData>)RefDesDictionary)[key] = value;
        }
    }

    public int Count
    {
        get
        {
            return ((IDictionary<string, CompData>)RefDesDictionary).Count;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            return ((IDictionary<string, CompData>)RefDesDictionary).IsReadOnly;
        }
    }

    public ICollection<string> Keys
    {
        get
        {
            return ((IDictionary<string, CompData>)RefDesDictionary).Keys;
        }
    }

    public ICollection<CompData> Values
    {
        get
        {
            return ((IDictionary<string, CompData>)RefDesDictionary).Values;
        }
    }

    public void Add(KeyValuePair<string, CompData> item)
    {
        ((IDictionary<string, CompData>)RefDesDictionary).Add(item);
    }

    public void Add(string key, CompData value)
    {
        try
        {
            CompData temp;

            if (!RefDesDictionary.ContainsKey(key))
                ((IDictionary<string, CompData>)RefDesDictionary).Add(key, value);
            else
            {
                temp = RefDesDictionary[key];

                if (!temp.Base_LibRef.Contains(value.Base_LibRef))
                    if (temp.Base_LibRef != "")
                        temp.Base_LibRef += "; " + value.Base_LibRef;
                    else
                        temp.Base_LibRef = value.Base_LibRef;

                if (!temp.Base_Partnumber.Contains(value.Base_Partnumber))
                    if (temp.Base_Partnumber != "")
                        temp.Base_Partnumber += "; " + value.Base_Partnumber;
                    else
                        temp.Base_Partnumber = value.Base_Partnumber;

                if (!temp.UniqueID.Contains(value.UniqueID))
                    if (temp.UniqueID != "")
                        temp.UniqueID += "; " + value.UniqueID;
                    else
                        temp.UniqueID = value.UniqueID;

                if (!temp.Var_Eng_LibRef.Contains(value.Var_Eng_LibRef))
                    if (temp.Var_Eng_LibRef != "")
                        temp.Var_Eng_LibRef += "; " + value.Var_Eng_LibRef;
                    else
                        temp.Var_Eng_LibRef = value.Var_Eng_LibRef;

                if (!temp.Var_Eng_Partnumber.Contains(value.Var_Eng_Partnumber))
                    if (temp.Var_Eng_Partnumber != "")
                        temp.Var_Eng_Partnumber += "; " + value.Var_Eng_Partnumber;
                    else
                        temp.Var_Eng_Partnumber = value.Var_Eng_Partnumber;

                if (!temp.Var_Flt_LibRef.Contains(value.Var_Flt_LibRef))
                    if (temp.Var_Flt_LibRef != "")
                        temp.Var_Flt_LibRef += "; " + value.Var_Flt_LibRef;
                    else
                        temp.Var_Flt_LibRef += "; " + value.Var_Flt_LibRef;

                if (!temp.Var_Flt_Partnumber.Contains(value.Var_Flt_Partnumber))
                    if (temp.Var_Flt_Partnumber != "")
                        temp.Var_Flt_Partnumber += "; " + value.Var_Flt_Partnumber;
                    else
                        temp.Var_Flt_Partnumber = value.Var_Flt_Partnumber;

                if (!temp.PE_ENG_Partnumber.Contains(value.PE_ENG_Partnumber))
                    if (temp.PE_ENG_Partnumber != "")
                        temp.PE_ENG_Partnumber += "; " + value.PE_ENG_Partnumber;
                    else
                        temp.PE_ENG_Partnumber = value.PE_ENG_Partnumber;

                if (!temp.PE_FLT_Partnumber.Contains(value.PE_FLT_Partnumber))
                    if (temp.PE_FLT_Partnumber != "")
                        temp.PE_FLT_Partnumber += "; " + value.PE_FLT_Partnumber;
                    else
                        temp.PE_FLT_Partnumber = value.PE_FLT_Partnumber;

                RefDesDictionary[key] = temp;
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

    public void Clear()
    {
        ((IDictionary<string, CompData>)RefDesDictionary).Clear();
    }

    public bool Contains(KeyValuePair<string, CompData> item)
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).Contains(item);
    }

    public bool ContainsKey(string key)
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).ContainsKey(key);
    }

    public void CopyTo(KeyValuePair<string, CompData>[] array, int arrayIndex)
    {
        ((IDictionary<string, CompData>)RefDesDictionary).CopyTo(array, arrayIndex);
    }

    public IEnumerator<KeyValuePair<string, CompData>> GetEnumerator()
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).GetEnumerator();
    }

    public bool Remove(KeyValuePair<string, CompData> item)
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).Remove(item);
    }

    public bool Remove(string key)
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).Remove(key);
    }

    public bool TryGetValue(string key, out CompData value)
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).TryGetValue(key, out value);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IDictionary<string, CompData>)RefDesDictionary).GetEnumerator();
    }
}
public struct CompData
{
    public string RefDes;
    public string UniqueID;
    public string Base_Partnumber;
    public string Base_LibRef;
    public string Var_Flt_Partnumber;
    public string Var_Flt_LibRef;
    public string Var_Eng_Partnumber;
    public string Var_Eng_LibRef;
    public string Footprint;
    public string PE_FLT_Partnumber;
    public string PE_ENG_Partnumber;
    public CompData(string PreLoad)
    {
        RefDes = PreLoad;
        UniqueID = PreLoad;
        Base_Partnumber = PreLoad;
        Base_LibRef = PreLoad;
        Var_Flt_Partnumber = PreLoad;
        Var_Flt_LibRef = PreLoad;
        Var_Eng_Partnumber = PreLoad;
        Var_Eng_LibRef = PreLoad;
        Footprint = PreLoad;
        PE_FLT_Partnumber = PreLoad;
        PE_ENG_Partnumber = PreLoad;
    }
    public override string ToString()
    {
        return PE_FLT_Partnumber + "," + PE_ENG_Partnumber + "," + Base_Partnumber + "," + Base_LibRef + "," + RefDes + "," + UniqueID + "," + Var_Flt_Partnumber + "," + Var_Flt_LibRef + "," + Var_Eng_Partnumber + "," + Var_Eng_LibRef;
    }
}
