using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;
using FastReport.Utils;
using System.Drawing.Design;

namespace FastReport
{
    /// <summary>
    /// Represents text outline.
    /// </summary>
    [ToolboxItem(false)]
    [TypeConverter(typeof(FastReport.TypeConverters.FRExpandableObjectConverter))]
    public class TextOutline// : Component
    {
        #region Fields

        private bool enabled;
        private Color color;
        private float width;
        private DashStyle style;
        private bool drawbehind;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating that outline is enabled.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// Enable or disable draw the outline behind of text.
        /// </summary>
        [DefaultValue(false)]
        [Browsable(true)]
        public bool DrawBehind
        {
            get { return drawbehind; }
            set { drawbehind = value; }
        }

        /// <summary>
        /// Gets or sets the outline color.
        /// </summary>
        [Editor("FastReport.TypeEditors.ColorEditor, FastReport", typeof(UITypeEditor))]
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets or sets the outline width.
        /// </summary>
        [DefaultValue(1.0f)]
        [Browsable(true)]
        public float Width
        {
            get { return width; }
            set { width = value; }
        }

        /// <summary>
        /// Specifies the style of an outline.
        /// </summary>
        [DefaultValue(DashStyle.Solid)]
        [Browsable(true)]
        public DashStyle Style
        {
            get { return style; }
            set { style = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutline"/> class.
        /// </summary>
        public TextOutline()
        {
            enabled = false;
            color = Color.Black;
            width = 1.0f;
            style = DashStyle.Solid;
            drawbehind = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextOutline"/> class with specified parameters.
        /// </summary>
        /// <param name="enabled">True if outline enabled.</param>
        /// <param name="color">Outline color.</param>
        /// <param name="width">Outline width.</param>
        /// <param name="style">Outline style.</param>
        /// <param name="drawbehind">True if outline should be drawn behind text.</param>
        public TextOutline(bool enabled, Color color, float width, DashStyle style, bool drawbehind)
        {
            this.enabled = enabled;
            this.color = color;
            this.width = width;
            this.style = style;
            this.drawbehind = drawbehind;
        }

        #endregion // Constructors

        #region Public Methods

        /// <summary>
        /// Copies the content of another TextOutline.
        /// </summary>
        /// <param name="src">The TextOutline instance to copy the contents from.</param>
        public void Assign(TextOutline src)
        {
            enabled = src.Enabled;
            color = src.Color;
            width = src.Width;
            style = src.Style;
            drawbehind = src.DrawBehind;
        }

        /// <summary>
        /// Creates the exact copy of this outline.
        /// </summary>
        /// <returns>Copy of this outline.</returns>
        public TextOutline Clone()
        {
            return new TextOutline(enabled, color, width, style, drawbehind);
        }

        /// <summary>
        /// Serializes the TextOutline.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="prefix">TextOutline property name.</param>
        /// <param name="diff">Another TextOutline to compare with.</param>
        public void Serialize(FRWriter writer, string prefix, TextOutline diff)
        {
            if (enabled != diff.Enabled)
            {
                writer.WriteBool(prefix + ".Enabled", enabled);
            }
            if (color != diff.Color)
            {
                writer.WriteValue(prefix + ".Color", color);
            }
            if (width != diff.Width)
            {
                writer.WriteFloat(prefix + ".Width", width);
            }
            if (style != diff.Style)
            {
                writer.WriteValue(prefix + ".Style", style);
            }
            if (drawbehind != diff.DrawBehind)
            {
                writer.WriteBool(prefix + ".DrawBehind", drawbehind);
            }
        }

        #endregion // Public Methods
    }
}
