using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Configures the report title band.
    /// </summary>
    /// <typeparam name="T">The report row type.</typeparam>
    public class ReportTitleBuilder<T> : TextBandBuilderBase<ReportTitleBuilder<T>, T>
    {
        /// <summary>
        /// Initializes a report title builder.
        /// </summary>
        /// <param name="report">The owning report builder.</param>
        public ReportTitleBuilder(ReportBuilder<T> report)
            : base(report, report._reportTitle)
        {
        }

        /// <summary>
        /// Sets the report title font family and size using the default bold title style.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <param name="emSize">The font size in points.</param>
        /// <returns>The current title builder.</returns>
        public new ReportTitleBuilder<T> Font(string familyName, float emSize)
        {
            return Font(familyName, emSize, FontStyle.Regular | FontStyle.Bold);
        }

        /// <summary>
        /// Sets the report title font family using the default bold title size and style.
        /// </summary>
        /// <param name="familyName">The font family name.</param>
        /// <returns>The current title builder.</returns>
        public new ReportTitleBuilder<T> Font(string familyName)
        {
            return Font(familyName, 14, FontStyle.Regular | FontStyle.Bold);
        }
    }
}
