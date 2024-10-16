using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Services
{
    /// <summary>
    /// Interface for interacting with reports. Allows you to preview and search WebReport
    /// </summary>
    /// <remarks>
    /// The interface may change over time
    /// </remarks>
    public interface IReportService
    {
        /// <summary>
        /// Asynchronously returns a report for Preview on the Web
        /// </summary>
        /// <param name="webReport">Report a preview of which you want to create</param>
        /// <param name="params">Report preview creation options</param>
        /// <returns>Returns the HTML string of the report preview</returns>
        Task<string> GetReportAsync(WebReport webReport, GetReportServiceParams @params, CancellationToken cancellationToken = default);

        /// <summary>
        /// Finds the report by ID
        /// </summary>
        /// <param name="reportId">The ID of the report you want to find</param>
        /// <param name="webReport">Found report</param>
        /// <returns>WebReport</returns>
        bool TryFindWebReport(string reportId, out WebReport webReport);

        /// <summary>
        /// Touch report
        /// </summary>
        /// <param name="reportId">Report ID</param>
        void Touch(string reportId);

        /// <summary>
        /// Returns the report after clicking on an element with id = elementId
        /// </summary>
        /// <param name="webReport">Report to which the action applies</param>
        /// <param name="elementId">ID of the clicked item</param>
        /// <returns>Updated WebReport</returns>
        Task<string> InvokeCustomElementAction(WebReport webReport, string elementId, string inputValue);
    }
}
