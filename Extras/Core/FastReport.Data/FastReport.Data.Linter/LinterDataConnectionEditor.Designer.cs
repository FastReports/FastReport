using System.Windows.Forms;

namespace FastReport.Data.ConnectionEditors
{
    partial class LinterDataConnectionEditor
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
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.lbUserID = new Label();
            this.lbPassword = new Label();
            this.lbDataSource = new Label();
            this.tbUserID = new TextBox();
            this.tbPassword = new TextBox();
            this.tbDataSource = new TextBox();
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.gbDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.tbPassword);
            this.gbDatabase.Controls.Add(this.tbUserID);
            this.gbDatabase.Controls.Add(this.lbPassword);
            this.gbDatabase.Controls.Add(this.lbUserID);
            this.gbDatabase.Controls.Add(this.tbDataSource);
            this.gbDatabase.Controls.Add(this.lbDataSource);
            this.gbDatabase.Controls.Add(this.btnAdvanced);
            this.gbDatabase.Location = new System.Drawing.Point(8, 4);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 123);
            this.gbDatabase.TabIndex = 1;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // lbUserID
            // 
            this.lbUserID.AutoSize = true;
            this.lbUserID.Location = new System.Drawing.Point(8, 18);
            this.lbUserID.Name = "lbUserID";
            this.lbUserID.Size = new System.Drawing.Size(32, 13);
            this.lbUserID.TabIndex = 2;
            this.lbUserID.Text = "UserID";
            // 
            // tbUserID
            // 
            this.tbUserID.Location = new System.Drawing.Point(130, 15);
            this.tbUserID.Name = "tbUserID";
            this.tbUserID.Size = new System.Drawing.Size(180, 20);
            this.tbUserID.TabIndex = 0;
            // 
            // lbPassword
            // 
            this.lbPassword.AutoSize = true;
            this.lbPassword.Location = new System.Drawing.Point(8, 43);
            this.lbPassword.Name = "lbPassword";
            this.lbPassword.Size = new System.Drawing.Size(32, 13);
            this.lbPassword.TabIndex = 2;
            this.lbPassword.Text = "Password";
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(130, 40);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(180, 20);
            this.tbPassword.TabIndex = 0;
            this.tbPassword.PasswordChar = '*';
            // 
            // lbDataSource
            // 
            this.lbDataSource.AutoSize = true;
            this.lbDataSource.Location = new System.Drawing.Point(8, 68);
            this.lbDataSource.Name = "lbDataSource";
            this.lbDataSource.Size = new System.Drawing.Size(32, 13);
            this.lbDataSource.TabIndex = 2;
            this.lbDataSource.Text = "DataSource";
            // 
            // tbDataSource
            // 
            this.tbDataSource.Location = new System.Drawing.Point(130, 65);
            this.tbDataSource.Name = "tbDataSource";
            this.tbDataSource.Size = new System.Drawing.Size(180, 20);
            this.tbDataSource.TabIndex = 0;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.btnAdvanced.Location = new System.Drawing.Point(180, 93);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(130, 23);
            this.btnAdvanced.TabIndex = 8;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);

            // 
            // LinterConnectionEditor
            // 
            this.Controls.Add(this.gbDatabase);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Name = "LinterConnectionEditor";
            this.Size = new System.Drawing.Size(336, 135);
            this.gbDatabase.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.Label lbUserID;
        private System.Windows.Forms.Label lbPassword;
        private System.Windows.Forms.Label lbDataSource;
        private System.Windows.Forms.TextBox tbUserID;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbDataSource;
        private System.Windows.Forms.GroupBox gbDatabase;
        #endregion
    }
}
