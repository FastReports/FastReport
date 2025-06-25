using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace FastReport.Web.Cache
{
    internal sealed class WebReportMemoryCache : IWebReportCache
    {

        private readonly IMemoryCache _cache;

        private readonly MemoryCacheEntryOptions _memoryCacheEntryDefaultOptions;

        public WebReportMemoryCache(IMemoryCache cache, CacheOptions cacheOptions)
        {
            _cache = cache;

            _memoryCacheEntryDefaultOptions = GetOptions(cacheOptions);
        }

        public void Add(WebReport webReport)
        {
            if (_cache.TryGetValue(webReport.ID, out _))
            {
                Debug.WriteLine($"WebReport with '{webReport.ID}' id was added before, but someone is trying to rewrite it");
                return;
            }

            if (webReport.CacheOptions != null)
                _cache.Set(webReport.ID, webReport, GetOptions(webReport.CacheOptions));
            else
                _cache.Set(webReport.ID, webReport, _memoryCacheEntryDefaultOptions);
        }

        private static void EvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            var webReport = value as WebReport;
            webReport.InternalDispose();
            //GC.Collect();
        }

        public bool Touch(string id)
        {
            return _cache.TryGetValue(id, out _);
        }

        public WebReport Find(string id)
        {
            return (WebReport)_cache.Get(id);
        }

        public void Remove(WebReport webReport)
        {
            Remove(webReport.ID);
        }

        public void Remove(string id)
        {
            _cache.Remove(id);
        }

        private static MemoryCacheEntryOptions GetOptions(WebReportCacheOptions cacheOptions)
        {
            var callback = new PostEvictionCallbackRegistration
            {
                EvictionCallback = EvictionCallback
            };
            return new MemoryCacheEntryOptions
            {
                SlidingExpiration = cacheOptions.CacheDuration,
                AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpirationDuration,
                AbsoluteExpiration = cacheOptions.AbsoluteExpiration,
                PostEvictionCallbacks = { callback },
            };
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }

}
