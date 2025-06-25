#if DESIGNER
using FastReport.Utils;
using FastReport.Web.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Services
{
    internal sealed class ReportDesignerService : IReportDesignerService
    {
        private readonly DesignerOptions _designerOptions;
        private readonly IWebReportCache _webReportCache;
        private readonly IServiceProvider _serviceProvider;

        public ReportDesignerService(DesignerOptions designerOptions, IWebReportCache webReportCache, IServiceProvider serviceProvider)
        {
            _designerOptions = designerOptions;
            _webReportCache = webReportCache;
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetPOSTReportStringAsync(Stream requestBody, CancellationToken cancellationToken = default)
        {
            string requestString;
            using (StreamReader textReader = new StreamReader(requestBody))
                 requestString = await textReader.ReadToEndAsync();

            const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
            StringBuilder result = new StringBuilder(xmlHeader.Length + requestString.Length);
            result.Append(xmlHeader);
            result.Append(requestString).
                Replace("&gt;", ">").
                Replace("&lt;", "<").
                Replace("&quot;", "\"").
                Replace("&amp;#10;", "&#10;").
                Replace("&amp;#13;", "&#13;").
                Replace("&amp;quot;", "&quot;").
                Replace("&amp;amp;", "&").
                Replace("&amp;lt;", "&lt;").
                Replace("&amp;gt;", "&gt;").
                Replace("&amp;#xD;", "&#xD;").
                Replace("&amp;#xA;", "&#xA;");

            return result.ToString();
        }

        private async Task<MemoryStream> GetPOSTReportAsync(Stream requestBody, CancellationToken cancellationToken)
        {
            var reportString = await GetPOSTReportStringAsync(requestBody, cancellationToken);
            var bytes = Encoding.UTF8.GetBytes(reportString);
            return new MemoryStream(bytes, 0, bytes.Length, false, true);
        }


        public async Task<SaveReportResponseModel> SaveReportAsync(WebReport webReport, SaveReportServiceParams @params, CancellationToken cancellationToken = default)
        {
            if (webReport.Designer.SaveMethod == null && webReport.Designer.SaveMethodAsync == null)
            {
                // old saving way by self-request
                return await DesignerSaveReport(webReport, @params, cancellationToken);
            }
            else
            {
                // save by using a Func

                var reportStream = await GetPOSTReportAsync(@params.RequestBody, cancellationToken);
                var result = new SaveReportResponseModel();
                result.Code = 200;
                try
                {
                    // paste restricted back in report before save
                    var restrictedReport = PasteRestricted(webReport, reportStream);
                    webReport.Report.Load(restrictedReport);
                    var report = webReport.Report.SaveToString();

                    if (webReport.Designer.SaveMethodAsync != null)
                    {
                        result.Msg = await webReport.Designer.SaveMethodAsync(webReport.ID, webReport.ReportFileName, report, cancellationToken);
                    }

                    if (webReport.Designer.SaveMethod != null)
                    {
                        result.Msg = webReport.Designer.SaveMethod(webReport.ID, webReport.ReportFileName, report);
                    }
                }
                catch (Exception ex)
                {
                    result.Code = 500;
                    result.Msg = ex.Message;
                }
                return result;
            }
        }

        private async Task<SaveReportResponseModel> DesignerSaveReport(WebReport webReport, SaveReportServiceParams @params, CancellationToken cancellationToken)
        {
            var result = new SaveReportResponseModel();

            result.Code = 200;

            var reportStream = await GetPOSTReportAsync(@params.RequestBody, cancellationToken);

            try
            {
                // paste restricted back in report before save
                var reportStreamRestricted = PasteRestricted(webReport, reportStream);
                SafeLoadReport(webReport, reportStreamRestricted);

                webReport.OnSaveDesignedReport();

                if (!webReport.Designer.SaveCallBack.IsNullOrWhiteSpace())
                {
                    string reportFileName = webReport.ReportFileName;

                    UriBuilder uri = new UriBuilder
                    {
                        Scheme = @params.Scheme,
                        Host = @params.Host
                    };

                    //if (!FastReportGlobal.FastReportOptions.CloudEnvironmet)
                    if (@params.Port != null)
                        uri.Port = (int)@params.Port;
                    else if (uri.Scheme == "https")
                        uri.Port = 443;
                    else
                        uri.Port = 80;

                    // TODO:
                    //uri.Path = webReport.ResolveUrl(webReport.DesignerSaveCallBack);
                    uri.Path = webReport.Designer.SaveCallBack;
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
                    string queryToAppend = $"reportID={webReport.ID}&reportUUID={reportFileName}";

                    if (uri.Query != null && uri.Query.Length > 1)
                        uri.Query = uri.Query.Substring(1) + "&" + queryToAppend;
                    else
                        uri.Query = queryToAppend;

                    string callBackURL = uri.ToString();

                    // return "true" to force the certificate to be accepted.
                    ServicePointManager.ServerCertificateValidationCallback = (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) => true;
                    
                    // TODO: use HttpClient
                    HttpWebRequest request = WebRequest.CreateHttp(callBackURL);

                    if (request != null)
                    {
                        // set up the custom headers
                        if (webReport.RequestHeaders != null)
                            request.Headers = webReport.RequestHeaders;

                        CopyCookies(request, @params);

                        // TODO: why here??
                        // if save report in reports folder
                        if (!String.IsNullOrEmpty(webReport.Designer.SavePath))
                        {
                            string savePath = WebUtils.MapPath(webReport.Designer.SavePath); // TODO: do we really need this?
                            if (Directory.Exists(savePath))
                            {
                                using var fs = File.OpenWrite(Path.Combine(savePath, reportFileName));
                                webReport.Report.Save(fs);
                            }
                            else
                            {
                                result.Msg = "DesignerSavePath does not exist";
                                result.Code = 404;
                            }

                            request.Method = "GET";
                        }
                        else
                        // send report directly in POST
                        {
                            request.Method = "POST";
                            request.ContentType = "text/xml";
                            Stream reqStream = request.GetRequestStream();
                            webReport.Report.Save(reqStream);
                            // request.ContentLength = postData.Length;
                            reqStream.Close();
                        }

                        // Request call-back
                        try
                        {
                            using (HttpWebResponse resp = request.GetResponse() as HttpWebResponse)
                            {
                                //context.Response.StatusCode = (int)resp.StatusCode;
                                //context.Response.Write(resp.StatusDescription);

                                result.Code = (int)resp.StatusCode;
                                result.Msg = resp.StatusDescription;
                            }
                        }
                        catch (WebException err)
                        {
                            result.Code = 500;

                            if (webReport.Debug)
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

                                        result.Msg = text;
                                        result.Code = (int)(err.Response as HttpWebResponse).StatusCode;
                                    }
                                }
                            }
                            else
                            {
                                result.Msg = err.Message;
                            }
                        }

                    }
                    request = null;
                }
            }
            catch (Exception e)
            {
                result.Code = (int)HttpStatusCode.InternalServerError;
                if (webReport.Debug)
                    result.Msg = e.Message;
            }

            result.Msg = "";
            return result;
        }

        public async Task<string> DesignerMakePreviewAsync(WebReport webReport, Stream requestBody, CancellationToken cancellationToken)
        {
            var previewReport = new WebReport
            {
                Debug = webReport.Debug,
                Report = webReport.Report,
                LocalizationFile = webReport.LocalizationFile,
                Toolbar = webReport.Toolbar,
                Pictures = webReport.Pictures,
                EmbedPictures = webReport.EmbedPictures,
            };

            var receivedReport = await GetPOSTReportAsync(requestBody, cancellationToken);
            var reportStream = PasteRestricted(webReport, receivedReport);
            previewReport.Report.Load(reportStream);
            previewReport.Mode = WebReportMode.Preview;
            previewReport.Report.PreparedPages?.Clear();

            return (await previewReport.Render()).ToString();
        }

        public string GetDesignerReport(WebReport webReport)
        {
            var reportStream = SaveToStream(webReport.Report);
            var report = CutRestricted(webReport, reportStream);

            if (report.Contains("inherited"))
            {
                List<string> reportInheritance = new List<string>();
                string baseReport = report;
                while (!String.IsNullOrEmpty(baseReport))
                {
                    reportInheritance.Add(baseReport);
                    baseReport = GetBaseReport(baseReport, webReport);
                }

                StringBuilder responseBuilder = new StringBuilder();
                responseBuilder.Append("{\"reports\":[");
                for (int i = reportInheritance.Count - 1; i >= 0; i--)
                {
                    string s = reportInheritance[i];
                    responseBuilder.Append('\"');
                    responseBuilder.Append(s.Replace("\r\n", "").Replace("\"", "\\\""));
                    if (i > 0)
                        responseBuilder.Append("\",");
                    else
                        responseBuilder.Append('"');
                }
                responseBuilder.Append("]}");

                return responseBuilder.ToString();
            }

            return report;


            static string GetBaseReport(string reportXml, WebReport webReport)
            {
                const string BASE_REPORT_PROP_NAME = nameof(Report.BaseReport);
                var startSymbol = reportXml.IndexOf(BASE_REPORT_PROP_NAME);
                if (startSymbol == -1)
                    return string.Empty;

                var baseReportStartPosition = startSymbol + BASE_REPORT_PROP_NAME.Length + "=\"".Length;
                var lastSymbol = reportXml.IndexOf('"', baseReportStartPosition);
                string baseReportFilePath = reportXml.Substring(baseReportStartPosition, lastSymbol - baseReportStartPosition);
                if (!Path.IsPathRooted(baseReportFilePath))
                    baseReportFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(webReport.Report.FileName), baseReportFilePath)); //was ReportPath before(ToDo)

                if (File.Exists(baseReportFilePath))
                    return File.ReadAllText(baseReportFilePath, Encoding.UTF8);

                return String.Empty;
            }
        }

        private static MemoryStream SaveToStream(Report report)
        {
            string password = report.Password;
            var compressed = report.Compressed;
            try
            {
                report.Password = string.Empty;
                report.Compressed = false;
                var ms = new MemoryStream();
                report.Save(ms, false);
                return ms;
            }
            finally
            {
                report.Password = password;
                report.Compressed = compressed;
            }
        }

        private static void SafeLoadReport(WebReport webReport, MemoryStream updatedReportStream)
        {
            var report = webReport.Report;
            var compressed = report.Compressed;

            report.Load(updatedReportStream);

            // restore compressed flag after loading non-compressed version
            webReport.Report.Compressed = compressed;
        }

        static string CutRestricted(WebReport webReport, MemoryStream reportStream)
        {
            reportStream.Position = 0;

            using (var xml = new XmlDocument())
            {
                xml.Load(reportStream);

                if (!webReport.Designer.ScriptCode)
                {
                    xml.Root.SetProp("CodeRestricted", "true");
                    // cut script
                    var scriptItem = xml.Root.FindItem(nameof(Report.ScriptText));
                    if (scriptItem != null && !String.IsNullOrEmpty(scriptItem.Value))
                        scriptItem.Value = String.Empty;
                }

                // cut connection strings
                var dictionary = xml.Root.FindItem(nameof(Report.Dictionary));
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

                // delete password (for loading passworded webreport in web-designer)
                var password = xml.Root.FindItem(nameof(Report.Password));
                {
                    if (password != null)
                    {

                        if (!String.IsNullOrEmpty(xml.Root.GetProp("Password")))
                        {
                            xml.Root.RemoveProp("Password");
                        }

                        xml.Root.Items.Remove(xml.Root.FindItem("Password"));
                    }
                }

                return XmlDocumentToString(xml);
            }
        }

        private static string XmlDocumentToString(XmlDocument xml)
        {
            // save prepared xml
            using (var stringWriter = new StringWriter(new StringBuilder(1024)))
            {
                xml.Save(stringWriter);
                return stringWriter.ToString();
            }
        }

        static MemoryStream PasteRestricted(WebReport webReport, MemoryStream xmlStream2)
        {
            xmlStream2.Position = 0;
            using var xml1 = new XmlItem();
            using var xml2 = new XmlDocument();
            using var frWriter = new FRWriter(xml1);
            frWriter.SaveExternalPages = false;
            frWriter.Write(webReport.Report);

            xml2.Load(xmlStream2);

            if (!webReport.Designer.ScriptCode)
            {
                xml2.Root.SetProp("CodeRestricted", "");
                // paste old script
                var scriptItem1 = xml1.FindItem(nameof(Report.ScriptText));
                var scriptText = webReport.Report.ScriptText;
                if (scriptItem1 != null && String.IsNullOrEmpty(scriptItem1.Value))
                {
                    var scriptItem2 = xml2.Root.FindItem(nameof(Report.ScriptText));
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
            var dictionary1 = xml1.FindItem(nameof(Report.Dictionary));
            var dictionary2 = xml2.Root.FindItem(nameof(Report.Dictionary));
            if (dictionary1 != null && dictionary2 != null)
            {
                for (int i = 0; i < dictionary1.Items.Count; i++)
                {
                    var item1 = dictionary1.Items[i];
                    string connectionString = item1.GetProp("ConnectionString");
                    if (!String.IsNullOrEmpty(connectionString))
                    {
                        var item2 = dictionary2.FindItem(item1.Name);
                        var newConnectionString = item2.GetProp("ConnectionString");

                        if (item2 != null && newConnectionString.IsNullOrEmpty())
                        {
                            item2.SetProp("ConnectionString", connectionString);
                        }
                    }
                }
            }

            // save prepared xml
            MemoryStream secondXmlStream = new MemoryStream();
            xml2.Save(secondXmlStream);
            secondXmlStream.Position = 0;
            return secondXmlStream;
        }

        public async Task<string> CreateReport(Stream requestBody, CancellationToken cancellationToken = default)
        {
            var webReport = new WebReport()
            {
                Mode = WebReportMode.Designer,
            };
            var reportStream = await GetPOSTReportAsync(requestBody, cancellationToken);
            webReport.Report.Load(reportStream);

            if (_designerOptions.OnWebReportCreatedAsync != null)
            {
                using var scope = _serviceProvider.CreateScope();
                await _designerOptions.OnWebReportCreatedAsync(webReport, scope.ServiceProvider, cancellationToken);
            }
            else if (_designerOptions.OnWebReportCreated != null)
            {
                using var scope = _serviceProvider.CreateScope();
                _designerOptions.OnWebReportCreated.Invoke(webReport, scope.ServiceProvider);
            }
            _webReportCache.Add(webReport);
            return webReport.ID;
        }

        private static void CopyCookies(HttpWebRequest request, SaveReportServiceParams @params)
        {
            CookieContainer cookieContainer = new CookieContainer();
            UriBuilder uri = new UriBuilder
            {
                Scheme = @params.Scheme,
                Host = @params.Host
            };

            string ARRAffinity = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(ARRAffinity))
                cookieContainer.Add(uri.Uri, new Cookie("ARRAffinity", ARRAffinity));

            foreach (string key in @params.Cookies.Keys)
                cookieContainer.Add(uri.Uri, new Cookie(key, WebUtility.UrlEncode(@params.Cookies[key])));

            request.CookieContainer = cookieContainer;
        }

        internal static string GetARRAffinity()
        {
            string id = GetWebsiteInstanceId();
            if (!String.IsNullOrEmpty(id))
                return String.Concat("&ARRAffinity=", id);
            else
                return String.Empty;
        }

        private static string GetWebsiteInstanceId()
        {
            return Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID");
        }
    }
}
#endif