using System;
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

        sealed class CacheItem
        {
            internal Timer Timer;
            internal WebReport WebReport;
            internal readonly WeakReference<WebReport> WeakReference;

            public CacheItem(WebReport webReport)
            {
                WebReport = webReport;
                WeakReference = new WeakReference<WebReport>(WebReport);
            }
        }

        readonly List<CacheItem> cache = new List<CacheItem>();

        public void Add(WebReport webReport)
        {
            Clean();

            if (webReport == null)
                throw new ArgumentNullException(nameof(webReport));

            var item = new CacheItem(webReport);

            item.Timer = new Timer(state =>
            {
                // clear reference to object
                var _item = (CacheItem)state;
                Debug.WriteLine($"Timer action! {_item.WebReport.ID} is null now!");
                _item.WebReport = null;
            },
            item, FastReportGlobal.FastReportOptions.CacheOptions.CacheDuration, Timeout.InfiniteTimeSpan);

            cache.Add(item);
        }


        public void Touch(string id)
        {
            Find(id);
        }

        public WebReport Find(string id)
        {
            Clean();

            return FindPrivate(id)?.WebReport;
        }

        private CacheItem FindPrivate(string id)
        {
            return cache.Find(item =>
            {
                if (item.WeakReference.TryGetTarget(out WebReport target) && target.ID == id)
                {
                    // refresh item
                    item.WebReport = target;
                    item.Timer.Change(FastReportGlobal.FastReportOptions.CacheOptions.CacheDuration, Timeout.InfiniteTimeSpan);
                    return true;
                }
                return false;
            });
        }

        public void Remove(WebReport webReport)
        {
            var cacheItem = FindPrivate(webReport.ID);
            cache.Remove(cacheItem);
        }

        private void Clean()
        {
            int removed = cache.RemoveAll(item => !item.WeakReference.TryGetTarget(out WebReport _));
            Debug.WriteLineIf(removed > 0, $"Removed from cache: {removed}");
        }


        public void Dispose()
        {
            cache.Clear();
        }
    }
}
