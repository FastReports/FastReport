using Microsoft.AspNetCore.Hosting;
using System;

namespace FastReport.Web
{
    class FastReportGlobal
    {
        internal static IHostingEnvironment HostingEnvironment = null;
        internal static FastReportOptions FastReportOptions = new FastReportOptions();
    }
}
