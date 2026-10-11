using System.Drawing;

namespace FastReport.Forms
{
    partial class TransportReportDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            //System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextEditorForm));
            this.btnOk = new FastReport.Transport.WinForms.Controls.RoundedButton();
            this.btnCancel = new FastReport.Transport.WinForms.Controls.RoundedButton();
            this.lbFileName = new System.Windows.Forms.Label();
            this.tbFileName = new FastReport.Transport.WinForms.Controls.RoundedTextBox();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(615, 450);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(73, 23);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += btnOk_Click;
            this.btnOk.BackColor = Color.White;
            this.btnOk.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(700, 450);
            this.btnCancel.Size = new System.Drawing.Size();
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(73, 23);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.TabIndex = 1;
            // 
            // lbFileName
            // 
            this.lbFileName.AutoSize = true;
            this.lbFileName.Location = new System.Drawing.Point(12, 424);
            this.lbFileName.Name = "lbFileName";
            this.lbFileName.Size = new System.Drawing.Size(54, 13);
            this.lbFileName.TabIndex = 0;
            this.lbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
| System.Windows.Forms.AnchorStyles.Right)));
            this.lbFileName.Text = "File name:";
            // 
            // tbFileName
            // 
            this.tbFileName.Location = new System.Drawing.Point(148, 420);
            this.tbFileName.Name = "tbFileName";
            this.tbFileName.Size = new System.Drawing.Size(625, 23);
            this.tbFileName.Control.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFileName.TabIndex = 3;
            this.tbFileName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
| System.Windows.Forms.AnchorStyles.Right)));
            // 
            // TransportSaveFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(785, 477);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lbFileName);
            this.Controls.Add(this.tbFileName);
            this.SetStyle(System.Windows.Forms.ControlStyles.DoubleBuffer, true);
            this.MinimumSize = new System.Drawing.Size(784, 472);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "TransportSaveFileDialog";
            this.ShowIcon = false;
            this.Text = "Save file";
            this.Controls.SetChildIndex(this.btnOk, 0);
            this.Controls.SetChildIndex(this.btnCancel, 0);
            this.Controls.SetChildIndex(this.lbFileName, 0);
            this.Controls.SetChildIndex(this.tbFileName, 0);
            this.ResumeLayout(false);
        }

        #endregion

        private FastReport.Transport.WinForms.Controls.RoundedButton btnOk;
        private FastReport.Transport.WinForms.Controls.RoundedButton btnCancel;
        private System.Windows.Forms.Label lbFileName;
        private FastReport.Transport.WinForms.Controls.RoundedTextBox tbFileName;
    }
}