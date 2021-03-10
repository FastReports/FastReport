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
            this.label1 = new FastReport.Controls.LabelLine();
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.port = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.tbServer = new System.Windows.Forms.TextBox();
            this.tbUserName = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblUserName = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.lblDatabase = new System.Windows.Forms.Label();
            this.tbDatabase = new System.Windows.Forms.TextBox();
            this.gbServer.SuspendLayout();
            this.gbDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 17);
            this.label1.TabIndex = 2;
            // 
            // gbServer
            // 
            this.gbServer.Controls.Add(this.tbPort);
            this.gbServer.Controls.Add(this.port);
            this.gbServer.Controls.Add(this.lblServer);
            this.gbServer.Controls.Add(this.tbServer);
            this.gbServer.Controls.Add(this.tbUserName);
            this.gbServer.Controls.Add(this.tbPassword);
            this.gbServer.Controls.Add(this.lblUserName);
            this.gbServer.Controls.Add(this.lblPassword);
            this.gbServer.Location = new System.Drawing.Point(8, 16);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(320, 156);
            this.gbServer.TabIndex = 4;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(120, 74);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(188, 20);
            this.tbPort.TabIndex = 6;
            // 
            // port
            // 
            this.port.AutoSize = true;
            this.port.Location = new System.Drawing.Point(12, 78);
            this.port.Name = "port";
            this.port.Size = new System.Drawing.Size(66, 13);
            this.port.TabIndex = 5;
            this.port.Text = "Server port:";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(12, 20);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(72, 13);
            this.lblServer.TabIndex = 4;
            this.lblServer.Text = "Server name:";
            // 
            // tbServer
            // 
            this.tbServer.Location = new System.Drawing.Point(12, 40);
            this.tbServer.Name = "tbServer";
            this.tbServer.Size = new System.Drawing.Size(296, 20);
            this.tbServer.TabIndex = 0;
            // 
            // tbUserName
            // 
            this.tbUserName.Location = new System.Drawing.Point(120, 101);
            this.tbUserName.Name = "tbUserName";
            this.tbUserName.Size = new System.Drawing.Size(188, 20);
            this.tbUserName.TabIndex = 1;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(120, 125);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(188, 20);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(12, 105);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(62, 13);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "User name:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 129);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(57, 13);
            this.lblPassword.TabIndex = 1;
            this.lblPassword.Text = "Password:";
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.lblDatabase);
            this.gbDatabase.Controls.Add(this.tbDatabase);
            this.gbDatabase.Location = new System.Drawing.Point(8, 187);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 76);
            this.gbDatabase.TabIndex = 5;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // lblDatabase
            // 
            this.lblDatabase.AutoSize = true;
            this.lblDatabase.Location = new System.Drawing.Point(12, 20);
            this.lblDatabase.Name = "lblDatabase";
            this.lblDatabase.Size = new System.Drawing.Size(57, 13);
            this.lblDatabase.TabIndex = 3;
            this.lblDatabase.Text = "Database:";
            // 
            // tbDatabase
            // 
            this.tbDatabase.Location = new System.Drawing.Point(12, 40);
            this.tbDatabase.Name = "tbDatabase";
            this.tbDatabase.Size = new System.Drawing.Size(296, 20);
            this.tbDatabase.TabIndex = 0;
            // 
            // ClickHouseConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.gbServer);
            this.Controls.Add(this.label1);
            this.Name = "ClickHouseConnectionEditor";
            this.Size = new System.Drawing.Size(336, 358);
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private FastReport.Controls.LabelLine label1;
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
