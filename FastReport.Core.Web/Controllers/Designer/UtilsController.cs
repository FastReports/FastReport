#if DESIGNER
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class UtilsController : ApiControllerBase
    {
        private readonly IDesignerUtilsService _designerUtilsService;
        private readonly IReportService _reportService;

        public UtilsController(IDesignerUtilsService designerUtilsService, IReportService reportService)
        {
            _designerUtilsService = designerUtilsService;
            _reportService = reportService;
        }

        #region Routes
        [HttpGet]
        [Route("/_fr/designer.objects/mschart/template")]
        public IActionResult GetMSChartTemplate(string name)
        {
            string response = string.Empty;

            try
            {
                response = _designerUtilsService.GetMSChartTemplateXML(name);
            }
            catch (Exception ex)
            {
                return new NotFoundResult();
            }

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = response
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getComponentProperties")]
        public IActionResult GetComponentProperties(string name)
        {
            string response = _designerUtilsService.GetPropertiesJSON(name);

            if (response.IsNullOrEmpty())
                return new NotFoundResult();
            else
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = response
                };
        }

        [HttpGet]
        [Route("/_fr/designer.getConfig")]
        public IActionResult GetConfig(string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = webReport.Designer.Config.IsNullOrWhiteSpace() ? "{}" : webReport.Designer.Config,
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getFunctions")]
        public IActionResult GetFunctions(string reportId)
        {
            if (!_reportService.TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            var buff = _designerUtilsService.GetFunctions(webReport.Report);

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = buff,
            };
        }
        #endregion
    }
}
#endif