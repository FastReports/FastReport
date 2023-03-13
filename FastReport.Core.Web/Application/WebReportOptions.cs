#if !WASM
using FastReport.Web.Cache;
#endif

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace FastReport.Web
{
    public sealed class WebReportOptions
    {

#if !WASM
        public CacheOptions CacheOptions { get; set; } = new CacheOptions();

#else
        /// <summary>
        /// Used to access .NET libraries to compile the report script
        /// </summary>
        /// <example>
        /// <code>
        /// options.HttpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        /// </code>
        /// </example>
        public HttpClient? HttpClient { get; set; }

#endif

    }
}
