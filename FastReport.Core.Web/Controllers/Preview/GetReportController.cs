using FastReport.Web.Infrastructure;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {

        [HttpPost("/preview.getReport")]
        public static IResult GetReport([FromQuery] string reportId,
            IReportService reportService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out WebReport webReport))
                return Results.NotFound();

            var query = GetReportServiceParams.ParseRequest(request);

            string render = reportService.GetReport(webReport, query);

            if (render.IsNullOrEmpty())
                return Results.Ok();

            return Results.Content(render,
                contentType: MediaTypeNames.Text.Html);
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
