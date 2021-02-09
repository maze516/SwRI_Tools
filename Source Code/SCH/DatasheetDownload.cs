using DXP;
using SCH;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using EDP;
//TODO: add option to copy over the schematic with datasheet paths redirected to new "downloads"

class DatasheetDownload
{
    List<Datasheet> Datasheets = new List<Datasheet>();
    string LatestPDF = "";

    public void DownloadDatasheets()
    {
        //if (MessageBox.Show("This tool will close schematic pages without saveing. Do you wish to continue?", "Data may be lost", MessageBoxButtons.YesNo) == DialogResult.No) return;
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            string ProjectFullPath = "";
            if (Directory.Exists(Path.GetDirectoryName(CurrentProject.DM_ProjectFullPath())))
            {
                ProjectFullPath = CurrentProject.DM_ProjectFullPath();
                CollectDatasheets(false);
                if (!Directory.Exists(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\"))
                    Directory.CreateDirectory(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\");

                if (File.Exists(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + Path.GetFileName(LatestPDF)))
                    File.Delete(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + Path.GetFileName(LatestPDF));

                File.Move(LatestPDF, Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + Path.GetFileName(LatestPDF));

                foreach (Datasheet item in Datasheets)
                {
                    if (!Directory.Exists(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + item.Document))
                        Directory.CreateDirectory(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + item.Document);

                    File.Copy(item.Path, Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + item.Document + "\\" + Path.GetFileName(item.Path), true);
                }

            }

            UpdatePDF(Path.GetDirectoryName(ProjectFullPath) + "\\Datasheets\\" + Path.GetFileName(LatestPDF), ProjectFullPath);

            MessageBox.Show("Proccess Complete");
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    bool UpdatePDF(string path, string ProjectFullPath)
    {
        try
        {
            if (!path.ToLower().EndsWith("pdf")) return true;
            Encoding encode = Encoding.Default;
            string text = File.ReadAllText(path, encode);
            string test;
            text = text.Replace(@"/:\\s\(\(\\/\)|\(\\\\\\\\\)\).+/", @"/:.+[a-z]/i"); // @"/:.+\\.+\(pdf\)/i", @"/:\\s[a-z]:.+/i"

            foreach (Datasheet item in Datasheets)
            {
                test = item.Path.Replace("\\", "\\\\\\\\");
                test = test.Replace("(", "\\(");
                test = test.Replace(")", "\\)");
                text = text.Replace(test, item.Document + "\\\\\\\\" + Path.GetFileName(item.Path));
            }
            File.WriteAllText(path, text, encode);
            return true;
        }
        catch (Exception)
        {
            return false;
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
            List<IServerDocument> ProjDocs = new List<IServerDocument>();
            //List<string> OpenDocsPath = new List<string>();
            IDXPDocument ActiveDoc = DXP.GlobalVars.DXPWorkSpace.DM_FocusedDocument(); //Save current open document so it can be reopened after process is done.
            bool DocOpened;
            string RefDes;
            VarParam<string, string> Parameters = new VarParam<string, string>();

            DXP.Utils.PercentInit("Gathering Datasheets", LogicalDocumentCount);

            string DocPath = "";
            //Loop through each SCH document in project.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {

                    SchDoc = CurrentSheet as ISch_Lib;
                    DocPath = CurrentSheet.DM_FullPath();
                    DocOpened = false;
                    //Open document if not already open.
                    if (Client.IsDocumentOpen(DocPath))
                    {
                        ServerDoc = Client.GetDocumentByPath(DocPath);
                        //OpenDocsPath.Add(DocPath);
                        DocOpened = true;
                    }
                    else
                    {
                        ServerDoc = Client.OpenDocument("SCH", DocPath);
                        
                    }

                    if (ServerDoc == null)
                    {
                        MessageBox.Show("Error opening project schematic document.\r\nPlease report to Randy Lyne x2259");
                        return;
                    }

                    ProjDocs.Add(ServerDoc);
                    Client.ShowDocument(ServerDoc);


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
                                            Param.SetState_Text(tmpDatasheet.Document + "\\" + System.IO.Path.GetFileName(Param.GetState_Text()));
                                    }
                            }
                            Param = PIterator.NextSchObject() as ISch_Parameter;
                        }

                        Component = LibraryIterator.NextSchObject() as ISch_Component;
                    }

                    if (!DocOpened)
                        Client.CloseDocument(ServerDoc);
                    //else
                    //{
                    //    Client.CloseDocument(ServerDoc);
                    //    Client.OpenDocument("SCH", DocPath);
                    //}
                    ServerDoc = null;

                }
                DXP.Utils.PercentUpdate();
            }

            Client.ShowDocument(Client.GetDocumentByPath(ActiveDoc.DM_FullPath()));



            string process, parameters;
            IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();
            List<IServerDocument> lstServerDocs = new List<IServerDocument>();
            IWSM_OutputJobDocument OutJobDoc;
            IOutputMedium OutputMedium;

            IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;

            List<string> lstOutjobDocPaths = new frmBatchOutjob().GetOutputJobPath(); //Get a list of all outjob docs in the project

            if (lstOutjobDocPaths == null)
                return;

            //TODO: new regex \w+\.(pdf|docx|doc|xlsx|xls)
            //Open the outjob docs
            foreach (string strPath in lstOutjobDocPaths)
                lstServerDocs.Add(Client.OpenDocument("OUTPUTJOB", strPath));


            //Get outjob mediums
            foreach (IServerDocument SrvrDoc in lstServerDocs)
            {
                OutJobDoc = (IWSM_OutputJobDocument)SrvrDoc;


                for (int i = 0; i < OutJobDoc.GetState_OutputMediumCount(); i++)
                {
                    //if (OutJobDoc.GetState_OutputMedium(i).GetState_TypeString() == "PDF")
                    //{
                    OutputMedium = OutJobDoc.GetState_OutputMedium(i);
                    if (OutputMedium.GetState_Name() == "PDF")
                    {
                        Client.ShowDocument(SrvrDoc);
                        string PDFPath = OutputMedium.GetState_OutputPath();

                        //OutputMedium.SetState_OutputPath("T:\\users\\RLYNE\\test projects\\SchB_PwbA\\Datasheets\\");
                        //OutJobDoc = (IWSM_OutputJobDocument)SrvrDoc;

                        //Generate outjob outputs.

                        process = "WorkspaceManager:Print";
                        parameters = "Action=PublishToPDF|DisableDialog=True|ObjectKind=OutputBatch" +
                                        "|OutputMedium=" + OutputMedium.GetState_Name() +
                                        "|PromptOverwrite=FALSE|OpenOutput=FALSE";
                        DXP.Utils.RunCommand(process, parameters);

                        //foreach (IServerDocument tmpServerDoc in lstServerDocs)
                        //    Client.CloseDocument(tmpServerDoc);

                        string[] PDFs;
                        PDFs = Directory.GetFiles(PDFPath, "*.pdf");
                        DateTime LatestDT = new DateTime();
                        foreach (string item in PDFs)
                        {
                            if (File.GetCreationTime(item).Date == DateTime.Today.Date)
                                if (File.GetCreationTime(item) > LatestDT)
                                {
                                    LatestDT = File.GetCreationTime(item);
                                    LatestPDF = item;
                                }
                        }

                        //return;
                    }

                    //}
                }
            }

            //foreach (IServerDocument item in ProjDocs)
            //    Client.CloseDocument(item);

            //foreach (string path in OpenDocsPath)
            //{
            //    Client.ShowDocument(Client.OpenDocument("SCH", path));

            //}
            DXP.Utils.PercentFinish();

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
