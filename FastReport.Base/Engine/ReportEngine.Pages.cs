using FastReport.Utils;
using System;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Fields

        private ReportPage page;
        private float columnStartY;
        private string pageNameForRecalc;

        #endregion Fields

        #region Private Methods

        private DataBand FindDeepmostDataBand(ReportPage page)
        {
            DataBand result = null;
            foreach (Base c in page.AllObjects)
            {
                if (c is DataBand)
                    result = c as DataBand;
            }
            return result;
        }

        private void RunReportPage(ReportPage page)
        {
            this.page = page;
            InitReprint();
            pageNameForRecalc = null;
            this.page.OnStartPage(EventArgs.Empty);
            bool previousPage = StartFirstPage();
            OnStateChanged(this.page, EngineState.ReportPageStarted);
            OnStateChanged(this.page, EngineState.PageStarted);

            DataBand keepSummaryBand = FindDeepmostDataBand(page);
            if (keepSummaryBand != null)
                keepSummaryBand.KeepSummary = true;

            if (this.page.IsManualBuild)
                this.page.OnManualBuild(EventArgs.Empty);
            else
                RunBands(page.Bands);

            OnStateChanged(this.page, EngineState.PageFinished);
            OnStateChanged(this.page, EngineState.ReportPageFinished);
            EndLastPage();
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

        private void RunReportPages()
        {
#if TIMETRIAL
      if (new DateTime($YEAR, $MONTH, $DAY) < SystemFake.DateTime.Now)
        throw new Exception("The trial version is now expired!");
#endif

            for (int i = 0; i < Report.Pages.Count; i++)
            {
                ReportPage page = Report.Pages[i] as ReportPage;
                if (page != null && page.Visible && page.Subreport == null)
                    RunReportPage(page);
                if (Report.Aborted)
                    break;
            }
        }

        private void RunBands(BandCollection bands)
        {
            for (int i = 0; i < bands.Count; i++)
            {
                BandBase band = bands[i];
                if (band is DataBand)
                    RunDataBand(band as DataBand);
                else if (band is GroupHeaderBand)
                    RunGroup(band as GroupHeaderBand);
                if (Report.Aborted)
                    break;
            }
        }

        private void ShowPageHeader()
        {
            ShowBand(page.PageHeader);
        }

        private void ShowPageFooter(bool startPage)
        {
            if (!FirstPass &&
                CurPage == TotalPages - 1 &&
                page.PageFooter != null &&
                (page.PageFooter.PrintOn & PrintOn.LastPage) > 0 &&
                (page.PageFooter.PrintOn & PrintOn.FirstPage) == 0 &&
                startPage)
            {
                ShiftLastPage();
            }
            else
                ShowBand(page.PageFooter);
        }

        private bool StartFirstPage()
        {
            page.InitializeComponents();

            CurX = 0;
            CurY = 0;
            curColumn = 0;

            if (page.ResetPageNumber)
                ResetLogicalPageNumber();

            bool previousPage = page.PrintOnPreviousPage && PreparedPages.Count > 0;
            // check that previous page has the same size
            if (previousPage)
            {
                using (ReportPage page0 = PreparedPages.GetPage(PreparedPages.Count - 1))
                {
                    if (page0.PaperWidth == this.page.PaperWidth)
                    {
                        if (page0.UnlimitedWidth == this.page.UnlimitedWidth)
                        {
                            previousPage = true;
                            if (this.page.UnlimitedWidth)
                                pageNameForRecalc = page0.Name;
                        }
                        else
                        {
                            previousPage = false;
                        }
                    }
                    else if (page0.UnlimitedWidth && this.page.UnlimitedWidth)
                    {
                        previousPage = true;
                        pageNameForRecalc = page0.Name;
                    }
                    else
                    {
                        previousPage = false;
                    }
                    if (previousPage)
                    {
                        if (page0.PaperHeight == this.page.PaperHeight)
                        {
                            if (page0.UnlimitedHeight == this.page.UnlimitedHeight)
                            {
                                previousPage = true;
                                if (this.page.UnlimitedHeight)
                                    pageNameForRecalc = page0.Name;
                            }
                            else
                            {
                                previousPage = false;
                            }
                        }
                        else if (page0.UnlimitedHeight && this.page.UnlimitedHeight)
                        {
                            previousPage = true;
                        }
                        else
                        {
                            previousPage = false;
                        }
                    }
                }
            }

            // update CurY or add new page
            if (previousPage)
                CurY = PreparedPages.GetLastY();
            else
            {
                PreparedPages.AddPage(page);
                if (page.StartOnOddPage && (CurPage % 2) == 1)
                    PreparedPages.AddPage(page);
            }

            // page numbers
            if (isFirstReportPage)
                firstReportPage = CurPage;
            if (isFirstReportPage && previousPage)
                IncLogicalPageNumber();
            isFirstReportPage = false;

            OutlineRoot();
            AddPageOutline();

            // show report title and page header
            if (previousPage)
                ShowBand(page.ReportTitle);
            else
            {
                if (page.Overlay != null)
                    ShowBand(page.Overlay);
                if (page.TitleBeforeHeader)
                {
                    ShowBand(page.ReportTitle);
                    ShowPageHeader();
                }
                else
                {
                    ShowPageHeader();
                    ShowBand(page.ReportTitle);
                }
            }

            // show column header
            columnStartY = CurY;
            ShowBand(page.ColumnHeader);

            // calculate CurX before starting column event depending on Right to Left or Left to Right layout
            if (Config.RightToLeft)
            {
                CurX = page.Columns.Positions[page.Columns.Positions.Count - 1] * Units.Millimeters;
            }
            else
            {
                CurX = 0;
            }

            // start column event
            OnStateChanged(page, EngineState.ColumnStarted);
            ShowProgress();
            return previousPage;
        }

        private void EndLastPage()
        {
            // end column event
            OnStateChanged(page, EngineState.ColumnFinished);

            if (page.ReportSummary != null)
            {
                // do not show column footer here! It's a special case and is handled in the ShowBand.
                ShowBand(page.ReportSummary);
            }
            else
            {
                ShowBand(page.ColumnFooter);
            }

            ShowPageFooter(false);
            OutlineRoot();
            page.FinalizeComponents();
        }

        internal void EndColumn()
        {
            EndColumn(true);
        }

        private void EndColumn(bool showColumnFooter)
        {
            // end column event
            OnStateChanged(page, EngineState.ColumnFinished);

            // check keep
            if (keeping)
                CutObjects();

            ShowReprintFooters();

            if (showColumnFooter)
                ShowBand(page.ColumnFooter);

            curColumn++;

            if (curColumn >= page.Columns.Count)
                curColumn = 0;

            // apply Right to Left layot if needed
            if (Config.RightToLeft)
            {
                curX = page.Columns.Positions[page.Columns.Count - curColumn - 1] * Units.Millimeters;
            }
            else
            {
                curX = curColumn == 0 ? 0 : page.Columns.Positions[curColumn] * Units.Millimeters;
            }

            if (CurColumn == 0)
            {
                EndPage();
            }
            else
            {
                StartColumn();
            }

            // end keep
            if (keeping)
                PasteObjects();
        }

        private void StartColumn()
        {
            curY = columnStartY;

            ShowBand(page.ColumnHeader);
            ShowReprintHeaders();

            // start column event
            OnStateChanged(page, EngineState.ColumnStarted);
        }

        private void EndPage()
        {
            EndPage(true);
        }

        private void StartPage()
        {
            // apply Right to Left layout if needed 
            if (Config.RightToLeft)
            {
                CurX = page.Columns.Positions[page.Columns.Positions.Count - 1] * Units.Millimeters;
            }
            else
            {
                CurX = 0;
            }

            CurY = 0;
            curColumn = 0;

            PreparedPages.AddPage(page);
            AddPageOutline();

            if (page.Overlay != null)
                ShowBand(page.Overlay);
            ShowPageHeader();
            OnStateChanged(page, EngineState.PageStarted);

            columnStartY = CurY;

            StartColumn();
            ShowProgress();
        }


        #endregion Private Methods

        #region Internal Methods

        internal void EndPage(bool startPage)
        {
            OnStateChanged(page, EngineState.PageFinished);
            ShowPageFooter(startPage);

            if (pagesLimit > 0 && PreparedPages.Count >= pagesLimit)
                Report.Abort();
            if (Report.MaxPages > 0 && PreparedPages.Count >= Report.MaxPages)
                Report.Abort();
            if (startPage)
                StartPage();
        }

        #endregion Internal Methods

        #region Public Methods

        /// <summary>
        /// Starts a new page.
        /// </summary>
        public void StartNewPage()
        {
            EndPage();
        }

        /// <summary>
        /// Starts a new column.
        /// </summary>
        public void StartNewColumn()
        {
            EndColumn();
        }

        #endregion Public Methods
    }
}