using System;
using FastReport.Table;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.CrossView
{
    /// <summary>
    /// Represents the crossview object that is used to print cube slice or slicegrid.
    /// </summary>
    public partial class CrossViewObject : TableBase
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
