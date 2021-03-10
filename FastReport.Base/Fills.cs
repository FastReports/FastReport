using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;
using System.IO;
using System.Drawing.Imaging;

namespace FastReport
{
  /// <summary>
  /// Base class for all fills.
  /// </summary>
  [TypeConverter(typeof(FastReport.TypeConverters.FillConverter))]
  public abstract class FillBase
  {
    internal string Name
    {
      get { return GetType().Name.Replace("Fill", ""); }
    }

    /// <summary>
    /// Returned true if Color = Transparent
    /// </summary>
    internal abstract bool IsTransparent
    {
        get;
    }

    internal bool FloatDiff(float f1, float f2)
    {
      return Math.Abs(f1 - f2) > 1e-4;
    }


    /// <summary>
    /// Creates exact copy of this fill.
    /// </summary>
    /// <returns>Copy of this object.</returns>
    public abstract FillBase Clone();

    /// <summary>
    /// Creates the GDI+ Brush object.
    /// </summary>
    /// <param name="rect">Drawing rectangle.</param>
    /// <returns>Brush object.</returns>
    public abstract Brush CreateBrush(RectangleF rect);

    /// <summary>
    /// Creates the GDI+ Brush object with scaling.
    /// </summary>
    /// <param name="rect">Drawing rectangle.</param>
    /// <param name="scaleX">X scaling coefficient.</param>
    /// <param name="scaleY">Y scaling coefficient.</param>
    /// <returns>Brush object.</returns>
    public virtual Brush CreateBrush(RectangleF rect, float scaleX, float scaleY)
    {
        return CreateBrush(rect);
    }

    /// <summary>
    /// Serializes the fill.
    /// </summary>
    /// <param name="writer">Writer object.</param>
    /// <param name="prefix">Name of the fill property.</param>
    /// <param name="fill">Fill object to compare with.</param>
    /// <remarks>
    /// This method is for internal use only.
    /// </remarks>
    public virtual void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      if (fill.GetType() != GetType())
        writer.WriteStr(prefix, Name);
    }

    public virtual void Deserialize(FRReader reader, string prefix)
    {
    }

    public virtual void FinalizeComponent()
    {
    }

    public virtual void InitializeComponent()
    {
    }

    /// <summary>
    /// Fills the specified rectangle.
    /// </summary>
    /// <param name="e">Draw event arguments.</param>
    /// <param name="rect">Drawing rectangle.</param>
    public virtual void Draw(FRPaintEventArgs e, RectangleF rect)
    {
      rect = new RectangleF(rect.Left * e.ScaleX, rect.Top * e.ScaleY, rect.Width * e.ScaleX, rect.Height * e.ScaleY);
      using (Brush brush = CreateBrush(rect, e.ScaleX, e.ScaleY))
      {
        e.Graphics.FillRectangle(brush, rect.Left, rect.Top, rect.Width, rect.Height);
      }
    }
  }
  
  /// <summary>
  /// Class represents the solid fill.
  /// </summary>
  public class SolidFill : FillBase
  {
    private Color color;

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color Color
    {
      get { return color; }
      set { color = value; }
    }

    internal override bool IsTransparent 
    {
        get { return color.A == 0; }
    }

    /// <inheritdoc/>
    public override FillBase Clone()
    {
      return new SolidFill(Color);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return Color.GetHashCode();
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      SolidFill f = obj as SolidFill;
      return f != null && Color == f.Color;
    }

    /// <inheritdoc/>
    public override Brush CreateBrush(RectangleF rect)
    {
      return new SolidBrush(Color);
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      base.Serialize(writer, prefix, fill);
      SolidFill c = fill as SolidFill;
      
      if (c == null || c.Color != Color)
        writer.WriteValue(prefix + ".Color", Color);
    }

    /// <inheritdoc/>
    public override void Draw(FRPaintEventArgs e, RectangleF rect)
    {
      if (Color == Color.Transparent)
        return;
      Brush brush = e.Cache.GetBrush(Color);
      e.Graphics.FillRectangle(brush, rect.Left * e.ScaleX, rect.Top * e.ScaleY, rect.Width * e.ScaleX, rect.Height * e.ScaleY);
    }
    
    /// <summary>
    /// Initializes the <see cref="SolidFill"/> class with Transparent color.
    /// </summary>
    public SolidFill() : this(Color.Transparent)
    {
    }
    
    /// <summary>
    /// Initializes the <see cref="SolidFill"/> class with specified color.
    /// </summary>
    /// <param name="color"></param>
    public SolidFill(Color color)
    {
      Color = color;
    }
  }
  
  /// <summary>
  /// Class represents the linear gradient fill.
  /// </summary>
  public class LinearGradientFill : FillBase
  {
    private Color startColor;
    private Color endColor;
    private int angle;
    private float focus;
    private float contrast;

    /// <summary>
    /// Gets or sets the start color of the gradient. 
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color StartColor
    {
      get { return startColor; }
      set { startColor = value; }
    }

    /// <summary>
    /// Gets or sets the end color of the gradient. 
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color EndColor
    {
      get { return endColor; }
      set { endColor = value; }
    }

    internal override bool IsTransparent 
    {
        get { return startColor.A == 0 && endColor.A == 0; }
    }

    /// <summary>
    /// Gets or sets the angle of the gradient.
    /// </summary>
    [Editor("FastReport.TypeEditors.AngleEditor, FastReport", typeof(UITypeEditor))]
    public int Angle
    {
      get { return angle; }
      set { angle = value % 360; }
    }
    
    /// <summary>
    /// Gets or sets the focus point of the gradient.
    /// </summary>
    /// <remarks>
    /// Value is a floating point value from 0 to 1.
    /// </remarks>
    public float Focus
    {
      get { return focus; }
      set 
      { 
        if (value < 0)
          value = 0;
        if (value > 1)
          value = 1;
        focus = value; 
      }
    }
    
    /// <summary>
    /// Gets or sets the gradient contrast.
    /// </summary>
    /// <remarks>
    /// Value is a floating point value from 0 to 1.
    /// </remarks>
    public float Contrast
    {
      get { return contrast; }
      set 
      {
        if (value < 0)
          value = 0;
        if (value > 1)
          value = 1;
        contrast = value; 
      }
    }

    /// <inheritdoc/>
    public override FillBase Clone()
    {
      return new LinearGradientFill(StartColor, EndColor, Angle, Focus, Contrast);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return StartColor.GetHashCode() ^ (EndColor.GetHashCode() << 1) ^ 
        ((Angle.GetHashCode() + 1) << 2) ^ ((Focus.GetHashCode() + 1) << 10) ^ ((Contrast.GetHashCode() + 1) << 20);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      LinearGradientFill f = obj as LinearGradientFill;
      return f != null && StartColor == f.StartColor && EndColor == f.EndColor && Angle == f.Angle && 
        !FloatDiff(Focus, f.Focus) && !FloatDiff(Contrast, f.Contrast);
    }

    /// <inheritdoc/>
    public override Brush CreateBrush(RectangleF rect)
    {
      // workaround the gradient bug
      rect.Inflate(1, 1);
      
      LinearGradientBrush result = new LinearGradientBrush(rect, StartColor, EndColor, Angle);
      result.SetSigmaBellShape(Focus, Contrast);
      return result;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      base.Serialize(writer, prefix, fill);
      LinearGradientFill c = fill as LinearGradientFill;

      if (c == null || c.StartColor != StartColor)
        writer.WriteValue(prefix + ".StartColor", StartColor);
      if (c == null || c.EndColor != EndColor)
        writer.WriteValue(prefix + ".EndColor", EndColor);
      if (c == null || c.Angle != Angle)
        writer.WriteInt(prefix + ".Angle", Angle);
      if (c == null || FloatDiff(c.Focus, Focus))
        writer.WriteFloat(prefix + ".Focus", Focus);
      if (c == null || FloatDiff(c.Contrast, Contrast))
        writer.WriteFloat(prefix + ".Contrast", Contrast);
    }

    /// <summary>
    /// Initializes the <see cref="LinearGradientFill"/> class with default settings.
    /// </summary>
    public LinearGradientFill() : this(Color.Black, Color.White, 0, 100, 100)
    {
    }

    /// <summary>
    /// Initializes the <see cref="LinearGradientFill"/> class with start and end colors.
    /// </summary>
    /// <param name="startColor">Start color.</param>
    /// <param name="endColor">End color.</param>
    public LinearGradientFill(Color startColor, Color endColor) : this(startColor, endColor, 0)
    {
    }

    /// <summary>
    /// Initializes the <see cref="LinearGradientFill"/> class with start, end colors and angle.
    /// </summary>
    /// <param name="startColor">Start color.</param>
    /// <param name="endColor">End color.</param>
    /// <param name="angle">Angle.</param>
    public LinearGradientFill(Color startColor, Color endColor, int angle) : this(startColor, endColor, angle, 0, 100)
    {
    }

    /// <summary>
    /// Initializes the <see cref="LinearGradientFill"/> class with start and end colors, angle, focus and contrast.
    /// </summary>
    /// <param name="startColor">Start color.</param>
    /// <param name="endColor">End color.</param>
    /// <param name="angle">Angle.</param>
    /// <param name="focus">Focus.</param>
    /// <param name="contrast">Contrast.</param>
    public LinearGradientFill(Color startColor, Color endColor, int angle, float focus, float contrast)
    {
      StartColor = startColor;
      EndColor = endColor;
      Angle = angle;
      Focus = focus;
      Contrast = contrast;
    }
    
  }

  
  /// <summary>
  /// The style of the path gradient.
  /// </summary>
  public enum PathGradientStyle 
  { 
    /// <summary>
    /// Elliptic gradient.
    /// </summary>
    Elliptic, 
    
    /// <summary>
    /// Rectangular gradient.
    /// </summary>
    Rectangular
  }


  /// <summary>
  /// Class represents the path gradient fill.
  /// </summary>
  public class PathGradientFill : FillBase
  {
    private Color centerColor;
    private Color edgeColor;
    private PathGradientStyle style;

    /// <summary>
    /// Gets or sets the center color of the gradient.
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color CenterColor
    {
      get { return centerColor; }
      set { centerColor = value; }
    }

    /// <summary>
    /// Gets or sets the edge color of the gradient.
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color EdgeColor
    {
      get { return edgeColor; }
      set { edgeColor = value; }
    }

    /// <summary>
    /// Gets or sets the style of the gradient.
    /// </summary>
    public PathGradientStyle Style
    {
      get { return style; }
      set { style = value; }
    }

    internal override bool IsTransparent 
    {
        get { return centerColor.A == 0 && edgeColor.A == 0; }
    }

    /// <inheritdoc/>
    public override FillBase Clone()
    {
      return new PathGradientFill(CenterColor, EdgeColor, Style);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return CenterColor.GetHashCode() ^ (EdgeColor.GetHashCode() << 1) ^ ((Style.GetHashCode() + 1) << 2);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      PathGradientFill f = obj as PathGradientFill;
      return f != null && CenterColor == f.CenterColor && EdgeColor == f.EdgeColor && Style == f.Style;
    }

    /// <inheritdoc/>
    public override Brush CreateBrush(RectangleF rect)
    {
      GraphicsPath path = new GraphicsPath();
      if (Style == PathGradientStyle.Rectangular)
        path.AddRectangle(rect);
      else
      {
        float radius = (float)Math.Sqrt(rect.Width * rect.Width + rect.Height * rect.Height) / 2 + 1;
        PointF center = new PointF(rect.Left + rect.Width / 2 - 1, rect.Top + rect.Height / 2 - 1);
        RectangleF r = new RectangleF(center.X - radius, center.Y - radius, radius * 2, radius * 2);
        path.AddEllipse(r);
      }
      PathGradientBrush result = new PathGradientBrush(path);
      path.Dispose();
      result.CenterColor = CenterColor;
      result.SurroundColors = new Color[] { EdgeColor };
      return result;
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      base.Serialize(writer, prefix, fill);
      PathGradientFill c = fill as PathGradientFill;

      if (c == null || c.CenterColor != CenterColor)
        writer.WriteValue(prefix + ".CenterColor", CenterColor);
      if (c == null || c.EdgeColor != EdgeColor)
        writer.WriteValue(prefix + ".EdgeColor", EdgeColor);
      if (c == null || c.Style != Style)
        writer.WriteValue(prefix + ".Style", Style);
    }

    /// <summary>
    /// Initializes the <see cref="PathGradientFill"/> class with default settings.
    /// </summary>
    public PathGradientFill() : this(Color.Black, Color.White, PathGradientStyle.Elliptic)
    {
    }

    /// <summary>
    /// Initializes the <see cref="PathGradientFill"/> class with center, edge colors and style.
    /// </summary>
    /// <param name="centerColor">Center color.</param>
    /// <param name="edgeColor">Edge color.</param>
    /// <param name="style">Gradient style.</param>
    public PathGradientFill(Color centerColor, Color edgeColor, PathGradientStyle style)
    {
      CenterColor = centerColor;
      EdgeColor = edgeColor;
      Style = style;
    }
  }

  /// <summary>
  /// Class represents the hatch fill.
  /// </summary>
  public class HatchFill : FillBase
  {
    private Color foreColor;
    private Color backColor;
    private HatchStyle style;

    /// <summary>
    /// Gets or sets the foreground color.
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color ForeColor
    {
      get { return foreColor; }
      set { foreColor = value; }
    }

    /// <summary>
    /// Gets or sets the background color.
    /// </summary>
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color BackColor
    {
      get { return backColor; }
      set { backColor = value; }
    }

    /// <summary>
    /// Gets or sets the hatch style.
    /// </summary>
    public HatchStyle Style
    {
      get { return style; }
      set { style = value; }
    }

    internal override bool IsTransparent 
    {
        get { return foreColor.A == 0 && backColor.A == 0; }
    }

    /// <inheritdoc/>
    public override FillBase Clone()
    {
      return new HatchFill(ForeColor, BackColor, Style);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return ForeColor.GetHashCode() ^ (BackColor.GetHashCode() << 1) ^ ((Style.GetHashCode() + 1) << 2);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      HatchFill f = obj as HatchFill;
      return f != null && ForeColor == f.ForeColor && BackColor == f.BackColor && Style == f.Style;
    }

    /// <inheritdoc/>
    public override Brush CreateBrush(RectangleF rect)
    {
      return new HatchBrush(Style, ForeColor, BackColor);
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      base.Serialize(writer, prefix, fill);
      HatchFill c = fill as HatchFill;

      if (c == null || c.ForeColor != ForeColor)
        writer.WriteValue(prefix + ".ForeColor", ForeColor);
      if (c == null || c.BackColor != BackColor)
        writer.WriteValue(prefix + ".BackColor", BackColor);
      if (c == null || c.Style != Style)
        writer.WriteValue(prefix + ".Style", Style);
    }
    
    /// <summary>
    /// Initializes the <see cref="HatchFill"/> class with default settings.
    /// </summary>
    public HatchFill() : this(Color.Black, Color.White, HatchStyle.BackwardDiagonal)
    {
    }
    
    /// <summary>
    /// Initializes the <see cref="HatchFill"/> class with foreground, background colors and hatch style.
    /// </summary>
    /// <param name="foreColor">Foreground color.</param>
    /// <param name="backColor">Background color.</param>
    /// <param name="style">Hatch style.</param>
    public HatchFill(Color foreColor, Color backColor, HatchStyle style)
    {
      ForeColor = foreColor;
      BackColor = backColor;
      Style = style;
    }
  }


  /// <summary>
  /// Class represents the glass fill.
  /// </summary>
  public class GlassFill : FillBase
  {
    private Color color;
    private float blend;
    private bool hatch;

    /// <summary>
    /// Gets or sets the fill color.
    /// </summary>
    
    [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
    public Color Color
    {
      get { return color; }
      set { color = value; }
    }
    
    /// <summary>
    /// Gets or sets the blend value.
    /// </summary>
    /// <remarks>Value must be between 0 and 1.
    /// </remarks>
    [DefaultValue(0.2f)]
    public float Blend
    {
      get { return blend; }
      set { blend = value < 0 ? 0 : value > 1 ? 1 : value; }
    }
    
    /// <summary>
    /// Gets or sets a value determines whether to draw a hatch or not.
    /// </summary>
    [DefaultValue(true)]
    public bool Hatch
    {
      get { return hatch; }
      set { hatch = value; }
    }

    internal override bool IsTransparent 
    {
        get { return color.A == 0; }
    }

    /// <inheritdoc/>
    public override FillBase Clone()
    {
      return new GlassFill(Color, Blend, Hatch);
    }

    /// <inheritdoc/>
    public override int GetHashCode()
    {
      return Color.GetHashCode() ^ (Blend.GetHashCode() + 1) ^ ((Hatch.GetHashCode() + 1) << 2);
    }

    /// <inheritdoc/>
    public override bool Equals(object obj)
    {
      GlassFill f = obj as GlassFill;
      return f != null && Color == f.Color && Blend == f.Blend && Hatch == f.Hatch;
    }

    /// <inheritdoc/>
    public override void Draw(FRPaintEventArgs e, RectangleF rect)
    {
      rect = new RectangleF(rect.Left * e.ScaleX, rect.Top * e.ScaleY, rect.Width * e.ScaleX, rect.Height * e.ScaleY);

      // draw fill
      using (SolidBrush b = new SolidBrush(Color))
      {
        e.Graphics.FillRectangle(b, rect.Left, rect.Top, rect.Width, rect.Height);
      }
      
      // draw hatch
      if (Hatch)
      {
        using (HatchBrush b = new HatchBrush(HatchStyle.DarkUpwardDiagonal,
          Color.FromArgb(40, Color.White), Color.Transparent))
        {
          e.Graphics.FillRectangle(b, rect.Left, rect.Top, rect.Width, rect.Height);
        }
      }

      // draw blend
      using (SolidBrush b = new SolidBrush(Color.FromArgb((int)(Blend * 255), Color.White)))
      {
        e.Graphics.FillRectangle(b, rect.Left, rect.Top, rect.Width, rect.Height / 2);
      }
    }

    /// <inheritdoc/>
    public override Brush CreateBrush(RectangleF rect)
    {
      return new SolidBrush(Color);
    }

    /// <inheritdoc/>
    public override void Serialize(FRWriter writer, string prefix, FillBase fill)
    {
      base.Serialize(writer, prefix, fill);
      GlassFill c = fill as GlassFill;

      if (c == null || c.Color != Color)
        writer.WriteValue(prefix + ".Color", Color);
      if (c == null || c.Blend != Blend)
        writer.WriteFloat(prefix + ".Blend", Blend);
      if (c == null || c.Hatch != Hatch)
        writer.WriteBool(prefix + ".Hatch", Hatch);
    }

    /// <summary>
    /// Initializes the <see cref="GlassFill"/> class with default settings.
    /// </summary>
    public GlassFill() : this(Color.White, 0.2f, true)
    {
    }

    /// <summary>
    /// Initializes the <see cref="GlassFill"/> class with given color, blend ratio and hatch style.
    /// </summary>
    /// <param name="color">Color.</param>
    /// <param name="blend">Blend ratio (0..1).</param>
    /// <param name="hatch">Display the hatch.</param>
    public GlassFill(Color color, float blend, bool hatch)
    {
      Color = color;
      Blend = blend;
      Hatch = hatch;
    }
  }

    /// <summary>
    /// Class represents the Texture fill.
    /// </summary>
    public class TextureFill : FillBase
    {
        #region Fields

        private Image image;
        private int imageWidth;
        private int imageHeight;
        private bool preserveAspectRatio;
        private WrapMode wrapMode;
        private int imageIndex;
        private byte[] imageData;
        private int imageOffsetX;
        private int imageOffsetY;
        private static string dummyImageHash;

        #endregion // Fields

        #region  Properties

        /// <summary>
        /// Gets or sets value, indicating that image should preserve aspect ratio
        /// </summary>
        public bool PreserveAspectRatio
        {
            get { return preserveAspectRatio; }
            set { preserveAspectRatio = value; }
        }

        /// <summary>
        /// Gets or sets the image width
        /// </summary>
        public int ImageWidth
        {
            get
            {
                if (imageWidth <= 0)
                    ForceLoadImage();
                return imageWidth;
            }
            set
            {
                if (value != imageWidth && value > 0)
                {
                    if (PreserveAspectRatio && imageHeight > 0 && imageWidth > 0)
                    {
                        imageHeight = (int)(imageHeight * (float)value / imageWidth);
                    }
                    imageWidth = value;
                    ResizeImage(imageWidth, ImageHeight);
                }
            }
        }

        /// <summary>
        /// Gets or sets the image height
        /// </summary>
        public int ImageHeight
        {
            get
            {
                if (imageHeight <= 0)
                    ForceLoadImage();
                return imageHeight;
            }
            set
            {
                if (value != imageHeight && value > 0)
                {
                    if (PreserveAspectRatio && imageWidth > 0 && imageHeight > 0)
                    {
                        imageWidth = (int)(imageWidth * (float)value / imageHeight);
                    }
                    imageHeight = value;
                    ResizeImage(imageWidth, ImageHeight);
                }
            }
        }

        /// <summary>
        /// Gets or sets the texture wrap mode
        /// </summary>
        public WrapMode WrapMode
        {
            get { return wrapMode; }
            set { wrapMode = value; }
        }

        /// <summary>
        /// Gets or sets the image index
        /// </summary>
        public int ImageIndex
        {
            get { return imageIndex; }
            set { imageIndex = value; }
        }

        /// <summary>
        /// Gets or sets the image data
        /// </summary>
        public byte[] ImageData
        {
            get { return imageData; }
            set
            {
                SetImageData(value);
            }
        }

        /// <summary>
        /// Image left offset
        /// </summary>
        public int ImageOffsetX
        {
            get { return imageOffsetX; }
            set { imageOffsetX = value; }
        }

        /// <summary>
        /// Image top offset
        /// </summary>
        public int ImageOffsetY
        {
            get { return imageOffsetY; }
            set { imageOffsetY = value; }
        }

        internal override bool IsTransparent 
        {
            get { return false; }
        }

        #endregion // Properties

        #region Private Methods

        private void Clear()
        {
            if (image != null)
                image.Dispose();
            image = null;
            imageData = null;
        }

        private void ResizeImage(int width, int height)
        {
            if (imageData == null || width <= 0 || height <= 0)
                return;
            else
            {
                image = ImageHelper.Load(imageData);
                image = new Bitmap(image, width, height);
            }
                
        }
        private void ResetImageIndex()
        {
            imageIndex = -1;
        }
        private void ForceLoadImage()
        {
            byte[] data = imageData;
            if (data == null)
                return;
            byte[] saveImageData = data;
            // imageData will be reset after this line, keep it
            image = ImageHelper.Load(data);
            if (imageWidth <= 0 && imageHeight <= 0)
            {
                imageWidth = image.Width;
                imageHeight = image.Height;
            }
            else if (imageWidth != image.Width || imageHeight != image.Height)
            {
                ResizeImage(imageWidth, imageHeight);
            }
            data = saveImageData;
        }

        #endregion // Private Methods

        #region Public Methods

        /// <summary>
        /// Sets image data to imageData
        /// </summary>
        /// <param name="data">input image data</param>
        public void SetImageData(byte[] data)
        {
            ResetImageIndex();
            image = null;
            imageData = data;
            ResizeImage(imageWidth, imageHeight);
        }

        /// <summary>
        /// Set image
        /// </summary>
        /// <param name="image">input image</param>
        public void SetImage(Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, image.RawFormat);
                SetImageData(ms.ToArray());
            }
        }

        /// <inheritdoc/>
        public override FillBase Clone()
        {
            
            TextureFill f = new TextureFill(imageData.Clone() as byte[], ImageWidth, ImageHeight, PreserveAspectRatio, WrapMode, ImageOffsetX, ImageOffsetY);
            //f.ImageIndex = ImageIndex;
            return f;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return ImageData.GetHashCode() ^ (ImageWidth.GetHashCode() << 1) ^
                ((ImageHeight.GetHashCode() + 1) << 2) ^
                ((PreserveAspectRatio.GetHashCode() + 1) << 10) ^
                ((WrapMode.GetHashCode() + 1) << 20) ^
                ((ImageOffsetX.GetHashCode() + 1) << 40) ^
                ((ImageOffsetY.GetHashCode() + 1) << 60);
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            TextureFill f = obj as TextureFill;
            return f != null && ImageData == f.ImageData && 
                ImageWidth == f.ImageWidth && 
                ImageHeight == f.ImageHeight &&
                PreserveAspectRatio == f.PreserveAspectRatio &&
                WrapMode == f.WrapMode &&
                ImageOffsetX == f.ImageOffsetX &&
                ImageOffsetY == f.ImageOffsetY;
        }

        /// <inheritdoc/>
        public override Brush CreateBrush(RectangleF rect)
        {
            if (image == null)
                ForceLoadImage();          
            TextureBrush brush = new TextureBrush(image, WrapMode);
            brush.TranslateTransform(rect.Left + ImageOffsetX, rect.Top + ImageOffsetY);
            return brush;
        }

        /// <inheritdoc/>
        public override Brush CreateBrush(RectangleF rect, float scaleX, float scaleY)
        {
            if (image == null)
                ForceLoadImage();
            TextureBrush brush = new TextureBrush(image, WrapMode);
            brush.TranslateTransform(rect.Left + ImageOffsetX * scaleX, rect.Top + ImageOffsetY * scaleY);
            brush.ScaleTransform(scaleX, scaleY);
            return brush;
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer, string prefix, FillBase fill)
        {
            base.Serialize(writer, prefix, fill);
            TextureFill c = fill as TextureFill;
            if (c == null || c.ImageWidth != ImageWidth)
                writer.WriteValue(prefix + ".ImageWidth", ImageWidth);
            if (c == null || c.ImageHeight != ImageHeight)
                writer.WriteValue(prefix + ".ImageHeight", ImageHeight);
            if (c == null || c.PreserveAspectRatio != PreserveAspectRatio)
                writer.WriteBool(prefix + ".PreserveAspectRatio", PreserveAspectRatio);
            if (c == null || c.WrapMode != WrapMode)
                writer.WriteValue(prefix + ".WrapMode", WrapMode);
            if (c == null || c.ImageOffsetX != ImageOffsetX)
                writer.WriteValue(prefix + ".ImageOffsetX", ImageOffsetX);
            if (c == null || c.ImageOffsetY != ImageOffsetY)
                writer.WriteValue(prefix + ".ImageOffsetY", ImageOffsetY);

            // store image data
            if (writer.SerializeTo != SerializeTo.SourcePages)
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
                                ImageHelper.Save(image, stream, ImageFormat.Png);
                                bytes = stream.ToArray();
                            }
                        }
                        if (bytes != null)
                        {
                            string imgHash = BitConverter.ToString(new Murmur3().ComputeHash(bytes));
                            if (imgHash != dummyImageHash)
                                imageIndex = writer.BlobStore.AddOrUpdate(bytes, imgHash.Replace("-", String.Empty));
                        }
                    }
                }
                else
                {
                    if (imageData != null)
                    {
                        string hash = BitConverter.ToString(new Murmur3().ComputeHash(imageData));
                        if (hash != dummyImageHash)
                        {
                            if (c == null || !writer.AreEqual(ImageData, c.ImageData))
                                writer.WriteStr(prefix + ".ImageData", Convert.ToBase64String(ImageData));
                        }
                    }
                }

                if (writer.BlobStore != null || writer.SerializeTo == SerializeTo.Undo)
                    writer.WriteInt(prefix + ".ImageIndex", imageIndex);
            }
        }

        /// <inheritdoc/>
        public override void Deserialize(FRReader reader, string prefix)
        {
            base.Deserialize(reader, prefix);
            if (reader.HasProperty(prefix + ".ImageIndex"))
            {
                imageIndex = reader.ReadInt(prefix + ".ImageIndex");
            }
            if (reader.BlobStore != null && imageIndex != -1)
            {
                SetImageData(reader.BlobStore.Get(imageIndex));
            }
        }

        /// <inheritdoc/>
        public override void FinalizeComponent()
        {
            base.FinalizeComponent();
            Clear();
            ResetImageIndex();
            
        }

        /// <inheritdoc/>
        public override void InitializeComponent()
        {
            base.InitializeComponent();
            ResetImageIndex();
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e, RectangleF rect)
        {
            if (image == null)
                ForceLoadImage();
            if (image == null)
                return;
            else
                base.Draw(e, rect);
        }

        #endregion //Public Methods

        #region Constructors

        /// <summary>
        /// Initializes the <see cref="TextureFill"/> class with default texture.
        /// </summary>
        public TextureFill()
        {
            ResetImageIndex();
            SetImageData(null);
            Stream dummy = ResourceLoader.GetStream("FastReport", "icon16.ico");
            using (MemoryStream ms = new MemoryStream())
            {
                const int BUFFER_SIZE = 4 * 1024;
                dummy.CopyTo(ms, BUFFER_SIZE);
                SetImageData(ms.ToArray());
            }
            WrapMode = WrapMode.Tile;
            PreserveAspectRatio = true;
        }

        /// <summary>
        /// Initializes the <see cref="TextureFill"/> class with specified image.
        /// </summary>
        /// <param name="imageBytes"></param>
        public TextureFill(byte[] imageBytes)
        {
            ResetImageIndex();
            SetImageData(imageBytes);
            WrapMode = WrapMode.Tile;
            PreserveAspectRatio = true;
        }

        /// <summary>
        /// Initializes the <see cref="TextureFill"/> class with specified image.
        /// </summary>
        /// <param name="image"></param>
        public TextureFill(byte[] imageBytes, int width, int height, bool preserveAspectRatio, WrapMode wrapMode, int imageOffsetX, int imageOffsetY) : this(imageBytes)
        {
            PreserveAspectRatio = preserveAspectRatio;
            WrapMode = wrapMode;
            imageWidth = width;
            imageHeight = height;
            ImageOffsetX = imageOffsetX;
            ImageOffsetY = imageOffsetY;
        }

        static TextureFill()
        {
            dummyImageHash = "62-57-78-BF-92-9F-81-12-C0-43-6B-5D-B1-D8-04-DD";
        }

        #endregion //Constructors
    }
}
