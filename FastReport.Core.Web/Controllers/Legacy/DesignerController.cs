#if DESIGNER
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;

namespace FastReport.Web.Controllers
{
    class DesignerController : BaseController
    {
        #region Routes

        public DesignerController() : base()
        {
            RegisterHandler("/designer.getReport", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var report = ReportDesignerService.Instance.GetDesignerReport(webReport);

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = report,
                };
            });

            RegisterHandler("/designer.saveReport", async () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                string contentType = "text/html";

                var saveReportParams = SaveReportServiceParams.ParseRequest(Request);

                var result = await ReportDesignerService.Instance.SaveReportAsync(webReport, saveReportParams);

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
            });

            RegisterHandler("/designer.previewReport", async () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                string receivedReportString = await ReportDesignerService.Instance.GetPOSTReportAsync(Context.Request.Body);
                string response = default;
                
                try
                {
                    response = await ReportDesignerService.Instance.DesignerMakePreviewAsync(webReport, receivedReportString);
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
            });

            RegisterHandler("/designer.getConfig", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = webReport.Designer.Config.IsNullOrWhiteSpace() ? "{}" : webReport.Designer.Config,
                };
            });

            RegisterHandler("/designer.getFunctions", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var buff = DesignerUtilsService.Instance.GetFunctions(webReport.Report);

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/xml",
                    Content = buff,
                };
            });

            RegisterHandler("/designer.getConnectionTypes", () =>
            {
                var names = ConnectionService.Instance.GetConnectionTypes();

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "{" + String.Join(",", names.ToArray()) + "}",
                };
            });

            RegisterHandler("/designer.getConnectionTables", () =>
            {
                var connectionType = Request.Query["connectionType"].ToString();
                var connectionString = Request.Query["connectionString"].ToString();
                var response = ConnectionService.Instance.GetConnectionTables(connectionType, connectionString, out bool isError);

                return isError ? new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = response,
                }
                : new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/xml",
                    Content = response
                };
            });

            RegisterHandler("/designer.getConnectionStringProperties", () =>
            {
                var connectionType = Request.Query["connectionType"].ToString();
                var connectionString = Request.Query["connectionString"].ToString();
                var response = ConnectionService.Instance.GetConnectionStringPropertiesJSON(connectionType, connectionString, out bool isError);

                return isError ? new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = response,
                }
                : new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/xml",
                    Content = response
                };
            });

            RegisterHandler("/designer.makeConnectionString", () =>
            {
                var connectionType = Request.Query["connectionType"].ToString();
                var form = Request.Form;
                var response = ConnectionService.Instance.CreateConnectionStringJSON(connectionType, form, out bool isError);

                return isError ? new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = response,
                }
                : new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/xml",
                    Content = response
                };
            });

            RegisterHandler("/designer.objects/mschart/template", () =>
            {
                var resourceName = Request.Query["name"].ToString();
                var response = DesignerUtilsService.Instance.GetMSChartTemplateXML(resourceName);

                if (response.IsNullOrEmpty())
                    return new NotFoundResult();
                else
                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "application/xml",
                        Content = response
                    };
            });

            RegisterHandler("/designer.getComponentProperties", () =>
            {
                var componentName = Request.Query["name"].ToString();
                string responseJson = DesignerUtilsService.Instance.GetPropertiesJSON(componentName);

                if (responseJson.IsNullOrEmpty())
                    return new NotFoundResult();
                else
                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "application/json",
                        Content = responseJson
                    };

            });
        }
    }
}
        #endregion
#endif