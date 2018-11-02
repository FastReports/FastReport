using FastReport.Utils;


namespace FastReport.Data
{
    public class CouchbaseAssemblyInitializer : AssemblyInitializerBase
    {
        public CouchbaseAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(CouchbaseDataConnection));
        }
    }
}