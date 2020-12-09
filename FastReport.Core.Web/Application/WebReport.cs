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
        Designer,
        Dialog
    }
    
    public partial class WebReport
    {
        private string localizationFile;

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

        internal WebRes Res { get; set; } = new WebRes();


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


        #region ToolbarSettings
        public bool ShowToolbar { get; set; } = true;
        public bool ShowPrevButton { get; set; } = true;
        public bool ShowNextButton { get; set; } = true;
        public bool ShowFirstButton { get; set; } = true;
        public bool ShowLastButton { get; set; } = true;
        public bool ShowExports { get; set; } = true;
        public bool ShowRefreshButton { get; set; } = true;
        public bool ShowZoomButton { get; set; } = true;

        public bool ShowPrint { get; set; } = true;
        public bool PrintInHtml { get; set; } = true;
#if !OPENSOURCE
        public bool PrintInPdf { get; set; } = true;
#endif

        public bool ShowBottomToolbar { get; set; } = false;

        public Color ToolbarColor { get; set; } = Color.LightGray;

        #endregion
        public float Zoom { get; set; } = 1.0f;
        public bool Debug { get; set; } = false;
        internal bool Canceled { get; set; } = false;

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
        private int ToolbarHeight { get; set; } = 40;

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
                case WebReportMode.Preview:
                case WebReportMode.Dialog:
                    return new HtmlString(template_render(renderBody));
                case WebReportMode.Designer:
                    return RenderDesigner();
                default:
                    throw new Exception($"Unknown mode: {Mode}");
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
            if (CurrentPageIndex < TotalPages - 1)
                CurrentPageIndex++;
        }

        /// <summary>
        /// Force go to previous report page
        /// </summary>
        public void PrevPage()
        {
            if (CurrentPageIndex > 0)
                CurrentPageIndex--;
        }

        /// <summary>
        /// Force go to first report page
        /// </summary>
        public void FirstPage()
        {
            CurrentPageIndex = 0;
        }

        /// <summary>
        /// Force go to last report page
        /// </summary>
        public void LastPage()
        {
            CurrentPageIndex = TotalPages - 1;
        }

        /// <summary>
        /// Force go to "value" report page
        /// </summary>
        public void GotoPage(int value)
        {
            if (value >= 0 && value < TotalPages)
                CurrentPageIndex = value;
        }

#endregion

#region Script Security

        private static ScriptSecurity ScriptSecurity = null;

#endregion
    }
}
