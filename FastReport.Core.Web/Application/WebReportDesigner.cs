#if DESIGNER
using Microsoft.AspNetCore.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FastReport.Web.Services;

namespace FastReport.Web
{
    partial class WebReport
    {
        #region Designer Properties

        /// <summary>
        /// Designer settings
        /// </summary>
        public DesignerSettings Designer { get; set; } = DesignerSettings.Default; 

        /// <summary>
        /// Enable code editor in the Report Designer
        /// </summary>
        [Obsolete("DesignerScriptCode is obsolete, please use Designer.ScriptCode")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool DesignScriptCode { get => Designer.ScriptCode; set => Designer.ScriptCode = value; }

        /// <summary>
        /// Gets or sets path to the Report Designer
        /// </summary>
        [Obsolete("DesignerPath is obsolete, please use Designer.Path")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DesignerPath { get => Designer.Path; set => Designer.Path = value; }
        
        /// <summary>
        /// Gets or sets path to a folder for save designed reports
        /// If value is empty then designer posts saved report in variable ReportFile on call the DesignerSaveCallBack // TODO
        /// </summary>
        [Obsolete("DesignerPath is obsolete, please use Designer.SavePath")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DesignerSavePath { get => Designer.SavePath; set => Designer.SavePath = value; }

        /// <summary>
        /// Gets or sets path to callback page after Save from Designer
        /// </summary>
        [Obsolete("DesignerSaveCallBack is obsolete, please use Designer.SaveCallBack instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DesignerSaveCallBack { get => Designer.SaveCallBack; set => Designer.SaveCallBack = value; } 

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
        [Obsolete("DesignerSaveMethod is obsolete, please use Designer.SaveMethod instead.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Func<string, string, string, string> DesignerSaveMethod { get => Designer.SaveMethod; set => Designer.SaveMethod = value; }

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
        [Obsolete("DesignerLocale is obsolete, please use Designer.Locale")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DesignerLocale { get => Designer.Locale; set => Designer.Locale = value; }

        /// <summary>
        /// Gets or sets the text of configuration of Online Designer
        /// </summary>
        [Obsolete("DesignerConfig is obsolete, please use Designer.Config")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string DesignerConfig { get => Designer.Config; set => Designer.Config = value; } 

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
        internal void OnSaveDesignedReport()
        {
            if (SaveDesignedReport != null)
            {
                SaveDesignedReportEventArgs e = new SaveDesignedReportEventArgs();
                e.Stream = new MemoryStream();
                Report.Save(e.Stream);
                e.Stream.Position = 0;
                SaveDesignedReport.Invoke(this, e);
            }
            
        }

        #endregion

        #region Private Methods
        HtmlString RenderDesigner()
        {
            //string designerPath = WebUtils.GetAppRoot(DesignerPath);
            string designerLocale = Designer.Locale.IsNullOrWhiteSpace() ? "" : $"&lang={Designer.Locale}";
            return new HtmlString($@"
<iframe src=""{Designer.Path}?uuid={ID}{ReportDesignerService.GetARRAffinity()}{designerLocale}"" style=""border:none;"" width=""{Width}"" height=""{Height}"">
    <p style=""color:red"">ERROR: Browser does not support IFRAME!</p>
</iframe>
");
            // TODO: add fit script
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
        //        string designerLocale = String.IsNullOrEmpty(webReport.Designer.Locale) ? "" : "&lang=" + webReport.Designer.Locale;
        //        sb.AppendFormat("<iframe src=\"{0}?uuid={1}{2}{3}\" style=\"border:none;\" width=\"{4}\" height=\"{5}\" >",
        //            designerPath, //0
        //            uuid, //1
        //            WebUtils.GetARRAffinity(), //2
        //            designerLocale, //3
        //            webReport.Width.ToString(), //4 
        //            webReport.Height.ToString() //5
        //            );
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

        string GetFitScript(string ID)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<script>");
            sb.AppendLine("(function() {");
            sb.AppendLine($"var div = document.querySelector('#{ID}'),");
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
#endif