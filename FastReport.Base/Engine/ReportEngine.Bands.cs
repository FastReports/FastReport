using System;
using System.Collections.Generic;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Fields

        private BandBase outputBand;

        #endregion Fields

        #region Private Methods

        private void PrepareBand(BandBase band, bool getData)
        {
            if (band.Visible)
            {
                if (getData)
                {
                    band.GetData();
                }
                RenderInnerSubreports(band);
                band.CalcHeight();
            }
        }

        private float CalcHeight(BandBase band)
        {
            band.SaveState();
            try
            {
                PrepareBand(band, true);
                return band.Height;
            }
            finally
            {
                band.RestoreState();
            }
        }

        private BandBase CloneBand(BandBase band)
        {
            // clone a band and all its objects
            BandBase cloneBand = Activator.CreateInstance(band.GetType()) as BandBase;
            cloneBand.Assign(band);
            cloneBand.SetReport(Report);
            cloneBand.SetRunning(true);

            foreach (ReportComponentBase obj in band.Objects)
            {
                ReportComponentBase cloneObj = Activator.CreateInstance(obj.GetType()) as ReportComponentBase;
                cloneObj.AssignAll(obj);
                cloneBand.Objects.Add(cloneObj);
            }

            return cloneBand;
        }

        private void AddToOutputBand(BandBase band, bool getData)
        {
            band.SaveState();

            try
            {
                PrepareBand(band, getData);

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

        private void ShowBandToPreparedPages(BandBase band, bool getData)
        {
            // handle "StartNewPage". Skip if it's the first row, avoid empty first page.
            if ((band.StartNewPage && !(band.Parent is PageHeaderBand)) && band.FlagUseStartNewPage && (band.RowNo != 1 || band.FirstRowStartsNewPage) &&
                !band.Repeated)
            {
                EndColumn();
            }

            band.SaveState();
            try
            {
                PrepareBand(band, getData);

                if (band.Visible)
                {
                    AddToPreparedPages(band);
                }
            }
            finally
            {
                band.RestoreState();
            }
        }

        private void ShowBand(BandBase band, BandBase outputBand, float offsetX, float offsetY)
        {
            float saveCurX = CurX;
            float saveCurY = CurY;
            BandBase saveOutputBand = this.outputBand;
            CurX = offsetX;
            CurY = offsetY;

            try
            {
                this.outputBand = outputBand;
                ShowBand(band);
            }
            finally
            {
                this.outputBand = saveOutputBand;
                CurX = saveCurX;
                CurY = saveCurY;
            }
        }

        /// <summary>
        /// Shows band at the current position.
        /// </summary>
        /// <param name="band">Band to show.</param>
        /// <remarks>
        /// After the band is shown, the current position is advanced by the band's height.
        /// </remarks>
        public void ShowBand(BandBase band)
        {
            if (band != null)
                for (int i = 0; i < band.RepeatBandNTimes; i++)
                    ShowBand(band, true);
        }

        private void ShowBand(BandBase band, bool getData)
        {
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
                    AddToOutputBand(band, getData);
                }
                else
                {
                    ShowBandToPreparedPages(band, getData);
                }

                ProcessTotals(band);
                if (band.Visible)
                {
                    RenderOuterSubreports(band);
                }

                // show child band. Skip if child is used to fill empty space: it was processed already
                if (showChild)
                {
                    ShowBand(child);
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

        private void ProcessTotals(BandBase band)
        {
            Report.Dictionary.Totals.ProcessBand(band);
        }

        #endregion Private Methods

        #region Internal Methods

        internal bool CanPrint(ReportComponentBase obj)
        {
            // Apply visible expression if needed.
            if (!String.IsNullOrEmpty(obj.VisibleExpression))
            {
                object expression = null;
                // Calculate expressions with TotalPages only on FinalPass.
                if (!obj.VisibleExpression.Contains("TotalPages") || FinalPass)
                {
                    expression = Report.Calc(Code.CodeUtils.FixExpressionWithBrackets(obj.VisibleExpression));
                }
                if (expression != null && expression is bool)
                {
                    if (!obj.VisibleExpression.Contains("TotalPages"))
                    {
                        obj.Visible = (bool)expression;
                    }
                    else if (FirstPass)
                    {
                        obj.Visible = true;
                    }
                    else
                    {
                        obj.Visible = (bool)expression;
                    }
                }
            }

            // Apply exportable expression if needed.
            if (!String.IsNullOrEmpty(obj.ExportableExpression))
            {
                object expression = null;
                expression = Report.Calc(Code.CodeUtils.FixExpressionWithBrackets(obj.ExportableExpression));
                if (expression is bool)
                {
                    obj.Exportable = (bool)expression;
                }
            }

            // Apply printable expression if needed.
            if (!String.IsNullOrEmpty(obj.PrintableExpression))
            {
                object expression = null;
                expression = Report.Calc(Code.CodeUtils.FixExpressionWithBrackets(obj.PrintableExpression));
                if (expression is bool)
                {
                    obj.Printable = (bool)expression;
                }
            }

            if (!obj.Visible || !obj.FlagPreviewVisible)
            {
                return false;
            }

            bool isFirstPage = CurPage == firstReportPage;
            bool isLastPage = CurPage == TotalPages - 1;
            bool isRepeated = obj.Band != null && obj.Band.Repeated;
            bool canPrint = false;

            if ((obj.PrintOn & PrintOn.OddPages) > 0 && CurPage % 2 == 1)
            {
                canPrint = true;
            }
            if ((obj.PrintOn & PrintOn.EvenPages) > 0 && CurPage % 2 == 0)
            {
                canPrint = true;
            }

            if (isLastPage)
            {
                if ((obj.PrintOn & PrintOn.LastPage) == 0)
                {
                    canPrint = false;
                }
                if (obj.PrintOn == PrintOn.LastPage || obj.PrintOn == (PrintOn.LastPage | PrintOn.SinglePage) ||
                    obj.PrintOn == (PrintOn.FirstPage | PrintOn.LastPage))
                {
                    canPrint = true;
                }
            }
            if (isFirstPage)
            {
                if ((obj.PrintOn & PrintOn.FirstPage) == 0)
                {
                    canPrint = false;
                }
                if (obj.PrintOn == PrintOn.FirstPage || obj.PrintOn == (PrintOn.FirstPage | PrintOn.SinglePage) ||
                    obj.PrintOn == (PrintOn.FirstPage | PrintOn.LastPage))
                {
                    canPrint = true;
                }
            }
            if (isFirstPage && isLastPage)
            {
                canPrint = (obj.PrintOn & PrintOn.SinglePage) > 0;
            }
            if (isRepeated)
            {
                canPrint = (obj.PrintOn & PrintOn.RepeatedBand) > 0;
            }

            return canPrint;
        }

        internal void AddToPreparedPages(BandBase band)
        {
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
            if (checkFreeSpace && FreeSpace < band.Height)
            {
                // we don't have enough space. What should we do?
                // - if band can break, break it
                // - if band cannot break, check the band height:
                //   - it's the first row of a band and is bigger than page: break it immediately.
                //   - in other case, add a new page/column and tell the band that it must break next time.
                if (band.CanBreak || band.FlagMustBreak || (band.AbsRowNo == 1 && band.Height > PageHeight - PageFooterHeight))
                {
                    // since we don't show the column footer band in the EndLastPage, do it here.
                    if (isReportSummary)
                    {
                        ShowReprintFooters();
                        ShowBand(page.ColumnFooter);
                    }
                    BreakBand(band);
                    return;
                }
                else
                {
                    EndColumn();
                    band.FlagMustBreak = true;
                    AddToPreparedPages(band);
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
                    ShowBand(page.ColumnFooter);
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
                while (FreeSpace - bandHeight - band.Child.Height >= 0)
                {
                    float saveCurY = CurY;
                    ShowBand(band.Child);
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
                CurY = PageHeight - GetBandHeightWithChildren(band);
            }
            if (!isPageBand)
            {
                band.Left += originX + CurX;
            }
            if (band.PrintOnBottom)
            {
                CurY = PageHeight - PageFooterHeight - ColumnFooterHeight;
                // if PrintOnBottom is applied to a band like DataFooter, print it with all its child bands
                // if PrintOnBottom is applied to a child band, print this band only.
                if (band is ChildBand)
                {
                    CurY -= band.Height;
                }
                else
                {
                    CurY -= GetBandHeightWithChildren(band);
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

        #endregion Internal Methods
    }
}
