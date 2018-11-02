using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using System.Data.SQLite;

namespace FastReport.Data
{
    public partial class SQLiteDataConnection
    {
        public override string GetConnectionId()
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(ConnectionString);
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
