using Microsoft.Extensions.Caching.Memory;

namespace FastReport.Web.Cache
{
    /// <summary>
    /// For backward compatibility
    /// </summary>
    internal static class WebReportCache
    {
        private static IWebReportCache _instance;

        public static IWebReportCache Instance
        {
            get
            {
                return _instance;
            }
            internal set
            {
                _instance = value;
            }
        }

        internal static MemoryCache GetDefaultMemoryCache() => new MemoryCache(new MemoryCacheOptions());

    }

}
