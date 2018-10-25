using FastReport.Utils;
using System.ComponentModel;
using System.Drawing;

namespace FastReport.Gauge.Simple.Progress
{
    /// <summary>
    /// Represents a simple progress gauge.
    /// </summary>
    public partial class SimpleProgressGauge : SimpleGauge
    {
        /// <summary>
        /// Gets or sets gauge label.
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        public override GaugeLabel Label
        {
            get { return base.Label; }
            set { base.Label = value; }
        }

        /// <summary>
        /// Gets scale. Should be disabled for SimpleProgressGauge
        /// </summary>
        [Browsable(false)]
        public new GaugeScale Scale
        {
            get { return base.Scale; }
        }

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleGauge"/> class.
        /// </summary>
        public SimpleProgressGauge() : base()
        {
            Pointer = new SimpleProgressPointer(this);

            (Scale as SimpleScale).FirstSubScale.Enabled = false;
            (Scale as SimpleScale).SecondSubScale.Enabled = false;
            (Pointer as SimplePointer).PointerRatio = 1f;
            (Pointer as SimplePointer).HorizontalOffset = 0;
            Label = new SimpleProgressLabel(this);
            Pointer.BorderColor = Color.Transparent;
            Border.Lines = BorderLines.All;
        }

        #endregion // Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void Draw(FRPaintEventArgs e)
        {
            base.Draw(e);
            Label.Draw(e);
        }

        #endregion // Public Methods
    }
}
