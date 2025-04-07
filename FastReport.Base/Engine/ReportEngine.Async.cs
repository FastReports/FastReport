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
            cancellationToken.ThrowIfCancellationRequested();

            OnStateChanged(Report, EngineState.ReportStarted);

            if (page == null)
                await RunReportPagesAsync(cancellationToken);
            else
                await RunReportPageAsync(page, cancellationToken);

            OnStateChanged(Report, EngineState.ReportFinished);
        }

        private async Task<float> GetBandHeightWithChildrenAsync(BandBase band, CancellationToken cancellationToken)
        {
            float result = 0;

            while (band != null)
            {
                if (CanPrint(band))
                    result += (band.CanGrow || band.CanShrink) ? await CalcHeightAsync(band, cancellationToken) : band.Height;
                else if (FinalPass && !String.IsNullOrEmpty(band.VisibleExpression) && band.VisibleExpression.Contains("TotalPages"))
                    result += (band.CanGrow || band.CanShrink) ? await CalcHeightAsync(band, cancellationToken) : band.Height;
                band = band.Child;
                if (band != null && ((band as ChildBand).FillUnusedSpace || (band as ChildBand).CompleteToNRows != 0))
                    break;
            }

            return result;
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

        internal Task<float> GetPageFooterHeightAsync(CancellationToken token) => GetBandHeightWithChildrenAsync(page.PageFooter, token);

        internal Task<float> GetColumnFooterHeightAsync(CancellationToken token) => GetBandHeightWithChildrenAsync(page.ColumnFooter, token);

        internal async Task<float> GetFreeSpaceAsync(CancellationToken token)
        {
            float pageHeight = PageHeight;
            pageHeight -= await GetPageFooterHeightAsync(token);
            pageHeight -= await GetColumnFooterHeightAsync(token);
            pageHeight -= GetFootersHeight();
            return Converter.DecreasePrecision(pageHeight - CurY, 2);
        }

        #endregion Internal Methods
    }
}
