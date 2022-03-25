
    partial class frmBatchOutjob
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.treeOutjobs = new System.Windows.Forms.TreeView();
            this.cboVariant = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbClose = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Available Outjobs";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGenerate.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnGenerate.Location = new System.Drawing.Point(9, 363);
            this.btnGenerate.Margin = new System.Windows.Forms.Padding(2);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(62, 22);
            this.btnGenerate.TabIndex = 2;
            this.btnGenerate.Text = "Generate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(76, 363);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(62, 22);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // treeOutjobs
            // 
            this.treeOutjobs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeOutjobs.CheckBoxes = true;
            this.treeOutjobs.Location = new System.Drawing.Point(9, 24);
            this.treeOutjobs.Margin = new System.Windows.Forms.Padding(2);
            this.treeOutjobs.Name = "treeOutjobs";
            this.treeOutjobs.PathSeparator = "-";
            this.treeOutjobs.Size = new System.Drawing.Size(276, 336);
            this.treeOutjobs.TabIndex = 3;
            this.treeOutjobs.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeOutjobs_AfterCheck);
            // 
            // cboVariant
            // 
            this.cboVariant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboVariant.FormattingEnabled = true;
            this.cboVariant.Location = new System.Drawing.Point(194, 2);
            this.cboVariant.Margin = new System.Windows.Forms.Padding(2);
            this.cboVariant.Name = "cboVariant";
            this.cboVariant.Size = new System.Drawing.Size(92, 21);
            this.cboVariant.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 4);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Variant:";
            // 
            // cbClose
            // 
            this.cbClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cbClose.AutoSize = true;
            this.cbClose.Checked = true;
            this.cbClose.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbClose.Location = new System.Drawing.Point(153, 365);
            this.cbClose.Name = "cbClose";
            this.cbClose.Size = new System.Drawing.Size(131, 17);
            this.cbClose.TabIndex = 7;
            this.cbClose.Text = "Close When Complete";
            this.cbClose.UseVisualStyleBackColor = true;
            // 
            // frmBatchOutjob
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(295, 396);
            this.Controls.Add(this.cbClose);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cboVariant);
            this.Controls.Add(this.treeOutjobs);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(311, 435);
            this.Name = "frmBatchOutjob";
            this.Text = "frmBatchOutjob";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmBatchOutjob_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnCancel;
        internal System.Windows.Forms.TreeView treeOutjobs;
        private System.Windows.Forms.ComboBox cboVariant;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cbClose;
    }
