using System;
using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;
using FastReport.Utils;
using System.Windows.Forms;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Represents a Picture object that can display pictures.
    /// </summary>
    /// <remarks>
    /// The Picture object can display the following kind of pictures:
    /// <list type="bullet">
    ///   <item>
    ///     <description>picture that is embedded in the report file. Use the <see cref="Image"/>
    ///     property to do this;</description>
    ///   </item>
    ///   <item>
    ///     <description>picture that is stored in the database BLOb field. Use the <see cref="DataColumn"/>
    ///     property to specify the name of data column you want to show;</description>
    ///   </item>
    ///   <item>
    ///     <description>picture that is stored in the local disk file. Use the <see cref="ImageLocation"/>
    ///     property to specify the name of the file;</description>
    ///   </item>
    ///   <item>
    ///     <description>picture that is stored in the Web. Use the <see cref="ImageLocation"/>
    ///     property to specify the picture's URL.</description>
    ///   </item>
    /// </list>
    /// <para/>Use the <see cref="SizeMode"/> property to specify a size mode. The <see cref="MaxWidth"/>
    /// and <see cref="MaxHeight"/> properties can be used to restrict the image size if <b>SizeMode</b>
    /// is set to <b>AutoSize</b>.
    /// <para/>The <see cref="TransparentColor"/> property can be used to display an image with
    /// transparent background. Use the <see cref="Transparency"/> property if you want to display
    /// semi-transparent image.
    /// </remarks>
    public partial class PictureObject : PictureObjectBase
    {
        #region Fields
        private Image image;

        private int imageIndex;

        private Color transparentColor;
        private float transparency;
        private bool tile;
        private Bitmap transparentImage;
        private byte[] imageData;
        private bool shouldDisposeImage;
        private Bitmap grayscaleBitmap;
        private int grayscaleHash;
        private ImageFormat imageFormat;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <remarks>
        /// By default, image that you assign to this property is never disposed - you should
        /// take care about it. If you want to dispose the image when this <b>PictureObject</b> is disposed,
        /// set the <see cref="ShouldDisposeImage"/> property to <b>true</b> right after you assign an image:
        /// <code>
        /// myPictureObject.Image = new Bitmap("file.bmp");
        /// myPictureObject.ShouldDisposeImage = true;
        /// </code>
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ImageEditor, FastReport", typeof(UITypeEditor))]
        public virtual Image Image
        {
            get { return image; }
            set
            {
                image = value;
                imageData = null;
                UpdateAutoSize();
                UpdateTransparentImage();
                ResetImageIndex();
                imageFormat = CheckImageFormat();
                ShouldDisposeImage = false;
            }
        }

        /// <summary>
        /// Gets or sets the extension of image.
        /// </summary>
        [Category("Data")]
        public virtual ImageFormat ImageFormat
        {
            get { return imageFormat; }
            set
            {
                if (image == null)
                    return;
                bool wasC = false;
                using (MemoryStream stream = new MemoryStream())
                {
                    wasC = ImageHelper.SaveAndConvert(Image, stream, value);
                    imageData = stream.ToArray();
                }
                if (!wasC)
                    return;
                ForceLoadImage();
                imageFormat = CheckImageFormat();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the image should be displayed in grayscale mode.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        public override bool Grayscale
        {
            get { return base.Grayscale; }
            set
            {
                base.Grayscale = value;
                if (!value && grayscaleBitmap != null)
                {
                    grayscaleBitmap.Dispose();
                    grayscaleBitmap = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a hash of grayscale svg image
        /// </summary>
        [Browsable(false)]
        public int GrayscaleHash
        {
            get { return grayscaleHash; }
            set { grayscaleHash = value; }
        }

        /// <summary>
        /// Gets or sets the color of the image that will be treated as transparent.
        /// </summary>
        [Category("Appearance")]
        [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
        public Color TransparentColor
        {
            get { return transparentColor; }
            set
            {
                transparentColor = value;
                UpdateTransparentImage();
            }
        }

        /// <summary>
        /// Gets or sets the transparency of the PictureObject.
        /// </summary>
        /// <remarks>
        /// Valid range of values is 0..1. Default value is 0.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Appearance")]
        public float Transparency
        {
            get { return transparency; }
            set
            {
                if (value < 0)
                    value = 0;
                if (value > 1)
                    value = 1;
                transparency = value;
                UpdateTransparentImage();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating that the image should be tiled.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        public bool Tile
        {
            get { return tile; }
            set { tile = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the image stored in the <see cref="Image"/> 
        /// property should be disposed when this object is disposed.
        /// </summary>
        /// <remarks>
        /// By default, image assigned to the <see cref="Image"/> property is never disposed - you should
        /// take care about it. If you want to dispose the image when this <b>PictureObject</b> is disposed,
        /// set this property to <b>true</b> right after you assign an image to the <see cref="Image"/> property.
        /// </remarks>
        [Browsable(false)]
        public bool ShouldDisposeImage
        {
            get { return shouldDisposeImage; }
            set { shouldDisposeImage = value; }
        }

        /// <summary>
        /// Gets or sets a bitmap transparent image
        /// </summary>
        [Browsable(false)]
        public Bitmap TransparentImage
        {
            get { return transparentImage; }
            set { transparentImage = value; }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        protected override float ImageWidth
        {
            get
            {
                if (Image == null) return 0;
                return Image.Width;
            }
        }

        /// <inheritdoc/>
        [Browsable(false)]
        protected override float ImageHeight
        {
            get
            {
                if (Image == null) return 0;
                return Image.Height;
            }
        }
        #endregion

        #region Private Methods
        private ImageFormat CheckImageFormat()
        {
            if (Image == null || Image.RawFormat == null)
                return null;
            ImageFormat format = null;
            if (ImageFormat.Jpeg.Equals(image.RawFormat))
            {
                format = ImageFormat.Jpeg;
            }
            else if (ImageFormat.Gif.Equals(image.RawFormat))
            {
                format = ImageFormat.Gif;
            }
            else if (ImageFormat.Png.Equals(image.RawFormat))
            {
                format = ImageFormat.Png;
            }
            else if (ImageFormat.Emf.Equals(image.RawFormat))
            {
                format = ImageFormat.Emf;
            }
            else if (ImageFormat.Icon.Equals(image.RawFormat))
            {
                format = ImageFormat.Icon;
            }
            else if (ImageFormat.Tiff.Equals(image.RawFormat))
            {
                format = ImageFormat.Tiff;
            }
            else if (ImageFormat.Bmp.Equals(image.RawFormat) || ImageFormat.MemoryBmp.Equals(image.RawFormat))
            {
                format = ImageFormat.Bmp;
            }
            else if (ImageFormat.Wmf.Equals(image.RawFormat))
            {
                format = ImageFormat.Wmf;
            }
            if (format != null)
                return format;
            return ImageFormat.Bmp;
        }

        private void UpdateTransparentImage()
        {
            if (transparentImage != null)
                transparentImage.Dispose();
            transparentImage = null;
            if (Image is Bitmap)
            {
                if (TransparentColor != Color.Transparent)
                {
                    transparentImage = new Bitmap(Image);
                    transparentImage.MakeTransparent(TransparentColor);
                }
                else if (Transparency != 0)
                {
                    transparentImage = ImageHelper.GetTransparentBitmap(Image, Transparency);
                }
            }
        }

#if MONO
        private GraphicsPath GetRoundRectPath(RectangleF rectangleF, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            if (radius < 1)
                radius = 1;
            gp.AddLine(rectangleF.X + radius, rectangleF.Y, rectangleF.Width + rectangleF.X - radius, rectangleF.Y);
            gp.AddArc(rectangleF.Width + rectangleF.X - radius - 1, rectangleF.Y, radius + 1, radius + 1, 270, 90);
            gp.AddLine(rectangleF.Width + rectangleF.X, rectangleF.Y + radius, rectangleF.Width + rectangleF.X, rectangleF.Height + rectangleF.Y - radius);
            gp.AddArc(rectangleF.Width + rectangleF.X - radius - 1, rectangleF.Height + rectangleF.Y - radius - 1, radius + 1, radius + 1, 0, 90);
            gp.AddLine(rectangleF.Width + rectangleF.X - radius, rectangleF.Height + rectangleF.Y, rectangleF.X + radius, rectangleF.Height + rectangleF.Y);
            gp.AddArc(rectangleF.X, rectangleF.Height + rectangleF.Y - radius - 1, radius + 1, radius + 1, 90, 90);
            gp.AddLine(rectangleF.X, rectangleF.Height + rectangleF.Y - radius, rectangleF.X, rectangleF.Y + radius);
            gp.AddArc(rectangleF.X, rectangleF.Y, radius, radius, 180, 90);
            gp.CloseFigure();
            return gp;
        }
#else
        private GraphicsPath GetRoundRectPath(RectangleF rectangleF, float radius)
        {
            GraphicsPath gp = new GraphicsPath();
            if (radius < 1)
                radius = 1;
            gp.AddArc(rectangleF.Width + rectangleF.X - radius - 1, rectangleF.Y, radius + 1, radius + 1, 270, 90);
            gp.AddArc(rectangleF.Width + rectangleF.X - radius - 1, rectangleF.Height + rectangleF.Y - radius - 1, radius + 1, radius + 1, 0, 90);
            gp.AddArc(rectangleF.X, rectangleF.Height + rectangleF.Y - radius - 1, radius + 1, radius + 1, 90, 90);
            gp.AddArc(rectangleF.X, rectangleF.Y, radius, radius, 180, 90);
            gp.CloseFigure();
            return gp;
        }
#endif
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                DisposeImage();
            base.Dispose(disposing);
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            PictureObject src = source as PictureObject;
            if (src != null)
            {
                TransparentColor = src.TransparentColor;
                Transparency = src.Transparency;
                Tile = src.Tile;
                Image = src.Image == null ? null : src.Image.Clone() as Image;
                if (src.Image == null && src.imageData != null)
                    imageData = src.imageData;
                ShouldDisposeImage = true;
                ImageFormat = src.ImageFormat;
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        public override void DrawImage(FRPaintEventArgs e)
        {
            IGraphics g = e.Graphics;
            if (Image == null)
                ForceLoadImage();

            if (Image == null)
            {
                DrawErrorImage(g, e);
                return;
            }

            float drawLeft = (AbsLeft + Padding.Left) * e.ScaleX;
            float drawTop = (AbsTop + Padding.Top) * e.ScaleY;
            float drawWidth = (Width - Padding.Horizontal) * e.ScaleX;
            float drawHeight = (Height - Padding.Vertical) * e.ScaleY;

            RectangleF drawRect = new RectangleF(
              drawLeft,
              drawTop,
              drawWidth,
              drawHeight);

            GraphicsPath path = new GraphicsPath();
            IGraphicsState state = g.Save();
            try
            {
                //if (Config.IsRunningOnMono) // strange behavior of mono - we need to reset clip before we set new one
                g.ResetClip();

                EstablishImageForm(path, drawLeft, drawTop, drawWidth, drawHeight);

                g.SetClip(path, CombineMode.Replace);
                Report report = Report;
                if (report != null && report.SmoothGraphics)
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                }

                if (!Tile)
                    DrawImageInternal(e, drawRect);
                else
                {
                    float y = drawRect.Top;
                    float width = Image.Width * e.ScaleX;
                    float height = Image.Height * e.ScaleY;
                    while (y < drawRect.Bottom)
                    {
                        float x = drawRect.Left;
                        while (x < drawRect.Right)
                        {
                            if (transparentImage != null)
                                g.DrawImage(transparentImage, x, y, width, height);
                            else
                                g.DrawImage(Image, x, y, width, height);
                            x += width;
                        }
                        y += height;
                    }
                }
            }
            finally
            {
                g.Restore(state);
                g.ResetClip();
#if !SKIA
                path.Dispose(); 
#else
                path = null;
#endif    
            }

            if (IsPrinting)
            {
                DisposeImage();
            }
        }

        protected override void DrawImageInternal2(IGraphics graphics, PointF upperLeft, PointF upperRight, PointF lowerLeft)
        {
            Image image = transparentImage != null ? transparentImage : Image;
            if (image == null)
                return;
            if (Grayscale)
            {
                if (grayscaleHash != image.GetHashCode() || grayscaleBitmap == null)
                {
                    if (grayscaleBitmap != null)
                        grayscaleBitmap.Dispose();
                    grayscaleBitmap = ImageHelper.GetGrayscaleBitmap(image);
                    grayscaleHash = image.GetHashCode();
                }

                image = grayscaleBitmap;
            }

            //graphics.DrawImage(image, new PointF[] { upperLeft, upperRight, lowerLeft });

            DrawImage3Points(graphics, image, upperLeft, upperRight, lowerLeft);
        }

        // This is analogue of graphics.DrawImage(image, PointF[] points) method. 
        // The original gdi+ method does not work properly in mono on linux/macos.
        private void DrawImage3Points(IGraphics g, Image image, PointF p0, PointF p1, PointF p2)
        {
            // Skip drawing image, when height or width of the image equal zero.
            if (image == null || image.Width == 0 || image.Height == 0)
                return;
            // Skip drawing image, when height or width of the parallelogram for drawing equal zero.
            if (p0 == p1 || p0 == p2)
                return;

            RectangleF rect = new RectangleF(0, 0, image.Width, image.Height);
            float m11 = (p1.X - p0.X) / rect.Width;
            float m12 = (p1.Y - p0.Y) / rect.Width;
            float m21 = (p2.X - p0.X) / rect.Height;
            float m22 = (p2.Y - p0.Y) / rect.Height;
            g.MultiplyTransform(new System.Drawing.Drawing2D.Matrix(m11, m12, m21, m22, p0.X, p0.Y), MatrixOrder.Prepend);
            g.DrawImage(image, rect);
        }

        /// <summary>
        /// Sets image data to FImageData
        /// </summary>
        /// <param name="data"></param>
        public void SetImageData(byte[] data)
        {
            imageData = data;
            // if autosize is on, load the image.
            if (SizeMode == PictureBoxSizeMode.AutoSize)
                ForceLoadImage();
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            PictureObject c = writer.DiffObject as PictureObject;
            base.Serialize(writer);

#if PRINT_HOUSE
      writer.WriteStr("ImageLocation", ImageLocation);
#endif
            if (TransparentColor != c.TransparentColor)
                writer.WriteValue("TransparentColor", TransparentColor);
            if (FloatDiff(Transparency, c.Transparency))
                writer.WriteFloat("Transparency", Transparency);
            if (Tile != c.Tile)
                writer.WriteBool("Tile", Tile);
            if (ImageFormat != c.ImageFormat)
                writer.WriteValue("ImageFormat", ImageFormat);
            // store image data
            if (writer.SerializeTo != SerializeTo.SourcePages)
            {
                if (writer.SerializeTo == SerializeTo.Preview ||
                    (String.IsNullOrEmpty(ImageLocation) && String.IsNullOrEmpty(DataColumn)) ||

                    // Next condition should work when serializing to Undo and only when designing the prepared page.
                    (writer.SerializeTo == SerializeTo.Undo && IsDesigningInPreviewPageDesigner())
                   )
                {
                    if (writer.BlobStore != null)
                    {
                        // check FImageIndex >= writer.BlobStore.Count is needed when we close the designer
                        // and run it again, the BlobStore is empty, but FImageIndex is pointing to
                        // previous BlobStore item and is not -1.
                        if (imageIndex == -1 || imageIndex >= writer.BlobStore.Count)
                        {
                            byte[] bytes = imageData;
                            if (bytes == null)
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    ImageHelper.Save(Image, stream, imageFormat);
                                    bytes = stream.ToArray();
                                }
                            }
                            if (bytes != null)
                            {
                                string imgHash = BitConverter.ToString(new Murmur3().ComputeHash(bytes));
                                imageIndex = writer.BlobStore.AddOrUpdate(bytes, imgHash);
                            }
                        }
                    }
                    else
                    {
                        if (Image == null && imageData != null)
                            writer.WriteStr("Image", Convert.ToBase64String(imageData));
                        else if (!writer.AreEqual(Image, c.Image))
                            writer.WriteValue("Image", Image);
                    }

                    if (writer.BlobStore != null || writer.SerializeTo == SerializeTo.Undo)
                        writer.WriteInt("ImageIndex", imageIndex);
                }
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader)
        {
            base.Deserialize(reader);
            if (reader.HasProperty("ImageIndex"))
            {
                imageIndex = reader.ReadInt("ImageIndex");
                if (reader.BlobStore != null && imageIndex != -1)
                {
                    //int saveIndex = FImageIndex;
                    //Image = ImageHelper.Load(reader.BlobStore.Get(FImageIndex));
                    //FImageIndex = saveIndex;
                    SetImageData(reader.BlobStore.Get(imageIndex));
                }
            }
        }

        /// <summary>
        /// Loads image
        /// </summary>
        public override void LoadImage()
        {
            if (!String.IsNullOrEmpty(ImageLocation))
            {
                // 
                try
                {
                    Uri uri = CalculateUri();
                    if (uri.IsFile)
                        SetImageData(ImageHelper.Load(uri.LocalPath));
                    else
                        SetImageData(ImageHelper.LoadURL(uri.ToString()));
                }
                catch
                {
                    Image = null;
                }

                ShouldDisposeImage = true;
            }
        }

        /// <summary>
        /// Disposes image
        /// </summary>
        public void DisposeImage()
        {
            if (Image != null && ShouldDisposeImage)
                Image.Dispose();
            if (grayscaleBitmap != null)
                grayscaleBitmap.Dispose();
            grayscaleBitmap = null;
            Image = null;
        }

        protected override void ResetImageIndex()
        {
            imageIndex = -1;
        }

        /// <summary>
        /// The shape of the image is set using GraphicsPath
        /// </summary>
        /// <param name="path"></param>
        /// <param name="drawLeft"></param>
        /// <param name="drawTop"></param>
        /// <param name="drawWidth"></param>
        /// <param name="drawHeight"></param>
        public void EstablishImageForm(GraphicsPath path, float drawLeft, float drawTop, float drawWidth, float drawHeight)
        {
            RectangleF drawRect = new RectangleF(
              drawLeft,
              drawTop,
              drawWidth,
              drawHeight);

            switch (Shape)
            {
                case ShapeKind.Rectangle:
                    path.AddRectangle(drawRect);
                    break;
                case ShapeKind.RoundRectangle:
                    float min = Math.Min(drawWidth, drawHeight) / 4;
                    path.AddPath(GetRoundRectPath(drawRect, min), false);
                    break;
                case ShapeKind.Ellipse:
                    path.AddEllipse(drawLeft, drawTop, drawWidth, drawHeight);
                    break;
                case ShapeKind.Triangle:
                    PointF[] triPoints =
                    {
                            new PointF(drawLeft + drawWidth, drawTop + drawHeight), new PointF(drawLeft, drawTop + drawHeight),
                            new PointF(drawLeft + drawWidth / 2, drawTop), new PointF(drawLeft + drawWidth, drawTop + drawHeight)
                     };
                    path.AddPolygon(triPoints);
                    break;
                case ShapeKind.Diamond:
                    PointF[] diaPoints =
                    {
                            new PointF(drawLeft + drawWidth / 2, drawTop), new PointF(drawLeft + drawWidth, drawTop + drawHeight / 2),
                            new PointF(drawLeft + drawWidth / 2, drawTop + drawHeight), new PointF(drawLeft, drawTop + drawHeight / 2)
                    };
                    path.AddPolygon(diaPoints);
                    break;
            }
        }
#endregion

        #region Report Engine


        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            ResetImageIndex();
        }

        /// <inheritdoc/>
        public override void FinalizeComponent()
        {
            base.FinalizeComponent();
            ResetImageIndex();
        }



        /// <inheritdoc/>
        public override void GetData()
        {
            base.GetData();

            if (!String.IsNullOrEmpty(DataColumn))
            {
                // reset the image
                Image = null;
                imageData = null;

                object data = Report.GetColumnValueNullable(DataColumn);
                if (data is byte[])
                {
                    SetImageData((byte[])data);
                    return;
                }
                else if (data is Image)
                {
                    Image = data as Image;
                    return;
                }
                else if (data is string dataStr)
                {
                    SetImageLocation(dataStr, true);
                }
            }
            else
            {
                // no other data received
                UpdateImageLocation();
            }
        }

        /// <summary>
        /// Forces loading the image from a data column.
        /// </summary>
        /// <remarks>
        /// Call this method in the <b>AfterData</b> event handler to force loading an image 
        /// into the <see cref="Image"/> property. Normally, the image is stored internally as byte[] array 
        /// and never loaded into the <b>Image</b> property, to save the time. The side effect is that you 
        /// can't analyze the image properties such as width and height. If you need this, call this method
        /// before you access the <b>Image</b> property. Note that this will significantly slow down the report.
        /// </remarks>
        public void ForceLoadImage()
        {
            if (imageData == null)
                return;

            byte[] saveImageData = imageData;
            // FImageData will be reset after this line, keep it
            Image = ImageHelper.Load(imageData);
            imageData = saveImageData;
            ShouldDisposeImage = true;
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="PictureObject"/> class with default settings.
        /// </summary>
        public PictureObject()
        {
            transparentColor = Color.Transparent;
            SetFlags(Flags.HasSmartTag, true);
            ResetImageIndex();
        }

    }
}