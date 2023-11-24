using System;
using System.Collections.Generic;

namespace FastReport.Web.Services
{
    /// <summary>
    /// Interface for using the export service. Allows you to export a report and get the export settings window.
    /// </summary>
    /// <remarks>
    /// The interface may change over time
    /// </remarks>
    public interface IExportsService
    {
        /// <summary>
        /// Exports the report in a specified format with specified parameters and returns an array of bytes - the exported report file
        /// </summary>
        /// <param name="webReport">The FastReport component whose properties you want to get. Like TextObject, PictureObject etc.</param>
        /// <returns>Returns an array of bytes - the exported report. Returns null if there is an error.</returns>
        byte[] ExportReport(WebReport webReport, KeyValuePair<string, string>[] exportParams, string exportFormat, out string filename);

        /// <summary>
        /// Searches the settings window by format name and returns an HTML string with a modal settings container. 
        /// </summary>
        /// <param name="webReport">Report, for which the export settings are using</param>
        /// <param name="format">Export format. Like "pdf", "html", "image", etc.</param>
        /// <returns>Returns an HTML string with the export settings container of the selected format. If the format is not found, it returns an empty string.</returns>
        string GetExportSettings(WebReport webReport, string format);

#if !OPENSOURCE
        /// <summary>
        /// Creates an email export, assigns parameters to it and sends the email
        /// </summary>
        /// <param name="webReport">WebReport to be sent</param>
        /// <param name="emailExportParameters">Email export settings</param>
        void ExportEmail(WebReport webReport, EmailExportParams emailExportParameters);
#endif
    }
}
