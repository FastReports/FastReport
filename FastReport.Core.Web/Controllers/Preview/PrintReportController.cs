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
    [Route("/_fr/preview.printReport")]
    public sealed class PrintReportController : ApiControllerBase
    {

        public PrintReportController()
        {
        }

        public sealed class PrintReportParams
        {
            public string ReportId { get; set; }

            public string PrintMode { get; set; }
        }

        [HttpGet]
        public IActionResult PrintReport([FromQuery] PrintReportParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var printMode = query.PrintMode.ToLower();
            switch (printMode)
            {
                case "html":
                    return webReport.PrintHtml();
#if !OPENSOURCE
                case "pdf":
                    return webReport.PrintPdf();
#endif
            }

            return new UnsupportedMediaTypeResult();
        }

        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

    }
}
