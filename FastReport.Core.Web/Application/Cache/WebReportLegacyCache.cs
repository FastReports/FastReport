using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
            internal readonly Timer Timer;
            internal WebReport WebReport;
            internal readonly WeakReference<WebReport> WeakReference;

            public CacheItem(WebReport webReport, TimeSpan dueTime)
            {
                WebReport = webReport;
                WeakReference = new WeakReference<WebReport>(webReport);
                Timer = new Timer(TimerAction, this, dueTime, Timeout.InfiniteTimeSpan);
            }

            private static void TimerAction(object state)
            {
                // clear reference to object
                var _item = (CacheItem)state;
                Debug.WriteLine($"Timer action! {_item.WebReport.ID} is null now!");
                _item.WebReport = null;
            }

            public void Dispose()
            {
                Timer.Dispose();
                WebReport?.InternalDispose();
                WebReport = null;
            }
        }

        private readonly List<CacheItem> cache = new List<CacheItem>();
        private readonly CacheOptions _cacheOptions;

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

            var item = new CacheItem(webReport, _cacheOptions.CacheDuration);

            cache.Add(item);
        }


        public void Touch(string id)
        {
            Find(id);
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
                if(cacheItem.WeakReference.TryGetTarget(out WebReport target))
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
            var cacheItem = FindPrivate(webReport.ID);
            cache.Remove(cacheItem);
            cacheItem.Dispose();
        }

        private CacheItem FindPrivate(string id)
        {
            return cache.Find(item => item.WeakReference.TryGetTarget(out WebReport target) && target.ID == id);
        }

        private void Clean()
        {
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

                return !item.WeakReference.TryGetTarget(out WebReport _);
            });
            Debug.WriteLineIf(removed > 0, $"Removed from cache: {removed}");
        }


        public void Dispose()
        {
            cache.Clear();
        }
    }
}
