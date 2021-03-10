namespace FastReport.Data
{
    partial class SQLiteConnectionEditor
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
            this.gbDatabase = new System.Windows.Forms.GroupBox();
            this.btBrowse = new System.Windows.Forms.Button();
            this.lblDataSource = new System.Windows.Forms.Label();
            this.tbDataSource = new System.Windows.Forms.TextBox();
            this.label1 = new FastReport.Controls.LabelLine();
            this.gbDatabase.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.AutoSize = true;
            this.btnAdvanced.Location = new System.Drawing.Point(252, 220);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(77, 23);
            this.btnAdvanced.TabIndex = 0;
            this.btnAdvanced.Text = "Advanced...";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            this.btnAdvanced.Click += new System.EventHandler(this.btnAdvanced_Click);
            // 
            // gbDatabase
            // 
            this.gbDatabase.Controls.Add(this.btBrowse);
            this.gbDatabase.Controls.Add(this.lblDataSource);
            this.gbDatabase.Controls.Add(this.tbDataSource);
            this.gbDatabase.Location = new System.Drawing.Point(8, 3);
            this.gbDatabase.Name = "gbDatabase";
            this.gbDatabase.Size = new System.Drawing.Size(320, 211);
            this.gbDatabase.TabIndex = 1;
            this.gbDatabase.TabStop = false;
            this.gbDatabase.Text = "Database";
            // 
            // btBrowse
            // 
            this.btBrowse.Location = new System.Drawing.Point(270, 45);
            this.btBrowse.Name = "btBrowse";
            this.btBrowse.Size = new System.Drawing.Size(32, 21);
            this.btBrowse.TabIndex = 4;
            this.btBrowse.Text = "..";
            this.btBrowse.UseVisualStyleBackColor = true;
            this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
            // 
            // lblDataSource
            // 
            this.lblDataSource.AutoSize = true;
            this.lblDataSource.Location = new System.Drawing.Point(6, 29);
            this.lblDataSource.Name = "lblDataSource";
            this.lblDataSource.Size = new System.Drawing.Size(67, 13);
            this.lblDataSource.TabIndex = 3;
            this.lblDataSource.Text = "DataSource:";
            // 
            // tbDataSource
            // 
            this.tbDataSource.Location = new System.Drawing.Point(9, 45);
            this.tbDataSource.Name = "tbDataSource";
            this.tbDataSource.Size = new System.Drawing.Size(255, 20);
            this.tbDataSource.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 244);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(320, 17);
            this.label1.TabIndex = 2;
            // 
            // SQLiteConnectionEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbDatabase);
            this.Controls.Add(this.btnAdvanced);
            this.Name = "SQLiteConnectionEditor";
            this.Size = new System.Drawing.Size(336, 263);
            this.gbDatabase.ResumeLayout(false);
            this.gbDatabase.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.GroupBox gbDatabase;
        private System.Windows.Forms.Label lblDataSource;
        private FastReport.Controls.LabelLine label1;
        private System.Windows.Forms.TextBox tbDataSource;
        private System.Windows.Forms.Button btBrowse;
    }
}
