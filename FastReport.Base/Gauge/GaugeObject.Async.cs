using System;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Gauge
{
    public partial class GaugeObject
    {
        #region Report Engine

        /// <inheritdoc/>
        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);
            GetDataShared();
        }

        #endregion // Report Engine
    }
}
