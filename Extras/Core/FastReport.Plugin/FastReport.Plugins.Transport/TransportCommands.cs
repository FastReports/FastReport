using FastReport.Design;
using FastReport.Forms;
using FastReport.Transport;
using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FastReport.Cloud.Transports
{
    internal static class TransportCommands
    {
        private static Dictionary<string, ITransport> cachedTransports = new Dictionary<string, ITransport>();
        public static ITransport GetTransport(Type info)
        {
            if (info != null)
            {
                if (cachedTransports.ContainsKey(info.Name))
                    return cachedTransports[info.Name];

                ITransport client = Activator.CreateInstance(info) as ITransport;
                XmlItem xi = Config.Root.FindItem(info.Name).FindItem("StorageSettings");

                if (!string.IsNullOrEmpty(xi.Value))
                {
                    client = DeserializeWithReflection(info, xi.Value);
                }
                else
                {        
                    Form form = TransportClientInfoFormFactory.Create(client);
                    form.FormClosed += Form_FormClosed;
                    if (form.ShowDialog() != DialogResult.OK)
                    {
                        return null;
                    }
                }
                cachedTransports.Add(info.Name, client);
                return client;
            }

            return null;
        }

        private static ITransport DeserializeWithReflection(Type type, string value)
        {
            var method = typeof(TransportSerializer)
                .GetMethod(nameof(TransportSerializer.DeserializeXml))
                .MakeGenericMethod(type);

            return (ITransport)method.Invoke(null, new object[] { value });
        }

        private static void Form_FormClosed(object sender, FormClosedEventArgs e)
        {
            var form = sender as Transport.Forms.ClientInfoForms.TransportClientInfoForm;
            SaveTransportData(form.Transport);
        }

        private static void SaveTransportData(ITransport transport)
        {
            var xi = Config.Root.FindItem(transport.GetType().Name).FindItem("StorageSettings");
            xi.Value = TransportSerializer.SerializeXml(transport);
        }

        private static string TransportStr(ITransport transport)
        {
            return transport.GetType().Name.Replace("Transport", "");
        }

        public static async Task<bool> Auth(ITransport transport)
        {
            try
            {
                if (!(transport is IOAuthTransport authTransport) || authTransport.Token == null)
                {
                    await transport.Authorize();
                    SaveTransportData(transport);
                }
            }
            catch (ArgumentException e)
            {
                Form form = TransportClientInfoFormFactory.Create(transport);
                form.FormClosed += Form_FormClosed;
                if (form.ShowDialog() == DialogResult.OK)
                    return await Auth(transport);
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Localization.Get("TransportForm,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;
        }

        public static async Task<bool> Resave(Report report)
        {
            if (report.TransportFileInfo != null &&
                RegisteredTransport.AvailableTransportList.TryGetValue(report.TransportFileInfo.TransportName, out var availableTransport))
            {
                var transport = GetTransport(availableTransport);
                try
                {
                    if (await Auth(transport))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            report.Save(ms);
                            ms.Position = 0;
                            await transport.UploadFile(ms, report.TransportFileInfo.FilePath);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localization.Get("TransportForm,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        public static async Task<Stream> DownloadPreviewFile(ITransport transport)
        {
            if (await Auth(transport))
            {
                var form = new TransportReportDialog(transport);
                form.FileType = TransportReportDialog.TargetFileType.FPX;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        return await form.Transport.DownloadFile(form.SelectFile);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Localization.Get("TransportForm,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return null;
        }

        public static async Task<TransportFile> DownloadTemplateFile(ITransport transport, Designer designer)
        {
            if (await Auth(transport))
            {
                var form = new TransportReportDialog(transport);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return await DownloadTemplateFile(transport, form.SelectFile, designer);
                }
            }
            return null;
        }

        public static async Task<TransportFile> DownloadTemplateFile(ITransport transport, string fileName, Designer designer)
        {
            if (await Auth(transport))
            {
                Config.ReportSettings.OnStartLoadingProgress(designer.MdiMode ? null : designer.ActiveReportTab);
                try
                {
                    return new TransportFile()
                    {
                        File = await transport.DownloadFile(fileName),
                        FileInfo = new TransportFileInfo()
                        {
                            FilePath = fileName,
                            TransportName = TransportStr(transport)
                        }
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, Localization.Get("TransportForm,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    Config.ReportSettings.OnFinishLoadingProgress(designer.MdiMode ? null : designer.ActiveReportTab);
                }
            }
            return null;
        }

        public static async Task<TransportFileInfo> UploadTemplateFile(ITransport transport, Report report)
        {
            if (await Auth(transport))
            {
                MemoryStream ms = new MemoryStream();
                report.Save(ms);
                ms.Position = 0;
                var form = new TransportReportDialog(transport);
                form.DialogMode = TransportReportDialog.Mode.Save;
                form.FileName = report.GetReportName;
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        await form.Transport.UploadFile(ms, form.SelectFile);
                        return new TransportFileInfo() 
                        {
                            FilePath = form.SelectFile,
                            TransportName = TransportStr(transport)
                        };

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, Localization.Get("TransportForm,Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            return null;
        }
    }
}