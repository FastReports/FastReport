using System.Data;
using ClickHouse.Client.ADO;

namespace ClickHouse.Client
{
    public interface IClickHouseConnection : IDbConnection
    {
        new ClickHouseCommand CreateCommand();
    }
}
