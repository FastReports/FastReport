using System;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Barcode
{
    public partial class BarcodeObject
    {
        #region Report Engine

        /// <inheritdoc/>
        public override async Task GetDataAsync(CancellationToken cancellationToken)
        {
            await base.GetDataAsync(cancellationToken);
            GetDataShared();
        }
        #endregion
    }
}
