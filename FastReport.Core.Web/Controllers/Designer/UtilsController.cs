#if DESIGNER
using FastReport.Web.Services;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;
using FastReport.Web.Infrastructure;
using Microsoft.AspNetCore.Http;
using System.Net.Mime;
using System.Collections.Generic;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        [HttpGet("/designer.objects/mschart/template")]
        public static IResult GetMSChartTemplate(string name, IDesignerUtilsService designerUtilsService)
        {
            string response;

            try
            {
                response = designerUtilsService.GetMSChartTemplateXML(name);
            }
            catch (Exception ex)
            {
                return Results.NotFound();
            }

            return Results.Content(response, "application/xml");
        }

        [HttpGet("/designer.getComponentProperties")]
        public static IResult GetComponentProperties(string name, IDesignerUtilsService designerUtilsService)
        {
            var response = designerUtilsService.GetPropertiesJSON(name);

            return response.IsNullOrEmpty()
                ? Results.NotFound()
                : Results.Content(response, "application/json");
        }

        [HttpGet("/designer.getConfig")]
        public static IResult GetConfig(string reportId, IReportService reportService, IDesignerUtilsService designerUtilsService)
        {
            if (!reportService.TryFindWebReport(reportId, out var webReport))
                return Results.NotFound();

            var content = designerUtilsService.GetConfig(webReport);

            return Results.Content(content, "application/json");
        }

        [HttpGet("/designer.getFunctions")]
        public static IResult GetFunctions(string reportId,
            IReportService reportService, 
            IDesignerUtilsService designerUtilsService)
        {
            if (!reportService.TryFindWebReport(reportId, out var webReport))
                return Results.NotFound();

            var buff = designerUtilsService.GetFunctions(webReport.Report);

            return Results.Content(buff, "application/xml");
        }

        [HttpPost("/designer.objects/preview")]
        public static async Task<IResult> GetDesignerObjectPreview(string reportId, 
            IReportService reportService, 
            IReportDesignerService reportDesignerService,
            IDesignerUtilsService designerUtilsService,
            HttpRequest request)
        {
            if (!reportService.TryFindWebReport(reportId, out var webReport))
                return Results.NotFound();

            try
            {
                var reportObj = await reportDesignerService.GetPOSTReportStringAsync(request.Body);
                var response = designerUtilsService.DesignerObjectPreview(webReport, reportObj);

                return Results.Content(response, MediaTypeNames.Text.Html);
            }
            catch (Exception ex)
            {
                var content = webReport.Debug ? ex.Message : "";

                return Results.BadRequest(content); 
            }
        }

        [HttpGet("/designer.getClassDetails")]
        public static IResult GetClassDetails(string className, IDesignerUtilsService designerUtilsService,
            [FromServices] DesignerOptions designerOptions)
        {
            if (!designerOptions.EnableIntelliSense)
                return Results.BadRequest();

            var result = designerUtilsService.GetClassDetailsJson(className);

            return result is null ? 
                Results.NotFound() : 
                Results.Content(result, "application/json");
        }

        [HttpPost("/designer.getNamespacesInfo")]
        public static IResult GetNamespacesInfo([FromBody] IReadOnlyCollection<string> namespaces, IDesignerUtilsService designerUtilsService,
            [FromServices] DesignerOptions designerOptions)
        {
            if (!designerOptions.EnableIntelliSense)
                return Results.BadRequest();

            var result = designerUtilsService.GetNamespacesInfoJson(namespaces);

            return Results.Content(result, "application/json");
        }
    }
}
#endif