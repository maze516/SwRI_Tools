using DXP;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;


public partial class DoFileGen : ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public const string PanelName = "DoReport";
    public const string PanelCaption = "DO File Gen";
    public DoFileGen()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        InitializeComponent();
        UI.ApplyADUITheme(this);
        //ListBoxItem Test = new ListBoxItem { Text = "Test", Tag = "Test"};
        //chkReportList.Items.Add(Test,false);
    }

    private void chkReportList_MouseUp(object sender, MouseEventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        ArrayList Output = new ArrayList();
        CheckedListBox.CheckedItemCollection CheckedItems = chkReportList.CheckedItems;
        try
        {

            foreach (object item in CheckedItems)
            {
                switch (item.ToString())
                {
                    case "xSignals":
                        Output.AddRange(new Export().xSignalDoReport());
                        break;
                    case "Rules":
                        Output.AddRange(new Export().RuleDoReport());
                        break;
                    case "Layer Select":
                        Output.AddRange(new Export().LayerSelectDo());
                        break;
                    case "Layer Orientation":
                        Output.AddRange(new Export().LayerDirectionDo());
                        break;
                    case "Same Net Checking":
                        Output.AddRange(new Export().SameNetDo());
                        break;
                    case "Diff Pairs":
                        Output.AddRange(new Export().DiffPairDoFile());
                        break;

                }

                //MessageBox.Show(item.GetType().ToString());
            }
            txtOutput.Lines = (string[])Output.ToArray(typeof(string));

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

    private void btnSave_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        SaveFileDialog FileDiag = new SaveFileDialog();
        FileDiag.OverwritePrompt = true;
        FileDiag.DefaultExt = "txt";
        FileDiag.Filter = "Text files (*.txt)|*.txt|DO files (*.DO)|*.DO|All files (*.*)|*.*";
        DialogResult DiagResult = FileDiag.ShowDialog();
        if (DiagResult == DialogResult.OK) {
            IClient Client = DXP.GlobalVars.Client;

            File.WriteAllLines(FileDiag.FileName, txtOutput.Lines);

            Client.ShowDocument(Client.OpenDocument("Text", FileDiag.FileName));
        }
    }

    private void chkReportList_SelectedIndexChanged(object sender, EventArgs e)
    {

    }
}

//public class ListBoxItem
//{
//    public string Text { get; set; }
//    public object Tag { get; set; }
//    public override string ToString()
//    {
//        return Text;
//    }
//}