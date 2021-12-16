using NLog;
using PCB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// Aligns variant footprint to the base design footprint
/// </summary>
public class AlignVariants
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    List<string> SelectedRef;
    List<IPCB_Component> SelectedComp;
    public AlignVariants()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        SelectedRef = new List<string>();
        SelectedComp = new List<IPCB_Component>();
    }
    public void AlignSelectedVariants()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        GetSelectedComponents();
        AlignSelected();
    }
    /// <summary>
    /// Compiles a list of selected components.
    /// </summary>
    void GetSelectedComponents()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IPCB_BoardIterator BoardIterator;
        IPCB_Component Component;

        string RefDes;

        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        Component = (IPCB_Component)BoardIterator.FirstPCBObject();

        while (Component != null)
        {
            RefDes = Component.GetState_Name().GetState_Text();
            if (Component.GetState_SourceUniqueId() != null)
                if (Component.GetState_SourceUniqueId().Contains("@") || RefDes.Contains("EM") || RefDes.Contains("FM")) //Verify selected component is a variant.
                {
                    RefDes = Component.GetState_Name().GetState_Text();
                    if (Component.GetState_Selected())
                    { //Add to lists.
                        SelectedRef.Add(RefDes);
                        SelectedComp.Add(Component);
                    }
                }

            Component = (IPCB_Component)BoardIterator.NextPCBObject();
        }
        //Iterator clean-up
        Board.BoardIterator_Destroy(ref BoardIterator);

    }

    /// <summary>
    /// Aligns variant component to base component.
    /// </summary>
    void AlignSelected()
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        IPCB_BoardIterator BoardIterator;
        IPCB_Component Component;

        string RefDes;

        IPCB_Board Board = Util.GetCurrentPCB();

        if (Board == null)
            return;

        //Iterate theough all components on the board.
        BoardIterator = Board.BoardIterator_Create();
        PCB.TObjectSet FilterSet = new PCB.TObjectSet();
        //Filter for components only.
        FilterSet.Add(PCB.TObjectId.eComponentObject);
        BoardIterator.AddFilter_ObjectSet(FilterSet);
        BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet); //Filter all layers.
        BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);

        Component = (IPCB_Component)BoardIterator.FirstPCBObject();
        Board.BeginModify();

        while (Component != null)
        {
            RefDes = Component.GetState_Name().GetState_Text();
            if (Component.GetState_SourceUniqueId() != null)
                if (!Component.GetState_SourceUniqueId().Contains("@") || RefDes.Contains("EM") || RefDes.Contains("FM")) //Verify component is not a variant.
                {
                    if (RefDes.Contains("EM"))
                        RefDes = RefDes.Replace("EM", "FM");
                    else if (RefDes.Contains("FM"))
                        RefDes = RefDes.Replace("FM", "EM");

                    if (SelectedRef.Contains(RefDes))
                    {
                        foreach (IPCB_Component item in SelectedComp)
                        {
                            if (item.GetState_Name().GetState_Text() == RefDes) //Match component 
                            {
                                //Copy position, laye and rotation settings from base to variant.
                                item.SetState_Layer(Component.GetState_Layer());
                                item.SetState_XLocation(Component.GetState_XLocation());
                                item.SetState_YLocation(Component.GetState_YLocation());
                                item.SetState_Rotation(Component.GetState_Rotation());

                                Board.SetState_DocumentHasChanged();
                                break;
                            }
                        }
                    }
                }


            Component = (IPCB_Component)BoardIterator.NextPCBObject();
        }
        Board.EndModify();

        Board.GraphicalView_ZoomRedraw();
        Board.GraphicallyInvalidate();
        //Iterator clean-up
        Board.BoardIterator_Destroy(ref BoardIterator);
        MessageBox.Show("Process Complete");
    }
}

