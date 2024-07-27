using FastReport.Code;
using FastReport.Web;
using FastReport.Web.Cache;
using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class FastReportServiceCollectionExtensions
    {

        private static void AddServices(IServiceCollection services, WebReportOptions options)
        {
            AddFastReportWebServices(services, options);
        }

        private static void AddFastReportWebServices(IServiceCollection services, WebReportOptions options)
        {
            services.TryAddSingleton(options.CacheOptions);
            if (options.CacheOptions.UseLegacyWebReportCache)
            {
                services.TryAddSingleton<IWebReportCache, WebReportLegacyCache>();
            }
            else // MemoryCache
            {
                // check for registered IMemoryCache
                var memoryCacheDescriptor = GetServiceDescriptorFromCollection<IMemoryCache>(services);

                if(memoryCacheDescriptor != null)
                {
                    services.TryAddSingleton<IWebReportCache, WebReportMemoryCache>();
                }
                else
                {
                    services.TryAddSingleton<IWebReportCache>(
                        static serviceProvider => 
                        {
                            var memoryCache = serviceProvider.GetService<IMemoryCache>() 
                                ?? WebReportCache.GetDefaultMemoryCache();

                            var cacheOptions = serviceProvider.GetService<CacheOptions>();
                            return new WebReportMemoryCache(memoryCache, cacheOptions);
                        });
                }
            }

            FastReportGlobal.InternalEmailExportOptions = options.EmailExportOptions;
            FastReportGlobal.AllowCustomSqlQueries = options.AllowCustomSqlQueries;
            FastReportGlobal.EnableIntellisense = options.EnableOnlineDesignerIntelliSense;
            FastReportGlobal.IntelliSenseAssemblies = options.IntelliSenseAssemblies;

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

    }
}
