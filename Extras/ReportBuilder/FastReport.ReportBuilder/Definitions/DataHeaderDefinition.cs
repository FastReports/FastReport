using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores appearance settings for the data header band.
    /// </summary>
    public class DataHeaderDefinition
    {
        /// <summary>
        /// Gets or sets the font used by header cells.
        /// </summary>
        public Font Font { get; set; } = new Font("Times New Roman", 10.0f, FontStyle.Regular);

        /// <summary>
        /// Gets or sets a value indicating whether the data header band is rendered.
        /// </summary>
        public bool Visible { get; set; } = true;

        /// <summary>
        /// Gets or sets the header text color.
        /// </summary>
        public Color TextColor { get; set; } = Color.Black;

        /// <summary>
        /// Gets or sets the header background fill color.
        /// </summary>
        public Color FillColor { get; set; } = Color.FromArgb(235, 243, 251);
    }
}
