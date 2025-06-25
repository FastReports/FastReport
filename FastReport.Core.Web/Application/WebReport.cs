using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Application;
using System.Drawing;
using System.ComponentModel;
#if !WASM
using FastReport.Web.Cache;
#endif

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
#if DIALOGS
        internal Dialog Dialog { get; }
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

#if WASM
        [Obsolete("Doesn't support in Wasm. Please, use SetLocalization(Stream) instead", true)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string LocalizationFile { get; set; }
#endif


        internal IWebRes Res { get; } = new WebRes();


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
        /// Switches between Preview and Designer modes
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

        /// <summary>
        /// Toolbar settings
        /// </summary>
        public ToolbarSettings Toolbar { get; set; } = ToolbarSettings.Default; 


        [Obsolete("Please, use Toolbar.Show")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowToolbar { get => Toolbar.Show; set => Toolbar.Show = value; }
        [Obsolete("Please, use Toolbar.ShowPrevButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowPrevButton { get => Toolbar.ShowPrevButton; set => Toolbar.ShowPrevButton = value; }
        [Obsolete("Please, use Toolbar.ShowNextButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowNextButton { get => Toolbar.ShowNextButton; set => Toolbar.ShowNextButton = value; }
        [Obsolete("Please, use Toolbar.ShowFirstButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowFirstButton { get => Toolbar.ShowFirstButton; set => Toolbar.ShowFirstButton = value; }
        [Obsolete("Please, use Toolbar.ShowLastButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowLastButton { get => Toolbar.ShowLastButton; set => Toolbar.ShowLastButton = value; }

        [Obsolete("Please, use Toolbar.Exports.Show")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowExports { get => Toolbar.Exports.Show; set => Toolbar.Exports.Show = value; }

        [Obsolete("Please, use Toolbar.ShowRefreshButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowRefreshButton { get => Toolbar.ShowRefreshButton; set => Toolbar.ShowRefreshButton = value; }
        [Obsolete("Please, use Toolbar.ShowZoomButton")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowZoomButton { get => Toolbar.ShowZoomButton; set => Toolbar.ShowZoomButton = value; }

        [Obsolete("Please, use Toolbar.ShowPrint")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowPrint { get => Toolbar.ShowPrint; set => Toolbar.ShowPrint = value; }
        [Obsolete("Please, use Toolbar.PrintInHtml")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool PrintInHtml { get => Toolbar.PrintInHtml; set => Toolbar.PrintInHtml = value; }
#if !OPENSOURCE
        [Obsolete("Please, use Toolbar.PrintInPdf")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool PrintInPdf { get => Toolbar.PrintInPdf; set => Toolbar.PrintInPdf = value; }
#endif

        [Obsolete("Please, use Toolbar.Position")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool ShowBottomToolbar { get => Toolbar.ShowBottomToolbar; set => Toolbar.ShowBottomToolbar = value; }

        [Obsolete("Please, use Toolbar.Color")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Color ToolbarColor { get => Toolbar.Color; set => Toolbar.Color = value; }

        /// <summary>
        /// Toolbar height in pixels
        /// </summary>
        [Obsolete("Please, use Toolbar.Height")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ToolbarHeight { get => Toolbar.Height; set => Toolbar.Height = value; }

        #endregion

        public float Zoom { get; set; } = 1.0f;
        public bool Debug { get; set; }
#if DEBUG
            = true;
#else
            = false;
#endif
        internal bool Canceled { get; set; } = false;

        /// <summary>
        /// Shows sidebar with outline.
        /// Default value: true.
        /// </summary>
        public bool Outline { get; set; } = true;

#endregion

#region Non-public

        internal readonly Dictionary<string, byte[]> PictureCache = new Dictionary<string, byte[]>();

        internal string InlineStyle
        {
            get
            {
                if (Inline)
                    return "inline-flex";
                return "flex";
            }
        }

        #endregion

        public WebReport()
        {
#if !WASM
            string path = WebUtils.MapPath(LocalizationFile);
            Res.LoadLocale(path);
#endif
#if DIALOGS
            Dialog = new Dialog(this);
#endif
        }

        static WebReport()
        {
            ScriptSecurity = new ScriptSecurity(new ScriptChecker());
        }

        /// <summary>
        /// Sets WebReport localization using <see cref="Stream"/>
        /// </summary>
        /// <param name="stream">Stream with localization in `*.frl` format</param>
        public void SetLocalization(Stream stream)
        {
            Res.LoadLocale(stream);
        }

        public void LoadPrepared(string filename)
        {
            Report.LoadPrepared(filename);
        }

        public void LoadPrepared(Stream stream)
        {
            Report.LoadPrepared(stream);
        }


        internal void InternalDispose()
        {
            //foreach(var picture in PictureCache.Values)
            //{
            //    ArrayPool<byte>.Shared.Return(picture);
            //}
            PictureCache.Clear();

            foreach(var tab in Tabs)
            {
                tab.Report.Dispose();
            }
            Res.Dispose();
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
#if !OPENSOURCE
        /// <summary>
        /// Forced transition to the report page containing the required text.
        /// </summary>
        public bool ReportSearch(string searchText, bool backward, bool matchCase, bool wholeWord)
        {
            if (string.IsNullOrEmpty(searchText))
                return true;

            int indexPage = CurrentPageIndex;
            if (backward)
                indexPage--;
            else
                indexPage++;

            for (; indexPage < TotalPages && indexPage >= 0;)
            {
                ReportPage page = CurrentTab.Report.PreparedPages.GetPage(indexPage);
                ObjectCollection pageObjects = page.AllObjects;
                for (int objNo = 0; objNo < pageObjects.Count; objNo++)
                {
                    ISearchable obj = pageObjects[objNo] as ISearchable;
                    if (obj != null)
                    {
                        CharacterRange[] ranges = obj.SearchText(searchText, matchCase, wholeWord);
                        if (ranges != null)
                        {
                            GotoPage(indexPage);
                            return true;
                        }
                    }
                }
                if (backward)
                    indexPage--;
                else
                    indexPage++;
            }
            return false;
        }
#endif
#endregion

        #region Script Security

        private static ScriptSecurity ScriptSecurity = null;

#endregion
    }
}
