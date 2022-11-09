using FastReport.Utils;
using System;
using System.ComponentModel;
using System.Drawing;

namespace FastReport.Gauge
{
    /// <summary>
    /// Represents a label of a gauge.
    /// </summary>
#if !DEBUG
    [DesignTimeVisible(false)]
#endif
    public class GaugeLabel : Component
    {
        #region Private Fields
        private string text;
        private Font font;
        private Color color;
        private GaugeObject parent;

        #endregion //Private Fields

        #region Properties
        /// <summary>
        /// Gets or sets the label text
        /// </summary>
        public virtual string Text
        {
            get { return text; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Text");
                text = value;
            }
        }

        /// <summary>
        /// Gets or sets the label font
        /// </summary>
        public Font Font
        {
            get { return font; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Font");
                font = value;
            }
        }

        /// <summary>
        /// Gets or sets the label color
        /// </summary>
        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
            }
        }

        /// <summary>
        /// Gets or sets the label parent
        /// </summary>
        [Browsable(false)]
        public GaugeObject Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        #endregion  //Properties

        #region Constractors

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeLabel"/> class.
        /// </summary>
        public GaugeLabel(GaugeObject parent)
        {
            Text = "";
            Color = Color.Black;
            Font = parent.Scale.Font;
            this.parent = parent;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GaugeLabel"/> class.
        /// </summary>
        /// <param name="text">Label text</param>
        /// <param name="font">Label font</param>
        /// <param name="color">Label color</param>
        /// <param name="parent">Label parent</param>
        public GaugeLabel(GaugeObject parent, string text, Font font, Color color)
        {
            Text = text;
            Font = font;
            Color = color;
            this.parent = parent;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Copies the contents of another GaugeLabel.
        /// </summary>
        /// <param name="src">The GaugeLabel instance to copy the contents from.</param>
        public virtual void Assign(GaugeLabel src)
        {
            Text = src.Text;
            Font = src.Font;
            Color = src.Color;
        }

        /// <summary>
        /// Draws the gauge label.
        /// </summary>
        /// <param name="e">Draw event arguments.</param>
        public virtual void Draw(FRPaintEventArgs e)
        {
        }

        /// <summary>
        /// Serializes the gauge label.
        /// </summary>
        /// <param name="writer">Writer object.</param>
        /// <param name="prefix">Gauge label property name.</param>
        /// <param name="diff">Another GaugeLabel to compare with.</param>
        /// <remarks>
        /// This method is for internal use only.
        /// </remarks>
        public virtual void Serialize(FRWriter writer, string prefix, GaugeLabel diff)
        {
            if (Text != diff.Text)
            {
                writer.WriteStr(prefix + ".Text", Text);
            }
            if ((writer.SerializeTo != SerializeTo.Preview || !Font.Equals(diff.Font)) && writer.ItemName != "inherited")
            {
                writer.WriteValue(prefix + ".Font", Font);
            }
            if (Color != diff.Color)
            {
                writer.WriteValue(prefix + ".Color", Color);
            }
        }

        #endregion // Public Methods

    }
}
