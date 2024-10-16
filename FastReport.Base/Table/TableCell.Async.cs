using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Table
{
    public partial class TableCell
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
