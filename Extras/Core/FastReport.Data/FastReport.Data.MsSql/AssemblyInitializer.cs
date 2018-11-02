using FastReport.Utils;

namespace FastReport.Data
{
    public class MsSqlAssemblyInitializer : AssemblyInitializerBase
  {
    public MsSqlAssemblyInitializer()
    {
      RegisteredObjects.AddConnection(typeof(MsSqlDataConnection));
    }
  }
}
