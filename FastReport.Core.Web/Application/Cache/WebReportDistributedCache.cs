using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;

namespace FastReport.Web.Cache
{

    internal class WebReportDistributedCache : IWebReportCache
    {

        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _cacheEntryOptions;

        //private static readonly IFormatter _bf = new BinaryFormatter();

        public WebReportDistributedCache(IDistributedCache cache, CacheOptions cacheOptions)
        {
            _cache = cache;
            _cacheEntryOptions = GetOptions(cacheOptions);
        }

        public void Add(WebReport webReport)
        {
            if(_cache.Get(webReport.ID) != null)
            {
                Debug.WriteLine($"WebReport with '{webReport.ID}' id was added before, but someone is trying to rewrite it");
                return;
            }

            if (webReport.CacheOptions != null)
                _cache.Set(webReport.ID, WebReportToBytes(webReport), GetOptions(webReport.CacheOptions));
            else
                _cache.Set(webReport.ID, WebReportToBytes(webReport), _cacheEntryOptions);
        }

        public bool Touch(string id)
        {
            _cache.Refresh(id);
            return true;
        }

        public WebReport Find(string id)
        {
            var bytes = _cache.Get(id);
            var webReport = BytesToWebReport(bytes);
            return webReport;
        }

        public void Remove(WebReport webReport)
        {
            Remove(webReport.ID);
        }

        public void Remove(string webReportId)
        {
            _cache.Remove(webReportId);
        }

        public void Dispose()
        {
            
        }

        private static DistributedCacheEntryOptions GetOptions(WebReportCacheOptions cacheOptions)
        {
            return new DistributedCacheEntryOptions
            {
                SlidingExpiration = cacheOptions.CacheDuration,
                AbsoluteExpirationRelativeToNow = cacheOptions.AbsoluteExpirationDuration,
                AbsoluteExpiration = cacheOptions.AbsoluteExpiration,
            };
        }

        private static byte[] WebReportToBytes(WebReport value)
        {
            throw new NotImplementedException();

            //var ms = new MemoryStream();
            //_bf.Serialize(ms, value);
            //return ms.ToArray();
        }

        private static WebReport BytesToWebReport(byte[] value)
        {
            throw new NotImplementedException();
            //using (var ms = new MemoryStream(value, false))
            //{
            //    var obj = _bf.Deserialize(ms);
            //    var webReport = obj as WebReport;
            //    return webReport;
            //}
        }
    }

}
