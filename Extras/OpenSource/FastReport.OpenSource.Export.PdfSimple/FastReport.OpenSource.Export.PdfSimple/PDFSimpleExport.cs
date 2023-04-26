using FastReport.Export.PdfSimple.PdfCore;
using FastReport.Export.PdfSimple.PdfObjects;
using FastReport.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace FastReport.Export.PdfSimple
{
    /// <summary>
    /// This is a simple PDF export made for OpenSource edition.
    /// If possible, use full export <see cref="FastReport.Export.Pdf.PDFExport"/>
    /// </summary>
    /// <remarks>
    /// If the size of the images exceeds 2 GB, the images will not be exported.
    /// Be careful when you set the page size and images dpi.
    /// </remarks>
    public partial class PDFSimpleExport : ExportBase
    {
        #region Private Fields

        private Bitmap pageBitmap;
        private PdfContents pageContent;
        private Graphics pageGraphics;
        private PdfPage pdfPage;
        private PdfPages pdfPages;
        private PdfIndirectObject pdfPagesLink;
        private PdfWriter pdfWriter;
        private float scaleFactor = 1f;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public PDFSimpleExport()
        {
        }

        #endregion Public Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            ClearBitmaps();
        }

        /// <inheritdoc/>
        protected override void ExportBand(BandBase band)
        {
            base.ExportBand(band);
            ExportObj(band);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                if (!(c is Table.TableColumn || c is Table.TableCell || c is Table.TableRow))
                    ExportObj(c);
            }
        }

        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            // begin and prepare
            base.ExportPageBegin(page);
            pdfPage = new PdfPage();
            pdfPage.Parent = pdfPagesLink;
            pdfPage.MediaBox = new System.Drawing.RectangleF(0, 0,
                ExportUtils.GetPageWidth(page) * PdfWriter.PDF_PAGE_DIVIDER,
                ExportUtils.GetPageHeight(page) * PdfWriter.PDF_PAGE_DIVIDER);

            // export page as one image
            {
                ClearBitmaps();

                scaleFactor = ImageDpi / 96f;
                int width = (int)(ExportUtils.GetPageWidth(page) * scaleFactor * Units.Millimeters);
                int height = (int)(ExportUtils.GetPageHeight(page) * scaleFactor * Units.Millimeters);
                // check for max bitmap object size
                // 2GB (max .net object size) / 4 (Format32bppArgb is 4 bytes)
                // see http://stackoverflow.com/a/29175905/4667434
                const ulong maxPixels = 536870912;
                if ((ulong)width * (ulong)height < maxPixels)
                {
                    pageBitmap = new Bitmap(width, height);
                    pageGraphics = Graphics.FromImage(pageBitmap);
                    pageGraphics.TranslateTransform(this.scaleFactor * page.LeftMargin * Units.Millimeters, this.scaleFactor *  page.TopMargin * Units.Millimeters, System.Drawing.Drawing2D.MatrixOrder.Append);
                    //pageGraphics.ScaleTransform(scale, scale, System.Drawing.Drawing2D.MatrixOrder.Append);
                }
            }

            pageContent = new PdfContents();

            // export page background
            using (TextObject pageFill = new TextObject())
            {
                pageFill.Fill = page.Fill;
                pageFill.Left = -page.LeftMargin * Units.Millimeters;
                pageFill.Top = -page.TopMargin * Units.Millimeters;
                pageFill.Width = ExportUtils.GetPageWidth(page) * Units.Millimeters;
                pageFill.Height = ExportUtils.GetPageHeight(page) * Units.Millimeters;
                ExportObj(pageFill);
            }

            // export bottom watermark
            if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);

            // page borders
            if (page.Border.Lines != BorderLines.None)
            {
                using (TextObject pageBorder = new TextObject())
                {
                    pageBorder.Border = page.Border;
                    pageBorder.Left = 0;
                    pageBorder.Top = 0;
                    pageBorder.Width = (ExportUtils.GetPageWidth(page) - page.LeftMargin - page.RightMargin) * PdfWriter.PDF_PAGE_DIVIDER / PdfWriter.PDF_DIVIDER;
                    pageBorder.Height = (ExportUtils.GetPageHeight(page) - page.TopMargin - page.BottomMargin) * PdfWriter.PDF_PAGE_DIVIDER / PdfWriter.PDF_DIVIDER;
                    ExportObj(pageBorder);
                }
            }
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            base.ExportPageEnd(page);

            // export top watermark
            if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);

            pageGraphics.Dispose();
            pageGraphics = null;
            DrawImage(new System.Drawing.RectangleF(0, 0,
                ExportUtils.GetPageWidth(page) * PdfWriter.PDF_PAGE_DIVIDER,
                ExportUtils.GetPageHeight(page) * PdfWriter.PDF_PAGE_DIVIDER), pageBitmap);
            pdfPage["Contents"] = pdfWriter.Write(pageContent);

            pdfPages.Kids.Add(pdfWriter.Write(pdfPage));
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            base.Finish();

            PdfInfo info = new PdfInfo();
            info.Title = Title;
            info.Subject = Subject;
            info.Keywords = Keywords;
            info.Author = Author;
            pdfWriter.Write(info);
            pdfWriter.Finish();
            Stream.Flush();
            ClearBitmaps();
        }

        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            return new MyRes("FileFilters").Get("PdfFile");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            ClearBitmaps();
            base.Start();
            pdfWriter = new PdfWriter(Stream);
            pdfWriter.Begin();
            PdfCatalog catalog = new PdfCatalog();
            pdfPages = new PdfPages();
            pdfPagesLink = pdfWriter.Prepare(pdfPages);
            catalog.Pages = pdfPagesLink;
            pdfWriter.Prepare(catalog);
            hashList = new Dictionary<string, PdfIndirectObject>();
        }

        #endregion Protected Methods

        #region Private Methods

        private void AddImageWatermark(ReportPage page)
        {
            if (pageGraphics != null)
            {
                page.Watermark.DrawImage(new FRPaintEventArgs(pageGraphics, scaleFactor, scaleFactor, Report.GraphicCache),
                    new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, ExportUtils.GetPageWidth(page) * Units.Millimeters, ExportUtils.GetPageHeight(page) * Units.Millimeters),
                    page.Report, false);
            }
        }

        private void AddTextWatermark(ReportPage page)
        {
            if (pageGraphics != null)
            {
                if (string.IsNullOrEmpty(page.Watermark.Text))
                    return;
                
                page.Watermark.DrawText(new FRPaintEventArgs(pageGraphics, scaleFactor, scaleFactor, Report.GraphicCache),
                    new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, ExportUtils.GetPageWidth(page) * Units.Millimeters, ExportUtils.GetPageHeight(page) * Units.Millimeters),
                    page.Report, false);
            }
        }

        private void ClearBitmaps()
        {
            if (pageGraphics != null)
            {
                pageGraphics.Dispose();
                pageGraphics = null;
            }
            if (pageBitmap != null)
            {
                pageBitmap.Dispose();
                pageBitmap = null;
            }
        }

        private void ExportObj(Base obj)
        {
            if (pageGraphics != null)
            {
                if (obj is ReportComponentBase && (obj as ReportComponentBase).Exportable)
                    (obj as ReportComponentBase).Draw(new FRPaintEventArgs(pageGraphics, scaleFactor, scaleFactor, Report.GraphicCache));
            }
        }

        #endregion Private Methods
    }
}
