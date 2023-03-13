using FastReport.Web.Cache;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class ExportReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IExportsService _exportsService;

        public ExportReportController(IReportService reportService, IExportsService exportsService)
        {
            _reportService = reportService;
            _exportsService = exportsService;
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
            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            // TODO:
            // skip extra key/value pairs
            var exportFormat = query.ExportFormat.ToLower();
            var exportParams = Request.Query.Where(pair => pair.Key != "exportFormat" && pair.Key != "reportId")
                .Select(item => new KeyValuePair<string, string>(item.Key, item.Value)).ToArray();
            byte[] file;
            string filename;

            try
            {
                file = _exportsService.ExportReport(webReport, exportParams, exportFormat, out filename);
            }
            catch(Exception ex)
            {
                return new UnsupportedMediaTypeResult();
            }
    
            return new FileContentResult(file, "application/octet-stream")
            {
                FileDownloadName = $"{filename}.{exportFormat}"
            };
        }

        public sealed class ExportSettingsParams
        {
            public string ReportId { get; set; }

            public string Format { get; set; }
        }


        [HttpPost]
        [Route("/_fr/exportsettings.getSettings")]
        public IActionResult GetExportSettings([FromQuery] ExportSettingsParams query)
        {
            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();
            
            var msg = _exportsService.GetExportSettings(webReport, query.Format);

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
    }
}
