using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using FastReport.Utils;

namespace FastReport.Data
{
  /// <summary>
  /// Represents a connection to any database through ODBC.
  /// </summary>
  /// <example>This example shows how to add a new connection to the report.
  /// <code>
  /// Report report1;
  /// OdbcDataConnection conn = new OdbcDataConnection();
  /// conn.ConnectionString = "your_connection_string";
  /// report1.Dictionary.Connections.Add(conn);
  /// conn.CreateAllTables();
  /// </code>
  /// </example>
  public partial class OdbcDataConnection : DataConnectionBase
  {
    /// <inheritdoc/>
    protected override string GetConnectionStringWithLoginInfo(string userName, string password)
    {
      OdbcConnectionStringBuilder builder = new OdbcConnectionStringBuilder(ConnectionString);

      builder.Remove("uid");
      builder.Add("uid", userName);

      builder.Remove("pwd");
      builder.Add("pwd", password);

      return builder.ToString();
    }

    /// <inheritdoc/>
    public override string QuoteIdentifier(string value, DbConnection connection)
    {
      // already quoted?
      if (value.EndsWith("\"") || value.EndsWith("]") || value.EndsWith("'") || value.EndsWith("`"))
        return value;

      // Odbc is universal connection, so we need quoting dependent on used database type
      using (OdbcCommandBuilder builder = new OdbcCommandBuilder())
      {
        return builder.QuoteIdentifier(value, connection as OdbcConnection);
      }
    }

    /// <inheritdoc/>
    public override Type GetConnectionType()
    {
      return typeof(OdbcConnection);
    }

    /// <inheritdoc/>
    public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
      CommandParameterCollection parameters)
    {
      OdbcDataAdapter adapter = new OdbcDataAdapter(selectCommand, connection as OdbcConnection);
      foreach (CommandParameter p in parameters)
      {
        OdbcParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (OdbcType)p.DataType, p.Size);
        parameter.Value = p.Value;
      }

      return adapter;
    }

    /// <inheritdoc/>
    public override Type GetParameterType()
    {
      return typeof(OdbcType);
    }
  }
}
