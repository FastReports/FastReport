using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    public class FastReportOptions
    {
        /// <summary>
        /// Base url for API.
        /// Default value: "/_fr".
        /// </summary>
        public string RouteBasePath { get; set; } = "/_fr";

        /// <summary>
        /// A function that determines who can access API.
        /// It should return true when the request client has access, false for a 401 to be returned.
        /// HttpRequest parameter is the current request.
        /// </summary>
        public Func<HttpRequest, bool> Authorize { get; set; } = null;

        /// <summary>
        /// Reports' lifetime in minutes.
        /// If report is not used the specified time and there is no references to the object, it will be deleted from cache.
        /// Default value: "15".
        /// </summary>
        public int CacheDuration { get; set; } = 15;

        /// <summary>
        /// Enable or disable the multiple instances environment.
        /// Default value: "false".
        /// </summary>
        //public bool CloudEnvironmet { get; set; } = false;
    }
}
