#if DESIGNER
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Services
{
    /// <summary>
    /// Interface for interacting with reports. Allows you to design and save WebReport
    /// </summary>
    /// <remarks>
    /// The interface may change over time
    /// </remarks>
    public interface IReportDesignerService
    {
        /// <summary>
        /// Asynchronously returns a report preview string for Online Designer
        /// </summary>
        /// <param name="webReport">WebReport preview of which you want to return</param>
        /// <param name="receivedReportString">Report string from Online Designer</param>
        /// <returns></returns>
        Task<string> DesignerMakePreviewAsync(WebReport webReport, string receivedReportString);

        /// <summary>
        /// A method for saving the report
        /// </summary>
        /// <param name="webReport">Report to be saved</param>
        /// <param name="params">Parameters for saving the report</param>
        /// <returns>Returns the object with the response</returns>
        Task<SaveReportResponseModel> SaveReportAsync(WebReport webReport, SaveReportServiceParams @params, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the report for Online Designer
        /// </summary>
        /// <param name="webReport">WebReport to be displayed in the designer</param>
        /// <returns>Returns the report string for Online Designer</returns>
        string GetDesignerReport(WebReport webReport);

        /// <summary>
        /// Asynchronously returns a string with the report
        /// </summary>
        /// <param name="requestBody">Request body with report</param>
        /// <returns>String with the report</returns>
        Task<string> GetPOSTReportAsync(Stream requestBody, CancellationToken cancellationToken = default);
    }
}
#endif