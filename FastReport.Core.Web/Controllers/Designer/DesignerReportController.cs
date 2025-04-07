#if DESIGNER
using FastReport.Web.Services;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using FastReport.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Threading;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        [HttpPost("/designer.createReport")]
        public static async Task<IResult> CreateReport(HttpRequest request,
            IReportDesignerService reportDesignerService,
            CancellationToken cancellationToken)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            var webReportId = await reportDesignerService.CreateReport(request.Body, cancellationToken);
            return Results.Ok(webReportId);
        }

        [HttpGet("/designer.getReport")]
        public static IResult GetReport(string reportId,
            HttpRequest request, 
            IReportDesignerService reportDesignerService,
            IReportService reportService)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out WebReport webReport))
                return Results.NotFound();

            var report = reportDesignerService.GetDesignerReport(webReport);

            return Results.Content(report, "text/html");
        }

        [HttpPost("/designer.saveReport")]
        public static async Task<IResult> SaveReport(string reportId, HttpRequest request, IReportService reportService,
            IReportDesignerService reportDesignerService)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out var webReport))
                return Results.NotFound();

            const string contentType = "text/html";
            var saveReportParams = SaveReportServiceParams.ParseRequest(request);
            var result = await reportDesignerService.SaveReportAsync(webReport, saveReportParams);

            if (webReport.Designer.SaveMethod == null)
            {
                return result.Msg.IsNullOrEmpty()
                    ? Results.Ok()
                    : Results.Content(result.Msg, contentType);
            }

            return Results.Content(result.Msg, contentType);
        }

        [HttpPost("/designer.previewReport")]
        public static async Task<IResult> GetPreviewReport(string reportId, HttpRequest request, IReportService reportService, IReportDesignerService reportDesignerService, CancellationToken cancellationToken)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(reportId, out var webReport))
                return Results.NotFound();

            string response;
            try
            {
                response = await reportDesignerService.DesignerMakePreviewAsync(webReport, request.Body, cancellationToken);
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }

            return Results.Content(response, MediaTypeNames.Text.Html);
        }
    }
}
#endif