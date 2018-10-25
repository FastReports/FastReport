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
    public partial class PictureObject : ReportComponentBase
    {
        #region Fields
        private Image image;
        private PictureBoxSizeMode sizeMode;
        private Padding padding;
        private int imageIndex;
        private string imageLocation;
        private string dataColumn;
        private float maxWidth;
        private float maxHeight;
        private Color transparentColor;
        private float transparency;
        private bool showErrorImage;
        private bool tile;
        private Bitmap transparentImage;
        private byte[] imageData;
        internal bool dragAccept;
        private bool shouldDisposeImage;
        private int angle;
        private PictureBoxSizeMode saveSizeMode;
        private bool grayscale;
        private int grayscaleHash;
        private Bitmap grayscaleBitmap;
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
                ShouldDisposeImage = false;
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies how an image is positioned within a PictureObject.
        /// </summary>
        [DefaultValue(PictureBoxSizeMode.Zoom)]
        [Category("Behavior")]
        public virtual PictureBoxSizeMode SizeMode
        {
            get { return sizeMode; }
            set
            {
                sizeMode = value;
                UpdateAutoSize();
            }
        }

        /// <summary>
        /// Gets or sets the maximum width of a Picture object, in pixels.
        /// </summary>
        /// <remarks>
        /// Use this property to restrict the object size if the <see cref="SizeMode"/> property 
        /// is set to <b>AutoSize</b>.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Layout")]
        [TypeConverterAttribute("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        /// <summary>
        /// Gets or sets the maximum height of a Picture object, in pixels.
        /// </summary>
        /// <remarks>
        /// Use this property to restrict the object size if the <see cref="SizeMode"/> property 
        /// is set to <b>AutoSize</b>.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Layout")]
        [TypeConverterAttribute("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
        }

        /// <summary>
        /// Gets or sets padding within the PictureObject.
        /// </summary>
        [Category("Layout")]
        public Padding Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        /// <summary>
        /// Gets or sets the path for the image to display in the PictureObject.
        /// </summary>
        /// <remarks>
        /// This property may contain the path to the image file as well as external URL.
        /// </remarks>
        [Category("Data")]
        public string ImageLocation
        {
            get { return imageLocation; }
            set
            {
                if (!String.IsNullOrEmpty(Config.ReportSettings.ImageLocationRoot))
                    imageLocation = value.Replace(Config.ReportSettings.ImageLocationRoot, "");
                else
                    imageLocation = value;
                LoadImage();
                ResetImageIndex();
            }
        }

        /// <summary>
        /// Gets or sets the data column name to get the image from.
        /// </summary>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.DataColumnEditor, FastReport", typeof(UITypeEditor))]
        public string DataColumn
        {
            get { return dataColumn; }
            set { dataColumn = value; }
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
        /// Gets or sets a value indicating whether the PictureObject should display 
        /// the error indicator if there is no image in it.
        /// </summary>
        [DefaultValue(false)]
        [Category("Behavior")]
        public bool ShowErrorImage
        {
            get { return showErrorImage; }
            set { showErrorImage = value; }
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
        /// Gets or sets the image rotation angle, in degrees. Possible values are 0, 90, 180, 270.
        /// </summary>
        [DefaultValue(0)]
        [Category("Appearance")]
        public int Angle
        {
            get { return angle; }
            set { angle = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating that the image should be displayed in grayscale mode.
        /// </summary>
        [DefaultValue(false)]
        [Category("Appearance")]
        public virtual bool Grayscale
        {
            get { return grayscale; }
            set
            {
                grayscale = value;
                if (!grayscale && grayscaleBitmap != null)
                {
                    grayscaleBitmap.Dispose();
                    grayscaleBitmap = null;
                }
            }
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

        /// <inheritdoc/>
        public override float Width
        {
            get { return base.Width; }
            set
            {
                if (MaxWidth != 0 && value > MaxWidth)
                    value = MaxWidth;
                base.Width = value;
            }
        }

        /// <inheritdoc/>
        public override float Height
        {
            get { return base.Height; }
            set
            {
                if (MaxHeight != 0 && value > MaxHeight)
                    value = MaxHeight;
                base.Height = value;
            }
        }

        /// <summary>
        /// Gets a value indicating that the image stored in the separate file
        /// </summary>
        [Browsable(false)]
        public bool IsFileLocation
        {
            get
            {
                if (String.IsNullOrEmpty(ImageLocation))
                    return false;
                Uri uri = CalculateUri();
                return uri.IsFile;
            }
        }

        /// <summary>
        /// Gets a value indicating that the image stored in the Web
        /// </summary>
        [Browsable(false)]
        public bool IsWebLocation
        {
            get
            {
                if (String.IsNullOrEmpty(ImageLocation))
                    return false;
                Uri uri = CalculateUri();
                return !uri.IsFile;
            }
        }

        /// <summary>
        /// Gets a value indicating that the image stored in the databases column
        /// </summary>
        [Browsable(false)]
        public bool IsDataColumn
        {
            get { return !String.IsNullOrEmpty(DataColumn); }
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

        /// <summary>
        /// Gets or sets a hash of grayscale svg image
        /// </summary>
        [Browsable(false)]
        public int GrayscaleHash
        {
            get { return grayscaleHash; }
            set { grayscaleHash = value; }
        }
        #endregion

        #region Private Methods
        private void UpdateAutoSize()
        {
            if (SizeMode == PictureBoxSizeMode.AutoSize)
            {
                if (Image == null || Image.Width == 0 || Image.Height == 0)
                {
                    if (IsRunning)
                    {
                        Width = 0;
                        Height = 0;
                    }
                }
                else
                {
                    //bool rotate = Angle == 90 || Angle == 270;
                    //Width = (rotate ? Image.Height : Image.Width) + Padding.Horizontal;
                    //Height = (rotate ? Image.Width : Image.Height) + Padding.Vertical;

                    PointF[] p = new PointF[4];
                    p[0] = new PointF(-Image.Width / 2, -Image.Height / 2);
                    p[1] = new PointF(Image.Width / 2, -Image.Height / 2);
                    p[2] = new PointF(Image.Width / 2, Image.Height / 2);
                    p[3] = new PointF(-Image.Width / 2, Image.Height / 2);

                    float minX = float.MaxValue;
                    float maxX = float.MinValue;
                    float minY = float.MaxValue;
                    float maxY = float.MinValue;
                    for (int i = 0; i < 4; i++)
                    {
                        p[i] = rotateVector(p[i], Angle);
                        if (minX > p[i].X) minX = p[i].X;
                        if (maxX < p[i].X) maxX = p[i].X;
                        if (minY > p[i].Y) minY = p[i].Y;
                        if (maxY < p[i].Y) maxY = p[i].Y;
                    }

                    Width = maxX - minX;
                    Height = maxY - minY;


                    // if width/height restrictions are set, use zoom mode to keep aspect ratio
                    if (IsRunning && (MaxWidth != 0 || MaxHeight != 0))
                        SizeMode = PictureBoxSizeMode.Zoom;
                }
            }
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
                SizeMode = src.SizeMode;
                MaxWidth = src.MaxWidth;
                MaxHeight = src.MaxHeight;
                Padding = src.Padding;
                ImageLocation = src.ImageLocation;
                DataColumn = src.DataColumn;
                TransparentColor = src.TransparentColor;
                Transparency = src.Transparency;
                ShowErrorImage = src.ShowErrorImage;
                Tile = src.Tile;
                Angle = src.Angle;
                Grayscale = src.Grayscale;
                Image = src.Image == null ? null : src.Image.Clone() as Image;
                if (src.Image == null && src.imageData != null)
                    imageData = src.imageData;
                ShouldDisposeImage = true;
            }
        }

        /// <summary>
        /// Draws the image.
        /// </summary>
        /// <param name="e">Paint event args.</param>
        public virtual void DrawImage(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
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

            GraphicsState state = g.Save();
            try
            {
                g.SetClip(drawRect);
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
            }

            if (IsPrinting)
            {
                DisposeImage();
            }
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
        public override void Draw(FRPaintEventArgs e)
        {
            UpdateAutoSize();
            base.Draw(e);
            DrawImage(e);
            DrawMarkers(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            DrawDesign(e);
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            PictureObject c = writer.DiffObject as PictureObject;
            base.Serialize(writer);

#if PRINT_HOUSE
      writer.WriteStr("ImageLocation", ImageLocation);
#endif

            if (SizeMode != c.SizeMode)
                writer.WriteValue("SizeMode", SizeMode);
            if (FloatDiff(MaxWidth, c.MaxWidth))
                writer.WriteFloat("MaxWidth", MaxWidth);
            if (FloatDiff(MaxHeight, c.MaxHeight))
                writer.WriteFloat("MaxHeight", MaxHeight);
            if (Padding != c.Padding)
                writer.WriteValue("Padding", Padding);
            if (writer.SerializeTo != SerializeTo.Preview && writer.SerializeTo != SerializeTo.SourcePages &&
              ImageLocation != c.ImageLocation)
                writer.WriteStr("ImageLocation", ImageLocation);
            if (DataColumn != c.DataColumn)
                writer.WriteStr("DataColumn", DataColumn);
            if (TransparentColor != c.TransparentColor)
                writer.WriteValue("TransparentColor", TransparentColor);
            if (FloatDiff(Transparency, c.Transparency))
                writer.WriteFloat("Transparency", Transparency);
            if (ShowErrorImage != c.ShowErrorImage)
                writer.WriteBool("ShowErrorImage", ShowErrorImage);
            if (Tile != c.Tile)
                writer.WriteBool("Tile", Tile);
            if (Angle != c.Angle)
                writer.WriteInt("Angle", Angle);
            if (Grayscale != c.Grayscale)
                writer.WriteBool("Grayscale", Grayscale);
            // store image data
            if (writer.SerializeTo != SerializeTo.SourcePages)
            {
                if (writer.SerializeTo == SerializeTo.Preview ||
                  (String.IsNullOrEmpty(ImageLocation) && String.IsNullOrEmpty(DataColumn)))
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
                                    ImageHelper.Save(Image, stream, ImageFormat.Png);
                                    bytes = stream.ToArray();
                                }
                            }
                            imageIndex = writer.BlobStore.Add(bytes);
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
        /// Calculates URI from ImageLocation
        /// </summary>
        /// <returns></returns>
        public Uri CalculateUri()
        {
            try
            {
                return new Uri(ImageLocation);
            }
            catch (UriFormatException)
            {
                string path;
                if (!String.IsNullOrEmpty(Config.ReportSettings.ImageLocationRoot))
                    path = Path.Combine(Config.ReportSettings.ImageLocationRoot, ImageLocation.Replace('/', '\\'));
                else
                    path = Path.GetFullPath(ImageLocation);
                return new Uri(path);
            }
        }

        /// <summary>
        /// Draws not tiled image 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="drawRect"></param>
        internal virtual void DrawImageInternal(FRPaintEventArgs e, RectangleF drawRect)
        {
            if (Image == null)
                return;

            bool rotate = Angle == 90 || Angle == 270;
            float imageWidth = Image.Width;//rotate ? Image.Height : Image.Width;
            float imageHeight = Image.Height;//rotate ? Image.Width : Image.Height;


            PointF upperLeft;
            PointF upperRight;
            PointF lowerLeft;
            System.Drawing.Drawing2D.Matrix matrix = e.Graphics.Transform;
            GetImageAngleTransform(drawRect, imageWidth, imageHeight, e.ScaleX, e.ScaleY, matrix.OffsetX, matrix.OffsetY, out upperLeft, out upperRight, out lowerLeft);




            Image image = transparentImage != null ? transparentImage : Image;

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
            
            // TODO translate tranform matrix, WTF mono or coreCompat or both
            // cant work with negative transforms so need to fix it

#if NETSTANDARD2_0


            System.Drawing.Drawing2D.Matrix matrixBack = e.Graphics.Transform;

            System.Drawing.Drawing2D.Matrix matrixTemp = new System.Drawing.Drawing2D.Matrix(
                matrixBack.Elements[0],
                matrixBack.Elements[1],
                matrixBack.Elements[2],
                matrixBack.Elements[3],
                0,
                0
                );

            upperLeft.X += matrixBack.OffsetX;
            upperLeft.Y += matrixBack.OffsetY;

            upperRight.X += matrixBack.OffsetX;
            upperRight.Y += matrixBack.OffsetY;

            lowerLeft.X += matrixBack.OffsetX;
            lowerLeft.Y += matrixBack.OffsetY;

            e.Graphics.Transform = matrixTemp;
#endif

            e.Graphics.DrawImage(image, new PointF[] { upperLeft, upperRight, lowerLeft });

#if NETSTANDARD2_0
            e.Graphics.Transform = matrixBack;
#endif
        }

        /// <summary>
        /// gets points for transform this image
        /// </summary>
        /// <param name="drawRect">the box where to draw image</param>
        /// <param name="imageWidth">image width</param>
        /// <param name="imageHeight">image height</param>
        /// <param name="scaleX">scale horizontal</param>
        /// <param name="scaleY">scale vertical</param>
        /// <param name="offsetX">offset of left</param>
        /// <param name="offsetY">offset of top</param>
        /// <param name="upperLeft">out start of vectors</param>
        /// <param name="upperRight">out end of frist vector</param>
        /// <param name="lowerLeft">out end of second vector</param>
        public void GetImageAngleTransform(RectangleF drawRect, float imageWidth, float imageHeight, float scaleX, float scaleY, float offsetX, float offsetY, out PointF upperLeft, out PointF upperRight, out PointF lowerLeft)
        {
            RectangleF rect = drawRect;
            switch (SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                case PictureBoxSizeMode.AutoSize:
                    rect.Width = imageWidth * scaleX;
                    rect.Height = imageHeight * scaleY;
                    if (Angle == 90 || Angle == 180)
                        rect.X -= rect.Width - drawRect.Width;
                    if (Angle == 180)
                        rect.Y -= rect.Height - drawRect.Height;
                    break;

                case PictureBoxSizeMode.CenterImage:
                    rect.Offset((Width - imageWidth) * scaleX / 2, (Height - imageHeight) * scaleY / 2);
                    rect.Width = imageWidth * scaleX;
                    rect.Height = imageHeight * scaleY;
                    break;

                case PictureBoxSizeMode.StretchImage:
                    break;

                case PictureBoxSizeMode.Zoom:
                    /*float kx = drawRect.Width / imageWidth;
                    float ky = drawRect.Height / imageHeight;
                    if (kx < ky)
                      rect.Height = imageHeight * kx;
                    else
                      rect.Width = imageWidth * ky;
                    rect.Offset(
                      (Width * e.ScaleX - rect.Width) / 2,
                      (Height * e.ScaleY - rect.Height) / 2);*/
                    break;
            }


            float gridCompensationX = offsetX + rect.X;
            gridCompensationX = (int)gridCompensationX - gridCompensationX;
            float gridCompensationY = offsetY + rect.Y;
            gridCompensationY = (int)gridCompensationY - gridCompensationY;
            if (gridCompensationX < 0)
                gridCompensationX = 1 + gridCompensationX;
            if (gridCompensationY < 0)
                gridCompensationY = 1 + gridCompensationY;

            rect.Offset(gridCompensationX, gridCompensationY);

            upperLeft = new PointF(0, 0);
            upperRight = new PointF(rect.Width, 0);
            lowerLeft = new PointF(0, rect.Height);
            float angle = Angle;

            switch (SizeMode)
            {
                case PictureBoxSizeMode.Normal:
                    {
                        upperLeft = MovePointOnAngle(drawRect.Location, drawRect.Size, Angle);
                        PointF ur = rotateVector(upperRight, angle);
                        PointF ll = rotateVector(lowerLeft, angle);
                        upperRight = PointF.Add(upperLeft, new SizeF(ur));
                        lowerLeft = PointF.Add(upperLeft, new SizeF(ll));
                    }
                    break;
                case PictureBoxSizeMode.StretchImage:
                    {

                        upperLeft = MovePointOnAngle(drawRect.Location, drawRect.Size, Angle);

                        upperRight = MovePointOnAngle(
                            drawRect.Location,
                            drawRect.Size, Angle + 90);
                        lowerLeft = MovePointOnAngle(
                            drawRect.Location,
                            drawRect.Size, Angle + 270);
                    }
                    break;
                case PictureBoxSizeMode.CenterImage:
                    {
                        PointF rotatedVector;
                        float w = rect.Left - (drawRect.Left + drawRect.Width / 2);
                        float h = rect.Top - (drawRect.Top + drawRect.Height / 2);
                        rotatedVector = rotateVector(new PointF(w, h), Angle);
                        upperLeft = new PointF(rect.Left + rotatedVector.X - w, rect.Top + rotatedVector.Y - h);
                        rotatedVector = rotateVector(new PointF(rect.Width, 0), Angle);
                        upperRight = new PointF(upperLeft.X + rotatedVector.X, upperLeft.Y + rotatedVector.Y);
                        rotatedVector = rotateVector(new PointF(0, rect.Height), Angle);
                        lowerLeft = new PointF(upperLeft.X + rotatedVector.X, upperLeft.Y + rotatedVector.Y);
                    }
                    break;
                case PictureBoxSizeMode.AutoSize:
                case PictureBoxSizeMode.Zoom:
                    {
                        rect = new RectangleF(0, 0, imageWidth * 100f, imageHeight * 100f);
                        PointF center = new PointF(drawRect.Left + drawRect.Width / 2,
                            drawRect.Top + drawRect.Height / 2);
                        PointF[] p = new PointF[4];
                        p[0] = new PointF(-rect.Width / 2, -rect.Height / 2);
                        p[1] = new PointF(rect.Width / 2, -rect.Height / 2);
                        p[2] = new PointF(rect.Width / 2, rect.Height / 2);
                        p[3] = new PointF(-rect.Width / 2, rect.Height / 2);

                        float scaleToMin = 1;

                        for (int i = 0; i < 4; i++)
                            p[i] = rotateVector(p[i], Angle);

                        for (int i = 0; i < 4; i++)
                        {
                            if (p[i].X * scaleToMin < -drawRect.Width / 2)
                                scaleToMin = -drawRect.Width / 2 / p[i].X;
                            if (p[i].X * scaleToMin > drawRect.Width / 2)
                                scaleToMin = drawRect.Width / 2 / p[i].X;

                            if (p[i].Y * scaleToMin < -drawRect.Height / 2)
                                scaleToMin = -drawRect.Height / 2 / p[i].Y;
                            if (p[i].Y * scaleToMin > drawRect.Height / 2)
                                scaleToMin = drawRect.Height / 2 / p[i].Y;
                        }
                        upperLeft = PointF.Add(center, new SizeF(p[0].X * scaleToMin, p[0].Y * scaleToMin));
                        upperRight = PointF.Add(center, new SizeF(p[1].X * scaleToMin, p[1].Y * scaleToMin));
                        lowerLeft = PointF.Add(center, new SizeF(p[3].X * scaleToMin, p[3].Y * scaleToMin));
                    }
                    break;
            }

            /*switch (Angle)
          {
              case 90:
                  upperLeft = new PointF(rect.Right, rect.Top);
                  upperRight = new PointF(rect.Right, rect.Bottom);
                  lowerLeft = new PointF(rect.Left, rect.Top);
                  break;

              case 180:
                  upperLeft = new PointF(rect.Right, rect.Bottom);
                  upperRight = new PointF(rect.Left, rect.Bottom);
                  lowerLeft = new PointF(rect.Right, rect.Top);
                  break;

              case 270:
                  upperLeft = new PointF(rect.Left, rect.Bottom);
                  upperRight = new PointF(rect.Left, rect.Top);
                  lowerLeft = new PointF(rect.Right, rect.Bottom);
                  break;

              default:
                  upperLeft = new PointF(rect.Left, rect.Top);
                  upperRight = new PointF(rect.Right, rect.Top);
                  lowerLeft = new PointF(rect.Left, rect.Bottom);
                  break;
          }*/
            /* default:
                         PointF rotatedVector;
                         float w = rect.Left - (drawRect.Left + drawRect.Width / 2) ;
                         float h = rect.Top - (drawRect.Top + drawRect.Height/2);
                         rotatedVector = rotateVector(new PointF(w, h), Angle);
                         upperLeft = new PointF(rect.Left + rotatedVector.X - w, rect.Top + rotatedVector.Y - h);
                         rotatedVector = rotateVector(new PointF(rect.Width, 0), Angle);
                         upperRight = new PointF(upperLeft.X + rotatedVector.X, upperLeft.Y + rotatedVector.Y);
                         rotatedVector = rotateVector(new PointF(0, rect.Height), Angle);
                         lowerLeft = new PointF(upperLeft.X + rotatedVector.X, upperLeft.Y + rotatedVector.Y);
                         break;
                 }*/
        }
        //static int number = 0;

        /// <summary>
        /// Moves the point on specified angle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="size"></param>
        /// <param name="fangle"></param>
        /// <returns></returns>
        public PointF MovePointOnAngle(PointF p, SizeF size, float fangle)
        {
            while (fangle >= 360) fangle -= 360;
            while (fangle < 0) fangle += 360;
            float x, y;
            if (fangle < 90)
            {
                x = fangle / 90f * size.Width;
                y = 0;
            }
            else if (fangle < 180)
            {
                x = size.Width;
                y = (fangle - 90f) / 90f * size.Height;
            }
            else if (fangle < 270)
            {
                x = size.Width - (fangle - 180f) / 90f * size.Width;
                y = size.Height;
            }
            else
            {
                x = 0;
                y = size.Height - (fangle - 270f) / 90f * size.Height;
            }

            return PointF.Add(p, new SizeF(x, y));
        }

        /// <summary>
        /// Rotates vector on specified angle
        /// </summary>
        /// <param name="p"></param>
        /// <param name="fangle"></param>
        /// <returns></returns>
        public PointF rotateVector(PointF p, float fangle)
        {
            float angle = (float)(fangle / 180.0 * Math.PI);
            float ax = p.X;
            float ay = p.Y;

            float bx = ax * (float)Math.Cos(angle) - ay * (float)Math.Sin(angle);
            float by = ax * (float)Math.Sin(angle) + ay * (float)Math.Cos(angle);

            return new PointF(bx, by);
        }

        /// <summary>
        /// Loads image
        /// </summary>
        public virtual void LoadImage()
        {
            if (!String.IsNullOrEmpty(ImageLocation))
            {
                // 
                try
                {
                    Uri uri = CalculateUri();
                    if (uri.IsFile)
                        imageData = ImageHelper.Load(uri.LocalPath);
                    else
                        imageData = ImageHelper.LoadURL(uri.ToString());
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
            Image = null;
        }

        internal void ResetImageIndex()
        {
            imageIndex = -1;
        }
        #endregion

        #region Report Engine
        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());
            if (!String.IsNullOrEmpty(DataColumn))
                expressions.Add(DataColumn);
            return expressions.ToArray();
        }

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
        public override void SaveState()
        {
            base.SaveState();
            saveSizeMode = SizeMode;
        }

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            // avoid UpdateAutoSize call, use FSizeMode
            sizeMode = saveSizeMode;
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
                }
                else if (data is Image)
                {
                    Image = data as Image;
                }
                else if (data is string)
                {
                    ImageLocation = data.ToString();
                }
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
            sizeMode = PictureBoxSizeMode.Zoom;
            padding = new Padding();
            imageLocation = "";
            dataColumn = "";
            transparentColor = Color.Transparent;
            SetFlags(Flags.HasSmartTag, true);
            ResetImageIndex();
        }
    }
}