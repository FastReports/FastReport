using FastReport.Utils;

namespace FastReport.Data.Greenplum
{
    public class GreenplumAssemblyInitializer : AssemblyInitializerBase
    {
        public GreenplumAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(GreenplumDataConnection));
        }
    }
}
