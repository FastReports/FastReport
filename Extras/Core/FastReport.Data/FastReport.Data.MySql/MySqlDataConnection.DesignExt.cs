using System;
using FastReport.Data.ConnectionEditors;
using MySqlConnector;

namespace FastReport.Data
{
    public partial class MySqlDataConnection
    {
        public override Type GetParameterType()
        {
            return typeof(MySqlDbType);
        }

        public override string GetConnectionId()
        {
            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.Database;
            }
            catch
            {
            }
            return "MySQL: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new MySqlConnectionEditor();
        }
    }
}
