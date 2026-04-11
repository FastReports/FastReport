namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores grouping settings for the group header band.
    /// </summary>
    public class GroupHeaderDefinition
    {
        /// <summary>
        /// Gets or sets the property name used for grouping.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether grouping is enabled.
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Gets or sets the sort order applied to grouped values.
        /// </summary>
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

        /// <summary>
        /// Gets or sets an optional expression used to transform the group value.
        /// </summary>
        public string Expression { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the group header text is shown.
        /// </summary>
        public bool TextVisible { get; set; } = true;

        /// <summary>
        /// Gets or sets the group header height in centimeters.
        /// </summary>
        public float Height { get; set; }
    }
}
