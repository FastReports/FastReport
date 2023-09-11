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
            _cacheEntryOptions = new DistributedCacheEntryOptions()
            {
                SlidingExpiration = cacheOptions.CacheDuration
            };
        }

        public void Add(WebReport webReport)
        {
            if(_cache.Get(webReport.ID) != null)
            {
                Debug.WriteLine($"WebReport with '{webReport.ID}' id was added before, but someone is trying to rewrite it");
                return;
            }

            _cache.Set(webReport.ID, WebReportToBytes(webReport), _cacheEntryOptions);
        }

        public void Touch(string id)
        {
            _cache.Refresh(id);
        }

        public WebReport Find(string id)
        {
            var bytes = _cache.Get(id);
            var webReport = BytesToWebReport(bytes);
            return webReport;
        }

        public void Remove(WebReport webReport)
        {
            _cache.Remove(webReport.ID);
        }


        public void Dispose()
        {
            
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
