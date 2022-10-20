using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data.ElasticSearch
{
    public class ElasticSearchAssemblyInitializer : AssemblyInitializerBase
    {
        public ElasticSearchAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(ESDataSourceConnection));
        }
    }
}
