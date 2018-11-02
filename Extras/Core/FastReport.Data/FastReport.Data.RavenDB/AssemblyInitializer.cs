using FastReport.Utils;


namespace FastReport.Data
{
    public class RavenDBAssemblyInitializer : AssemblyInitializerBase
    {
        public RavenDBAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(RavenDBDataConnection));
        }
    }
}