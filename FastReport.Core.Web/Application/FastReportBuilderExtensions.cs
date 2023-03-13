#if !WASM
using System;
using FastReport.Web;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;

namespace Microsoft.AspNetCore.Builder
{
    public static class FastReportBuilderExtensions
    {
        public static IApplicationBuilder UseFastReport(this IApplicationBuilder app, Action<FastReportOptions> setupAction = null)
        {
            var options = new FastReportOptions();
            setupAction?.Invoke(options);

            FastReportGlobal.FastReportOptions = options;
            if (FastReportGlobal.InternalCacheOptions != null && FastReportGlobal.InternalCacheOptions.IsChangedByUser)
            {
                FastReportGlobal.FastReportOptions.CacheOptions = FastReportGlobal.InternalCacheOptions;
            }

            if (FastReportOptions.UseNewControllers)
            {
                Debug.WriteLine("Application is using new controllers");
                return app;     // Do nothing
            }
            // fallback


            FastReport.Utils.Config.WebMode = true;

            // TODO: find better way to share global objects
#if BLAZOR
            FastReportGlobal.HostingEnvironment = (IWebHostEnvironment)app.ApplicationServices.GetService(typeof(IWebHostEnvironment));
#else
            FastReportGlobal.HostingEnvironment = (IHostingEnvironment)app.ApplicationServices.GetService(typeof(IHostingEnvironment));
#endif


            return app.UseMiddleware<FastReportMiddleware>();
        }
    }
}
#endif