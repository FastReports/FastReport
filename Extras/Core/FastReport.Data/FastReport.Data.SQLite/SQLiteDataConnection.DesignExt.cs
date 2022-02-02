#undef FRCORE

using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
#if FRCORE
using Microsoft.Data.Sqlite;
#else
using System.Data.SQLite;
#endif

namespace FastReport.Data
{
    public partial class SQLiteDataConnection
    {
        public override string GetConnectionId()
        {
#if FRCORE
            var builder = new SqliteConnectionStringBuilder(ConnectionString);
#else
            var builder = new SQLiteConnectionStringBuilder(ConnectionString);
#endif
            string info = "";
            try
            {
                info = builder.DataSource ?? String.Empty;
            }
            catch
            {
            }
            return "SQLite: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new SQLiteConnectionEditor();
        }
    }
}
