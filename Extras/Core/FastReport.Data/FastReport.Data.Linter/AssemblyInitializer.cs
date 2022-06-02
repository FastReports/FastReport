using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data.Cassandra
{
    public class LinterAssemblyInitializer : AssemblyInitializerBase
    {
        public LinterAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(LinterDataConnection));
        }
    }
}
