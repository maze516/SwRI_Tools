using DXP;
using NLog;
using SCH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class RenameNets
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
    void LoadNets(string FilePath)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

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
    void SetNets()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        //try
        //{
        //    IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
        //    IDXPProject CurrentProject;
        //    int LogicalDocumentCount;
        //    int LoopIterator;
        //    IDXPDocument CurrentSheet;
        //    CurrentProject = CurrentWorkspace.DM_FocusedProject();
        //    LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();
        //    ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;
        //    IClient Client = DXP.GlobalVars.Client;
        //    IServerDocument ServerDoc;
        //    IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

        //    bool DocOpened = false;
        //    for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
        //    {
        //        CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
        //        if (CurrentSheet.DM_DocumentKind() == "SCH")
        //        {
        //            DocOpened = false;
        //            if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
        //            {
        //                ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
        //                DocOpened = true;
        //            }
        //            else
        //                ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());

        //            //Client.ShowDocument(ServerDoc);

        //            ISch_Lib SchDoc;
        //            SchDoc = SchServer.LoadSchDocumentByPath(CurrentSheet.DM_FullPath()) as ISch_Lib;

        //            ISch_Iterator NetIterator;
        //            ISch_NetLabel Net;
        //            VarParam<string, string> CompVars = new VarParam<string, string>();
        //            ISch_Parameter Param;
        //            bool Flt = false, Eng = false;
        //            //Iterate theough all components on the schematic.
        //            NetIterator = SchDoc.SchIterator_Create();
        //            NetIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eNetLabel));

        //            Net = NetIterator.FirstSchObject() as ISch_NetLabel;
        //            while (Net != null)
        //            {//TODO: what to do when multiple keys
        //                if (!Output.ContainsKey(Net.GetState_SchDesignator().GetState_Text()))
        //                    Output.Add(Net.GetState_SchDesignator().GetState_Text(), Net.GetState_DesignItemId());


        //                //Iterate theough all parameters in the component.
        //                PIterator = Net.SchIterator_Create();
        //                PIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eParameter));

        //                Param = PIterator.FirstSchObject() as ISch_Parameter;
        //                while (Param != null)
        //                {
        //                    if (Param.GetState_Name() != null)
        //                        if ("PE_ENG" == Param.GetState_Name().ToUpper())
        //                            Eng = true;
        //                        else if (Param.GetState_Name().ToUpper() == "PE_FLT")
        //                            Flt = true;

        //                    Param = PIterator.NextSchObject() as ISch_Parameter;
        //                }

        //                if (!Flt)
        //                {
        //                    Param = Net.AddSchParameter();
        //                    Param.SetState_Name("PE_FLT");
        //                    Param.SetState_Text("x");
        //                }

        //                if (!Eng)
        //                {
        //                    Param = Net.AddSchParameter();
        //                    Param.SetState_Name("PE_ENG");
        //                    Param.SetState_Text("x");
        //                }
        //                Flt = false;
        //                Eng = false;
        //                Net = LibraryIterator.NextSchObject() as ISch_Component;
        //            }

        //            if (!DocOpened)
        //                Client.CloseDocument(ServerDoc);

        //            ServerDoc = null;

        //        }
        //    }

        //    Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));

        //    return Output;
        //}
        //catch (Exception ex)
        //{
        //    ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        //    return null;
        //}
    }
}

