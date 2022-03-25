
partial class frmEmbededResistorRpt
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
            this.dgvLayers = new System.Windows.Forms.DataGridView();
            this.colSource = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDest = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colEnable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvLayers
            // 
            this.dgvLayers.AllowUserToAddRows = false;
            this.dgvLayers.AllowUserToDeleteRows = false;
            this.dgvLayers.AllowUserToResizeRows = false;
            this.dgvLayers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLayers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dgvLayers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLayers.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSource,
            this.colDest,
            this.colEnable});
            this.dgvLayers.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvLayers.Location = new System.Drawing.Point(0, 0);
            this.dgvLayers.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.dgvLayers.MultiSelect = false;
            this.dgvLayers.Name = "dgvLayers";
            this.dgvLayers.RowTemplate.Height = 24;
            this.dgvLayers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvLayers.ShowEditingIcon = false;
            this.dgvLayers.Size = new System.Drawing.Size(331, 195);
            this.dgvLayers.TabIndex = 2;
            this.dgvLayers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_ContentClicked);
            // 
            // colSource
            // 
            this.colSource.HeaderText = "Source";
            this.colSource.Name = "colSource";
            this.colSource.ReadOnly = true;
            this.colSource.Width = 66;
            // 
            // colDest
            // 
            this.colDest.HeaderText = "Destination";
            this.colDest.Name = "colDest";
            this.colDest.ReadOnly = true;
            this.colDest.Width = 85;
            // 
            // colEnable
            // 
            this.colEnable.HeaderText = "Enable";
            this.colEnable.Name = "colEnable";
            this.colEnable.Width = 46;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(9, 200);
            this.btnOK.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(56, 19);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "Continue";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(70, 200);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(56, 19);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // frmEmbededResistorRpt
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(331, 228);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.dgvLayers);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MinimumSize = new System.Drawing.Size(269, 233);
            this.Name = "frmEmbededResistorRpt";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EmbededResistorRpt";
            ((System.ComponentModel.ISupportInitialize)(this.dgvLayers)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion
    public System.Windows.Forms.DataGridView dgvLayers;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSource;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDest;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colEnable;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnCancel;
}
