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

            var callback = new PostEvictionCallbackRegistration
            {
                EvictionCallback = EvictionCallback
            };
            _memoryCacheEntryDefaultOptions = GetOptions(cacheOptions);
            _memoryCacheEntryDefaultOptions.PostEvictionCallbacks.Add(callback);
        }

        public void Add(WebReport webReport)
        {
            if(_cache.TryGetValue(webReport.ID, out _))
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
            _cache.Remove(webReport.ID);
        }

        private static MemoryCacheEntryOptions GetOptions(WebReportCacheOptions cacheOptions)
        {
            return new MemoryCacheEntryOptions
            {
                SlidingExpiration = cacheOptions.CacheDuration,
                AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpirationDuration,
                AbsoluteExpiration = cacheOptions.AbsoluteExpiration,
            };
        }

        public void Dispose()
        {
            _cache.Dispose();
        }
    }

}
