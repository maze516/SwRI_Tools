
partial class frmViaReplace
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
            this.lstBefore = new System.Windows.Forms.ListBox();
            this.lstAfter = new System.Windows.Forms.ListBox();
            this.btnReplaceAll = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSelected = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnUpdateList = new System.Windows.Forms.Button();
            this.numDrill = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.numPad = new System.Windows.Forms.NumericUpDown();
            this.radImperial = new System.Windows.Forms.RadioButton();
            this.radMetric = new System.Windows.Forms.RadioButton();
            this.btnRemoveDupe = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numDrill)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPad)).BeginInit();
            this.SuspendLayout();
            // 
            // lstBefore
            // 
            this.lstBefore.Location = new System.Drawing.Point(12, 106);
            this.lstBefore.Name = "lstBefore";
            this.lstBefore.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstBefore.Size = new System.Drawing.Size(157, 251);
            this.lstBefore.TabIndex = 0;
            // 
            // lstAfter
            // 
            this.lstAfter.FormattingEnabled = true;
            this.lstAfter.Location = new System.Drawing.Point(175, 106);
            this.lstAfter.Name = "lstAfter";
            this.lstAfter.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstAfter.Size = new System.Drawing.Size(157, 251);
            this.lstAfter.TabIndex = 1;
            // 
            // btnReplaceAll
            // 
            this.btnReplaceAll.Location = new System.Drawing.Point(15, 363);
            this.btnReplaceAll.Name = "btnReplaceAll";
            this.btnReplaceAll.Size = new System.Drawing.Size(75, 23);
            this.btnReplaceAll.TabIndex = 2;
            this.btnReplaceAll.Text = "Replace All";
            this.btnReplaceAll.UseVisualStyleBackColor = true;
            this.btnReplaceAll.Click += new System.EventHandler(this.btnReplaceAll_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 90);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Initial Drill Pair";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "New Drill Pair";
            // 
            // btnSelected
            // 
            this.btnSelected.Location = new System.Drawing.Point(97, 363);
            this.btnSelected.Name = "btnSelected";
            this.btnSelected.Size = new System.Drawing.Size(100, 23);
            this.btnSelected.TabIndex = 5;
            this.btnSelected.Text = "Replace Selected";
            this.btnSelected.UseVisualStyleBackColor = true;
            this.btnSelected.Click += new System.EventHandler(this.btnSelected_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Enabled = false;
            this.btnSelect.Location = new System.Drawing.Point(203, 363);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(74, 23);
            this.btnSelect.TabIndex = 6;
            this.btnSelect.Text = "Select";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnUpdateList
            // 
            this.btnUpdateList.Location = new System.Drawing.Point(12, 62);
            this.btnUpdateList.Name = "btnUpdateList";
            this.btnUpdateList.Size = new System.Drawing.Size(75, 23);
            this.btnUpdateList.TabIndex = 9;
            this.btnUpdateList.Text = "Update List";
            this.btnUpdateList.UseVisualStyleBackColor = true;
            this.btnUpdateList.Click += new System.EventHandler(this.btnUpdateList_Click);
            // 
            // numDrill
            // 
            this.numDrill.DecimalPlaces = 3;
            this.numDrill.Location = new System.Drawing.Point(172, 65);
            this.numDrill.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numDrill.Name = "numDrill";
            this.numDrill.Size = new System.Drawing.Size(64, 20);
            this.numDrill.TabIndex = 10;
            this.numDrill.Value = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(167, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Drill Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(252, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Pad Size";
            // 
            // numPad
            // 
            this.numPad.DecimalPlaces = 3;
            this.numPad.Location = new System.Drawing.Point(257, 65);
            this.numPad.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numPad.Name = "numPad";
            this.numPad.Size = new System.Drawing.Size(64, 20);
            this.numPad.TabIndex = 12;
            this.numPad.Value = new decimal(new int[] {
            2,
            0,
            0,
            65536});
            // 
            // radImperial
            // 
            this.radImperial.AutoSize = true;
            this.radImperial.Location = new System.Drawing.Point(170, 12);
            this.radImperial.Name = "radImperial";
            this.radImperial.Size = new System.Drawing.Size(61, 17);
            this.radImperial.TabIndex = 14;
            this.radImperial.Text = "Imperial";
            this.radImperial.UseVisualStyleBackColor = true;
            // 
            // radMetric
            // 
            this.radMetric.AutoSize = true;
            this.radMetric.Checked = true;
            this.radMetric.Location = new System.Drawing.Point(170, 28);
            this.radMetric.Name = "radMetric";
            this.radMetric.Size = new System.Drawing.Size(54, 17);
            this.radMetric.TabIndex = 14;
            this.radMetric.TabStop = true;
            this.radMetric.Text = "Metric";
            this.radMetric.UseVisualStyleBackColor = true;
            // 
            // btnRemoveDupe
            // 
            this.btnRemoveDupe.Location = new System.Drawing.Point(15, 392);
            this.btnRemoveDupe.Name = "btnRemoveDupe";
            this.btnRemoveDupe.Size = new System.Drawing.Size(91, 23);
            this.btnRemoveDupe.TabIndex = 15;
            this.btnRemoveDupe.Text = "Remove Dupes";
            this.btnRemoveDupe.UseVisualStyleBackColor = true;
            this.btnRemoveDupe.Click += new System.EventHandler(this.btnRemoveDupe_Click);
            // 
            // frmViaReplace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(340, 422);
            this.Controls.Add(this.btnRemoveDupe);
            this.Controls.Add(this.radMetric);
            this.Controls.Add(this.radImperial);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numPad);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numDrill);
            this.Controls.Add(this.btnUpdateList);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.btnSelected);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReplaceAll);
            this.Controls.Add(this.lstAfter);
            this.Controls.Add(this.lstBefore);
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(0, 422);
            this.Name = "frmViaReplace";
            this.Text = "ViaReplace";
            ((System.ComponentModel.ISupportInitialize)(this.numDrill)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPad)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ListBox lstBefore;
    private System.Windows.Forms.ListBox lstAfter;
    private System.Windows.Forms.Button btnReplaceAll;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btnSelected;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnUpdateList;
    private System.Windows.Forms.NumericUpDown numDrill;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.NumericUpDown numPad;
    private System.Windows.Forms.RadioButton radImperial;
    private System.Windows.Forms.RadioButton radMetric;
    private System.Windows.Forms.Button btnRemoveDupe;
}
