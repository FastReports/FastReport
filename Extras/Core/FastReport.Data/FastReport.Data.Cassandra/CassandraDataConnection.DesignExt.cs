using FastReport.Data.ConnectionEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data.Cassandra
{
    public partial class CassandraDataConnection
    {
        /// <inheritdoc/>
        public override void TestConnection()
        {
            InitConnection();
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "Cassandra: " + ConnectionString;
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new CassandraConnectionEditor();
        }
    }
}
