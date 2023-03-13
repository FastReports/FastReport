#if !WASM
using FastReport.Web.Cache;

using Microsoft.AspNetCore.Hosting;
#endif
using System;

namespace FastReport.Web
{
    class FastReportGlobal
    {
#if WASM
        // ..
#elif BLAZOR
        internal static IWebHostEnvironment HostingEnvironment = null;
#else
        internal static IHostingEnvironment HostingEnvironment = null;
#endif
        internal static FastReportOptions FastReportOptions = new FastReportOptions();

#if !WASM
        internal static CacheOptions InternalCacheOptions { get; set; }
#endif

    }
}
