namespace FastReport.ReportBuilder
{
    public class DataDefinition
    {
        public string Title { get; set; }
        public string Name { get; set; }
        public uint Width { get; set; }
        public string Format { get; set; }
        public string Expression { get; set; }
        public VertAlign? VertAlign { get; set; }
        public HorzAlign? HorzAlign { get; set; }
    }
}
