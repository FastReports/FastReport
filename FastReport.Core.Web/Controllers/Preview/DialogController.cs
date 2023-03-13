#if DIALOGS
using FastReport.Web.Cache;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class DialogController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public DialogController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpPost]
        [Route("/_fr/dialog")]
        public IActionResult TouchDialog([FromQuery] string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            var dialogParams = new DialogParams();
            dialogParams.ParseRequest(Request);

            webReport.Dialogs(dialogParams);

            return Ok();
        }
    }
}
#endif
