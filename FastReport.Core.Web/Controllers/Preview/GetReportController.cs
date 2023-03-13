using FastReport.Web.Cache;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace FastReport.Web.Controllers
{
    public sealed class GetReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public GetReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public sealed class GetReportParams
        {
            public string ReportId { get; set; }

            public string RenderBody { get; set; }

            //public bool RenderBody { get; set; } = false;

            public string SkipPrepare { get; set; }

            public string ForceRefresh { get; set; }

        }

        [HttpPost]
        [Route("/_fr/preview.getReport")]
        public IActionResult GetReport(string reportId, [FromQuery] GetReportServiceParams query)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            query.ParseRequest(Request);

            string render = _reportService.GetReport(webReport, query);

            if (render.IsNullOrEmpty())
                return new OkResult();

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
                Content = render,
            };
        }

        [HttpPost]
        [Route("/_fr/preview.touchReport")]
        public IActionResult TouchReport(string reportId)
        {
            _reportService.Touch(reportId);
            return Ok();
        }
    }
}
