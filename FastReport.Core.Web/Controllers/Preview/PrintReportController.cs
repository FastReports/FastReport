using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FastReport.Web.Controllers
{
    [Route("/_fr/preview.printReport")]
    public sealed class PrintReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IPrintService _printService;

        public PrintReportController(IReportService reportService, IPrintService printService)
        {
            _printService = printService;
            _reportService = reportService;
        }

        public sealed class PrintReportParams
        {
            public string ReportId { get; set; }

            public string PrintMode { get; set; }
        }

        [HttpGet]
        public IActionResult PrintReport([FromQuery] PrintReportParams query)
        {
            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var printMode = query.PrintMode.ToLower();
            var response = _printService.PrintReport(webReport, printMode);

            if (!(response is null))
            {
                switch (printMode)
                {
                    case "html":
                        return new FileContentResult(response, "text/html");
#if !OPENSOURCE
                    case "pdf":
                        return new FileContentResult(response, "application/pdf");
#endif
                }
            }

            return new UnsupportedMediaTypeResult();
        }
    }
}
