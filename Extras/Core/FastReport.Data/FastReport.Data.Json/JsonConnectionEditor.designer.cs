namespace FastReport.Data.ConnectionEditors
{
  partial class JsonConnectionEditor
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
            this.gbSelect = new System.Windows.Forms.GroupBox();
            this.tbJsonPath = new FastReport.Controls.TextBoxButton();
            this.lblJsonPath = new System.Windows.Forms.Label();
            this.gbSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSelect
            // 
            this.gbSelect.Controls.Add(this.lblJsonPath);
            this.gbSelect.Controls.Add(this.tbJsonPath);
            this.gbSelect.Location = new System.Drawing.Point(8, 4);
            this.gbSelect.Name = "gbSelect";
            this.gbSelect.Size = new System.Drawing.Size(320, 77);
            this.gbSelect.TabIndex = 1;
            this.gbSelect.TabStop = false;
            this.gbSelect.Text = "Select database file";
            // 
            // tbJsonPath
            // 
            this.tbJsonPath.Image = null;
            this.tbJsonPath.Location = new System.Drawing.Point(12, 40);
            this.tbJsonPath.Name = "tbJsonPath";
            this.tbJsonPath.Size = new System.Drawing.Size(296, 21);
            this.tbJsonPath.TabIndex = 2;
            this.tbJsonPath.ButtonClick += new System.EventHandler(this.tbFile_ButtonClick);
            // 
            // lblJsonPath
            // 
            this.lblJsonPath.AutoSize = true;
            this.lblJsonPath.Location = new System.Drawing.Point(12, 20);
            this.lblJsonPath.Name = "lblJsonPath";
            this.lblJsonPath.Size = new System.Drawing.Size(78, 13);
            this.lblJsonPath.TabIndex = 3;
            this.lblJsonPath.Text = "Json file or url:";
            // 
            // JsonConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.Controls.Add(this.gbSelect);
            this.Name = "JsonConnectionEditor";
            this.Size = new System.Drawing.Size(336, 91);
            this.gbSelect.ResumeLayout(false);
            this.gbSelect.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox gbSelect;
    private FastReport.Controls.TextBoxButton tbJsonPath;
        private System.Windows.Forms.Label lblJsonPath;
    }
}
