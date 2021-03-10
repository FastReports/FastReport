using System.Data.Common;
using ClickHouse.Client.ADO.Adapters;
using ClickHouse.Client.ADO.Parameters;

namespace ClickHouse.Client.ADO
{
    public class ClickHouseConnectionFactory : DbProviderFactory
    {
        public override DbConnection CreateConnection() => new ClickHouseConnection();

        public override DbDataAdapter CreateDataAdapter() => new ClickHouseDataAdapter();

        public override DbConnectionStringBuilder CreateConnectionStringBuilder() => new ClickHouseConnectionStringBuilder();

        public override DbParameter CreateParameter() => new ClickHouseDbParameter();

        public override DbCommand CreateCommand() => new ClickHouseCommand();
    }
}
