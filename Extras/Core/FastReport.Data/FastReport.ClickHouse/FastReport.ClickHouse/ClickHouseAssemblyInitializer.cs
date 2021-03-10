using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.ClickHouse
{
    public class ClickHouseAssemblyInitializer : AssemblyInitializerBase
    {
        public ClickHouseAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(ClickHouseDataConnection),"ClickHouse");
        }
    }
}
