#if !WASM
using System;
using System.Diagnostics;
using FastReport.Web;
using FastReport.Web.Cache;
using FastReport.Web.Services;
using FastReport.Web.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    public static class FastReportBuilderExtensions
    {
        public static IApplicationBuilder UseFastReport(this IApplicationBuilder app, Action<FastReportOptions> setupAction = null)
        {
            var options = SetupFastReport(setupAction, app.ApplicationServices);
            FastReportGlobal.FastReportOptions = options;

            ControllerBuilder.InitializeControllers();

            return app.UseMiddleware<FastReportMiddleware>();
        }

        private static FastReportOptions SetupFastReport(Action<FastReportOptions> setupAction, IServiceProvider serviceProvider)
        {
            FastReportServicesCheck(serviceProvider);

            var options = new FastReportOptions();
            setupAction?.Invoke(options);

            FastReport.Utils.Config.WebMode = true;
            
            // because WebReport..ctor adds WebReport instances to WebReportCache without DI
            WebReportCache.Instance = serviceProvider.GetService<IWebReportCache>();

            // TODO: find better way to share global objects
            FastReportGlobal.HostingEnvironment = serviceProvider.GetService<IWebHostEnvironment>();
            WebReport.ResourceLoader = serviceProvider.GetService<IResourceLoader>();

            return options;
        }

#if false
        public static WebApplication UseFastReport(this WebApplication app, Action<FastReportOptions> setupAction = null)
        {
            var options = SetupFastReport(setupAction, app.Services);

            return UseMinimalApiRouting(app, options);
        }

        private static WebApplication UseMinimalApiRouting(WebApplication app, FastReportOptions options)
        {
            var methods = ControllerBuilder.GetControllerMethods();
            foreach (var method in methods)
            {
                var @delegate = ControllerBuilder.BuildMinimalAPIDelegate(method);

                var httpMethod = method.GetCustomAttribute<HttpMethodAttribute>();

                if (httpMethod == null)
                    throw new Exception($"There isn't any 'HttpMethodAttribute' in this method {method.Name}");
                
                var path = WebUtils.ToUrl(options.RouteBasePath, httpMethod.Template);

                RouteHandlerBuilder builder;
                if (httpMethod is HttpGetAttribute)
                {
                    builder = app.MapGet(path, @delegate);
                }
                else if (httpMethod is HttpPostAttribute)
                {
                    builder = app.MapPost(path, @delegate);
                }
                else if (httpMethod is HttpPutAttribute)
                {
                    builder = app.MapPut(path, @delegate);
                }
                else if (httpMethod is HttpDeleteAttribute)
                {
                    builder = app.MapDelete(path, @delegate);
                }
                else
                    throw new NotSupportedException();

                builder.ExcludeFromDescription()            // exclude from Swagger (optional)
                        .AllowAnonymous();                  // disable user authorization for this endpoint
            }
            return app;
        }
#endif

        private static void FastReportServicesCheck(IServiceProvider serviceProvider)
        {
            const string HAVE_TO_REGISTER_SERVICES = "Please, register FastReport services in DI container. Use services.AddFastReport()";

            var _ = serviceProvider.GetService<IReportService>() ?? throw new Exception(HAVE_TO_REGISTER_SERVICES);
        }

    }
}
#endif
