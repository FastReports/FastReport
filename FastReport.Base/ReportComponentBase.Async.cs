using System.Threading.Tasks;
using System.Threading;
using System;

namespace FastReport
{
    public abstract partial class ReportComponentBase
    {
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