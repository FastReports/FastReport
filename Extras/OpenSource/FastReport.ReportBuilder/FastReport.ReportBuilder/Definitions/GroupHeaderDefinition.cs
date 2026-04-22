namespace FastReport.ReportBuilder
{
    public class GroupHeaderDefinition
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
        public string Expression { get; set; }
        public bool TextVisible { get; set; } = true;
        public float Height { get; set; }
    }
}
