using FastReport.Data;
using FastReport.Preview;
using FastReport.Utils;
using System;
using System.Collections.Generic;

namespace FastReport.Engine
{
    /// <summary>
    /// Represents the report engine.
    /// </summary>
    public partial class ReportEngine
    {
        #region Fields

        private Report report;
        private float curX;
        private float originX;
        private float curY;
        private int curColumn;
        private BandBase curBand;
        private DateTime date;
        private int hierarchyLevel;
        private float hierarchyIndent;
        private string hierarchyRowNo;
        private bool finalPass;
        private int firstReportPage;
        private bool isFirstReportPage;
        private int pagesLimit = 0;

        #endregion Fields

        #region Properties

        private Report Report
        {
            get { return report; }
        }

        private PreparedPages PreparedPages
        {
            get { return report.PreparedPages; }
        }

        /// <summary>
        /// Gets or sets the current X offset.
        /// </summary>
        /// <remarks>
        /// This property specifies the X offset where the current band will be printed.
        /// </remarks>
        public float CurX
        {
            get { return curX; }
            set { curX = Converter.DecreasePrecision(value, 2); }
        }

        /// <summary>
        /// Gets or sets the current Y offset.
        /// </summary>
        /// <remarks>
        /// This property specifies the Y offset where the current band will be printed.
        /// After the band is printed, this value is incremented by the band's height.
        /// </remarks>
        public float CurY
        {
            get { return curY; }
            set { curY = Converter.DecreasePrecision(value, 2); }
        }

        /// <summary>
        /// Gets the index of currently printing column in the multi-column report.
        /// </summary>
        /// <remarks>
        /// This value is 0-based.
        /// </remarks>
        public int CurColumn
        {
            get { return curColumn; }
        }

        /// <summary>
        /// Gets or sets index of current prepared page the current band will print on.
        /// </summary>
        /// <remarks>
        /// Note: the page with specified index must exists. This property is used to print side-by-side
        /// subreports and Table object. Usually you don't need to use it.
        /// </remarks>
        public int CurPage
        {
            get { return PreparedPages.CurPage; }
            set { PreparedPages.CurPage = value; }
        }

        /// <summary>
        /// Gets the current page width, in pixels.
        /// </summary>
        /// <remarks>
        /// This property returns a paper width minus left and right margins.
        /// </remarks>
        public float PageWidth
        {
            get { return page.WidthInPixels - (page.LeftMargin + page.RightMargin) * Units.Millimeters; }
        }

        /// <summary>
        /// Gets the current page height, in pixels.
        /// </summary>
        /// <remarks>
        /// This property returns a paper height minus top and bottom margins.
        /// </remarks>
        public float PageHeight
        {
            get { return page.HeightInPixels - (page.TopMargin + page.BottomMargin) * Units.Millimeters; }
        }

        /// <summary>
        /// Gets the value indicating whether the page has unlimited height.
        /// </summary>
        public bool UnlimitedHeight
        {
            get { return page.UnlimitedHeight; }
        }

        /// <summary>
        /// Gets the value indicating whether the page has unlimited width.
        /// </summary>
        public bool UnlimitedWidth
        {
            get { return page.UnlimitedWidth; }
        }

        /// <summary>
        /// Gets or sets the current height of unlimited page.
        /// </summary>
        public float UnlimitedHeightValue
        {
            get { return page.UnlimitedHeightValue; }
            set { page.UnlimitedHeightValue = value; }
        }

        /// <summary>
        /// Gets or sets the current width of unlimited page.
        /// </summary>
        public float UnlimitedWidthValue
        {
            get { return page.UnlimitedWidthValue; }
            set { page.UnlimitedWidthValue = value; }
        }

        /// <summary>
        /// Gets the height of page footer (including all its child bands), in pixels.
        /// </summary>
        public float PageFooterHeight
        {
            get { return GetBandHeightWithChildren(page.PageFooter); }
        }

        /// <summary>
        /// Gets the height of column footer (including all its child bands), in pixels.
        /// </summary>
        public float ColumnFooterHeight
        {
            get { return GetBandHeightWithChildren(page.ColumnFooter); }
        }

        /// <summary>
        /// Gets the free space on the current page, in pixels.
        /// </summary>
        /// <remarks>
        /// This property returns the page height minus footers height minus <b>CurY</b> value.
        /// </remarks>
        public float FreeSpace
        {
            get
            {
                float pageHeight = PageHeight;
                pageHeight -= PageFooterHeight;
                pageHeight -= ColumnFooterHeight;
                pageHeight -= GetFootersHeight();
                return Converter.DecreasePrecision(pageHeight - CurY, 2);
            }
        }

        /// <summary>
        /// Gets the current prepared page number.
        /// </summary>
        /// <remarks>
        /// This value is 1-based. The initial value (usually 1) is set in the Report.InitialPageNumber property.
        /// </remarks>
        public int PageNo
        {
            get { return GetLogicalPageNumber(); }
        }

        /// <summary>
        /// Gets the number of total pages in a prepared report.
        /// </summary>
        /// <remarks>
        /// To use this property, your report must be two-pass. Set the <see cref="FastReport.Report.DoublePass"/>
        /// property to <b>true</b>.
        /// </remarks>
        public int TotalPages
        {
            get { return GetLogicalTotalPages(); }
        }

        /// <summary>
        /// Gets the string that represents the current page number.
        /// </summary>
        /// <remarks>
        /// This property returns a locale-based value, for example: "Page 1".
        /// </remarks>
        public string PageN
        {
            get { return String.Format(Res.Get("Misc,PageN"), PageNo); }
        }

        /// <summary>
        /// Gets the string that represents the "Page N of M" number.
        /// </summary>
        /// <remarks>
        /// This property returns a locale-based value, for example: "Page 1 of 10".
        /// </remarks>
        public string PageNofM
        {
            get { return String.Format(Res.Get("Misc,PageNofM"), PageNo, TotalPages); }
        }

        /// <summary>
        /// Gets the current row number of currently printing band.
        /// </summary>
        /// <remarks>
        /// This value is 1-based. It resets to 1 on each new group.
        /// </remarks>
        public int RowNo
        {
            get { return curBand == null ? 0 : curBand.RowNo; }
        }

        /// <summary>
        /// Gets the running current row number of currently printing band.
        /// </summary>
        /// <remarks>
        /// This value is 1-based.
        /// </remarks>
        public int AbsRowNo
        {
            get { return curBand == null ? 0 : curBand.AbsRowNo; }
        }

        /// <summary>
        /// Gets the date of report start.
        /// </summary>
        public DateTime Date
        {
            get { return date; }
        }

        /// <summary>
        /// Gets a value indicating whether the report is executing the final pass.
        /// </summary>
        /// <remarks>
        /// This property is <b>true</b> if report is one-pass, or if report is two-pass and
        /// the second pass is executing.
        /// </remarks>
        public bool FinalPass
        {
            get { return finalPass; }
        }

        /// <summary>
        /// Gets a value indicating whether the report is executing the first pass.
        /// </summary>
        /// <remarks>
        /// This property is <b>true</b> if report is one-pass, or if report is two-pass and
        /// the first pass is executing.
        /// </remarks>
        public bool FirstPass
        {
            get { return !(Report.DoublePass && FinalPass); }
        }

        /// <summary>
        /// Gets a level of hierarchy when printing hierarchical bands.
        /// </summary>
        /// <remarks>
        /// The first level of hierarchy has 0 index.
        /// </remarks>
        public int HierarchyLevel
        {
            get { return hierarchyLevel; }
        }

        /// <summary>
        /// Gets the row number like "1.2.1" when printing hierarchical bands.
        /// </summary>
        public string HierarchyRowNo
        {
            get { return hierarchyRowNo; }
        }

        #endregion Properties

        #region Constructors

        internal ReportEngine(Report report)
        {
            this.report = report;
            objectsToProcess = new List<ProcessInfo>();
        }

        #endregion Constructors

        #region Private Methods

        private void ResetDesigningFlag()
        {
            ObjectCollection allObjects = Report.AllObjects;
            foreach (Base c in allObjects)
            {
                c.SetDesigning(false);
            }
        }

        private void InitializeData()
        {
            foreach (Base c in Report.Dictionary.AllObjects)
            {
                if (c is DataComponentBase)
                    (c as DataComponentBase).InitializeComponent();
            }
        }

        private void InitializeSecondPassData()
        {
            foreach (Base c in Report.Dictionary.AllObjects)
            {
                if (c is DataSourceBase)
                {
                    DataSourceBase data = c as DataSourceBase;
                    if (data.RowCount > 0)
                        data.First();
                }
            }
        }

        private void PrepareToFirstPass(bool append)
        {
            finalPass = !Report.DoublePass;
            if (!append)
                PreparedPages.Clear();
            else
                PreparedPages.CurPage = PreparedPages.Count > 0 ? PreparedPages.Count - 1 : 0;
            isFirstReportPage = true;
            hierarchyLevel = 1;
            PreparedPages.PrepareToFirstPass();
            Report.Dictionary.Totals.ClearValues();
            objectsToProcess.Clear();
            InitializePages();
            InitPageNumbers();
        }

        private void PrepareToSecondPass()
        {
            Report.Dictionary.Totals.ClearValues();
            objectsToProcess.Clear();
            PreparedPages.ClearFirstPass();
            InitializeSecondPassData();
        }

        private float GetBandHeightWithChildren(BandBase band)
        {
            float result = 0;

            while (band != null)
            {
                if (CanPrint(band))
                    result += (band.CanGrow || band.CanShrink) ? band.CalcHeight() : band.Height;
                else if (FinalPass && !String.IsNullOrEmpty(band.VisibleExpression) && band.VisibleExpression.Contains("TotalPages"))
                    result += (band.CanGrow || band.CanShrink) ? band.CalcHeight() : band.Height;
                band = band.Child;
                if (band != null && ((band as ChildBand).FillUnusedSpace || (band as ChildBand).CompleteToNRows != 0))
                    break;
            }

            return result;
        }

        private void RunReportPages(ReportPage page)
        {
            OnStateChanged(Report, EngineState.ReportStarted);

            if (page == null)
                RunReportPages();
            else
                RunReportPage(page);

            OnStateChanged(Report, EngineState.ReportFinished);
        }

        #endregion Private Methods

        #region Internal Methods

        internal bool Run(bool runDialogs, bool append, bool resetDataState)
        {
            return Run(runDialogs, append, resetDataState, null);
        }

        internal bool Run(bool runDialogs, bool append, bool resetDataState, int pagesLimit)
        {
            this.pagesLimit = pagesLimit;
            return Run(runDialogs, append, resetDataState, null);
        }

        internal bool Run(bool runDialogs, bool append, bool resetDataState, ReportPage page)
        {
            date = SystemFake.DateTime.Now;
            Report.SetOperation(ReportOperation.Running);
            ResetDesigningFlag();

            // don't reset the data state if we run the hyperlink's detail page or refresh a report.
            // This is necessary to keep data filtering settings alive
            if (resetDataState)
                InitializeData();
            Report.OnStartReport(EventArgs.Empty);

            try
            {
                if (runDialogs && !RunDialogs())
                    return false;

                Config.ReportSettings.OnStartProgress(Report);
                PrepareToFirstPass(append);
                RunReportPages(page);

                ResetLogicalPageNumber();
                if (Report.DoublePass && !Report.Aborted)
                {
                    finalPass = true;
                    PrepareToSecondPass();
                    RunReportPages(page);
                }
            }
            finally
            {
                Report.OnFinishReport(EventArgs.Empty);
                Config.ReportSettings.OnFinishProgress(Report);
                Report.SetOperation(ReportOperation.None);

                // limit the prepared pages
                if (Report.MaxPages > 0)
                {
                    while (PreparedPages.Count > Report.MaxPages)
                    {
                        PreparedPages.RemovePage(PreparedPages.Count - 1);
                    }
                }

                // Limit the prepared pages again because pagesLimit has better priority than Report.MaxPages.
                if (pagesLimit > 0)
                {
                    while (PreparedPages.Count > pagesLimit)
                    {
                        PreparedPages.RemovePage(PreparedPages.Count - 1);
                    }
                }
            }
            return true;
        }

        internal void RunPhase1()
        {
            date = SystemFake.DateTime.Now;
            Report.SetOperation(ReportOperation.Running);
            ResetDesigningFlag();
            InitializeData();
            Report.OnStartReport(EventArgs.Empty);
        }

        internal void RunPhase2()
        {
            try
            {
                Config.ReportSettings.OnStartProgress(Report);
                PrepareToFirstPass(false);
                RunReportPages();

                ResetLogicalPageNumber();
                if (Report.DoublePass && !Report.Aborted)
                {
                    finalPass = true;
                    PrepareToSecondPass();
                    RunReportPages();
                }
            }
            finally
            {
                Report.OnFinishReport(EventArgs.Empty);
                Config.ReportSettings.OnFinishProgress(Report);
                Report.SetOperation(ReportOperation.None);

                // limit the prepared pages
                if (Report.MaxPages > 0)
                {
                    while (PreparedPages.Count > Report.MaxPages)
                    {
                        PreparedPages.RemovePage(PreparedPages.Count - 1);
                    }
                }

                // Limit the prepared pages again because pagesLimit has better priority than Report.MaxPages.
                if (pagesLimit > 0)
                {
                    while (PreparedPages.Count > pagesLimit)
                    {
                        PreparedPages.RemovePage(PreparedPages.Count - 1);
                    }
                }
            }
        }

        #endregion Internal Methods
    }
}
