using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Parameters;

namespace ClickHouse.Client
{
    public interface IClickHouseCommand : IDbCommand
    {
        new ClickHouseDbParameter CreateParameter();

        Task<ClickHouseRawResult> ExecuteRawResultAsync(CancellationToken cancellationToken);
    }
}
