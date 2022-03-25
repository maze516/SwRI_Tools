
    partial class FootprintSelect
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
            this.tvList = new System.Windows.Forms.TreeView();
            this.btnZoom = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.chkMask = new System.Windows.Forms.CheckBox();
            this.btnAlignSelected = new System.Windows.Forms.Button();
            this.cbSelectBase = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tvList
            // 
            this.tvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvList.Location = new System.Drawing.Point(0, 0);
            this.tvList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tvList.Name = "tvList";
            this.tvList.Size = new System.Drawing.Size(356, 487);
            this.tvList.TabIndex = 1;
            this.tvList.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvList_AfterSelect);
            // 
            // btnZoom
            // 
            this.btnZoom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnZoom.Location = new System.Drawing.Point(2, 547);
            this.btnZoom.Margin = new System.Windows.Forms.Padding(4);
            this.btnZoom.Name = "btnZoom";
            this.btnZoom.Size = new System.Drawing.Size(353, 28);
            this.btnZoom.TabIndex = 2;
            this.btnZoom.Text = "Zoom Selected";
            this.btnZoom.UseVisualStyleBackColor = true;
            this.btnZoom.Click += new System.EventHandler(this.btnZoom_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(2, 511);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(353, 28);
            this.btnRefresh.TabIndex = 2;
            this.btnRefresh.Text = "Refresh List";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // chkMask
            // 
            this.chkMask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMask.AutoSize = true;
            this.chkMask.Location = new System.Drawing.Point(239, 2);
            this.chkMask.Name = "chkMask";
            this.chkMask.Size = new System.Drawing.Size(97, 17);
            this.chkMask.TabIndex = 3;
            this.chkMask.Text = "Mask Selected";
            this.chkMask.UseVisualStyleBackColor = true;
            // 
            // btnAlignSelected
            // 
            this.btnAlignSelected.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAlignSelected.Location = new System.Drawing.Point(2, 583);
            this.btnAlignSelected.Margin = new System.Windows.Forms.Padding(4);
            this.btnAlignSelected.Name = "btnAlignSelected";
            this.btnAlignSelected.Size = new System.Drawing.Size(353, 28);
            this.btnAlignSelected.TabIndex = 2;
            this.btnAlignSelected.Text = "Align Selected to Base";
            this.btnAlignSelected.UseVisualStyleBackColor = true;
            this.btnAlignSelected.Click += new System.EventHandler(this.btnAlignSelected_Click);
            // 
            // cbSelectBase
            // 
            this.cbSelectBase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbSelectBase.AutoSize = true;
            this.cbSelectBase.Location = new System.Drawing.Point(239, 25);
            this.cbSelectBase.Name = "cbSelectBase";
            this.cbSelectBase.Size = new System.Drawing.Size(103, 17);
            this.cbSelectBase.TabIndex = 4;
            this.cbSelectBase.Text = "Select Base/FM";
            this.cbSelectBase.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(51, 490);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "refdes";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(14, 490);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Red";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.BackColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(101, 489);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "means footprints on different layers.";
            // 
            // btnReport
            // 
            this.btnReport.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReport.Location = new System.Drawing.Point(2, 619);
            this.btnReport.Margin = new System.Windows.Forms.Padding(4);
            this.btnReport.Name = "btnReport";
            this.btnReport.Size = new System.Drawing.Size(353, 28);
            this.btnReport.TabIndex = 8;
            this.btnReport.Text = "Generate Report";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // FootprintSelect
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(357, 650);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSelectBase);
            this.Controls.Add(this.chkMask);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnAlignSelected);
            this.Controls.Add(this.btnZoom);
            this.Controls.Add(this.tvList);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FootprintSelect";
            this.Text = "Footprint Select";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion
    private System.Windows.Forms.TreeView tvList;
    private System.Windows.Forms.Button btnZoom;
    private System.Windows.Forms.Button btnRefresh;
    private System.Windows.Forms.CheckBox chkMask;
    private System.Windows.Forms.Button btnAlignSelected;
    private System.Windows.Forms.CheckBox cbSelectBase;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnReport;
}
