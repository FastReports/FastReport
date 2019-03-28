namespace WinFormsOpenSource
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripOpenBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripFirstBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripPrewBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripPageNum = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripNextBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripLastBtn = new System.Windows.Forms.ToolStripButton();
            this.toolStripZoomIn = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripZoomOut = new System.Windows.Forms.ToolStripButton();
            this.ReportsList = new System.Windows.Forms.ListBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(3, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(808, 644);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripOpenBtn,
            this.toolStripFirstBtn,
            this.toolStripPrewBtn,
            this.toolStripPageNum,
            this.toolStripNextBtn,
            this.toolStripLastBtn,
            this.toolStripZoomIn,
            this.toolStripSeparator1,
            this.toolStripZoomOut});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(1054, 25);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripOpenBtn
            // 
            this.toolStripOpenBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripOpenBtn.Image = global::WinFormsOpenSource.Properties.Resources._068;
            this.toolStripOpenBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripOpenBtn.Name = "toolStripOpenBtn";
            this.toolStripOpenBtn.Size = new System.Drawing.Size(23, 22);
            this.toolStripOpenBtn.Text = "toolStripButton1";
            this.toolStripOpenBtn.Click += new System.EventHandler(this.toolStripOpnBtn_Click);
            // 
            // toolStripFirstBtn
            // 
            this.toolStripFirstBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripFirstBtn.Image = global::WinFormsOpenSource.Properties.Resources.frst;
            this.toolStripFirstBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripFirstBtn.Name = "toolStripFirstBtn";
            this.toolStripFirstBtn.Size = new System.Drawing.Size(23, 22);
            this.toolStripFirstBtn.Text = "toolStripButton2";
            this.toolStripFirstBtn.Click += new System.EventHandler(this.toolStripFirstBtn_Click);
            // 
            // toolStripPrewBtn
            // 
            this.toolStripPrewBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripPrewBtn.Image = global::WinFormsOpenSource.Properties.Resources.prr;
            this.toolStripPrewBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripPrewBtn.Name = "toolStripPrewBtn";
            this.toolStripPrewBtn.Size = new System.Drawing.Size(23, 22);
            this.toolStripPrewBtn.Text = "toolStripButton3";
            this.toolStripPrewBtn.Click += new System.EventHandler(this.toolStripPrewBtn_Click);
            // 
            // toolStripPageNum
            // 
            this.toolStripPageNum.Name = "toolStripPageNum";
            this.toolStripPageNum.Size = new System.Drawing.Size(30, 25);
            this.toolStripPageNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.toolStripPageNum_KeyDown);
            // 
            // toolStripNextBtn
            // 
            this.toolStripNextBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripNextBtn.Image = global::WinFormsOpenSource.Properties.Resources.nxt;
            this.toolStripNextBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNextBtn.Name = "toolStripNextBtn";
            this.toolStripNextBtn.Size = new System.Drawing.Size(23, 22);
            this.toolStripNextBtn.Text = "toolStripButton4";
            this.toolStripNextBtn.Click += new System.EventHandler(this.toolStripNextBtn_Click);
            // 
            // toolStripLastBtn
            // 
            this.toolStripLastBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLastBtn.Image = global::WinFormsOpenSource.Properties.Resources.lst;
            this.toolStripLastBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripLastBtn.Name = "toolStripLastBtn";
            this.toolStripLastBtn.Size = new System.Drawing.Size(23, 22);
            this.toolStripLastBtn.Text = "toolStripButton5";
            this.toolStripLastBtn.Click += new System.EventHandler(this.toolStripLastBtn_Click);
            // 
            // toolStripZoomIn
            // 
            this.toolStripZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripZoomIn.Name = "toolStripZoomIn";
            this.toolStripZoomIn.Size = new System.Drawing.Size(54, 22);
            this.toolStripZoomIn.Text = "Zoom +";
            this.toolStripZoomIn.Click += new System.EventHandler(this.toolStripZoomIn_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripZoomOut
            // 
            this.toolStripZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("toolStripZoomOut.Image")));
            this.toolStripZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripZoomOut.Name = "toolStripZoomOut";
            this.toolStripZoomOut.Size = new System.Drawing.Size(51, 22);
            this.toolStripZoomOut.Text = "Zoom -";
            this.toolStripZoomOut.Click += new System.EventHandler(this.toolStripZoomOut_Click);
            // 
            // ReportsList
            // 
            this.ReportsList.Dock = System.Windows.Forms.DockStyle.Left;
            this.ReportsList.FormattingEnabled = true;
            this.ReportsList.Location = new System.Drawing.Point(0, 25);
            this.ReportsList.Name = "ReportsList";
            this.ReportsList.Size = new System.Drawing.Size(223, 650);
            this.ReportsList.TabIndex = 3;
            this.ReportsList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Location = new System.Drawing.Point(229, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(814, 650);
            this.panel1.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1054, 675);
            this.Controls.Add(this.ReportsList);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Report viewer";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripOpenBtn;
        private System.Windows.Forms.ToolStripButton toolStripFirstBtn;
        private System.Windows.Forms.ToolStripButton toolStripPrewBtn;
        private System.Windows.Forms.ToolStripTextBox toolStripPageNum;
        private System.Windows.Forms.ToolStripButton toolStripNextBtn;
        private System.Windows.Forms.ToolStripButton toolStripLastBtn;
        private System.Windows.Forms.ListBox ReportsList;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripButton toolStripZoomIn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripZoomOut;
    }
}

