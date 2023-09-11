using FastReport.Web;

using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class FastReportServiceCollectionExtensions
    {
        /// <summary>
        /// Adds FastReport services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static IServiceCollection AddFastReport(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            var options = new WebReportOptions();
            AddFastReportPrivate(services, options);

            return services;
        }

        /// <summary>
        /// Adds FastReport services to the specified Microsoft.Extensions.DependencyInjection.IServiceCollection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="setupAction"></param>
        /// <exception cref="ArgumentNullException"></exception>
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

            var options = new WebReportOptions();
            setupAction(options);
            AddFastReportPrivate(services, options);

            return services;
        }

        private static void AddFastReportPrivate(IServiceCollection services, WebReportOptions options)
        {
            FastReport.Utils.Config.WebMode = true;

#if !WASM
            AddServices(services, options);
#else
            AddWasmServices(services, options);
#endif
        }

        private static ServiceDescriptor GetServiceDescriptorFromCollection<T>(IServiceCollection services)
        {
            return services.LastOrDefault((ServiceDescriptor d) => d.Lifetime == ServiceLifetime.Singleton && d.ServiceType == typeof(T));
        }
    }
}
