using FastReport.Code;
using FastReport.Web;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class FastReportServiceCollectionExtensions
    {

        private static void ConfigureApplicationPart(IServiceCollection services, WebReportOptions options)
        {
            var manager = GetServiceFromCollection<ApplicationPartManager>(services);
            if (manager != null)
            {
                var curAssembly = typeof(FastReportServiceCollectionExtensions).Assembly;
                var curAssemblyName = curAssembly.GetName().Name;

                var frAppPart = manager.ApplicationParts.FirstOrDefault(apPart => apPart.Name == curAssemblyName);
                if (frAppPart == null)
                {
                    // Register this assembly as part of user application
                    var assemblyPart = new AssemblyPart(curAssembly);
                    manager.ApplicationParts.Add(assemblyPart);
                }


#if DEBUG       // TODO: fix 
                // Then we can use new controllers
                FastReportOptions.UseNewControllers = true;
#endif

                AddFastReportWebServices(services, options);
            }

            FastReportGlobal.InternalCacheOptions = options.CacheOptions;
        }

        private static void AddFastReportWebServices(IServiceCollection services, WebReportOptions options)
        {
            services.TryAddSingleton<IResourceLoader, InternalResourceLoader>();
            services.TryAddSingleton<IDesignerUtilsService, DesignerUtilsService>();
            services.TryAddSingleton<IReportService, ReportService>();
#if DESIGNER
            services.TryAddSingleton<IReportDesignerService, ReportDesignerService>();
#endif            
            services.TryAddSingleton<IConnectionsService, ConnectionService>();
            services.TryAddSingleton<ITextEditService, TextEditService>();
            services.TryAddSingleton<IExportsService, ExportService>();
            services.TryAddSingleton<IPrintService, PrintService>();
        }

        // possible memory leaks, be careful!
        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)(GetServiceDescriptorFromCollection<T>(services)?.ImplementationInstance);
        }
    }
}
