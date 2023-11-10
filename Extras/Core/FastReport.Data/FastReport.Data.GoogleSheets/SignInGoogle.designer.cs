namespace FastReport.Data
{
	partial class SignInGoogle
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
            this.lblPath = new System.Windows.Forms.Label();
            this.lblClientSecret = new System.Windows.Forms.Label();
            this.tbClientSecret = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.tbClientId = new System.Windows.Forms.TextBox();
            this.lblCaption = new System.Windows.Forms.Label();
            this.btnSignIn = new System.Windows.Forms.Button();
            this.gbxSignInGoogleAPI = new System.Windows.Forms.GroupBox();
            this.gbxGoogleApiKey = new System.Windows.Forms.GroupBox();
            this.lblApiKey = new System.Windows.Forms.Label();
            this.tbApiKey = new System.Windows.Forms.TextBox();
            this.cbxChangeSignIn = new System.Windows.Forms.ComboBox();
            this.gbxSignInGoogleAPI.SuspendLayout();
            this.gbxGoogleApiKey.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(151, 315);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Size = new System.Drawing.Size(94, 29);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(252, 315);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Size = new System.Drawing.Size(94, 29);
            // 
            // cbxPathToJsonFile
            // 
            this.cbxPathToJsonFile.AutoSize = true;
            this.cbxPathToJsonFile.Location = new System.Drawing.Point(21, 126);
            this.cbxPathToJsonFile.Margin = new System.Windows.Forms.Padding(4);
            this.cbxPathToJsonFile.Name = "cbxPathToJsonFile";
            this.cbxPathToJsonFile.Size = new System.Drawing.Size(121, 21);
            this.cbxPathToJsonFile.TabIndex = 17;
            this.cbxPathToJsonFile.Text = "Check .json file";
            this.cbxPathToJsonFile.UseVisualStyleBackColor = true;
            this.cbxPathToJsonFile.CheckedChanged += new System.EventHandler(this.cbxPathToJsonFile_CheckedChanged);
            // 
            // tbPathToJsonFile
            // 
            this.tbPathToJsonFile.ButtonText = "";
            this.tbPathToJsonFile.Image = null;
            this.tbPathToJsonFile.Location = new System.Drawing.Point(22, 39);
            this.tbPathToJsonFile.Margin = new System.Windows.Forms.Padding(4);
            this.tbPathToJsonFile.Name = "tbPathToJsonFile";
            this.tbPathToJsonFile.Size = new System.Drawing.Size(291, 26);
            this.tbPathToJsonFile.TabIndex = 16;
            this.tbPathToJsonFile.Visible = false;
            this.tbPathToJsonFile.ButtonClick += new System.EventHandler(this.tbPathToJsonFile_ButtonClick);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(18, 20);
            this.lblPath.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(87, 17);
            this.lblPath.TabIndex = 14;
            this.lblPath.Text = "Path to .json";
            this.lblPath.Visible = false;
            // 
            // lblClientSecret
            // 
            this.lblClientSecret.AutoSize = true;
            this.lblClientSecret.Location = new System.Drawing.Point(19, 74);
            this.lblClientSecret.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClientSecret.Name = "lblClientSecret";
            this.lblClientSecret.Size = new System.Drawing.Size(84, 17);
            this.lblClientSecret.TabIndex = 12;
            this.lblClientSecret.Text = "Client Secret";
            // 
            // tbClientSecret
            // 
            this.tbClientSecret.Location = new System.Drawing.Point(22, 94);
            this.tbClientSecret.Margin = new System.Windows.Forms.Padding(4);
            this.tbClientSecret.Name = "tbClientSecret";
            this.tbClientSecret.Size = new System.Drawing.Size(290, 24);
            this.tbClientSecret.TabIndex = 13;
            this.tbClientSecret.UseSystemPasswordChar = true;
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(19, 20);
            this.lblClientId.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(59, 17);
            this.lblClientId.TabIndex = 10;
            this.lblClientId.Text = "Client ID";
            // 
            // tbClientId
            // 
            this.tbClientId.Location = new System.Drawing.Point(22, 39);
            this.tbClientId.Margin = new System.Windows.Forms.Padding(4);
            this.tbClientId.Name = "tbClientId";
            this.tbClientId.Size = new System.Drawing.Size(290, 24);
            this.tbClientId.TabIndex = 11;
            this.tbClientId.UseSystemPasswordChar = true;
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Font = new System.Drawing.Font("Calibri Light", 19.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCaption.Location = new System.Drawing.Point(15, 11);
            this.lblCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(253, 40);
            this.lblCaption.TabIndex = 9;
            this.lblCaption.Text = "Sign in Google API";
            // 
            // btnSignIn
            // 
            this.btnSignIn.Location = new System.Drawing.Point(220, 158);
            this.btnSignIn.Margin = new System.Windows.Forms.Padding(4);
            this.btnSignIn.Name = "btnSignIn";
            this.btnSignIn.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.btnSignIn.Size = new System.Drawing.Size(94, 29);
            this.btnSignIn.TabIndex = 0;
            this.btnSignIn.Text = "Sign In";
            this.btnSignIn.UseVisualStyleBackColor = true;
            this.btnSignIn.Click += new System.EventHandler(this.btnSignIn_Click);
            // 
            // gbxSignInGoogleAPI
            // 
            this.gbxSignInGoogleAPI.Controls.Add(this.btnSignIn);
            this.gbxSignInGoogleAPI.Controls.Add(this.lblClientId);
            this.gbxSignInGoogleAPI.Controls.Add(this.cbxPathToJsonFile);
            this.gbxSignInGoogleAPI.Controls.Add(this.tbClientId);
            this.gbxSignInGoogleAPI.Controls.Add(this.tbClientSecret);
            this.gbxSignInGoogleAPI.Controls.Add(this.tbPathToJsonFile);
            this.gbxSignInGoogleAPI.Controls.Add(this.lblClientSecret);
            this.gbxSignInGoogleAPI.Controls.Add(this.lblPath);
            this.gbxSignInGoogleAPI.Location = new System.Drawing.Point(15, 114);
            this.gbxSignInGoogleAPI.Margin = new System.Windows.Forms.Padding(4);
            this.gbxSignInGoogleAPI.Name = "gbxSignInGoogleAPI";
            this.gbxSignInGoogleAPI.Padding = new System.Windows.Forms.Padding(4);
            this.gbxSignInGoogleAPI.Size = new System.Drawing.Size(331, 194);
            this.gbxSignInGoogleAPI.TabIndex = 19;
            this.gbxSignInGoogleAPI.TabStop = false;
            this.gbxSignInGoogleAPI.Text = "Sign in Google API with OAuth 2.0";
            // 
            // gbxGoogleApiKey
            // 
            this.gbxGoogleApiKey.Controls.Add(this.lblApiKey);
            this.gbxGoogleApiKey.Controls.Add(this.tbApiKey);
            this.gbxGoogleApiKey.Enabled = false;
            this.gbxGoogleApiKey.Location = new System.Drawing.Point(15, 114);
            this.gbxGoogleApiKey.Margin = new System.Windows.Forms.Padding(4);
            this.gbxGoogleApiKey.Name = "gbxGoogleApiKey";
            this.gbxGoogleApiKey.Padding = new System.Windows.Forms.Padding(4);
            this.gbxGoogleApiKey.Size = new System.Drawing.Size(331, 152);
            this.gbxGoogleApiKey.TabIndex = 22;
            this.gbxGoogleApiKey.TabStop = false;
            this.gbxGoogleApiKey.Text = "Sign in Google API with API key";
            this.gbxGoogleApiKey.Visible = false;
            // 
            // lblApiKey
            // 
            this.lblApiKey.AutoSize = true;
            this.lblApiKey.Location = new System.Drawing.Point(19, 20);
            this.lblApiKey.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblApiKey.Name = "lblApiKey";
            this.lblApiKey.Size = new System.Drawing.Size(54, 17);
            this.lblApiKey.TabIndex = 10;
            this.lblApiKey.Text = "API key";
            // 
            // tbApiKey
            // 
            this.tbApiKey.Location = new System.Drawing.Point(22, 45);
            this.tbApiKey.Margin = new System.Windows.Forms.Padding(4);
            this.tbApiKey.Name = "tbApiKey";
            this.tbApiKey.Size = new System.Drawing.Size(290, 24);
            this.tbApiKey.TabIndex = 11;
            this.tbApiKey.UseSystemPasswordChar = true;
            // 
            // cbxChangeSignIn
            // 
            this.cbxChangeSignIn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxChangeSignIn.FormattingEnabled = true;
            this.cbxChangeSignIn.Location = new System.Drawing.Point(15, 68);
            this.cbxChangeSignIn.Margin = new System.Windows.Forms.Padding(4);
            this.cbxChangeSignIn.Name = "cbxChangeSignIn";
            this.cbxChangeSignIn.Size = new System.Drawing.Size(145, 24);
            this.cbxChangeSignIn.TabIndex = 18;
            this.cbxChangeSignIn.SelectedIndexChanged += new System.EventHandler(this.cbxChangeSignIn_SelectedIndexChanged);
            // 
            // SignInGoogle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(364, 359);
            this.Controls.Add(this.gbxSignInGoogleAPI);
            this.Controls.Add(this.cbxChangeSignIn);
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.gbxGoogleApiKey);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "SignInGoogle";
            this.Controls.SetChildIndex(this.gbxGoogleApiKey, 0);
            this.Controls.SetChildIndex(this.lblCaption, 0);
            this.Controls.SetChildIndex(this.cbxChangeSignIn, 0);
            this.Controls.SetChildIndex(this.gbxSignInGoogleAPI, 0);
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.gbxSignInGoogleAPI.ResumeLayout(false);
            this.gbxSignInGoogleAPI.PerformLayout();
            this.gbxGoogleApiKey.ResumeLayout(false);
            this.gbxGoogleApiKey.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.CheckBox cbxPathToJsonFile;
		private Controls.TextBoxButton tbPathToJsonFile;
		private System.Windows.Forms.Label lblPath;
		private System.Windows.Forms.Label lblClientSecret;
		private System.Windows.Forms.TextBox tbClientSecret;
		private System.Windows.Forms.Label lblClientId;
		private System.Windows.Forms.TextBox tbClientId;
		private System.Windows.Forms.Label lblCaption;
		private System.Windows.Forms.Button btnSignIn;
		private System.Windows.Forms.GroupBox gbxSignInGoogleAPI;
		private System.Windows.Forms.ComboBox cbxChangeSignIn;
		private System.Windows.Forms.GroupBox gbxGoogleApiKey;
		private System.Windows.Forms.Label lblApiKey;
		private System.Windows.Forms.TextBox tbApiKey;
	}
}
