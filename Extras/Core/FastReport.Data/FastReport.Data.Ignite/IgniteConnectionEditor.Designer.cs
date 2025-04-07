using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data
{

    partial class IgniteConnectionEditor
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
            this.gbServer = new System.Windows.Forms.GroupBox();
            this.tbEndpoints = new System.Windows.Forms.TextBox();
            this.lblEndpoints = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.gbServer.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbServer
            // 
            this.gbServer.Controls.Add(this.tbEndpoints);
            this.gbServer.Controls.Add(this.lblEndpoints);
            this.gbServer.Controls.Add(this.tbPassword);
            this.gbServer.Controls.Add(this.lblPassword);
            this.gbServer.Controls.Add(this.tbUsername);
            this.gbServer.Controls.Add(this.lblUsername);
            this.gbServer.Location = new System.Drawing.Point(3, 3);
            this.gbServer.Name = "gbServer";
            this.gbServer.Size = new System.Drawing.Size(330, 167);
            this.gbServer.TabIndex = 0;
            this.gbServer.TabStop = false;
            this.gbServer.Text = "Server";
            // 
            // tbEndpoints
            // 
            this.tbEndpoints.Location = new System.Drawing.Point(138, 25);
            this.tbEndpoints.Name = "tbEndpoints";
            this.tbEndpoints.Size = new System.Drawing.Size(170, 26);
            this.tbEndpoints.TabIndex = 0;
            // 
            // lblEndpoints
            // 
            this.lblEndpoints.AutoSize = true;
            this.lblEndpoints.Location = new System.Drawing.Point(12, 28);
            this.lblEndpoints.Name = "lblEndpoints";
            this.lblEndpoints.Size = new System.Drawing.Size(57, 13);
            this.lblEndpoints.TabIndex = 0;
            this.lblEndpoints.Text = "Endpoints:";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(138, 73);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(170, 26);
            this.tbPassword.TabIndex = 2;
            this.tbPassword.UseSystemPasswordChar = true;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 73);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 20);
            this.lblPassword.TabIndex = 0;
            this.lblPassword.Text = "Password:";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(138, 48);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(170, 26);
            this.tbUsername.TabIndex = 1;
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(12, 48);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(87, 20);
            this.lblUsername.TabIndex = 0;
            this.lblUsername.Text = "Username:";
            // 
            // IgniteConnectionEditor
            // 
            this.Controls.Add(this.gbServer);
            this.Name = "IgniteConnectionEditor";
            this.Size = new System.Drawing.Size(336, 173);
            this.gbServer.ResumeLayout(false);
            this.gbServer.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbServer;
        private System.Windows.Forms.Label lblEndpoints;
        private System.Windows.Forms.TextBox tbEndpoints;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox tbPassword;
    }
}
