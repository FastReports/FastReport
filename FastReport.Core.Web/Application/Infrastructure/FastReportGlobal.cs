#if !WASM
using FastReport.Web.Cache;

using Microsoft.AspNetCore.Hosting;
using System;

namespace FastReport.Web.Infrastructure
{
    internal static class FastReportGlobal
    {
#if BLAZOR
        internal static IWebHostEnvironment HostingEnvironment = null;
#else
        internal static IHostingEnvironment HostingEnvironment = null;
#endif
        internal static FastReportOptions FastReportOptions = new FastReportOptions();

    }
}
#endif
