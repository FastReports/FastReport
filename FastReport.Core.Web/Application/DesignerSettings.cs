using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web
{
    public class DesignerSettings
    {
        public static DesignerSettings Default => new DesignerSettings();

        /// <summary>
        /// Gets or sets the locale of Designer
        /// </summary>
        public string Locale { get; set; } = "";

        /// <summary>
        /// Enable code editor in the Report Designer
        /// </summary>
        public bool ScriptCode { get; set; } = false;

        /// <summary>
        /// Gets or sets the json of configuration of Online Designer
        /// </summary>
        public string Config { get; set; } = "";

        /// <summary>
        /// Gets or sets path to the Report Designer
        /// </summary>
        public string Path { get; set; } = "/WebReportDesigner/index.html";

        /// <summary>
        /// Callback method for saving an edited report by Online Designer
        /// Params: reportID, report file name, report, out - message
        /// </summary>
        /// <example>
        /// <code>
        /// webReport.Designer.SaveMethod = (string reportID, string filename, string report) =>
        /// {
        ///     string webRootPath = _hostingEnvironment.WebRootPath;
        ///     string pathToSave = Path.Combine(webRootPath, filename);
        ///     System.IO.File.WriteAllText(pathToSave, report);
        ///     
        ///     return "OK";
        /// };
        /// </code>
        /// </example>
        public Func<string, string, string, string> SaveMethod { get; set; }

        /// <summary>
        /// Async callback method for saving an edited report by Online Designer
        /// <example>
        /// <code>
        ///webReport.Designer.SaveMethodAsync = async (reportID, filename, report, token) =>
        ///{
        ///    string webRootPath = _hostingEnvironment.WebRootPath;
        ///
        ///    string pathToSave = Path.Combine(webRootPath, "DesignedReports", "AsyncReports", filename);
        ///    if (!Directory.Exists(pathToSave))
        ///        Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));
        ///        
        ///    await System.IO.File.WriteAllTextAsync(pathToSave, report, token);
        ///    return "OK";
        ///};
        /// </code>
        /// </example>
        /// </summary>
        public Func<string, string, string, CancellationToken , Task<string>> SaveMethodAsync { get; set; }

        /// <summary>
        /// Gets or sets path to a folder for save designed reports
        /// If value is empty then designer posts saved report in variable ReportFile on call the DesignerSaveCallBack // TODO
        /// </summary>
        public string SavePath { get; set; } = "";

        /// <summary>
        /// Gets or sets path to callback page after Save from Designer
        /// </summary>
        [Obsolete("Designer.SaveCallBack is obsolete, please use Designer.SaveMethod instead.")]
        public string SaveCallBack { get; set; } = "";
    }
}
