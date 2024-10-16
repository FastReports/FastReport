using System;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport
{
    public partial class RFIDLabel
    {

        /// <inheritdoc/>
        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);
            GetDataShared();
        }
    }
}
