using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web
{
    class WebReportCache
    {
        #region Singletone

        public static readonly WebReportCache Instance;

        static WebReportCache()
        {
            Instance = new WebReportCache();
        }

        private WebReportCache()
        {
        }

        #endregion

        class CacheItem
        {
            public Timer Timer;
            public WebReport WebReport1;
            public WeakReference<WebReport> WebReport2;
        }

        List<CacheItem> cache = new List<CacheItem>();

        public void Add(WebReport webReport)
        {
            Clean();

            if (webReport == null)
                throw new ArgumentNullException(nameof(webReport));

            var item = new CacheItem()
            {
                WebReport1 = webReport,
                WebReport2 = new WeakReference<WebReport>(webReport),
            };

            item.Timer = new Timer(state =>
            {
                // clear reference to object
                var _item = ((CacheItem)state);
                _item.WebReport1 = null;
            },
            item, TimeSpan.FromMinutes(FastReportGlobal.FastReportOptions.CacheDuration), Timeout.InfiniteTimeSpan);

            cache.Add(item);
        }

        public bool Touch(string id)
        {
            Clean();
            return Find(id) != null;
        }

        public WebReport Find(string id)
        {
            Clean();

            return cache.Find(item =>
            {
                if (item.WebReport2.TryGetTarget(out WebReport target) && target.ID == id)
                {
                    // refresh item
                    item.WebReport1 = target;
                    item.Timer.Change(TimeSpan.FromMinutes(FastReportGlobal.FastReportOptions.CacheDuration), Timeout.InfiniteTimeSpan);
                    return true;
                }
                return false;
            })?.WebReport1;
        }

        public void Remove(WebReport webReport)
        {
            cache = cache.Where(item => item.WebReport2.TryGetTarget(out WebReport target) && target != webReport).ToList();
        }

        private void Clean()
        {
            cache = cache.Where(item => item.WebReport2.TryGetTarget(out WebReport _)).ToList();
        }
    }
}
