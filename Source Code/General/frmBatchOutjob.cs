using DXP;
using EDP;
using NLog;
using PCB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


public partial class frmBatchOutjob : Form
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public Dictionary<string, clsOutJob> dictOutputMedium = new Dictionary<string, clsOutJob>();
    public IClient tmpClient = DXP.GlobalVars.Client;
    public List<IServerDocument> lstServerDocs = new List<IServerDocument>();
    public IWSM_OutputJobDocument OutJobDoc;
    public IOutputMedium OutputMedium;
    public IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
    clsODBFiles ODBFiles = new clsODBFiles();

    public frmBatchOutjob()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
    }

    private bool Checking = false;
    private void treeOutjobs_AfterCheck(object sender, TreeViewEventArgs e)
    {
        if (Checking) return;
        Checking = true;
        TreeNode nodeSelected = e.Node;
        //If selected node has child nodes, they are checked/unchecked as well.
        if (nodeSelected.Nodes.Count > 0)
        {
            foreach (TreeNode node in nodeSelected.Nodes)
                node.Checked = nodeSelected.Checked;
        }
        //Check the parent node of the selected node.
        else
        {
            nodeSelected = nodeSelected.Parent;
            foreach (TreeNode node in nodeSelected.Nodes)
            {
                if (node.Checked)
                {
                    nodeSelected.Checked = true;
                    Checking = false;
                    return;
                }
            }
            nodeSelected.Checked = false;
            Checking = false;
            return;
        }
        Checking = false;
    }

    private void btnGenerate_Click(object sender, System.EventArgs e)
    {
        try
        {
            foreach (TreeNode node in treeOutjobs.Nodes)
            {
                foreach (TreeNode _node in node.Nodes)
                {
                    _node.BackColor = System.Drawing.Color.White;
                }
            }
            string process, parameters;
            //Have the user select what outjobs to run.

            DXP.Utils.PercentInit("Running Selected Outjobs", treeOutjobs.Nodes.Count);

            //Start running outjobs based on what nodes are checked
            foreach (TreeNode node in treeOutjobs.Nodes)
            {
                if (node.Checked && node.Nodes.Count > 0)
                {
                    foreach (TreeNode _node in node.Nodes)
                    {
                        if (_node.Checked)
                        {
                            if (dictOutputMedium[_node.FullPath].ODB && ToolsPreferences.ODB_HideRefDes)//If ODB option enabled then hide all refdes' on the PCB before running outjob.
                                if (!PreODB()) break;
                            tmpClient.ShowDocument(dictOutputMedium[_node.FullPath].ServerDoc);
                            OutputMedium = dictOutputMedium[_node.FullPath].OutputMedium;

                            if (cboVariant.Enabled == true)
                            {
                                if (cboVariant.Text != "Original")
                                {
                                    OutJobDoc = (IWSM_OutputJobDocument)dictOutputMedium[_node.FullPath].ServerDoc;
                                    //Check to see if variant scope of outjob doc is not set to effect the entire document.
                                    if (OutJobDoc.GetState_VariantScope() != TOutputJobVariantScope.eVariantScope_DefinedForWholeOutputJob)
                                        //Set variant scope to effect entire document.
                                        OutJobDoc.SetState_VariantScope(TOutputJobVariantScope.eVariantScope_DefinedForWholeOutputJob);
                                    //Set variant setting in outjob to selected variant.
                                    if (OutJobDoc.GetState_VariantName() != cboVariant.Text)
                                        OutJobDoc.SetState_VariantName(cboVariant.Text);
                                }
                            }
                            //#if DEBUG
                            //                                string files="";
                            //                                OutJobDoc = (IWSM_OutputJobDocument)dictOutputMedium[_node.FullPath].ServerDoc;
                            //                                OutJobDoc.GetState_MediumOutputer(OutputMedium, 0).DM_Generate_OutputFilesTo(@"T:\users\RLYNE\test projects\SchB_PwbA\Datasheets", OutJobDoc.GetState_MediumOutputer(OutputMedium, 0).DM_Parameters(0).DM_Value());

                            //                                for (int i = 0; i < OutJobDoc.GetState_OutputerCount(); i++)
                            //                                {
                            //                                    _Log.Debug(OutJobDoc.GetState_Outputer(i).DM_GeneratorName());
                            //                                }

                            //#endif
                            //Generate outjob outputs.
                            switch (OutputMedium.GetState_TypeString())
                            {
                                case "PDF":
                                    process = "WorkspaceManager:Print";
                                    parameters = "Action=PublishToPDF|DisableDialog=True|ObjectKind=OutputBatch" +
                                                    "|OutputMedium=" + OutputMedium.GetState_Name() +
                                                    "|PromptOverwrite=FALSE|OpenOutput=FALSE";
                                    DXP.Utils.RunCommand(process, parameters);
                                    break;
                                case "Generate Files":

                                    process = "WorkspaceManager:GenerateReport";
                                    parameters = "Action=Run|ObjectKind=OutputBatch" +
                                                    "|OutputMedium=" + OutputMedium.GetState_Name() +
                                                    "|PromptOverwrite=FALSE|OpenOutput=FALSE";
                                    DXP.Utils.RunCommand(process, parameters);
                                    break;
                                case "Print":
                                    break;
                                default:
                                    MessageBox.Show("Unknown Output type: " + OutputMedium.GetState_TypeString() + ", name: " + OutputMedium.GetState_Name());
                                    break;

                            }
                            //Undo hidden refdes' if option is enabled.
                            if (dictOutputMedium[_node.FullPath].ODB && ToolsPreferences.ODB_HideRefDes)
                                if (!PostODB()) break;
                            //_node.ForeColor = System.Drawing.Color.ForestGreen;
                            _node.BackColor = System.Drawing.Color.LightGreen;
                            this.Refresh();
                            treeOutjobs.Refresh();
                        }
                    }
                }
                DXP.Utils.PercentUpdate();
            }

            DXP.Utils.PercentFinish();

            //Close outjob docs
            foreach (IServerDocument ServerDoc in lstServerDocs)
                tmpClient.CloseDocument(ServerDoc);
            //lstServerDocs.Clear();
            //dictOutputMedium.Clear();
            MessageBox.Show("Outjob batch generation complete.");

            DXP.Utils.StatusBarSetStateDefault();

            if (cbClose.Checked)
                this.Close();
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

    public void frmBatchOutjob_Load(object sender, System.EventArgs e)
    {
        IDXPProject Project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject();

        IProject project = DXP.GlobalVars.DXPWorkSpace.DM_FocusedProject() as IProject;

        //Load cboVariant with all variants in the active projet.
        if (project.DM_ProjectVariantCount() > 0)
        {
            cboVariant.Enabled = true;
            cboVariant.Items.Add("Original");
            cboVariant.Items.Add("[No Variations]");

            for (int i = 0; i < project.DM_ProjectVariantCount(); i++)
                cboVariant.Items.Add(project.DM_ProjectVariants(i).DM_Description());
            cboVariant.SelectedIndex = 0;
        }
        else
            cboVariant.Enabled = false;

        bool ODB = false;

        DXP.Utils.StatusBarSetState(2, "Collecting Outjobs");

        List<string> lstOutjobDocPaths = GetOutputJobPath(); //Get a list of all outjob docs in the project

        if (lstOutjobDocPaths == null) return;

        this.Text = Project.DM_ProjectFileName();


        //Open the outjob docs
        foreach (string strPath in lstOutjobDocPaths)
            lstServerDocs.Add(tmpClient.OpenDocument("OUTPUTJOB", strPath));

        DXP.Utils.PercentInit("Load Outjobs", lstServerDocs.Count);//Progressbar init.
        List<TreeNode> lstTreeNode;
        //Get outjob mediums
        foreach (IServerDocument ServerDoc in lstServerDocs)
        {
            OutJobDoc = (IWSM_OutputJobDocument)ServerDoc;

            lstTreeNode = new List<TreeNode>();
            for (int i = 0; i < OutJobDoc.GetState_OutputMediumCount(); i++)
            {
                ODB = false;
                for (int j = 0; j < OutJobDoc.GetState_MediumOutputersCount(OutJobDoc.GetState_OutputMedium(i)); j++)
                {
                    //Check to see if outjob container is will generate ODB++ files. Flag it for hide refdes later.
                    if (OutJobDoc.GetState_MediumOutputer(OutJobDoc.GetState_OutputMedium(i), j).DM_GetDescription() == "ODB++ Files")
                        ODB = true;
                }
                //If outjob is not a Print then store the information and add to list.
                if (OutJobDoc.GetState_OutputMedium(i).GetState_TypeString() != "Print")
                {
                    dictOutputMedium.Add(System.IO.Path.GetFileName(ServerDoc.GetFileName()) + "-" + OutJobDoc.GetState_OutputMedium(i).GetState_Name(), new clsOutJob(OutJobDoc.GetState_OutputMedium(i), ServerDoc, ODB));
                    lstTreeNode.Add(new TreeNode(OutJobDoc.GetState_OutputMedium(i).GetState_Name()));//Build list of nodes for form tree view
                }
                else
                {//Dont remember why this is excluded.
                 //throw new  NotImplementedException("Batchoutjob \"print\" medium type");
                }
            }
            //Add collected outjob info to treeview.
            treeOutjobs.Nodes.Add(new TreeNode(System.IO.Path.GetFileName(ServerDoc.GetFileName()), lstTreeNode.ToArray())); //Populate form treenodes
            DXP.Utils.PercentUpdate();
        }
        treeOutjobs.ExpandAll();

        DXP.Utils.PercentFinish();
        DXP.Utils.StatusBarSetState(2, "Select Outjobs to Run");
    }
    /// <summary>
    /// Hides refdes' before running ODB++ outputs
    /// </summary>
    /// <returns>True/False if successful.</returns>
    bool PreODB()
    {
        try
        {
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            IDXPDocument PCBDoc = null;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            IClient tmpClient = DXP.GlobalVars.Client;
            IServerDocument PCBServerDoc;

            //Get pcb doc.
            for (int i = 0; i < CurrentProject.DM_LogicalDocumentCount(); i++)
                if (CurrentProject.DM_LogicalDocuments(i).DM_DocumentKind() == "PCB")
                {
                    PCBDoc = CurrentProject.DM_LogicalDocuments(i);
                    break;
                }
            if (PCBDoc == null)
            {
                MessageBox.Show("Unable to get PCB.");
                return false;
            }
            PCBServerDoc = tmpClient.GetDocumentByPath(PCBDoc.DM_FullPath());
            if (PCBServerDoc == null) //PCB not already open.
            {
                PCBServerDoc = tmpClient.OpenDocument("PCB", PCBDoc.DM_FullPath());
                ODBFiles.WasOpen = false;
            }

            tmpClient.ShowDocument(PCBServerDoc);

            IPCB_Board Board;

            if (PCBServer == null)
                return false;
            //Get board from pcb doc.
            Board = PCBServer.GetPCBBoardByPath(PCBDoc.DM_FullPath()); //Get current board
            if (Board == null)
                return false;

            //Prep ODB class.
            ODBFiles.Board = Board;
            ODBFiles.PCBServerDoc = PCBServerDoc;

            PCBServer.PreProcess();
            //Hid refdes if enables in preferences.
            if (ToolsPreferences.ODB_HideRefDes)
                new ShowHideRefDes().ShowHide(Board, false);
            PCBServer.PostProcess();
            Board.ViewManager_FullUpdate();
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
    /// Undo PreODB() changes or close document.
    /// </summary>
    /// <returns>True/False if successful.</returns>
    bool PostODB()
    {
        try
        {
            IClient tmpClient = DXP.GlobalVars.Client;

            if (!ODBFiles.WasOpen) tmpClient.CloseDocument(ODBFiles.PCBServerDoc);//Close document if it wasnt already open.
            else//Undo changes.
            {
                tmpClient.ShowDocument(ODBFiles.PCBServerDoc);
                DXP.Utils.RunCommand("PCB:Undo", "");
                ODBFiles.Board.ViewManager_FullUpdate();
            }
            ODBFiles = new clsODBFiles();
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
    /// Get paths of all outjobs in the project.
    /// </summary>
    /// <returns>List of outjobs in project.</returns>
    public List<string> GetOutputJobPath()
    {
        try
        {
            List<string> DirList = new List<string>();
            IDXPWorkSpace CurrentWorkspace = DXP.GlobalVars.DXPWorkSpace;
            IDXPProject CurrentProject;
            int LogicalDocumentCount;
            int LoopIterator;
            IDXPDocument CurrentSheet;
            CurrentProject = CurrentWorkspace.DM_FocusedProject();
            LogicalDocumentCount = CurrentProject.DM_LogicalDocumentCount();

            int BoardCount = 0;
            string BoardName = "";
            DialogResult dlgResult;

            //Loop through project documents.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                //Trying to determine if there are multiple PCB files. Give warning if multiple PCBs.
                if (CurrentSheet.DM_DocumentKind() == "PCB")
                    if (BoardCount == 0)
                    {
                        BoardCount += 1;
                        BoardName = CurrentSheet.DM_FileName();
                    }
                    else
                    {
                        dlgResult = MessageBox.Show("There are multiple boards in this project. PCB outjobs will only run on the first board :" + BoardName + "\nDo you wish to continue?", "Multiple PCBs", MessageBoxButtons.YesNo);
                        if (dlgResult == DialogResult.No) return null;
                    }
                //Add outjob path to list.
                if (CurrentSheet.DM_DocumentKind() == "OUTPUTJOB")
                {
                    DirList.Add(CurrentSheet.DM_FullPath());
                }
            }
            return DirList;
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

    private void btnCancel_Click(object sender, EventArgs e)
    {
        foreach (IServerDocument ServerDoc in lstServerDocs)
            tmpClient.CloseDocument(ServerDoc);
        lstServerDocs.Clear();
        dictOutputMedium.Clear();
        this.Close();
    }
}

