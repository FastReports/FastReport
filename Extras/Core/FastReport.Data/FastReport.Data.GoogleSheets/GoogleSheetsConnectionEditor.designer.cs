namespace FastReport.Data
{
	partial class GoogleSheetsConnectionEditor
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
            this.gbConnectionProperties = new System.Windows.Forms.GroupBox();
            this.cbxIncludeHidden = new System.Windows.Forms.CheckBox();
            this.btnConfigureAuth = new System.Windows.Forms.Button();
            this.cbxFieldNames = new System.Windows.Forms.CheckBox();
            this.tbSpreadsheetId = new System.Windows.Forms.TextBox();
            this.lblSpreadsheetId = new System.Windows.Forms.Label();
            this.gbConnectionProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbConnectionProperties
            // 
            this.gbConnectionProperties.Controls.Add(this.cbxIncludeHidden);
            this.gbConnectionProperties.Controls.Add(this.btnConfigureAuth);
            this.gbConnectionProperties.Controls.Add(this.cbxFieldNames);
            this.gbConnectionProperties.Controls.Add(this.tbSpreadsheetId);
            this.gbConnectionProperties.Controls.Add(this.lblSpreadsheetId);
            this.gbConnectionProperties.Location = new System.Drawing.Point(8, 4);
            this.gbConnectionProperties.Name = "gbConnectionProperties";
            this.gbConnectionProperties.Size = new System.Drawing.Size(320, 135);
            this.gbConnectionProperties.TabIndex = 1;
            this.gbConnectionProperties.TabStop = false;
            this.gbConnectionProperties.Text = "Database";
            // 
            // cbxIncludeHidden
            // 
            this.cbxIncludeHidden.AutoSize = true;
            this.cbxIncludeHidden.Location = new System.Drawing.Point(15, 88);
            this.cbxIncludeHidden.Name = "cbxIncludeHidden";
            this.cbxIncludeHidden.Size = new System.Drawing.Size(130, 17);
            this.cbxIncludeHidden.TabIndex = 9;
            this.cbxIncludeHidden.Text = "Include hidden sheets";
            this.cbxIncludeHidden.UseVisualStyleBackColor = true;
            // 
            // btnConfigureAuth
            // 
            this.btnConfigureAuth.Location = new System.Drawing.Point(239, 106);
            this.btnConfigureAuth.Name = "btnConfigureAuth";
            this.btnConfigureAuth.Size = new System.Drawing.Size(75, 23);
            this.btnConfigureAuth.TabIndex = 8;
            this.btnConfigureAuth.Text = "Sign in";
            this.btnConfigureAuth.UseVisualStyleBackColor = true;
            this.btnConfigureAuth.Click += new System.EventHandler(this.btnConfigureAuth_Click);
            // 
            // cbxFieldNames
            // 
            this.cbxFieldNames.AutoSize = true;
            this.cbxFieldNames.Location = new System.Drawing.Point(15, 64);
            this.cbxFieldNames.Name = "cbxFieldNames";
            this.cbxFieldNames.Size = new System.Drawing.Size(200, 17);
            this.cbxFieldNames.TabIndex = 7;
            this.cbxFieldNames.Text = "Use first data row as column headers";
            this.cbxFieldNames.UseVisualStyleBackColor = true;
            // 
            // tbSpreadsheetId
            // 
            this.tbSpreadsheetId.Location = new System.Drawing.Point(15, 36);
            this.tbSpreadsheetId.Name = "tbSpreadsheetId";
            this.tbSpreadsheetId.Size = new System.Drawing.Size(299, 20);
            this.tbSpreadsheetId.TabIndex = 1;
            // 
            // lblSpreadsheetId
            // 
            this.lblSpreadsheetId.AutoSize = true;
            this.lblSpreadsheetId.Location = new System.Drawing.Point(12, 20);
            this.lblSpreadsheetId.Name = "lblSpreadsheetId";
            this.lblSpreadsheetId.Size = new System.Drawing.Size(121, 13);
            this.lblSpreadsheetId.TabIndex = 0;
            this.lblSpreadsheetId.Text = "Spreadsheet ID or URL:";
            // 
            // GoogleSheetsConnectionEditor
            // 
            this.Controls.Add(this.gbConnectionProperties);
            this.Name = "GoogleSheetsConnectionEditor";
            this.Size = new System.Drawing.Size(336, 145);
            this.gbConnectionProperties.ResumeLayout(false);
            this.gbConnectionProperties.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbConnectionProperties;
		private System.Windows.Forms.Label lblSpreadsheetId;
		private System.Windows.Forms.TextBox tbSpreadsheetId;
		private System.Windows.Forms.CheckBox cbxFieldNames;
		private System.Windows.Forms.Button btnConfigureAuth;
        private System.Windows.Forms.CheckBox cbxIncludeHidden;
    }
}