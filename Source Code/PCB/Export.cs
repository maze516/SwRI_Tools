using DXP;
using NLog;
using PCB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

class Export
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    /// <summary>
    /// Will generate a human readable list of xSignals for the active board.
    /// </summary>
    public void xSignalReport()
    {
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        IPCB_Board Board;
        IPCB_BoardIterator Iterator;
        IPCB_ObjectClass2 ObjectClass;
        IPCB_PinPairsManager PinPairsManager;
        IPCB_PinPair PinPair;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        IPCB_Primitive Prim;
        IPCB_Pad Pad;


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

        PinPairsManager.InvalidateAll();


        Iterator = Board.BoardIterator_Create();
        try
        {
            //Filter for class objects
            Iterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
            Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);

            ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
            while (ObjectClass != null)
            {
                if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
                {
                    Report.Add("xSignal Class : " + ObjectClass.GetState_DisplayName());

                    //Loop through all xSignals of the board.
                    for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
                    {
                        PinPair = PinPairsManager.GetState_PinPairs(I);

                        if (ObjectClass.IsMember(PinPair.GetState_Name()))
                        {
                            Report.Add(String.Format("    xSignal : {0}, Node Count : {1}, Signal Length : {2}mils, Routed Length : {3}mils, Unrouted Length : {4}mils, Primitive Count : {5}",
                                        PinPair.GetState_Name(),
                                        PinPair.GetState_NodeCount(),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_Length()),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_RoutedLength()),
                                        EDP.Utils.CoordToMils((int)PinPair.GetState_UnroutedLength()),
                                        PinPair.GetState_PrimitivesCount()));

                            //Loop through all the pins of the xSignal.
                            for (int j = 0; j <= PinPair.GetState_PrimitivesCount() - 1; j++)
                            {
                                Prim = PinPair.GetPrimitives(j);
                                if (Prim.GetState_DescriptorString().StartsWith("Pad")) //need refdes, pin number, net name
                                {
                                    Pad = (IPCB_Pad)Prim; //Pad.GetState_PinDescriptorString() = "U6-C17"
                                    Report.Add("        Pin: " + Pad.GetState_PinDescriptorString() + ", Net: " + Prim.GetState_Net().GetState_Name());
                                }
                            }
                        }
                    }
                }

                ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
            }

            //Write report file.
            File.WriteAllLines(Util.ProjPath() + "\\xSignals.txt", (string[])Report.ToArray(typeof(string)));
            //File.WriteAllLines("C:\\xSignals.txt", (string[])Report.ToArray(typeof(string)));
            //Open file.
            Client.ShowDocument(Client.OpenDocument("Text", Util.ProjPath() + "\\xSignals.txt"));
        }
        //Error catch if the file is open.
        catch (IOException)
        {
            MessageBox.Show("File in use. Please close the file and try again.");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref Iterator);
        }
    }



    /// <summary>
    /// Will generate a human readable list of net class' for the active board.
    /// </summary>
    public void NetClassReport()
    {

        int i;
        IPCB_Board Board;
        IPCB_BoardIterator ClassIterator;
        IPCB_ObjectClass NetClass;
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;

        Board = PCBServer.GetCurrentPCBBoard();

        ClassIterator = Board.BoardIterator_Create();

        try
        {
            //Filter for class objects
            ClassIterator.SetState_FilterAll();
            ClassIterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
            NetClass = (IPCB_ObjectClass)ClassIterator.FirstPCBObject();
            while (NetClass != null)
            {
                i = 0;
                if (NetClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Net)
                {
                    Report.Add("NetClass Name: " + NetClass.GetState_Name());
                    while (NetClass.GetState_MemberName(i) != null)
                    {
                        Report.Add("    Member: " + NetClass.GetState_MemberName(i));
                        i++;
                    }

                }
                NetClass = (IPCB_ObjectClass)ClassIterator.NextPCBObject();
            }
            File.WriteAllLines(Util.ProjPath() + "\\NetClass.txt", (string[])Report.ToArray(typeof(string)));
            Client.ShowDocument(Client.OpenDocument("Text", Util.ProjPath() + "\\NetClass.txt"));
        }
        catch (IOException)
        {
            MessageBox.Show("File in use. Please close the file and try again.");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref ClassIterator); //Iterator clean-up
        }
    }

    /// <summary>
    /// Will generate a human readable list of rules for the active board.
    /// </summary>
    public void RuleReport()
    {

        IPCB_Board Board;
        IPCB_Rule Rule;
        IPCB_BoardIterator BoardIterator;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;

        Board = Util.GetCurrentPCB();
        if (Board == null)
            return;

        BoardIterator = Board.BoardIterator_Create();

        try
        {
            //Iterate through all rules
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            FilterSet.Add(PCB.TObjectId.eRuleObject); //Filter for rules only
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
            Rule = (IPCB_Rule)BoardIterator.FirstPCBObject();

            //Step through all rules
            while (Rule != null)
            {
                Report.Add("Rule : " + Rule.GetState_Name() +
                    ", Enabled: " + Rule.GetState_DRCEnabled() +
                    ", Desc : " + Rule.GetState_DescriptorString() +
                    ", Detail : " + Rule.GetState_DetailString());

                Rule = (IPCB_Rule)BoardIterator.NextPCBObject();
            }
            string OutputFile = Util.ProjPath() + "\\" + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "_" + DateTime.Today.ToString("MM-dd-yyyy") + "_Rules.txt";
            File.WriteAllLines(OutputFile, (string[])Report.ToArray(typeof(string)));
            Client.ShowDocument(Client.OpenDocument("Text", OutputFile));
        }
        //Catch error if file is open.
        catch (IOException)
        {
            MessageBox.Show("File in use. Please close the file and try again.");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
        }
    }

    /// <summary>
    /// Will generate a Do file for rules for the active board.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList RuleDoReport()
    {
        IPCB_Board Board;
        IPCB_Rule Rule;
        IPCB_BoardIterator BoardIterator;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        string Output;

        Board = Util.GetCurrentPCB();
        if (Board == null)
            return null;
        BoardIterator = Board.BoardIterator_Create();

        try
        {

            //Iterate through all rules
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            FilterSet.Add(PCB.TObjectId.eRuleObject); //Filter for rules only
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
            Rule = (IPCB_Rule)BoardIterator.FirstPCBObject();
            //Step through all rules.
            while (Rule != null)
            {
                Output = DecodeRule(Rule, Board);//Decodes rule into Do file command based on rule type.
                if (Output != "")
                    Report.Add(Output);

                Rule = (IPCB_Rule)BoardIterator.NextPCBObject();
            }




            //Placeholder for missed rules that will be
            //removed from the report and added to the end.
            ArrayList TempArray = new ArrayList();
            foreach (string item in Report)
            {
                if (item.Contains("Missed Rule"))
                    TempArray.Add(item);

            }
            //Removing "Missing Rules" from the report.
            foreach (string item in TempArray)
            {
                Report.Remove(item);
            }
            //Adding "Missing Rules" to the bottom of the report.
            Report.AddRange(TempArray);

            return Report;
            //Generate report.
            //File.WriteAllLines(Util.ProjPath() + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "-Rules.do", (string[])Report.ToArray(typeof(string)));
            //Client.ShowDocument(Client.OpenDocument("Text", Util.ProjPath() + "\\" + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "-Rules.do"));
        }
        //Catch error if file is open.
        catch (IOException)
        {
            MessageBox.Show("File in use. Please close the file and try again.");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
        }
        return null;
    }

    /// <summary>
    /// Will generate a DO file of xSignals for the active board.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList xSignalDoReport()//Const View : IServerDocumentView; Var Parameters : WideString)
    {
        Dictionary<string, List<string>> PrimList = new Dictionary<string, List<string>>();
        string output;
        IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;
        IPCB_Board Board;
        IPCB_BoardIterator Iterator;
        IPCB_ObjectClass2 ObjectClass;
        IPCB_PinPairsManager PinPairsManager;
        IPCB_PinPair PinPair;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        IPCB_Primitive Prim;
        IPCB_Pad Pad;
        bool Reported = false;

        if (PCBServer == null)
            return null;
        //Get current board
        Board = PCBServer.GetCurrentPCBBoard();
        if (Board == null)
            return null;
        //Get pinpair manager
        PinPairsManager = Board.GetState_PinPairsManager();
        if (PinPairsManager == null)
            return null;

        PinPairsManager.InvalidateAll();

        Iterator = Board.BoardIterator_Create();
        try
        {
            Iterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
            Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);

            //Geting xSignals
            ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
            while (ObjectClass != null)
            {
                if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
                {
                    //Collect xSignal pin pairs for group definitions.
                    if (ObjectClass.GetState_DisplayName() == "<All xSignals>")
                        for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
                        {

                            PinPair = PinPairsManager.GetState_PinPairs(I);
                            //Make sure the pinpair is apart of All xSignals class.
                            if (ObjectClass.IsMember(PinPair.GetState_Name()))
                            {
                                PrimList = new Dictionary<string, List<string>>();

                                //Iterate through all the primitives of the pinpair
                                for (int j = 0; j <= PinPair.GetState_PrimitivesCount() - 1; j++)
                                {
                                    Prim = PinPair.GetPrimitives(j);
                                    //Make sure the primitive is a pad.
                                    if (Prim.GetState_DescriptorString().StartsWith("Pad")) //need refdes, pin number, net name
                                    {
                                        Pad = (IPCB_Pad)Prim;
                                        //Get net data
                                        if (Prim.GetState_Net() != null)
                                            if (PrimList.ContainsKey(Prim.GetState_Net().GetState_Name()))
                                                PrimList[Prim.GetState_Net().GetState_Name()].Add(Pad.GetState_PinDescriptorString());
                                            else
                                            {
                                                PrimList.Add(Prim.GetState_Net().GetState_Name(), new List<string>());
                                                PrimList[Prim.GetState_Net().GetState_Name()].Add(Pad.GetState_PinDescriptorString());
                                            }
                                        //Notifies the user is an sXignal pad is missing a net. Should not happen narutally.
                                        else if (Reported == false)
                                        {
                                            MessageBox.Show("There is an xSignal without a net. xSignals without nets will not be outputed.");
                                            Reported = true;
                                        }
                                    }
                                }

                                //Generate report
                                //Do file format:
                                //define(group SERDESCLK0
                                //  (fromto R90 - 1 U10 - 46(net NetR90_1))
                                //  (fromto R90 - 2 U5 - AU7(net SERDESCLK0))
                                //)
                                Report.Add("define ( group " + PinPair.GetState_Name());
                                foreach (KeyValuePair<string, List<string>> item in PrimList)
                                {
                                    output = "  (fromto ";
                                    foreach (string item2 in item.Value)
                                    {
                                        output += item2 + " ";
                                    }
                                    output += "(net " + item.Key + " ))";
                                    Report.Add(output);
                                }
                                Report.Add(")");
                                Report.Add("");
                            }
                        }
                }
                ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
            }

            //Geting xsignal classes

            //Iterate through object classes
            ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
            PrimList = new Dictionary<string, List<string>>();
            while (ObjectClass != null)
            {
                if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
                {
                    //Looking for only xSignal classes
                    if (ObjectClass.GetState_DisplayName() != "<All xSignals>")
                    {
                        //Collect xSignal class data.
                        if (!PrimList.ContainsKey(ObjectClass.GetState_DisplayName()))
                            PrimList.Add(ObjectClass.GetState_DisplayName(), new List<string>());
                        //Getting xSignal names
                        for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
                        {
                            PinPair = PinPairsManager.GetState_PinPairs(I);

                            if (ObjectClass.IsMember(PinPair.GetState_Name()))
                            {
                                PrimList[ObjectClass.GetState_DisplayName()].Add(PinPair.GetState_Name());
                            }
                        }
                    }
                }

                ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
            }

            //Generate report
            //Do file format:
            //define(group_set ADC (add_group ADC_DI0_N ADC_DI0_P ADC_DI1_N ADC_DI1_P *define xsignal class
            //                ADC_DI2_N ADC_DI2_P ADC_DI3_N ADC_DI3_P
            //                ADC_DI4_N ADC_DI4_P ADC_DI5_N ADC_DI5_P
            //               )
            //)
            foreach (KeyValuePair<string, List<string>> item in PrimList)
            {
                Report.Add("define ( group_set " + item.Key + " (add_group ");
                foreach (string item2 in item.Value)
                {
                    Report.Add("    " + item2 + " ");
                }
                Report.Add("    )");
                Report.Add(")");
            }
            return Report;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return null;
        }
        finally
        {
            Board.BoardIterator_Destroy(ref Iterator);
        }
    }

    /// <summary>
    /// Creates a list of mid layer with layout directions
    /// in a DO file format.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList LayerDirectionDo()
    {
        IPCB_Board Board;
        Board = Util.GetCurrentPCB();
        if (Board == null)
            return null;

        ArrayList Output = new ArrayList();

        //Setup layer direction
        List<V7_Layer> LayerList;
        //List<string> Layers = new List<string>();
        string LayerName;
        List<KeyValuePair<string, string>> LayerUserNames = new List<KeyValuePair<string, string>>();

        try
        {

            LayerList = Util.GetV7SigLayers(Board);

            Output.Add("#********** LAYER DIRECTION **********");

            foreach (V7_Layer item in LayerList)
            {

                LayerName = EDP.Utils.LayerToString(item).Replace(" ", "");
                if (LayerName.Contains("MidLayer"))
                {
                    LayerUserNames.Add(new KeyValuePair<string, string>(LayerName, Util.GetLayerName(Board, item)));
                    //Layers.Add(LayerName);
                }
            }
            LayerUserNames.Sort(new KvpKeyComparer());
            //Layers.Sort(new StringListCompare());

            bool Direction = true;
            foreach (KeyValuePair<string, string> Layer in LayerUserNames)
            {
                if (Direction)
                {
                    Output.Add("#Layer Name: " + Layer.Value);
                    Output.Add("direction " + Layer.Key + " vertical");
                    Direction = !Direction;
                }
                else
                {
                    Output.Add("#Layer Name: " + Layer.Value);
                    Output.Add("direction " + Layer.Key + " horizontal");
                    Direction = !Direction;
                }
            }
            Output.Add("");

            //Select/unselect layers.
            //#********** SELECT LAYER  **********
            //#Prevents routing on Top and Bottom iterate through layers
            //select layer MidLayer1
            //select layer MidLayer2
            //select layer MidLayer3
            //select layer MidLayer4
            //unselect layer TopLayer
            //unselect layer BottomLayer
            return Output;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        return null;
    }

    /// <summary>
    /// Creates a list of mid layer select commands
    /// in a DO file format.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList LayerSelectDo()
    {
        IPCB_Board Board;
        Board = Util.GetCurrentPCB();
        if (Board == null)
            return null;

        ArrayList Output = new ArrayList();

        //Setup layer direction
        List<V7_Layer> LayerList;
        List<string> Layers = new List<string>();
        string LayerName;
        try
        {
            LayerList = Util.GetV7SigLayers(Board);

            foreach (V7_Layer item in LayerList)
            {
                LayerName = EDP.Utils.LayerToString(item).Replace(" ", "");
                if (LayerName.Contains("MidLayer"))
                    Layers.Add(LayerName);
            }

            Layers.Sort(new StringListCompare());

            Output.Add("#********** SELECT LAYER  **********");
            Output.Add("#Prevents routing on Top and Bottom iterate through layers");
            Output.Add("#Defaults");
            Output.Add("unselect all vias");
            Output.Add("select via Via0_TB_RoutingVias");
            Output.Add("#protect all wires");
            Output.Add("#protect all wires (type soft)");
            Output.Add("#Layers");
            Output.Add("unselect layer TopLayer");
            Output.Add("unselect layer BottomLayer");
            foreach (string Layer in Layers)
            {
                Output.Add("select layer " + Layer);
            }
            Output.Add("");

            return Output;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        return null;
    }

    /// <summary>
    /// Creates a Same net command
    /// in a DO file format.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList SameNetDo()
    {
        ArrayList Output = new ArrayList();

        //Adding constant rule to report.
        Output.Add("#********** DEFINE SAME NET CHECKING RULE **********");
        Output.Add("set same_net_checking on");
        Output.Add("");

        return Output;
    }

    /// <summary>
    /// Generates DO file command based on the rule type provided.
    /// </summary>
    /// <param name="Rule">Rule object to gather information from.</param>
    /// <param name="Board">Active board object.</param>
    /// <returns>DO file command.</returns>
    string DecodeRule(IPCB_Rule Rule, IPCB_Board Board)
    {
        try
        {
            string LayerName;
            string Output = "";
            Rule.Export_ToParameters_1(ref Output);
            Output = "";
            List<V7_Layer> LayerList;
            switch (Rule.GetState_RuleKind())
            {
                case TRuleKind.eRule_MatchedLengths: //Matched Length
                                                     //Extract the tolerance value.
                    string value = Regex.Match(Rule.GetState_DescriptorString(), "Tolerance=([0-9])+").Value.Replace("Tolerance=", "");
                    if (Rule.GetState_DescriptorString().Contains("InNetClass"))
                    {
                        //Extract the net names.
                        MatchCollection nets = Regex.Matches(Rule.GetState_DescriptorString(), @"'([^']*)'");
                        Output = "";
                        //Loop through each net and add to report.
                        //DO file format:
                        //circuit class DAC_DB (match_net_length on(tolerance 100))
                        if (nets != null)
                            foreach (Match item in nets)
                        {
                            Output += String.Format("circuit class {0} (match_net_length on (tolerance {1}))",
                                item.Value.Replace("'", ""), value);
                        }
                    }
                    if (Rule.GetState_DescriptorString().Contains("InxSignalClass"))
                    {
                        //Extract the net names.
                        MatchCollection nets = Regex.Matches(Rule.GetState_DescriptorString(), @"'([^']*)'");
                        Output = "";
                        //Loop through each net and add to report.
                        //DO file format:
                        //circuit group_set ADC(match_group_length on (tolerance 100))
                        if (nets != null)
                            foreach (Match item in nets)
                        {
                            Output += String.Format("circuit group_set {0} (match_group_length on (tolerance {1}))",
                                item.Value.Replace("'", ""), value);
                        }
                    }
                    break;
                case TRuleKind.eRule_MaxMinLength: //Length Contraint

                    //Extrac max value.
                    string MaxValue = Regex.Match(Rule.GetState_DescriptorString(), "Max=([0-9]*)").Value.Replace("Max=", "");
                    //Check for net specific rule.
                    if (Rule.GetState_DescriptorString().Contains("InNet"))
                    {
                        //Extract net names
                        //DO file format:
                        //circuit net DAC_DB0_N(length 4200(type actual))
                        MatchCollection nets = Regex.Matches(Rule.GetState_DescriptorString(), @"InNet\('([A-Z]+)");//.Value.Replace("'", "");
                        Output = "";
                        if(nets!=null)
                        foreach (Match item in nets)
                        {
                            Output += String.Format("circuit net {0} (length {1}(type actual))",
                               item.Value.Replace("'", ""), MaxValue);
                        }
                    }
                    //Check for xSignal class specific rule.
                    if (Rule.GetState_DescriptorString().Contains("InxSignalClass"))
                    {
                        //Extract xSignal class names
                        //DO file format:
                        //circuit group ADC_DI3_P(max_total_length 2200)
                        MatchCollection Xsignals = Regex.Matches(Rule.GetState_DescriptorString(), @"InxSignalClass\('([A-Z]+)");//.Value.Replace("'", "")
                        List<string> Result;
                        Output = "";
                        foreach (Match item in Xsignals)
                        {
                            Result = GetxSignalNets(item.Value.Replace("InxSignalClass('", ""), Board);
                            if (Result != null)
                                foreach (string item2 in Result)
                                {
                                    Output += String.Format("circuit group {0} (max_total_length {1})",
                                    item2, MaxValue);
                                }

                        }

                    }
                    //Check for xSignal specific rule.
                    if (Rule.GetState_DescriptorString().Contains("InxSignal"))
                    {
                        //what to do?
                    }
                    //  no option in altium (create DO for every xsignal in a class based on the xclass length rule)
                    //Max=([0-9]*)      Max=2000
                    //Min=([0-9]*)      Min=0
                    break;
                case TRuleKind.eRule_MaxMinWidth:

                    if (Rule.GetState_NetScope() != TNetScope.eNetScope_DifferentNetsOnly)
                        break;
                    //Only get the ALL rule.
                    if (Rule.GetState_Scope1Expression() == "All" && Rule.GetState_Scope2Expression() == "All")
                    {
                        //DO file format:
                        //rule layer layername (clerance 8)
                        LayerList = Util.GetV7SigLayers(Board);
                        IPCB_MaxMinWidthConstraint WidthRule = (IPCB_MaxMinWidthConstraint)Rule;
                        Output = "#********** PCB WIRE WIDTH RULE **********\n";
                        double Width;
                        foreach (V7_Layer item in LayerList)
                        {
                            Width = EDP.Utils.CoordToMils(WidthRule.GetState_FavoredWidth(item));
                            LayerName = EDP.Utils.LayerToString(item).Replace(" ", "");
                            Output += "rule layer " + LayerName + " (width " + Width + ")\n";
                        }
                    }
                    break;
                case TRuleKind.eRule_Clearance:
                    string mod = "";
                    if (Rule.GetState_NetScope() == TNetScope.eNetScope_DifferentNetsOnly)
                        mod = "";
                    else if (Rule.GetState_NetScope() == TNetScope.eNetScope_SameNetOnly)
                        mod = "_same_net";
                    //#********** PCB CLEARANCE RULES **********
                    //rule pcb (clearance 21(type area_area))
                    //rule pcb (clearance 10(type pin_area))                          //5mils eObjectClearanceID_Arc
                    //rule pcb (clearance 11(type pin_pin))                           //5mils eObjectClearanceID_Track (wire)
                    //rule pcb (clearance 12(type smd_area))                          //5mils eObjectClearanceID_SMDPad (smd)
                    //rule pcb (clearance 13(type smd_pin))                           //5mils eObjectClearanceID_THPad (pin)
                    //rule pcb (clearance 14(type smd_smd))                           //5mils eObjectClearanceID_Via (via)
                    //rule pcb (clearance 9(type via_area))                           //5mils eObjectClearanceID_Fill 
                    //rule pcb (clearance 6(type via_pin))                            //5mils eObjectClearanceID_Poly 
                    //rule pcb (clearance 7(type via_smd))                            //5mils eObjectClearanceID_Region
                    //rule pcb (clearance 8(type via_via))                            //5mils eObjectClearanceID_Text
                    //rule pcb (clearance 1(type wire_area))                          //5mils eObjectClearanceID_OutlineEdge
                    //rule pcb (clearance 2(type wire_pin))                           //5mils eObjectClearanceID_CutoutEdge
                    //rule pcb (clearance 3(type wire_smd))                           //5mils eObjectClearanceID_CavityEdge
                    //rule pcb (clearance 4(type wire_via))                           //5mils eObjectClearanceID_SplitBarrier
                    //rule pcb (clearance 5(type wire_wire))                          //5mils eObjectClearanceID_SplitContinuation
                    //rule pcb (clearance 15(type testpoint_area))
                    //rule pcb (clearance 16(type testpoint_pin))
                    //rule pcb (clearance 17(type testpoint_smd))
                    //rule pcb (clearance 18(type testpoint_via))
                    //rule pcb (clearance 19(type testpoint_wire))
                    //rule pcb (clearance 20(type testpoint_testpoint))

                    //Generate pcb based clearance rule.
                    //DO file format:
                    //rule pcb (clearance 10(type area_area))
                    IPCB_ClearanceConstraint ClearanceConstraint = (IPCB_ClearanceConstraint)Rule;
                    if (ClearanceConstraint.GetState_ScopeDescriptorString() == "(All),(All)")
                    {

                        Output = "#********** PCB CLEARANCE RULES **********\n";
                        if (mod == "") Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetState_Gap()) + ")\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_THPad, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type pin_pin" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_SMDPad, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type smd_pin" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_SMDPad, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type smd_smd" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type via_pin" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type via_smd" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_Via)) + "(type via_via" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type wire_pin" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type wire_smd" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_Via)) + "(type wire_via" + mod + "))\n";
                        Output += "rule pcb (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_Track)) + "(type wire_wire" + mod + "))\n";
                    }
                    //Generate layer based clearance rule.
                    //DO file format:
                    //rule layer TopLayer (clearance 10(type area_area))
                    if (ClearanceConstraint.GetState_ScopeDescriptorString().Contains("OnLayer('"))
                    {
                        string Layer = Regex.Match(ClearanceConstraint.GetState_ScopeDescriptorString(), @"'([^']*)'").Value.Replace("'", "").Replace(" ", "");
                        Output = "#********** PCB " + Layer + " CLEARANCE RULES **********\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_THPad, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type pin_pin" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_SMDPad, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type smd_pin" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_SMDPad, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type smd_smd" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type via_pin" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type via_smd" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Via, TObjectClearanceId.eObjectClearanceID_Via)) + "(type via_via" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_THPad)) + "(type wire_pin" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_SMDPad)) + "(type wire_smd" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_Via)) + "(type wire_via" + mod + "))\n";
                        Output += "rule layer " + Layer + " (clearance " + EDP.Utils.CoordToMils(ClearanceConstraint.GetClearance_1(TObjectClearanceId.eObjectClearanceID_Track, TObjectClearanceId.eObjectClearanceID_Track)) + "(type wire_wire" + mod + "))\n";
                    }

                    break;
                case TRuleKind.eRule_DifferentialPairsRouting:
                    if (Rule.GetState_DescriptorString().Contains("(All)"))
                    {
                        LayerList = Util.GetV7SigLayers(Board);
                        IPCB_DifferentialPairsRoutingRule differentialPairsRoutingRule = Rule as IPCB_DifferentialPairsRoutingRule;
                        string DiffSettings = "";

                        //Generate max uncoupled length with contant data
                        //DO file format:
                        //#********** DIFF PAIR RULES **********
                        //rule pcb (edge_primary_gap 6)
                        //set average_pair_length off
                        Rule.Export_ToParameters(ref DiffSettings);
                        Output = "\n#********** DIFF PAIR RULES **********\n" +
                            "rule pcb (max_uncoupled_length " + EDP.Utils.CoordToMils(differentialPairsRoutingRule.GetState_MaxUncoupledLength()) + ")\n" +
                            "set average_pair_length off\n\n";

                        //Generate layer based values
                        foreach (V7_Layer item in LayerList)
                        {

                            LayerName = EDP.Utils.LayerToString(item).Replace(" ", "");

                            //"PREFGAP" DO file format:
                            //rule layer MidLayer1(edge_primary_gap 7)
                            Output += "rule layer " + LayerName + "(edge_primary_gap " + EDP.Utils.CoordToMils(differentialPairsRoutingRule.GetState_PreferedGap(item)) + ")\n";
                            //"PREFWIDTH" DO file format:
                            //rule layer MidLayer1(diffpair_line_width 8)
                            Output += "rule layer " + LayerName + "(diffpair_line_width " + EDP.Utils.CoordToMils(differentialPairsRoutingRule.GetState_PreferedWidth(item)) + ")\n";
                            //"MINGAP" DO file format:
                            //rule layer MidLayer1(min_line_spacing 7.5) 
                            Output += "rule layer " + LayerName + "(min_line_spacing " + EDP.Utils.CoordToMils(differentialPairsRoutingRule.GetState_MinGap(item)) + ")\n";

                        }
                    }
                    break;


                //#********** PCB WIRE WIDTH RULE **********
                //rule pcb (width 6)
                //rule pcb (clerance 8)

                #region Unused rules
                case TRuleKind.eRule_ParallelSegment:
                    break;
                case TRuleKind.eRule_DaisyChainStubLength:
                    break;
                case TRuleKind.eRule_PowerPlaneConnectStyle:
                    break;
                case TRuleKind.eRule_RoutingTopology:
                    break;
                case TRuleKind.eRule_RoutingPriority:
                    break;
                case TRuleKind.eRule_RoutingLayers:
                    break;
                case TRuleKind.eRule_RoutingCornerStyle:
                    break;
                case TRuleKind.eRule_RoutingViaStyle:
                    break;
                case TRuleKind.eRule_PowerPlaneClearance:
                    break;
                case TRuleKind.eRule_SolderMaskExpansion:
                    break;
                case TRuleKind.eRule_PasteMaskExpansion:
                    break;
                case TRuleKind.eRule_ShortCircuit:
                    break;
                case TRuleKind.eRule_BrokenNets:
                    break;
                case TRuleKind.eRule_ViasUnderSMD:
                    break;
                case TRuleKind.eRule_MaximumViaCount:
                    break;
                case TRuleKind.eRule_MinimumAnnularRing:
                    break;
                case TRuleKind.eRule_PolygonConnectStyle:
                    break;
                case TRuleKind.eRule_AcuteAngle:
                    break;
                case TRuleKind.eRule_ConfinementConstraint:
                    break;
                case TRuleKind.eRule_SMDToCorner:
                    break;
                case TRuleKind.eRule_ComponentClearance:
                    break;
                case TRuleKind.eRule_ComponentRotations:
                    break;
                case TRuleKind.eRule_PermittedLayers:
                    break;
                case TRuleKind.eRule_NetsToIgnore:
                    break;
                case TRuleKind.eRule_SignalStimulus:
                    break;
                case TRuleKind.eRule_Overshoot_FallingEdge:
                    break;
                case TRuleKind.eRule_Overshoot_RisingEdge:
                    break;
                case TRuleKind.eRule_Undershoot_FallingEdge:
                    break;
                case TRuleKind.eRule_Undershoot_RisingEdge:
                    break;
                case TRuleKind.eRule_MaxMinImpedance:
                    break;
                case TRuleKind.eRule_SignalTopValue:
                    break;
                case TRuleKind.eRule_SignalBaseValue:
                    break;
                case TRuleKind.eRule_FlightTime_RisingEdge:
                    break;
                case TRuleKind.eRule_FlightTime_FallingEdge:
                    break;
                case TRuleKind.eRule_LayerStack:
                    break;
                case TRuleKind.eRule_MaxSlope_RisingEdge:
                    break;
                case TRuleKind.eRule_MaxSlope_FallingEdge:
                    break;
                case TRuleKind.eRule_SupplyNets:
                    break;
                case TRuleKind.eRule_MaxMinHoleSize:
                    break;
                case TRuleKind.eRule_TestPointStyle:
                    break;
                case TRuleKind.eRule_TestPointUsage:
                    break;
                case TRuleKind.eRule_UnconnectedPin:
                    break;
                case TRuleKind.eRule_SMDToPlane:
                    break;
                case TRuleKind.eRule_SMDNeckDown:
                    break;
                case TRuleKind.eRule_LayerPair:
                    break;
                case TRuleKind.eRule_FanoutControl:
                    break;
                case TRuleKind.eRule_MaxMinHeight:
                    break;
                case TRuleKind.eRule_HoleToHoleClearance:
                    break;
                case TRuleKind.eRule_MinimumSolderMaskSliver:
                    break;
                case TRuleKind.eRule_SilkToSolderMaskClearance:
                    break;
                case TRuleKind.eRule_SilkToSilkClearance:
                    break;
                case TRuleKind.eRule_NetAntennae:
                    break;
                case TRuleKind.eRule_AssyTestPointStyle:
                    break;
                case TRuleKind.eRule_AssyTestPointUsage:
                    break;
                case TRuleKind.eRule_SilkToBoardRegion:
                    break;
                case TRuleKind.eRule_SMDPADEntry:
                    break;
                case TRuleKind.eRule_None:
                    break;
                case TRuleKind.eRule_ModifiedPolygon:
                    break;
                case TRuleKind.eRule_BoardOutlineClearance:
                    break;
                #endregion
                default:
                    break;
            }

            //Add rules that dont have a DO format.
            if (Output == "")
            {
                Output = "#Missed Rule : " + Rule.GetState_Name() +
                    ", Enabled: " + Rule.GetState_DRCEnabled() +
                    ", Desc : " + Rule.GetState_DescriptorString();
            }
            return Output;
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
            return "";
        }
    }

    /// <summary>
    /// Retrieves all the xSignal names for specified xSignal class.
    /// </summary>
    /// <param name="xSignalClass">xSignal class name</param>
    /// <param name="Board">Active board object.</param>
    /// <returns>List of xSignal names.</returns>
    List<string> GetxSignalNets(string xSignalClass, IPCB_Board Board)
    {
        Dictionary<string, List<string>> PrimList = new Dictionary<string, List<string>>();
        List<string> output = new List<string>();
        IPCB_BoardIterator Iterator;
        IPCB_ObjectClass2 ObjectClass;
        IPCB_PinPairsManager PinPairsManager;
        IPCB_PinPair PinPair;
        IClient Client = DXP.GlobalVars.Client;

        //Get pinpair manager
        PinPairsManager = Board.GetState_PinPairsManager();
        if (PinPairsManager == null)
            return null;

        PinPairsManager.InvalidateAll();

        Iterator = Board.BoardIterator_Create();
        try
        {
            //filter class objects
            Iterator.AddFilter_ObjectSet(Util.MKset(PCB.TObjectId.eClassObject));
            Iterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);

            //Getting xSignals
            ObjectClass = (IPCB_ObjectClass2)Iterator.FirstPCBObject();
            while (ObjectClass != null)
            {
                if (ObjectClass.GetState_MemberKind() == TClassMemberKind.eClassMemberKind_Signal)
                {
                    //Check to see if its in the xSignal class provided.
                    if (ObjectClass.GetState_DisplayName() == xSignalClass)
                    {
                        //Loop through collecting xSignal names.
                        for (int I = 0; I <= PinPairsManager.GetState_PinPairsCount() - 1; I++)
                        {
                            PinPair = PinPairsManager.GetState_PinPairs(I);
                            output.Add(PinPair.GetState_Name());
                        }
                        return output;
                    }
                }
                ObjectClass = (IPCB_ObjectClass2)Iterator.NextPCBObject();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Creates a list of Diff Pairs
    /// in a DO file format.
    /// </summary>
    /// <returns>Returns an arraylist of strings.</returns>
    public ArrayList DiffPairDoFile()
    {
        IPCB_Board Board;
        IPCB_BoardIterator BoardIterator;
        IPCB_DifferentialPair DiffPair;
        ArrayList Report = new ArrayList();
        IClient Client = DXP.GlobalVars.Client;
        string PosNet, NegNet;

        Board = Util.GetCurrentPCB();
        if (Board == null)
            return null;
        BoardIterator = Board.BoardIterator_Create();

        try
        {
            //Adding rule title to report.
            Report.Add("#********** DEFINE DIFFERENTIAL NET PAIR **********");


            //Iterate through all rules
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            FilterSet.Add(PCB.TObjectId.eDifferentialPairObject); //Filter for rules only
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
            DiffPair = (IPCB_DifferentialPair)BoardIterator.FirstPCBObject();
            //Step through all rules.
            while (DiffPair != null)
            {
                //DO file command format
                //define(pair(nets ADC_DI0_N ADC_DI0_P))
                PosNet = DiffPair.GetState_PositiveNet().GetState_Name();
                NegNet = DiffPair.GetState_NegativeNet().GetState_Name();
                Report.Add("define(pair(nets " + NegNet + " " + PosNet + "))");
                DiffPair = (IPCB_DifferentialPair)BoardIterator.NextPCBObject();
            }
            string OutputPath = Util.ProjPath() + "\\" + Path.GetFileNameWithoutExtension(Board.GetState_FileName()) + "-DiffPair.do";
            return Report;
            //Generate report.
            //File.WriteAllLines(OutputPath, (string[])Report.ToArray(typeof(string)));
            //Client.ShowDocument(Client.OpenDocument("Text", OutputPath));
        }
        //Catch error if file is open.
        catch (IOException)
        {
            MessageBox.Show("File in use. Please close the file and try again.");
        }
        catch (Exception ex)
        {
            var sb = new System.Text.StringBuilder();
            sb.AppendLine("");
            sb.AppendLine(ex.ToString());
            _Log.Fatal(sb);
            ErrorMail.LogError("Error in " + System.Reflection.MethodBase.GetCurrentMethod().Name + ".", ex);
        }
        finally
        {
            Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
        }
        return null;
    }


    /// <summary>
    /// Calculates the total and indavidual areas covered by components on the board.
    /// </summary>
    public void ComponentDensity()
    {
        try
        {
            IPCB_BoardIterator BoardIterator;
            IPCB_Component Component;
            string RefDes;
            IPCB_Board Board = Util.GetCurrentPCB();

            if (Board == null)
                return;
            BoardIterator = Board.BoardIterator_Create();

            List<string> TopParts = new List<string>();
            List<string> BottomParts = new List<string>();

            double tmp;
            double OriginX = EDP.Utils.CoordToMils(Board.GetState_XOrigin());
            double OriginY = EDP.Utils.CoordToMils(Board.GetState_YOrigin());
            double TopArea = 0.0, BotArea = 0.0;
            int intTopParts = 0, intBottomParts = 0;



            //Iterate theough all components on the board.
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
                if (RefDes != null)
                {
                    //Get component areas based on layer.
                    if (Component.GetState_Layer() == TV6_Layer.eV6_TopLayer)
                    {
                        intTopParts++;
                        //ABS((right-originx)-(left-originx)) * ABS((top-originy)-(bottom-originy))
                        tmp = (Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Right) - OriginX) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Left) - OriginX)) *
                            Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Top) - OriginY) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Bottom) - OriginY)));
                        TopArea += tmp;
                        TopParts.Add("Top Layer," + RefDes + "," + Math.Round(tmp, 3));
                    }
                    else if (Component.GetState_Layer() == TV6_Layer.eV6_BottomLayer)
                    {
                        intBottomParts++;
                        //ABS((right-originx)-(left-originx)) * ABS((top-originy)-(bottom-originy))
                        tmp = (Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Right) - OriginX) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Left) - OriginX)) *
                            Math.Abs((EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Top) - OriginY) - (EDP.Utils.CoordToMils(Component.BoundingRectangleNoNameCommentForSignals().Bottom) - OriginY)));
                        BotArea += tmp;
                        BottomParts.Add("Bottom Layer," + RefDes + "," + Math.Round(tmp, 3));
                    }
                    else
                        MessageBox.Show("Component " + RefDes + " not on Top or Bottom Layer.");

                }
                Component = (IPCB_Component)BoardIterator.NextPCBObject();
            }

            //Iterator clean-up
            Board.BoardIterator_Destroy(ref BoardIterator);

            //Get board area
            double BoardX, BoardY;
            BoardX = (EDP.Utils.CoordToMils(Board.GetState_BoardOutline().BoundingRectangle().Right) - OriginX) - (EDP.Utils.CoordToMils(Board.GetState_BoardOutline().BoundingRectangle().Left) - OriginX);
            BoardY = (EDP.Utils.CoordToMils(Board.GetState_BoardOutline().BoundingRectangle().Top) - OriginY) - (EDP.Utils.CoordToMils(Board.GetState_BoardOutline().BoundingRectangle().Bottom) - OriginY);
            double BoardArea = Math.Abs(BoardX) * Math.Abs(BoardY);
            ArrayList Report = new ArrayList();
            IClient Client = DXP.GlobalVars.Client;


            //Generate summary report.
            Report.Add("Component Density Report");
            Report.Add("");
            Report.Add("Board Area: " + Math.Round(BoardArea / 1000000.0, 3) + "sq\\in area.");
            Report.Add("");
            Report.Add("Top Layer");
            Report.Add(intTopParts + " components on the Top Layer.");
            Report.Add("Components cover a " + Math.Round(TopArea / 1000000.0, 3) + "sq\\in area.");
            Report.Add("");
            Report.Add("");
            Report.Add("Bottom Layer");
            Report.Add(intBottomParts + " components on the Bottom Layer.");
            Report.Add("Components cover a " + Math.Round(BotArea / 1000000.0, 3) + "sq\\in area.");


            if (!Directory.Exists(Util.ProjPath() + "Project Outputs\\"))
                Directory.CreateDirectory(Util.ProjPath() + "Project Outputs\\");

            string OutputFile = Util.ProjPath() + "Project Outputs\\";
            if (File.Exists(OutputFile + "Component Density Summary.txt"))
                if (IsFileLocked(new FileInfo(OutputFile + "Component Density Summary.txt")))
                {
                    MessageBox.Show("Export file, Component Density Summary.txt, is in use. Please close the file and try again.");
                    return;
                }

            if (File.Exists(OutputFile + "Component Density Expanded.csv"))
                if (IsFileLocked(new FileInfo(OutputFile + "Component Density Expanded.csv")))
                {
                    MessageBox.Show("Export file, Component Density Expanded.csv, is in use. Please close the file and try again.");
                    return;
                }

            File.WriteAllLines(OutputFile + "Component Density Summary.txt", (string[])Report.ToArray(typeof(string)));

            //Generate expanded report.
            Report = new ArrayList();
            Report.Add("Layer,Refdes,Area (sq\\mil)");
            Report.AddRange(TopParts);
            Report.AddRange(BottomParts);

            File.WriteAllLines(OutputFile + "Component Density Expanded.csv", (string[])Report.ToArray(typeof(string)));


            Client.ShowDocument(Client.OpenDocument("Text", OutputFile + "Component Density Summary.txt"));
            Client.ShowDocument(Client.OpenDocument("Text", OutputFile + "Component Density Expanded.csv"));
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
    protected virtual bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (FileStream stream = file.Open(FileMode.Open, FileAccess.Write, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }
}


//?Rule.GetState_RuleKind().ToString()
//"eRule_MatchedLengths"
//?Rule.GetState_Scope1Expression()
//"InNetClass('SERDESCLK')"

//circuit group ADC_DI3_P(max_total_length 2200) *xsignal
//  no option in altium (create DO for every xsignal in a class based on the xclass length rule)
//circuit net DAC_DB0_N(length 4200(type actual)) *net
//  Rule : Length, Desc : Length Constraint (Min=0mil) (Max=2000mil) (InNet('ADC_DI0_N')), Detail : 
//xsignalclass max length
//  Rule : Length_2, Desc : Length Constraint (Min=0mil) (Max=100000mil) (InxSignalClass('ADC')), Detail : 

//circuit class DAC_DB (match_net_length on(tolerance 100)) *net class "InNetClass"
//  Rule : MatchedLengthsDAC, Desc : Matched Lengths(Tolerance=10mil) (InNetClass('DAC')), Detail : 
//circuit group_set ADC(match_group_length on (tolerance 100)) *xsignal class
//  Rule : MatchedLengthsADC, Desc : Matched Lengths(Tolerance=10mil) (InxSignalClass('ADC')), Detail 


