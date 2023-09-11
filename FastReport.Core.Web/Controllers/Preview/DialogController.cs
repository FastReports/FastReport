#if DIALOGS
using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {

        [HttpPost("/dialog")]
        public static IResult TouchDialog([FromQuery] string reportId,
            IReportService reportService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out WebReport webReport))
                return Results.NotFound();

            var dialogParams = new DialogParams();
            dialogParams.ParseRequest(request);

            webReport.Dialogs(dialogParams);

            return Results.Ok();
        }
    }
}
#endif
