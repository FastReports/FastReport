using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.Image;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net;
using System.Web;
#if  !OPENSOURCE
using FastReport.Export.Csv;
using FastReport.Export.Dbf;
using FastReport.Export.Json;
using FastReport.Export.Mht;
using FastReport.Export.Odf;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.Export.Ppml;
using FastReport.Export.PS;
using FastReport.Export.RichText;
using FastReport.Export.Svg;
using FastReport.Export.Text;
using FastReport.Export.XAML;
using FastReport.Export.Xml;
#endif

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

                webReport.Dialogs(Request);

                if (webReport.Canceled)
                    return new OkResult();

                if (webReport.Mode != WebReportMode.Dialog)
                {
                    if (!webReport.ReportPrepared && Request.Query["skipPrepare"].ToString() != "yes")
                        webReport.Report.Prepare();
                }
                else
                    webReport.Report.PreparePhase1();

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
#if  !OPENSOURCE
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

                using (var export = GetExport(exportFormat))
                using (var ms = new MemoryStream())
                {
                    if (exportFormat == "fpx")
                        webReport.Report.SavePrepared(ms);
                    else if (export != null)
                        export.Export(webReport.Report, ms);
                    else
                        return new UnsupportedMediaTypeResult();

                    ms.Position = 0;

                    var filename = webReport.GetCurrentTabName();
                    if (filename.IsNullOrWhiteSpace())
                        filename = "report";

                    return new FileContentResult(ms.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = $"{filename}.{exportFormat}"
                    };
                }
            });

            RegisterHandler("/dialog", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                webReport.Dialogs(Request);

                return new OkResult();
            });

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
        }

#endregion

#region Private Methods

        bool FindWebReport(out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(Request.Query["reportId"].ToString());
            return webReport != null;
        }

        ExportBase GetExport(string exportFormat)
        {
            ExportBase export = null;

            switch (exportFormat)
            {
#if  !OPENSOURCE
                 case "pdf":
                    export = new PDFExport();
                    break;
                case "rtf":
                    export = new RTFExport();
                    break;
                  case "mht":
                    export = new MHTExport();
                    break;
                case "xml":
                    export = new XMLExport();
                    break;
                case "xlsx":
                    export = new Excel2007Export();
                    break;
                case "docx":
                    export = new Word2007Export();
                    break;
                case "pptx":
                    export = new PowerPoint2007Export();
                    break;
                case "ods":
                    export = new ODSExport();
                    break;
                case "odt":
                    export = new ODTExport();
                    break;
                case "xps":
                    export = new XPSExport();
                    break;
                case "csv":
                    export = new CSVExport();
                    break;
                case "dbf":
                    export = new DBFExport();
                    break;
                case "txt":
                    export = new TextExport();
                    break;
                case "xaml":
                    export = new XAMLExport();
                    break;
                case "svg":
                    export = new SVGExport();
                    break;
                case "ppml":
                    export = new PPMLExport();
                    break;
                case "ps":
                    export = new PSExport();
                    break;
                case "json":
                    export = new JsonExport();
                    break;
#endif
                case "html":
                    export = new HTMLExport();
                    break;        
                case "bmp":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Bmp };
                    break;
                case "gif":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Gif };
                    break;
                case "jpg":
                case "jpeg":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Jpeg };
                    break;
                case "emf":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Metafile };
                    break;
                case "png":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Png };
                    break;
                case "tif":
                case "tiff":
                    export = new ImageExport() { ImageFormat = ImageExportFormat.Tiff };
                    break;
            }

            return export;
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