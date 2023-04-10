using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web.Services
{
    /// <summary>
    /// The interface that provides utilities for the Online Designer. Allows to get MsChart Template, component properties and report functions
    /// </summary>
    /// <remarks>
    /// The interface may change over time
    /// </remarks>
    public interface IDesignerUtilsService
    {
        /// <summary>
        /// Searches for a template by name and returns an XML string with that template
        /// </summary>
        /// <param name="templateName">Name of the template. Like Blue, Black, etc.</param>
        /// <returns>Returns the XML string with the MsChart template. If the template is not found, it returns null</returns>
        string GetMSChartTemplateXML(string templateName);

        /// <summary>
        /// Returns JSON string of connection string properties.
        /// </summary>
        /// <param name="componentName">The FastReport component whose properties you want to get. Like TextObject, PictureObject etc.</param>
        /// <returns>Returns JSON string with FastReport component properties. If the component is not found - returns null</returns>
        string GetPropertiesJSON(string componentName);

        /// <summary>
        /// Retrieves the report functions and returns them to string
        /// </summary>
        /// <param name="report">Report, the functions of which are to be obtained</param>
        /// <returns>Returns a string with the report functions</returns>
        string GetFunctions(Report report);

        string DesignerObjectPreview(WebReport webReport, string reportObj);
    }
}
