namespace FastReport.Data
{
    partial class ClickHouseConnectionEditor
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
            gbServer = new System.Windows.Forms.GroupBox();
            tbPort = new System.Windows.Forms.TextBox();
            port = new System.Windows.Forms.Label();
            lblServer = new System.Windows.Forms.Label();
            tbServer = new System.Windows.Forms.TextBox();
            tbUserName = new System.Windows.Forms.TextBox();
            tbPassword = new System.Windows.Forms.TextBox();
            lblUserName = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            gbDatabase = new System.Windows.Forms.GroupBox();
            lblDatabase = new System.Windows.Forms.Label();
            tbDatabase = new System.Windows.Forms.TextBox();
            gbServer.SuspendLayout();
            gbDatabase.SuspendLayout();
            SuspendLayout();
            // 
            // gbServer
            // 
            gbServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gbServer.Controls.Add(tbPort);
            gbServer.Controls.Add(port);
            gbServer.Controls.Add(lblServer);
            gbServer.Controls.Add(tbServer);
            gbServer.Controls.Add(tbUserName);
            gbServer.Controls.Add(tbPassword);
            gbServer.Controls.Add(lblUserName);
            gbServer.Controls.Add(lblPassword);
            gbServer.Location = new System.Drawing.Point(8, 4);
            gbServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbServer.Name = "gbServer";
            gbServer.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbServer.Size = new System.Drawing.Size(320, 180);
            gbServer.TabIndex = 4;
            gbServer.TabStop = false;
            gbServer.Text = "Server";
            // 
            // tbPort
            // 
            tbPort.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbPort.Location = new System.Drawing.Point(110, 86);
            tbPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbPort.Name = "tbPort";
            tbPort.Size = new System.Drawing.Size(198, 23);
            tbPort.TabIndex = 6;
            // 
            // port
            // 
            port.AutoSize = true;
            port.Location = new System.Drawing.Point(12, 90);
            port.Name = "port";
            port.Size = new System.Drawing.Size(67, 15);
            port.TabIndex = 5;
            port.Text = "Server port:";
            // 
            // lblServer
            // 
            lblServer.AutoSize = true;
            lblServer.Location = new System.Drawing.Point(12, 23);
            lblServer.Name = "lblServer";
            lblServer.Size = new System.Drawing.Size(75, 15);
            lblServer.TabIndex = 4;
            lblServer.Text = "Server name:";
            // 
            // tbServer
            // 
            tbServer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbServer.Location = new System.Drawing.Point(12, 46);
            tbServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbServer.Name = "tbServer";
            tbServer.Size = new System.Drawing.Size(296, 23);
            tbServer.TabIndex = 0;
            // 
            // tbUserName
            // 
            tbUserName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbUserName.Location = new System.Drawing.Point(110, 116);
            tbUserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbUserName.Name = "tbUserName";
            tbUserName.Size = new System.Drawing.Size(198, 23);
            tbUserName.TabIndex = 1;
            // 
            // tbPassword
            // 
            tbPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbPassword.Location = new System.Drawing.Point(110, 144);
            tbPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbPassword.Name = "tbPassword";
            tbPassword.Size = new System.Drawing.Size(198, 23);
            tbPassword.TabIndex = 2;
            tbPassword.UseSystemPasswordChar = true;
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.Location = new System.Drawing.Point(12, 119);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new System.Drawing.Size(66, 15);
            lblUserName.TabIndex = 0;
            lblUserName.Text = "User name:";
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new System.Drawing.Point(12, 148);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(60, 15);
            lblPassword.TabIndex = 1;
            lblPassword.Text = "Password:";
            // 
            // gbDatabase
            // 
            gbDatabase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            gbDatabase.Controls.Add(lblDatabase);
            gbDatabase.Controls.Add(tbDatabase);
            gbDatabase.Location = new System.Drawing.Point(8, 192);
            gbDatabase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbDatabase.Name = "gbDatabase";
            gbDatabase.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            gbDatabase.Size = new System.Drawing.Size(320, 88);
            gbDatabase.TabIndex = 5;
            gbDatabase.TabStop = false;
            gbDatabase.Text = "Database";
            // 
            // lblDatabase
            // 
            lblDatabase.AutoSize = true;
            lblDatabase.Location = new System.Drawing.Point(12, 23);
            lblDatabase.Name = "lblDatabase";
            lblDatabase.Size = new System.Drawing.Size(58, 15);
            lblDatabase.TabIndex = 3;
            lblDatabase.Text = "Database:";
            // 
            // tbDatabase
            // 
            tbDatabase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            tbDatabase.Location = new System.Drawing.Point(12, 46);
            tbDatabase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            tbDatabase.Name = "tbDatabase";
            tbDatabase.Size = new System.Drawing.Size(296, 23);
            tbDatabase.TabIndex = 0;
            // 
            // ClickHouseConnectionEditor
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            Controls.Add(gbDatabase);
            Controls.Add(gbServer);
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            MinimumSize = new System.Drawing.Size(0, 0);
            Name = "ClickHouseConnectionEditor";
            Size = new System.Drawing.Size(336, 290);
            gbServer.ResumeLayout(false);
            gbServer.PerformLayout();
            gbDatabase.ResumeLayout(false);
            gbDatabase.PerformLayout();
            ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.TextBox tbServer;
        private System.Windows.Forms.TextBox tbUserName;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.Label lblUserName;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.GroupBox gbDatabase;
        private System.Windows.Forms.Label lblDatabase;
        private System.Windows.Forms.TextBox tbDatabase;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.Label port;
    }
}
