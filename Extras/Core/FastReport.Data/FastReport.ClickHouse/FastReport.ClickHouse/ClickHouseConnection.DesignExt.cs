using ClickHouse.Client.ADO;
using ClickHouse.Client.Types;
using FastReport.Data;
using FastReport.Data.ConnectionEditors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.ClickHouse
{
    public partial class ClickHouseDataConnection
    {
        public override Type GetParameterType()
        {
            return typeof(ClickHouseTypeCode);
        }
        public override ConnectionEditorBase GetEditor()
        {
            return new ClickHouseConnectionEditor();
        }
        public override string GetConnectionId()
        {
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder(ConnectionString);
            string info = "";
            try
            {
                info = builder.Database;
            }
            catch
            {
            }
            return "ClickHouse: " + info;
        }
    }
}
