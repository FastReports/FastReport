using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using FastReport.Utils;

namespace FastReport.Gauge.Linear
{
    /// <summary>
    /// Represents a linear gauge.
    /// </summary>
    public partial class LinearGauge : GaugeObject
    {
        #region Fields

        private bool inverted;

        #endregion // Fields

        #region Properties

        /// <summary>
        /// Gets or sets the value that specifies inverted gauge or not.
        /// </summary>
        [Category("Appearance")]
        public bool Inverted
        {
            get { return inverted; }
            set { inverted = value; }
        }

        /// <summary>
        /// Gets or sets gauge label.
        /// </summary>
        [Browsable(false)]
        public override GaugeLabel Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }

        #endregion // Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearGauge"/> class.
        /// </summary>
        public LinearGauge() : base()
        {
            InitializeComponent();
            Scale = new LinearScale(this);
            Pointer = new LinearPointer(this);
            Height = 2.0f * Units.Centimeters;
            Width = 8.0f * Units.Centimeters;
            inverted = false;
        }

        #endregion // Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Assign(Base source)
        {
            base.Assign(source);

            LinearGauge src = source as LinearGauge;
            Inverted = src.Inverted;
        }

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (Report != null && Report.SmoothGraphics)
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
            }

            base.Draw(e);
            Scale.Draw(e);
            Pointer.Draw(e);
            Border.Draw(e, new RectangleF(AbsLeft, AbsTop, Width, Height));
        }

        /// <inheritdoc/>
        public override void Serialize(FRWriter writer)
        {
            LinearGauge c = writer.DiffObject as LinearGauge;
            base.Serialize(writer);

            if (Inverted != c.Inverted)
            {
                writer.WriteBool("Inverted", Inverted);
            }
        }

        #endregion // Public Methods
    }
}
