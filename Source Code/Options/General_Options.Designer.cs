
partial class General_Options
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
            this.chkBatchRefHide = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtEmbededResLayer = new System.Windows.Forms.TextBox();
            this.txtFromAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtToAddress = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSMTPHost = new System.Windows.Forms.TextBox();
            this.chkErrorEnable = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtLayerCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtExtFileConfig = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.BackColor = System.Drawing.SystemColors.Control;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(2, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(371, 19);
            this.label1.TabIndex = 5;
            this.label1.Text = "Batch Outjob";
            // 
            // chkBatchRefHide
            // 
            this.chkBatchRefHide.AutoSize = true;
            this.chkBatchRefHide.Checked = true;
            this.chkBatchRefHide.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkBatchRefHide.Location = new System.Drawing.Point(9, 50);
            this.chkBatchRefHide.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkBatchRefHide.Name = "chkBatchRefHide";
            this.chkBatchRefHide.Size = new System.Drawing.Size(263, 17);
            this.chkBatchRefHide.TabIndex = 3;
            this.chkBatchRefHide.Text = "Hide Refdes values when generating ODB++ files.";
            this.chkBatchRefHide.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.SystemColors.Control;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(2, 97);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(371, 19);
            this.label2.TabIndex = 4;
            this.label2.Text = "Embeded Resistor Layer Mapping";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 143);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "First Mechanical Layer";
            // 
            // txtEmbededResLayer
            // 
            this.txtEmbededResLayer.Location = new System.Drawing.Point(130, 141);
            this.txtEmbededResLayer.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtEmbededResLayer.Name = "txtEmbededResLayer";
            this.txtEmbededResLayer.Size = new System.Drawing.Size(36, 20);
            this.txtEmbededResLayer.TabIndex = 0;
            this.txtEmbededResLayer.Text = "20";
            // 
            // txtFromAddress
            // 
            this.txtFromAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFromAddress.Location = new System.Drawing.Point(124, 272);
            this.txtFromAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtFromAddress.Name = "txtFromAddress";
            this.txtFromAddress.Size = new System.Drawing.Size(242, 20);
            this.txtFromAddress.TabIndex = 2;
            this.txtFromAddress.Text = "SwRI_Error_Report@swri.org";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 275);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "From Address";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.BackColor = System.Drawing.SystemColors.Control;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(2, 207);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(371, 20);
            this.label5.TabIndex = 7;
            this.label5.Text = "Email Error Reporting";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 297);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "To Address";
            // 
            // txtToAddress
            // 
            this.txtToAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToAddress.Location = new System.Drawing.Point(124, 295);
            this.txtToAddress.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtToAddress.Name = "txtToAddress";
            this.txtToAddress.Size = new System.Drawing.Size(242, 20);
            this.txtToAddress.TabIndex = 3;
            this.txtToAddress.Text = "rlyne@swri.org";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 320);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "SMTP Host";
            // 
            // txtSMTPHost
            // 
            this.txtSMTPHost.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSMTPHost.Location = new System.Drawing.Point(124, 318);
            this.txtSMTPHost.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtSMTPHost.Name = "txtSMTPHost";
            this.txtSMTPHost.Size = new System.Drawing.Size(242, 20);
            this.txtSMTPHost.TabIndex = 4;
            this.txtSMTPHost.Text = "smtp.swri.org";
            // 
            // chkErrorEnable
            // 
            this.chkErrorEnable.AutoSize = true;
            this.chkErrorEnable.Checked = true;
            this.chkErrorEnable.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkErrorEnable.Location = new System.Drawing.Point(9, 250);
            this.chkErrorEnable.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.chkErrorEnable.Name = "chkErrorEnable";
            this.chkErrorEnable.Size = new System.Drawing.Size(133, 17);
            this.chkErrorEnable.TabIndex = 3;
            this.chkErrorEnable.Text = "Enable Error Reporting";
            this.chkErrorEnable.UseVisualStyleBackColor = true;
            this.chkErrorEnable.CheckedChanged += new System.EventHandler(this.chkErrorEnable_CheckedChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 166);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(118, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "Number of Layers Used";
            // 
            // txtLayerCount
            // 
            this.txtLayerCount.Location = new System.Drawing.Point(130, 163);
            this.txtLayerCount.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtLayerCount.Name = "txtLayerCount";
            this.txtLayerCount.Size = new System.Drawing.Size(36, 20);
            this.txtLayerCount.TabIndex = 1;
            this.txtLayerCount.Text = "5";
            // 
            // label9
            // 
            this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label9.BackColor = System.Drawing.SystemColors.Control;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(2, 349);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(371, 20);
            this.label9.TabIndex = 7;
            this.label9.Text = "External File Config Path";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 379);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(65, 13);
            this.label10.TabIndex = 8;
            this.label10.Text = "INI File Path";
            // 
            // txtExtFileConfig
            // 
            this.txtExtFileConfig.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExtFileConfig.Location = new System.Drawing.Point(124, 377);
            this.txtExtFileConfig.Margin = new System.Windows.Forms.Padding(2);
            this.txtExtFileConfig.Name = "txtExtFileConfig";
            this.txtExtFileConfig.Size = new System.Drawing.Size(242, 20);
            this.txtExtFileConfig.TabIndex = 4;
            this.txtExtFileConfig.Text = "G:\\CADTOOLS\\Software\\Altium\\SwRI Software\\Altium Extensions\\SwRI_Tools Paths.ini";
            // 
            // General_Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(374, 418);
            this.Controls.Add(this.txtExtFileConfig);
            this.Controls.Add(this.txtSMTPHost);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtToAddress);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtFromAddress);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtLayerCount);
            this.Controls.Add(this.txtEmbededResLayer);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chkErrorEnable);
            this.Controls.Add(this.chkBatchRefHide);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "General_Options";
            this.Text = "General_Options";
            this.ResumeLayout(false);
            this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.CheckBox chkBatchRefHide;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txtEmbededResLayer;
    private System.Windows.Forms.TextBox txtFromAddress;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txtToAddress;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox txtSMTPHost;
    private System.Windows.Forms.CheckBox chkErrorEnable;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox txtLayerCount;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TextBox txtExtFileConfig;
}
