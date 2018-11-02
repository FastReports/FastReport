using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
    public class MongoDBAssemblyInitializer : AssemblyInitializerBase
    {
        public MongoDBAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(MongoDBDataConnection));
        }
    }
}
