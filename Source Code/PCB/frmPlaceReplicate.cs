using DXP;
using NLog;
using PCB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

//todo: add sch/pcb part selection when selecting stuff on the src/dst lists.

public partial class frmPlaceReplicate : ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    public const string PanelName = "PlaceReplicate";
    public const string PanelCaption = "Place Replicate";

    PlaceReplicate PR = new PlaceReplicate();
    public frmPlaceReplicate()
    {
        _Log.Debug("frmPlaceReplicate");


        InitializeComponent();

        UI.ApplyADUITheme(this);

    }

    private void btnSource_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnSource_Click");

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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnDest_Click(object sender, EventArgs e)
    {//TODO: allow adding dest from sch
        _Log.Debug("btnDest_Click");

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
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnMatch_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnMatch_Click");

        try
        {
            string selectedSource, selectedDest;
            DXP.Utils.PercentBeginComplexOperation("Adding matches.");
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

            DXP.Utils.PercentEndComplexOperation();
            DXP.Utils.PercentFinish();
        }
        catch (Exception ex)
        {
            if (ex.Message == "Exit Automatch")
                return;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private void btnReset_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnReset_Click");

        RemoveMatched();
    }

    private void btnPlace_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnPlace_Click");

        try
        {
            _Log.Debug("Place button");
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
                _Log.Debug("OffsetX: "+OffsetX+ ", OffsetY: " + OffsetY);
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
            _Log.Debug("arcObjects");
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
            _Log.Debug("padObjects");
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
            _Log.Debug("ViaObjects");
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
            _Log.Debug("trackObjects");
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
            _Log.Debug("textObjects");
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
            _Log.Debug("fillObjects");
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
            _Log.Debug("polygonObjects");
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
            _Log.Debug("primitiveObjects");
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
            _Log.Debug("regionObjects");
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



            string srcRef, dstRef,srcKey,dstKey;

            IPCB_Component srcComp, dstComp;

            foreach (string item in lstMatched.Items)
            {
                srcRef = item.Split('>')[0];
                dstRef = item.Split('>')[1];

                if (PR.Destination.Components.ContainsRefdes(dstRef, out dstKey) && PR.Source.Components.ContainsRefdes(srcRef, out srcKey))
                {
                    srcComp = PR.selectedSourceObjects.GetComponent(srcKey);
                    dstComp = PR.selectedDestinationObjects.GetComponent(dstKey);

                    dstComp.BeginModify();
                    if (srcComp.GetState_Layer() != dstComp.GetState_Layer())
                        dstComp.FlipComponent();
                    dstComp.SetState_Rotation(srcComp.GetState_Rotation());
                    dstComp.MoveToXY(OffsetX + srcComp.GetState_XLocation(), OffsetY + srcComp.GetState_YLocation());
                    //dstComp.Rebuild();
                    dstComp.EndModify();
                }
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
            _Log.Debug("placement done");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return;
        }
    }

    private Dictionary<string, string> GetNetDiff()
    {

        _Log.Debug(">GetNetDiff");
        string src, dst;
        string srcKey, dstKey;
        Dictionary<string, string> tempOut = new Dictionary<string, string>();
        foreach (string item in lstMatched.Items)
        {
            src = item.Split('>')[0];
            dst = item.Split('>')[1];

            if (PR.Source.Components.ContainsRefdes(src, out srcKey) && PR.Destination.Components.ContainsRefdes(dst, out dstKey))
                foreach (string pin in PR.Source.Components[srcKey].Nets.Keys)
                {
                    if (!tempOut.ContainsKey(PR.Source.Components[srcKey].Nets[pin]))
                        tempOut.Add(PR.Source.Components[srcKey].Nets[pin], PR.Destination.Components[dstKey].Nets[pin]);
                    else
                    {
                        if (tempOut[PR.Source.Components[srcKey].Nets[pin]] != PR.Destination.Components[dstKey].Nets[pin])
                            tempOut[PR.Source.Components[srcKey].Nets[pin]] = "dupe";
                    }
                }
        }
        _Log.Debug("GetNetDiff>");
        return tempOut;
    }

    private void RemoveMatched(string Match = "")
    {
        _Log.Debug("RemoveMatched");

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

    /// <summary>
    /// Set true when attempting the AutoMatch process.
    /// </summary>
    bool AutoMatching = false;
    /// <summary>
    /// Attempting to match components based on net patterns.
    /// </summary>
    bool SmartNetMatching = false;
    /// <summary>
    /// Try rerunning simple matching methods once more matches have already been made.
    /// </summary>
    bool SmartNetMatchRecursion = false;

    private void AddMatch(string Source, string Dest)
    {
        _Log.Debug("AddMatch");

        if (Matched(Source)) return;

        lstDest.Items.Remove(Dest);
        lstSource.Items.Remove(Source);

        lstMatched.Items.Add(Source + ">" + Dest);

        if (chkAutoMatch.Checked & !AutoMatching)
            AttemptAutoMatch(Dest, Source, lstSource.Items.OfType<string>().ToList<string>(), lstDest.Items.OfType<string>().ToList<string>());

    }
    private void lstMatched_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        _Log.Debug("lstMatched_MouseDoubleClick");

        RemoveMatched(lstMatched.SelectedItem.ToString());
    }

    private void lstDest_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        _Log.Debug("lstDest_MouseDoubleClick");

        string selectedSource, selectedDest;
        DXP.Utils.PercentBeginComplexOperation("Adding matches.");
        selectedSource = lstSource.SelectedItem.ToString();
        selectedDest = lstDest.SelectedItem.ToString();

        AddMatch(selectedSource, selectedDest);

        lstSource.Update();
        lstDest.Update();

        DXP.Utils.PercentEndComplexOperation();
        DXP.Utils.PercentFinish();
    }

    private void lstSource_MouseDoubleClick(object sender, MouseEventArgs e)
    {
        _Log.Debug("lstSource_MouseDoubleClick");

        string selectedSource, selectedDest;
        DXP.Utils.PercentBeginComplexOperation("Adding matches.");
        selectedSource = lstSource.SelectedItem.ToString();
        selectedDest = lstDest.SelectedItem.ToString();

        AddMatch(selectedSource, selectedDest);

        lstSource.Update();
        lstDest.Update();

        DXP.Utils.PercentEndComplexOperation();
        DXP.Utils.PercentFinish();
    }

    private void lstSource_MouseClick(object sender, MouseEventArgs e)
    {
        _Log.Debug("lstSource_MouseClick");

        //SelectCmpt();
    }

    private void lstDest_MouseClick(object sender, MouseEventArgs e)
    {
        _Log.Debug("lstDest_MouseClick");

        //SelectCmpt();
    }


    string prevSelectedSource, prevSelectedDest;
    void SelectCmpt()
    {
        _Log.Debug("SelectCmpt");

        string selectedSource, selectedDest;
        IPCB_Component cmptSource, cmptDest;

        IPCB_Board brd = PR.selectedSourceObjects.componentObjects[0].GetState_Board();

        //Util.DeselectBoard(brd);
        //deselect previouse part
        if (prevSelectedDest != "")
        {
            cmptDest = PR.selectedDestinationObjects.GetComponent(prevSelectedDest);
            if (cmptDest != null) cmptDest.SetState_Selected(false);
        }

        //deselect previouse part
        if (prevSelectedSource != "")
        {
            cmptSource = PR.selectedSourceObjects.GetComponent(prevSelectedSource);
            if (cmptSource != null) cmptSource.SetState_Selected(false);
        }

        //select new part
        if (lstDest.SelectedItem != null)
        {
            selectedDest = lstDest.SelectedItem.ToString();
            prevSelectedDest = selectedDest;
            cmptDest = PR.selectedDestinationObjects.GetComponent(selectedDest);
            if (cmptDest != null) cmptDest.SetState_Selected(true);
        }

        //select new part 
        if (lstSource.SelectedItem != null)
        {
            selectedSource = lstSource.SelectedItem.ToString();
            prevSelectedSource = selectedSource;
            cmptSource = PR.selectedSourceObjects.GetComponent(selectedSource);
            if (cmptSource != null) cmptSource.SetState_Selected(true);
        }


        brd.GraphicalView_ZoomRedraw();
        brd.GraphicallyInvalidate();
    }


    void AttemptAutoMatch(string Dest, string Source, List<string> SrcList, List<string> DstList)
    {
        _Log.Debug("AttemptAutoMatch");

        try
        {

            _Log.Debug(Dest);
            _Log.Debug(Source);

            AutoMatching = true;
            Dictionary<string, string> Matches = new Dictionary<string, string>();
            _Log.Debug("Match channels");
            #region Match Channels

            if (Source.Contains("U") & Source.Contains("_") & !Source.Contains("EM") & !Source.Contains("FM"))
            {
                foreach (string src in SrcList)
                    if (src.Contains("_"))
                        foreach (string dest in DstList)
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

            Matches = new Dictionary<string, string>();

            #region Getting Source component data

            string srcKey = null;
            st_IPCB_Component srcComp = null;
            string[] splitSource;

            if (Source.Contains(" "))
            {
                splitSource = Source.Split(' ');
                splitSource[1] = splitSource[1].Replace("(", "").Replace(")", "");

                foreach (string item in PR.Source.Components.GetRefDesKeys(splitSource[0]))
                {
                    srcComp = PR.Source.Components[item];
                    if (srcComp.RefDes == splitSource[0] && srcComp.Footprint == splitSource[1])
                    {
                        srcKey = srcComp.ID;
                        break;
                    }
                }
            }
            else
            {

                srcComp = PR.Source.Components.GetComponent(Source);//todo: check for dupes
                if (srcComp == null) return;
                srcKey = srcComp.ID;
            }
            //todo: what if srcComp still null?
            #endregion


            #region Getting Destination component data

            string dstKey = null;
            st_IPCB_Component dstComp = null;
            string[] splitDest;

            if (Dest.Contains(" "))
            {
                splitDest = Dest.Split(' ');
                splitDest[1] = splitDest[1].Replace("(", "").Replace(")", "");

                foreach (string item in PR.Destination.Components.GetRefDesKeys(splitDest[0]))
                {
                    dstComp = PR.Destination.Components[item];
                    if (dstComp.RefDes == splitDest[0] && dstComp.Footprint == splitDest[1])
                    {
                        dstKey = dstComp.ID;
                        break;
                    }
                }
            }
            else
            {
                dstComp = PR.Destination.Components.GetComponent(Dest);//todo: check for dupes
                dstKey = dstComp.ID;
            }
            //todo: what if dstComp still null?
            #endregion


            #region Add alternate footprints

            if (srcComp.Dupe)
            {
                foreach (string item in PR.Source.Components.GetRefDesKeys(srcComp.RefDes))
                {
                    if (!Matched(PR.Source.Components[item].RefDes + " (" + PR.Source.Components[item].Footprint + ")"))
                    {
                        string tmpSrc = PR.Source.Components[item].RefDes + " (" + PR.Source.Components[item].Footprint + ")";
                        string tmpDest = dstComp.RefDes + " (" + PR.Source.Components[item].Footprint + ")";
                        if (PR.Source.Components.GetComponent(dstComp.RefDes, PR.Source.Components[item].Footprint) != null)
                            AddMatch(tmpSrc, tmpDest);
                    }
                }
            }

            #endregion




            _Log.Debug("Match Part Numbers");
            #region Match Part Numbers

            string secondSrcKey = null;
            string secondDestKey = null;
            foreach (string src in SrcList)
            {
                if (PR.Source.Components.ContainsRefdes(src, out secondSrcKey))//todo: check for dupes
                    foreach (string dest in DstList)
                    {
                        if (PR.Destination.Components.ContainsRefdes(dest, out secondDestKey))//todo: check for dupes
                            if (src.StartsWith(dest.First().ToString()))
                                //if (PR.Source.Components[src].Footprint == PR.Destination.Components[dest].Footprint)
                                //{
                                if (PR.Source.Components[secondSrcKey].Parameters.ContainsKey("PartNumber") & PR.Destination.Components[secondDestKey].Parameters.ContainsKey("PartNumber"))
                                    if (PR.Source.Components[secondSrcKey].Parameters["PartNumber"] == PR.Destination.Components[secondDestKey].Parameters["PartNumber"])
                                        if (!Matched(src))
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

            _Log.Debug("Match Nets");
            #region Match Nets

            Matches = new Dictionary<string, string>();

            Dictionary<string, string> srcMatches = new Dictionary<string, string>();
            Dictionary<string, string> dstMatches = new Dictionary<string, string>();



            _Log.Debug("Src: " + Source + ", Dst: " + Dest);
            _Log.Debug("Source");

            //bool match = true;
            foreach (st_IPCB_Component srcSecond in PR.Source.Components.Values)
            {
                //match = true;
                _Log.Debug(srcSecond.RefDes);
                //if (Source.Contains(" "))
                //{
                //    if (Source != srcSecond.RefDes + " (" + srcSecond.Footprint + ")")
                //    {
                //        match = false;
                //    }
                //}
                //else
                //{
                //    if (srcSecond.RefDes != Source)
                //    {
                //        match = false;
                //    }
                //}

                if (srcComp.ID != srcSecond.ID)
                    foreach (KeyValuePair<string, string> firstNet in PR.Source.Components[srcKey].Nets)
                    {
                        foreach (KeyValuePair<string, string> secondNet in PR.Source.Components[srcSecond.ID].Nets)
                        {
                            if (firstNet.Value == secondNet.Value)
                            {

                                //if (!Matched(Source))
                                if (srcMatches.ContainsKey(Source + "," + firstNet.Key + "," + firstNet.Value))
                                    srcMatches[Source + "," + firstNet.Key + "," + firstNet.Value] = "multi";
                                else
                                {
                                    if (srcSecond.Dupe)
                                        srcMatches.Add(Source + "," + firstNet.Key + "," + firstNet.Value, srcSecond.RefDes + " (" + srcSecond.Footprint + ")");
                                    else
                                        srcMatches.Add(Source + "," + firstNet.Key + "," + firstNet.Value, srcSecond.RefDes);
                                }
                            }
                        }
                    }
            }


            _Log.Debug("Dest");
            //string[] secondDestName;
            foreach (st_IPCB_Component destSecond in PR.Destination.Components.Values)
            //foreach (string destSecond in DstList)
            {
                _Log.Debug(destSecond);
                if (dstComp.ID != destSecond.ID)
                    foreach (KeyValuePair<string, string> firstNet in PR.Destination.Components[dstKey].Nets)//Dest
                    {
                        foreach (KeyValuePair<string, string> secondNet in PR.Destination.Components[destSecond.ID].Nets)//secondDest
                        {
                            if (firstNet.Value == secondNet.Value)
                            {
                                //if (!Matched(Dest))
                                if (dstMatches.ContainsKey(Dest + "," + firstNet.Key + "," + firstNet.Value))
                                    dstMatches[Dest + "," + firstNet.Key + "," + firstNet.Value] = "multi";
                                else
                                {
                                    if (destSecond.Dupe)
                                        dstMatches.Add(Dest + "," + firstNet.Key + "," + firstNet.Value, destSecond.RefDes + " (" + destSecond.Footprint + ")");
                                    else
                                        dstMatches.Add(Dest + "," + firstNet.Key + "," + firstNet.Value, destSecond.RefDes);
                                }
                            }
                        }
                    }
            }

            _Log.Debug("Matches");
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

            #endregion

            AutoMatching = false;
            _Log.Debug("Smart Net Matching");
            #region Smart Net Matching

            Matches = new Dictionary<string, string>();
            Dictionary<string, string> SNets = PR.Source.Components[srcKey].Nets;
            Dictionary<string, string> DNets = PR.Destination.Components[dstKey].Nets;

            if (SNets.Count == DNets.Count)
                foreach (KeyValuePair<string, string> srcItem in SNets)
                {

                    List<string> lstSrc = new List<string>();
                    List<string> lstDst = new List<string>();

                    foreach (structNet item2 in PR.SourceNets[srcItem.Value])
                        if (PR.Source.Components.DuplicateRef(item2.RefDes))
                            foreach (string srcKeys in PR.Source.Components.GetRefDesKeys(item2.RefDes))
                            {
                                string tmpSrc = PR.Source.Components[srcKeys].RefDes + " (" + PR.Source.Components[srcKeys].Footprint + ")";
                                lstSrc.Add(tmpSrc);
                            }
                        else
                            lstSrc.Add(item2.RefDes);

                    foreach (structNet item3 in PR.DestNets[DNets[srcItem.Key]])
                        if (PR.Destination.Components.DuplicateRef(item3.RefDes))
                            foreach (string dstKeys in PR.Destination.Components.GetRefDesKeys(item3.RefDes))
                            {
                                string tmpSrc = PR.Destination.Components[dstKeys].RefDes + " (" + PR.Destination.Components[dstKeys].Footprint + ")";
                                lstDst.Add(tmpSrc);
                            }
                        else
                            lstDst.Add(item3.RefDes);

                    #region Only one other component on net
                    if (PR.SourceNets[srcItem.Value].Count == 2)
                    {
                        _Log.Debug("Only one other component on net");
                        foreach (structNet item2 in PR.SourceNets[srcItem.Value])
                        {
                            if (!Matched(item2.RefDes))
                            {
                                if (!item2.RefDes.Contains(' ') && !PR.Source.Components.DuplicateRef(item2.RefDes))
                                    foreach (structNet item3 in PR.DestNets[DNets[srcItem.Key]])
                                    {
                                        if (item3.Pin == item2.Pin)
                                        {
                                            //if (!Matched(item2.RefDes))
                                            if (Matches.ContainsKey(item2.RefDes))
                                                Matches[item2.RefDes] = "multi";
                                            else
                                                Matches.Add(item2.RefDes, item3.RefDes);
                                        }
                                    }
                            }
                        }
                    }
                    #endregion

                    #region Only one component unmatched
                    else if (Unmatched(lstSrc) == 1)
                    {
                        _Log.Debug("Only one component unmatched");
                        string Ref1 = "", Ref2 = "";
                        foreach (string item in lstSrc)
                            if (!Matched(item)) { Ref1 = item; break; }

                        foreach (string item in lstDst)
                            if (!Matched(item)) { Ref2 = item; break; }

                        if (Ref1 != "" && Ref2 != "")
                            if (!PR.Source.Components.DuplicateRef(Ref1))
                                if (!PR.Destination.Components.DuplicateRef(Ref2))
                                    if (!Matches.ContainsKey(Ref1))
                                        Matches.Add(Ref1, Ref2);

                    }
                    #endregion
                    else
                    {
                        if (!SmartNetMatchRecursion)
                        {
                            SmartNetMatchRecursion = true;
                            _Log.Info("Smart Net AttemptAutoMatch: " + Dest + ", " + Source + ", " + lstSrc + ", " + lstDst);
                            AttemptAutoMatch(Dest, Source, lstSrc, lstDst);
                            SmartNetMatchRecursion = false;
                        }
                    }

                    foreach (KeyValuePair<string, string> item in Matches)
                    {
                        if (item.Value != "multi")
                            AddMatch(item.Key, item.Value);
                    }
                    #endregion
                    _Log.Debug("Only one of each refdes type (R,C,U ...)");
                    #region Only one of each refdes type (R,C,U ...)
                    if (!Source.Contains(' '))
                    {
                        Matches = new Dictionary<string, string>();
                        Dictionary<string, string> SourceCount = new Dictionary<string, string>();
                        Dictionary<string, string> DestCount = new Dictionary<string, string>();
                        var digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                        foreach (structNet structNet in PR.SourceNets[srcItem.Value])
                        {
                            if (structNet.RefDes != Source) // && structNet.RefDes != Dest)
                            {
                                string refType = structNet.RefDes.TrimEnd(digits);
                                if (!Matched(structNet.RefDes))
                                    if (!SourceCount.ContainsKey(refType))
                                        SourceCount.Add(refType, structNet.RefDes);
                                    else
                                        SourceCount[refType] = "multi";
                            }
                        }

                        foreach (structNet structNet in PR.DestNets[DNets[srcItem.Key]])
                        {
                            if (structNet.RefDes != Dest) // && structNet.RefDes != Dest)
                            {
                                string refType = structNet.RefDes.TrimEnd(digits);
                                if (!Matched(structNet.RefDes))
                                    if (!DestCount.ContainsKey(refType))
                                        DestCount.Add(refType, structNet.RefDes);
                                    else
                                        DestCount[refType] = "multi";
                            }
                        }

                        foreach (KeyValuePair<string, string> cnt in SourceCount)
                        {
                            if (cnt.Value != "multi")
                            {
                                if (DestCount.Count > 0)
                                    if (!Matched(cnt.Value))
                                        if (!Matches.ContainsKey(cnt.Value))
                                            Matches.Add(cnt.Value, DestCount[cnt.Key]);
                                //AddMatch(cnt.Value, DestCount[cnt.Key]);
                            }
                        }

                        foreach (KeyValuePair<string, string> item in Matches)
                        {
                            if (item.Value != "multi")
                                if (!Matched(item.Key))
                                    AddMatch(item.Key, item.Value);
                        }
                    }
                    #endregion
                    _Log.Debug("Rerun Matched");
                    #region Rerun Matched
                    if (chkInDepth.Checked)
                        if (!SmartNetMatching)
                        {
                            SmartNetMatching = true;

                            lstSrc = new List<string>();
                            lstDst = new List<string>();

                            foreach (string item in lstMatched.Items)
                            {
                                lstSrc.Add(item.Split('>')[0]);
                                lstDst.Add(item.Split('>')[1]);
                            }

                            while (lstSrc.Count > 0)
                            {

                                AttemptAutoMatch(lstDst[0], lstSrc[0], lstSrc.GetRange(1, lstSrc.Count - 1), lstDst.GetRange(1, lstDst.Count - 1));//lstSource.Items.OfType<string>().ToList<string>(), lstDest.Items.OfType<string>().ToList<string>());//
                                lstSrc.RemoveAt(0);
                                lstDst.RemoveAt(0);
                            }
                            SmartNetMatching = false;
                        }
                    #endregion


                    AutoMatching = false;
                }
            //else
            //    throw new Exception("Net counts dont match");


        }
        catch (Exception ex)
        {
            if(ex.Message=="Exit Automatch")
                throw new Exit_AttempteAutomatch("Exit Automatch");
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            AutoMatching = false;
            SmartNetMatching = false;
            SmartNetMatchRecursion = false;
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            throw new Exit_AttempteAutomatch("Exit Automatch");
        }
    }

    int Unmatched(List<string> RefDes)
    {
        _Log.Debug("Unmatched");

        int output = 0;
        foreach (string match in lstMatched.Items)
        {
            foreach (string refdes in RefDes)
            {
                if (match.Contains(refdes + ">")) { output++; break; }
                else if (match.Contains(">" + refdes)) { output++; break; }
            }
        }
        return RefDes.Count - output;
    }

    private void chkAutoMatch_CheckedChanged(object sender, EventArgs e)
    {
        _Log.Debug("chkAutoMatch_CheckedChanged");

        chkInDepth.Enabled = chkAutoMatch.Checked;
    }


    private void btnFullReset_Click(object sender, EventArgs e)
    {
        _Log.Debug("btnFullReset_Click");

        PR = new PlaceReplicate();

        lstDest.Items.Clear();
        lstSource.Items.Clear();
        lstMatched.Items.Clear();

        AutoMatching = false;
        SmartNetMatching = false;
        SmartNetMatchRecursion = false;

    }

    /// <summary>
    /// Check if supplied refdes has already been matched.
    /// </summary>
    /// <param name="RefDes"></param>
    /// <returns>Refdes has been matched.</returns>
    bool Matched(string RefDes)
    {
        string[] matched;
        foreach (string item in lstMatched.Items)
        {
            matched = item.Split('>');
            if (matched[0] == RefDes) return true;
            //if (matched[0].StartsWith(RefDes + " (")) return true;
            if (matched[1] == RefDes) return true;
            //if (matched[1].StartsWith(RefDes + " (")) return true;
        }
        return false;
    }


}

//PCB.IPCB_DocumentPainterListenerHelper.OnEndDrag(this PCB.IPCB_DocumentPainterListener, PCB.IPCB_Primitive)
//PCB.IPCB_DocumentPainterListenerHelper.OnDragging(this PCB.IPCB_DocumentPainterListener, PCB.IPCB_Primitive, PCB.TDegreeOfFreedom, double)