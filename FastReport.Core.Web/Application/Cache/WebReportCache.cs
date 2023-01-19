using Microsoft.Extensions.Caching.Memory;

namespace FastReport.Web.Cache
{
    /// <summary>
    /// For backward compability
    /// </summary>
    internal static class WebReportCache
    {
        private static IWebReportCache _instance;

        public static IWebReportCache Instance
        {
            get
            {
                if (_instance == null)
                {
                    if (FastReportGlobal.FastReportOptions.CacheOptions.UseLegacyWebReportCache)
                        _instance = new WebReportLegacyCache();
                    else
                        _instance = new WebReportMemoryCache(new MemoryCache(new MemoryCacheOptions()));

                }
                return _instance;
            }
        }

    }

}
