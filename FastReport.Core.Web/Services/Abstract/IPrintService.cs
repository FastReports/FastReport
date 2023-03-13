using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web.Services
{
    /// <summary>
    /// Interface for printing the report.
    /// </summary>
    /// <remarks>
    /// The interface may change over time
    /// </remarks>
    public interface IPrintService
    {
        /// <summary>
        /// Prints report
        /// </summary>
        /// <param name="webReport">WebReport to be printed</param>
        /// <param name="printMode">The print mode is "pdf" or "html".</param>
        /// <returns>Returns an array of bytes depending on the print mode. File to print from browser if print mode is "html" and pdf file if print mode is "pdf"</returns>
        byte[] PrintReport(WebReport webReport, string printMode);
    }
}
