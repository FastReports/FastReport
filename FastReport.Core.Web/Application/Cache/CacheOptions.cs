using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web.Cache
{
    public class CacheOptions : WebReportCacheOptions
    {
        /// <summary>
        /// Use legacy WebReport cache instead of <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/>.
        /// Default value: true
        /// </summary>
        public bool UseLegacyWebReportCache { get; set; } = true;

        /// <summary>
        /// Enables WebReport cache management based on the user's <see cref="Microsoft.AspNetCore.Components.Server.Circuits.Circuit"/> in Blazor Server mode.
        /// 
        /// When enabled, the WebReport lifetime follows the user's session and settings such as 
        /// <see cref="WebReportCacheOptions.CacheDuration"/>, <see cref="WebReportCacheOptions.AbsoluteExpirationDuration"/>
        /// and <see cref="WebReportCacheOptions.AbsoluteExpiration"/>.
        /// </summary>
        /// <remarks>
        /// If a user selects a different WebReport during the same session, the previous report is immediately removed from the cache.
        /// Similarly, when the user closes the browser tab or disconnects, the associated WebReport is promptly removed.
        /// </remarks>
        public bool UseCircuitScope { get; set; } = false;

        //public bool UseDistributedCache { get; set; } = false;

        // ?
        //public bool UseCustomCache ??
    }
}
