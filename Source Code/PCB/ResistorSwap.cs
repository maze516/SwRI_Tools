using PCB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;


class ResistorSwap
{

    public void SwapResistors()
    {
        //IPCB_Component Component; // component object
        IPCB_BoardIterator Iterator;
        IPCB_Primitive Item;
        IPCB_Board Board; // document board object
        IPCB_Net Net;
        Board = Util.GetCurrentPCB();
        if (Board == null)
            return;
        //Create board iterator
        Iterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eConnectionObject);
        Iterator.AddFilter_ObjectSet(FilterSet);
        Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        Iterator.AddFilter_Method(TIterationMethod.eProcessAll);


        Item = Iterator.FirstPCBObject();
        while (Item != null)
        {
            if (Item.GetState_Selected())
                Net = Item.GetState_Net();
            //Net.GetPrimitiveCount(new PCB.TObjectSet(new PCB.TObjectId[] { PCB.TObjectId.ePadObject }))
            //?Net.GetPrimitiveAt(1, TObjectId.ePadObject).GetState_DescriptorString()
            //"Pad U11-40(1557.72mil,2321.85mil) on Top Layer"
            //? Net.GetPrimitiveAt(2, TObjectId.ePadObject).GetState_DescriptorString()
            //"Pad R161-2(3000.004mil,2675mil) on Bottom Layer"
            Item = Iterator.NextPCBObject();
        }

    }

    // Find the point of intersection between
    // the lines p1 --> p2 and p3 --> p4.
    /// <summary>
    /// Determins if lines 1 and 2 cross.
    /// </summary>
    /// <param name="L1a">Line 1 coord a</param>
    /// <param name="L1b">Line 1 coord b</param>
    /// <param name="L2a">Line 2 coord a</param>
    /// <param name="L2b">Line 2 coord b</param>
    /// <returns></returns>
    private bool SegmentsIntersection(            PointF L1a, PointF L1b, PointF L2a, PointF L2b)
    {
        // Get the segments' parameters.
        float dx1 = L1b.X - L1a.X;
        float dy1 = L1b.Y - L1a.Y;
        float dx2 = L2b.X - L2a.X;
        float dy2 = L2b.Y - L2a.Y;

        // Solve for t1 and t2
        float denominator = (dy1 * dx2 - dx1 * dy2);

        float t1 = ((L1a.X - L2a.X) * dy2 + (L2a.Y - L1a.Y) * dx2)
                / denominator;
        if (float.IsInfinity(t1))
        {
            // The lines are parallel (or close enough to it).
            return false;
        }

        float t2 = ((L2a.X - L1a.X) * dy1 + (L1a.Y - L2a.Y) * dx1) / -denominator;

        // The segments intersect if t1 and t2 are between 0 and 1.
        return ((t1 >= 0) && (t1 <= 1) && (t2 >= 0) && (t2 <= 1));

    }

}

class NetData
{

}