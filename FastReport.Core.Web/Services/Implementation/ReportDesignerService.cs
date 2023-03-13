#if DESIGNER
using FastReport.Utils;
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
        [Obsolete]
        internal static ReportDesignerService Instance { get; } = new ReportDesignerService();

        public async Task<string> GetPOSTReportAsync(Stream requestBody, CancellationToken cancellationToken = default)
        {
            string requestString = "";
            using (TextReader textReader = new StreamReader(requestBody))
                 requestString = await textReader.ReadToEndAsync();

            const string xmlHeader = "<?xml version=\"1.0\" encoding=\"utf-8\"?>";
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

        public async Task<SaveReportResponseModel> SaveReportAsync(WebReport webReport, SaveReportServiceParams @params, CancellationToken cancellationToken = default)
        {
            var result = new SaveReportResponseModel();

            if (webReport.Designer.SaveMethod == null && webReport.Designer.SaveMethodAsync == null)
            {
                // old saving way by self-request
                result = await DesignerSaveReport(webReport, @params);
            }
            else
            {
                // save by using a Func

                string report = await GetPOSTReportAsync(@params.RequestBody);
                report = FixLandscapeProperty(report);
                result.Msg = string.Empty;
                result.Code = 200;
                try
                {
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
            }

            return result;
        }

        private async Task<SaveReportResponseModel> DesignerSaveReport(WebReport webReport, SaveReportServiceParams @params)
        {
            var result = new SaveReportResponseModel();

            result.Code = 200;

            string reportString = await GetPOSTReportAsync(@params.RequestBody);

            try
            {
                // paste restricted back in report before save
                string restrictedReport = PasteRestricted(webReport, reportString);
                restrictedReport = FixLandscapeProperty(restrictedReport);
                webReport.Report.LoadFromString(restrictedReport);

                webReport.OnSaveDesignedReport();

                if (!webReport.Designer.SaveCallBack.IsNullOrWhiteSpace())
                {
                    string report = webReport.Report.SaveToString();
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
                                File.WriteAllText(Path.Combine(savePath, reportFileName), report, Encoding.UTF8);
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
            return null;
        }

        public async Task<string> DesignerMakePreviewAsync(WebReport webReport, string receivedReportString)
        {
            var previewReport = new WebReport();
            previewReport.Report = webReport.Report;
            //previewReport.Prop.Assign(webReport.Prop);
            //previewReport.CurrentTab = CurrentTab.Clone();
            previewReport.LocalizationFile = webReport.LocalizationFile;
            previewReport.Toolbar = webReport.Toolbar;
            //previewReport.Width = "880px";
            //previewReport.Height = "770px";
            //previewReport.Toolbar.EnableFit = true;
            //previewReport.Layers = true;
            string reportString = PasteRestricted(webReport, receivedReportString);
            reportString = FixLandscapeProperty(reportString);
            previewReport.Report.ReportResourceString = reportString; // TODO
            //previewReport.ReportFile = String.Empty;
            previewReport.ReportResourceString = reportString; // TODO
            previewReport.Mode = WebReportMode.Preview;
            previewReport.Report.PreparedPages?.Clear();

            return (await previewReport.Render()).ToString();
        }

        public string GetDesignerReport(WebReport webReport)
        {
            string reportString = webReport.Report.SaveToString();
            string report = CutRestricted(webReport, reportString);

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
                                baseReportFile = Path.GetFullPath(Path.GetDirectoryName(webReport.Report.FileName) + Path.DirectorySeparatorChar + baseReportFile); //was ReportPath before(ToDo)

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
                    responseBuilder.Append('\"');
                    responseBuilder.Append(s.Replace("\r\n", "").Replace("\"", "\\\""));
                    if (i > 0)
                        responseBuilder.Append("\",");
                    else
                        responseBuilder.Append('\"');
                }
                responseBuilder.Append("]}");

                return responseBuilder.ToString();
            }

            return report;
        }

        string CutRestricted(WebReport webReport, string xmlString)
        {
            using (MemoryStream xmlStream = new MemoryStream())
            {
                WebUtils.Write(xmlStream, xmlString);
                xmlStream.Position = 0;

                using (var xml = new XmlDocument())
                {
                    xml.Load(xmlStream);

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

                    // save prepared xml
                    using (MemoryStream secondXmlStream = new MemoryStream())
                    {
                        xml.Save(secondXmlStream);
                        secondXmlStream.Position = 0;
                        int secondXmlLength = (int)secondXmlStream.Length;
                        bool rent = secondXmlLength > 1024;
                        byte[] buff = rent ?
                            ArrayPool<byte>.Shared.Rent(secondXmlLength)
                            : new byte[secondXmlLength];
                        secondXmlStream.Read(buff, 0, secondXmlLength);
                        xmlString = Encoding.UTF8.GetString(buff, 0, secondXmlLength);
                        if (rent) ArrayPool<byte>.Shared.Return(buff);
                    }
                }
            }
            return xmlString;
        }

        string PasteRestricted(WebReport webReport, string xmlString)
        {
            using (MemoryStream xmlStream1 = new MemoryStream())
            using (MemoryStream xmlStream2 = new MemoryStream())
            {
                WebUtils.Write(xmlStream1, webReport.Report.SaveToString());
                WebUtils.Write(xmlStream2, xmlString);
                xmlStream1.Position = 0;
                xmlStream2.Position = 0;
                var xml1 = new XmlDocument();
                var xml2 = new XmlDocument();
                xml1.Load(xmlStream1);
                xml2.Load(xmlStream2);

                if (!webReport.Designer.ScriptCode)
                {
                    xml2.Root.SetProp("CodeRestricted", "");
                    // paste old script
                    var scriptItem1 = xml1.Root.FindItem(nameof(Report.ScriptText));
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
                var dictionary1 = xml1.Root.FindItem(nameof(Report.Dictionary));
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
                    int secondXmlLength = (int)secondXmlStream.Length;
                    bool rent = secondXmlLength > 1024;
                    byte[] buff = rent ?
                        ArrayPool<byte>.Shared.Rent(secondXmlLength)
                        : new byte[secondXmlLength];
                    secondXmlStream.Read(buff, 0, secondXmlLength);
                    xmlString = Encoding.UTF8.GetString(buff, 0, secondXmlLength);
                    if (rent) ArrayPool<byte>.Shared.Return(buff);
                }
                xml1.Dispose();
                xml2.Dispose();
            }
            return xmlString;
        }

        private static string FixLandscapeProperty(string reportString)
        {
            int indexOfLandscape = reportString.IndexOf(nameof(ReportPage.Landscape));
            if (indexOfLandscape != -1)
            {
                // Landscape="~"
                int lastIndexOfLandscapeValue =
                    reportString.IndexOf('"', indexOfLandscape + nameof(ReportPage.Landscape).Length + 2, 10);

                var indexOfPage = reportString.IndexOf(nameof(ReportPage), 0, indexOfLandscape);
                int startposition = indexOfPage + nameof(ReportPage).Length + 1;
                if (indexOfLandscape == startposition)
                    return reportString;

                StringBuilder sb = new StringBuilder(reportString);
                var property = reportString.Substring(indexOfLandscape, lastIndexOfLandscapeValue - indexOfLandscape + 2);

                sb.Remove(indexOfLandscape, property.Length);

                sb.Insert(startposition, property);
                reportString = sb.ToString();
            }
            return reportString;
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