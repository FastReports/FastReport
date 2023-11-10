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
            this.gbSelect = new System.Windows.Forms.GroupBox();
            this.btSignInGoogle = new System.Windows.Forms.Button();
            this.cbxFieldNames = new System.Windows.Forms.CheckBox();
            this.tbGoogleId = new System.Windows.Forms.TextBox();
            this.lblSelectGId = new System.Windows.Forms.Label();
            this.gbSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSelect
            // 
            this.gbSelect.Controls.Add(this.btSignInGoogle);
            this.gbSelect.Controls.Add(this.cbxFieldNames);
            this.gbSelect.Controls.Add(this.tbGoogleId);
            this.gbSelect.Controls.Add(this.lblSelectGId);
            this.gbSelect.Location = new System.Drawing.Point(8, 4);
            this.gbSelect.Name = "gbSelect";
            this.gbSelect.Size = new System.Drawing.Size(320, 135);
            this.gbSelect.TabIndex = 1;
            this.gbSelect.TabStop = false;
            this.gbSelect.Text = "Select database file";
            // 
            // btSignInGoogle
            // 
            this.btSignInGoogle.Location = new System.Drawing.Point(239, 106);
            this.btSignInGoogle.Name = "btSignInGoogle";
            this.btSignInGoogle.Size = new System.Drawing.Size(75, 23);
            this.btSignInGoogle.TabIndex = 8;
            this.btSignInGoogle.Text = "Sign in";
            this.btSignInGoogle.UseVisualStyleBackColor = true;
            this.btSignInGoogle.Click += new System.EventHandler(this.btSignInGoogle_Click);
            // 
            // cbxFieldNames
            // 
            this.cbxFieldNames.AutoSize = true;
            this.cbxFieldNames.Location = new System.Drawing.Point(15, 73);
            this.cbxFieldNames.Name = "cbxFieldNames";
            this.cbxFieldNames.Size = new System.Drawing.Size(174, 20);
            this.cbxFieldNames.TabIndex = 7;
            this.cbxFieldNames.Text = "Field names in first string";
            this.cbxFieldNames.UseVisualStyleBackColor = true;
            // 
            // tbGoogleId
            // 
            this.tbGoogleId.Location = new System.Drawing.Point(15, 36);
            this.tbGoogleId.Name = "tbGoogleId";
            this.tbGoogleId.Size = new System.Drawing.Size(299, 22);
            this.tbGoogleId.TabIndex = 1;
            // 
            // lblSelectGId
            // 
            this.lblSelectGId.AutoSize = true;
            this.lblSelectGId.Location = new System.Drawing.Point(12, 20);
            this.lblSelectGId.Name = "lblSelectGId";
            this.lblSelectGId.Size = new System.Drawing.Size(102, 16);
            this.lblSelectGId.TabIndex = 0;
            this.lblSelectGId.Text = "Select sheetsId:";
            // 
            // GoogleSheetsConnectionEditor
            // 
            this.Controls.Add(this.gbSelect);
            this.Name = "GoogleSheetsConnectionEditor";
            this.Size = new System.Drawing.Size(336, 145);
            this.gbSelect.ResumeLayout(false);
            this.gbSelect.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbSelect;
		private System.Windows.Forms.Label lblSelectGId;
		private System.Windows.Forms.TextBox tbGoogleId;
		private System.Windows.Forms.CheckBox cbxFieldNames;
		private System.Windows.Forms.Button btSignInGoogle;
	}
}
