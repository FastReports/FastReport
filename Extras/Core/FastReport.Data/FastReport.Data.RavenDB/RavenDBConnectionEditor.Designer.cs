namespace FastReport.Data
{
    partial class RavenDBConnectionEditor
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tbCertificatePath = new Controls.TextBoxButton();
            gbDatabase = new System.Windows.Forms.GroupBox();
            tbDatabase = new System.Windows.Forms.TextBox();
            lblDatabase = new System.Windows.Forms.Label();
            tbPassword = new System.Windows.Forms.TextBox();
            lblUserName = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            gbServer = new System.Windows.Forms.GroupBox();
            tbHost = new System.Windows.Forms.TextBox();
            lblHost = new System.Windows.Forms.Label();
            gbDatabase.SuspendLayout();
            gbServer.SuspendLayout();
            SuspendLayout();
            // 
            // tbCertificatePath
            // 
            tbCertificatePath.ButtonText = "";
            tbCertificatePath.Image = null;
            tbCertificatePath.Location = new System.Drawing.Point(139, 67);
            tbCertificatePath.Name = "tbCertificatePath";
            tbCertificatePath.Size = new System.Drawing.Size(169, 21);
            tbCertificatePath.TabIndex = 1;
            tbCertificatePath.ButtonClick += tbCertificatePath_ButtonClick;
            // 
            // gbDatabase
            // 
            gbDatabase.Controls.Add(tbDatabase);
            gbDatabase.Controls.Add(lblDatabase);
            gbDatabase.Location = new System.Drawing.Point(8, 12);
            gbDatabase.Name = "gbDatabase";
            gbDatabase.Size = new System.Drawing.Size(320, 66);
            gbDatabase.TabIndex = 0;
            gbDatabase.TabStop = false;
            gbDatabase.Text = "Database";
            // 
            // tbDatabase
            // 
            tbDatabase.Location = new System.Drawing.Point(12, 33);
            tbDatabase.Name = "tbDatabase";
            tbDatabase.Size = new System.Drawing.Size(296, 21);
            tbDatabase.TabIndex = 0;
            // 
            // lblDatabase
            // 
            lblDatabase.AutoSize = true;
            lblDatabase.Location = new System.Drawing.Point(12, 17);
            lblDatabase.Name = "lblDatabase";
            lblDatabase.Size = new System.Drawing.Size(91, 15);
            lblDatabase.TabIndex = 0;
            lblDatabase.Text = "Database name:";
            // 
            // tbPassword
            // 
            tbPassword.Location = new System.Drawing.Point(139, 93);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new System.Drawing.Size(169, 21);
            tbPassword.TabIndex = 2;
            tbPassword.UseSystemPasswordChar = true;
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Location = new System.Drawing.Point(12, 70);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new System.Drawing.Size(91, 15);
            lblUserName.TabIndex = 0;
            lblUserName.Text = "Certificate path:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(12, 96);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(60, 15);
            lblPassword.TabIndex = 0;
            lblPassword.Text = "Password:";
            // 
            // gbServer
            // 
            gbServer.Controls.Add(tbHost);
            gbServer.Controls.Add(lblHost);
            gbServer.Controls.Add(tbPassword);
            gbServer.Controls.Add(lblUserName);
            gbServer.Controls.Add(tbCertificatePath);
            gbServer.Controls.Add(lblPassword);
            gbServer.Location = new System.Drawing.Point(8, 84);
            gbServer.Name = "gbServer";
            gbServer.Size = new System.Drawing.Size(320, 126);
            gbServer.TabIndex = 1;
            gbServer.TabStop = false;
            gbServer.Text = "Server";
            // 
            // tbHost
            // 
            tbHost.Location = new System.Drawing.Point(12, 41);
            tbHost.Name = "tbHost";
            tbHost.Size = new System.Drawing.Size(296, 21);
            tbHost.TabIndex = 0;
            // 
            // lblHost
            // 
            lblHost.AutoSize = true;
            lblHost.Location = new System.Drawing.Point(12, 25);
            lblHost.Name = "lblHost";
            lblHost.Size = new System.Drawing.Size(35, 15);
            lblHost.TabIndex = 0;
            lblHost.Text = "Host:";
            // 
            // RavenDBConnectionEditor
            // 
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            Controls.Add(gbDatabase);
            Controls.Add(gbServer);
            Name = "RavenDBConnectionEditor";
            Size = new System.Drawing.Size(338, 220);
            gbDatabase.ResumeLayout(false);
            gbDatabase.PerformLayout();
            gbServer.ResumeLayout(false);
            gbServer.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private FastReport.Controls.TextBoxButton tbCertificatePath;
        private System.Windows.Forms.GroupBox gbDatabase;
        private System.Windows.Forms.TextBox tbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.Label lblHost;
    }
}
