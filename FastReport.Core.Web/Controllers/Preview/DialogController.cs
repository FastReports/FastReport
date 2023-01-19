#if DIALOGS
using FastReport.Web.Cache;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class DialogController : ApiControllerBase
    {

        public DialogController()
        {
        }

        public sealed class DialogChangeParams
        {
            public string ReportId { get; set; }
        }


        [HttpPost]
        [Route("/_fr/dialog")]
        public IActionResult TouchDialog([FromQuery] DialogChangeParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            webReport.Dialogs(Request);
            return Ok();
        }

        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

    }
}
#endif
