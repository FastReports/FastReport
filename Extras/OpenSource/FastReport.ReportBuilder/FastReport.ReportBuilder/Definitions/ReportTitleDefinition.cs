using System.Drawing;

namespace FastReport.ReportBuilder
{
    public class ReportTitleDefinition
    {
        public string Text { get; set; }
        public Font Font { get; set; } = new Font("Times New Roman", 14, FontStyle.Bold | FontStyle.Regular);
        public bool Visible { get; set; }
        public Color TextColor { get; set; } = Color.Black;
        public Color FillColor { get; set; }
        public VertAlign? VertAlign { get; set; }
        public HorzAlign? HorzAlign { get; set; }
    }
}
