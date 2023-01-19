using FastReport.Web.Cache;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers.Designer
{
    public sealed class ReportController : ApiControllerBase
    {
        public ReportController()
        {

        }

        #region Routes
        [HttpGet]
        [Route("/_fr/designer.getReport")]
        public IActionResult GetReport(string reportId)
        {
            if (!TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();
            
            return webReport.DesignerGetReport();
        }

        [HttpPost]
        [Route("/_fr/designer.saveReport")]
        public IActionResult SaveReport(string reportId)
        {
            if (!TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();
            
            if (webReport.Designer.SaveMethod == null)
            {
                // old saving way by self-request
                return webReport.DesignerSaveReport(HttpContext);
            }
            else
            {
                // save by using a Func

                string report = webReport.GetPOSTReport(HttpContext);
                report = WebReport.FixLandscapeProperty(report);
                string msg = string.Empty;
                int code = 200;
                try
                {
                    msg = webReport.Designer.SaveMethod(webReport.ID, webReport.ReportFileName, report);
                }
                catch (Exception ex)
                {
                    code = 500;
                    msg = ex.Message;
                }

                var result = new ContentResult()
                {
                    StatusCode = code,
                    ContentType = "text/html",
                    Content = msg
                };
                return result;
            }
        }

        [HttpPost]
        [Route("/_fr/designer.previewReport")]
        public async Task<IActionResult> GetPreviewReport(string reportId)
        {
            if (!TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            return await webReport.DesignerMakePreview(HttpContext);
        }
        #endregion

        #region PrivateMethods
        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }
        #endregion
    }
}
