using DXP;
using SCH;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class DuplicateRefDes
{

    public void CheckRefDes()
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
            ISch_Lib SchDoc;
            IClient Client = DXP.GlobalVars.Client;
            IServerDocument ServerDoc;
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.

            List<string> lstRefDes = new List<string>();
            List<string> Output = new List<string>();
            string RefDes;

            Output.Add(DateTime.Now + "," + System.IO.Path.GetFileName(CurrentProject.DM_ProjectFullPath()));
            Output.Add("");
            Output.Add("Compare,Ref1,Ref2");

            DXP.Utils.PercentInit("Collecting Refdes Values", LogicalDocumentCount);

            bool DocOpened = false;

            //Loop through each SCH document in project.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    DocOpened = false;
                    SchDoc = CurrentSheet as ISch_Lib;
                    //Open document if not already open.
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());


                    Client.ShowDocument(ServerDoc);

                    if (ServerDoc == null)
                        return;

                    ISch_Iterator LibraryIterator;
                    ISch_Component Component;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SCH.GlobalVars.SchServer.GetCurrentSchDocument().SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;

                    while (Component != null)
                    {

                        RefDes = Component.GetState_SchDesignator().GetState_Text();
                        if (Component.GetState_CurrentPartID() == 1)
                            lstRefDes.Add(RefDes);

                        Component = LibraryIterator.NextSchObject() as ISch_Component;
                    }

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);
                    ServerDoc = null;

                }
                DXP.Utils.PercentUpdate();
            }
            DXP.Utils.PercentFinish();
            Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));

            lstRefDes.Sort();

            DXP.Utils.PercentInit("Checking for Dupes", lstRefDes.Count);
            for (int i = 1; i < lstRefDes.Count; i++)
            {
                if (lstRefDes[i - 1] == lstRefDes[i])
                    Output.Add("Duplicate, " + lstRefDes[i]);
                DXP.Utils.PercentUpdate();
            }
            DXP.Utils.PercentFinish();

            DXP.Utils.PercentInit("Checking for Similar", lstRefDes.Count);
            string[] First, Second;
            for (int i = 1; i < lstRefDes.Count; i++)
            {
                //if (lstRefDes[i].Length > 3)
                First = SplitString(lstRefDes[i]);

                if (First.Length >= 2)
                    for (int j = 1; j < lstRefDes.Count; j++)
                    {
                        //if (lstRefDes[i-1].Length > 3)
                        Second = SplitString(lstRefDes[j]);
                        if (First.Length != Second.Length)
                            if (First.Length != 3 || Second.Length != 3)
                                if (Second.Length > 2)
                                {
                                    if (First[0] == Second[0] && First[1] == Second[1] && i != j)
                                        Output.Add("Similar, " + lstRefDes[i] + ", " + lstRefDes[j]);
                                }

                    }
                //if (lstRefDes[i].Contains(lstRefDes[i - 1]))
                //Output.Add("Similar, " + lstRefDes[i] + ", " + lstRefDes[i - 1]);
                DXP.Utils.PercentUpdate();
            }
            //Output.Add("");
            DXP.Utils.PercentFinish();

            bool match = false;
            DXP.Utils.PercentInit("Checking EM/FM Parts", lstRefDes.Count);
            for (int i = 0; i < lstRefDes.Count; i++)
            {
                if (lstRefDes[i].Length > 3)
                    if (lstRefDes[i].Contains("EM") || lstRefDes[i].Contains("FM"))
                    {
                        match = false;
                        RefDes = lstRefDes[i].Remove(lstRefDes[i].Length - 2);
                        for (int j = 1; j < lstRefDes.Count; j++)
                        {
                            if (lstRefDes[j].Contains("EM") || lstRefDes[j].Contains("FM"))
                                if (RefDes == lstRefDes[j].Remove(lstRefDes[j].Length - 2) && i != j)
                                {
                                    Output.Add("Matched EM/FM, " + lstRefDes[i] + ", " + lstRefDes[j]);
                                    match = true;
                                    i += 1;
                                    break;
                                }
                        }
                        if (match == false)
                            Output.Add("Unmatched EM/FM, " + lstRefDes[i]);
                    }

                DXP.Utils.PercentUpdate();
            }
            DXP.Utils.PercentFinish();

            Util.Log(Output, Util.ProjPath() + "Refdes Report.csv");
            ServerDoc = Client.OpenDocument("Text", Util.ProjPath() + "Refdes Report.csv");
            Client.ShowDocument(ServerDoc);
            return;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }

    }

    string[] SplitString(string input)
    {

        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(input);
        string remainder = input.Replace(result.Groups[0].Value, null);
        string[] output;

        if (remainder != "")
            output = new string[result.Groups.Count];
        else
            output = new string[result.Groups.Count - 1];

        output[0] = result.Groups[1].Value;
        output[1] = result.Groups[2].Value;

        if (remainder != "")
            output[output.Length - 1] = remainder;


        return output;
    }
}

