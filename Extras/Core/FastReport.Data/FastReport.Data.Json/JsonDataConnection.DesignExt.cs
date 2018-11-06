using System;
using System.Data;
using System.ComponentModel;
using FastReport.Data.ConnectionEditors;
using System.Data.Common;
using System.Net;
using FastReport.Json;
using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to json database.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report = new Report();
    /// JsonDataConnection conn = new JsonDataConnection();
    /// conn.Json = @"c:\data.txt";
    /// report.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public partial class JsonDataConnection : DataConnectionBase
    {
        #region Public Methods
 
        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new JsonConnectionEditor();
        }
		
        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "Json: " + Json;
        }
		
		        /// <inheritdoc/>
        public override void TestConnection()
        {
            using (DataSet dataset = CreateDataSet())
            {
            }
        }

        #endregion
    }
}
