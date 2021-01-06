
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
            this.lstComponents.Location = new System.Drawing.Point(12, 29);
            this.lstComponents.Name = "lstComponents";
            this.lstComponents.Size = new System.Drawing.Size(225, 310);
            this.lstComponents.Sorted = true;
            this.lstComponents.TabIndex = 0;
            // 
            // lnkSelectAll
            // 
            this.lnkSelectAll.AutoSize = true;
            this.lnkSelectAll.Location = new System.Drawing.Point(12, 9);
            this.lnkSelectAll.Name = "lnkSelectAll";
            this.lnkSelectAll.Size = new System.Drawing.Size(66, 17);
            this.lnkSelectAll.TabIndex = 1;
            this.lnkSelectAll.TabStop = true;
            this.lnkSelectAll.Text = "Select All";
            this.lnkSelectAll.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelectAll_LinkClicked);
            // 
            // lnkSelectNone
            // 
            this.lnkSelectNone.AutoSize = true;
            this.lnkSelectNone.Location = new System.Drawing.Point(152, 9);
            this.lnkSelectNone.Name = "lnkSelectNone";
            this.lnkSelectNone.Size = new System.Drawing.Size(85, 17);
            this.lnkSelectNone.TabIndex = 1;
            this.lnkSelectNone.TabStop = true;
            this.lnkSelectNone.Text = "Select None";
            this.lnkSelectNone.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSelectNone_LinkClicked);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnUpdate.Location = new System.Drawing.Point(12, 346);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 28);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "Update";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(161, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 29);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmHeightReport
            // 
            this.AcceptButton = this.btnUpdate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(246, 384);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lnkSelectNone);
            this.Controls.Add(this.lnkSelectAll);
            this.Controls.Add(this.lstComponents);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(264, 429);
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
