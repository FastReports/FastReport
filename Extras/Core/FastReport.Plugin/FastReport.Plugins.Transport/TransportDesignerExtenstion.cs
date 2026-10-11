using FastReport.Cloud.Transports;
using FastReport.Design;
using FastReport.Design.StandardDesigner;
using FastReport.Transport;
using FastReport.Utils;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastReport.Plugins.Transport
{
    public class TransportDesignerExtension : IDisposable
    {
        Designer designer;
        ButtonWrapper miFileOpenUsingTransport;
        ButtonWrapper miFileSaveUsingTransport;

        public bool IsDisposed { get; private set; }

        public TransportDesignerExtension(Designer designer)
        {
            Config.DesignerSettings.ReportSaveToTransport += DesignerSettings_ReportSaveToTransport;
            Config.DesignerSettings.RecentReportOpenFromTransport += DesignerSettings_RecentReportOpenFromTransport;

            this.designer = designer;
            DesignerMenu menu = null;
            foreach (Control item in designer.Controls)
            {
                if (item.Name == "MainMenu")
                    menu = (DesignerMenu)item;
            }

            miFileOpenUsingTransport = menu.CreateMenuItem();
            miFileOpenUsingTransport.ImageIndex = 261;
            miFileOpenUsingTransport.SetFont(DrawUtils.DefaultFont);
            CreateTransportButtons(menu, miFileOpenUsingTransport, OpenWithTransport_Click);

            miFileSaveUsingTransport = menu.CreateMenuItem();
            miFileSaveUsingTransport.ImageIndex = 262;
            miFileSaveUsingTransport.SetFont(DrawUtils.DefaultFont);
            CreateTransportButtons(menu, miFileSaveUsingTransport, SaveWithTransport_Click);

            menu.miFile.InsertSubItem(3, miFileOpenUsingTransport);
            menu.miFile.InsertSubItem(
#if !MONO
                8
#else
                12
#endif
                , miFileSaveUsingTransport);
            Localize();
        }

        public void Localize()
        {
            if (miFileOpenUsingTransport != null)
                miFileOpenUsingTransport.Text = Res.Get("Designer,Menu,File,OpenUsingTransport");
            if (miFileSaveUsingTransport != null)
                miFileSaveUsingTransport.Text = Res.Get("Designer,Menu,File,SaveUsingTransport");
        }

        private void CreateTransportButtons(DesignerMenu menu, ButtonWrapper parent, EventHandler click)
        {
            foreach (var node in RegisteredTransport.AvailableTransportList)
            {
                ButtonWrapper item = menu.CreateMenuItem(238, node.Key, click);
                item.Tag = node.Value;
                item.SetFont(DrawUtils.DefaultFont);
                parent.AddSubItem(item);
            }
        }

        protected async void OpenWithTransport_Click(object sender, EventArgs e)
        {
            var btn = ButtonWrapper.CreateFromSender(sender);
            if (btn != null && !designer.Restrictions.DontLoadReport)
            {
                ITransport transport = TransportCommands.GetTransport(btn.Tag as Type);
                if (transport != null)
                {
                    designer.Enabled = false;
                    TransportFile file = await TransportCommands.DownloadTemplateFile(transport, designer);
                    OpenTemplate(file);
                    designer.Enabled = true;
                }
            }
        }

        protected async void DesignerSettings_RecentReportOpenFromTransport(object sender, OpenSaveReportEventArgs e)
        {
            if (designer.cmdRecentFiles.IsTransportFile(e.FileName) && designer == sender)
            {
                if (!RegisteredTransport.AvailableTransportList.TryGetValue(e.Report.TransportFileInfo.TransportName, out var availableTransport)
                    || designer.Restrictions.DontLoadReport)
                    return;
                designer.Enabled = false;
                ITransport transport = TransportCommands.GetTransport(availableTransport);
                if (transport != null)
                {
                    TransportFile file = null;
                    file = await TransportCommands.DownloadTemplateFile(transport, e.FileName, designer);
                    OpenTemplate(file);
                }
                designer.Enabled = true;
            }
        }

        protected void OpenTemplate(TransportFile file)
        {
            if (file != null && file.File != null)
            {
                ReportTab reportTab = null;

                if (designer.MdiMode)
                {
                    Report report = new Report();
                    report.Designer = designer;
                    reportTab = designer.CreateReportTab(report);
                }
                else
                    reportTab = designer.ActiveReportTab;

                if (reportTab.LoadFileFromTransport(file))
                {
                    if (designer.MdiMode)
                        designer.AddReportTab(reportTab);
                }
                else if (designer.MdiMode)
                    reportTab.Dispose();
            }
        }


        protected void DesignerSettings_ReportSaveToTransport(object sender, OpenSaveReportEventArgs e)
        {
            if (e.Report.TransportFileInfo != null && designer == sender && !designer.Restrictions.DontSaveReport)
            {
                Config.ReportSettings.OnStartLoadingProgress(designer.MdiMode ? null : designer.ActiveReportTab);
                Task.Run(() => TransportCommands.Resave(e.Report));
                Config.ReportSettings.OnFinishLoadingProgress(designer.MdiMode ? null : designer.ActiveReportTab);
            }
        }

        protected async void SaveWithTransport_Click(object sender, EventArgs e)
        {
            var btn = ButtonWrapper.CreateFromSender(sender);
            if (btn != null && !designer.Restrictions.DontSaveReport)
            {
                ITransport transport = TransportCommands.GetTransport(btn.Tag as Type);
                if (transport != null)
                {
                    TransportFileInfo fileInfo = await TransportCommands.UploadTemplateFile(transport, designer.ActiveReport);
                    if (fileInfo != null)
                    {
                        designer.ActiveReport.TransportFileInfo = fileInfo;
                        designer.ActiveReport.FileName = fileInfo.FilePath;
                        designer.cmdRecentFiles.AddTransportReport(designer.ActiveReport);
                        designer.UpdatePlugins(null);
                        if (designer.ActiveReportTab != null)
                            designer.ActiveReportTab.UpdateCaption();
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Config.DesignerSettings.ReportSaveToTransport -= DesignerSettings_ReportSaveToTransport;
                Config.DesignerSettings.RecentReportOpenFromTransport -= DesignerSettings_RecentReportOpenFromTransport;
                IsDisposed = true;
            }
        }
    }
}
