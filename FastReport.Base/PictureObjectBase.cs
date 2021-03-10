using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Windows.Forms;
using FastReport.Utils;

namespace FastReport
{

    /// <summary>
    /// Specifies the alignment of a image in the border.
    /// </summary>
    public enum ImageAlign
    {
        /// <summary>
        /// Specifies that image is not aligned in the layout rectangle.
        /// </summary>
        None,

        /// <summary>
        /// Specifies that image is aligned in the top-left of the layout rectangle.
        /// </summary>
        Top_Left,

        /// <summary>
        /// Specifies that image is aligned in the top-center of the layout rectangle.
        /// </summary>
        Top_Center,

        /// <summary>
        /// Specifies that image is aligned in the top-right of the layout rectangle.
        /// </summary>
        Top_Right,

        /// <summary>
        /// Specifies that image is aligned in the center-left of the layout rectangle.
        /// </summary>
        Center_Left,

        /// <summary>
        /// Specifies that image is aligned in the center-center of the layout rectangle.
        /// </summary>
        Center_Center,

        /// <summary>
        /// Specifies that image is aligned in the center-right of the layout rectangle.
        /// </summary>
        Center_Right,

        /// <summary>
        /// Specifies that image is aligned in the center-left of the layout rectangle.
        /// </summary>
        Bottom_Left,

        /// <summary>
        /// Specifies that image is aligned in the center-center of the layout rectangle.
        /// </summary>
        Bottom_Center,

        /// <summary>
        /// Specifies that image is aligned in the center-right of the layout rectangle.
        /// </summary>
        Bottom_Right,
    }

 

    /// <summary>
    /// the base class for all picture objects
    /// </summary>
    public abstract partial class PictureObjectBase : ReportComponentBase
    {
        #region Internal Fields


        #endregion Internal Fields

        #region Private Fields

        private int angle;
        private string dataColumn;
        private bool grayscale;
        private string imageLocation;
        private string imageSourceExpression;
        private float maxHeight;
        private float maxWidth;
        private Padding padding;
        private PictureBoxSizeMode saveSizeMode;
        private bool showErrorImage;
        private PictureBoxSizeMode sizeModeInternal;
        private ImageAlign imageAlign;

        #endregion Private Fields

        #region Public Properties

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
        /// Gets or sets the expression that determines the source for the image to display in the PictureObject.
        /// </summary>
        /// <remarks>
        /// The result of the expression should be data column name or path to the image file.
        /// The data column name will be saved to the <see cref="DataColumn"/> property.
        /// The path will be savetd to the <see cref="ImageLocation"/> property.
        /// </remarks>
        [Category("Data")]
        [Editor("FastReport.TypeEditors.ExpressionEditor, FastReport", typeof(UITypeEditor))]
        public string ImageSourceExpression
        {
            get { return imageSourceExpression; }
            set
            {
                imageSourceExpression = value;

                if (!String.IsNullOrEmpty(ImageSourceExpression) && Report != null)
                {
                    string expression = ImageSourceExpression;
                    if (ImageSourceExpression.StartsWith("[") && ImageSourceExpression.EndsWith("]"))
                    {
                        expression = ImageSourceExpression.Substring(1, ImageSourceExpression.Length - 2);
                    }
                    if (Data.DataHelper.IsValidColumn(Report.Dictionary, expression))
                    {
                        DataColumn = expression;
                    }
                    if (Data.DataHelper.IsValidParameter(Report.Dictionary, expression))
                    {
                        ImageLocation = Report.GetParameterValue(expression).ToString();
                    }
                }
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
        /// Gets or sets the maximum height of a Picture object, in pixels.
        /// </summary>
        /// <remarks>
        /// Use this property to restrict the object size if the <see cref="SizeMode"/> property
        /// is set to <b>AutoSize</b>.
        /// </remarks>
        [DefaultValue(0f)]
        [Category("Layout")]
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float MaxHeight
        {
            get { return maxHeight; }
            set { maxHeight = value; }
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
        [TypeConverter("FastReport.TypeConverters.UnitsConverter, FastReport")]
        public float MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
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
        /// Gets or sets a value that specifies how an image is positioned within a PictureObject.
        /// </summary>
        [DefaultValue(PictureBoxSizeMode.Zoom)]
        [Category("Behavior")]
        public virtual PictureBoxSizeMode SizeMode
        {
            get { return sizeModeInternal; }
            set
            {
                sizeModeInternal = value;
                UpdateAutoSize();
            }
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

        /// <summary>
        /// Gets or sets the alignment of a image in the border.
        /// </summary>
        [DefaultValue(ImageAlign.None)]
        [Category("Appearance")]
        public ImageAlign ImageAlign
        {
            get { return imageAlign; }
            set { imageAlign = value; }
        }


        #endregion Public Properties

        #region Protected Properties

        /// <summary>
        /// Return base size of image, internal use only
        /// </summary>
        [Browsable(false)]
        protected abstract float ImageHeight { get; }

        /// <summary>
        /// Return base size of image, internal use only
        /// </summary>
        [Browsable(false)]
        protected abstract float ImageWidth { get; }

        #endregion Protected Properties

        #region Public Constructors

        /// <inheritdoc/>
        public PictureObjectBase()
        {
            sizeModeInternal = PictureBoxSizeMode.Zoom;
            padding = new Padding();
            imageLocation = "";
            dataColumn = "";
            imageSourceExpression = "";
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            PictureObjectBase src = source as PictureObjectBase;
            if (src != null)
            {
                ImageLocation = src.ImageLocation;
                DataColumn = src.DataColumn;
                ImageSourceExpression = src.ImageSourceExpression;
                Padding = src.Padding;
                SizeMode = src.SizeMode;
                MaxWidth = src.MaxWidth;
                MaxHeight = src.MaxHeight;
                Angle = src.Angle;
                Grayscale = src.Grayscale;
                ShowErrorImage = src.ShowErrorImage;
                ImageAlign = src.ImageAlign;
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
                // TODO
                // the problem with linux???? PATH!!!

                string path;
                if (!String.IsNullOrEmpty(Config.ReportSettings.ImageLocationRoot))
                    path = Path.Combine(Config.ReportSettings.ImageLocationRoot, ImageLocation.Replace('/', '\\'));
                else
                    path = Path.GetFullPath(ImageLocation);
                return new Uri(path);
            }
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

        public abstract void DrawImage(FRPaintEventArgs e);

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

            if (ImageAlign != ImageAlign.None)
                UpdateAlign(drawRect, ref upperLeft, ref upperRight, ref lowerLeft);

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

        private void UpdateAlign(RectangleF drawRect, ref PointF upperLeft, ref PointF upperRight, ref PointF lowerLeft)
        {
            PointF lowerRight = new PointF(upperRight.X + lowerLeft.X - upperLeft.X,
                upperRight.Y + lowerLeft.Y - upperLeft.Y);
            float top = Math.Min(Math.Min(upperLeft.Y, Math.Min(upperRight.Y, lowerLeft.Y)), lowerRight.Y);
            float botom = Math.Max( Math.Max(upperLeft.Y, Math.Max(upperRight.Y, lowerLeft.Y)), lowerRight.Y);
            float height = botom - top;
            float offsetY = drawRect.Y - top;

            float left = Math.Min(Math.Min(upperLeft.X, Math.Min(upperRight.X, lowerLeft.X)), lowerRight.X);
            float right = Math.Max(Math.Max(upperLeft.X, Math.Max(upperRight.X, lowerLeft.X)), lowerRight.X);
            float width = right - left;
            float offsetX = drawRect.X - left;

            switch (ImageAlign)
            {
                case ImageAlign.Top_Left:
                    break;
                case ImageAlign.Top_Center:
                    offsetX += (drawRect.Width - width) / 2;
                    break;
                case ImageAlign.Top_Right:
                    offsetX += drawRect.Width - width;
                    break;
                case ImageAlign.Center_Left:
                    offsetY += (drawRect.Height - height) / 2;
                    break;
                case ImageAlign.Center_Center:
                    offsetX += (drawRect.Width - width) / 2;
                    offsetY += (drawRect.Height - height) / 2;
                    break;
                case ImageAlign.Center_Right:
                    offsetX += drawRect.Width - width;
                    offsetY += (drawRect.Height - height) / 2;
                    break;
                case ImageAlign.Bottom_Left:
                    offsetY += drawRect.Height - height;
                    break;
                case ImageAlign.Bottom_Center:
                    offsetX += (drawRect.Width - width) / 2;
                    offsetY += drawRect.Height - height;
                    break;
                case ImageAlign.Bottom_Right:
                    offsetX += drawRect.Width - width;
                    offsetY += drawRect.Height - height;
                    break;

            }

            upperLeft.X += offsetX;
            upperRight.X += offsetX;
            lowerLeft.X += offsetX;
            upperLeft.Y += offsetY;
            upperRight.Y += offsetY;
            lowerLeft.Y += offsetY;
        }

        

        /// <summary>
        /// Loads image
        /// </summary>
        public abstract void LoadImage();

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

        /// <inheritdoc/>
        public override void RestoreState()
        {
            base.RestoreState();
            // avoid UpdateAutoSize call, use sizeModeInternal
            sizeModeInternal = saveSizeMode;
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

        /// <inheritdoc/>
        public override void SaveState()
        {
            base.SaveState();
            saveSizeMode = SizeMode;
        }

        public override void Serialize(FRWriter writer)
        {
            PictureObjectBase c = writer.DiffObject as PictureObjectBase;
            base.Serialize(writer);

            if (writer.SerializeTo != SerializeTo.Preview
                && writer.SerializeTo != SerializeTo.SourcePages
                && ImageLocation != c.ImageLocation)
                writer.WriteStr("ImageLocation", ImageLocation);

            if (DataColumn != c.DataColumn)
                writer.WriteStr("DataColumn", DataColumn);

            if (ImageSourceExpression != c.ImageSourceExpression)
                writer.WriteStr("ImageSourceExpression", ImageSourceExpression);

            if (Padding != c.Padding)
                writer.WriteValue("Padding", Padding);
            if (SizeMode != c.SizeMode)
                writer.WriteValue("SizeMode", SizeMode);
            if (FloatDiff(MaxWidth, c.MaxWidth))
                writer.WriteFloat("MaxWidth", MaxWidth);
            if (FloatDiff(MaxHeight, c.MaxHeight))
                writer.WriteFloat("MaxHeight", MaxHeight);
            if (Angle != c.Angle)
                writer.WriteInt("Angle", Angle);
            if (Grayscale != c.Grayscale)
                writer.WriteBool("Grayscale", Grayscale);
            if (ShowErrorImage != c.ShowErrorImage)
                writer.WriteBool("ShowErrorImage", ShowErrorImage);
            if (ImageAlign != ImageAlign.None)
                writer.WriteValue("ImageAlign", ImageAlign);
        }

        #endregion Public Methods

        #region Internal Methods
        /// <summary>
        /// Draws not tiled image
        /// </summary>
        /// <param name="e"></param>
        /// <param name="drawRect"></param>
        /// <param name="image"></param>
        internal virtual void DrawImageInternal(FRPaintEventArgs e, RectangleF drawRect)
        {
            bool rotate = Angle == 90 || Angle == 270;
            float imageWidth = ImageWidth;//rotate ? Image.Height : Image.Width;
            float imageHeight = ImageHeight;//rotate ? Image.Width : Image.Height;

            PointF upperLeft;
            PointF upperRight;
            PointF lowerLeft;
            System.Drawing.Drawing2D.Matrix matrix = e.Graphics.Transform;
            GetImageAngleTransform(drawRect, imageWidth, imageHeight, e.ScaleX, e.ScaleY, matrix.OffsetX, matrix.OffsetY, out upperLeft, out upperRight, out lowerLeft);
            DrawImageInternal2(e.Graphics, upperLeft, upperRight, lowerLeft);
        }
        #endregion Internal Methods

        #region Protected Methods

        protected abstract void DrawImageInternal2(Graphics graphics, PointF upperLeft, PointF upperRight, PointF lowerLeft);

        /// <summary>
        /// Reset index of image
        /// </summary>
        protected abstract void ResetImageIndex();

        /// <summary>
        /// When auto size was updated, internal use only
        /// </summary>
        protected void UpdateAutoSize()
        {
            if (SizeMode == PictureBoxSizeMode.AutoSize)
            {
                if (ImageWidth == 0 || ImageHeight == 0)
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
                    p[0] = new PointF(-ImageWidth / 2, -ImageHeight / 2);
                    p[1] = new PointF(ImageWidth / 2, -ImageHeight / 2);
                    p[2] = new PointF(ImageWidth / 2, ImageHeight / 2);
                    p[3] = new PointF(-ImageWidth / 2, ImageHeight / 2);

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

        #endregion Protected Methods

        /// <inheritdoc/>
        public override string[] GetExpressions()
        {
            List<string> expressions = new List<string>();
            expressions.AddRange(base.GetExpressions());
            if (!String.IsNullOrEmpty(DataColumn))
                expressions.Add(DataColumn);

            if (!String.IsNullOrEmpty(ImageSourceExpression))
            {
                if (ImageSourceExpression.StartsWith("[") && ImageSourceExpression.EndsWith("]"))
                {
                    expressions.Add(ImageSourceExpression.Substring(1, ImageSourceExpression.Length - 2));
                }
                else
                {
                    expressions.Add(ImageSourceExpression);
                }
            }

            return expressions.ToArray();
        }
    }
}