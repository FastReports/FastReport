using FastReport.Controls;

namespace FastReport.Data.ConnectionEditors
{
    partial class OdbcConnectionEditor
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
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.gbDatasource = new System.Windows.Forms.GroupBox();
            this.tbConnectionString = new FastReport.Controls.TextBoxButton();
            this.rbConnectionString = new System.Windows.Forms.RadioButton();
            this.cbxDsName = new System.Windows.Forms.ComboBox();
            this.rbDsName = new System.Windows.Forms.RadioButton();
            this.gbLogin = new System.Windows.Forms.GroupBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUserName = new System.Windows.Forms.Label();
            this.gbDatasource.SuspendLayout();
            this.gbLogin.SuspendLayout();
            this.SuspendLayout();
            //
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdvanced.Location = new System.Drawing.Point(234, 70);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(75, 23);
            this.btnAdvanced.TabIndex = 6;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // gbDatasource
            // 
            this.gbDatasource.Controls.Add(this.tbConnectionString);
            this.gbDatasource.Controls.Add(this.rbConnectionString);
            this.gbDatasource.Controls.Add(this.cbxDsName);
            this.gbDatasource.Controls.Add(this.rbDsName);
            this.gbDatasource.Location = new System.Drawing.Point(8, 4);
            this.gbDatasource.Name = "gbDatasource";
            this.gbDatasource.Size = new System.Drawing.Size(320, 128);
            this.gbDatasource.TabIndex = 0;
            this.gbDatasource.TabStop = false;
            this.gbDatasource.Text = "Data source";
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.ButtonText = "";
            this.tbConnectionString.Image = null;
            this.tbConnectionString.Location = new System.Drawing.Point(32, 92);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.Size = new System.Drawing.Size(276, 21);
            this.tbConnectionString.TabIndex = 3;
            this.tbConnectionString.ButtonClick += new System.EventHandler(this.tbConnectionString_ButtonClick);
            // 
            // rbConnectionString
            // 
            this.rbConnectionString.AutoSize = true;
            this.rbConnectionString.Location = new System.Drawing.Point(12, 72);
            this.rbConnectionString.Name = "rbConnectionString";
            this.rbConnectionString.Size = new System.Drawing.Size(131, 17);
            this.rbConnectionString.TabIndex = 1;
            this.rbConnectionString.TabStop = true;
            this.rbConnectionString.Text = "Use connection string:";
            this.rbConnectionString.UseVisualStyleBackColor = true;
            this.rbConnectionString.CheckedChanged += new System.EventHandler(this.rbDsName_CheckedChanged);
            // 
            // cbxDsName
            // 
            this.cbxDsName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxDsName.FormattingEnabled = true;
            this.cbxDsName.Location = new System.Drawing.Point(32, 40);
            this.cbxDsName.Name = "cbxDsName";
            this.cbxDsName.Size = new System.Drawing.Size(276, 21);
            this.cbxDsName.TabIndex = 2;
            this.cbxDsName.SelectedIndexChanged += new System.EventHandler(this.cbxDsName_SelectedIndexChanged);
            // 
            // rbDsName
            // 
            this.rbDsName.AutoSize = true;
            this.rbDsName.Location = new System.Drawing.Point(12, 20);
            this.rbDsName.Name = "rbDsName";
            this.rbDsName.Size = new System.Drawing.Size(205, 17);
            this.rbDsName.TabIndex = 0;
            this.rbDsName.TabStop = true;
            this.rbDsName.Text = "Use user or system data source name:";
            this.rbDsName.UseVisualStyleBackColor = true;
            this.rbDsName.CheckedChanged += new System.EventHandler(this.rbDsName_CheckedChanged);
            // 
            // gbLogin
            // 
            this.gbLogin.Controls.Add(this.tbPassword);
            this.gbLogin.Controls.Add(this.tbUserName);
            this.gbLogin.Controls.Add(this.lblPassword);
            this.gbLogin.Controls.Add(this.lblUserName);
            this.gbLogin.Controls.Add(this.btnAdvanced);
            this.gbLogin.Location = new System.Drawing.Point(8, 136);
            this.gbLogin.Name = "gbLogin";
            this.gbLogin.Size = new System.Drawing.Size(320, 103);
            this.gbLogin.TabIndex = 1;
            this.gbLogin.TabStop = false;
            this.gbLogin.Text = "Login";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 44);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(188, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(120, 20);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(188, 20);
            this.tbUserName.TabIndex = 4;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 48);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(56, 13);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Password:";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 24);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(61, 13);
            this.lblUserName.TabIndex = 1;
            this.lblUserName.Text = "User name:";
            // 
            // OdbcConnectionEditor
            // 
            //this.Controls.Add(this.btnAdvanced);
            this.Controls.Add(this.gbDatasource);
            this.Controls.Add(this.gbLogin);
            this.Name = "OdbcConnectionEditor";
            this.Size = new System.Drawing.Size(336, 250);
            this.gbDatasource.ResumeLayout(false);
            this.gbDatasource.PerformLayout();
            this.gbLogin.ResumeLayout(false);
            this.gbLogin.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.GroupBox gbDatasource;
        private FastReport.Controls.TextBoxButton tbConnectionString;
        private System.Windows.Forms.ComboBox cbxDsName;
        private System.Windows.Forms.GroupBox gbLogin;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.RadioButton rbConnectionString;
        private System.Windows.Forms.RadioButton rbDsName;
    }
}
