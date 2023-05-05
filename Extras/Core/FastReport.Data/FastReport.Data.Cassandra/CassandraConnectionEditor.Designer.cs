using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data.Cassandra
{

    partial class CassandraConnectionEditor
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
            this.gbSelect = new System.Windows.Forms.GroupBox();
            this.tbHostName = new System.Windows.Forms.TextBox();
            this.lblHostName = new System.Windows.Forms.Label();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.tbKeyspace = new System.Windows.Forms.TextBox();
            this.lblKeyspace = new System.Windows.Forms.Label();
            this.tbPort = new System.Windows.Forms.NumericUpDown();
            this.lblPort = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.lblUsername = new System.Windows.Forms.Label();
            this.gbSelect.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbSelect
            // 
            this.gbSelect.Controls.Add(this.tbHostName);
            this.gbSelect.Controls.Add(this.lblHostName);
            this.gbSelect.Controls.Add(this.tbPassword);
            this.gbSelect.Controls.Add(this.lblPassword);
            this.gbSelect.Controls.Add(this.tbKeyspace);
            this.gbSelect.Controls.Add(this.lblKeyspace);
            this.gbSelect.Controls.Add(this.tbPort);
            this.gbSelect.Controls.Add(this.lblPort);
            this.gbSelect.Controls.Add(this.tbUsername);
            this.gbSelect.Controls.Add(this.lblUsername);
            this.gbSelect.Location = new System.Drawing.Point(8, 4);
            this.gbSelect.Name = "gbSelect";
            this.gbSelect.Size = new System.Drawing.Size(320, 159);
            this.gbSelect.TabIndex = 0;
            this.gbSelect.TabStop = false;
            this.gbSelect.Text = "Select database file";
            // 
            // tbHostName
            // 
            this.tbHostName.Location = new System.Drawing.Point(138, 25);
            this.tbHostName.Name = "tbHostName";
            this.tbHostName.Size = new System.Drawing.Size(170, 22);
            this.tbHostName.TabIndex = 1;
            // 
            // lblHostName
            // 
            this.lblHostName.AutoSize = true;
            this.lblHostName.Location = new System.Drawing.Point(12, 25);
            this.lblHostName.Name = "lblHostName";
            this.lblHostName.Size = new System.Drawing.Size(75, 16);
            this.lblHostName.TabIndex = 2;
            this.lblHostName.Text = "HostName:";
            // 
            // tbClusterName
            // 
            this.tbPassword.Location = new System.Drawing.Point(138, 125);
            this.tbPassword.Name = "tbClusterName";
            this.tbPassword.Size = new System.Drawing.Size(170, 22);
            this.tbPassword.TabIndex = 1;
            // 
            // lblClusterName
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(12, 125);
            this.lblPassword.Name = "lblClusterName";
            this.lblPassword.Size = new System.Drawing.Size(75, 16);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // tbPassword
            // 
            this.tbKeyspace.Location = new System.Drawing.Point(138, 75);
            this.tbKeyspace.Name = "tbPassword";
            this.tbKeyspace.Size = new System.Drawing.Size(170, 22);
            this.tbKeyspace.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblKeyspace.AutoSize = true;
            this.lblKeyspace.Location = new System.Drawing.Point(12, 75);
            this.lblKeyspace.Name = "lblPassword";
            this.lblKeyspace.Size = new System.Drawing.Size(75, 16);
            this.lblKeyspace.TabIndex = 4;
            this.lblKeyspace.Text = "Keyspace:";
            // 
            // tbUsername
            // 
            this.tbPort.Location = new System.Drawing.Point(138, 50);
            this.tbPort.Name = "tbUsername";
            this.tbPort.Size = new System.Drawing.Size(170, 22);
            this.tbPort.TabIndex = 1;
            this.tbPort.Minimum = 0;
            this.tbPort.Maximum = UInt16.MaxValue;
            this.tbPort.Value = 9042;
            this.tbPort.DecimalPlaces = 0;
            // 
            // lblUsername
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(12, 50);
            this.lblPort.Name = "lblUsername";
            this.lblPort.Size = new System.Drawing.Size(75, 16);
            this.lblPort.TabIndex = 5;
            this.lblPort.Text = "Port:";
            // 
            // tbPort
            // 
            this.tbUsername.Location = new System.Drawing.Point(138, 100);
            this.tbUsername.Name = "tbPort";
            this.tbUsername.Size = new System.Drawing.Size(170, 22);
            this.tbUsername.TabIndex = 1;
            // 
            // lblPort
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(12, 100);
            this.lblUsername.Name = "lblPort";
            this.lblUsername.Size = new System.Drawing.Size(75, 16);
            this.lblUsername.TabIndex = 6;
            this.lblUsername.Text = "Username:";
            // 
            // CassandraConnectionEditor
            // 
            this.Controls.Add(this.gbSelect);
            this.Name = "CassandraConnectionEditor";
            this.Size = new System.Drawing.Size(336, 173);
            this.gbSelect.ResumeLayout(false);
            this.gbSelect.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox gbSelect;
        private System.Windows.Forms.Label lblHostName;
        private System.Windows.Forms.TextBox tbHostName;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.NumericUpDown tbPort;
        private System.Windows.Forms.Label lblKeyspace;
        private System.Windows.Forms.TextBox tbKeyspace;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.TextBox tbUsername;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox tbPassword;
    }
}
