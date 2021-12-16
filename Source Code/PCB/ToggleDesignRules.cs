using NLog;
using PCB;
using System;
using System.Windows.Forms;


public class ToggleDesignRules
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);
    IPCB_ServerInterface PCBServer = PCB.GlobalVars.PCBServer;

    private string myRuleName = "";
    /// <summary>
    /// Property to get/set myRuleName.
    /// </summary>
    public string RuleName
    {
        get
        {
            return myRuleName;
        }
        set
        {
            myRuleName = value;
        }
    }
    /// <summary>
    /// Property used to add a check mark to the menu item if rule, matching myRuleName, is enabled.
    /// </summary>
    public bool Status
    {
        get
        {
            if (myRuleName == "")
                return false;
            IPCB_Rule Rule = GetRule(myRuleName); //Get rule by name.
            if (Rule == null)
                return false;
            return Rule.GetState_DRCEnabled();
        }
    }
    /// <summary>
    /// Property used to enable/disable menu item if rule exists.
    /// </summary>
    public bool Enabled
    {
        get
        {
            if (myRuleName == "")
                return false;
            IPCB_Rule Rule = GetRule(myRuleName);
            if (Rule == null)
                return false;
            return true;
        }
    }
    /// <summary>
    /// Get rule based on name.
    /// </summary>
    /// <param name="RuleName">Name of the rule</param>
    /// <returns>Returns rule object matching RuleName or null.</returns>
    private IPCB_Rule GetRule(string RuleName)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            IPCB_Board Board;
            IPCB_Rule Rule;
            IPCB_BoardIterator BoardIterator;

            Board = Util.GetCurrentPCB();
            if (Board == null)
                return null;

            //Iterate through all rules
            BoardIterator = Board.BoardIterator_Create();
            PCB.TObjectSet FilterSet = new PCB.TObjectSet();
            FilterSet.Add(PCB.TObjectId.eRuleObject); //Filter for rules only
            BoardIterator.AddFilter_ObjectSet(FilterSet);
            BoardIterator.AddFilter_LayerSet(PCBConstant.V6AllLayersSet);
            BoardIterator.AddFilter_Method(TIterationMethod.eProcessAll);
            Rule = (IPCB_Rule)BoardIterator.FirstPCBObject();
            //Step through all rules to find one matching RuleName.
            while (Rule != null)
            {
                if (Rule.GetState_Name() == RuleName)
                {
                    Board.BoardIterator_Destroy(ref BoardIterator); //Iterator clean-up
                    return Rule; //Return matching rule.
                }
                Rule = (IPCB_Rule)BoardIterator.NextPCBObject();
            }

            return null; //No match found.
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
    }
    /// <summary>
    /// Toggle rule on/off.
    /// </summary>
    /// <param name="RuleName">Name of rule to toggle</param>
    public void ToggleDesignRule(string RuleName)
    {
        _Log.Debug(System.Reflection.MethodBase.GetCurrentMethod().Name);

        try
        {
            IPCB_Rule Rule;
            IPCB_Board Board = Util.GetCurrentPCB();
            if (Board == null)
                return;

            Rule = GetRule(RuleName);
            if (Rule == null)
            {
                MessageBox.Show("No rule found that matches '" + RuleName + "'.");
                return;
            }
            PCBServer.PreProcess();
            Rule.BeginModify();   //        {Rule has to be prepared for modification}
            Rule.SetState_DRCEnabled(!Rule.GetState_DRCEnabled()); // Toggle rule state.
            Rule.EndModify();     //        {Let script know we are done modifying the rule}


            Rule = null;
            PCBServer.PostProcess();
            //{Dispatch message to system now that processing is complete}
            Board.DispatchMessage(SCH.SCHConstant.FromSystem, SCH.SCHConstant.BroadCast, SCH.SCHConstant.SCHMYieldToRobots, SCH.SCHConstant.NoEventData);


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
