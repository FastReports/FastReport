using System.Drawing;

namespace FastReport.ReportBuilder
{
    /// <summary>
    /// Stores presentation settings for the report title band.
    /// </summary>
    public class ReportTitleDefinition : TextBandDefinition
    {
        /// <summary>
        /// Initializes a new report title definition with title-specific defaults.
        /// </summary>
        public ReportTitleDefinition()
        {
            Font = new Font("Times New Roman", 14, FontStyle.Bold | FontStyle.Regular);
            Height = 1.0f;
        }
    }
}
