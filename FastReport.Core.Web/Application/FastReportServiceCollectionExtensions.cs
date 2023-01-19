using FastReport.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class FastReportServiceCollectionExtensions
    {
        [EditorBrowsable(EditorBrowsableState.Never)]   // TODO
        public static IServiceCollection AddFastReport(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            FastReport.Utils.Config.WebMode = true;

            var manager = GetServiceFromCollection<ApplicationPartManager>(services);
            if(manager != null)
            {
                var curAssembly = typeof(FastReportServiceCollectionExtensions).Assembly;
                var frAssembly = manager.ApplicationParts.FirstOrDefault(apPart => apPart.Name == curAssembly.GetName().Name);
                if (frAssembly == null)
                {
                    // Register this assembly as part of user application
                    var assemblyPart = new AssemblyPart(curAssembly);
                    manager.ApplicationParts.Add(assemblyPart);
                }

                // Then we can use new controllers
                FastReportOptions.UseNewControllers = true;

                services.TryAddSingleton<IResourceLoader, InternalResourceLoader>();
            }

            return services;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]   // TODO
        public static IServiceCollection AddFastReport(this IServiceCollection services, Action<WebReportOptions> setupAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (setupAction == null)
            {
                throw new ArgumentNullException(nameof(setupAction));
            }

            AddFastReport(services);

            return services;
        }

        private static T GetServiceFromCollection<T>(IServiceCollection services)
        {
            return (T)(services.LastOrDefault((ServiceDescriptor d) => d.ServiceType == typeof(T))?.ImplementationInstance);
        }
    }
}
