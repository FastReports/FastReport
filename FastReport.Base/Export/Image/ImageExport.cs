using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using FastReport.Utils;

namespace FastReport.Export.Image
{
    /// <summary>
    /// Specifies the image export format.
    /// </summary>
    public enum ImageExportFormat
    {
        /// <summary>
        /// Specifies the .bmp format.
        /// </summary>
        Bmp,

        /// <summary>
        /// Specifies the .png format.
        /// </summary>
        Png,

        /// <summary>
        /// Specifies the .jpg format.
        /// </summary>
        Jpeg,

        /// <summary>
        /// Specifies the .gif format.
        /// </summary>
        Gif,

        /// <summary>
        /// Specifies the .tif format.
        /// </summary>
        Tiff,

        /// <summary>
        /// Specifies the .emf format.
        /// </summary>
        Metafile
    }

    /// <summary>
    /// Represents the image export filter.
    /// </summary>
    public partial class ImageExport : ExportBase
    {
        private ImageExportFormat imageFormat;
        private bool separateFiles;
        private int resolutionX;
        private int resolutionY;
        private int jpegQuality;
        private bool multiFrameTiff;
        private bool monochromeTiff;
        private EncoderValue monochromeTiffCompression;
        private System.Drawing.Image masterTiffImage;
        private System.Drawing.Image bigImage;
        private Graphics bigGraphics;
        private float curOriginY;
        private bool firstPage;
        private int paddingNonSeparatePages;
        private int pageNumber;
        private System.Drawing.Image image;
        private Graphics g;
        private int height;
        private int width;
        private int widthK;
        private string fileSuffix;
        private float zoomX;
        private float zoomY;
        private System.Drawing.Drawing2D.GraphicsState state;

        #region Properties
        /// <summary>
        /// Gets or sets the image format.
        /// </summary>
        public ImageExportFormat ImageFormat
        {
            get { return imageFormat; }
            set { imageFormat = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether to generate separate image file 
        /// for each exported page.
        /// </summary>
        /// <remarks>
        /// If this property is set to <b>false</b>, the export filter will produce one big image
        /// containing all exported pages. Be careful using this property with a big report
        /// because it may produce out of memory error.
        /// </remarks>
        public bool SeparateFiles
        {
            get { return separateFiles; }
            set { separateFiles = value; }
        }

        /// <summary>
        /// Gets or sets image resolution, in dpi.
        /// </summary>
        /// <remarks>
        /// By default this property is set to 96 dpi. Use bigger values (300-600 dpi)
        /// if you going to print the exported images.
        /// </remarks>
        public int Resolution
        {
            get { return resolutionX; }
            set
            {
                resolutionX = value;
                resolutionY = value;
            }
        }

        /// <summary>
        /// Gets or sets horizontal image resolution, in dpi.
        /// </summary>
        /// <remarks>
        /// Separate horizontal and vertical resolution is used when exporting to TIFF. In other
        /// cases, use the <see cref="Resolution"/> property instead.
        /// </remarks>
        public int ResolutionX
        {
            get { return resolutionX; }
            set { resolutionX = value; }
        }

        /// <summary>
        /// Gets or sets vertical image resolution, in dpi.
        /// </summary>
        /// <remarks>
        /// Separate horizontal and vertical resolution is used when exporting to TIFF. In other
        /// cases, use the <see cref="Resolution"/> property instead.
        /// </remarks>
        public int ResolutionY
        {
            get { return resolutionY; }
            set { resolutionY = value; }
        }

        /// <summary>
        /// Gets or sets the jpg image quality.
        /// </summary>
        /// <remarks>
        /// This property is used if <see cref="ImageFormat"/> is set to <b>Jpeg</b>. By default
        /// it is set to 100. Use lesser value to decrease the jpg file size.
        /// </remarks>
        public int JpegQuality
        {
            get { return jpegQuality; }
            set { jpegQuality = value; }
        }

        /// <summary>
        /// Gets or sets the value determines whether to produce multi-frame tiff file.
        /// </summary>
        public bool MultiFrameTiff
        {
            get { return multiFrameTiff; }
            set { multiFrameTiff = value; }
        }

        /// <summary>
        /// Gets or sets a value that determines whether the Tiff export must produce monochrome image.
        /// </summary>
        /// <remarks>
        /// Monochrome tiff image is compressed using the compression method specified in the 
        /// <see cref="MonochromeTiffCompression"/> property.
        /// </remarks>
        public bool MonochromeTiff
        {
            get { return monochromeTiff; }
            set { monochromeTiff = value; }
        }

        /// <summary>
        /// Gets or sets the compression method for a monochrome TIFF image.
        /// </summary>
        /// <remarks>
        /// This property is used only when exporting to TIFF image, and the <see cref="MonochromeTiff"/> property
        /// is set to <b>true</b>. 
        /// <para/>The valid values for this property are: <b>EncoderValue.CompressionNone</b>, 
        /// <b>EncoderValue.CompressionLZW</b>, <b>EncoderValue.CompressionRle</b>, 
        /// <b>EncoderValue.CompressionCCITT3</b>, <b>EncoderValue.CompressionCCITT4</b>. 
        /// The default compression method is CCITT4.
        /// </remarks>
        public EncoderValue MonochromeTiffCompression
        {
            get { return monochromeTiffCompression; }
            set { monochromeTiffCompression = value; }
        }

        /// <summary>
        /// Sets padding in non separate pages
        /// </summary>
        public int PaddingNonSeparatePages
        {
            get { return paddingNonSeparatePages; }
            set { paddingNonSeparatePages = value; }
        }

        private bool IsMultiFrameTiff
        {
            get { return ImageFormat == ImageExportFormat.Tiff && MultiFrameTiff; }
        }
        #endregion

        #region Private Methods
        private System.Drawing.Image CreateImage(int width, int height, string suffix)
        {
            widthK = width;
            if (ImageFormat == ImageExportFormat.Metafile)
                return CreateMetafile(suffix);
            return new Bitmap(width, height);
        }

        private System.Drawing.Image CreateMetafile(string suffix)
        {
            string extension = Path.GetExtension(FileName);
            string fileName = Path.ChangeExtension(FileName, suffix + extension);

            System.Drawing.Image image;
            using (Bitmap bmp = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                IntPtr hdc = g.GetHdc();
                if (suffix == "")
                    image = new Metafile(Stream, hdc);
                else
                {
                    image = new Metafile(fileName, hdc);
                    if (!GeneratedFiles.Contains(fileName))
                        GeneratedFiles.Add(fileName);
                }
                g.ReleaseHdc(hdc);
            }
            return image;
        }

        private Bitmap ConvertToBitonal(Bitmap original)
        {
            Bitmap source = null;

            // If original bitmap is not already in 32 BPP, ARGB format, then convert
            if (original.PixelFormat != PixelFormat.Format32bppArgb)
            {
                source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
                source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
                using (Graphics g = Graphics.FromImage(source))
                {
                    g.DrawImageUnscaled(original, 0, 0);
                }
            }
            else
            {
                source = original;
            }

            // Lock source bitmap in memory
            BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Copy image data to binary array
            int imageSize = sourceData.Stride * sourceData.Height;
            byte[] sourceBuffer = new byte[imageSize];
            Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

            // Unlock source bitmap
            source.UnlockBits(sourceData);

            // Create destination bitmap
            Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

            // Lock destination bitmap in memory
            BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

            // Create destination buffer
            imageSize = destinationData.Stride * destinationData.Height;
            byte[] destinationBuffer = new byte[imageSize];

            int sourceIndex = 0;
            int destinationIndex = 0;
            int pixelTotal = 0;
            byte destinationValue = 0;
            int pixelValue = 128;
            int height = source.Height;
            int width = source.Width;
            int threshold = 500;

            // Iterate lines
            for (int y = 0; y < height; y++)
            {
                sourceIndex = y * sourceData.Stride;
                destinationIndex = y * destinationData.Stride;
                destinationValue = 0;
                pixelValue = 128;

                // Iterate pixels
                for (int x = 0; x < width; x++)
                {
                    // Compute pixel brightness (i.e. total of Red, Green, and Blue values)
                    pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
                    if (pixelTotal > threshold)
                    {
                        destinationValue += (byte)pixelValue;
                    }
                    if (pixelValue == 1)
                    {
                        destinationBuffer[destinationIndex] = destinationValue;
                        destinationIndex++;
                        destinationValue = 0;
                        pixelValue = 128;
                    }
                    else
                    {
                        pixelValue >>= 1;
                    }
                    sourceIndex += 4;
                }
                if (pixelValue != 128)
                {
                    destinationBuffer[destinationIndex] = destinationValue;
                }
            }

            // Copy binary image data to destination bitmap
            Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

            // Unlock destination bitmap
            destination.UnlockBits(destinationData);

            // Dispose of source if not originally supplied bitmap
            if (source != original)
            {
                source.Dispose();
            }

            // Return
            destination.SetResolution(ResolutionX, ResolutionY);
            return destination;
        }

        private void SaveImage(System.Drawing.Image image, string suffix)
        {
            // store the resolution in output file.
            // Call this method after actual draw because it may affect drawing the text
            if (image is Bitmap)
                (image as Bitmap).SetResolution(ResolutionX, ResolutionY);
            if (IsMultiFrameTiff)
            {
                // select the image encoder
                ImageCodecInfo info = ExportUtils.GetCodec("image/tiff");
                EncoderParameters ep = new EncoderParameters(2);
                ep.Param[0] = new EncoderParameter(Encoder.Compression, MonochromeTiff ?
                  (long)MonochromeTiffCompression : (long)EncoderValue.CompressionLZW);

                if (image == masterTiffImage)
                {
                    // save the master bitmap
                    if (MonochromeTiff)
                        masterTiffImage = ConvertToBitonal(image as Bitmap);
                    ep.Param[1] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
                    masterTiffImage.Save(Stream, info, ep);
                }
                else
                {
                    // save the frame
                    if (MonochromeTiff)
                    {
                        System.Drawing.Image oldImage = image;
                        image = ConvertToBitonal(image as Bitmap);
                        oldImage.Dispose();
                    }
                    ep.Param[1] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
                    masterTiffImage.SaveAdd(image, ep);
                }
            }
            else if (ImageFormat != ImageExportFormat.Metafile)
            {
                string extension = Path.GetExtension(FileName);
                string fileName = Path.ChangeExtension(FileName, suffix + extension);
                // empty suffix means that we should use the Stream that was created in the ExportBase
                Stream stream = suffix == "" ? Stream : new FileStream(fileName, FileMode.Create);
                if (suffix != "")
                    GeneratedFiles.Add(fileName);

                if (ImageFormat == ImageExportFormat.Jpeg)
                    ExportUtils.SaveJpeg(image, stream, JpegQuality);
                else if (ImageFormat == ImageExportFormat.Tiff && MonochromeTiff)
                {
                    // handle monochrome tiff separately
                    ImageCodecInfo info = ExportUtils.GetCodec("image/tiff");
                    EncoderParameters ep = new EncoderParameters();
                    ep.Param[0] = new EncoderParameter(Encoder.Compression, (long)MonochromeTiffCompression);

                    using (Bitmap bwImage = ConvertToBitonal(image as Bitmap))
                    {
                        bwImage.Save(stream, info, ep);
                    }
                }
                else
                {
                    ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                    switch (ImageFormat)
                    {
                        case ImageExportFormat.Gif:
                            format = System.Drawing.Imaging.ImageFormat.Gif;
                            break;

                        case ImageExportFormat.Png:
                            format = System.Drawing.Imaging.ImageFormat.Png;
                            break;

                        case ImageExportFormat.Tiff:
                            format = System.Drawing.Imaging.ImageFormat.Tiff;
                            break;
                    }
                    image.Save(stream, format);
                }

                if (suffix != "")
                    stream.Dispose();
            }

            if (image != masterTiffImage)
                image.Dispose();
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override string GetFileFilter()
        {
            string filter = ImageFormat.ToString();
            return Res.Get("FileFilters," + filter + "File");
        }

        /// <inheritdoc/>
        protected override void Start()
        {
            base.Start();

            //init
            pageNumber = 0;
            height = 0;
            width = 0;
            image = null;
            g = null;
            zoomX = 1;
            zoomY = 1;
            state = null;

            curOriginY = 0;
            firstPage = true;

            if (!SeparateFiles && !IsMultiFrameTiff)
            {
                // create one big image. To do this, calculate max width and sum of pages height
                float w = 0;
                float h = 0;

                foreach (int pageNo in Pages)
                {
                    SizeF size = Report.PreparedPages.GetPageSize(pageNo);
                    if (size.Width > w)
                        w = size.Width;
                    h += size.Height + paddingNonSeparatePages * 2;
                }

                w += paddingNonSeparatePages * 2;

                bigImage = CreateImage((int)(w * ResolutionX / 96f), (int)(h * ResolutionY / 96f), "");
                bigGraphics = Graphics.FromImage(bigImage);
                bigGraphics.Clear(Color.Transparent);
            }
            pageNumber = 0;
        }


        /// <inheritdoc/>
        protected override void ExportPageBegin(ReportPage page)
        {
            base.ExportPageBegin(page);
            zoomX = ResolutionX / 96f;
            zoomY = ResolutionY / 96f;
            width = (int)(ExportUtils.GetPageWidth(page) * Units.Millimeters * zoomX);
            height = (int)(ExportUtils.GetPageHeight(page) * Units.Millimeters * zoomY);
            int suffixDigits = Pages[Pages.Length - 1].ToString().Length;
            fileSuffix = firstPage ? "" : (pageNumber + 1).ToString("".PadLeft(suffixDigits, '0'));
            if (SeparateFiles || IsMultiFrameTiff)
            {
                image = CreateImage(width, height, fileSuffix);
                if (IsMultiFrameTiff && masterTiffImage == null)
                    masterTiffImage = image;
            }
            else
                image = bigImage;

            if (bigGraphics != null)
                g = bigGraphics;
            else
                g = Graphics.FromImage(image);

            state = g.Save(); 

            g.FillRegion(Brushes.Transparent, new Region(new RectangleF(0, curOriginY, width, height)));
            if (bigImage != null && curOriginY + height * 2 > bigImage.Height)
                page.Fill.Draw(new FRPaintEventArgs(g, 1, 1, Report.GraphicCache), new RectangleF(0, curOriginY, widthK, bigImage.Height - curOriginY));
            else
                page.Fill.Draw(new FRPaintEventArgs(g, 1, 1, Report.GraphicCache), new RectangleF(0, curOriginY, widthK, height + paddingNonSeparatePages * 2));


            if (image == bigImage)
            {
                if (ImageFormat != ImageExportFormat.Metafile)
                    g.TranslateTransform(image.Width / 2 - width / 2 + page.LeftMargin * Units.Millimeters * zoomX,
                    curOriginY + paddingNonSeparatePages + page.TopMargin * Units.Millimeters * zoomY);
                else
                    g.TranslateTransform(widthK / 2 - width / 2 + page.LeftMargin * Units.Millimeters * zoomX,
                    curOriginY + paddingNonSeparatePages + page.TopMargin * Units.Millimeters * zoomY);

            }
            else
                g.TranslateTransform(page.LeftMargin * Units.Millimeters * zoomX, page.TopMargin * Units.Millimeters * zoomY);

            g.ScaleTransform(1, zoomY / zoomX);

            // export bottom watermark
            if (page.Watermark.Enabled && !page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && !page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);
        }

        /// <inheritdoc/>
        protected override void ExportBand(Base band)
        {
            base.ExportBand(band);
            ExportObj(band);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                if (!(c is Table.TableColumn || c is Table.TableCell || c is Table.TableRow))
                    ExportObj(c);
            }
        }

        private void ExportObj(Base obj)
        {
            if (obj is ReportComponentBase && (obj as ReportComponentBase).Exportable)
                (obj as ReportComponentBase).Draw(new FRPaintEventArgs(g, zoomX, zoomX, Report.GraphicCache));
        }

        /// <inheritdoc/>
        protected override void ExportPageEnd(ReportPage page)
        {
            // export top watermark
            if (page.Watermark.Enabled && page.Watermark.ShowImageOnTop)
                AddImageWatermark(page);
            if (page.Watermark.Enabled && page.Watermark.ShowTextOnTop)
                AddTextWatermark(page);

            g.Restore(state);
            if (g != bigGraphics)
                g.Dispose();
            if (SeparateFiles || IsMultiFrameTiff)
                SaveImage(image, fileSuffix);
            else
                curOriginY += height + paddingNonSeparatePages * 2;
            firstPage = false;
            pageNumber++;
        }

        private void AddImageWatermark(ReportPage page)
        {
            page.Watermark.DrawImage(new FRPaintEventArgs(g, zoomX, zoomX, Report.GraphicCache),
                new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, width / zoomX, height / zoomY),
                page.Report, false);
        }

        private void AddTextWatermark(ReportPage page)
        {
            if (string.IsNullOrEmpty(page.Watermark.Text))
                return;
            page.Watermark.DrawText(new FRPaintEventArgs(g, zoomX, zoomX, Report.GraphicCache),
                new RectangleF(-page.LeftMargin * Units.Millimeters, -page.TopMargin * Units.Millimeters, width / zoomX, height / zoomY),
                page.Report, false);
        }

        /// <inheritdoc/>
        protected override void Finish()
        {
            if (IsMultiFrameTiff)
            {
                // close the file.
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
                masterTiffImage.SaveAdd(ep);
            }
            else if (!SeparateFiles)
            {
                bigGraphics.Dispose();
                bigGraphics = null;
                SaveImage(bigImage, "");
            }
            if (masterTiffImage != null)
            {
                masterTiffImage.Dispose();
                masterTiffImage = null;
            }
        }
        #endregion

        #region Public Methods


        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            base.Serialize(writer);
            writer.WriteValue("ImageFormat", ImageFormat);
            writer.WriteBool("SeparateFiles", SeparateFiles);
            writer.WriteInt("ResolutionX", ResolutionX);
            writer.WriteInt("ResolutionY", ResolutionY);
            writer.WriteInt("JpegQuality", JpegQuality);
            writer.WriteBool("MultiFrameTiff", MultiFrameTiff);
            writer.WriteBool("MonochromeTiff", MonochromeTiff);
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="ImageExport"/> class.
        /// </summary>
        public ImageExport()
        {
            paddingNonSeparatePages = 10;
            fileSuffix = String.Empty;
            HasMultipleFiles = true;
            imageFormat = ImageExportFormat.Jpeg;
            separateFiles = true;
            Resolution = 96;
            jpegQuality = 100;
            monochromeTiffCompression = EncoderValue.CompressionCCITT4;
        }
    }
}
