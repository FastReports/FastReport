using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;
using System.ComponentModel;

namespace FastReport.Gauge.Simple
{
    /// <summary>
    /// Represents a simple gauge.
    /// </summary>
    public partial class SimpleGauge : GaugeObject
    {
        /// <summary>
        /// Gets or sets gauge label.
        /// </summary>
        [Browsable(false)]
        public override GaugeLabel Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleGauge"/> class.
        /// </summary>
        public SimpleGauge() : base()
        {
            InitializeComponent();
            Value = 75;
            Scale = new SimpleScale(this);
            Pointer = new SimplePointer(this);
            Height = 2.0f * Units.Centimeters;
            Width = 8.0f * Units.Centimeters;
        }

        #endregion // Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            Scale.Draw(e);
            Pointer.Draw(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
            Graphics g = e.Graphics;

            if (Report != null && Report.SmoothGraphics)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            Scale.Draw(e);
            Pointer.Draw(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }

        #endregion // Public Methods
    }
}
