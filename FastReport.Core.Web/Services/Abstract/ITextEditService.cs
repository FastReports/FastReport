using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web.Services
{
    /// <summary>
    /// The interface may change over time
    /// </summary>
    public interface ITextEditService
    {
        /// <summary>
        /// Returns a string with template text edit form
        /// </summary>
        /// <param name="click">Click options - page, coordinates</param>
        /// <param name="webReport">WebReport clicked on</param>
        /// <returns></returns>
        string GetTemplateTextEditForm(string click, WebReport webReport);
    }
}
