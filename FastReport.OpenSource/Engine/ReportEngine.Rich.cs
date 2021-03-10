namespace FastReport.Engine
{
    partial class ReportEngine
    {
        private void InitializePages()
        {
            for (int i = 0; i < Report.Pages.Count; i++)
            {
                ReportPage page = Report.Pages[i] as ReportPage;
                if (page != null)
                    PreparedPages.AddSourcePage(page);
            }
        }
    }
}