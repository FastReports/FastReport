using System;
using System.Drawing;
using System.ComponentModel;
using FastReport.Utils;

namespace FastReport.Barcode
{
    /// <summary>
    /// The base class for all barcodes.
    /// </summary>
    [TypeConverter(typeof(FastReport.TypeConverters.BarcodeConverter))]
    public abstract class BarcodeBase
    {
        #region Fields
        internal string text;
        internal int angle;
        internal bool showText;
        internal float zoom;
        internal bool showMarker;
        private Color color;
        private Font font;

        private static readonly Font DefaultFont = new Font("Arial", 8);
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of barcode.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get { return Barcodes.GetName(GetType()); }
        }

        /// <summary>
        /// Gets or sets the color of barcode.
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// Gets or sets the font of barcode.
        /// </summary>
        public Font Font
        {
            get { return font; }
            set { font = value; }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Creates the exact copy of this barcode.
        /// </summary>
        /// <returns>The copy of this barcode.</returns>
        public BarcodeBase Clone()
        {
            BarcodeBase result = Activator.CreateInstance(GetType()) as BarcodeBase;
            result.Assign(this);
            return result;
        }

        /// <summary>
        /// Assigns properties from other, similar barcode.
        /// </summary>
        /// <param name="source">Barcode object to assign properties from.</param>
        public virtual void Assign(BarcodeBase source)
        {
            Color = source.Color;
            Font = source.Font;
        }

        internal virtual void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            if (diff.GetType() != GetType())
                writer.WriteStr("Barcode", Name);
            if (diff.Color != Color)
                writer.WriteValue(prefix + "Color", Color);
            if (diff.Font != Font)
                writer.WriteValue(prefix + "Font", Font);
        }

        internal virtual void Initialize(string text, bool showText, int angle, float zoom)
        {
            this.text = text;
            this.showText = showText;
            this.angle = (angle / 90 * 90) % 360;
            this.zoom = zoom;
        }

        internal virtual void Initialize(string text, bool showText, int angle, float zoom, bool showMarker)
        {
            this.text = text;
            this.showText = showText;
            this.angle = (angle / 90 * 90) % 360;
            this.zoom = zoom;
            this.showMarker = showMarker;
        }

        internal virtual SizeF CalcBounds()
        {
            return SizeF.Empty;
        }

        internal virtual string StripControlCodes(string data)
        {
            return data;
        }

        /// <summary>
        /// Draws a barcode.
        /// </summary>
        /// <param name="g">The graphic surface.</param>
        /// <param name="displayRect">Display rectangle.</param>
        public virtual void DrawBarcode(IGraphics g, RectangleF displayRect)
        {
        }
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BarcodeBase"/> class with default settings.
        /// </summary>
        public BarcodeBase()
        {
            text = "";
            color = Color.Black;
            Font = DefaultFont;
        }

        /// <summary>
        /// Get default value of this barcode
        /// </summary>
        /// <returns></returns>
        public virtual string GetDefaultValue()
        {
            return "12345678";
        }
    }
}
