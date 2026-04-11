using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores presentation settings for a text-based report band.
    /// </summary>
    public class TextBandDefinition
    {
        /// <summary>
        /// Gets or sets the text or expression rendered by the band.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the font used by the band text.
        /// </summary>
        public Font Font { get; set; } = new Font("Times New Roman", 10.0f, FontStyle.Regular);

        /// <summary>
        /// Gets or sets a value indicating whether the band is rendered.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the text color.
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the fill color.
        /// </summary>
        public Color FillColor { get; set; }

        /// <summary>
        /// Gets or sets the vertical text alignment.
        /// </summary>
        public VertAlign? VertAlign { get; set; }

        /// <summary>
        /// Gets or sets the horizontal text alignment.
        /// </summary>
        public HorzAlign? HorzAlign { get; set; }

        /// <summary>
        /// Gets or sets the band height in centimeters.
        /// </summary>
        public float Height { get; set; } = 0.5f;
    }
}
