namespace FastReport.Data
{
	partial class GoogleAuthConfigurationDialog
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
            this.cbxPathToJsonFile = new System.Windows.Forms.CheckBox();
            this.tbPathToJsonFile = new FastReport.Controls.TextBoxButton();
            this.lblPathToJsonFile = new System.Windows.Forms.Label();
            this.lblClientSecret = new System.Windows.Forms.Label();
            this.tbClientSecret = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.tbClientId = new System.Windows.Forms.TextBox();
            this.gbxOAuth = new System.Windows.Forms.GroupBox();
            this.gbxApiKey = new System.Windows.Forms.GroupBox();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.tbApiKey = new System.Windows.Forms.TextBox();
            this.cbxAuthMode = new System.Windows.Forms.ComboBox();
            this.lblCaption = new System.Windows.Forms.Label();
            this.gbxOAuth.SuspendLayout();
            this.gbxApiKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.None;
            this.btnOk.Location = new System.Drawing.Point(122, 189);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(204, 189);
            // 
            // cbxPathToJsonFile
            // 
            this.cbxPathToJsonFile.AutoSize = true;
            this.cbxPathToJsonFile.Location = new System.Drawing.Point(17, 101);
            this.cbxPathToJsonFile.Name = "cbxPathToJsonFile";
            this.cbxPathToJsonFile.Size = new System.Drawing.Size(99, 17);
            this.cbxPathToJsonFile.TabIndex = 17;
            this.cbxPathToJsonFile.Text = "Check .json file";
            this.cbxPathToJsonFile.UseVisualStyleBackColor = true;
            this.cbxPathToJsonFile.CheckedChanged += new System.EventHandler(this.cbxPathToJsonFile_CheckedChanged);
            // 
            // tbPathToJsonFile
            // 
            this.tbPathToJsonFile.ButtonText = "";
            this.tbPathToJsonFile.Image = null;
            this.tbPathToJsonFile.Location = new System.Drawing.Point(18, 31);
            this.tbPathToJsonFile.Name = "tbPathToJsonFile";
            this.tbPathToJsonFile.Size = new System.Drawing.Size(233, 21);
            this.tbPathToJsonFile.TabIndex = 16;
            this.tbPathToJsonFile.Visible = false;
            this.tbPathToJsonFile.ButtonClick += new System.EventHandler(this.tbPathToJsonFile_ButtonClick);
            // 
            // lblPathToJsonFile
            // 
            this.lblPathToJsonFile.AutoSize = true;
            this.lblPathToJsonFile.Location = new System.Drawing.Point(14, 16);
            this.lblPathToJsonFile.Name = "lblPathToJsonFile";
            this.lblPathToJsonFile.Size = new System.Drawing.Size(69, 13);
            this.lblPathToJsonFile.TabIndex = 14;
            this.lblPathToJsonFile.Text = "Path to .json";
            this.lblPathToJsonFile.Visible = false;
            // 
            // lblClientSecret
            // 
            this.lblClientSecret.AutoSize = true;
            this.lblClientSecret.Location = new System.Drawing.Point(15, 59);
            this.lblClientSecret.Name = "lblClientSecret";
            this.lblClientSecret.Size = new System.Drawing.Size(68, 13);
            this.lblClientSecret.TabIndex = 12;
            this.lblClientSecret.Text = "Client Secret";
            // 
            // tbClientSecret
            // 
            this.tbClientSecret.Location = new System.Drawing.Point(17, 75);
            this.tbClientSecret.Name = "tbClientSecret";
            this.tbClientSecret.Size = new System.Drawing.Size(233, 21);
            this.tbClientSecret.TabIndex = 13;
            this.tbClientSecret.UseSystemPasswordChar = true;
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(15, 16);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(48, 13);
            this.lblClientId.TabIndex = 10;
            this.lblClientId.Text = "Client ID";
            // 
            // tbClientId
            // 
            this.tbClientId.Location = new System.Drawing.Point(17, 31);
            this.tbClientId.Name = "tbClientId";
            this.tbClientId.Size = new System.Drawing.Size(233, 21);
            this.tbClientId.TabIndex = 11;
            this.tbClientId.UseSystemPasswordChar = true;
            // 
            // gbxOAuth
            // 
            this.gbxOAuth.Controls.Add(this.lblClientId);
            this.gbxOAuth.Controls.Add(this.cbxPathToJsonFile);
            this.gbxOAuth.Controls.Add(this.tbClientId);
            this.gbxOAuth.Controls.Add(this.tbClientSecret);
            this.gbxOAuth.Controls.Add(this.tbPathToJsonFile);
            this.gbxOAuth.Controls.Add(this.lblClientSecret);
            this.gbxOAuth.Controls.Add(this.lblPathToJsonFile);
            this.gbxOAuth.Enabled = false;
            this.gbxOAuth.Location = new System.Drawing.Point(12, 52);
            this.gbxOAuth.Name = "gbxOAuth";
            this.gbxOAuth.Size = new System.Drawing.Size(265, 122);
            this.gbxOAuth.TabIndex = 19;
            this.gbxOAuth.TabStop = false;
            this.gbxOAuth.Visible = false;
            // 
            // gbxApiKey
            // 
            this.gbxApiKey.Controls.Add(this.lblApiKey);
            this.gbxApiKey.Controls.Add(this.tbApiKey);
            this.gbxApiKey.Enabled = false;
            this.gbxApiKey.Location = new System.Drawing.Point(12, 52);
            this.gbxApiKey.Name = "gbxApiKey";
            this.gbxApiKey.Size = new System.Drawing.Size(265, 122);
            this.gbxApiKey.TabIndex = 22;
            this.gbxApiKey.TabStop = false;
            this.gbxApiKey.Visible = false;
            // 
            // lblApiKey
            // 
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(15, 16);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(44, 13);
            this.lblApiKey.TabIndex = 10;
            this.lblApiKey.Text = "API key";
            // 
            // tbApiKey
            // 
            this.tbApiKey.Location = new System.Drawing.Point(18, 36);
            this.tbApiKey.Name = "tbApiKey";
            this.tbApiKey.Size = new System.Drawing.Size(233, 21);
            this.tbApiKey.TabIndex = 11;
            this.tbApiKey.UseSystemPasswordChar = true;
            // 
            // cbxAuthMode
            // 
            this.cbxAuthMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxAuthMode.FormattingEnabled = true;
            this.cbxAuthMode.Location = new System.Drawing.Point(12, 25);
            this.cbxAuthMode.Name = "cbxAuthMode";
            this.cbxAuthMode.Size = new System.Drawing.Size(117, 21);
            this.cbxAuthMode.TabIndex = 18;
            this.cbxAuthMode.SelectedIndexChanged += new System.EventHandler(this.cbxAuthMode_SelectedIndexChanged);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(12, 9);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(106, 13);
            this.lblCaption.TabIndex = 9;
            this.lblCaption.Text = "Authentication mode";
            // 
            // GoogleAuthConfigurationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(291, 228);
            this.Controls.Add(this.gbxOAuth);
            this.Controls.Add(this.cbxAuthMode);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.gbxApiKey);
            this.Name = "GoogleAuthConfigurationDialog";
            this.Text = "Sign in Google Sheets";
            this.Controls.SetChildIndex(this.gbxApiKey, 0);
            this.Controls.SetChildIndex(this.lblCaption, 0);
            this.Controls.SetChildIndex(this.cbxAuthMode, 0);
            this.Controls.SetChildIndex(this.gbxOAuth, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.gbxOAuth.ResumeLayout(false);
            this.gbxOAuth.PerformLayout();
            this.gbxApiKey.ResumeLayout(false);
            this.gbxApiKey.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox cbxPathToJsonFile;
		private Controls.TextBoxButton tbPathToJsonFile;
		private System.Windows.Forms.Label lblPathToJsonFile;
		private System.Windows.Forms.Label lblClientSecret;
		private System.Windows.Forms.TextBox tbClientSecret;
		private System.Windows.Forms.Label lblClientId;
		private System.Windows.Forms.TextBox tbClientId;
		private System.Windows.Forms.GroupBox gbxOAuth;
		private System.Windows.Forms.ComboBox cbxAuthMode;
		private System.Windows.Forms.GroupBox gbxApiKey;
		private System.Windows.Forms.Label lblApiKey;
		private System.Windows.Forms.TextBox tbApiKey;
        private System.Windows.Forms.Label lblCaption;
    }
}
