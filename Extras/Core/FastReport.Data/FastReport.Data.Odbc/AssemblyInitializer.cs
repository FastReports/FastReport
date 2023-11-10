using FastReport.Utils;

namespace FastReport.Data
{
    public class OdbcAssemblyInitializer : AssemblyInitializerBase
    {
        public OdbcAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(OdbcDataConnection));
        }
    }
}
