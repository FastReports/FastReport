namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores layout and formatting information for a data column.
    /// </summary>
    public class DataDefinition
    {
        /// <summary>
        /// Gets or sets the displayed column title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the bound property name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the column width as a percentage value multiplied by ten.
        /// </summary>
        public uint Width { get; set; }

        /// <summary>
        /// Gets or sets the custom format string applied to the column value.
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Gets or sets the FastReport expression used to render the column value.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets the column vertical alignment override.
        /// </summary>
        public VertAlign? VertAlign { get; set; }

        /// <summary>
        /// Gets or sets the column horizontal alignment override.
        /// </summary>
        public HorzAlign? HorzAlign { get; set; }
    }
}
