using FastReport.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private async Task RunReportPageAsync(ReportPage page, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            this.page = page;
            InitReprint();
            pageNameForRecalc = null;
            this.page.OnStartPage(EventArgs.Empty);
            bool previousPage = await StartFirstPageAsync(cancellationToken);
            OnStateChanged(this.page, EngineState.ReportPageStarted);
            OnStateChanged(this.page, EngineState.PageStarted);

            DataBand keepSummaryBand = FindDeepmostDataBand(page);
            if (keepSummaryBand != null)
                keepSummaryBand.KeepSummary = true;

            if (this.page.IsManualBuild)
                this.page.OnManualBuild(EventArgs.Empty);
            else
                await RunBandsAsync(page.Bands, cancellationToken);

            OnStateChanged(this.page, EngineState.PageFinished);
            OnStateChanged(this.page, EngineState.ReportPageFinished);
            EndLastPage();  // TODO
            //recalculate unlimited
            if (page.UnlimitedHeight || page.UnlimitedWidth)
            {
                PreparedPages.ModifyPageSize(page.Name);
                if (previousPage && pageNameForRecalc != null)
                    PreparedPages.ModifyPageSize(pageNameForRecalc);
            }
            //recalculate unlimited
            this.page.OnFinishPage(EventArgs.Empty);

            if (this.page.BackPage)
            {
                PreparedPages.InterleaveWithBackPage(PreparedPages.CurPage);
            }
        }

        private async Task RunReportPagesAsync(CancellationToken cancellationToken)
        {
#if TIMETRIAL
      if (new DateTime($YEAR, $MONTH, $DAY) < System.DateTime.Now)
        throw new Exception("The trial version is now expired!");
#endif

            for (int i = 0; i < Report.Pages.Count; i++)
            {
                ReportPage page = Report.Pages[i] as ReportPage;

                if (page != null)
                {
                    // Calc and apply visible expression if needed.
                    if (!String.IsNullOrEmpty(page.VisibleExpression))
                    {
                        page.Visible = page.CalcVisibleExpression(page.VisibleExpression);
                    }

                    // Apply printable expression if needed.
                    if (!String.IsNullOrEmpty(page.PrintableExpression))
                    {
                        page.Printable = page.CalcVisibleExpression(page.PrintableExpression);
                    }

                    if (page.Visible && page.Subreport == null)
                        await RunReportPageAsync(page, cancellationToken);
                }
                if (Report.Aborted)
                    break;
            }
        }

        private async Task RunBandsAsync(BandCollection bands, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            for (int i = 0; i < bands.Count; i++)
            {
                BandBase band = bands[i];
                if (band is DataBand)
                    await RunDataBandAsync(band as DataBand, cancellationToken);
                else if (band is GroupHeaderBand)
                    await RunGroupAsync(band as GroupHeaderBand, cancellationToken);
                if (Report.Aborted)
                    break;
            }
        }

        private async Task<bool> StartFirstPageAsync(CancellationToken cancellationToken)
        {
            var previousPage = StartFirstPageShared();

            // show report title and page header
            if (previousPage)
                await ShowBandAsync(page.ReportTitle, cancellationToken);
            else
            {
                if (page.Overlay != null)
                    await ShowBandAsync(page.Overlay, cancellationToken);
                if (page.TitleBeforeHeader)
                {
                    await ShowBandAsync(page.ReportTitle, cancellationToken);
                    ShowPageHeader();
                }
                else
                {
                    ShowPageHeader();
                    await ShowBandAsync(page.ReportTitle, cancellationToken);
                }
            }

            // show column header
            columnStartY = CurY;
            await ShowBandAsync(page.ColumnHeader, cancellationToken);

            // calculate CurX before starting column event depending on Right to Left or Left to Right layout
            if (Config.RightToLeft)
            {
                CurX = page.Columns.Positions[page.Columns.Positions.Count - 1] * Units.Millimeters;
            }
            else
            {
                CurX = page.Columns.Positions[0] * Units.Millimeters;
            }

            // start column event
            OnStateChanged(page, EngineState.ColumnStarted);
            ShowProgress();
            return previousPage;
        }

        #endregion Private Methods
    }
}