using System.Threading.Tasks;
using System.Threading;
using System;

namespace FastReport
{
    public abstract partial class ReportComponentBase
    {
        /// <summary>
        /// Gets the data from a datasource that the object is connected to.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task object.</returns>
        public virtual async Task GetDataAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Hyperlink.Calculate();

            if (!String.IsNullOrEmpty(Bookmark))
            {
                object value = await Report.CalcAsync(Bookmark, cancellationToken);
                Bookmark = value == null ? "" : value.ToString();
            }
        }
    }
}