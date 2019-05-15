using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using FirebirdSql.Data.FirebirdClient;

namespace FastReport.Data
{
  public partial class FirebirdDataConnection : DataConnectionBase
  {
    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      return "\"" + value + "\"";
    }
    
    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      FbConnectionStringBuilder builder = new FbConnectionStringBuilder(ConnectionString);

      builder.UserID = userName;
      builder.Password = password;

      return builder.ToString();
    }

    public override Type GetConnectionType()
    {
      return typeof(FbConnection);
    }

    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
      CommandParameterCollection parameters)
    {
      FbDataAdapter adapter = new FbDataAdapter(selectCommand, connection as FbConnection);
      foreach (CommandParameter p in parameters)
      {
        FbParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (FbDbType)p.DataType, p.Size);
        parameter.Value = p.Value;
      }
      return adapter;
    }

    public override Type GetParameterType()
    {
      return typeof(FbDbType);
    }
  }
}
