using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport.Data.ConnectionEditors
{
    internal partial class JsonConnectionEditor : ConnectionEditorBase
    {
        private void Localize()
        {
            gbSelect.Text = Res.Get("ConnectionEditors,Common,Database");
            lblJsonPath.Text = Res.Get("ConnectionEditors,Json,Path");
            tbJsonPath.ImageIndex = 1;
        }

        private void tbFile_ButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                    tbJsonPath.Text = dialog.FileName;
            }
        }

        protected override string GetConnectionString()
        {
            JsonConnectionStringBuilder builder = new JsonConnectionStringBuilder();
            builder.Json = tbJsonPath.Text;
            return builder.ToString();
        }

        protected override void SetConnectionString(string value)
        {
            JsonConnectionStringBuilder builder = new JsonConnectionStringBuilder(value);
            tbJsonPath.Text = builder.Json;
        }

        public JsonConnectionEditor()
        {
            InitializeComponent();
            Localize();
        }
    }
}