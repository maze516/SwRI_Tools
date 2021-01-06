using PCB;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

class clFixEmbededResistors
{
    bool error = false;

    /// <summary>
    /// Checks mechanical layers 20 thru 24 for any objects. FixEmbededResistors Will move content to these layers which may interfere with current content or layer mapping.
    /// </summary>
    /// <returns>True: Layers are clear, False:Objects on one of the layers.</returns>
    public Boolean MechanicalClear()
    {
        try
        {
            //Checking Mechanical Layers

            //IPCB_Component Component; // component object
            IPCB_BoardIterator Iterator;
            IPCB_Primitive Item;
            IPCB_Board Board; // document board object

            Board = Util.GetCurrentPCB();
            if (Board == null)
                return false;
            //Create board iterator
            Iterator = Board.BoardIterator_Create();
            //Create filter set for all availible primitives.
            PCB.TObjectSet FilterSet = new PCB.TObjectSet(new PCB.TObjectId[] { PCB.TObjectId.eArcObject, PCB.TObjectId.eViaObject, PCB.TObjectId.eTrackObject, PCB.TObjectId.eTextObject, PCB.TObjectId.eFillObject, PCB.TObjectId.ePadObject, PCB.TObjectId.eComponentObject, PCB.TObjectId.eNetObject, PCB.TObjectId.ePolyObject, PCB.TObjectId.eDimensionObject, PCB.TObjectId.eCoordinateObject, PCB.TObjectId.eEmbeddedObject, PCB.TObjectId.eEmbeddedBoardObject, PCB.TObjectId.eFromToObject, PCB.TObjectId.eConnectionObject, PCB.TObjectId.eComponentBodyObject });
            Iterator.AddFilter_ObjectSet(FilterSet);
            //Filter for mechanical layers only.
            Iterator.AddFilter_LayerSet_2(PCBConstant.V7MechanicalLayersSet); //BoardIterator.AddFilter_LayerSet(PCBConstant.MechanicalLayersSet);
            Iterator.AddFilter_Method(TIterationMethod.eProcessAll);

            int x = 0;
            Item = Iterator.FirstPCBObject();
            while (Item != null) { x++; Item = Iterator.NextPCBObject(); }
            DXP.Utils.PercentInit("Checking Mechanical Layers", x);

            ///Step through all objects on mechanical layers
            ///to see if they are on layer 20 through 24.
            Item = Iterator.FirstPCBObject();
            while (Item != null)
            {
                for (ulong i = (ulong)ToolsPreferences.FirstResistorLayer + 1; i <= (ulong)(ToolsPreferences.FirstResistorLayer + ToolsPreferences.LayerCount); i++)
                {
                    if (Item.GetState_V7Layer().ID == V7_Layer.MechanicalLayer(i).ID) //Compare item layer to MechanicalLayer i.
                    {
                        return false;
                    }
                }
                Item = Iterator.NextPCBObject();
                DXP.Utils.PercentUpdate();
            }

            DXP.Utils.PercentFinish();
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref Iterator);
            return true;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            error = true;
            return false;
        }
    }

    /// <summary>
    /// Moves the mechanical objects of a component on a midlayer to its respective layer.
    /// </summary>
    public void FixEmbededResistors()
    {
        try
        {
            List<IPCB_Component> EmbededResistors = new List<IPCB_Component>();
            List<TV6_Layer> UsedLayers = new List<TV6_Layer>();
            IPCB_Component Component; // component object
            IPCB_BoardIterator BoardIterator;
            IPCB_Board Board; // document board object
            SortedDictionary<string, int> LayerCounts = new SortedDictionary<string, int>();

            DXP.Utils.StatusBarSetState(2, "Checking Mechanical Layers");
            //Check to see if the mechanical layers we will be using are clear.
            DXP.Utils.StatusBarSetStateDefault();
            if (!MechanicalClear())
            {
                if (error) return;
                if (MessageBox.Show("There is information on Layers " + (ToolsPreferences.FirstResistorLayer + 1) + " to " + (ToolsPreferences.FirstResistorLayer + ToolsPreferences.LayerCount - 1) + ". New information may be added to these layers." + "\n" + "Do you wish to continue?", "Mechanical Layers Not Empyt", MessageBoxButtons.YesNo) == DialogResult.No) return;
            }
            if (error) return;

            Board = Util.GetCurrentPCB();
            if (Board == null)
                return;
            //Filter by components on mid signal layers
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            //Filter for components only
            FilterSet.Add(PCB.TObjectId.eComponentObject);
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6cMidLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

            //Iterate through all components looking for components on mid layers.
            Component = (IPCB_Component)BoardIterator.FirstPCBObject();
            while (Component != null)
            {
                if (UsedLayers.IndexOf(Component.GetState_Layer()) == -1)
                {
                    UsedLayers.Add(Component.GetState_Layer()); //Collect a non-duplicate list of mid layers with components
                    LayerCounts.Add(EDP.Utils.LayerToString(new V7_Layer(Component.GetState_Layer())), 1);
                }
                else
                    LayerCounts[EDP.Utils.LayerToString(new V7_Layer(Component.GetState_Layer()))]++;

                EmbededResistors.Add(Component); //Colect a list of all components on mid layers.
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }
            Board.BoardIterator_Destroy(ref BoardIterator);

            //No embeded resistors on this board.
            if (EmbededResistors.Count <= 0)
            {
                LayerReport(LayerCounts);
                return;
            }

            ///Sort the list of layers 
            ///First comp layer in list will be assigned to mech layer 19.
            ///Second comp layer will be assigned to mech layer 20 and so forth.
            UsedLayers.Sort();

            LayerCounts = LayerReport(LayerCounts);

            if (LayerCounts != null)
            {
                List<TV6_Layer> tmpUsedLayers = new List<TV6_Layer>(UsedLayers.Count);
                //Build a temp list of used layers.
                UsedLayers.ForEach((item) =>
                {
                    tmpUsedLayers.Add(item);
                });

                //Remove layers from the list that the user turned off.
                foreach (TV6_Layer item in tmpUsedLayers)
                {
                    if (!LayerCounts.ContainsKey(EDP.Utils.LayerToString(new V7_Layer(item))))
                    {
                        UsedLayers.Remove(item);
                    }
                }

                DXP.Utils.StatusBarSetState(2, "Updating Embeded Components");

                MoveMechLayers(EmbededResistors, UsedLayers, LayerCounts);

                DXP.Utils.StatusBarSetStateDefault();

                //Redraw board to refresh changes.
                Board.GraphicalView_ZoomRedraw();
                DXP.Utils.ShowInfo("Process complete.");
            }
            else
                DXP.Utils.ShowInfo("Process canceled.");
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
    }

    /// <summary>
    /// Displays a reort to the user of embeded components detected
    /// so they can enable/disable layers to process.
    /// </summary>
    /// <param name="LayerList">List of layers containing embeded components.</param>
    /// <returns>
    /// Updated list of layers with embeded components 
    /// that the user wants to run the process on.
    /// </returns>
    SortedDictionary<string, int> LayerReport(SortedDictionary<string, int> LayerList)
    {
            frmEmbededResistorRpt frmReport = new frmEmbededResistorRpt();
        if (LayerList.Count <= 0)
        {
            frmReport.Show();
            DXP.Utils.ShowInfo("No embeded components found.");
            frmReport.Close();
            return null;
        }
        frmReport.FillList(LayerList);
        DialogResult Result = frmReport.ShowDialog();
        if (Result == DialogResult.OK)
        {
            return frmReport.GetSelection();
        }
        else
            return null;
    }

    /// <summary>
    /// Moves the all mechanical objects in embeded components to their new layers.
    /// </summary>
    /// <param name="EmbededResistors">List of components to be processed.</param>
    /// <param name="UsedLayers">List of used layers</param>
    /// <param name="LayerCounts">
    /// List of layers used that has been updated by the user.
    /// Allows for the user to disable specific layers to be modified.
    /// </param>
    void MoveMechLayers(List<IPCB_Component> EmbededResistors, List<TV6_Layer> UsedLayers, SortedDictionary<string, int> LayerCounts)
    {
        
        DXP.Utils.PercentInit("Updating Embeded Components", EmbededResistors.Count);

        IPCB_GroupIterator CompIterator;
        IPCB_Primitive CompItem;
        IPCB_Component Component;

        //Step through each embeded component
        for (int i = 0; i <= EmbededResistors.Count - 1; i++)
        {
            Component = EmbededResistors[i];
            //Iterate through all objects of the current component.
            CompIterator = Component.GroupIterator_Create();
            CompIterator.AddFilter_LayerSet_2(PCBConstant.V7MechanicalLayersSet);
            CompItem = CompIterator.FirstPCBObject();

            while (CompItem != null)
            {
                //Check if object is on mech layer.
                if (new V7_Layer(CompItem.GetState_V7Layer()).IsMechanicalLayer() && LayerCounts.ContainsKey(EDP.Utils.LayerToString(new V7_Layer(Component.GetState_Layer()))))
                {
                    //Move object to its new respective mech layer.
                    CompItem.BeginModify();
                    CompItem.SetState_V7Layer(V7_Layer.MechanicalLayer((ulong)UsedLayers.IndexOf(Component.GetState_Layer()) + 19));
                    CompItem.EndModify();
                }
                CompItem = CompIterator.NextPCBObject();
            }
            DXP.Utils.PercentUpdate();
        }
        DXP.Utils.PercentFinish();
    }
}
