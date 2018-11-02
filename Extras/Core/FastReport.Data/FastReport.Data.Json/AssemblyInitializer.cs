using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
    public class JsonAssemblyInitializer : AssemblyInitializerBase
    {
        public JsonAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(JsonDataConnection));
        }
    }
}
