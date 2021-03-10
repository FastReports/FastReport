using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Web
{
    partial class WebReport
    {
        #region Public Properties

        /// <summary>
        /// Enable code editor in the Report Designer
        /// </summary>
        public bool DesignScriptCode { get; set; } = false;

        /// <summary>
        /// Gets or sets path to the Report Designer
        /// </summary>
        
        public string DesignerPath { get; set; } = "/WebReportDesigner/index.html";

        /// <summary>
        /// Gets or sets path to a folder for save designed reports
        /// If value is empty then designer posts saved report in variable ReportFile on call the DesignerSaveCallBack // TODO
        /// </summary>
        public string DesignerSavePath { get; set; } = "";

        /// <summary>
        /// Gets or sets path to callback page after Save from Designer
        /// </summary>
        [Obsolete("DesignerSaveCallBack is obsolete, please use DesignerSaveMethod instead.")]
        public string DesignerSaveCallBack { get; set; } = "";

        /// <summary>
        /// Callback method for saving an edited report by Online Designer
        /// Params: reportID, report file name, report, out - message
        /// </summary>
        /// <example>
        /// webReport.DesignerSaveMethod = (string reportID, string filename, string report) =>
        /// {
        ///     string webRootPath = _hostingEnvironment.WebRootPath;
        ///     string pathToSave = Path.Combine(webRootPath, filename);
        ///     System.IO.File.WriteAllTextAsync(pathToSave, report);
        ///     
        ///     return "OK";
        /// };
        /// </example>
        public Func<string, string, string, string> DesignerSaveMethod { get; set; }

        /// <summary>
        /// Report name without extension
        /// </summary>
        public string ReportName
        {
            get
            {
                return (!string.IsNullOrEmpty(Report.ReportInfo.Name) ?
                     Report.ReportInfo.Name : Path.GetFileNameWithoutExtension(Report.FileName));
            }
        }

        /// <summary>
        /// Report file name with extension (*.frx)
        /// </summary>
        public string ReportFileName => $"{ReportName}.frx";


        /// <summary>
        /// Gets or sets the locale of Designer
        /// </summary>
        public string DesignerLocale { get; set; } = "";

        /// <summary>
        /// Gets or sets the text of configuration of Online Designer
        /// </summary>
        public string DesignerConfig { get; set; } = "";

        /// <summary>
        /// Gets or sets the request headers
        /// </summary>
        public WebHeaderCollection RequestHeaders { get; set; }

        /// <summary>
        /// Occurs when designed report save is started.
        /// </summary>
        public event EventHandler<SaveDesignedReportEventArgs> SaveDesignedReport;

        #endregion

        #region Public Methods

        /// <summary>
        /// Runs on designed report save
        /// </summary>
        public void OnSaveDesignedReport(SaveDesignedReportEventArgs e)
        {
            SaveDesignedReport?.Invoke(this, e);
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Save report from designer
        /// </summary>
        internal IActionResult DesignerSaveReport(HttpContext context)
        {
            var result = new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
            };

            string reportString = GetPOSTReport(context);

            try
            {
                // paste restricted back in report before save
                Report.LoadFromString(PasteRestricted(reportString));

                if (SaveDesignedReport != null)
                {
                    SaveDesignedReportEventArgs e = new SaveDesignedReportEventArgs();
                    e.Stream = new MemoryStream();
                    Report.Save(e.Stream);
                    e.Stream.Position = 0;
                    OnSaveDesignedReport(e);
                }

                if (!DesignerSaveCallBack.IsNullOrWhiteSpace())
                {
                    string report = Report.SaveToString();
                    string reportFileName = ReportFileName;

                    UriBuilder uri = new UriBuilder
                    {
                        Scheme = context.Request.Scheme,
                        Host = context.Request.Host.Host
                    };

                    //if (!FastReportGlobal.FastReportOptions.CloudEnvironmet)
                    if (context.Request.Host.Port != null)
                        uri.Port = (int)context.Request.Host.Port;
                    else if (uri.Scheme == "https")
                        uri.Port = 443;
                    else
                        uri.Port = 80;

                    // TODO:
                    //uri.Path = webReport.ResolveUrl(webReport.DesignerSaveCallBack);
                    uri.Path = DesignerSaveCallBack;
                    //var designerSaveCallBack = new Uri(DesignerSaveCallBack);
                    //if (!designerSaveCallBack.IsAbsoluteUri)
                    //{
                    //    designerSaveCallBack = new UriBuilder()
                    //    {
                    //        Scheme = context.Request.Scheme,
                    //        Host = context.Request.Host.Host,
                    //        Port = context.Request.Host.Port ?? 80,
                    //        Path = DesignerSaveCallBack,
                    //    }.Uri;
                    //}
                    //uri.Path = designerSaveCallBack.ToString();

                    // TODO: rename param names
                    string queryToAppend = $"reportID={ID}&reportUUID={reportFileName}";

                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
                    else
                        uri.Query = queryToAppend;

                    string callBackURL = uri.ToString();

                    // return "true" to force the certificate to be accepted.
                    ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;

                    WebRequest request = WebRequest.Create(callBackURL);

                    if (request != null)
                    {
                        // set up the custom headers
                        if (RequestHeaders != null)
                            request.Headers = RequestHeaders;

                        WebUtils.CopyCookies(request, context);

                        // TODO: why here??
                        // if save report in reports folder
                        if (!String.IsNullOrEmpty(DesignerSavePath))
                        {
                            string savePath = WebUtils.MapPath(DesignerSavePath); // TODO: do we really need this?
                            if (Directory.Exists(savePath))
                            {
                                File.WriteAllText(Path.Combine(savePath, reportFileName), report, Encoding.UTF8);
                            }
                            else
                            {
                                result = new ContentResult()
                                {
                                    StatusCode = (int)HttpStatusCode.NotFound,
                                    ContentType = "text/html",
                                    Content = "DesignerSavePath does not exist",
                                };
                            }

                            request.Method = "GET";
                        }
                        else
                        // send report directly in POST
                        {
                            request.Method = "POST";
                            request.ContentType = "text/xml";
                            byte[] postData = Encoding.UTF8.GetBytes(report);
                            request.ContentLength = postData.Length;
                            Stream reqStream = request.GetRequestStream();
                            reqStream.Write(postData, 0, postData.Length);
                            postData = null;
                            reqStream.Close();
                        }

                        // Request call-back
                        try
                        {
                            using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                            {
                                //context.Response.StatusCode = (int)resp.StatusCode;
                                //context.Response.Write(resp.StatusDescription);

                                result = new ContentResult()
                                {
                                    StatusCode = (int)resp.StatusCode,
                                    ContentType = "text/html",
                                    Content = resp.StatusDescription,
                                };
                            }
                        }
                        catch (WebException err)
                        {
                            result = new ContentResult()
                            {
                                StatusCode = (int)HttpStatusCode.InternalServerError,
                                ContentType = "text/html",
                            };

                            if (Debug)
                            {
                                using (Stream data = err.Response.GetResponseStream())
                                using (StreamReader reader = new StreamReader(data))
                                {
                                    string text = reader.ReadToEnd();
                                    if (!String.IsNullOrEmpty(text))
                                    {
                                        int startExceptionText = text.IndexOf("<!--");
                                        int endExceptionText = text.LastIndexOf("-->");
                                        if (startExceptionText != -1)
                                            text = text.Substring(startExceptionText + 6, endExceptionText - startExceptionText - 6);

                                        result.Content = text;
                                        result.StatusCode = (int)(err.Response as HttpWebResponse).StatusCode;
                                    }
                                }
                            }
                            else
                            {
                                result.Content = err.Message;
                            }
                        }

                    }
                    request = null;
                }
            }
            catch (Exception e)
            {
                result.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (Debug)
                    result.Content = e.Message;
            }

            return result;
        }

        // send report to the designer
        internal IActionResult DesignerGetReport()
        {
            string reportString = Report.SaveToString();
            string report = CutRestricted(reportString);

            if (report.IndexOf("inherited") != -1)
            {
                List<string> reportInheritance = new List<string>();
                string baseReport = report;

                while (!String.IsNullOrEmpty(baseReport))
                {
                    reportInheritance.Add(baseReport);
                    using (MemoryStream xmlStream = new MemoryStream())
                    {
                        WebUtils.Write(xmlStream, baseReport);
                        xmlStream.Position = 0;
                        using (var xml = new XmlDocument())
                        {
                            xml.Load(xmlStream);
                            string baseReportFile = xml.Root.GetProp("BaseReport");
                            //string fileName = context.Request.MapPath(baseReportFile, webReport.Prop.ReportPath, true);
                            if (!Path.IsPathRooted(baseReportFile))
                                baseReportFile = Path.GetFullPath(Path.GetDirectoryName(Report.FileName) + Path.DirectorySeparatorChar + baseReportFile); //was ReportPath before(ToDo)

                            if (File.Exists(baseReportFile))
                            {
                                baseReport = File.ReadAllText(baseReportFile, Encoding.UTF8);
                            }
                            else
                                baseReport = String.Empty;
                        }
                    }
                }
                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.Append("{\"reports\":[");
                for (int i = reportInheritance.Count - 1; i >= 0; i--)
                {
                    string s = reportInheritance[i];
                    responseBuilder.Append("\"");
                    responseBuilder.Append(s.Replace("\r\n", "").Replace("\"", "\\\""));
                    if (i > 0)
                        responseBuilder.Append("\",");
                    else
                        responseBuilder.Append("\"");
                }
                responseBuilder.Append("]}");

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = responseBuilder.ToString(),
                };
            }

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "text/html",
                Content = report,
            };
        }

        // preview for Designer
        internal async Task<IActionResult> DesignerMakePreview(HttpContext context)
        {
            string receivedReportString = GetPOSTReport(context);

            try
            {
                var previewReport = new WebReport();
                //previewReport.ID = webReport.ID + "-preview";
                previewReport.Report = Report;
                //previewReport.Prop.Assign(webReport.Prop);
                //previewReport.CurrentTab = CurrentTab.Clone();
                //previewReport.LocalizationFile = webReport.LocalizationFile;
                //previewReport.Width = "880px";
                //previewReport.Height = "770px";
                //previewReport.Toolbar.EnableFit = true;
                //previewReport.Layers = true;
                string reportString = PasteRestricted(receivedReportString);
                previewReport.Report.ReportResourceString = reportString; // TODO
                //previewReport.ReportFile = String.Empty;
                previewReport.ReportResourceString = reportString; // TODO
                //previewReport.Report.Prepare();
                //StringBuilder sb = new StringBuilder();
                //sb.Append("<script src=\"").Append(GetResourceTemplateUrl(context, "fr_util.js")).AppendLine("\" type=\"text/javascript\"></script>");
                //HtmlTextWriter writer = new HtmlTextWriter(new StringWriter(sb, System.Globalization.CultureInfo.InvariantCulture));
                //previewReport.ShowZoomButton = false;
                //previewReport.PreviewMode = true;
                previewReport.Mode = WebReportMode.Preview;
                //previewReport.Page = null;
                //previewReport.RenderControl(writer);
                //string responseText = WebReportGlobals.ScriptsAsString() + previewReport.Toolbar.GetCss() + sb.ToString();
                //context.Response.Write(responseText);

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "text/html",
                    Content = (await previewReport.Render()).ToString(),
                };
            }
            catch (Exception e)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/html",
                    Content = Debug ? e.Message : "",
                };
            }
        }

        #endregion

        #region Private Methods

        HtmlString RenderDesigner()
        {
            //string designerPath = WebUtils.GetAppRoot(DesignerPath);
            string designerLocale = DesignerLocale.IsNullOrWhiteSpace() ? "" : $"&lang={DesignerLocale}";

            return new HtmlString($@"
<iframe src=""{DesignerPath}?uuid={ID}{WebUtils.GetARRAffinity()}{designerLocale}"" style=""border:none;"" width=""{Width}"" height=""{Height}"">
    <p style=""color:red"">ERROR: Browser does not support IFRAME!</p>
</iframe >
");
            // TODO: add fit script
        }

        string CutRestricted(string xmlString)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                WebUtils.Write(xmlStream, xmlString);
                xmlStream.Position = 0;

                using (var xml = new XmlDocument())
                {
                    xml.Load(xmlStream);

                    if (!DesignScriptCode)
                    {
                        xml.Root.SetProp("CodeRestricted", "true");
                        // cut script
                        var scriptItem = xml.Root.FindItem("ScriptText");
                        if (scriptItem != null && !String.IsNullOrEmpty(scriptItem.Value))
                            scriptItem.Value = String.Empty;
                    }

                    // cut connection strings
                    var dictionary = xml.Root.FindItem("Dictionary");
                    {
                        if (dictionary != null)
                        {
                            for (int i = 0; i < dictionary.Items.Count; i++)
                            {
                                var item = dictionary.Items[i];
                                if (!String.IsNullOrEmpty(item.GetProp("ConnectionString")))
                                {
                                    item.SetProp("ConnectionString", String.Empty);
                                }
                            }
                        }
                    }

                    // save prepared xml
                    using (MemoryStream secondXmlStream = new MemoryStream())
                    {
                        xml.Save(secondXmlStream);
                        secondXmlStream.Position = 0;
                        byte[] buff = new byte[secondXmlStream.Length];
                        secondXmlStream.Read(buff, 0, buff.Length);
                        xmlString = Encoding.UTF8.GetString(buff);
                    }
                }
            }
            return xmlString;
        }

        string PasteRestricted(string xmlString)
        {
            using (MemoryStream xmlStream1 = new MemoryStream())
            using (MemoryStream xmlStream2 = new MemoryStream())
            {
                WebUtils.Write(xmlStream1, Report.SaveToString());
                WebUtils.Write(xmlStream2, xmlString);
                xmlStream1.Position = 0;
                xmlStream2.Position = 0;
                var xml1 = new XmlDocument();
                var xml2 = new XmlDocument();
                xml1.Load(xmlStream1);
                xml2.Load(xmlStream2);

                if (!DesignScriptCode)
                {
                    xml2.Root.SetProp("CodeRestricted", "");
                    // paste old script
                    var scriptItem1 = xml1.Root.FindItem("ScriptText");
                    if (scriptItem1 != null && String.IsNullOrEmpty(scriptItem1.Value))
                    {
                        var scriptItem2 = xml2.Root.FindItem("ScriptText");
                        if (scriptItem2 != null)
                        {
                            scriptItem2.Value = scriptItem1.Value;
                            scriptItem2.Dispose();
                        }
                        else
                        {
                            xml2.Root.AddItem(scriptItem1);
                        }
                    }
                }

                // paste saved connection strings
                var dictionary1 = xml1.Root.FindItem("Dictionary");
                var dictionary2 = xml2.Root.FindItem("Dictionary");
                    if (dictionary1 != null && dictionary2 != null)
                    {
                        for (int i = 0; i < dictionary1.Items.Count; i++)
                        {
                            var item1 = dictionary1.Items[i];
                            string connectionString = item1.GetProp("ConnectionString");
                            if (!String.IsNullOrEmpty(connectionString))
                            {
                                var item2 = dictionary2.FindItem(item1.Name);
                                if (item2 != null)
                                {
                                    item2.SetProp("ConnectionString", connectionString);
                                }
                            }
                        }
                    }

                // save prepared xml
                using (MemoryStream secondXmlStream = new MemoryStream())
                {
                    xml2.Save(secondXmlStream);
                    secondXmlStream.Position = 0;
                    byte[] buff = new byte[secondXmlStream.Length];
                    secondXmlStream.Read(buff, 0, buff.Length);
                    xmlString = Encoding.UTF8.GetString(buff);
                }
                xml1.Dispose();
                xml2.Dispose();
            }
            return xmlString;
        }

        //void SendPreviewObjectResponse(HttpContext context)
        //{
        //    string uuid = context.Request.Params["previewobject"];
        //    SetUpWebReport(uuid, context);
        //    WebUtils.SetupResponse(webReport, context);

        //    if (!NeedExport(context) && !NeedPrint(context))
        //        SendReport(context);

        //    cache.PutObject(uuid, webReport);
        //    Finalize(context);
        //}

        // On-line Designer
        //void SendDesigner(HttpContext context, string uuid)
        //{
        //    WebUtils.SetupResponse(webReport, context);
        //    StringBuilder sb = new StringBuilder();
        //    context.Response.AddHeader("Content-Type", "html/text");
        //    try
        //    {
        //        string designerPath = WebUtils.GetAppRoot(context, webReport.DesignerPath);
        //        string designerLocale = String.IsNullOrEmpty(webReport.DesignerLocale) ? "" : "&lang=" + webReport.DesignerLocale;
        //        sb.Append(String.Format("<iframe src=\"{0}?uuid={1}{2}{3}\" style=\"border:none;\" width=\"{4}\" height=\"{5}\" >",
        //            designerPath, //0
        //            uuid, //1
        //            WebUtils.GetARRAffinity(), //2
        //            designerLocale, //3
        //            webReport.Width.ToString(), //4 
        //            webReport.Height.ToString() //5
        //            ));
        //        sb.Append("<p style=\"color:red\">ERROR: Browser does not support IFRAME!</p>");
        //        sb.AppendLine("</iframe>");

        //        // add resize here
        //        if (webReport.Height == System.Web.UI.WebControls.Unit.Percentage(100))
        //            sb.Append(GetFitScript(uuid));
        //    }
        //    catch (Exception e)
        //    {
        //        log.AddError(e);
        //    }

        //    if (log.Text.Length > 0)
        //    {
        //        context.Response.Write(log.Text);
        //        log.Clear();
        //    }

        //    SetContainer(context, Properties.ControlID);
        //    context.Response.Write(sb.ToString());
        //}

        internal string GetPOSTReport(HttpContext context)
        {
            string requestString = "";
            using (TextReader textReader = new StreamReader(context.Request.Body))
                requestString = textReader.ReadToEndAsync().Result;

            string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            StringBuilder result = new StringBuilder(xmlHeader.Length + requestString.Length + 100);
            result.Append(xmlHeader);
            result.Append(requestString.
                    Replace("&gt;", ">").
                    Replace("&lt;", "<").
                    Replace("&quot;", "\"").
                    Replace("&amp;#10;", "&#10;").
                    Replace("&amp;#13;", "&#13;").
                    Replace("&amp;quot;", "&quot;").
                    Replace("&amp;amp;", "&").
                    Replace("&amp;lt;", "&lt;").
                    Replace("&amp;gt;", "&gt;")).
                    Replace("&amp;#xD;", "&#xD;").
                    Replace("&amp;#xA;", "&#xA;");
            return result.ToString();
        }

        string GetFitScript(string ID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("(function() {");
            sb.AppendLine(String.Format("var div = document.querySelector('#{0}'),", ID));
            sb.AppendLine("iframe,");
            sb.AppendLine("rect,");
            sb.AppendLine("e = document.documentElement,");
            sb.AppendLine("g = document.getElementsByTagName('body')[0],");
            //sb.AppendLine("x = window.innerWidth || e.clientWidth || g.clientWidth,");
            sb.AppendLine("y = window.innerHeight|| e.clientHeight|| g.clientHeight;");
            sb.AppendLine("if (div) {");
            sb.AppendLine("iframe = div.querySelector('iframe');");
            sb.AppendLine("if (iframe) {");
            sb.AppendLine("rect = iframe.getBoundingClientRect();");
            //sb.AppendLine("iframe.setAttribute('width', x - rect.left);");
            sb.AppendLine("iframe.setAttribute('height', y - rect.top - 11);");
            sb.AppendLine("}}}());");
            sb.AppendLine("</script>");
            return sb.ToString();
        }

        #endregion
    }
}
