using System.Threading;
using System.Threading.Tasks;

namespace FastReport
{
    public partial class ZipCodeObject
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
