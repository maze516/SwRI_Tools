using DXP;
using NLog;
using PCB;
using System;
using System.Windows.Forms;


public partial class frmTracks : ServerPanelForm //Form
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;

    public const string PanelName = "TrackUtil";
    public const string PanelCaption = "Track Util";
    int trackWidth = 10;

    structPos LastPos = new structPos() { x = null, y = null, absolute = true };

    public frmTracks()
    {
        InitializeComponent();
        UI.ApplyADUITheme(this);
    }


    private void txtCmd_KeyPress(object sender, KeyPressEventArgs e)
    {

        structPos NewPos = new structPos() { x = null, y = null, absolute = true };
        if (e.KeyChar == (char)Keys.Enter)
        {
            if (txtCmd.Text.StartsWith("iy", StringComparison.CurrentCultureIgnoreCase) || txtCmd.Text.StartsWith("ix", StringComparison.CurrentCultureIgnoreCase) || txtCmd.Text.StartsWith("x", StringComparison.CurrentCultureIgnoreCase) || txtCmd.Text.StartsWith("y", StringComparison.CurrentCultureIgnoreCase))
            {
                if (LastPos.x == null && LastPos.y == null)
                {
                    LastPos = DecodeCommand(txtCmd.Text);
                    txtLog.AppendText("\n" + txtCmd.Text);
                    if (LastPos.x == null || LastPos.y == null)
                    {
                        txtLog.AppendText("\nIncorrect command format. (i.e. x 123 123)");
                        LastPos = new structPos() { x = null, y = null, absolute = true };
                        return;
                    }
                    txtCmd.Text = "";
                }
                else
                {
                    NewPos = DecodeCommand(txtCmd.Text);
                    if (NewPos.x == null && NewPos.y == null)
                    {
                        txtLog.AppendText("\nIncorrect command format. (i.e. ix 123 123)");
                        return;
                    }
                    if ((NewPos.x == null || NewPos.y == null) && NewPos.absolute == true)
                    {
                        txtLog.AppendText("\nIncorrect command format. (i.e. x 123 123)");
                        NewPos = new structPos() { x = null, y = null, absolute = true };
                        return;
                    }
                    if (NewPos.x == null) NewPos.x = 0;
                    if (NewPos.y == null) NewPos.y = 0;

                    txtLog.AppendText("\n" + txtCmd.Text);
                    txtCmd.Text = "";
                    AddTrack(NewPos, trackWidth);
                }

            }
            else if (txtCmd.Text.ToLower() == "new")
            {
                LastPos = new structPos() { x = null, y = null, absolute = true };
                txtLog.AppendText("\nProvide first coord.");
                txtCmd.Text = "";
                return;
            }
            else if (txtCmd.Text.ToLower() == "help")
            {
                //txtLog.AppendText("\n");
                txtLog.AppendText(@"
Command options: 
Offset from last position
ix [x value] [y value] (Second value is optional.)
iy [y value] [x value] (Second value is optional.)
                                        
Absolute position:
x [x value] [y value]
y [y value] [x value]

Track width:
w [value]
                                        
Start a new line:
new
                                        
Help menu:
help");
                txtCmd.Text = "";
            }
            else if (txtCmd.Text.StartsWith("w", StringComparison.CurrentCultureIgnoreCase))
            {
                string[] CmdInfo = txtCmd.Text.Split(' ');
                if(CmdInfo.Length==2)
                {
                    if (!Int32.TryParse(CmdInfo[1].ToLower(), out trackWidth))
                    {
                        txtLog.AppendText("\nUnable to determine value. Please try again. (i.e. w 10)");
                    }
                    txtLog.AppendText("\n" + txtCmd.Text);
                    txtCmd.Text = "";
                }
                else
                {
                    txtLog.AppendText("\nIncorrect command format. (i.e. w 10)");
                    txtCmd.Text = "";
                }
            }
            else
            {
                txtLog.AppendText("\nIncorrect command format. (i.e. ix 123 123)");
                return;
            }
        }
    }
    
    /// <summary>
    /// Used to decode the text command provided by user.
    /// </summary>
    /// <param name="Cmd">Text command</param>
    /// <returns>Returns coord of command</returns>
    structPos DecodeCommand(string Cmd)
    {
        structPos Output = new structPos() { x = null, y = null, absolute = true };
        try
        {
            string[] CmdInfo = Cmd.Split(' ');
            if (CmdInfo.Length < 2)
            {
                return new structPos() { x = null, y = null };
            }

            if (CmdInfo[0].ToLower() == "iy" || CmdInfo[0].ToLower() == "y")
            {
                Output.y = Int32.Parse(CmdInfo[1]);
                if (CmdInfo.Length > 2)
                    Output.x = Int32.Parse(CmdInfo[2]);
            }
            else if (CmdInfo[0].ToLower() == "ix" || CmdInfo[0].ToLower() == "x")
            {
                Output.x = Int32.Parse(CmdInfo[1]);
                if (CmdInfo.Length > 2)
                    Output.y = Int32.Parse(CmdInfo[2]);
            }
            if (CmdInfo[0].ToLower() == "iy" || CmdInfo[0].ToLower() == "ix")
            {
                Output.absolute = false;
            }
            return Output;
        }
        catch// (Exception e)
        {
            Output.x = null;
            Output.y = null;
            return Output;
        }
    }
    /// <summary>
    /// Adds a new track to the PCB
    /// </summary>
    /// <param name="Offset"></param>
    /// <param name="argWidth"></param>
    private void AddTrack(structPos Offset, int argWidth)//, V7_Layer argLayer)
    {
        PCBServer = PCB.GlobalVars.PCBServer;
        if (Offset.x == null) Offset.x = 0;
        if (Offset.y == null) Offset.y = 0;

        int xL, yL, xO, yO;
        IPCB_Board pcbBoard = Util.GetCurrentPCB();

        if (pcbBoard == null)
            return;

        _Log.Debug(pcbBoard.GetState_DisplayUnit().ToString());



        TV6_Layer ActiveLayer = pcbBoard.GetState_CurrentLayer();

        int OriginX = pcbBoard.GetState_XOrigin();
        int OriginY = pcbBoard.GetState_YOrigin();

        PCBServer.PreProcess();

        // Set the value of J to point to the "next" vertex; this is normally
        // I + 1, but needs to be set to 0 instead for the very last vertex
        // that is processed by this loop.
        IPCB_Primitive primitive = PCBServer.PCBObjectFactory(TObjectId.eTrackObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default);
        if (primitive == null)
            return;
        IPCB_Track track = primitive as IPCB_Track;

        if (pcbBoard.GetState_DisplayUnit() == TUnit.eImperial)
        {
            xL = EDP.Utils.MilsToCoord((double)LastPos.x);
            yL = EDP.Utils.MilsToCoord((double)LastPos.y);
            xO = EDP.Utils.MilsToCoord((double)Offset.x);
            yO = EDP.Utils.MilsToCoord((double)Offset.y);
            argWidth = EDP.Utils.MilsToCoord((double)argWidth);
        }
        else
        {
            xL = EDP.Utils.MMsToCoord((double)LastPos.x);
            yL = EDP.Utils.MMsToCoord((double)LastPos.y);
            xO = EDP.Utils.MMsToCoord((double)Offset.x);
            yO = EDP.Utils.MMsToCoord((double)Offset.y);
            argWidth = EDP.Utils.MMsToCoord((double)argWidth);
        }

        if (Offset.absolute)
        {
            track.SetState_X1(OriginX + xL);
            track.SetState_Y1(OriginY + yL);
            track.SetState_X2(OriginX + xO);
            track.SetState_Y2(OriginY + yO);

            LastPos = Offset;
        }
        else
        {
            track.SetState_X1(OriginX + xL);
            track.SetState_Y1(OriginY + yL);
            track.SetState_X2(OriginX + (xL + xO));
            track.SetState_Y2(OriginY + (yL + yO));
            
            LastPos.x = LastPos.x + Offset.x;
            LastPos.y = LastPos.y + Offset.y;
        }

        track.SetState_Layer(ActiveLayer);
        track.SetState_Width(argWidth);
        pcbBoard.AddPCBObject(primitive);

        PCBServer.PostProcess();


        // Refresh PCB workspace.
        //DXP.Utils.RunCommand("PCB:Zoom", "Action=Redraw");
    }
    
    /// <summary>
    /// XY coord and absolute value bool.
    /// </summary>
    struct structPos
    {
        public float? x, y;
        public bool absolute;
    }


    private void txtLog_TextChanged(object sender, EventArgs e)
    {
        txtLog.ScrollToCaret();
    }

    ///// <summary>
    ///// Not used. Needs to be tested.
    ///// </summary>
    //public void Command_CreatePCBLib()//IServerDocumentView view, ref string parameters)
    //{
    //    //DXP.Utils.RunCommand("WorkspaceManager:OpenObject", "ObjectKind=NewAnything|Kind=DefaultPcbLib");

    //    IServerDocument doc = DXP.GlobalVars.Client.GetCurrentView().GetOwnerDocument();
    //    if (doc.GetKind() != "PCBLIB")
    //    {
    //        DXP.Utils.ShowWarning("Fail to create PCBLib document!");
    //        return;
    //    }
    //    //doc.SetFileName("C:\test.PcbLib");
    //    //doc.DoFileSave("PCB Library File");

    //    IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
    //    if (PCBServer == null)
    //        return;

    //    IPCB_Library pcbLib = PCBServer.GetCurrentPCBLibrary();
    //    if (pcbLib == null)
    //    {
    //        DXP.Utils.ShowMessage("Fail to create PCBLib document!");
    //        return;
    //    }

    //    //IPCB_LibComponent libComp = PCBServer.CreatePCBLibComp();
    //    IPCB_LibComponent libComp = pcbLib.GetState_CurrentComponent();

    //    if (libComp != null)
    //    {
    //        libComp.BeginModify();
    //        //libComp.SetState_Pattern("TestComp");
    //        //pcbLib.RegisterComponent(libComp);
    //        IPCB_ComponentBody Body = CreateABodyElement(libComp);
    //        //pcbLib.SetBoardToComponentByName("TestComp");
    //        libComp.EndModify();
    //        DXP.Utils.RunCommand("PCB:Zoom", "Action=All");
    //    }
    //    else
    //        DXP.Utils.ShowMessage("Fail to create PCB library component.");


    //}

    ///// <summary>
    ///// Not used. Needs to be tested.
    ///// </summary>
    //void PopulateContour(IPCB_Contour contour)
    //{
    //    contour.SetState_Count(5);
    //    contour.SetState_PointX(0, EDP.Utils.MilsToCoord(100));
    //    contour.SetState_PointY(0, EDP.Utils.MilsToCoord(100));

    //    contour.SetState_PointX(1, EDP.Utils.MilsToCoord(200));
    //    contour.SetState_PointY(1, EDP.Utils.MilsToCoord(100));

    //    contour.SetState_PointX(2, EDP.Utils.MilsToCoord(100));
    //    contour.SetState_PointY(2, EDP.Utils.MilsToCoord(200));

    //    contour.SetState_PointX(3, EDP.Utils.MilsToCoord(100));
    //    contour.SetState_PointY(3, EDP.Utils.MilsToCoord(200));

    //    contour.SetState_PointX(4, EDP.Utils.MilsToCoord(100));
    //    contour.SetState_PointY(4, EDP.Utils.MilsToCoord(100));
    //}

    ///// <summary>
    ///// Not used. Needs to be tested.
    ///// </summary>
    //IPCB_ComponentBody CreateABodyElement(IPCB_LibComponent libComp)
    //{
    //    IPCB_Board board = Util.GetCurrentPCB();
    //    if (board == null)
    //        return null;
    //    //PCB.IPCB_Board.GetState_XOrigin()
    //    //PCB.IPCB_Board.GetState_YOrigin()
    //    IPCB_ComponentBody compBody = PCBServer.PCBObjectFactory(TObjectId.eComponentBodyObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default) as IPCB_ComponentBody;
    //    if (compBody != null)
    //    {
    //        IPCB_Model model = compBody.ModelFactory_CreateExtruded(0, EDP.Utils.MilsToCoord(10), 0);
    //        if (model != null)
    //        {
    //            model.SetState(0, 0, 0, 0);
    //            PopulateContour(compBody.GetMainContour());
    //            compBody.SetOverallHeight(EDP.Utils.MilsToCoord(10));
    //            compBody.SetModel(model);
    //            compBody.SetStandoffHeight(0);
    //            compBody.SetBodyColor3D(255);
    //            compBody.SetState_Identifier("Test");
    //            libComp.AddPCBObject(compBody);
    //            IServerDocument doc = DXP.GlobalVars.Client.GetDocumentByPath(board.GetState_FileName());
    //            if (doc != null)
    //                doc.SetModified(true);
    //        }
    //    }

    //    return compBody;
    //}


}

