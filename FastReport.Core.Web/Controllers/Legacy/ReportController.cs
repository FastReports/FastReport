using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.Image;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Reflection;
using System.Globalization;
using FastReport.Web.Cache;
using FastReport.Web.Services;
using System.Collections.Generic;

namespace FastReport.Web.Controllers
{
    class ReportController : BaseController
    {
        #region Routes

        public ReportController()
        {

            RegisterHandler("/preview.getReport", async () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var getReportParams = new GetReportServiceParams
                {
                    SkipPrepare = Request.Query["skipPrepare"].ToString(),
                    ForceRefresh = Request.Query["forceRefresh"].ToString(),
                    RenderBody = Request.Query["renderBody"].ToString(),
                };

                getReportParams.ParseRequest(Request);

                var render = ReportService.Instance.GetReport(webReport, getReportParams);

                if (render.IsNullOrEmpty())
                    return new OkResult();

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = render,
                };
            });

            RegisterHandler("/preview.touchReport", () =>
            {
                var reportId = Request.Query["reportId"].ToString();
                ReportService.Instance.Touch(reportId);
                return new OkResult();
            });

            RegisterHandler("/preview.getPicture", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var pictureId = Request.Query["pictureId"].ToString().TrimStart('=');
                if (webReport.PictureCache.TryGetValue(pictureId, out byte[] value))
                {
                    string imgType = WebUtils.IsPng(value) ? "image/png" : "image/svg+xml";
                    return new FileContentResult(value, imgType);
                }

                return new NotFoundResult();
            });

            RegisterHandler("/preview.printReport", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var printMode = Request.Query["printMode"].ToString().ToLower();

                var response = PrintService.Instance.PrintReport(webReport, printMode);

                if (!(response is null))
                {
                    switch (printMode)
                    {
                        case "html":
                            return new FileContentResult(response, "text/html");
#if !OPENSOURCE
                        case "pdf":
                            return new FileContentResult(response, "application/pdf");
#endif
                    }
                }

                return new UnsupportedMediaTypeResult();
            });

            RegisterHandler("/preview.exportReport", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var exportFormat = Request.Query["exportFormat"].ToString().ToLower();

                // skip extra key/value pairs
                var exportParams = Request.Query.Where(pair => pair.Key != "exportFormat" && pair.Key != "reportId")
                    .Select(item => new KeyValuePair<string, string>(item.Key, item.Value)).ToArray();

                byte[] file;
                string filename;

                try
                {
                    file = ExportService.Instance.ExportReport(webReport, exportParams, exportFormat, out filename);
                }
                catch (Exception ex)
                {
                    return new UnsupportedMediaTypeResult();
                }

                return new FileContentResult(file, "application/octet-stream")
                {
                    FileDownloadName = $"{filename}.{exportFormat}"
                };
            });

#if DIALOGS
            RegisterHandler("/dialog", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var dialogParams = new DialogParams();
                dialogParams.ParseRequest(Request);

                webReport.Dialogs(dialogParams);

                return new OkResult();
            });
#endif

            RegisterHandler("/preview.textEditForm", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var result = TextEditService.Instance.GetTemplateTextEditForm(Request.Query["click"].ToString(), webReport);

                if (result != null)
                {
                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "text/html",
                        Content = result,
                    };
                }
                else
                {
                    return new NotFoundResult();
                }
            });

            RegisterHandler("/exportsettings.getSettings", () =>
            {
                if (!ReportService.Instance.TryFindWebReport(Request.Query["reportId"].ToString(), out WebReport webReport))
                    return new NotFoundResult();

                var format = Request.Query["format"];

                var msg = ExportService.Instance.GetExportSettings(webReport, format);

                if (msg != null)
                {
                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "text/html",
                        Content = msg,
                    };
                }
                return new NotFoundResult();
            });

        }

#endregion
    }
}