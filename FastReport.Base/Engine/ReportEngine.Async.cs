using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Engine
{
    /// <summary>
    /// Represents the report engine.
    /// </summary>
    public partial class ReportEngine
    {

        #region Private Methods

        private async Task RunReportPagesAsync(ReportPage page, CancellationToken cancellationToken)
        {
            OnStateChanged(Report, EngineState.ReportStarted);

            if (page == null)
                await RunReportPagesAsync(cancellationToken);
            else
                await RunReportPageAsync(page, cancellationToken);

            OnStateChanged(Report, EngineState.ReportFinished);
        }

        #endregion Private Methods

        #region Internal Methods


        internal Task<bool> RunAsync(bool runDialogs, bool append, bool resetDataState, CancellationToken cancellationToken)
        {
            return RunAsync(runDialogs, append, resetDataState, null, cancellationToken);
        }

        internal async Task<bool> RunAsync(bool runDialogs, bool append, bool resetDataState, ReportPage page, CancellationToken cancellationToken)
        {
            try
            {
                RunPhase1(resetDataState);
                if (runDialogs && !(await RunDialogsAsync()))
                    return false;
                await RunPhase2Async(append, page, cancellationToken);
            }
            finally
            {
                RunFinished();
            }
            return true;
        }

        internal async Task RunPhase2Async(bool append, ReportPage page, CancellationToken cancellationToken)
        {
            Config.ReportSettings.OnStartProgress(Report);
            PrepareToFirstPass(append);
            await RunReportPagesAsync(page, cancellationToken);

            ResetLogicalPageNumber();
            if (Report.DoublePass && !Report.Aborted)
            {
                finalPass = true;
                PrepareToSecondPass();
                await RunReportPagesAsync(page, cancellationToken);
            }
        }

        #endregion Internal Methods
    }
}
