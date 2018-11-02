using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
  public class PostgresAssemblyInitializer : AssemblyInitializerBase
  {
    public PostgresAssemblyInitializer()
    {
      RegisteredObjects.AddConnection(typeof(PostgresDataConnection));
    }
  }
}
