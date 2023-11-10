using System;
using System.Collections.Generic;
using System.Text;
using FastReport.Utils;

namespace FastReport.Data
{
  public class GoogleAssemblyInitializer : AssemblyInitializerBase
  {
    public GoogleAssemblyInitializer()
    {
        RegisteredObjects.AddConnection(typeof(GoogleSheetsDataConnection));
	}
  }
}
