namespace FastReport.Data
{
    partial class CouchbaseConnectionEditor
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
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.tbDatabase = new System.Windows.Forms.TextBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.tbHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.labelLine1 = new FastReport.Controls.LabelLine();
            this.gbDatabase.SuspendLayout();
            this.gbServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.tbDatabase);
            this.gbDatabase.Controls.Add(this.lblDatabase);
            this.gbDatabase.Location = new System.Drawing.Point(15, 12);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(308, 66);
            this.gbDatabase.TabIndex = 0;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // tbDatabase
            // 
            this.tbDatabase.Location = new System.Drawing.Point(6, 33);
            this.tbDatabase.Name = "tbDatabase";
            this.tbDatabase.Size = new System.Drawing.Size(296, 20);
            this.tbDatabase.TabIndex = 0;
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(6, 17);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(86, 13);
            this.lblDatabase.TabIndex = 0;
            this.lblDatabase.Text = "Database name:";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(114, 93);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(188, 20);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(6, 70);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(62, 13);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User name:";
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(114, 67);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(188, 20);
            this.tbUserName.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(6, 96);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 13);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password:";
            // 
            // gbServer
            // 
            this.gbServer.Controls.Add(this.tbHost);
            this.gbServer.Controls.Add(this.lblHost);
            this.gbServer.Controls.Add(this.tbPassword);
            this.gbServer.Controls.Add(this.lblUserName);
            this.gbServer.Controls.Add(this.tbUserName);
            this.gbServer.Controls.Add(this.lblPassword);
            this.gbServer.Location = new System.Drawing.Point(15, 84);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(308, 126);
            this.gbServer.TabIndex = 1;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // tbHost
            // 
            this.tbHost.Location = new System.Drawing.Point(9, 41);
            this.tbHost.Name = "tbHost";
            this.tbHost.Size = new System.Drawing.Size(293, 20);
            this.tbHost.TabIndex = 0;
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(6, 25);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(33, 13);
            this.lblHost.TabIndex = 0;
            this.lblHost.Text = "Host:";
            // 
            // labelLine1
            // 
            this.labelLine1.Location = new System.Drawing.Point(15, 216);
            this.labelLine1.Name = "labelLine1";
            this.labelLine1.Size = new System.Drawing.Size(308, 16);
            this.labelLine1.TabIndex = 0;
            // 
            // CouchbaseConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.gbServer);
            this.Controls.Add(this.labelLine1);
            this.Name = "CouchbaseConnectionEditor";
            this.Size = new System.Drawing.Size(336, 237);
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbDatabase;
        private System.Windows.Forms.TextBox tbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.TextBox tbHost;
        private System.Windows.Forms.Label lblHost;
        private Controls.LabelLine labelLine1;
    }
}
