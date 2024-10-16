using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Application;
using System.Drawing;

namespace FastReport.Web
{
    public partial class WebReport
    {
        private int numberNextTab = 1;
        private int currentTabIndex;


        #region Public Properties

        /// <summary>
        /// Active report tab
        /// </summary>
        public ReportTab CurrentTab
        {
            get => Tabs[CurrentTabIndex];
            set => Tabs[CurrentTabIndex] = value;
        }

        /// <summary>
        /// Active tab index
        /// </summary>
        public int CurrentTabIndex
        {
            get
            {
                if (currentTabIndex >= Tabs.Count)
                    currentTabIndex = Tabs.Count - 1;
                if (currentTabIndex < 0)
                    currentTabIndex = 0;
                return currentTabIndex;
            }
            set => currentTabIndex = value;
        }

        /// <summary>
        /// Shows different ReportPage in tabs.
        /// Default value: false.
        /// </summary>
        public bool SplitReportPagesInTabs { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the report name is displayed in the tab.
        /// If set to <c>true</c>, the tab will display the report name. If <c>false</c>, it will display report parameters.
        /// Default value: false.
        /// </summary>
        public bool ShowReportNameInTab { get; set; } = false;

        /// <summary>
        /// List of report tabs
        /// </summary>
        public ReportTabCollection Tabs { get; } = new ReportTabCollection() { new ReportTab() { Report = new Report(), Closeable = false } };


        // for Tabs max-width
        internal int ReportMaxWidth { get; set; } = 800;

#endregion


        /// <summary>
        /// Add report pages in tabs after load report
        /// </summary>
        internal void SplitReportPagesByTabs()
        {
            if (SplitReportPagesInTabs)
            {
                var report = Report;
          
                for (int pageN = 0; pageN < report.Pages.Count; pageN++)
                {
                    var page = report.Pages[pageN];

                    if (page is ReportPage reportPage)
                    {
                        if (pageN == 0)
                        {
                            Tabs[0].Name = reportPage.Name;
                            Tabs[0].MinPageIndex = 0;
                            
                            continue;
                        }
                      
                        if (!reportPage.Visible)
                            continue;
                        int numberPage = 0;
                        for (int i = 0; i < report.PreparedPages.Count; i++)
                        {

                            var preparedPage = report.PreparedPages.GetPage(i);
                            if (preparedPage.OriginalComponent.Name == reportPage.Name)
                            {
                                numberPage = i;
                                break;
                            }
                        }
                      
                        Tabs.Add(new ReportTab()
                        {
                            Closeable = false,
                            CurrentPageIndex = numberPage,
                            MinPageIndex = numberPage,
                            Name = reportPage.Name,
                            NeedParent = false,
                            Report = report//,
                        });
                    }
                }
            }
        }

        internal string GetCurrentTabName()
        {
            if (SplitReportPagesInTabs)
                return Report.GetReportName;

            return GetTabName(CurrentTabIndex);
        }

        internal string GetTabName(int i)
        {
            if (string.IsNullOrEmpty(Tabs[i].Name) || ShowReportNameInTab)
            {
                var s = Tabs[i].Report.ReportInfo.Name;
                if (string.IsNullOrEmpty(s))
                    s = Path.GetFileNameWithoutExtension(Tabs[i].Report.FileName);
                if (string.IsNullOrEmpty(s))
                    s = (i + 1).ToString();
                return s;
            }

            return Tabs[i].Name;
        }


        #region Navigation

        internal void SetTab(int value)
        {
            CurrentTabIndex = value;
            if (CurrentTabIndex < Tabs.Count - 1)
                numberNextTab = value + 1;
            else
                numberNextTab = value;

            //CurrentPageIndex = CurrentTab.MinPageIndex;
        }

        #endregion

    }
}
