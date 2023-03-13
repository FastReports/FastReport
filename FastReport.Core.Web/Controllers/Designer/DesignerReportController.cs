#if DESIGNER
using FastReport.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers
{
    public sealed class DesignerReportController : ApiControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IReportDesignerService _reportDesignerService;

        public DesignerReportController(IReportService reportService, IReportDesignerService reportDesignerService)
        {
            _reportService = reportService;
            _reportDesignerService = reportDesignerService;
        }

        [HttpGet]
        [Route("/_fr/designer.getReport")]
        public IActionResult GetReport(string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            var report = _reportDesignerService.GetDesignerReport(webReport);

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
                Content = report,
            };
        }

        [HttpPost]
        [Route("/_fr/designer.saveReport")]
        public async Task<IActionResult> SaveReport(string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            string contentType = "text/html";

            var saveReportParams = SaveReportServiceParams.ParseRequest(Request);

            var result = await _reportDesignerService.SaveReportAsync(webReport, saveReportParams);

            if (webReport.Designer.SaveMethod == null)
            {
                if (result.Msg.IsNullOrEmpty())
                {
                    return new ContentResult()
                    {
                        ContentType = contentType,
                        StatusCode = result.Code,
                    };
                }
                else
                {
                    return new ContentResult()
                    {
                        ContentType = contentType,
                        StatusCode = result.Code,
                        Content = result.Msg
                    };
                }
            }
            else
            {
                return new ContentResult()
                {
                    StatusCode = result.Code,
                    ContentType = "text/html",
                    Content = result.Msg
                };
            }
        }

        [HttpPost]
        [Route("/_fr/designer.previewReport")]
        public async Task<IActionResult> GetPreviewReport(string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            string receivedReportString = await _reportDesignerService.GetPOSTReportAsync(HttpContext.Request.Body);
            string response = default;

            try
            {
                response = await _reportDesignerService.DesignerMakePreviewAsync(webReport, receivedReportString);
            }
            catch (Exception ex)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/html",
                    Content = ex.Message
                };
            }

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
                Content = response,
            };
        }
    }
}
#endif