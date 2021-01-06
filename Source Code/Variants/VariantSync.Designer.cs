
    partial class VariantSync
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
            this.cbSource = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnSync = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.rbOverwrite = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.rbPrompt = new System.Windows.Forms.RadioButton();
            this.rbSkip = new System.Windows.Forms.RadioButton();
            this.pbProgress = new System.Windows.Forms.ProgressBar();
            this.cbForce = new System.Windows.Forms.CheckBox();
            this.cbDest = new System.Windows.Forms.CheckedListBox();
            this.SuspendLayout();
            // 
            // cbSource
            // 
            this.cbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSource.FormattingEnabled = true;
            this.cbSource.Location = new System.Drawing.Point(14, 28);
            this.cbSource.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbSource.Name = "cbSource";
            this.cbSource.Size = new System.Drawing.Size(92, 21);
            this.cbSource.TabIndex = 0;
            this.cbSource.SelectedIndexChanged += new System.EventHandler(this.cbSource_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 11);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "From";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(151, 11);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(20, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "To";
            // 
            // btnSync
            // 
            this.btnSync.Location = new System.Drawing.Point(48, 137);
            this.btnSync.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(56, 21);
            this.btnSync.TabIndex = 2;
            this.btnSync.Text = "Sync";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(135, 137);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 21);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // rbOverwrite
            // 
            this.rbOverwrite.AutoSize = true;
            this.rbOverwrite.Checked = true;
            this.rbOverwrite.Location = new System.Drawing.Point(14, 104);
            this.rbOverwrite.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbOverwrite.Name = "rbOverwrite";
            this.rbOverwrite.Size = new System.Drawing.Size(70, 17);
            this.rbOverwrite.TabIndex = 3;
            this.rbOverwrite.TabStop = true;
            this.rbOverwrite.Text = "Overwrite";
            this.rbOverwrite.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 88);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Existing Data";
            // 
            // rbPrompt
            // 
            this.rbPrompt.AutoSize = true;
            this.rbPrompt.Location = new System.Drawing.Point(85, 104);
            this.rbPrompt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbPrompt.Name = "rbPrompt";
            this.rbPrompt.Size = new System.Drawing.Size(58, 17);
            this.rbPrompt.TabIndex = 3;
            this.rbPrompt.Text = "Prompt";
            this.rbPrompt.UseVisualStyleBackColor = true;
            // 
            // rbSkip
            // 
            this.rbSkip.AutoSize = true;
            this.rbSkip.Location = new System.Drawing.Point(145, 104);
            this.rbSkip.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.rbSkip.Name = "rbSkip";
            this.rbSkip.Size = new System.Drawing.Size(46, 17);
            this.rbSkip.TabIndex = 3;
            this.rbSkip.Text = "Skip";
            this.rbSkip.UseVisualStyleBackColor = true;
            // 
            // pbProgress
            // 
            this.pbProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbProgress.Location = new System.Drawing.Point(-2, 173);
            this.pbProgress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pbProgress.Maximum = 10;
            this.pbProgress.Name = "pbProgress";
            this.pbProgress.Size = new System.Drawing.Size(256, 19);
            this.pbProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pbProgress.TabIndex = 5;
            // 
            // cbForce
            // 
            this.cbForce.Enabled = false;
            this.cbForce.Location = new System.Drawing.Point(14, 50);
            this.cbForce.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbForce.Name = "cbForce";
            this.cbForce.Size = new System.Drawing.Size(129, 33);
            this.cbForce.TabIndex = 6;
            this.cbForce.Text = "Force 0 Alternates to Fitted";
            this.cbForce.UseVisualStyleBackColor = true;
            this.cbForce.CheckedChanged += new System.EventHandler(this.cbForce_CheckedChanged);
            // 
            // cbDest
            // 
            this.cbDest.CheckOnClick = true;
            this.cbDest.FormattingEnabled = true;
            this.cbDest.Location = new System.Drawing.Point(153, 28);
            this.cbDest.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDest.Name = "cbDest";
            this.cbDest.Size = new System.Drawing.Size(91, 64);
            this.cbDest.TabIndex = 7;
            this.cbDest.SelectedIndexChanged += new System.EventHandler(this.cbDest_SelectedIndexChanged);
            // 
            // VariantSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 191);
            this.Controls.Add(this.cbDest);
            this.Controls.Add(this.cbForce);
            this.Controls.Add(this.pbProgress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rbSkip);
            this.Controls.Add(this.rbPrompt);
            this.Controls.Add(this.rbOverwrite);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbSource);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "VariantSync";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sync Variants";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbSource;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.RadioButton rbOverwrite;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.RadioButton rbPrompt;
    private System.Windows.Forms.RadioButton rbSkip;
    private System.Windows.Forms.ProgressBar pbProgress;
    private System.Windows.Forms.CheckBox cbForce;
    private System.Windows.Forms.CheckedListBox cbDest;
}
