#if !WASM
using FastReport.Web.Cache;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace FastReport.Web
{
    public class FastReportOptions
    {
        private const string CACHEOPTIONS_OBSOLETE_MESSAGE = "Please, use services.AddFastReport(options => options.CacheOptions)";

        /// <summary>
        /// Request.PathBase url for API.
        /// Default value: "".
        /// </summary>
        internal string RoutePathBaseRoot { get; set; } = "";

        internal bool CheckAuthorization(HttpRequest request)
        {
            if (Authorize != null)
                return Authorize.Invoke(request);

            return true;
        }

        /// <summary>
        /// Request.Path part of url for API.
        /// Default value: "/_fr".
        /// </summary>
        public string RouteBasePath { get; set; } = "/_fr";

        /// <summary>
        /// A function that determines who can access API.
        /// It should return true when the request client has access, false for a 401 to be returned.
        /// HttpRequest parameter is the current request.
        /// </summary>
        public Func<HttpRequest, bool> Authorize { get; set; } = null;

        [Obsolete(CACHEOPTIONS_OBSOLETE_MESSAGE, true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public CacheOptions CacheOptions { get; set; } = new CacheOptions();

        /// <summary>
        /// Reports' lifetime in minutes.
        /// If report is not used the specified time and there is no references to the object, it will be deleted from cache.
        /// Default value: "15".
        /// </summary>
        [Obsolete(CACHEOPTIONS_OBSOLETE_MESSAGE, true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int CacheDuration { get => CacheOptions.CacheDuration.Minutes; set => CacheOptions.CacheDuration = TimeSpan.FromMinutes(value); }


        /// <summary>
        /// Enable or disable the multiple instances environment.
        /// Default value: "false".
        /// </summary>
        //public bool CloudEnvironment { get; set; } = false;
    }
}
#endif
