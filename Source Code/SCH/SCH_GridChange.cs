using DXP;
using NLog;
using SCH;
using System;
using System.Collections.Generic;


public class SCH_GridChange
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    ISch_ServerInterface SchServer = SCH.GlobalVars.SchServer;

    /// <summary>
    /// Class entry that will decode the provided parameters
    /// and run ChangeGridSize().
    /// </summary>
    /// <param name="parameters">Altium provided parameter string.</param>
    public void GridChange(string parameters)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            int GridSize = 0;
            bool Align = false;
            Dictionary<string, string> Params = new Dictionary<string, string>() { { "relative", "" }, { "filename", "" }, { "template", "" } };

            //Populate parameter list.
            foreach (string Param in parameters.Split(','))
            {
                if (Param.Split('=')[0].ToLower() == "size")
                    GridSize = Int32.Parse(Param.Split('=')[1].ToString());
                else if (Param.Split('=')[0].ToLower() == "align")
                {
                    if (Param.Split('=')[1].ToLower() == "false")
                        Align = false;
                    else if (Param.Split('=')[1].ToLower() == "true")
                        Align = true;
                }
            }

            if (GridSize != 0)
            {
                DXP.Utils.StatusBarSetState(2, "Updating Grid");

                ChangeGridSize(GridSize, Align);

                DXP.Utils.StatusBarSetStateDefault();
            }
            else
                DXP.Utils.ShowError("Parameters are incorrect. Please correct and try again.", "Parameter Error");
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
    /// Selects all components on the current schematic page.
    /// </summary>
    void SelectAll()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            ISch_Document SchDoc = SchServer.GetCurrentSchDocument();
            ISch_Iterator SchIterator;
            ISch_GraphicalObject Component;

            if (SchDoc == null)
                return;
            //Iterate theough all objects on the schematic.
            SchIterator = SchDoc.SchIterator_Create();
            SchIterator.AddFilter_ObjectSet(new SCH.TObjectSet(AllObjects()));
            //Select all objects on the current page.
            Component = SchIterator.FirstSchObject() as ISch_GraphicalObject;
            while (Component != null)
            {
                Component.SetState_Selection(true);
                Component = SchIterator.NextSchObject() as ISch_GraphicalObject;
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
    /// Sets the grid snap size for the current schematic page.
    /// </summary>
    /// <param name="SizeInMils">Desired grid size in mils.</param>
    void SetGrid(int SizeInMils)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            ISch_Document SchDoc = SCH.GlobalVars.SchServer.GetCurrentSchDocument(); //Get current SC document.

            SchDoc.SetState_SnapGridSize(EDP.Utils.MilsToCoord(SizeInMils)); //Change document grid size.
            SchDoc.UpdateDocumentProperties();
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
    /// Array of objects to be selected when GetParamHeights() is run.
    /// </summary>
    /// <returns>TObjectId array of object types to select.</returns>
    SCH.TObjectId[] AllObjects()
    {
        return new SCH.TObjectId[] { SCH.TObjectId.eWire, SCH.TObjectId.eNote, SCH.TObjectId.eLine, SCH.TObjectId.eLabel, SCH.TObjectId.eDesignator, SCH.TObjectId.eSchComponent, SCH.TObjectId.eSheetSymbol, SCH.TObjectId.eSymbol, SCH.TObjectId.ePowerObject, SCH.TObjectId.ePort, SCH.TObjectId.eProbe, SCH.TObjectId.eRectangle, SCH.TObjectId.eRoundRectangle, SCH.TObjectId.eSignalHarness, SCH.TObjectId.ePin, SCH.TObjectId.eNetLabel, SCH.TObjectId.eNoERC, SCH.TObjectId.eLine, SCH.TObjectId.eJunction, SCH.TObjectId.eTextFrame };
    }

    

    /// <summary>
    /// Selects all objects on current schematic page then runs the AlignObjects process.
    /// </summary>
    void AlignToGrid()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            SelectAll();
            DXP.Utils.RunCommand("Sch:AlignObjects", "Action=Grid");
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
    /// Iterates through all schematic docs of the active project and 
    /// changes the grid snap and aligns components based on parameters.
    /// </summary>
    /// <param name="SizeInMils">Desired grid size in mils.</param>
    /// <param name="AlignToGrid">True/False if components should be aligned to the new grid.</param>
    void ChangeGridSize(int SizeInMils, bool AlignToGrid = false)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

            DXP.Utils.PercentInit("Updating Grid", LogicalDocumentCount);

            bool DocOpened = false;

            //Loop through each SCH document in project.
            for (LoopIterator = 1; LoopIterator <= LogicalDocumentCount; LoopIterator++)
            {
                CurrentSheet = CurrentProject.DM_LogicalDocuments(LoopIterator - 1);
                if (CurrentSheet.DM_DocumentKind() == "SCH")
                {
                    DocOpened = false;
                    SchDoc = CurrentSheet as ISch_Document;
                    //Open document if not already open.
                    if (Client.IsDocumentOpen(CurrentSheet.DM_FullPath()))
                    {
                        ServerDoc = Client.GetDocumentByPath(CurrentSheet.DM_FullPath());
                        DocOpened = true;
                    }
                    else
                        ServerDoc = Client.OpenDocument("SCH", CurrentSheet.DM_FullPath());
                    Client.ShowDocument(ServerDoc);
                    SetGrid(SizeInMils); //Set document grid size.
                    ServerDoc.DoFileSave(""); //Save file.

                    //Align parts to grid of active document.
                    if (AlignToGrid)
                        this.AlignToGrid();
                    if (!DocOpened & !AlignToGrid)
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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }
}
