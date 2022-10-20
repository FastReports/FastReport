
using FastReport.Controls;

namespace FastReport.Data.ConnectionEditors
{
    partial class ESConnectionEditor
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
            this.gbConnection = new System.Windows.Forms.GroupBox();
            this.lbHeaders = new System.Windows.Forms.Label();
            this.dgvHeaders = new System.Windows.Forms.DataGridView();
            this.colHeader = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colContent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lbEndPoint = new System.Windows.Forms.Label();
            this.tbEndPoint = new System.Windows.Forms.TextBox();
            this.gbConnection.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).BeginInit();
            this.SuspendLayout();
            // 
            // gbConnection
            // 
            this.gbConnection.Controls.Add(this.lbHeaders);
            this.gbConnection.Controls.Add(this.dgvHeaders);
            this.gbConnection.Controls.Add(this.lbEndPoint);
            this.gbConnection.Controls.Add(this.tbEndPoint);
            this.gbConnection.Location = new System.Drawing.Point(8, 4);
            this.gbConnection.Margin = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.gbConnection.Name = "gbConnection";
            this.gbConnection.Size = new System.Drawing.Size(320, 233);
            this.gbConnection.TabIndex = 0;
            this.gbConnection.TabStop = false;
            this.gbConnection.Text = "Connection settings";
            // 
            // lbHeaders
            // 
            this.lbHeaders.AutoSize = true;
            this.lbHeaders.Location = new System.Drawing.Point(11, 70);
            this.lbHeaders.Name = "lbHeaders";
            this.lbHeaders.Size = new System.Drawing.Size(47, 13);
            this.lbHeaders.TabIndex = 12;
            this.lbHeaders.Text = "Headers";
            // 
            // dgvHeaders
            // 
            this.dgvHeaders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvHeaders.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHeader,
            this.colContent});
            this.dgvHeaders.Location = new System.Drawing.Point(11, 90);
            this.dgvHeaders.Name = "dgvHeaders";
            this.dgvHeaders.RowHeadersVisible = false;
            this.dgvHeaders.Size = new System.Drawing.Size(303, 131);
            this.dgvHeaders.TabIndex = 11;
            // 
            // colHeader
            // 
            this.colHeader.HeaderText = "Header";
            this.colHeader.Name = "colHeader";
            this.colHeader.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            // 
            // colContent
            // 
            this.colContent.HeaderText = "Content";
            this.colContent.Name = "colContent";
            this.colContent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            // 
            // lbJson
            // 
            this.lbEndPoint.AutoSize = true;
            this.lbEndPoint.Location = new System.Drawing.Point(8, 24);
            this.lbEndPoint.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this.lbEndPoint.Name = "lbESEndPoint";
            this.lbEndPoint.Size = new System.Drawing.Size(32, 13);
            this.lbEndPoint.TabIndex = 2;
            this.lbEndPoint.Text = "ElasticSearch EndPoint";
            // 
            // tbJson
            // 
            this.tbEndPoint.Location = new System.Drawing.Point(11, 44);
            this.tbEndPoint.Margin = new System.Windows.Forms.Padding(8, 3, 0, 5);
            this.tbEndPoint.Name = "tbESEndPoint";
            this.tbEndPoint.Size = new System.Drawing.Size(303, 20);
            this.tbEndPoint.TabIndex = 0;
            // 
            // JsonDataSourceConnectionEditor
            // 
            this.Controls.Add(this.gbConnection);
            this.Name = "ESDataSourceConnectionEditor";
            this.Size = new System.Drawing.Size(336, 241);
            this.gbConnection.ResumeLayout(false);
            this.gbConnection.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHeaders)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConnection;
        private System.Windows.Forms.TextBox tbEndPoint;
        private System.Windows.Forms.Label lbEndPoint;
        private System.Windows.Forms.Label lbHeaders;
        private System.Windows.Forms.DataGridView dgvHeaders;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHeader;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContent;
    }
}
