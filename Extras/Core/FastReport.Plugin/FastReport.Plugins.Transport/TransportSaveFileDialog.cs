using FastReport.Export;
using FastReport.Transport;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace FastReport.Forms
{
    internal partial class TransportSaveFileDialog : Transport.Forms.TransportForm
    {
        private readonly List<ExportBase> exports;
        private readonly Report report;

        public event EventHandler StartUpload;
        public event EventHandler EndUpload;

        public TransportSaveFileDialog(ITransport transport, Report report)
        {
            InitializeComponent();
            Init(transport);
            this.report = report;
            exports = new List<ExportBase>();
            List<ObjectInfo> list = new List<ObjectInfo>();
            RegisteredObjects.Exports.EnumItems(list);
            cbFileType.Items.Add(Res.Get("Preview,SaveNative"));
            exports.Add(null);
            foreach (ObjectInfo info in list)
            {
                if (info.Object != null)
                {
                    cbFileType.Items.Add(Res.TryGet(info.Text));
                    exports.Add(Activator.CreateInstance(info.Object) as ExportBase);
                }
            }
            cbFileType.SelectedIndex = 0;

            tbFileName.Control.Text = report.GetReportName;
            lvwData.SelectedIndexChanged += LvwData_SelectedIndexChanged;
        }

        public override void Localize()
        {
            base.Localize();
            Text = Res.Get("Transport,TransportSaveFileDialog");
            lblFileType.Text = Res.Get("Cloud,CloudStorage,FileType");
            btnSettings.Text = Res.Get("Cloud,CloudStorage,Settings");
            lbFileName.Text = Res.Get("Cloud,S3,FileName");
            btnOk.Text = Res.Get("Buttons,Ok");
            btnCancel.Text = Res.Get("Buttons,Cancel");
        }

        private void LvwData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwData.SelectedItems.Count == 1)
            {
                TransportItem transportItem = (TransportItem)lvwData.SelectedItems[0].Tag;
                if (transportItem.ItemType == TransportItemType.File)
                    tbFileName.Control.Text = transportItem.Name;
            }
        }

        private void cbFileType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnSettings.Enabled = cbFileType.SelectedIndex != 0;
            btnSettings.BackColor = btnSettings.Enabled ? Color.White : Color.Transparent;
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            ExportBase export = exports[cbFileType.SelectedIndex];
            export.SetReport(report);
            export.ShowDialog();
        }

        private string GetFileExtension(ExportBase export)
        {
            string extension = export.FileFilter;
            return extension.Substring(extension.LastIndexOf('.'));
        }

        protected MemoryStream PrepareToSave(Report report, ExportBase export)
        {
            MemoryStream stream = new MemoryStream();
            if (export != null)
            {
                export.OpenAfterExport = false;
                if (!export.HasMultipleFiles)
                {
                    export.Export(report, stream);
                }
                else
                {
                    export.ExportAndZip(report, stream);
                }
            }
            else
            {
                report.PreparedPages.Save(stream);
            }

            stream.Position = 0;
            return stream;
        }

        private async void btnOk_Click(object sender, EventArgs e)
        {
            string fileName;
            var index = cbFileType.SelectedIndex;
            bool hasLocalItem = false;

            if (StartUpload != null)
                StartUpload(this, new EventArgs());

            if (index == 0)
                fileName = tbFileName.Control.Text + ".fpx";
            else
                fileName = tbFileName.Control.Text + GetFileExtension(exports[index]);

            foreach (ListViewItem item in lvwData.Items)
            {
                TransportItem transportItem = (TransportItem)item.Tag;
                if (fileName == transportItem.Name)
                {
                    hasLocalItem = true;
                    break;
                }
            }

            if (hasLocalItem)
            {
                if (MessageBox.Show(
                    Localization.Get("TransportForm,FilesOverwritten"),
                    Localization.Get("TransportForm,Warning"),
                    MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Warning
                    ) != DialogResult.OK)
                    return;
            }

            MemoryStream memoryStream = PrepareToSave(report, exports[index]);
            await Transport.UploadFile(memoryStream, CurrentPath + fileName);
            if (EndUpload != null)
                EndUpload(this, new EventArgs());

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
