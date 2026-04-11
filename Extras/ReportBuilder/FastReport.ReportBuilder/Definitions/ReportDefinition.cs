using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores page-level report defaults and layout settings.
    /// </summary>
    public class ReportDefinition
    {
        /// <summary>
        /// Gets or sets the default font for report text objects.
        /// </summary>
        public Font Font { get; set; } = new Font("Times New Roman", 10.0f, FontStyle.Regular);

        /// <summary>
        /// Gets or sets the default vertical alignment for report text objects.
        /// </summary>
        public VertAlign VertAlign { get; set; }

        /// <summary>
        /// Gets or sets the default horizontal alignment for report text objects.
        /// </summary>
        public HorzAlign HorzAlign { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether pages use landscape orientation.
        /// </summary>
        public bool Landscape { get; set; }

        /// <summary>
        /// Gets or sets the paper width in millimeters.
        /// </summary>
        public float? PaperWidth { get; set; }

        /// <summary>
        /// Gets or sets the paper height in millimeters.
        /// </summary>
        public float? PaperHeight { get; set; }

        /// <summary>
        /// Gets or sets the left margin in millimeters.
        /// </summary>
        public float LeftMargin { get; set; } = 10;

        /// <summary>
        /// Gets or sets the top margin in millimeters.
        /// </summary>
        public float TopMargin { get; set; } = 10;

        /// <summary>
        /// Gets or sets the right margin in millimeters.
        /// </summary>
        public float RightMargin { get; set; } = 10;

        /// <summary>
        /// Gets or sets the bottom margin in millimeters.
        /// </summary>
        public float BottomMargin { get; set; } = 10;
    }
}
