using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Html;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Controllers;
using FastReport.Web.Application;
using System.Drawing;

namespace FastReport.Web
{
    public enum WebReportMode
    {
        Preview,
#if DESIGNER
        Designer,
#endif
#if DIALOGS
        Dialog,
#endif
    }

    public partial class WebReport
    {
        private string localizationFile;
        private int numberNextTab = 1;

#if DIALOGS
        internal Dialog Dialog {
            get;
        }
#endif

#region Public Properties

        /// <summary>
        /// Unique ID of this instance.
        /// Automatically generates on creation.
        /// </summary>
        public string ID { get; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Adds "display: inline-*" to html container.
        /// Default value: true.
        /// </summary>
        public bool Inline { get; set; } = true;

        /// <summary>
        /// Current report
        /// </summary>
        public Report Report
        {
            get => Tabs[CurrentTabIndex].Report; 
            set => Tabs[CurrentTabIndex].Report = value;
        }

        /// <summary>
        /// Gets or sets the WebReport's locale
        /// </summary>
        public string LocalizationFile
        {
            get => localizationFile;
            set
            {
                localizationFile = value;
                string path = WebUtils.MapPath(localizationFile);
                Res.LoadLocale(path);
            }
        }

        internal IWebRes Res { get; set; } = new WebRes();


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
        /// Page index of current report
        /// </summary>
        public int CurrentPageIndex
        {
            get => CurrentTab?.CurrentPageIndex ?? 0;
            set
            {
                var tab = CurrentTab;
                if (tab != null)
                    tab.CurrentPageIndex = value;
            }
        }

        /// <summary>
        /// Is current report prepared(.fpx) or not(.frx)
        /// </summary>
        public bool ReportPrepared
        {
            get => CurrentTab?.ReportPrepared ?? false;
            set
            {
                var tab = CurrentTab;
                if (tab != null)
                    tab.ReportPrepared = value;
            }
        }

        /// <summary>
        /// Total prepared pages of current report
        /// </summary>
        public int TotalPages => Report?.PreparedPages?.Count ?? 0;

        /// <summary>
        /// List of report tabs
        /// </summary>
        public ReportTabCollection Tabs { get; } = new ReportTabCollection() { new ReportTab() { Report = new Report(), Closeable = false } };

        /// <summary>
        /// Switches beetwen Preview and Designer modes
        /// </summary>
        public WebReportMode Mode { get; set; } = WebReportMode.Preview;

        /// <summary>
        /// Property to store user data
        /// </summary>
        public object UserData { get; set; } = null;

        public bool SinglePage { get; set; } = false;
        public bool Layers { get; set; } = true;
        public bool EnableMargins { get; set; } = true;
        public string Width { get; set; } = "";
        public string Height { get; set; } = "";
        public bool Pictures { get; set; } = true;
        public bool EmbedPictures { get; set; } = false;

        #region ExportMenuSettings

        /// <summary>
        /// ExportMenu settings
        /// </summary>
        public ExportMenuSettings ExportMenu{ get; set; } = ExportMenuSettings.Default;

        #endregion

        #region ToolbarSettings

        /// <summary>
        /// Toolbar settings
        /// </summary>
        public ToolbarSettings Toolbar { get; set; } = ToolbarSettings.Default; 


        [Obsolete("Please, use Toolbar.Show")]
        public bool ShowToolbar { get => Toolbar.Show; set => Toolbar.Show = value; }
        [Obsolete("Please, use Toolbar.ShowPrevButton")]
        public bool ShowPrevButton { get => Toolbar.ShowPrevButton; set => Toolbar.ShowPrevButton = value; }
        [Obsolete("Please, use Toolbar.ShowNextButton")]
        public bool ShowNextButton { get => Toolbar.ShowNextButton; set => Toolbar.ShowNextButton = value; }
        [Obsolete("Please, use Toolbar.ShowFirstButton")]
        public bool ShowFirstButton { get => Toolbar.ShowFirstButton; set => Toolbar.ShowFirstButton = value; }
        [Obsolete("Please, use Toolbar.ShowLastButton")]
        public bool ShowLastButton { get => Toolbar.ShowLastButton; set => Toolbar.ShowLastButton = value; }

        [Obsolete("Please, use Toolbar.Exports.Show")]
        public bool ShowExports { get => Toolbar.Exports.Show; set => Toolbar.Exports.Show = value; }

        [Obsolete("Please, use Toolbar.ShowRefreshButton")]
        public bool ShowRefreshButton { get => Toolbar.ShowRefreshButton; set => Toolbar.ShowRefreshButton = value; }
        [Obsolete("Please, use Toolbar.ShowZoomButton")]
        public bool ShowZoomButton { get => Toolbar.ShowZoomButton; set => Toolbar.ShowZoomButton = value; }

        [Obsolete("Please, use Toolbar.ShowPrint")]
        public bool ShowPrint { get => Toolbar.ShowPrint; set => Toolbar.ShowPrint = value; }
        [Obsolete("Please, use Toolbar.PrintInHtml")]
        public bool PrintInHtml { get => Toolbar.PrintInHtml; set => Toolbar.PrintInHtml = value; }
#if !OPENSOURCE
        [Obsolete("Please, use Toolbar.PrintInPdf")]
        public bool PrintInPdf { get => Toolbar.PrintInPdf; set => Toolbar.PrintInPdf = value; }
#endif

        [Obsolete("Please, use Toolbar.Position")]
        public bool ShowBottomToolbar { get => Toolbar.ShowBottomToolbar; set => Toolbar.ShowBottomToolbar = value; }

        [Obsolete("Please, use Toolbar.Color")]
        public Color ToolbarColor { get => Toolbar.Color; set => Toolbar.Color = value; }

        #endregion

        public float Zoom { get; set; } = 1.0f;
        public bool Debug { get; set; }
#if DEBUG
            = true;
#else
            = false;
#endif
        internal bool Canceled { get; set; } = false;

        // for Tabs max-width
        internal int ReportMaxWidth { get; set; } = 800;

        /// <summary>
        /// Shows sidebar with outline.
        /// Default value: true.
        /// </summary>
        public bool Outline { get; set; } = true;

#endregion

#region Non-public

        // TODO
        private string ReportFile { get; set; } = null;
        private string ReportPath { get; set; } = null;
        private string ReportResourceString { get; set; } = null;

        /// <summary>
        /// Toolbar height in pixels
        /// </summary>
        [Obsolete("Please, use Toolbar.Height")]
        public int ToolbarHeight { get => Toolbar.Height; set => Toolbar.Height = value; }

        internal readonly Dictionary<string, byte[]> PictureCache = new Dictionary<string, byte[]>();
        int currentTabIndex;
        
#endregion

        public WebReport()
        {
            string path = WebUtils.MapPath(LocalizationFile);
            Res.LoadLocale(path);
            WebReportCache.Instance.Add(this);
#if DIALOGS
            Dialog = new Dialog(this);
#endif
        }

        static WebReport()
        {
            ScriptSecurity = new ScriptSecurity(new ScriptChecker());
        }


        public HtmlString RenderSync()
        {
            return Task.Run(() => Render()).Result;
        }

        public async Task<HtmlString> Render()
        {
            if (Report == null)
                throw new Exception("Report is null");

            return Render(false);
        }

        public void LoadPrepared(string filename)
        {
            Report.LoadPrepared(filename);
            ReportPrepared = true;
        }

        public void LoadPrepared(Stream stream)
        {
            Report.LoadPrepared(stream);
            ReportPrepared = true;
        }

        internal HtmlString Render(bool renderBody)
        {
            switch (Mode)
            {
#if DIALOGS
                case WebReportMode.Dialog:
#endif
                case WebReportMode.Preview:
                    return new HtmlString(template_render(renderBody));
#if DESIGNER
                case WebReportMode.Designer:
                    return RenderDesigner();
#endif
                default:
                    throw new Exception($"Unknown mode: {Mode}");
            }
        }
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
                            Report = report,
                            ReportPrepared = true//,
                        });
                    }
                }
            }
        }


        /// <summary>
        /// Force report to be removed from internal cache
        /// </summary>
        public void RemoveFromCache()
        {
            WebReportCache.Instance.Remove(this);
        }

        // TODO
        // void ReportLoad()
        // void RegisterData()

#region Navigation

        /// <summary>
        /// Force go to next report page
        /// </summary>
        public void NextPage()
        {
            if (CurrentPageIndex >= TotalPages - 1)
                return;

            var curPageIndex = CurrentPageIndex + 1;
            if (SplitReportPagesInTabs && Tabs.Count > 1)
            {
                if (curPageIndex == Tabs[numberNextTab].MinPageIndex)
                {
                    CurrentTabIndex++;

                    if (numberNextTab < Tabs.Count - 1)
                    {
                        numberNextTab++;
                    }
                }
            }

            CurrentPageIndex = curPageIndex;
        }

        /// <summary>
        /// Force go to previous report page
        /// </summary>
        public void PrevPage()
        {
            if (CurrentPageIndex <= 0)
                return;

            var curPageIndex = CurrentPageIndex - 1;
            if (SplitReportPagesInTabs && Tabs.Count > 1)
            {
                if (CurrentTab.MinPageIndex > curPageIndex)
                {
                    if (numberNextTab != 1 && CurrentTabIndex != numberNextTab)
                    {
                        numberNextTab--;
                    }

                    CurrentTabIndex--;
                }
            }

            CurrentPageIndex = curPageIndex;
        }

        /// <summary>
        /// Force go to first report page
        /// </summary>
        public void FirstPage()
        {
            if (SplitReportPagesInTabs && Tabs.Count > 1)
            {
                numberNextTab = 1;
                CurrentTabIndex = 0;
            }

            CurrentPageIndex = 0;
        }

        /// <summary>
        /// Force go to last report page
        /// </summary>
        public void LastPage()
        {
            if (SplitReportPagesInTabs && Tabs.Count > 1)
            {
                numberNextTab = Tabs.Count - 1;
                CurrentTabIndex = Tabs.Count - 1;
            }

            CurrentPageIndex = TotalPages - 1;
        }

        /// <summary>
        /// Force go to "value" report page
        /// </summary>
        public void GotoPage(int value)
        {
            if (value < 0 || value >= TotalPages)
                return;

            if (SplitReportPagesInTabs && Tabs.Count > 1)
            {
                for(int i = 0; i < Tabs.Count; i++)
                {
                    // can be better
                    if (Tabs[i].MinPageIndex <= value)
                    {
                        CurrentTabIndex = i;
                        if (i != Tabs.Count - 1)
                            numberNextTab = i + 1;
                    }
                    else break;
                }
            }

            CurrentPageIndex = value;
        }

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

        #region Script Security

        private static ScriptSecurity ScriptSecurity = null;

#endregion
    }
}
