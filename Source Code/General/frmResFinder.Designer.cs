
partial class frmResFinder
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
            this.numTarget = new System.Windows.Forms.NumericUpDown();
            this.btnFind = new System.Windows.Forms.Button();
            this.txtOutput = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMax = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numTarget)).BeginInit();
            this.SuspendLayout();
            // 
            // numTarget
            // 
            this.numTarget.DecimalPlaces = 2;
            this.numTarget.Location = new System.Drawing.Point(11, 28);
            this.numTarget.Maximum = new decimal(new int[] {
            999999999,
            0,
            0,
            0});
            this.numTarget.Name = "numTarget";
            this.numTarget.Size = new System.Drawing.Size(120, 20);
            this.numTarget.TabIndex = 0;
            this.numTarget.Value = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.numTarget.ValueChanged += new System.EventHandler(this.numTarget_ValueChanged);
            this.numTarget.KeyDown += new System.Windows.Forms.KeyEventHandler(this.numTarget_KeyDown);
            // 
            // btnFind
            // 
            this.btnFind.Location = new System.Drawing.Point(11, 54);
            this.btnFind.Margin = new System.Windows.Forms.Padding(0);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(64, 23);
            this.btnFind.TabIndex = 1;
            this.btnFind.Text = "Find";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // txtOutput
            // 
            this.txtOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtOutput.Location = new System.Drawing.Point(12, 80);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.ReadOnly = true;
            this.txtOutput.Size = new System.Drawing.Size(301, 447);
            this.txtOutput.TabIndex = 2;
            this.txtOutput.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Target Resistor Value (Ohms)";
            // 
            // lblMax
            // 
            this.lblMax.AutoSize = true;
            this.lblMax.Location = new System.Drawing.Point(137, 30);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(0, 13);
            this.lblMax.TabIndex = 4;
            // 
            // frmResFinder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(325, 539);
            this.Controls.Add(this.lblMax);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.numTarget);
            this.KeyPreview = true;
            this.Name = "frmResFinder";
            this.Text = "Resistor Finder";
            this.Load += new System.EventHandler(this.frmResFinder_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numTarget)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion
     
    private System.Windows.Forms.NumericUpDown numTarget;
    private System.Windows.Forms.Button btnFind;
    private System.Windows.Forms.RichTextBox txtOutput;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblMax;
}


