using System;
using FastReport.Data.ConnectionEditors;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to Couchbase database.
    /// </summary>
    public partial class CouchbaseDataConnection
    {
        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(CouchbaseDataConnection);
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            CouchbaseConnectionStringBuilder builder = new CouchbaseConnectionStringBuilder(ConnectionString);
            return "Couchbase: " + builder.ToString();
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new CouchbaseConnectionEditor();
        }
    }
}
