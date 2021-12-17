using DXP;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public partial class frmResFinder : ServerPanelForm
{
    public static readonly Logger _Log = LogManager.GetLogger(Util.SERVERNAME);

    public const string PanelName = "ResFinder";
    public const string PanelCaption = "Resistor Value Finder";
    public List<double> ResValues;

    public frmResFinder()
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        InitializeComponent();
        UI.ApplyADUITheme(this);

    }

    private void btnFind_Click(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        double find = (double)numTarget.Value;
        List<Values> SeriesOutput = new List<Values>();
        List<Values> ParallelOutput = new List<Values>();


        int index;

        index = ResValues.BinarySearch(find);
        if (0 <= index)
            txtOutput.Text = "Found value " + find + " at list[" + index + "]";
        else
        {
            index = ~index;
            if (0 < index)
                txtOutput.Text = "Nearest Under: " + ResValues[index - 1];
            if (ResValues.Count > index)
                txtOutput.Text += "\r\nNearest Over: " + ResValues[index];
            //Console.Out.WriteLine("value "+ find+" should be inserted at index "+index);

        }

        //txtOutput.Text = "";
        int Step = 3;
        double OffsetLimit = 0.01;
        double tmpOffset, tmpResult;
        foreach (double Res1 in ResValues)
        {
            foreach (double Res2 in ResValues)
            {
                //Series resistors pairs
                tmpResult = Res1 + Res2;
                tmpOffset = Math.Abs(find - tmpResult) / find;
                if (tmpOffset <= OffsetLimit & Res1 >= find / Step & Res1 <= find * Step & Res2 >= find / Step & Res2 <= find * Step)
                    if (!SeriesOutput.Contains(new Values() { R1 = Res2, R2 = Res1, Offset = Math.Round(tmpOffset, 3), Series = true }))
                        SeriesOutput.Add(new Values() { R1 = Res1, R2 = Res2, Offset = Math.Round(tmpOffset, 3), Series = true });

                //Parallel resistor pairs
                tmpOffset = Math.Abs(find - ((Res1 * Res2) / (Res1 + Res2))) / find;
                if (tmpOffset <= OffsetLimit & Res1 >= find / Step & Res1 <= find * Step & Res2 >= find / Step & Res2 <= find * Step)
                    if (!ParallelOutput.Contains(new Values() { R1 = Res2, R2 = Res1, Offset = Math.Round(tmpOffset, 3), Series = false }))
                        ParallelOutput.Add(new Values() { R1 = Res1, R2 = Res2, Offset = Math.Round(tmpOffset, 3), Series = false });
            }
        }


        SeriesOutput.Sort((O1, O2) => O1.R1.CompareTo(O2.R1));
        SeriesOutput.Sort((O1, O2) => O1.Offset.CompareTo(O2.Offset));
        ParallelOutput.Sort((O1, O2) => O1.R1.CompareTo(O2.R1));
        ParallelOutput.Sort((O1, O2) => O1.Offset.CompareTo(O2.Offset));

        txtOutput.Text += "\r\n\r\nSeries Options";
        foreach (Values item in SeriesOutput)
            txtOutput.Text += "\r\nR1: " + item.R1 + ", R2: " + item.R2 + " Offset: " + item.Offset * 100 + "%";

        txtOutput.Text += "\r\n\r\nParallel Options";
        foreach (Values item in ParallelOutput)
            txtOutput.Text += "\r\nR1: " + item.R1 + ", R2: " + item.R2 + " Offset: " + item.Offset * 100 + "%";

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"> 48, 96, 192</param>
    /// <param name="d"> 1, 10, 100, 1000, 10000, 100000</param>
    void FillResValues(int[] Steps, List<double> Decades)
    { //r=d*10^(i/n)
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        double Multiplier;
        int tmp;
        double dtmp;
        int[] Step24 = new int[] { 100, 110, 120, 130, 150, 160, 180, 200, 220, 240, 270, 300, 330, 360, 390, 430, 470, 510, 560, 620, 680, 750, 820, 910 };

        if (Steps == null)
        {
            MessageBox.Show("Steps list is empty.");
            return;
        }
        else if (Steps.Length == 0)
        {
            MessageBox.Show("Steps list is empty.");
            return;
        }
        if (Decades == null)
        {
            MessageBox.Show("Decades list is empty.");
            return;
        }
        else if (Decades.Count == 0)
        {
            MessageBox.Show("Decades list is empty.");
            return;
        }

        List<int> tmpResList = new List<int>();
        //if (d > 100)
        //{
        //    Multiplier = d / 100;
        //    d = 100;
        //}
        foreach (double Step in Steps)
        {
            if (Step == 24)
            {
                foreach (int item in Step24)
                {
                    if (!tmpResList.Contains(item))
                        tmpResList.Add(item);
                }
            }
            else
                for (int i = 0; i <= Step; i++)
                {
                    tmp = (int)Math.Round(100 * Math.Pow(10.0, (i / Step)), 0);
                    if (!tmpResList.Contains(tmp))
                        tmpResList.Add(tmp);
                }
        }

        foreach (double Decade in Decades)
        {
            Multiplier = Decade / 100;
            foreach (int value in tmpResList)
            {
                dtmp = Math.Round(value * Multiplier, 2);
                if (!ResValues.Contains(dtmp))
                    ResValues.Add(dtmp);
            }
        }

    }



    struct Values
    {
        public bool Series;
        public double Offset;
        public double R1;
        public double R2;
        //public double R3;

    }


    private void numTarget_KeyDown(object sender, KeyEventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (e.KeyCode == Keys.Enter)
            btnFind_Click(null, null);
    }

    private void frmResFinder_Load(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        ResValues = new List<double>();// { 100, 110, 120, 130, 150, 160, 180, 200, 220, 240, 270, 300, 330, 360, 390, 430, 470, 510, 560, 620, 680, 750, 820, 910 };
        List<double> Decades = new List<double>();
        int[] Steps = new int[] { 24, 48, 96 }; //24, 48, 96, 192


        for (int i = 0; i < 8; i++)
        {
            Decades.Add(Math.Pow(10, i));
        }

        FillResValues(Steps, Decades);
        ResValues.Sort();
        //txtOutput.Text = String.Join(",", ResValues.Select(x => x.ToString()).ToArray()); ;
    }

    private void numTarget_ValueChanged(object sender, EventArgs e)
    {
        _Log.Trace(System.Reflection.MethodBase.GetCurrentMethod().Name);

        if (numTarget.Value > (decimal)ResValues[ResValues.Count - 1])
            lblMax.Text = "Max single value is " + ResValues[ResValues.Count - 1];
        else
            lblMax.Text = "";
    }
}


/*
 * r=d*10^(i/n)
 * 
 * d= 1, 10, 100, 1000, 10000
 * n=192, 96
 * i=0...n-1
 * 
 * y=T-x
 */
