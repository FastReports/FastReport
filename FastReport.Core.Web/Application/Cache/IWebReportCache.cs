using System;

namespace FastReport.Web.Cache
{

    internal interface IWebReportCache : IDisposable
    {
        void Add(WebReport webReport);

        void Touch(string id);

        WebReport Find(string id);

        void Remove(WebReport webReport);

    }
}
