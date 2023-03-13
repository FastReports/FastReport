using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FastReport.Web.Services
{
    internal sealed class ExportService : IExportsService
    {
        [Obsolete]
        internal static ExportService Instance { get; } = new ExportService();

        public byte[] ExportReport(WebReport webReport, KeyValuePair<string, string>[] exportParams, string exportFormat, out string filename) 
        {
            using (MemoryStream exportStream = new MemoryStream())
            {
                webReport.ExportReport(exportStream, exportFormat, exportParams);

                filename = webReport.GetCurrentTabName();
                if (filename.IsNullOrWhiteSpace())
                    filename = "report";

                return exportStream.ToArray();
            }
        }

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
#endif
            }

            return msg;
        }
    }
}
