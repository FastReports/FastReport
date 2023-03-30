namespace FastReport.Data
{
    partial class ExcelConnectionEditor
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
            this.tbExcelFile = new FastReport.Controls.TextBoxButton();
            this.lblSelectXlsx = new System.Windows.Forms.Label();
            this.cbxFieldNames = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // gbSelect
            // 
            this.gbSelect.Controls.Add(this.tbExcelFile);
            this.gbSelect.Controls.Add(this.lblSelectXlsx);
            this.gbSelect.Controls.Add(this.cbxFieldNames);
            this.gbSelect.Location = new System.Drawing.Point(8, 4);
            this.gbSelect.Name = "gbSelect";
            this.gbSelect.Size = new System.Drawing.Size(320, 110);
            this.gbSelect.TabIndex = 1;
            this.gbSelect.TabStop = false;
            this.gbSelect.Text = "Select database file";
            // 
            // tbExcelFile
            // 
            this.tbExcelFile.ButtonText = "";
            this.tbExcelFile.Image = null;
            this.tbExcelFile.Location = new System.Drawing.Point(12, 45);
            this.tbExcelFile.Name = "tbExcelFile";
            this.tbExcelFile.Size = new System.Drawing.Size(297, 21);
            this.tbExcelFile.TabIndex = 1;
            this.tbExcelFile.ButtonClick += TbExcelFile_ButtonClick;
            // 
            // lblSelectXlsx
            // 
            this.lblSelectXlsx.AutoSize = true;
            this.lblSelectXlsx.Location = new System.Drawing.Point(12, 25);
            this.lblSelectXlsx.Name = "lblSelectXlsx";
            this.lblSelectXlsx.Size = new System.Drawing.Size(77, 13);
            this.lblSelectXlsx.TabIndex = 1;
            this.lblSelectXlsx.Text = "Select .xlsx file:";
            // 
            // cbxFieldNames
            // 
            this.cbxFieldNames.AutoSize = true;
            this.cbxFieldNames.Location = new System.Drawing.Point(12, 75);
            this.cbxFieldNames.Name = "cbxFieldNames";
            this.cbxFieldNames.Size = new System.Drawing.Size(140, 17);
            this.cbxFieldNames.TabIndex = 2;
            this.cbxFieldNames.Text = "Field names in first string";
            this.cbxFieldNames.UseVisualStyleBackColor = true;
            // 
            // ExcelConnectionEditor
            // 
            this.Controls.Add(this.gbSelect);
            //this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ExcelConnectionEditor";
            this.Size = new System.Drawing.Size(336, 120);
            this.gbSelect.ResumeLayout(false);
            this.gbSelect.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion
        private System.Windows.Forms.Label lblSelectXlsx;
        private System.Windows.Forms.CheckBox cbxFieldNames;
        private Controls.TextBoxButton tbExcelFile;
        private System.Windows.Forms.GroupBox gbSelect;
    }
}
