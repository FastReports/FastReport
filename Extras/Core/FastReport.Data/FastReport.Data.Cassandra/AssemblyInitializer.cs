using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data.Cassandra
{
    public class CassandraAssemblyInitializer : AssemblyInitializerBase
    {
        public CassandraAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(CassandraDataConnection));
        }
    }
}
