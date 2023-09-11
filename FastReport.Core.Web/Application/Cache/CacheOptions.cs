using Microsoft.Extensions.Caching.Memory;

using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web.Cache
{
    public class CacheOptions
    {
        private const int DEFAULT_TIMER_MINUTES = 15;

        /// <summary>
        /// Use legacy WebReport cache instead of <see cref="Microsoft.Extensions.Caching.Memory.MemoryCache"/>.
        /// Default value: true
        /// </summary>
        public bool UseLegacyWebReportCache { get; set; } = true;


        //public bool UseDistributedCache { get; set; } = false;

        // ?
        //public bool UseCustomCache ??

        /// <summary>
        /// Reports' lifetime in cache.
        /// If report is not used the specified time and there is no references to the object, it will be deleted from cache.
        /// Default value: "15".
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(DEFAULT_TIMER_MINUTES);
    }
}
