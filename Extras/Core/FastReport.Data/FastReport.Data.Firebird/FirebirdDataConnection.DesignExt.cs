using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
  public partial class FirebirdDataConnection : DataConnectionBase
  {
    public override string GetConnectionId()
    {
      FbConnectionStringBuilder builder = new FbConnectionStringBuilder(ConnectionString);
      string info = "";
      try
      {
        info = builder.Database;
      }
      catch
      {
      }  
      return "Firebird: " + info;
    }

    public override ConnectionEditorBase GetEditor()
    {
      return new FirebirdConnectionEditor();
    }
  }
}
