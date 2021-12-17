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

    public void NetConnect()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

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

        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        FilterSet.Add(PCB.TObjectId.ePadObject);
        FilterSet.Add(PCB.TObjectId.eTrackObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
        CompItem = BoardIterator.FirstPCBObject();
        //Comp = (IPCB_Component)BoardIterator.FirstPCBObject();

        while (CompItem != null)
        {
            if (CompItem.GetState_Selected())
            {
                if (CompItem.GetState_ObjectIDString() == "Pad" || CompItem.GetState_ObjectIDString() == "Track")
                {
                    if (CompItem.GetState_InNet())
                        CompItem.GetState_Net().ShowNetConnects();
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
                            CompItem.GetState_Net().ShowNetConnects();
                        CompItem = CompIterator.NextPCBObject();
                    }
                }
            }
            CompItem = BoardIterator.NextPCBObject();
        }


#if DEBUG
        stopwatch.Stop();
        Debug.WriteLine(stopwatch.ElapsedMilliseconds);
#endif


    }
}

