using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Preview
{
    /// <summary>
    /// Specifies an action that will be performed on <b>PreparedPages.AddPage</b> method call.
    /// </summary>
    public enum AddPageAction
    {
        /// <summary>
        /// Do not add the new prepared page if possible, increment the <b>CurPage</b> instead.
        /// </summary>
        WriteOver,

        /// <summary>
        /// Add the new prepared page.
        /// </summary>
        Add
    }

    /// <summary>
    /// Represents the pages of a prepared report.
    /// </summary>
    /// <remarks>
    /// <para>Prepared page is a page that you can see in the preview window. Prepared pages can be
    /// accessed via <see cref="FastReport.Report.PreparedPages"/> property.</para>
    /// <para>The common scenarios of using this object are:
    /// <list type="bullet">
    ///   <item>
    ///     <description>Working with prepared pages after the report is finished: load 
    ///     (<see cref="Load(string)"/>) or save (<see cref="Save(string)"/>) pages 
    ///     from/to a .fpx file, get a page with specified index to work with its objects 
    ///     (<see cref="GetPage"/>); modify specified page (<see cref="ModifyPage"/>).
    ///     </description>
    ///   </item>
    ///   <item>
    ///     <description>Using the <see cref="AddPage"/>, <see cref="AddSourcePage"/>, <see cref="AddBand"/>
    ///     methods while report is generating to produce an output.
    ///     </description>
    ///   </item>
    /// </list>
    /// </para>
    /// </remarks>
    [ToolboxItem(false)]
    public partial class PreparedPages : IDisposable
    {
        #region Fields
        private SourcePages sourcePages;
        private List<PreparedPage> preparedPages;
        private Dictionary dictionary;
        private Bookmarks bookmarks;
        private Outline outline;
        private BlobStore blobStore;
        private int curPage;
        private AddPageAction addPageAction;
        private Report report;
        private PageCache pageCache;
        private FileStream tempFile;
        private bool canUpload;
        private string tempFileName;
        private XmlItem cutObjects;
        private Dictionary<string, object> macroValues;
        private int firstPassPage;
        private int firstPassPosition;
        #endregion

        #region Properties
        internal Report Report
        {
            get { return report; }
        }

        internal SourcePages SourcePages
        {
            get { return sourcePages; }
        }

        internal Dictionary Dictionary
        {
            get { return dictionary; }
        }

        internal Bookmarks Bookmarks
        {
            get { return bookmarks; }
        }

        internal Outline Outline
        {
            get { return outline; }
        }

        internal BlobStore BlobStore
        {
            get { return blobStore; }
        }

        internal FileStream TempFile
        {
            get { return tempFile; }
        }

        internal Dictionary<string, object> MacroValues
        {
            get { return macroValues; }
        }

        internal int CurPosition
        {
            get
            {
                if (CurPage < 0 || CurPage >= Count)
                    return 0;
                return preparedPages[CurPage].CurPosition;
            }
        }

        internal int CurPage
        {
            get { return curPage; }
            set { curPage = value; }
        }

        /// <summary>
        /// Gets the number of pages in the prepared report.
        /// </summary>
        public int Count
        {
            get { return preparedPages.Count; }
        }

        /// <summary>
        /// Specifies an action that will be performed on <see cref="AddPage"/> method call.
        /// </summary>
        public AddPageAction AddPageAction
        {
            get { return addPageAction; }
            set { addPageAction = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the prepared pages can be uploaded to the file cache.
        /// </summary>
        /// <remarks>
        /// <para>This property is used while report is generating.</para>
        /// <para>Default value for this property is <b>true</b>. That means the prepared pages may be uploaded to
        /// the file cache if needed. To prevent this (for example, if you need to access some objects
        /// on previously generated pages), set the property value to <b>false</b>.</para>
        /// </remarks>
        public bool CanUploadToCache
        {
            get { return canUpload; }
            set
            {
              if (canUpload != value)
              {
                canUpload = value;
                if (value)
                  UploadPages();
              }
            }
        }
        #endregion

        #region Private Methods
        private void UploadPages()
        {
            if (Report.UseFileCache)
            {
                for (int i = 0; i < Count - 1; i++)
                {
                    preparedPages[i].Upload();
                }
            }
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        public void Dispose()
        {
            Clear();
            if (tempFile != null)
            {
                tempFile.Dispose();
                tempFile = null;
                if (File.Exists(tempFileName))
                    File.Delete(tempFileName);
            }
            BlobStore.Dispose();
        }
        #endregion

        #region Public Methods
        internal void SetReport(Report report)
        {
            this.report = report;
        }

        /// <summary>
        /// Adds a source page to the prepared pages dictionary.
        /// </summary>
        /// <param name="page">The template page to add.</param>
        /// <remarks>
        /// Call this method before using <b>AddPage</b> and <b>AddBand</b> methods. This method adds
        /// a page to the dictionary that will be used to decrease size of the prepared report.
        /// </remarks>
        public void AddSourcePage(ReportPage page)
        {
            SourcePages.Add(page);
        }

        /// <summary>
        /// Adds a new page.
        /// </summary>
        /// <param name="page">The original (template) page to add.</param>
        /// <remarks>
        /// Call the <see cref="AddSourcePage"/> method before adding a page. This method creates 
        /// a new output page with settings based on <b>page</b> parameter.
        /// </remarks>
        public void AddPage(ReportPage page)
        {
            CurPage++;
            if (CurPage >= Count || AddPageAction != AddPageAction.WriteOver)
            {
                PreparedPage preparedPage = new PreparedPage(page, this);
                preparedPages.Add(preparedPage);

                // upload previous page to the file cache if enabled
                if (CanUploadToCache && Count > 1)
                    preparedPages[Count - 2].Upload();

                AddPageAction = AddPageAction.WriteOver;
                CurPage = Count - 1;
                Report.Engine.IncLogicalPageNumber();
            }
        }

        /// <summary>
        /// Prints a band with all its child objects.
        /// </summary>
        /// <param name="band">The band to print.</param>
        /// <returns><b>true</b> if band was printed; <b>false</b> if it can't be printed 
        /// on current page due to its <b>PrintOn</b> property value.</returns>
        /// <remarks>
        /// Call the <see cref="AddPage"/> method before adding a band.
        /// </remarks>
        public bool AddBand(BandBase band)
        {
            return preparedPages[CurPage].AddBand(band);
        }

        /// <summary>
        /// Gets a page with specified index.
        /// </summary>
        /// <param name="index">Zero-based index of page.</param>
        /// <returns>The page with specified index.</returns>
        public ReportPage GetPage(int index)
        {
            if (index >= 0 && index < preparedPages.Count)
            {
                macroValues["Page#"] = index + Report.InitialPageNumber;
                macroValues["TotalPages#"] = preparedPages.Count + Report.InitialPageNumber - 1;
                ReportPage page = preparedPages[index].GetPage();
                if (page.MirrorMargins && (index + 1) % 2 == 0)
                {
                    float f = page.LeftMargin;
                    page.LeftMargin = page.RightMargin;
                    page.RightMargin = f;
                }
                return page;
            }
            else
                return null;
        }

        internal PreparedPage GetPreparedPage(int index)
        {
            if (index >= 0 && index < preparedPages.Count)
            {
                macroValues["Page#"] = index + Report.InitialPageNumber;
                macroValues["TotalPages#"] = preparedPages.Count + Report.InitialPageNumber - 1;
                return preparedPages[index];
            }
            else
                return null;
        }

        internal ReportPage GetCachedPage(int index)
        {
            return pageCache.Get(index);
        }

        /// <summary>
        /// Gets the size of specified page, in pixels.
        /// </summary>
        /// <param name="index">Index of page.</param>
        /// <returns>the size of specified page, in pixels.</returns>
        public SizeF GetPageSize(int index)
        {
            return preparedPages[index].PageSize;
        }

        /// <summary>
        /// Replaces the prepared page with specified one.
        /// </summary>
        /// <param name="index">The index of prepared page to replace.</param>
        /// <param name="newPage">The new page to replace with.</param>
        public void ModifyPage(int index, ReportPage newPage)
        {
            PreparedPage preparedPage = new PreparedPage(newPage, this);
            foreach (Base obj in newPage.ChildObjects)
            {
                if (obj is BandBase)
                    preparedPage.AddBand(obj as BandBase);
            }
            preparedPages[index].Dispose();
            preparedPages[index] = preparedPage;
            pageCache.Remove(index);
        }

        /// <summary>
        /// Modify the prepared page with new sizes.
        /// </summary>
        /// <param name="name">The name of prepared page to reSize.</param>
        public void ModifyPageSize(string name)
        {
            foreach (PreparedPage page in preparedPages)
            {
                if (String.Equals(name, page.GetName(), StringComparison.InvariantCultureIgnoreCase))
                {
                    page.ReCalcSizes();
                }
            }
        }

        /// <summary>
        /// Removes a page with the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of page to remove.</param>
        public void RemovePage(int index)
        {
            preparedPages[index].Dispose();
            preparedPages.RemoveAt(index);
            pageCache.Clear();
        }


        /// <summary>
        /// Creates a copy of a page with specified index and inserts it after original one.
        /// </summary>
        /// <param name="index">The zero-based index of original page.</param>
        public void CopyPage(int index)
        {
            // insert a new empty page at specified index
            PreparedPage newPage = new PreparedPage(null, this);
            if (index == preparedPages.Count - 1)
                preparedPages.Add(newPage);
            else
                preparedPages.Insert(index + 1, newPage);

            // and copy source page into it
            ModifyPage(index + 1, GetPage(index));
            pageCache.Clear();
        }

        internal void InterleaveWithBackPage(int backPageIndex)
        {
            PreparedPage page = preparedPages[backPageIndex];
            int count = backPageIndex - 1;
            for (int i = 0; i < count; i++)
            {
                preparedPages.Insert(i * 2 + 1, page);
            }
        }

        internal void ApplyWatermark(Watermark watermark)
        {
            SourcePages.ApplyWatermark(watermark);
            pageCache.Clear();
        }

        internal void CutObjects(int index)
        {
            cutObjects = preparedPages[CurPage].CutObjects(index);
        }

        internal void PasteObjects(float x, float y)
        {
            preparedPages[CurPage].PasteObjects(cutObjects, x, y);
        }

        internal float GetLastY()
        {
            return preparedPages[CurPage].GetLastY();
        }

        internal void PrepareToFirstPass()
        {
            firstPassPage = CurPage;
            firstPassPosition = CurPosition;
            Outline.PrepareToFirstPass();
        }

        internal void ClearFirstPass()
        {
            Bookmarks.ClearFirstPass();
            Outline.ClearFirstPass();

            // clear all pages after specified FFirstPassPage
            while (firstPassPage < Count - 1)
            {
                RemovePage(Count - 1);
            }

            // if position is at begin, clear all pages
            if (firstPassPage == 0 && firstPassPosition == 0)
                RemovePage(0);

            // delete objects on the FFirstPassPage
            if (firstPassPage >= 0 && firstPassPage < Count)
                preparedPages[firstPassPage].CutObjects(firstPassPosition).Dispose();

            CurPage = firstPassPage;
        }

        internal bool ContainsBand(Type bandType)
        {
            return preparedPages[CurPage].ContainsBand(bandType);
        }

        internal bool ContainsBand(string bandName)
        {
            return preparedPages[CurPage].ContainsBand(bandName);
        }

        /// <summary>
        /// Saves prepared pages to a stream.
        /// </summary>
        /// <param name="stream">The stream to save to.</param>
        public void Save(Stream stream)
        {
            if (Config.PreparedCompressed)
                stream = Compressor.Compress(stream);

            using (XmlDocument doc = new XmlDocument())
            {
                doc.AutoIndent = true;
                doc.Root.Name = "preparedreport";

                // save ReportInfo
                doc.Root.SetProp("ReportInfo.Name", Report.ReportInfo.Name);
                doc.Root.SetProp("ReportInfo.Author", Report.ReportInfo.Author);
                doc.Root.SetProp("ReportInfo.Description", Report.ReportInfo.Description);                
                doc.Root.SetProp("ReportInfo.Created", SystemFake.DateTime.Now.ToString());
                doc.Root.SetProp("ReportInfo.Modified", SystemFake.DateTime.Now.ToString());
                doc.Root.SetProp("ReportInfo.CreatorVersion", Report.ReportInfo.CreatorVersion);                

                XmlItem pages = doc.Root.Add();
                pages.Name = "pages";

                // attach each page's xml to the doc
                foreach (PreparedPage page in preparedPages)
                {
                    page.Load();
                    pages.AddItem(page.Xml);
                }

                XmlItem sourcePages = doc.Root.Add();
                sourcePages.Name = "sourcepages";
                SourcePages.Save(sourcePages);

                XmlItem dictionary = doc.Root.Add();
                dictionary.Name = "dictionary";
                Dictionary.Save(dictionary);

                XmlItem bookmarks = doc.Root.Add();
                bookmarks.Name = "bookmarks";
                Bookmarks.Save(bookmarks);

                doc.Root.AddItem(Outline.Xml);

                XmlItem blobStore = doc.Root.Add();
                blobStore.Name = "blobstore";
                BlobStore.Save(blobStore);

                doc.Save(stream);

                // detach each page's xml from the doc
                foreach (PreparedPage page in preparedPages)
                {
                    page.Xml.Parent = null;
                    page.ClearUploadedXml();
                }
                Outline.Xml.Parent = null;
            }

            if (Config.PreparedCompressed)
                stream.Dispose();
        }

        /// <summary>
        /// Saves prepared pages to a .fpx file.
        /// </summary>
        /// <param name="fileName">The name of the file to save to.</param>
        public void Save(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                Save(stream);
            }
        }

        /// <summary>
        /// Loads prepared pages from a stream.
        /// </summary>
        /// <param name="stream">The stream to load from.</param>
        public void Load(Stream stream)
        {
            Clear();

            if (stream.Length == 0)
                return;

            if (!stream.CanSeek)
            {
                MemoryStream tempStream = new MemoryStream();
                const int BUFFER_SIZE = 32768;
                stream.CopyTo(tempStream, BUFFER_SIZE);
                tempStream.Position = 0;
                stream = tempStream;
            }

            bool compressed = Compressor.IsStreamCompressed(stream);
            if (compressed)
                stream = Compressor.Decompress(stream, false);

            try
            {
                using (XmlDocument doc = new XmlDocument())
                {
                    doc.Load(stream);

                    XmlItem sourcePages = doc.Root.FindItem("sourcepages");
                    SourcePages.Load(sourcePages);

                    XmlItem dictionary = doc.Root.FindItem("dictionary");
                    Dictionary.Load(dictionary);

                    XmlItem bookmarks = doc.Root.FindItem("bookmarks");
                    Bookmarks.Load(bookmarks);

                    XmlItem outline = doc.Root.FindItem("outline");
                    Outline.Xml = outline;

                    XmlItem blobStore = doc.Root.FindItem("blobstore");
                    BlobStore.LoadDestructive(blobStore);

                    XmlItem pages = doc.Root.FindItem("pages");
                    while (pages.Count > 0)
                    {
                        XmlItem pageItem = pages[0];
                        PreparedPage preparedPage = new PreparedPage(null, this);
                        preparedPages.Add(preparedPage);
                        preparedPage.Xml = pageItem;
                    }

                    // load ReportInfo
                    Report.ReportInfo.Name = doc.Root.GetProp("ReportInfo.Name");
                    Report.ReportInfo.Author = doc.Root.GetProp("ReportInfo.Author");
                    Report.ReportInfo.Description = doc.Root.GetProp("ReportInfo.Description");
                    DateTime createDate;
                    if (DateTime.TryParse(doc.Root.GetProp("ReportInfo.Created"), out createDate))
                        Report.ReportInfo.Created = createDate;
                    if (DateTime.TryParse(doc.Root.GetProp("ReportInfo.Modified"), out createDate))
                        Report.ReportInfo.Modified = createDate;
                    Report.ReportInfo.CreatorVersion = doc.Root.GetProp("ReportInfo.CreatorVersion");
                }
            }
            finally
            {
                if (compressed)
                    stream.Dispose();
            }
        }

        /// <summary>
        /// Loads prepared pages from a .fpx file.
        /// </summary>
        /// <param name="fileName">The name of the file to load from.</param>
        public void Load(string fileName)
        {
            using (FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                Load(stream);
            }
        }

        /// <summary>
        /// Clears the prepared report's pages.
        /// </summary>
        public void Clear()
        {
            sourcePages.Clear();
            pageCache.Clear();
            foreach (PreparedPage page in preparedPages)
            {
                page.Dispose();
            }
            preparedPages.Clear();
            dictionary.Clear();
            bookmarks.Clear();
            outline.Clear();
            blobStore.Clear();
            curPage = 0;
        }

        internal void ClearPageCache()
        {
            pageCache.Clear();
        }
        #endregion

        /// <summary>
        /// Creates the pages of a prepared report
        /// </summary>
        /// <param name="report"></param>
        public PreparedPages(Report report)
        {
            this.report = report;
            sourcePages = new SourcePages(this);
            preparedPages = new List<PreparedPage>();
            dictionary = new Dictionary(this);
            bookmarks = new Bookmarks();
            outline = new Outline();
            blobStore = new BlobStore(report != null ? report.UseFileCache : false);
            pageCache = new PageCache(this);
            macroValues = new Dictionary<string, object>();
            if (this.report != null && this.report.UseFileCache)
            {
                tempFileName = Config.CreateTempFile("");
                tempFile = new FileStream(tempFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            }
        }
    }
}
