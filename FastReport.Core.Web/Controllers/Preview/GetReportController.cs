using FastReport.Web.Cache;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class GetReportController : ApiControllerBase
    {

        public GetReportController()
        {
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
        public IActionResult GetReport([FromQuery] GetReportParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

#if DIALOGS
            webReport.Dialogs(Request);

            if (webReport.Canceled)
                return new OkResult();

            if (webReport.Mode == WebReportMode.Dialog)
            {
                webReport.Report.PreparePhase1();
            }
            else
#endif
            if (!webReport.ReportPrepared && query.SkipPrepare != "yes" || query.ForceRefresh == "yes")
            {
                webReport.Report.Prepare();

                webReport.SplitReportPagesByTabs();
            }

            webReport.SetReportTab(Request);
            webReport.SetReportPage(Request);
            webReport.SetReportZoom(Request);
            webReport.ProcessClick(Request);


            //bool renderBodyBool = query.RenderBody ?? default;
            bool renderBodyBool = query.RenderBody == "yes";
            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
                Content = (webReport.Render(renderBodyBool)).ToString(),
            };
        }

        [HttpPost]
        [Route("/_fr/preview.touchReport")]
        public IActionResult TouchReport(string reportId)
        {
            WebReportCache.Instance.Touch(reportId);
            return Ok();
        }

        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

    }
}
