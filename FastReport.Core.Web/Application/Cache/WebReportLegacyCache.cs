using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace FastReport.Web.Cache
{
    /// <summary>
    /// Legacy WebReportCache implementation
    /// </summary>
    internal sealed class WebReportLegacyCache : IWebReportCache
    {
        public WebReportLegacyCache(CacheOptions cacheOptions)
        {
            _cacheOptions = cacheOptions;
        }

        sealed class CacheItem : IDisposable
        {
            internal readonly string Id;
            internal readonly Timer Timer;
            internal WebReport WebReport;
            internal WeakReference<WebReport> WeakReference { get; private set; }
            internal readonly DateTimeOffset? AbsoluteExpirationDate;
            private bool _disposed;

            public CacheItem(WebReport webReport, TimeSpan dueTime, TimeSpan? absoluteExpirationDuration, DateTimeOffset? absoluteExpiration)
            {
                Id = webReport.ID;
                WebReport = webReport;
                WeakReference = new WeakReference<WebReport>(webReport);
                Timer = new Timer(TimerAction, this, dueTime, Timeout.InfiniteTimeSpan);

                if (absoluteExpirationDuration.HasValue || absoluteExpiration.HasValue)
                {
                    var expirationFromDuration = absoluteExpirationDuration.HasValue
                        ? DateTimeOffset.UtcNow + absoluteExpirationDuration.Value
                        : (DateTimeOffset?)null;

                    var expirationFromDate = absoluteExpiration;

                    AbsoluteExpirationDate = (expirationFromDuration, expirationFromDate) switch
                    {
                        (DateTimeOffset duration, DateTimeOffset date) => duration < date ? duration : date,
                        (DateTimeOffset duration, null) => duration,
                        (null, DateTimeOffset date) => date,
                    };
                }
            }

            private static void TimerAction(object state)
            {
                // clear reference to object
                var _item = (CacheItem)state;
                Debug.WriteLine($"Timer action! {_item.WebReport.ID} is null now!");
                _item.WebReport = null;
            }

            public bool TryGetTarget(out WebReport target)
            {
                if ( _disposed)
                {
                    target = null;
                    return false;
                }

                return WeakReference.TryGetTarget(out target);
            }

            public void Dispose()
            {
                if (_disposed) return;

                _disposed = true;
                Timer.Dispose();
                WebReport?.InternalDispose();
                WebReport = null;
                WeakReference = null;
            }
        }

        private readonly List<CacheItem> cache = new List<CacheItem>();
        private readonly WebReportCacheOptions _cacheOptions;

        public void Add(WebReport webReport)
        {
            if (webReport == null)
                throw new ArgumentNullException(nameof(webReport));

            Clean();

            // try to find webReport with the same key
            if(FindPrivate(webReport.ID) != null)
            {
                Debug.WriteLine($"WebReport with '{webReport.ID}' id was added before, but someone is trying to rewrite it");
                return;
            }

            var cacheOptions = webReport.CacheOptions ?? _cacheOptions;
            var item = new CacheItem(webReport,
                cacheOptions.CacheDuration,
                cacheOptions.AbsoluteExpirationDuration,
                cacheOptions.AbsoluteExpiration);
            cache.Add(item);
        }


        public bool Touch(string id)
        {
            return Find(id) != null;
        }

        public WebReport Find(string id)
        {
            Clean();

            return FindWithRefresh(id)?.WebReport;
        }

        private CacheItem FindWithRefresh(string id)
        {
            var cacheItem = FindPrivate(id);
            if (cacheItem != null)
            {
                if (cacheItem.AbsoluteExpirationDate.HasValue && cacheItem.AbsoluteExpirationDate < DateTimeOffset.UtcNow)
                {
                    cacheItem.Dispose();
                }

                if (cacheItem.TryGetTarget(out WebReport target))
                {
                    // refresh item
                    cacheItem.WebReport = target;
                    cacheItem.Timer.Change(_cacheOptions.CacheDuration, Timeout.InfiniteTimeSpan);
                }
            }
            return cacheItem;
        }

        public void Remove(WebReport webReport)
        {
            Remove(webReport.ID);
        }

        public void Remove(string webReportId)
        {
            var cacheItem = FindPrivate(webReportId);
            cache.Remove(cacheItem);
            cacheItem.Dispose();
        }

        private CacheItem FindPrivate(string id)
        {
            return cache.Find(item => item.Id == id);
        }

        private void Clean()
        {
            var now = DateTimeOffset.UtcNow;
            int removed = cache.RemoveAll(item =>
            {
                if (item == null)
                    return true;

                var weakReference = item.WeakReference;
                if (weakReference == null)
                {
                    item.Dispose();
                    return true;
                }

                if (item.AbsoluteExpirationDate != null && item.AbsoluteExpirationDate < now)
                {
                    item.Dispose();
                    return true;
                }

                return !item.TryGetTarget(out WebReport _);
            });
            Debug.WriteLineIf(removed > 0, $"Removed from cache: {removed}");
        }


        public void Dispose()
        {
            cache.Clear();
        }
    }
}
