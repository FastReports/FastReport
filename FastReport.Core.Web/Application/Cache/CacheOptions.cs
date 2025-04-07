using Microsoft.Extensions.Caching.Memory;

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


        //public bool UseDistributedCache { get; set; } = false;

        // ?
        //public bool UseCustomCache ??
    }
}
