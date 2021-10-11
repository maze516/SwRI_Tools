using NLog;
using PCB;
using System;

class ShowHideRefDes
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    /// <summary>
    /// Property used to enable/disable menu button.
    /// </summary>
    public bool Enabled
    {
        get
        {
            IPCB_Board Board = Util.GetCurrentPCB();
            if (Board == null)
                return false;
            return true;
        }
    }
    
    /// <summary>
    /// Will show or hide reference designators based on NameOn parameter.
    /// </summary>
    /// <param name="NameOn">true = show refdes', false = hide refdes'</param>
    public void ShowHide(IPCB_Board Board, bool NameOn,bool Discretes = false)
    {
        try
        {
            IPCB_BoardIterator BoardIterator;
            IPCB_Component Component;

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
                if (!Discretes)
                {
                    Component.BeginModify();
                    Component.SetState_NameOn(NameOn); //Show or hide refdes.
                    Component.EndModify();
                }
                else
                {
                    if (Component.GetState_Name().GetState_Text().StartsWith("R") || Component.GetState_Name().GetState_Text().StartsWith("C"))
                    {
                        Component.BeginModify();
                        Component.SetState_NameOn(NameOn); //Show or hide refdes.
                        Component.EndModify();
                    }
                }
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
            Board.GraphicalView_ZoomRedraw();
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
