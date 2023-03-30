using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Data
{
	/// <summary>
	/// Datasource for stored procedure.
	/// </summary>
	partial class CsvDataConnection : DataConnectionBase
	{
		/// <summary>
		/// Does nothing
		/// </summary>
		partial void CheckForChangeConnection(CsvConnectionStringBuilder builder, string connectionString);
	}
}