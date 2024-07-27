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

        /// <summary>
        /// Returns the Online Designer configuration
        /// </summary>
        /// <param name="webReport">WebReport configuration of which is to be returned</param>
        /// <returns>String in JSON format with configuration for Online Designer</returns>
        string GetConfig(WebReport webReport);

        /// <summary>
        /// Retrieves a designer object preview.
        /// </summary>
        /// <param name="webReport">The WebReport object.</param>
        /// <param name="reportObj">The report object.</param>
        /// <returns>Returns a string representing the designer object preview.</returns>
        string DesignerObjectPreview(WebReport webReport, string reportObj);

        /// <summary>
        /// Gets class details in JSON format.
        /// </summary>
        /// <param name="className">The name of the class.</param>
        /// <returns>Returns class details in JSON format.</returns>
        string GetClassDetailsJson(string className);

        /// <summary>
        /// Gets namespaces information in JSON format.
        /// </summary>
        /// <param name="namespaces">The collection of namespaces.</param>
        /// <returns>Returns namespaces information in JSON format.</returns>
        string GetNamespacesInfoJson(IReadOnlyCollection<string> namespaces);
    }
}
