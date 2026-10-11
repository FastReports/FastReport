using System.Windows.Forms;
using FastReport.Transport;
using System;
using System.Collections.Generic;
using FastReport.Utils;

namespace FastReport.Forms
{
    internal partial class TransportReportDialog : Transport.Forms.TransportForm
    {
        private Mode dialogMode = Mode.Open;
        public enum Mode
        {
            Open,
            Save,
        }
        public enum TargetFileType
        {
            FRX,
            FPX,
        }

        internal string FileExtension => FileType == TargetFileType.FRX ? ".frx" : ".fpx";
        public string SelectFile { get => CurrentPath + (FileName.EndsWith(FileExtension) ? FileName : FileName + FileExtension); }
        public string FileName { get => tbFileName.Control.Text; set => tbFileName.Control.Text = value; }
        public Mode DialogMode
        {
            get
            {
                return dialogMode;
            }
            set
            {
                dialogMode = value;
                LocalizeFormLabel();
            }
        }

        public TargetFileType FileType { get; set; } = TargetFileType.FRX;

        public TransportReportDialog(ITransport transport)
        {
            InitializeComponent();
            Init(transport);
            lvwData.SelectedIndexChanged += LvwData_SelectedIndexChanged;
        }

        public override void Localize()
        {
            base.Localize();
            LocalizeFormLabel();
            lbFileName.Text = Res.Get("Transport,TransportReportDialog,FileName");
            btnOk.Text = Res.Get("Buttons,Ok");
            btnCancel.Text = Res.Get("Buttons,Cancel");
        }

        private void LocalizeFormLabel()
        {
            if (DialogMode == Mode.Open)
            {
                Text = Res.Get("Transport,TransportReportDialog,OpenReport");
            }
            else
            {
                Text = Res.Get("Transport,TransportReportDialog,SaveReport");
            }
        }

        private void LvwData_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwData.SelectedItems.Count == 1)
            {
                TransportItem transportItem = (TransportItem)lvwData.SelectedItems[0].Tag;
                if (transportItem.ItemType == TransportItemType.File)
                {
                    if (transportItem.Name.EndsWith(FileExtension))
                        tbFileName.Control.Text = transportItem.Name.Remove(transportItem.Name.LastIndexOf(FileExtension));
                    else
                        tbFileName.Control.Text = transportItem.Name;
                }
            }
        }

        protected override void ClientFilter(ref List<TransportItem> list)
        {
            base.ClientFilter(ref list);
            list.RemoveAll(x => x.ItemType == TransportItemType.File && !x.Name.EndsWith(FileExtension));
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (DialogMode == Mode.Save)
            {
                bool hasLocalItem = false;
                var name = FileName.EndsWith(FileExtension) ? FileName : FileName + FileExtension;
                foreach (ListViewItem item in lvwData.Items)
                {
                    TransportItem transportItem = (TransportItem)item.Tag;
                    if (name == transportItem.Name)
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
            }
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
