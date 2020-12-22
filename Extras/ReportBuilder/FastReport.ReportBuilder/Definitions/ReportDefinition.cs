using System.Drawing;

namespace FastReport.ReportBuilder
{
    public class ReportDefinition
    {
        public Font Font { get; set; } = new Font("Times New Roman", 10.0f, FontStyle.Regular);
        public VertAlign VertAlign { get; set; }
        public HorzAlign HorzAlign { get; set; }
    }
}
