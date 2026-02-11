using System;
using System.Data;
using System.Threading;

namespace FastReport.Data
{
    /// <summary>
    /// Defines a platform-agnostic interface for showing a progress/status indicator.
    /// </summary>
    public interface IProgressIndicator
    {
        /// <summary>
        /// Executes a long-running function on a background thread while showing a modal, cancellable progress window on the UI thread.
        /// </summary>
        /// <param name="work">The function to execute. It receives a <see cref="CancellationToken"/> and must return a <see cref="DataSet"/>.</param>
        /// <returns>The <see cref="DataSet"/> result from the executed function.</returns>
        DataSet ShowAndRun(Func<CancellationToken, DataSet> work);
    }
}
