using FastReport.Web.Cache;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class ExportReportController : ApiControllerBase
    {

        public ExportReportController()
        {
        }

        public sealed class ExportReportParams
        {
            public string ReportId { get; set; }

            public string ExportFormat { get; set; }
        }

        [HttpGet]
        [Route("/_fr/preview.exportReport")]
        public IActionResult ExportReport([FromQuery] ExportReportParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var exportFormat = query.ExportFormat.ToLower();

            // TODO:
            // skip extra key/value pairs
            var exportParams = Request.Query.Where(pair => pair.Key != "exportFormat" && pair.Key != "reportId").ToArray();

            using (MemoryStream exportStream = new MemoryStream())
            {
                try
                {
                    webReport.ExportReport(exportStream, exportFormat, exportParams);
                }
                catch (UnsupportedExportException)
                {
                    return new UnsupportedMediaTypeResult();
                }

                var filename = webReport.GetCurrentTabName();
                if (filename.IsNullOrWhiteSpace())
                    filename = "report";

                return new FileContentResult(exportStream.ToArray(), "application/octet-stream")
                {
                    FileDownloadName = $"{filename}.{exportFormat}"
                };
            }
        }

        public sealed class ExportSettingsParams
        {
            public string ReportId { get; set; }

            public string Format { get; set; }
        }


        [HttpPost]
        [Route("/_fr/exportsettings.getSettings")]
        public IActionResult ExportReport([FromQuery] ExportSettingsParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            string msg = string.Empty;
            switch (query.Format)
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

            if (msg != null)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = msg,
                };
            }
            return new NotFoundResult();
        }


        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

    }
}
