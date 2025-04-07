using System;

namespace FastReport.Web.Cache
{
    public class WebReportCacheOptions
    {
        private const int DEFAULT_TIMER_MINUTES = 15;

        /// <summary>
        /// Reports' lifetime in cache.
        /// If report is not used the specified time and there is no references to the object, it will be deleted from cache.
        /// Default value: 15 minutes.
        /// </summary>
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(DEFAULT_TIMER_MINUTES);

        /// <summary>
        /// The absolute lifetime of a report in the cache.
        /// Unlike <see cref="CacheDuration"/>, which specifies a sliding expiration policy, this property defines
        /// the maximum amount of time a report can exist in the cache, regardless of activity or references.
        /// If not specified, the cache entry will rely solely on the sliding expiration defined by <see cref="CacheDuration"/>.
        /// </summary>
        public TimeSpan? AbsoluteExpirationDuration { get; set; }

        /// <summary>
        /// Specifies the exact expiration date and time for the report in the cache.
        /// When both <see cref="AbsoluteExpiration"/> and <see cref="AbsoluteExpirationDuration"/> are specified, 
        /// the earlier of the two values determines when the cache entry expires.
        /// If not set, the cache entry will rely on either the sliding expiration defined by <see cref="CacheDuration"/>
        /// or the absolute lifetime defined by <see cref="AbsoluteExpirationDuration"/>.
        /// </summary>
        public DateTimeOffset? AbsoluteExpiration { get; set; }
    }
}
