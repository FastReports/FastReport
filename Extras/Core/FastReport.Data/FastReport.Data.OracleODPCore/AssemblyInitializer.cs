using FastReport.Utils;

namespace FastReport.Data
{
    public class OracleODPCoreAssemblyInitializer : AssemblyInitializerBase
  {
    public OracleODPCoreAssemblyInitializer()
    {
      RegisteredObjects.AddConnection(typeof(OracleDataConnection));
    }
  }
}
