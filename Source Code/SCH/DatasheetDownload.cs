using DXP;
using SCH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

class DatasheetDownload
{
    List<Datasheet> Datasheets = new List<Datasheet>();
    public void DownloadDatasheets()
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();

            if (Directory.Exists(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath())))
            {
                CollectDatasheets(false);
                if (!Directory.Exists(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\Datasheets\\"))
                    Directory.CreateDirectory(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\Datasheets\\");
                foreach (Datasheet item in Datasheets)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\Datasheets\\" + item.Document))
                        Directory.CreateDirectory(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\Datasheets\\" + item.Document);

                    File.Copy(item.Path, Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath()) + "\\Datasheets\\" + item.Document + "\\" + Path.GetFileName(item.Path));
                }


            }
            MessageBox.Show("Proccess Complete");
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    void CollectDatasheets(bool Overwrite)
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

            string RefDes;
            VarParam<string, string> Parameters = new VarParam<string, string>();

            DXP.Utils.PercentInit("Gathering Datasheets", LogicalDocumentCount);

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

                    ISch_Iterator LibraryIterator, PIterator;
                    ISch_Component Component;
                    ISch_Parameter Param;
                    Datasheet tmpDatasheet;
                    //Iterate theough all components on the schematic.
                    LibraryIterator = SCH.GlobalVars.SchServer.GetCurrentSchDocument().SchIterator_Create();
                    LibraryIterator.AddFilter_ObjectSet(new SCH.TObjectSet(SCH.TObjectId.eSchComponent));

                    Component = LibraryIterator.FirstSchObject() as ISch_Component;

                    while (Component != null)
                    {

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
                                //Param.GetState_Name()   "ComponentLink1URL" string
                                if (Param.GetState_Name() == "ComponentLink1URL")
                                    if (System.IO.File.Exists(Param.GetState_Text()))
                                    {
                                        tmpDatasheet = new Datasheet();
                                        tmpDatasheet.Document = System.IO.Path.GetFileNameWithoutExtension(CurrentSheet.DM_FileName());
                                        tmpDatasheet.Path = Param.GetState_Text();
                                        if (!Datasheets.Contains(tmpDatasheet))
                                            Datasheets.Add(tmpDatasheet);

                                        if (Overwrite)
                                            Param.SetState_Text("..\\Datasheets\\"+ tmpDatasheet.Document+"\\"+ System.IO.Path.GetFileName(Param.GetState_Text()));
                                    }
                            }
                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }

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
            return;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }
    struct Datasheet
    {
        public string Document;
        public string Path;
    }
}
