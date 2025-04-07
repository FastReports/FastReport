using FastReport.Web.Infrastructure;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;
using System.Threading.Tasks;
using FastReport.Utils;
using System.Threading;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        private const string INVALID_REPORT_MESSAGE = "Error loading report: The report structure is invalid.";

        [HttpPost("/preview.getReport")]
        public static async Task<IResult> GetReport([FromQuery] string reportId,
            IReportService reportService,
            HttpRequest request,
            CancellationToken cancellationToken)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out WebReport webReport))
                return Results.NotFound();

            var query = GetReportServiceParams.ParseRequest(request);

            try
            {
#if !OPENSOURCE
                if (!reportService.SearchText(webReport, query.ReportSearchParams))
                    return Results.NotFound();
#endif
                string render = await reportService.GetReportAsync(webReport, query, cancellationToken);

                if (render.IsNullOrEmpty())
                    return Results.Ok();

                return Results.Content(render,
                    contentType: MediaTypeNames.Text.Html);
            }
            catch (CompilerException e)
            {
                if (webReport.Debug)
                    return Results.BadRequest(e.Message);
                else
                    return Results.BadRequest(INVALID_REPORT_MESSAGE);
            }
        }


        [HttpPost("/preview.touchReport")]
        public static IResult TouchReport(string reportId, IReportService reportService)
        {
            reportService.Touch(reportId);
            return Results.Ok();
        }


        [HttpPost("/preview.toolbarElementClick")]
        public static async Task<IResult> GetReportAfterElementClick(string reportId, string elementId, IReportService reportService,
            string inputValue = default)
        {
            if (!reportService.TryFindWebReport(reportId, out WebReport webReport))
                return Results.NotFound();

            var updatedWebreport = await reportService.InvokeCustomElementAction(webReport, elementId, inputValue);

            return Results.Content(updatedWebreport,
                contentType: MediaTypeNames.Text.Html);
        }

    }
}
