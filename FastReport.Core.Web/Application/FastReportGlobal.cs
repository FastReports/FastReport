using Microsoft.AspNetCore.Hosting;
using System;

namespace FastReport.Web
{
    class FastReportGlobal
    {
#if BLAZOR
        internal static IWebHostEnvironment HostingEnvironment = null;
#else
        internal static IHostingEnvironment HostingEnvironment = null;
#endif
        internal static FastReportOptions FastReportOptions = new FastReportOptions();
    }
}
