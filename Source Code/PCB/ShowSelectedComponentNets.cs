using PCB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using NLog;

class ShowSelectedComponentNets
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public void NetConnect(bool Show)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);
        _Log.Trace("Show = " + Show.ToString());
#if DEBUG
        Stopwatch stopwatch;
        stopwatch = new Stopwatch();
        stopwatch.Start();
#endif

        IPCB_BoardIterator BoardIterator;

        IPCB_GroupIterator CompIterator;
        IPCB_Primitive CompItem;
        IPCB_Board Board = Util.GetCurrentPCB();

        IPCB_Component Comp;

        if (Board == null)
            return;


        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();

        //Filter.
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        FilterSet.Add(PCB.TObjectId.ePadObject);
        FilterSet.Add(PCB.TObjectId.eTrackObject);
        FilterSet.Add(PCB.TObjectId.eViaObject);

        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
        CompItem = BoardIterator.FirstPCBObject();
        //Comp = (IPCB_Component)BoardIterator.FirstPCBObject();

        while (CompItem != null)
        {
            if (CompItem.GetState_Selected())
            {
                if (CompItem.GetState_ObjectIDString() == "Pad" || CompItem.GetState_ObjectIDString() == "Track" || CompItem.GetState_ObjectIDString() == "Via")
                {
                    if (CompItem.GetState_InNet())
                        if (Show)
                            CompItem.GetState_Net().ShowNetConnects();
                        else
                            CompItem.GetState_Net().HideNetConnects();
                }
                else
                {
                    Comp = (IPCB_Component)CompItem;
                    CompIterator = Comp.GroupIterator_Create();
                    CompIterator.AddFilter_LayerSet_2(PCBConstant.V7AllLayersSet);
                    CompItem = CompIterator.FirstPCBObject();

                    while (CompItem != null)
                    {
                        if (CompItem.GetState_InNet())
                            if (Show)
                                CompItem.GetState_Net().ShowNetConnects();
                            else
                                CompItem.GetState_Net().HideNetConnects();
                        CompItem = CompIterator.NextPCBObject();
                    }
                }
            }
            CompItem = BoardIterator.NextPCBObject();
        }

        //CompItem.GetState_Net().SetState_LiveHighlightMode(TLiveHighlightMode.eLiveHighlightMode_High);


        ////Reset mask if applied.
        //string process = "PCB:RunQuery";
        //string parameters = "Clear=True";
        //DXP.Utils.RunCommand(process, parameters);

        //Board.AddObjectToHighlightObjectList(Component);

        //Board.SetState_Navigate_HighlightObjectList(new EDP.THighlightMethodSet(EDP.THighlightMethod.eHighlight_Filter), false);






        //DXP.Utils.RunCommand("PCB:Netlist", "Action=CleanUpNets");

#if DEBUG
        stopwatch.Stop();
        Debug.WriteLine(stopwatch.ElapsedMilliseconds);
#endif


    }
}

