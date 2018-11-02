using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
    public class SQLiteAssemblyInitializer : AssemblyInitializerBase
    {
        public SQLiteAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(SQLiteDataConnection));
        }
    }
}
