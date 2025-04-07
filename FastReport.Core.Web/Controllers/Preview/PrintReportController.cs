using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {

        internal sealed class PrintReportParams
        {
            public string ReportId { get; init; }

            public string PrintMode { get; init; }
        }


        [HttpGet("/preview.printReport")]
        public static IResult PrintReport([FromQuery] PrintReportParams query,
            IReportService reportService,
            IPrintService printService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return Results.NotFound();

            var printMode = query.PrintMode.ToLower();
            var response = printService.PrintReport(webReport, printMode);

            if (!(response is null))
            {
                switch (printMode)
                {
                    case "html":
                        return Results.File(response,
                            contentType: "text/html");
#if !OPENSOURCE
                    case "pdf":
                        return Results.File(response,
                            contentType: "application/pdf");
#endif
                }
            }

            return Results.StatusCode((int)HttpStatusCode.UnsupportedMediaType);
        }
    }
}
