using NLog;
using PCB;
using System;


class FixRefdesOrientation
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    /// <summary>
    /// Will adjust refdes orientation to match component orientation.
    /// </summary>
    public void FixRefDesOrientation()
    {
        try
        {
            IPCB_BoardIterator BoardIterator;
            IPCB_Component Component;
            IPCB_Text RefDes;

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
            //int cnt = 0;
            //Component = (IPCB_Component)BoardIterator.FirstPCBObject();

            //while (Component != null)
            //{
            //    cnt++;
            //    Component = (IPCB_Component)BoardIterator.NextPCBObject();
            //}

            Component = (IPCB_Component)BoardIterator.FirstPCBObject();
            //DXP.Utils.PercentInit("Updating RefDes", cnt);//Progressbar init.
            while (Component != null)
            {
                RefDes = Component.GetState_Name();
                if (Component.GetState_NameAutoPos() == TTextAutoposition.eAutoPos_CenterCenter)
                {
                    Component.SetState_NameAutoPos(TTextAutoposition.eAutoPos_Manual);
                    Component.BeginModify();
                    RefDes.BeginModify();
                    switch (Convert.ToInt32(Component.GetState_Rotation()))
                    {
                        case 0://for bottom: 90=270 
                            RefDes.SetState_Rotation(0);
                            break;
                        case 90:
                            if (RefDes.GetState_Layer() == TV6_Layer.eV6_BottomOverlay)
                                RefDes.SetState_Rotation(270);
                            else
                                RefDes.SetState_Rotation(90);
                            break;
                        case 180:
                            RefDes.SetState_Rotation(0);
                            break;
                        case 270:
                            if (RefDes.GetState_Layer() == TV6_Layer.eV6_BottomOverlay)
                                RefDes.SetState_Rotation(270);
                            else
                                RefDes.SetState_Rotation(90);
                            break;
                    }



                    //Component.SetState_NameAutoPos(TTextAutoposition.eAutoPos_CenterCenter);
                    Component.ChangeNameAutoposition(TTextAutoposition.eAutoPos_CenterCenter);
                    RefDes.EndModify();
                    RefDes.GraphicallyInvalidate();
                    Component.EndModify();
                    Component.GraphicallyInvalidate();
                }
                //DXP.Utils.PercentUpdate();
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }
            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);
            Board.GraphicalView_ZoomRedraw();
            //Board.GraphicallyInvalidate();
            //DXP.Utils.PercentFinish();
            System.Windows.Forms.MessageBox.Show("Process Complete");
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

