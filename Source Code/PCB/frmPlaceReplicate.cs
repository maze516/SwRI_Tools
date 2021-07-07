using DXP;
using PCB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

//todo: add sch/pcb part selection when selecting stuff on the src/dst lists.

public partial class frmPlaceReplicate : ServerPanelForm
{
    public const string PanelName = "PlaceReplicate";
    public const string PanelCaption = "Place Replicate";

    PlaceReplicate PR = new PlaceReplicate();
    public frmPlaceReplicate()
    {

        InitializeComponent();

        UI.ApplyADUITheme(this);

    }

    private void btnSource_Click(object sender, EventArgs e)
    {
        try
        {
            PR.GetInitialParts();

            if (PR.Source.Components.Count == 0)
            {
                MessageBox.Show("Must select components.");
                return;
            }

            lstSource.Items.Clear();

            lstSource.Items.AddRange(PR.SelectedSourceRef.ToArray());

            lstSource.Sorted = true;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnDest_Click(object sender, EventArgs e)
    {//TODO: allow adding dest from sch
        try
        {
            PR.GetDestinationParts();

            if (PR.SelectedDestRef.Count == 0)
            {
                MessageBox.Show("Must select components.");
                return;
            }

            lstDest.Items.Clear();

            lstDest.Items.AddRange(PR.SelectedDestRef.ToArray());

            lstDest.Sorted = true;
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnMatch_Click(object sender, EventArgs e)
    {
        try
        {
            string selectedSource, selectedDest;

            if (lstSource.SelectedItem == null || lstDest.SelectedItem == null)
            {
                MessageBox.Show("Please select a source and destination component");
                return;

            }
            selectedSource = lstSource.SelectedItem.ToString();
            selectedDest = lstDest.SelectedItem.ToString();

            AddMatch(selectedSource, selectedDest);

            lstSource.Update();
            lstDest.Update();
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        RemoveMatched();
    }

    private void btnPlace_Click(object sender, EventArgs e)
    {
        try
        {
            clsSelectedObjects NewPlacement = new clsSelectedObjects();

            Dictionary<string, string> NetCompare = GetNetDiff();

            IPCB_Board brd = Util.GetCurrentPCB();
            brd.SelectedObjects_Clear();

            IPCB_Primitive temp;

            IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
            PCBServer.PreProcess();

            int OffsetX = 0, OffsetY = 0;
            if (!brd.ChooseLocation(ref OffsetX, ref OffsetY, "Select placement location"))
                return;
            try
            {

                OffsetX = OffsetX - PR.selectedSourceObjects.componentObjects[0].GetState_XLocation();
                OffsetY = OffsetY - PR.selectedSourceObjects.componentObjects[0].GetState_YLocation();
            }
            catch (Exception ex)
            {
                if (ex.HResult == -2147467259)
                {
                    MessageBox.Show("Source and destination data is corrupted due to board file being closed. Please restart the process.");
                    lstDest.Items.Clear();
                    lstSource.Items.Clear();
                    lstMatched.Items.Clear();
                    return;
                }
                throw;
            }

            PR.GetNets();

            brd.BeginModify();

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.arcObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                    }
                    else
                        //temp.SetState_Net(PCBServer.PCBObjectFactory(TObjectId.eNetObject, TDimensionKind.eNoDimension, TObjectCreationMode.eCreate_Default) as IPCB_Net);
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.arcObjects.Add(temp as IPCB_Arc);

            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.padObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);
                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.padObjects.Add(temp as IPCB_Pad);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.ViaObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {

                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.ViaObjects.Add(temp as IPCB_Via);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.trackObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {

                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.trackObjects.Add(temp as IPCB_Track);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.textObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.textObjects.Add(temp as IPCB_Text);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.fillObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.fillObjects.Add(temp as IPCB_Fill);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.polygonObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.polygonObjects.Add(temp as IPCB_Polygon);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.primitiveObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.primitiveObjects.Add(temp);
            }

            foreach (IPCB_Primitive item in PR.selectedSourceObjects.regionObjects)
            {
                temp = item.Replicate();
                brd.AddPCBObject(temp);
                temp.BeginModify();
                temp.MoveByXY(OffsetX, OffsetY);

                if (temp.GetState_Net() != null)
                    if (NetCompare.ContainsKey(temp.GetState_Net().GetState_Name()))
                    {
                        if (PR.BoardNets.ContainsKey(NetCompare[temp.GetState_Net().GetState_Name()]))
                            temp.SetState_Net(PR.BoardNets[NetCompare[temp.GetState_Net().GetState_Name()]]);
                        else
                            temp.SetState_Net(null);

                    }
                    else
                        temp.SetState_Net(null);
                temp.EndModify();
                NewPlacement.regionObjects.Add(temp as IPCB_Region);
            }

            string srcRef, dstRef;
            IPCB_Component srcComp, dstComp;

            foreach (string item in lstMatched.Items)
            {
                srcRef = item.Split('>')[0];
                dstRef = item.Split('>')[1];

                srcComp = PR.selectedSourceObjects.GetComponent(srcRef);
                dstComp = PR.selectedDestinationObjects.GetComponent(dstRef);

                dstComp.BeginModify();
                if (srcComp.GetState_Layer() != dstComp.GetState_Layer())
                    dstComp.FlipComponent();
                dstComp.SetState_Rotation(srcComp.GetState_Rotation());
                dstComp.MoveToXY(OffsetX + srcComp.GetState_XLocation(), OffsetY + srcComp.GetState_YLocation());
                //dstComp.Rebuild();
                dstComp.EndModify();
            }


            brd.EndModify();

            PCBServer.PostProcess();

            brd.GraphicallyInvalidate();
            //brd.Update_PCBGraphicalView(true, true);

            //DXP.Utils.RunCommand("PCB:Zoom", "Action=Redraw");

            //string process = "PCB:MoveObject";
            //string parameters = "Object= Selection";
            //DXP.Utils.RunCommand(process, parameters);
            RemoveMatched("Placed");
        }
        catch (Exception ex)
        {
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private Dictionary<string, string> GetNetDiff()
    {
        string src, dst;
        Dictionary<string, string> tempOut = new Dictionary<string, string>();
        foreach (string item in lstMatched.Items)
        {
            src = item.Split('>')[0];
            dst = item.Split('>')[1];
            if (PR.Source.Components.ContainsKey(src))
                foreach (string pin in PR.Source.Components[src].Nets.Keys)
                {
                    if (!tempOut.ContainsKey(PR.Source.Components[src].Nets[pin]))
                        tempOut.Add(PR.Source.Components[src].Nets[pin], PR.Destination.Components[dst].Nets[pin]);
                    else
                    {
                        if (tempOut[PR.Source.Components[src].Nets[pin]] != PR.Destination.Components[dst].Nets[pin])
                            tempOut[PR.Source.Components[src].Nets[pin]] = "dupe";
                    }
                }
        }
        return tempOut;
    }

    private void RemoveMatched(string Match = "")
    {
        string[] temp;
        if (Match == "")
        {
            foreach (string item in lstMatched.Items)
            {
                temp = item.Split('>');
                lstSource.Items.Add(temp[0]);
                lstDest.Items.Add(temp[1]);
            }
            lstMatched.Items.Clear();
        }
        else if (Match == "Placed")
        {
            foreach (string item in lstMatched.Items)
            {
                temp = item.Split('>');
                lstSource.Items.Add(temp[0]);
            }
            lstMatched.Items.Clear();
        }
        else
        {
            temp = Match.Split('>');
            lstSource.Items.Add(temp[0]);
            lstDest.Items.Add(temp[1]);
            lstMatched.Items.Remove(Match);
        }
    }

    bool matching = false;
    private void AddMatch(string Source, string Dest)
    {
        lstDest.Items.Remove(Dest);
        lstSource.Items.Remove(Source);

        lstMatched.Items.Add(Source + ">" + Dest);

        if (chkAutoMatch.Checked & !matching)
            AttemptAutoMatch(Dest, Source);

    }
    private void lstMatched_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        RemoveMatched(lstMatched.SelectedItem.ToString());
    }

    private void lstDest_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        string selectedSource, selectedDest;

        selectedSource = lstSource.SelectedItem.ToString();
        selectedDest = lstDest.SelectedItem.ToString();

        AddMatch(selectedSource, selectedDest);
    }

    private void lstSource_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        string selectedSource, selectedDest;

        selectedSource = lstSource.SelectedItem.ToString();
        selectedDest = lstDest.SelectedItem.ToString();

        AddMatch(selectedSource, selectedDest);
    }



    void AttemptAutoMatch(string Dest, string Source)
    {
        try
        {


            matching = true;
            Dictionary<string, string> Matches = new Dictionary<string, string>();

            #region Match Channels

            if (Source.Contains("U") & Source.Contains("_") & !Source.Contains("EM") & !Source.Contains("FM"))
            {
                foreach (string src in lstSource.Items)
                    if (src.Contains("_"))
                        foreach (string dest in lstDest.Items)
                            if (src.Split('_').Length > 1 && dest.Split('_').Length > 1)
                                if (Dest.Split('_').Length > 1)
                                    if (src.Split('_')[0] == dest.Split('_')[0])
                                        if (dest.Split('_')[1] == Dest.Split('_')[1])
                                            Matches.Add(src, dest);



            }

            foreach (KeyValuePair<string, string> item in Matches)
            {
                AddMatch(item.Key, item.Value);
            }

            #endregion

            #region Match Part Numbers

            Matches = new Dictionary<string, string>();

            foreach (string src in lstSource.Items)
            {

                foreach (string dest in lstDest.Items)
                {
                    if (src.StartsWith(dest.First().ToString()))
                        //if (PR.Source.Components[src].Footprint == PR.Destination.Components[dest].Footprint)
                        //{
                        if (PR.Source.Components[src].Parameters.ContainsKey("PartNumber") & PR.Destination.Components[dest].Parameters.ContainsKey("PartNumber"))
                            if (PR.Source.Components[src].Parameters["PartNumber"] == PR.Destination.Components[dest].Parameters["PartNumber"])
                                if (Matches.ContainsKey(src))
                                    Matches[src] = "multi";
                                else
                                    Matches.Add(src, dest);
                    //}
                    //else
                    //{
                    //    if (Matches.ContainsKey(src))
                    //        Matches[src] = "multi";
                    //    else
                    //        Matches.Add(src, dest);
                    //}
                }
            }

            foreach (KeyValuePair<string, string> item in Matches)
            {
                if (item.Value != "multi")
                    AddMatch(item.Key, item.Value);
            }

            #endregion

            #region Match Nets

            Matches = new Dictionary<string, string>();

            Dictionary<string, string> srcMatches = new Dictionary<string, string>();
            Dictionary<string, string> dstMatches = new Dictionary<string, string>();




            foreach (KeyValuePair<string, clsOutput.st_IPCB_Component> srcSecond in PR.Source.Components)
            {
                if (srcSecond.Value.RefDes != Source)
                    foreach (KeyValuePair<string, string> firstNet in PR.Source.Components[Source].Nets)
                    {
                        foreach (KeyValuePair<string, string> secondNet in PR.Source.Components[srcSecond.Key].Nets)
                        {
                            if (firstNet.Value == secondNet.Value)
                            {
                                if (srcMatches.ContainsKey(Source + "," + firstNet.Key + "," + firstNet.Value))
                                    srcMatches[Source + "," + firstNet.Key + "," + firstNet.Value] = "multi";
                                else
                                    srcMatches.Add(Source + "," + firstNet.Key + "," + firstNet.Value, srcSecond.Value.RefDes);
                            }
                        }
                    }
            }



            foreach (string destSecond in lstDest.Items)
            {
                if (destSecond != Dest)
                    foreach (KeyValuePair<string, string> firstNet in PR.Destination.Components[Dest].Nets)
                    {
                        foreach (KeyValuePair<string, string> secondNet in PR.Destination.Components[destSecond].Nets)
                        {
                            if (firstNet.Value == secondNet.Value)
                            {
                                if (dstMatches.ContainsKey(Dest + "," + firstNet.Key + "," + firstNet.Value))
                                    dstMatches[Dest + "," + firstNet.Key + "," + firstNet.Value] = "multi";
                                else
                                    dstMatches.Add(Dest + "," + firstNet.Key + "," + firstNet.Value, destSecond);
                            }
                        }
                    }
            }

            foreach (KeyValuePair<string, string> dstItem in dstMatches)
            {
                if (dstItem.Value != "multi")// || dstItem.Value.Contains("U"))
                {
                    foreach (KeyValuePair<string, string> srcItem in srcMatches)
                    {
                        if (dstItem.Key.Split(',')[1] == srcItem.Key.Split(',')[1])
                            if (srcItem.Value != "multi")
                                AddMatch(srcItem.Value, dstItem.Value);
                    }
                }
            }

            //foreach (KeyValuePair<string, string> item in Matches)
            //{
            //    if (item.Value != "multi")
            //        AddMatch(item.Key, item.Value);
            //}
            #endregion

            matching = false;
        }
        catch (Exception ex)
        {
            matching = false;
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

}

//PCB.IPCB_DocumentPainterListenerHelper.OnEndDrag(this PCB.IPCB_DocumentPainterListener, PCB.IPCB_Primitive)
//PCB.IPCB_DocumentPainterListenerHelper.OnDragging(this PCB.IPCB_DocumentPainterListener, PCB.IPCB_Primitive, PCB.TDegreeOfFreedom, double)