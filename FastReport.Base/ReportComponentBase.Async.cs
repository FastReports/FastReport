using System.Threading.Tasks;
using System.Threading;

namespace FastReport
{
    public abstract partial class ReportComponentBase
    {
        public virtual Task GetDataAsync(CancellationToken cancellationToken)
        {
            GetDataShared();
            return Task.CompletedTask;
        }
    }
}