using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FastReport.Web.Infrastructure;

namespace FastReport.Web.Services
{
    internal sealed class ExportService : IExportsService
    {

        public byte[] ExportReport(WebReport webReport, KeyValuePair<string, string>[] exportParams, string exportFormat, out string filename)
        {
            using var exportStream = new MemoryStream();
            webReport.ExportReport(exportStream, exportFormat, exportParams);

            filename = webReport.GetCurrentTabName();
            if (filename.IsNullOrWhiteSpace())
                filename = "report";

            return exportStream.ToArray();
        }

#if !OPENSOURCE
        public void ExportEmail(WebReport webReport, EmailExportParams exportParams)
        {
            var exportInfo = ExportsHelper.GetInfoFromExt(exportParams.ExportFormat);
            var export = exportInfo.CreateExport();
            var accountSettings = FastReportGlobal.InternalEmailExportOptions;

            var cc = exportParams.Address.Split(';');
            var address = cc[0];
            cc = cc.Skip(1).Select(s => s.Trim()).ToArray();

            var emailExport = new Export.Email.EmailExport
            {
                Export = export,
                MessageBody = exportParams.MessageBody,
                NameAttachmentFile = exportParams.NameAttachmentFile,
                Subject = exportParams.Subject,
                Address = address,
                CC = cc,
                Account = new Export.Email.EmailSettings
                {
                    Host = accountSettings.Host,
                    Address = accountSettings.Address,
                    Name = accountSettings.Name,
                    MessageTemplate = accountSettings.MessageTemplate,
                    UserName = accountSettings.Username,
                    Port = accountSettings.Port,
                    Password = accountSettings.Password,
                    EnableSSL = accountSettings.EnableSSL
                }
            };

            emailExport.SendEmail(webReport.Report);
        }
#endif

        public string GetExportSettings(WebReport webReport, string format)
        {
            string msg = string.Empty;
            switch (format)
            {
                case "image":
                    msg = webReport.template_ImageExportSettings();
                    break;
                case "html":
                    msg = webReport.template_HtmlExportSettings();
                    break;
#if !OPENSOURCE
                case "pdf":
                    msg = webReport.template_PdfExportSettings();
                    break;
                case "docx":
                    msg = webReport.template_DocxExportSettings();
                    break;
                case "xlsx":
                    msg = webReport.template_XlsxExportSettings();
                    break;
                case "ods":
                    msg = webReport.template_OdsExportSettings();
                    break;
                case "odt":
                    msg = webReport.template_OdtExportSettings();
                    break;
                case "svg":
                    msg = webReport.template_SvgExportSettings();
                    break;
                case "rtf":
                    msg = webReport.template_RtfExportSettings();
                    break;
                case "xml":
                    msg = webReport.template_XmlExportSettings();
                    break;
                case "pptx":
                    msg = webReport.template_PptxExportSettings();
                    break;
                case "email":
                    msg = webReport.template_EmailExportSettings();
                    break;
#endif
            }

            return msg;
        }
    }
}
