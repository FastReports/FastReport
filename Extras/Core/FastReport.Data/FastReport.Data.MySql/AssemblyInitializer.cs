using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
  public class MySqlAssemblyInitializer : AssemblyInitializerBase
  {
    public MySqlAssemblyInitializer()
    {
      RegisteredObjects.AddConnection(typeof(MySqlDataConnection));
    }
  }
}
