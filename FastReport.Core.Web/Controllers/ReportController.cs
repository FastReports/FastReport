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

namespace FastReport.Web.Controllers
{
    class ReportController : BaseController
    {
        #region Routes

        public ReportController()
        {

            RegisterHandler("/preview.getReport", async () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

#if DIALOGS
                webReport.Dialogs(Request);

                if (webReport.Canceled)
                    return new OkResult();

                if (webReport.Mode == WebReportMode.Dialog)
                {
                    webReport.Report.PreparePhase1();
                }
                else
#endif
                if (!webReport.ReportPrepared && Request.Query["skipPrepare"].ToString() != "yes" || Request.Query["forceRefresh"].ToString() == "yes")
                {
                    webReport.Report.Prepare();

                    webReport.SplitReportPagesByTabs();
                }

                webReport.SetReportTab(Request);
                webReport.SetReportPage(Request);
                webReport.SetReportZoom(Request);
                webReport.ProcessClick(Request);


                var renderBody = Request.Query["renderBody"].ToString() == "yes";
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = (webReport.Render(renderBody)).ToString(),
                };
            });

            RegisterHandler("/preview.touchReport", () =>
            {
                var reportId = Request.Query["reportId"].ToString();
                WebReportCache.Instance.Touch(reportId);
                return new OkResult();
            });

            RegisterHandler("/preview.getPicture", () =>
            {
                if (!FindWebReport(out WebReport webReport))
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
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                var printMode = Request.Query["printMode"].ToString().ToLower();
                switch (printMode)
                {
                    case "html":
                        return webReport.PrintHtml();
#if !OPENSOURCE
                    case "pdf":
                        return webReport.PrintPdf();
#endif
                }

                return new UnsupportedMediaTypeResult();
            });

            RegisterHandler("/preview.exportReport", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                var exportFormat = Request.Query["exportFormat"].ToString().ToLower();

                // skip extra key/value pairs
                var exportParams = Request.Query.Where(pair => pair.Key != "exportFormat" && pair.Key != "reportId").ToArray();

                using (MemoryStream exportStream = new MemoryStream())
                {
                    try
                    {
                        webReport.ExportReport(exportStream, exportFormat, exportParams);
                    }
                    catch (UnsupportedExportException)
                    {
                        return new UnsupportedMediaTypeResult();
                    }

                    var filename = webReport.GetCurrentTabName();
                    if (filename.IsNullOrWhiteSpace())
                        filename = "report";

                    return new FileContentResult(exportStream.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = $"{filename}.{exportFormat}"
                    };
                }
            });

#if DIALOGS
            RegisterHandler("/dialog", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                webReport.Dialogs(Request);

                return new OkResult();
            });
#endif

            RegisterHandler("/preview.textEditForm", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                var click = Request.Query["click"].ToString();
                if (!click.IsNullOrWhiteSpace())
                {
                    var @params = click.Split(',');
                    if (@params.Length == 4)
                    {
                        if (int.TryParse(@params[1], out var pageN) &&
                            float.TryParse(@params[2], out var left) &&
                            float.TryParse(@params[3], out var top))
                        {
                            string result = null;

                            webReport.Report.FindClickedObject<TextObject>(@params[0], pageN, left, top,
                                (textObject, reportPage, _pageN) =>
                                {
                                    webReport.Res.Root("Buttons");
                                    string okText = webReport.Res.Get("Ok");
                                    string cancelText = webReport.Res.Get("Cancel");
                                    result = Template_textedit_form(textObject.Text, okText, cancelText);
                                });

                            if (result != null)
                            {
                                return new ContentResult()
                                {
                                    StatusCode = (int)HttpStatusCode.OK,
                                    ContentType = "text/html",
                                    Content = result,
                                };
                            }
                        }
                    }
                }

                return new NotFoundResult();
            });

            RegisterHandler("/exportsettings.getSettings", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                var format = Request.Query["format"];   // pdf

                string msg = string.Empty;
                switch (format)
                {
                    case "image":
                        msg = webReport.template_ImageExportSettings();
                        break;
                    case "html":
                        msg = webReport.template_HtmlExportSettings();
                        break;
#if !OPENSOURCE
                    case "pdf":
                        msg = webReport.template_PdfExportSettings();
                        break;
                    case "docx":
                        msg = webReport.template_DocxExportSettings();
                        break;
                    case "xlsx":
                        msg = webReport.template_XlsxExportSettings();
                        break;
                    case "ods":
                        msg = webReport.template_OdsExportSettings();
                        break;
                    case "odt":
                        msg = webReport.template_OdtExportSettings();
                        break;
                    case "svg":
                        msg = webReport.template_SvgExportSettings();
                        break;
                    case "rtf":
                        msg = webReport.template_RtfExportSettings();
                        break;
                    case "xml":
                        msg = webReport.template_XmlExportSettings();
                        break;
                    case "pptx":
                        msg = webReport.template_PptxExportSettings();
                        break;
#endif
                }

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

#region Private Methods

        bool FindWebReport(out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(Request.Query["reportId"].ToString());
            return webReport != null;
        }

        string Template_textedit_form(string text, string okText, string cancelText) => $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Text edit</title>
    <style>
body {{
    margin: 8px;
    height: calc(100vh - 16px);
}}
textarea {{
    width: 100%;
    height: calc(100% - 42px);
    box-sizing : border-box;
}}
button {{
    float: right;
    margin-left: 8px;
    margin-top: 8px;
    height: 30px;
}}
    </style>
</head>
<body>
    <textarea autofocus>{HttpUtility.HtmlEncode(text)}</textarea>
    <br>
    <button onclick=""window.close();"">{HttpUtility.HtmlEncode(cancelText)}</button>
    <button onclick=""window.postMessage('submit', '*');"">{HttpUtility.HtmlEncode(okText)}</button>
</body>
</html>
";

#endregion
    }
}