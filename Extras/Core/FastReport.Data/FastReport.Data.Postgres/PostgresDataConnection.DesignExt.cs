using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using Npgsql;
using NpgsqlTypes;
using System.Data;

namespace FastReport.Data
{
    public partial class PostgresDataConnection
    {
        public override string GetConnectionId()
        {
            NpgsqlConnectionStringBuilder builder = new NpgsqlConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.Database;
            }
            catch
            {
            }
            return "Postgres: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            PostgresConnectionEditor editor = new PostgresConnectionEditor();
            return editor;
        }
    }
}