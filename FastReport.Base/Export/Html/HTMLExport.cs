using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;

namespace FastReport.Export.Html
{
    /// <summary>
    /// Represents the HTML export filter.
    /// </summary>
    public partial class HTMLExport : ExportBase
    {
        /// <summary>
        /// Draw any custom controls
        /// </summary>
        public event EventHandler<CustomDrawEventArgs> CustomDraw;

        /// <summary>
        /// Draw any custom controls.
        /// </summary>
        /// <param name="e"></param>
        public void OnCustomDraw(CustomDrawEventArgs e)
        {
            if (CustomDraw != null)
            {
                CustomDraw(this, e);
            }
        }

        #region Private fields

        private struct HTMLData
        {
            public int ReportPage;
            public int PageNumber;
            public int CurrentPage;
            public ReportPage page;
            public Stream PagesStream;
        }

        private struct PicsArchiveItem
        {
            public string FileName;
            public MemoryStream Stream;
        }

        /// <summary>
        /// Types of html export
        /// </summary>
        public enum ExportType
        {
            /// <summary>
            /// Simple export
            /// </summary>
            Export,

            /// <summary>
            /// Web preview mode
            /// </summary>
            WebPreview,

            /// <summary>
            /// Web print mode
            /// </summary>
            WebPrint
        }

        private bool layers;
        private bool wysiwyg;
        private MyRes res;
        private HtmlTemplates templates;
        private string targetPath;
        private string targetIndexPath;
        private string targetFileName;
        private string fileName;
        private string navFileName;

        //private string FOutlineFileName;
        private int pagesCount;

        private string documentTitle;
        private ImageFormat imageFormat;
        private bool subFolder;
        private bool navigator;
        private bool singlePage;
        private bool pictures;
        private bool embedPictures;
        private bool webMode;
        private List<HTMLPageData> pages;
        private HTMLPageData printPageData;
        private int count;
        private string webImagePrefix;
        private string webImageSuffix;
        private string stylePrefix;
        private string prevWatermarkName;
        private long prevWatermarkSize;
        private HtmlSizeUnits widthUnits;
        private HtmlSizeUnits heightUnits;
        private string singlePageFileName;
        private string subFolderPath;
        private HTMLExportFormat format;
        private MemoryStream mimeStream;
        private String boundary;
        private List<PicsArchiveItem> picsArchive;
        private List<ExportIEMStyle> prevStyleList;
        private int prevStyleListIndex;
        private bool pageBreaks;
        private bool print;
        private bool preview;
        private List<string> cssStyles;
        private float hPos;
        private NumberFormatInfo numberFormat;
        private string pageStyleName;

        private bool saveStreams;

        private string onClickTemplate = String.Empty;
        private string reportID;

        private const string BODY_BEGIN = "</head>\r\n<body bgcolor=\"#FFFFFF\" text=\"#000000\">";
        private const string BODY_END = "</body>";
        private const string PRINT_JS = "<script language=\"javascript\" type=\"text/javascript\"> parent.focus(); parent.print();</script>";
        private const string NBSP = "&nbsp;";
        private int currentPage = 0;
        private HTMLData d;
        private Graphics htmlMeasureGraphics;
        private float maxWidth, maxHeight;
        private FastString css;
        private FastString htmlPage;
        private float leftMargin, topMargin;
        private bool enableMargins = false;
        private ExportType exportMode;
        private bool enableVectorObjects = true;

        /// <summary>
        /// hash:base64Image
        /// </summary>
        private Dictionary<string, string> embeddedImages;
        #endregion Private fields

        #region Public properties

        /// <summary>
        /// Gets or sets images, embedded in html (hash:base64Image)
        /// </summary>
        public Dictionary<string, string> EmbeddedImages
        {
            get { return embeddedImages; }
            set { embeddedImages = value; }
        }

        /// <summary>
        /// Sets a ID of report
        /// </summary>
        public string ReportID
        {
            get { return reportID; }
            set { reportID = value; }
        }

        /// <summary>
        /// Sets an onclick template
        /// </summary>
        public string OnClickTemplate
        {
            get { return onClickTemplate; }
            set { onClickTemplate = value; }
        }

        /// <summary>
        /// Enable or disable layers export mode
        /// </summary>
        public bool Layers
        {
            get { return layers; }
            set { layers = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string StylePrefix
        {
            get { return stylePrefix; }
            set { stylePrefix = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string WebImagePrefix
        {
            get { return webImagePrefix; }
            set { webImagePrefix = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public string WebImageSuffix
        {
            get { return webImageSuffix; }
            set { webImageSuffix = value; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public int Count
        {
            get { return count; }
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public List<HTMLPageData> PreparedPages
        {
            get { return pages; }
        }

        /// <summary>
        /// Enable or disable showing of print dialog in browser when html document is opened
        /// </summary>
        public bool Print
        {
            get { return print; }
            set { print = value; }
        }

        /// <summary>
        /// Enable or disable preview in Web settings
        /// </summary>
        public bool Preview
        {
            get { return preview; }
            set { preview = value; }
        }

        /// <summary>
        /// Enable or disable the breaks between pages in print preview when single page mode is enabled
        /// </summary>
        public bool PageBreaks
        {
            get { return pageBreaks; }
            set { pageBreaks = value; }
        }

        /// <summary>
        /// Specifies the output format
        /// </summary>
        public HTMLExportFormat Format
        {
            get { return format; }
            set { format = value; }
        }

        /// <summary>
        /// Specifies the width units in HTML export
        /// </summary>
        public HtmlSizeUnits WidthUnits
        {
            get { return widthUnits; }
            set { widthUnits = value; }
        }

        /// <summary>
        /// Specifies the height units in HTML export
        /// </summary>
        public HtmlSizeUnits HeightUnits
        {
            get { return heightUnits; }
            set { heightUnits = value; }
        }

        /// <summary>
        /// Enable or disable the pictures in HTML export
        /// </summary>
        public bool Pictures
        {
            get { return pictures; }
            set { pictures = value; }
        }

        /// <summary>
        /// Enable or disable embedding pictures in HTML export
        /// </summary>
        public bool EmbedPictures
        {
            get { return embedPictures; }
            set { embedPictures = value; }
        }

        /// <summary>
        /// Enable or disable the WEB mode in HTML export
        /// </summary>
        internal bool WebMode
        {
            get { return webMode; }
            set { webMode = value; }
        }

        /// <summary>
        /// Gets or sets html export mode
        /// </summary>
        public ExportType ExportMode
        {
            get { return exportMode; }
            set { exportMode = value; }
        }

        /// <summary>
        /// Enable or disable the single HTML page creation
        /// </summary>
        public bool SinglePage
        {
            get { return singlePage; }
            set { singlePage = value; }
        }

        /// <summary>
        /// Enable or disable the page navigator in html export
        /// </summary>
        public bool Navigator
        {
            get { return navigator; }
            set { navigator = value; }
        }

        /// <summary>
        /// Enable or disable the sub-folder for files of export
        /// </summary>
        public bool SubFolder
        {
            get { return subFolder; }
            set { subFolder = value; }
        }

        /// <summary>
        ///  Gets or sets the Wysiwyg quality of export
        /// </summary>
        public bool Wysiwyg
        {
            get { return wysiwyg; }
            set { wysiwyg = value; }
        }

        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        public ImageFormat ImageFormat
        {
            get { return imageFormat; }
            set { imageFormat = value; }
        }

        /// <summary>
        /// Gets print page data
        /// </summary>
        public HTMLPageData PrintPageData
        {
            get { return printPageData; }
        }

        

        /// <summary>
        /// Enable or disable saving streams in GeneratedStreams collection.
        /// </summary>
        public bool SaveStreams
        {
            get { return saveStreams; }
            set { saveStreams = value; }
        }

        /// <summary>
        /// Enable or disable margins for pages. Works only for Layers-mode.
        /// </summary>
        public bool EnableMargins
        {
            get { return enableMargins; }
            set { enableMargins = value; }
        }

        /// <summary>
        /// Enable or disable export of vector objects such as Barcodes in SVG format.
        /// </summary>
        public bool EnableVectorObjects
        {
            get { return enableVectorObjects; }
            set { enableVectorObjects = value; }
        }

        #endregion Public properties

        #region Private methods

        private void GeneratedUpdate(string filename, Stream stream)
        {
            int i = GeneratedFiles.IndexOf(filename);
            if (i == -1)
            {
                GeneratedFiles.Add(filename);
                GeneratedStreams.Add(stream);
            }
            else
            {
                GeneratedStreams[i] = stream;
            }
        }

        private void ExportHTMLPageStart(FastString Page, int PageNumber, int CurrentPage)
        {
            if (webMode)
            {
                if (!layers)
                {
                    pages[CurrentPage].CSSText = Page.ToString();
                    Page.Clear();
                }
                pages[CurrentPage].PageNumber = PageNumber;
            }

            if (!webMode && !singlePage)
            {
                Page.AppendLine(BODY_BEGIN);
            }
        }

        private void ExportHTMLPageFinal(FastString CSS, FastString Page, HTMLData d, float MaxWidth, float MaxHeight)
        {
            if (!webMode)
            {
                if (!singlePage)
                    Page.AppendLine(BODY_END);
                if (d.PagesStream == null)
                {
                    string pageFileName = targetIndexPath + targetFileName + d.PageNumber.ToString() + ".html";
                    if (saveStreams)
                    {
                        string fileName = singlePage ? singlePageFileName : pageFileName;
                        int i = GeneratedFiles.IndexOf(fileName);
                        Stream outStream = (i == -1) ? new MemoryStream() : GeneratedStreams[i];
                        DoPage(outStream, documentTitle, CSS, Page);
                        GeneratedUpdate(fileName, outStream);
                    }
                    else
                    {
                        GeneratedFiles.Add(pageFileName);
                        using (FileStream outStream = new FileStream(pageFileName, FileMode.Create))
                        {
                            DoPage(outStream, documentTitle, CSS, Page);
                        }
                    }
                }
                else
                {
                    DoPage(d.PagesStream, documentTitle, CSS, Page);
                }
            }
            else
            {
                ExportHTMLPageFinalWeb(CSS, Page, d, MaxWidth, MaxHeight);
            }
        }

        private void ExportHTMLPageFinalWeb(FastString CSS, FastString Page, HTMLData d, float MaxWidth, float MaxHeight)
        {
            CalcPageSize(pages[d.CurrentPage], MaxWidth, MaxHeight);

            if (Page != null)
            {
                pages[d.CurrentPage].PageText = Page.ToString();
                Page.Clear();
            }
            if (CSS != null)
            {
                pages[d.CurrentPage].CSSText = CSS.ToString();
                CSS.Clear();
            }
            pages[d.CurrentPage].PageEvent.Set();
        }

        private void CalcPageSize(HTMLPageData page, float MaxWidth, float MaxHeight)
        {
            if (!layers)
            {
                page.Width = MaxWidth / Zoom;
                page.Height = MaxHeight / Zoom;
            }
            else
            {
                page.Width = MaxWidth;
                page.Height = MaxHeight;
            }
        }

        private void DoPage(Stream stream, string documentTitle, FastString CSS, FastString Page)
        {
            if (!singlePage)
                ExportUtils.Write(stream, String.Format(templates.PageTemplateTitle, documentTitle));
            if (CSS != null)
            {
                ExportUtils.Write(stream, CSS.ToString());
                CSS.Clear();
            }
            if (Page != null)
            {
                ExportUtils.Write(stream, Page.ToString());
                Page.Clear();
            }
            if (!singlePage)
                ExportUtils.Write(stream, templates.PageTemplateFooter);
        }

        private void ExportHTMLOutline(Stream OutStream)
        {
            if (!webMode)
            {
                // under construction
            }
            else
            {
                // under construction
            }
        }

        private void DoPageStart(Stream stream, string documentTitle, bool print)
        {
            ExportUtils.Write(stream, String.Format(templates.PageTemplateTitle, documentTitle));
            if (print)
                ExportUtils.WriteLn(stream, PRINT_JS);
            ExportUtils.WriteLn(stream, BODY_BEGIN);
        }

        private void DoPageEnd(Stream stream)
        {
            ExportUtils.WriteLn(stream, BODY_END);
            ExportUtils.Write(stream, templates.PageTemplateFooter);
        }

        private void ExportHTMLIndex(Stream stream)
        {
            ExportUtils.Write(stream, String.Format(templates.IndexTemplate,
                new object[] { documentTitle, ExportUtils.HtmlURL(navFileName),
                        ExportUtils.HtmlURL(targetFileName +
                        (singlePage ? ".main" : "1") + ".html") }));
        }

        private void ExportHTMLNavigator(Stream stream)
        {
            string prefix = ExportUtils.HtmlURL(fileName);
            //  {0} - pages count {1} - name of report {2} multipage document {3} prefix of pages
            //  {4} first caption {5} previous caption {6} next caption {7} last caption
            //  {8} total caption
            ExportUtils.Write(stream, String.Format(templates.NavigatorTemplate,
                new object[] { pagesCount.ToString(),
                        documentTitle, (singlePage ? "0" : "1"),
                        prefix, res.Get("First"), res.Get("Prev"),
                        res.Get("Next"), res.Get("Last"), res.Get("Total") }));
        }

        private void Init()
        {
            htmlMeasureGraphics = Report.MeasureGraphics;
            cssStyles = new List<string>();
            hPos = 0;
            count = Report.PreparedPages.Count;
            pagesCount = 0;
            prevWatermarkName = String.Empty;
            prevWatermarkSize = 0;
            prevStyleList = null;
            prevStyleListIndex = 0;
            picsArchive = new List<PicsArchiveItem>();
            GeneratedStreams = new List<Stream>();
        }

        private void StartMHT()
        {
            subFolder = false;
            singlePage = true;
            navigator = false;
            mimeStream = new MemoryStream();
            boundary = ExportUtils.GetID();
        }

        private void StartWeb()
        {
            pages.Clear();
            for (int i = 0; i < count; i++)
                pages.Add(new HTMLPageData());
        }

        private void StartSaveStreams()
        {
            if (singlePage)
                GeneratedUpdate("index.html", null);
            subFolder = false;
            navigator = false;
        }

        private void FinishMHT()
        {
            DoPageEnd(mimeStream);
            WriteMHTHeader(Stream, FileName);
            WriteMimePart(mimeStream, "text/html", "utf-8", "index.html");
            for (int i = 0; i < picsArchive.Count; i++)
            {
                string imagename = picsArchive[i].FileName;
                WriteMimePart(picsArchive[i].Stream, "image/" + imagename.Substring(imagename.LastIndexOf('.') + 1), "utf-8", imagename);
            }
            string last = "--" + boundary + "--";
            Stream.Write(Encoding.ASCII.GetBytes(last), 0, last.Length);
        }

        private void FinishSaveStreams()
        {
            // do append in memory stream
            int fileIndex = GeneratedFiles.IndexOf(singlePageFileName);
            ExportHTMLIndex(GeneratedStreams[fileIndex]);
            MemoryStream outStream = new MemoryStream();
            ExportHTMLNavigator(outStream);
            GeneratedUpdate(targetIndexPath + navFileName, outStream);
        }

        #endregion Private methods

        #region Protected methods

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            if (Format == HTMLExportFormat.HTML)
                return new MyRes("FileFilters").Get("HtmlFile");
            else
                return new MyRes("FileFilters").Get("MhtFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            Init();

            if (saveStreams)
            {
                StartSaveStreams();
            }

            if (!webMode)
            {
                if (format == HTMLExportFormat.MessageHTML)
                {
                    StartMHT();
                }

                if (FileName == "" && Stream != null)
                {
                    targetFileName = "html";
                    singlePage = true;
                    subFolder = false;
                    navigator = false;
                    if (format == HTMLExportFormat.HTML && !embedPictures)
                        pictures = false;
                }
                else
                {
                    targetFileName = Path.GetFileNameWithoutExtension(FileName);
                    fileName = targetFileName;
                    targetIndexPath = !String.IsNullOrEmpty(FileName) ? Path.GetDirectoryName(FileName) : FileName;
                }

                if (!String.IsNullOrEmpty(targetIndexPath))
                    targetIndexPath += Path.DirectorySeparatorChar;

                if (preview)
                {
                    pictures = true;
                    printPageData = new HTMLPageData();
                }
                else if (subFolder)
                {
                    subFolderPath = targetFileName + ".files" + Path.DirectorySeparatorChar;
                    targetPath = targetIndexPath + subFolderPath;
                    targetFileName = subFolderPath + targetFileName;
                    if (!Directory.Exists(targetPath))
                        Directory.CreateDirectory(targetPath);
                }
                else
                    targetPath = targetIndexPath;

                navFileName = targetFileName + ".nav.html";
                //FOutlineFileName = FTargetFileName + ".outline.html";
                documentTitle = (!String.IsNullOrEmpty(Report.ReportInfo.Name) ?
                    Report.ReportInfo.Name : Path.GetFileNameWithoutExtension(FileName));

                if (singlePage)
                {
                    if (navigator)
                    {
                        singlePageFileName = targetIndexPath + targetFileName + ".main.html";
                        if (saveStreams)
                        {
                            MemoryStream pageStream = new MemoryStream();
                            DoPageStart(pageStream, documentTitle, print);
                            GeneratedUpdate(singlePageFileName, pageStream);
                        }
                        else
                        {
                            using (Stream pageStream = new FileStream(singlePageFileName,
                                FileMode.Create))
                            {
                                DoPageStart(pageStream, documentTitle, print);
                            }
                        }
                    }
                    else
                    {
                        singlePageFileName = String.IsNullOrEmpty(FileName) ? "index.html" : FileName;
                        if (saveStreams)
                        {
                            GeneratedUpdate(singlePageFileName, new MemoryStream());
                        }
                        DoPageStart((format == HTMLExportFormat.HTML) ? Stream : mimeStream, documentTitle, print);
                    }
                }
            }
            else
            {
                StartWeb();
            }
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            if (ExportMode == ExportType.Export)
                base.ExportPageBegin(page);
            pagesCount++;
            if (!WebMode)
            {
                if (singlePage)
                {
                    d = new HTMLData();
                    d.page = page;
                    d.ReportPage = pagesCount;
                    d.PageNumber = pagesCount;
                    if (navigator)
                    {
                        if (saveStreams)
                        {
                            d.PagesStream = new MemoryStream();
                            ExportHTMLPageBegin(d);
                        }
                        else
                        {
                            d.PagesStream = new FileStream(singlePageFileName, FileMode.Append);
                            ExportHTMLPageBegin(d);
                        }
                    }
                    else
                    {
                        if (format == HTMLExportFormat.HTML)
                            d.PagesStream = Stream;
                        else
                            d.PagesStream = mimeStream;

                        ExportHTMLPageBegin(d);
                    }
                }
                else
                    ProcessPageBegin(pagesCount - 1, pagesCount, page);
            }
            else
                // Web
                ProcessPageBegin(pagesCount - 1, pagesCount, page);
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            if (!WebMode)
            {
                if (singlePage)
                {
                    if (navigator)
                    {
                        if (saveStreams)
                        {
                            ExportHTMLPageEnd(d);
                            GeneratedUpdate(singlePageFileName, d.PagesStream);
                        }
                        else
                        {
                            ExportHTMLPageEnd(d);
                            d.PagesStream.Close();
                            d.PagesStream.Dispose();
                        }
                    }
                    else
                    {
                        ExportHTMLPageEnd(d);
                    }
                }
                else
                    ProcessPageEnd(pagesCount - 1, pagesCount);
            }
            else
                // Web
                ProcessPageEnd(pagesCount - 1, pagesCount);
        }

        /// <summary>
        /// Process Page with number p and real page ReportPage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ReportPage"></param>
        /// <param name="page"></param>
        public void ProcessPageBegin(int p, int ReportPage, ReportPage page)
        {
            d.page = page;
            d.ReportPage = ReportPage;
            d.PageNumber = pagesCount;
            d.PagesStream = null;
            d.CurrentPage = p;
            ExportHTMLPageBegin(d);
        }

        /// <summary>
        /// Process Page with number p and real page ReportPage
        /// </summary>
        /// <param name="p"></param>
        /// <param name="ReportPage"></param>
        public void ProcessPageEnd(int p, int ReportPage)
        {
            ExportHTMLPageEnd(d);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (!webMode)
            {
                if (navigator)
                {
                    if (saveStreams)
                    {
                        FinishSaveStreams();
                    }
                    else
                    {
                        if (singlePage)
                        {
                            //if (saveStreams) // Commented because saveStreams is always false!!
                            //{
                            //    int fileIndex = GeneratedFiles.IndexOf(singlePageFileName);
                            //    DoPageEnd(generatedStreams[fileIndex]);
                            //}
                            //else
                            //{
                            using (Stream pageStream = new FileStream(singlePageFileName, FileMode.Append))
                            {
                                DoPageEnd(pageStream);
                            }
                            //} // Commented because saveStreams is always false!!
                        }
                        ExportHTMLIndex(Stream);
                        GeneratedFiles.Add(targetIndexPath + navFileName);
                        using (FileStream outStream = new FileStream(targetIndexPath + navFileName, FileMode.Create))
                        {
                            ExportHTMLNavigator(outStream);
                        }

                        //GeneratedFiles.Add(FTargetIndexPath + FOutlineFileName);
                        //using (FileStream OutStream = new FileStream(FTargetIndexPath + FOutlineFileName, FileMode.Create))
                        //    ExportHTMLOutline(OutStream);
                    }
                }
                else if (format == HTMLExportFormat.MessageHTML)
                {
                    FinishMHT();
                }
                else
                {
                    if (saveStreams)
                    {
                        if (!String.IsNullOrEmpty(singlePageFileName))
                        {
                            int fileIndex = GeneratedFiles.IndexOf(singlePageFileName);
                            DoPageEnd(GeneratedStreams[fileIndex]);
                        }
                    }
                    else
                    {
                        if (!singlePage)
                        {
                            DoPageStart(Stream, documentTitle, false);
                            int pageCounter = 0;
                            foreach (string genFile in GeneratedFiles)
                            {
                                string ext = Path.GetExtension(genFile);
                                if (ext == ".html" && genFile != FileName)
                                {
                                    string file = Path.GetFileName(genFile);
                                    if (subFolder)
                                        file = Path.Combine(subFolderPath, file);
                                    ExportUtils.WriteLn(Stream, String.Format("<a href=\"{0}\">Page {1}</a><br />", file, ++pageCounter));
                                }
                            }
                        }
                        DoPageEnd(Stream);
                    }
                }
            }
        }

        #endregion Protected methods

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteBool("Layers", Layers);
            writer.WriteBool("Wysiwyg", Wysiwyg);
            writer.WriteBool("Pictures", Pictures);
            writer.WriteBool("EmbedPictures", EmbedPictures);
            writer.WriteBool("SubFolder", SubFolder);
            writer.WriteBool("Navigator", Navigator);
            writer.WriteBool("SinglePage", SinglePage);
        }

        /// <summary>
        /// For internal use only.
        /// </summary>
        public void Init_WebMode()
        {
            subFolder = false;
            navigator = false;
            ShowProgress = false;
            pages = new List<HTMLPageData>();
            webMode = true;
            OpenAfterExport = false;
        }

        internal void Finish_WebMode()
        {
            pages.Clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLExport"/> class.
        /// </summary>
        public HTMLExport()
        {
            Zoom = 1.0f;
            HasMultipleFiles = true;
            layers = false;
            wysiwyg = true;
            pictures = true;
            webMode = false;
            subFolder = true;
            navigator = true;
            singlePage = false;
            widthUnits = HtmlSizeUnits.Pixel;
            heightUnits = HtmlSizeUnits.Pixel;
            imageFormat = ImageFormat.Png;
            templates = new HtmlTemplates();
            format = HTMLExportFormat.HTML;
            prevStyleList = null;
            prevStyleListIndex = 0;
            pageBreaks = true;
            print = false;
            preview = false;
            saveStreams = false;
            numberFormat = new NumberFormatInfo();
            numberFormat.NumberGroupSeparator = String.Empty;
            numberFormat.NumberDecimalSeparator = ".";
            exportMode = ExportType.Export;
            res = new MyRes("Export,Html");
            embeddedImages = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HTMLExport"/> class for WebPreview mode.
        /// </summary>
        public HTMLExport(bool webPreview) : this()
        {
            this.webPreview = webPreview;
            if (webPreview)
                exportMode = ExportType.WebPreview;
        }
    }

    /// <summary>
    /// Event arguments for custom drawing of report objects.
    /// </summary>
    public class CustomDrawEventArgs : EventArgs
    {
        /// <summary>
        /// Report object
        /// </summary>
        public Report report;

        /// <summary>
        /// ReportObject.
        /// </summary>
        public ReportComponentBase reportObject;

        /// <summary>
        /// Resulting successfull drawing flag.
        /// </summary>
        public bool done = false;

        /// <summary>
        /// Resulting HTML string.
        /// </summary>
        public string html;

        /// <summary>
        /// Resulting CSS string.
        /// </summary>
        public string css;

        /// <summary>
        /// Layers mode when true or Table mode when false.
        /// </summary>
        public bool layers;

        /// <summary>
        /// Zoom value for scale position and sizes.
        /// </summary>
        public float zoom;

        /// <summary>
        /// Left position.
        /// </summary>
        public float left;

        /// <summary>
        /// Top position.
        /// </summary>
        public float top;

        /// <summary>
        /// Width of object.
        /// </summary>
        public float width;

        /// <summary>
        /// Height of object.
        /// </summary>
        public float height;
    }
}