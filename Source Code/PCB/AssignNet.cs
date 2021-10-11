using PCB;
using DXP;
using EDP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NLog;

class AssignNetClss
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public void AssignNet()
    {
        IClient tmpClient = DXP.GlobalVars.Client;
        IPCB_Primitive FirstObj = null;
        IPCB_Primitive SecondObj = null;
        IPCB_Board board = Util.GetCurrentPCB();
        int NewX = 0, NewY = 0;
        try
        {
            //If a object is already selected, Use it. If not, then get the object under the cursor.
            if (board.GetState_SelectecObjectCount() != 1)
                FirstObj = (IPCB_Primitive)board.GetObjectAtXYAskUserIfAmbiguous(board.GetState_XCursor(), board.GetState_YCursor(), Util.PCBAllObject, PCBConstant.V6AllLayersSet, TEditingAction.eEditAction_Focus);
            if (FirstObj == null)
                FirstObj = (IPCB_Primitive)board.GetLastClickedObject(Util.PCBAllObject, TEditingAction.eEditAction_Select);
            else
                FirstObj = (IPCB_Primitive)board.GetState_SelectecObject(0);

            if (FirstObj == null) return;

            board.ChooseLocation(ref NewX, ref NewY, "Select desired net.");
            if (NewX > 0 & NewY > 0)
                SecondObj = (IPCB_Primitive)board.GetObjectAtXYAskUserIfAmbiguous(NewX, NewY, Util.PCBAllObject, PCBConstant.V6AllLayersSet, TEditingAction.eEditAction_Select);

            if (SecondObj == null) return;

            if (SecondObj.GetState_Net() != null)
            {
#if DEBUG
                MessageBox.Show(SecondObj.GetState_Net().GetState_Name());
#endif
                FirstObj.BeginModify();
                FirstObj.SetState_Net(SecondObj.GetState_Net());
                FirstObj.EndModify();
            }

            if (FirstObj.GetState_ObjectID() == TObjectId.eSplitPlaneObject)
            {
                DXP.Utils.RunCommand("PCB:EditInternalPlanes", "RebuildPlane=All");
            }
            //PCB:EditInternalPlanes
            //RebuildPlane=All
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
}

