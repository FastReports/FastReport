namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Configures the page footer band.
    /// </summary>
    /// <typeparam name="T">The report row type.</typeparam>
    public class PageFooterBuilder<T> : TextBandBuilderBase<PageFooterBuilder<T>, T>
    {
        /// <summary>
        /// Initializes a page footer builder.
        /// </summary>
        /// <param name="report">The owning report builder.</param>
        public PageFooterBuilder(ReportBuilder<T> report)
            : base(report, report._pageFooter)
        {
        }
    }
}
