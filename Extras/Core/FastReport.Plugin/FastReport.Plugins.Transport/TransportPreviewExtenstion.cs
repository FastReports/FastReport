using FastReport.Cloud.Transports;
using FastReport.Forms;
using FastReport.Preview;
using FastReport.Transport;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace FastReport.Plugins.Transport
{
    public class TransportPreviewExtension
    {
        PreviewControl preview;
        ButtonWrapper exportToTransport;
        ButtonWrapper openWithTransport;

        public TransportPreviewExtension(PreviewControl preview)
        {
            this.preview = preview;
            InitButtons();
        }

        public void InitButtons()
        {
            ButtonWrapper btnOpen = preview.btnOpen;
            ButtonWrapper btnSave = preview.btnSave;
            exportToTransport = CreateTransportButton(btnSave, "", 238, SaveUsingTransport_Click);
            openWithTransport = CreateTransportButton(btnOpen, "", 261, btnOpenUsingTransport_Click);
            Localize();
        }

        public void Localize()
        {
            if (exportToTransport != null)
                exportToTransport.Text = Res.Get("Export,ExportGroups,Transport");
            if (openWithTransport != null)
                openWithTransport.Text = Res.Get("Preview,OpenWithTransport");
        }

        protected ButtonWrapper CreateTransportButton(ButtonWrapper parent, string newItemText, int newItemImageIndex, EventHandler handler)
        {
            var newMenuItem = CreateItem(newItemText, newItemImageIndex);
            foreach (var transport in RegisteredTransport.AvailableTransportList)
            {
                var item = CreateItem(transport.Key, 238);
                item.Tag = transport.Value;
                item.Click += handler;
                newMenuItem.AddSubItem(item);
            }
            parent.AddSubItem(newMenuItem);
            return newMenuItem;
        }

        private ButtonWrapper CreateItem(string text, int imageIndex)
        {
            var item = new ButtonWrapper
            {
                ImageIndex = imageIndex,
                Text = text,
            };
            item.SetImage(preview.GetImage(imageIndex));
            item.SetFont(DrawUtils.DefaultFont);
            return item;
        }

        protected async void SaveUsingTransport_Click(object sender, EventArgs e)
        {
            if (preview.IsPreviewEmpty)
            {
                return;
            }
            try
            {
                preview.Enabled = false;
                var btn = ButtonWrapper.CreateFromSender(sender);
                if (btn != null)
                {
                    Type typeOfTransport = btn.Tag as Type;
                    ITransport transport = TransportCommands.GetTransport(typeOfTransport);
                    if (transport != null && await TransportCommands.Auth(transport))
                        ShowTransportForm(transport);
                }
            }
            finally
            {
                preview.Enabled = true;
            }
        }

        protected void ShowTransportForm(ITransport transport)
        {
            var form = new TransportSaveFileDialog(transport, preview.Report);
            form.EndUpload += preview.PreviewControl_AfterUpload;
            form.StartUpload += preview.Client_BeforeUpload;
            form.ShowDialog(
#if !MONO
                preview
#endif
                );
        }

        protected void btnOpenUsingTransport_Click(object sender, EventArgs e)
        {
            var btn = ButtonWrapper.CreateFromSender(sender);
            if (btn != null)
            {
                Type type = btn.Tag as Type;
                var transport = TransportCommands.GetTransport(type);
                OpenUsingTransport(transport);
            }
        }

        /// <summary>
        /// Open Transport form and shows selected prepared report.
        /// </summary>
        protected async void OpenUsingTransport(ITransport transport)
        {
            try
            {
                preview.Enabled = false;
                Stream stream = await TransportCommands.DownloadPreviewFile(transport);
                if (stream != null)
                    preview.Load(stream);
            }
            finally
            {
                preview.Enabled = true;
            }
        }
    }
}
