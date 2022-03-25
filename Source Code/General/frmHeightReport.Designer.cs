
    partial class frmHeightReport
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
            this.lstComponents = new System.Windows.Forms.CheckedListBox();
            this.lnkSelectAll = new System.Windows.Forms.LinkLabel();
            this.lnkSelectNone = new System.Windows.Forms.LinkLabel();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstComponents
            // 
            this.lstComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstComponents.FormattingEnabled = true;
            this.lstComponents.HorizontalScrollbar = true;
            this.lstComponents.Location = new System.Drawing.Point(9, 24);
            this.lstComponents.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstComponents.Name = "lstComponents";
            this.lstComponents.Size = new System.Drawing.Size(170, 244);
            this.lstComponents.Sorted = true;
            this.lstComponents.TabIndex = 0;
            // 
            // lnkSelectAll
            // 
            this.lnkSelectAll.AutoSize = true;
            this.lnkSelectAll.Location = new System.Drawing.Point(9, 7);
            this.lnkSelectAll.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkSelectAll.Name = "lnkSelectAll";
            this.lnkSelectAll.Size = new System.Drawing.Size(51, 13);
            this.lnkSelectAll.TabIndex = 1;
            this.lnkSelectAll.TabStop = true;
            this.lnkSelectAll.Text = "Select All";
            this.lnkSelectAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelectAll_LinkClicked);
            // 
            // lnkSelectNone
            // 
            this.lnkSelectNone.AutoSize = true;
            this.lnkSelectNone.Location = new System.Drawing.Point(114, 7);
            this.lnkSelectNone.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lnkSelectNone.Name = "lnkSelectNone";
            this.lnkSelectNone.Size = new System.Drawing.Size(66, 13);
            this.lnkSelectNone.TabIndex = 1;
            this.lnkSelectNone.TabStop = true;
            this.lnkSelectNone.Text = "Select None";
            this.lnkSelectNone.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelectNone_LinkClicked);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdate.Location = new System.Drawing.Point(9, 281);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(56, 23);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(121, 280);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmHeightReport
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(186, 317);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lnkSelectNone);
            this.Controls.Add(this.lnkSelectAll);
            this.Controls.Add(this.lstComponents);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(202, 356);
            this.Name = "frmHeightReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Component Update List";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lstComponents;
        private System.Windows.Forms.LinkLabel lnkSelectAll;
        private System.Windows.Forms.LinkLabel lnkSelectNone;
    private System.Windows.Forms.Button btnUpdate;
    private System.Windows.Forms.Button btnCancel;
}
