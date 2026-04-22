using System.Drawing;

namespace FastReport.ReportBuilder
{
    public class DataHeaderDefinition
    {
        public Font Font { get; set; } = new Font("Times New Roman", 10.0f, FontStyle.Regular);
        public bool Visible { get; set; } = true;
        public Color TextColor { get; set; } = Color.Black;
        public Color FillColor { get; set; } = Color.FromArgb(235, 243, 251);
    }
}
