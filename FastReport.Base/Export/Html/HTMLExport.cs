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

        private struct HTMLThreadData
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

        private List<Stream> generatedStreams;
        private bool saveStreams;

        private string onClickTemplate = String.Empty;
        private string reportID;

        private const string BODY_BEGIN = "</head>\r\n<body bgcolor=\"#FFFFFF\" text=\"#000000\">";
        private const string BODY_END = "</body>";
        private const string PRINT_JS = "<script language=\"javascript\" type=\"text/javascript\"> parent.focus(); parent.print();</script>";
        private const string NBSP = "&nbsp;";
        private int currentPage = 0;
        private HTMLThreadData d;
        private Graphics htmlMeasureGraphics;
        private float maxWidth, maxHeight;
        private FastString css;
        private FastString htmlPage;
        private float leftMargin, topMargin;
        private bool enableMargins = false;
        private ExportType exportMode;
        private bool enableVectorObjects = true;
        #endregion


        #region Public properties

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
            get { return subFolder;  }
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
        /// Gets list of generated streams.
        /// </summary>
        public List<Stream> GeneratedStreams
        {
            get { return generatedStreams; }
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

        #endregion


        #region Private methods


        private void GeneratedUpdate(string filename, Stream stream)
        {
            int i = GeneratedFiles.IndexOf(filename);
            if (i == -1)
            {
                GeneratedFiles.Add(filename);
                generatedStreams.Add(stream);
            }
            else
            {
                generatedStreams[i] = stream;
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

        private void ExportHTMLPageFinal(FastString CSS, FastString Page, HTMLThreadData d, float MaxWidth, float MaxHeight)
        {
            if (!webMode)
            {
                if (!singlePage)
                    Page.AppendLine(BODY_END);
                if (d.PagesStream == null)
                {
                    if (saveStreams)
                    {
                        string FPageFileName;
                        if (singlePage)
                            FPageFileName = singlePageFileName;
                        else
                            FPageFileName = targetIndexPath + targetFileName + d.PageNumber.ToString() + ".html";
                        int i = GeneratedFiles.IndexOf(FPageFileName);
                        Stream OutStream;
                        if (i == -1)
                            OutStream = new MemoryStream();
                        else
                            OutStream = generatedStreams[i];
                        if (!singlePage)
                            ExportUtils.Write(OutStream, String.Format(templates.PageTemplateTitle, documentTitle));
                        if (CSS != null)
                        {
                            ExportUtils.Write(OutStream, CSS.ToString());
                            CSS.Clear();
                        }
                        if (Page != null)
                        {
                            ExportUtils.Write(OutStream, Page.ToString());
                            Page.Clear();
                        }
                        if (!singlePage)
                            ExportUtils.Write(OutStream, templates.PageTemplateFooter);
                        GeneratedUpdate(FPageFileName, OutStream);
                    }
                    else
                    {
                        string FPageFileName = targetIndexPath + targetFileName + d.PageNumber.ToString() + ".html";
                        GeneratedFiles.Add(FPageFileName);
                        using (FileStream OutStream = new FileStream(FPageFileName, FileMode.Create))
                        using (StreamWriter Out = new StreamWriter(OutStream))
                        {
                            if (!singlePage)
                                Out.Write(String.Format(templates.PageTemplateTitle, documentTitle));
                            if (CSS != null)
                            {
                                Out.Write(CSS.ToString());
                                CSS.Clear();
                            }
                            if (Page != null)
                            {
                                Out.Write(Page.ToString());
                                Page.Clear();
                            }
                            if (!singlePage)
                                Out.Write(templates.PageTemplateFooter);
                        }
                    }
                }
                else
                {
                    if (!singlePage)
                        ExportUtils.Write(d.PagesStream, String.Format(templates.PageTemplateTitle, documentTitle));
                    if (CSS != null)
                    {
                        ExportUtils.Write(d.PagesStream, CSS.ToString());
                        CSS.Clear();
                    }
                    if (Page != null)
                    {
                        ExportUtils.Write(d.PagesStream, Page.ToString());
                        Page.Clear();
                    }
                    if (!singlePage)
                        ExportUtils.Write(d.PagesStream, templates.PageTemplateFooter);
                }
            }
            else
            {
                if (!layers)
                {
                    pages[d.CurrentPage].Width = MaxWidth / Zoom;
                    pages[d.CurrentPage].Height = MaxHeight / Zoom;
                }
                else
                {
                    pages[d.CurrentPage].Width = MaxWidth;
                    pages[d.CurrentPage].Height = MaxHeight;
                }

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

        private void ExportHTMLIndex(Stream Stream)
        {
            ExportUtils.Write(Stream, String.Format(templates.IndexTemplate,
                new object[] { documentTitle, ExportUtils.HtmlURL(navFileName), 
                        ExportUtils.HtmlURL(targetFileName + 
                        (singlePage ? ".main" : "1") + ".html") }));
        }

        private void ExportHTMLNavigator(Stream OutStream)
        {
            string prefix = ExportUtils.HtmlURL(fileName); 
            //  {0} - pages count {1} - name of report {2} multipage document {3} prefix of pages
            //  {4} first caption {5} previous caption {6} next caption {7} last caption
            //  {8} total caption
            ExportUtils.Write(OutStream, String.Format(templates.NavigatorTemplate,
                new object[] { pagesCount.ToString(), 
                        documentTitle, (singlePage ? "0" : "1"), 
                        prefix, res.Get("First"), res.Get("Prev"), 
                        res.Get("Next"), res.Get("Last"), res.Get("Total") }));
        }

        #endregion

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
            generatedStreams = new List<Stream>();

            if (saveStreams)
            {
                if (singlePage)
                    GeneratedUpdate("index.html", null);
                subFolder = false;
                navigator = false;
                //FSinglePage = true;
            }
            
            if (!webMode)
            {
                if (format == HTMLExportFormat.MessageHTML)
                {
                    subFolder = false;
                    singlePage = true;
                    navigator = false;
                    mimeStream = new MemoryStream();
                    boundary = ExportUtils.GetID();
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
                    subFolderPath = 
                        targetFileName + ".files" + 
                        Path.DirectorySeparatorChar;
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
                            MemoryStream PageStream = new MemoryStream();
                            ExportUtils.Write(PageStream, String.Format(templates.PageTemplateTitle, documentTitle));
                            if (print)
                                ExportUtils.WriteLn(PageStream, PRINT_JS);
                            ExportUtils.WriteLn(PageStream, BODY_BEGIN);
                            GeneratedUpdate(singlePageFileName, PageStream);
                        }
                        else
                        {
                            using (Stream PageStream = new FileStream(singlePageFileName,
                                FileMode.Create))
                            using (StreamWriter Out = new StreamWriter(PageStream))
                            {
                                Out.Write(String.Format(templates.PageTemplateTitle, documentTitle));
                                if (print)
                                    Out.WriteLine(PRINT_JS);
                                Out.WriteLine(BODY_BEGIN);
                            }
                        }
                    }
                    else
                    {
                        singlePageFileName = String.IsNullOrEmpty(FileName) ? "index.html" : FileName;
                        Stream PagesStream;

                        if (saveStreams)
                        {
                            PagesStream = new MemoryStream();
                            GeneratedUpdate(singlePageFileName, PagesStream);
                        }
                        else
                        {
                            if (format == HTMLExportFormat.HTML)
                                PagesStream = Stream;
                            else
                                PagesStream = mimeStream;
                        }
                        ExportUtils.Write(PagesStream, String.Format(templates.PageTemplateTitle, documentTitle));
                        if (print)
                            ExportUtils.WriteLn(PagesStream, PRINT_JS);
                        ExportUtils.WriteLn(PagesStream, BODY_BEGIN);
                    }
                }
            }
            else
            {
                pages.Clear();
                for (int i = 0; i < count; i++)
                    pages.Add(new HTMLPageData());                    
            }            
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            if(ExportMode == ExportType.Export)
                base.ExportPageBegin(page);
            pagesCount++;
            if (!WebMode)
            {
                if (singlePage)
                {
                    d = new HTMLThreadData();
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
                        // do append in memory stream
                        MemoryStream strm = new MemoryStream();
                        int i = GeneratedFiles.IndexOf(singlePageFileName);                        
                        ExportHTMLIndex(generatedStreams[i]);
                        MemoryStream OutStream = new MemoryStream();
                        ExportHTMLNavigator(OutStream);
                        GeneratedUpdate(targetIndexPath + navFileName, OutStream);
                    }
                    else
                    {
                        if (singlePage)
                        {
                            if (saveStreams)
                            {
                                int i = GeneratedFiles.IndexOf(singlePageFileName);
                                Stream PageStream = generatedStreams[i];
                                ExportUtils.WriteLn(PageStream, BODY_END);
                                ExportUtils.Write(PageStream, templates.PageTemplateFooter);
                            }
                            else
                            {
                                using (Stream PageStream = new FileStream(singlePageFileName,
                                    FileMode.Append))
                                using (StreamWriter Out = new StreamWriter(PageStream))
                                {
                                    Out.WriteLine(BODY_END);
                                    Out.Write(templates.PageTemplateFooter);
                                }
                            }
                        }
                        ExportHTMLIndex(Stream);
                        GeneratedFiles.Add(targetIndexPath + navFileName);
                        using (FileStream OutStream = new FileStream(targetIndexPath + navFileName, FileMode.Create))
                            ExportHTMLNavigator(OutStream);
                        //GeneratedFiles.Add(FTargetIndexPath + FOutlineFileName);
                        //using (FileStream OutStream = new FileStream(FTargetIndexPath + FOutlineFileName, FileMode.Create))
                        //    ExportHTMLOutline(OutStream);
                    }
                }
                else if (format == HTMLExportFormat.MessageHTML)
                {

                    ExportUtils.WriteLn(mimeStream, BODY_END);
                    ExportUtils.Write(mimeStream, templates.PageTemplateFooter);

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
                else
                {
                    if (saveStreams)
                    {
                        if (!String.IsNullOrEmpty(singlePageFileName))
                        {
                            int i = GeneratedFiles.IndexOf(singlePageFileName);
                            ExportUtils.WriteLn(generatedStreams[i], BODY_END);
                            ExportUtils.Write(generatedStreams[i], templates.PageTemplateFooter);
                        }
                    }
                    else
                    {
                        int pageCnt = 0;
                        foreach (string genFile in GeneratedFiles)
                        {
                            string ext = Path.GetExtension(genFile);
                            if (ext == ".html" && genFile != FileName)
                            {
                                string file = Path.GetFileName(genFile);
                                if (subFolder)
                                    file = Path.Combine(subFolderPath, file);
                                ExportUtils.WriteLn(Stream, String.Format("<a href=\"{0}\">Page {1}</a><br />", file, ++pageCnt));                                
                            }                            
                        }
                        //ExportUtils.WriteLn(Stream, BODY_END);
                        //ExportUtils.Write(Stream, FTemplates.PageTemplateFooter); 
                    }
                }
            }
        }
        #endregion

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
