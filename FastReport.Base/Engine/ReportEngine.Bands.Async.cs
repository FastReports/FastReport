using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private async Task PrepareBandAsync(BandBase band, bool getData, CancellationToken cancellationToken)
        {
            if (band.Visible)
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (getData)
                {
                    await band.GetDataAsync(cancellationToken);
                }
                PrepareBandShared(band);
            }
        }

        private async Task<float> CalcHeightAsync(BandBase band, CancellationToken cancellationToken)
        {
            // band is already prepared, its Height is ready to use
            if (band.IsRunning)
                return band.Height;

            band.SaveState();
            try
            {
                await PrepareBandAsync(band, true, cancellationToken);
                return band.Height;
            }
            finally
            {
                band.RestoreState();
            }
        }

        private async Task AddToOutputBandAsync(BandBase band, bool getData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            band.SaveState();

            try
            {
                await PrepareBandAsync(band, getData, cancellationToken);

                if (band.Visible)
                {
                    outputBand.SetRunning(true);

                    BandBase cloneBand = CloneBand(band);
                    cloneBand.Left = CurX;
                    cloneBand.Top = CurY;
                    cloneBand.Parent = outputBand;

                    CurY += cloneBand.Height;
                }
            }
            finally
            {
                band.RestoreState();
            }
        }

        private async Task ShowBandToPreparedPagesAsync(BandBase band, bool getData, CancellationToken cancellationToken)
        {
            bool bandCanStartNewPage = true;
            bandCanStartNewPage = BandCanStartNewPage(band);

            // handle "StartNewPage". Skip if it's the first row, avoid empty first page.
            if (band.StartNewPage && band.FlagUseStartNewPage && bandCanStartNewPage &&
                (band.RowNo != 1 || band.FirstRowStartsNewPage) && !band.Repeated)
            {
                EndColumn();
            }

            band.SaveState();
            try
            {
                await PrepareBandAsync(band, getData, cancellationToken);

                if (band.Visible)
                {
                    if (BandHasHardPageBreaks(band))
                    {
                        foreach (var b in SplitHardPageBreaks(band))
                        {
                            if (b.StartNewPage)
                                EndColumn();
                            await AddToPreparedPagesAsync(b, cancellationToken);
                        }
                    }
                    else
                    {
                        await AddToPreparedPagesAsync(band, cancellationToken);
                    }
                }
            }
            finally
            {
                band.RestoreState();
            }
        }


        public async Task ShowBandAsync(BandBase band, CancellationToken cancellationToken)
        {
            if (band != null)
                for (int i = 0; i < band.RepeatBandNTimes; i++)
                    await ShowBandAsync(band, true, cancellationToken);
        }

        private async Task ShowBandAsync(BandBase band, bool getData, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (band == null)
                return;

            BandBase saveCurBand = curBand;
            curBand = band;

            try
            {
                // do we need to keep child?
                ChildBand child = band.Child;
                bool showChild = child != null && !(band is DataBand && child.CompleteToNRows > 0) && !child.FillUnusedSpace &&
                    !(band is DataBand && child.PrintIfDatabandEmpty);
                if (showChild && band.KeepChild)
                {
                    StartKeep(band);
                }

                if (outputBand != null)
                {
                    await AddToOutputBandAsync(band, getData, cancellationToken);
                }
                else
                {
                    await ShowBandToPreparedPagesAsync(band, getData, cancellationToken);
                }

                ProcessTotals(band);
                if (band.Visible)
                {
                    await RenderOuterSubreportsAsync(band, cancellationToken);
                }

                // show child band. Skip if child is used to fill empty space: it was processed already
                if (showChild)
                {
                    await ShowBandAsync(child, cancellationToken);
                    if (band.KeepChild)
                    {
                        EndKeep();
                    }
                }
            }
            finally
            {
                curBand = saveCurBand;
            }
        }

        internal async Task AddToPreparedPagesAsync(BandBase band, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            bool isReportSummary = band is ReportSummaryBand;

            // check if band is service band (e.g. page header/footer/overlay).
            BandBase mainBand = band;
            // for child bands, check its parent band.
            if (band is ChildBand)
            {
                mainBand = (band as ChildBand).GetTopParentBand;
            }
            bool isPageBand = mainBand is PageHeaderBand || mainBand is PageFooterBand || mainBand is OverlayBand;
            bool isColumnBand = mainBand is ColumnHeaderBand || mainBand is ColumnFooterBand;

            // check if we have enough space for a band.
            bool checkFreeSpace = !isPageBand && !isColumnBand && band.FlagCheckFreeSpace;
            if (checkFreeSpace && await GetFreeSpaceAsync(cancellationToken) < band.Height)
            {
                // we don't have enough space. What should we do?
                // - if band can break, break it
                // - if band cannot break, check the band height:
                //   - it's the first row of a band and is bigger than page: break it immediately.
                //   - in other case, add a new page/column and tell the band that it must break next time.
                if (band.CanBreak || band.FlagMustBreak || (band.AbsRowNo == 1 && band.Height > PageHeight - await GetPageFooterHeightAsync(cancellationToken)))
                {
                    // since we don't show the column footer band in the EndLastPage, do it here.
                    if (isReportSummary)
                    {
                        ShowReprintFooters();
                        await ShowBandAsync(page.ColumnFooter, cancellationToken);
                    }
                    BreakBand(band);
                    return;
                }
                else
                {
                    EndColumn();
                    band.FlagMustBreak = true;
                    await AddToPreparedPagesAsync(band, cancellationToken);
                    band.FlagMustBreak = false;
                    return;
                }
            }
            else
            {
                // since we don't show the column footer band in the EndLastPage, do it here.
                if (isReportSummary)
                {
                    if ((band as ReportSummaryBand).KeepWithData)
                    {
                        EndKeep();
                    }
                    ShowReprintFooters(false);
                    await ShowBandAsync(page.ColumnFooter, cancellationToken);
                }
            }

            // check if we have a child band with FillUnusedSpace flag
            if (band.Child != null && band.Child.FillUnusedSpace)
            {
                // if we reprint a data/group footer, do not include the band height into calculation:
                // it is already counted in FreeSpace
                float bandHeight = band.Height;
                if (band.Repeated)
                {
                    bandHeight = 0;
                }
                while (await GetFreeSpaceAsync(cancellationToken) - bandHeight - band.Child.Height >= 0)
                {
                    float saveCurY = CurY;
                    await ShowBandAsync(band.Child, cancellationToken);
                    // nothing was printed, break to avoid an endless loop
                    if (CurY == saveCurY)
                    {
                        break;
                    }
                }
            }

            // adjust the band location
            if (band is PageFooterBand && !UnlimitedHeight)
            {
                CurY = PageHeight - await GetBandHeightWithChildrenAsync(band, cancellationToken);
            }
            if (!isPageBand)
            {
                band.Left += originX + CurX;
            }
            if (band.PrintOnBottom)
            {
                CurY = PageHeight - await GetPageFooterHeightAsync(cancellationToken) - ColumnFooterHeight;
                // if PrintOnBottom is applied to a band like DataFooter, print it with all its child bands
                // if PrintOnBottom is applied to a child band, print this band only.
                if (band is ChildBand)
                {
                    CurY -= band.Height;
                }
                else
                {
                    CurY -= await GetBandHeightWithChildrenAsync(band, cancellationToken);
                }
            }
            band.Top = CurY;

            // shift the band and decrease its width when printing hierarchy
            float saveLeft = band.Left;
            float saveWidth = band.Width;
            if (!isPageBand && !isColumnBand)
            {
                band.Left += hierarchyIndent;
                band.Width -= hierarchyIndent;
            }

            // add outline
            AddBandOutline(band);

            // add bookmarks
            band.AddBookmarks();

            // put the band to prepared pages. Do not put page bands twice
            // (this may happen when we render a subreport, or append a report to another one).
            bool bandAdded = true;
            bool bandAlreadyExists = false;
            if (isPageBand)
            {
                if (band is ChildBand)
                {
                    bandAlreadyExists = PreparedPages.ContainsBand(band.Name);
                }
                else
                {
                    bandAlreadyExists = PreparedPages.ContainsBand(band.GetType());
                }
            }

            if (!bandAlreadyExists)
            {
                bandAdded = PreparedPages.AddBand(band);
            }

            // shift CurY
            if (bandAdded && !(mainBand is OverlayBand))
            {
                CurY += band.Height;
            }

            // set left&width back
            band.Left = saveLeft;
            band.Width = saveWidth;
        }

        #endregion Private Methods
    }
}
