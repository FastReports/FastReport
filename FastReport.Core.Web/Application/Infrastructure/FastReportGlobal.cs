#if !WASM
using FastReport.Web.Cache;

using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;

namespace FastReport.Web.Infrastructure
{
    internal static class FastReportGlobal
    {
        internal static IWebHostEnvironment HostingEnvironment = null;
        internal static FastReportOptions FastReportOptions = new FastReportOptions();

#if !WASM
        internal static EmailExportOptions InternalEmailExportOptions = null;
#endif

    }
}
#endif
