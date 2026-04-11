namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Configures the page header band.
    /// </summary>
    /// <typeparam name="T">The report row type.</typeparam>
    public class PageHeaderBuilder<T> : TextBandBuilderBase<PageHeaderBuilder<T>, T>
    {
        /// <summary>
        /// Initializes a page header builder.
        /// </summary>
        /// <param name="report">The owning report builder.</param>
        public PageHeaderBuilder(ReportBuilder<T> report)
            : base(report, report._pageHeader)
        {
        }
    }
}
