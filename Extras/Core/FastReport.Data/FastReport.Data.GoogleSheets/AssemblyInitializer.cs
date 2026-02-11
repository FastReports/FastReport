using FastReport.Utils;

namespace FastReport.Data
{
    public class GoogleSheetsAssemblyInitializer : AssemblyInitializerBase
    {
        public GoogleSheetsAssemblyInitializer()
        {
            RegisteredObjects.AddConnection(typeof(GoogleSheetsDataConnection));
        }
    }
}
