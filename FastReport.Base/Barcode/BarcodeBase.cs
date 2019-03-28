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
        private Color color;
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
        }

        internal virtual void Serialize(FRWriter writer, string prefix, BarcodeBase diff)
        {
            if (diff.GetType() != GetType())
                writer.WriteStr("Barcode", Name);
            if (diff.Color != Color)
                writer.WriteValue(prefix + "Color", Color);
        }

        internal virtual void Initialize(string text, bool showText, int angle, float zoom)
        {
            this.text = text;
            this.showText = showText;
            this.angle = (angle / 90 * 90) % 360;
            this.zoom = zoom;
        }

        internal virtual SizeF CalcBounds()
        {
            return SizeF.Empty;
        }

        internal virtual string StripControlCodes(string data)
        {
            return data;
        }

        public virtual void DrawBarcode(IGraphicsRenderer g, RectangleF displayRect)
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
        }
    }
}
