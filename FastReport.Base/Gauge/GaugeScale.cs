using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using FastReport.Utils;

namespace FastReport.Gauge
{
    /// <summary>
    /// Represents a scale of a gauge.
    /// </summary>
    public class GaugeScale : Component
    {
        #region Fields

        private GaugeObject parent;
        private Font font;
        private FillBase textFill;
        private ScaleTicks majorTicks;
        private ScaleTicks minorTicks;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets major ticks of scale.
        /// </summary>
        [Browsable(true)]
        public ScaleTicks MajorTicks
        {
            get { return majorTicks; }
            set { majorTicks = value; }
        }

        /// <summary>
        /// Gets or sets minor ticks of scale.
        /// </summary>
        [Browsable(true)]
        public ScaleTicks MinorTicks
        {
            get { return minorTicks; }
            set { minorTicks = value; }
        }

        /// <summary>
        /// Gets or sets the parent gauge object.
        /// </summary>
        [Browsable(false)]
        public GaugeObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }

        /// <summary>
        /// Gets or sets the font of scale.
        /// </summary>
        [Browsable(true)]
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        /// <summary>
        /// Gets or sets the scale font color
        /// </summary>
        [Editor("FastReport.TypeEditors.FillEditor, FastReport", typeof(UITypeEditor))]
        public FillBase TextFill
        {
            get { return textFill; }
            set { textFill = value; }
        }
        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeScale"/> class.
        /// </summary>
        /// <param name="parent">The parent gauge object.</param>
        public GaugeScale(GaugeObject parent)
        {
            this.parent = parent;
            font = new Font("Arial", 8.0f);
            TextFill = new SolidFill(Color.Black);
            majorTicks = new ScaleTicks();
            minorTicks = new ScaleTicks();
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Copies the contents of another GaugeScale.
        /// </summary>
        /// <param name="src">The GaugeScale instance to copy the contents from.</param>
        public virtual void Assign(GaugeScale src)
        {
            Font = src.Font;
            TextFill = src.TextFill;
        }

        /// <summary>
        /// Draws the scale of gauge.
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public virtual void Draw(FRPaintEventArgs e)
        {
        }

        /// <summary>
        /// Serializes the gauge scale.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="prefix">Scale property name.</param>
        /// <param name="diff">Another GaugeScale to compare with.</param>
        /// <remarks>
        /// This method is for internal use only.
        /// </remarks>
        public virtual void Serialize(FRWriter writer, string prefix, GaugeScale diff)
        {
            TextFill.Serialize(writer, prefix + ".TextFill", diff.TextFill);
            if ((writer.SerializeTo != SerializeTo.Preview || !Font.Equals(diff.Font)) && writer.ItemName != "inherited")
            {
                writer.WriteValue(prefix + ".Font", Font);
            }
        }

        #endregion // Public Methods
    }

    /// <summary>
    /// Represents a scale ticks.
    /// </summary>
    [ToolboxItem(false)]
    public class ScaleTicks : Component
    {
        #region Fields

        private float length;
        private int width;
        private Color color;
        private int count;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the length of ticks.
        /// </summary>
        [Browsable(false)]
        public float Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// Gets or sets the width of ticks.
        /// </summary>
        [Browsable(true)]
        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Gets or sets the color of ticks.
        /// </summary>
        [Browsable(true)]
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets or sets the count of ticks
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTicks"/> class.
        /// </summary>
        public ScaleTicks()
        {
            length = 8.0f;
            width = 1;
            color = Color.Black;
            count = 6;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTicks"/> class.
        /// </summary>
        /// <param name="length">Ticks length.</param>
        /// <param name="width">Ticks width.</param>
        /// <param name="color">Ticks color.</param>
        public ScaleTicks(float length, int width, Color color)
        {
            this.length = length;
            this.width = width;
            this.color = color;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ScaleTicks"/> class.
        /// </summary>
        /// <param name="length">Ticks length.</param>
        /// <param name="width">Ticks width.</param>
        /// <param name="color">Ticks color.</param>
        /// <param name="count">Ticks count.</param>
        public ScaleTicks(float length, int width, Color color, int count)
        {
            this.length = length;
            this.width = width;
            this.color = color;
            this.count = count;
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Copies the contents of another ScaleTicks.
        /// </summary>
        /// <param name="src">The ScaleTicks instance to copy the contents from.</param>
        public virtual void Assign(ScaleTicks src)
        {
            Length = src.Length;
            Width = src.Width;
            Color = src.Color;
        }

        /// <summary>
        /// Serializes the scale ticks.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="prefix">Scale ticks property name.</param>
        /// <param name="diff">Another ScaleTicks to compare with.</param>
        /// <remarks>
        /// This method is for internal use only.
        /// </remarks>
        public virtual void Serialize(FRWriter writer, string prefix, ScaleTicks diff)
        {
            if (Length != diff.Length)
            {
                writer.WriteFloat(prefix + ".Length", Length);
            }
            if (Width != diff.Width)
            {
                writer.WriteInt(prefix + ".Width", Width);
            }
            if (Color != diff.Color)
            {
                writer.WriteValue(prefix + ".Color", Color);
            }
        }

        #endregion // Public Methods
    }
}
