#if !WASM
using FastReport.Web.Cache;
using Microsoft.AspNetCore.Http;
#endif
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FastReport.Web
{
    public class FastReportOptions
    {
        /// <summary>
        /// Request.PathBase url for API.
        /// Default value: "".
        /// </summary>
        internal string RoutePathBaseRoot { get; set; } = "";

        internal static bool UseNewControllers { get; set; } = false;

        /// <summary>
        /// Request.Path part of url for API.
        /// Default value: "/_fr".
        /// </summary>
        public string RouteBasePath { get; set; } = "/_fr";

#if !WASM
        /// <summary>
        /// A function that determines who can access API.
        /// It should return true when the request client has access, false for a 401 to be returned.
        /// HttpRequest parameter is the current request.
        /// </summary>
        public Func<HttpRequest, bool> Authorize { get; set; } = null;

        public CacheOptions CacheOptions { get; set; } = new CacheOptions();

        /// <summary>
        /// Reports' lifetime in minutes.
        /// If report is not used the specified time and there is no references to the object, it will be deleted from cache.
        /// Default value: "15".
        /// </summary>
        [Obsolete("Please, use CacheOptions.CacheDuration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int CacheDuration { get => CacheOptions.CacheDuration.Minutes; set => CacheOptions.CacheDuration = TimeSpan.FromMinutes(value); }

#endif

        /// <summary>
        /// Enable or disable the multiple instances environment.
        /// Default value: "false".
        /// </summary>
        //public bool CloudEnvironment { get; set; } = false;
    }
}
