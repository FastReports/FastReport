using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
    public class FirebirdAssemblyInitializer : AssemblyInitializerBase
    {
        public FirebirdAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(FirebirdDataConnection));
        }
    }
}
