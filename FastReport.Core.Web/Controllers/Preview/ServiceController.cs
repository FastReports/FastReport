using FastReport.Web.Cache;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class ServiceController : ApiControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ITextEditService _textEditService;

        public ServiceController(IReportService reportService, ITextEditService textEditService)
        {
            _reportService = reportService;
            _textEditService = textEditService;
        }

        public sealed class PrintReportParams
        {
            public string ReportId { get; set; }

            public string Click { get; set; }
        }

        [HttpGet]
        [Route("/_fr/preview.textEditForm")]
        public IActionResult TextEditForm([FromQuery] PrintReportParams query)
        {
            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var result = _textEditService.GetTemplateTextEditForm(query.Click, webReport);

            if (result != null)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = result,
                };
            }
            else
            {
                return new NotFoundResult();
            }
        }
    }
}
