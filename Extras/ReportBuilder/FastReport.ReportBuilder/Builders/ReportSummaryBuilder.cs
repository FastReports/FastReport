namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Configures the report summary band.
    /// </summary>
    /// <typeparam name="T">The report row type.</typeparam>
    public class ReportSummaryBuilder<T> : TextBandBuilderBase<ReportSummaryBuilder<T>, T>
    {
        /// <summary>
        /// Initializes a report summary builder.
        /// </summary>
        /// <param name="report">The owning report builder.</param>
        public ReportSummaryBuilder(ReportBuilder<T> report)
            : base(report, report._reportSummary)
        {
        }
    }
}
