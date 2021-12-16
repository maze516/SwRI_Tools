using PCB;
using DXP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using NLog;

class xSignals
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public void Get_xSignals()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        xSignalReport();
        //IPCB_Board board = Util.GetCurrentPCB();
        //IPCB_PinPairsManager PinPairs = board.GetState_PinPairsManager();
        //Utils.ShowMessage(PinPairs.GetState_PinPairsCount().ToString());
    }
     void xSignalReport()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        IPCB_Board Board;
        
        IPCB_PinPairsManager PinPairsManager;
        IPCB_PinPair PinPair;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        
        if (PCBServer == null)
            return;
        //Get current board.
        Board = PCBServer.GetCurrentPCBBoard();
        if (Board == null)
            return;

        //Get board pinpair manager used for xSignals.
        PinPairsManager = Board.GetState_PinPairsManager();
        if (PinPairsManager == null)
            return;
        List<IPCB_PinPair> test = new List<IPCB_PinPair>();
        PinPair = PinPairsManager.CreateFromPinsDescriptors("U66|U7");
        //PinPair.GetState_Length() if = -1 then broken.
        
        //PinPair.SetState_Name("Test2");
        test.Add(PinPair);
        //PinPairsManager.AssignPinPairClass(PinPair, "Temp");
        PinPairsManager.InvalidateAll();
        
        //Iterator = Board.BoardIterator_Create();
        //try
        //{
        //    //Filter for class objects
        //    Iterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
        //    Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);

        //    ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
        //    while (ObjectClass != null)
        //    {
        //        if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
        //        {
        //            Report.Add("xSignal Class : " + ObjectClass.GetState_DisplayName());

        //            //Loop through all xSignals of the board.
        //            for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
        //            {
        //                PinPair = PinPairsManager.GetState_PinPairs(I);

        //                if (ObjectClass.IsMember(PinPair.GetState_Name()))
        //                {
        //                    Report.Add(String.Format("    xSignal : {0}, Node Count : {1}, Signal Length : {2}mils, Routed Length : {3}mils, Unrouted Length : {4}mils, Primitive Count : {5}",
        //                                PinPair.GetState_Name(),
        //                                PinPair.GetState_NodeCount(),
        //                                EDP.Utils.CoordToMils((int)PinPair.GetState_Length()),
        //                                EDP.Utils.CoordToMils((int)PinPair.GetState_RoutedLength()),
        //                                EDP.Utils.CoordToMils((int)PinPair.GetState_UnroutedLength()),
        //                                PinPair.GetState_PrimitivesCount()));

        //                    //Loop through all the pins of the xSignal.
        //                    for (int j = 0; j <= PinPair.GetState_PrimitivesCount() - 1; j++)
        //                    {
        //                        Prim = PinPair.GetPrimitives(j);
        //                        if (Prim.GetState_DescriptorString().StartsWith("Pad")) //need refdes, pin number, net name
        //                        {
        //                            Pad = (IPCB_Pad)Prim; //Pad.GetState_PinDescriptorString() = "U6-C17"
        //                            Report.Add("        Pin: " + Pad.GetState_PinDescriptorString() + ", Net: " + Prim.GetState_Net().GetState_Name());
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
        //    }

        //    //Write report file.
        //    File.WriteAllLines(Util.ProjPath() + "\\xSignals.txt", (string[])Report.ToArray(typeof(string)));
        //    //Open file.
        //    Client.ShowDocument(Client.OpenDocument("Text", Util.ProjPath() + "\\xSignals.txt"));
        //}
        ////Error catch if the file is open.
        //catch (IOException)
        //{
        //    Utils.ShowMessage("File in use. Please close the file and try again.");
        //}
        //catch (Exception ex)
        //{
        //    ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        //}
        //finally
        //{
        //    Board.BoardIterator_Destroy(ref Iterator);
        //}
    }
}

