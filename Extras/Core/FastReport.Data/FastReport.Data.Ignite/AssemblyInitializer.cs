using FastReport.Utils;

namespace FastReport.Data
{
    public class IgniteAssemblyInitializer : AssemblyInitializerBase
    {
        public IgniteAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(IgniteDataConnection));
        }
    }
}
