
    partial class frmPlaceReplicate
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
            this.lstSource = new System.Windows.Forms.ListBox();
            this.lstDest = new System.Windows.Forms.ListBox();
            this.btnMatch = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lstMatched = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDest = new System.Windows.Forms.Button();
            this.btnPlace = new System.Windows.Forms.Button();
            this.btnSource = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.chkAutoMatch = new System.Windows.Forms.CheckBox();
            this.btnFullReset = new System.Windows.Forms.Button();
            this.chkInDepth = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lstSource
            // 
            this.lstSource.FormattingEnabled = true;
            this.lstSource.HorizontalScrollbar = true;
            this.lstSource.Location = new System.Drawing.Point(14, 54);
            this.lstSource.Name = "lstSource";
            this.lstSource.Size = new System.Drawing.Size(63, 199);
            this.lstSource.Sorted = true;
            this.lstSource.TabIndex = 0;
            this.lstSource.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstSource_MouseClick);
            this.lstSource.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstSource_MouseDoubleClick);
            // 
            // lstDest
            // 
            this.lstDest.FormattingEnabled = true;
            this.lstDest.HorizontalScrollbar = true;
            this.lstDest.Location = new System.Drawing.Point(99, 54);
            this.lstDest.Name = "lstDest";
            this.lstDest.Size = new System.Drawing.Size(63, 199);
            this.lstDest.Sorted = true;
            this.lstDest.TabIndex = 0;
            this.lstDest.MouseClick += new System.Windows.Forms.MouseEventHandler(this.lstDest_MouseClick);
            this.lstDest.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstDest_MouseDoubleClick);
            // 
            // btnMatch
            // 
            this.btnMatch.Location = new System.Drawing.Point(58, 324);
            this.btnMatch.Name = "btnMatch";
            this.btnMatch.Size = new System.Drawing.Size(75, 23);
            this.btnMatch.TabIndex = 1;
            this.btnMatch.Text = "Match";
            this.btnMatch.UseVisualStyleBackColor = true;
            this.btnMatch.Click += new System.EventHandler(this.btnMatch_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(202, 353);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(90, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 38);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(96, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Destination";
            // 
            // lstMatched
            // 
            this.lstMatched.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstMatched.FormattingEnabled = true;
            this.lstMatched.HorizontalScrollbar = true;
            this.lstMatched.Location = new System.Drawing.Point(202, 54);
            this.lstMatched.MinimumSize = new System.Drawing.Size(90, 264);
            this.lstMatched.Name = "lstMatched";
            this.lstMatched.Size = new System.Drawing.Size(90, 264);
            this.lstMatched.TabIndex = 0;
            this.lstMatched.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstMatched_MouseDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(199, 38);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Matched Pairs";
            // 
            // btnDest
            // 
            this.btnDest.Location = new System.Drawing.Point(99, 12);
            this.btnDest.Name = "btnDest";
            this.btnDest.Size = new System.Drawing.Size(79, 23);
            this.btnDest.TabIndex = 1;
            this.btnDest.Text = "Add Dest";
            this.btnDest.UseVisualStyleBackColor = true;
            this.btnDest.Click += new System.EventHandler(this.btnDest_Click);
            // 
            // btnPlace
            // 
            this.btnPlace.Location = new System.Drawing.Point(202, 324);
            this.btnPlace.Name = "btnPlace";
            this.btnPlace.Size = new System.Drawing.Size(90, 23);
            this.btnPlace.TabIndex = 1;
            this.btnPlace.Text = "Place";
            this.btnPlace.UseVisualStyleBackColor = true;
            this.btnPlace.Click += new System.EventHandler(this.btnPlace_Click);
            // 
            // btnSource
            // 
            this.btnSource.Location = new System.Drawing.Point(14, 12);
            this.btnSource.Name = "btnSource";
            this.btnSource.Size = new System.Drawing.Size(79, 23);
            this.btnSource.TabIndex = 1;
            this.btnSource.Text = "Add Source";
            this.btnSource.UseVisualStyleBackColor = true;
            this.btnSource.Click += new System.EventHandler(this.btnSource_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(12, 405);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.Size = new System.Drawing.Size(280, 101);
            this.txtMessage.TabIndex = 3;
            // 
            // chkAutoMatch
            // 
            this.chkAutoMatch.AutoSize = true;
            this.chkAutoMatch.Checked = true;
            this.chkAutoMatch.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoMatch.Location = new System.Drawing.Point(14, 353);
            this.chkAutoMatch.Name = "chkAutoMatch";
            this.chkAutoMatch.Size = new System.Drawing.Size(120, 17);
            this.chkAutoMatch.TabIndex = 4;
            this.chkAutoMatch.Text = "Attempt Auto-Match";
            this.chkAutoMatch.UseVisualStyleBackColor = true;
            this.chkAutoMatch.CheckedChanged += new System.EventHandler(this.chkAutoMatch_CheckedChanged);
            // 
            // btnFullReset
            // 
            this.btnFullReset.Location = new System.Drawing.Point(113, 512);
            this.btnFullReset.Name = "btnFullReset";
            this.btnFullReset.Size = new System.Drawing.Size(75, 23);
            this.btnFullReset.TabIndex = 5;
            this.btnFullReset.Text = "Full Reset";
            this.btnFullReset.UseVisualStyleBackColor = true;
            this.btnFullReset.Click += new System.EventHandler(this.btnFullReset_Click);
            // 
            // chkInDepth
            // 
            this.chkInDepth.AutoSize = true;
            this.chkInDepth.Location = new System.Drawing.Point(28, 375);
            this.chkInDepth.Name = "chkInDepth";
            this.chkInDepth.Size = new System.Drawing.Size(125, 17);
            this.chkInDepth.TabIndex = 6;
            this.chkInDepth.Text = "In Depth Auto-Match";
            this.chkInDepth.UseVisualStyleBackColor = true;
            // 
            // frmPlaceReplicate
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(299, 542);
            this.Controls.Add(this.chkInDepth);
            this.Controls.Add(this.btnFullReset);
            this.Controls.Add(this.chkAutoMatch);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.lstMatched);
            this.Controls.Add(this.btnSource);
            this.Controls.Add(this.btnDest);
            this.Controls.Add(this.btnPlace);
            this.Controls.Add(this.btnMatch);
            this.Controls.Add(this.lstDest);
            this.Controls.Add(this.lstSource);
            this.KeyPreview = true;
            this.Name = "frmPlaceReplicate";
            this.Text = "frmPlaceReplicate";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    #endregion

    private System.Windows.Forms.ListBox lstSource;
    private System.Windows.Forms.ListBox lstDest;
    private System.Windows.Forms.Button btnMatch;
    private System.Windows.Forms.Button btnReset;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ListBox lstMatched;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button btnDest;
    private System.Windows.Forms.Button btnPlace;
    private System.Windows.Forms.Button btnSource;
    private System.Windows.Forms.TextBox txtMessage;
    private System.Windows.Forms.CheckBox chkAutoMatch;
    private System.Windows.Forms.Button btnFullReset;
    private System.Windows.Forms.CheckBox chkInDepth;
}
