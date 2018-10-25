using System;
using FastReport.Web;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;

namespace Microsoft.AspNetCore.Builder
{
    public static class FastReportBuilderExtensions
    {
        public static IApplicationBuilder UseFastReport(this IApplicationBuilder app, Action<FastReportOptions> setupAction = null)
        {
            var options = new FastReportOptions();
            setupAction?.Invoke(options);

            FastReport.Utils.Config.WebMode = true;

            // TODO: find better way to share global objects
            FastReportGlobal.HostingEnvironment = (IHostingEnvironment)app.ApplicationServices.GetService(typeof(IHostingEnvironment));
            FastReportGlobal.FastReportOptions = options;

            return app.UseMiddleware<FastReportMiddleware>();
        }
    }
}
