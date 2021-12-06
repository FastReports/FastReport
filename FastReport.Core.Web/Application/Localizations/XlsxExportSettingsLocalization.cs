
namespace FastReport.Web.Application.Localizations
{
    internal class XlsxExportSettingsLocalization
    {
        internal readonly string Title;   
        internal readonly string Options;
        internal readonly string PrintScaling;
        internal readonly string NoScaling;
        internal readonly string FitSheetOnOnePage;
        internal readonly string FitAllColumsOnOnePage;
        internal readonly string FitAllRowsOnOnePage;
        internal readonly string FontScale;

        internal readonly string PageBreaks;
        internal readonly string Seamless;
        internal readonly string PrintOptimized;
        internal readonly string SplitPages;

        internal readonly string DataOnly;

        public XlsxExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Xlsx");
            Title = res.Get("");
            PrintScaling = res.Get("PrintScaling");
            NoScaling = res.Get("NoScaling");
            FitSheetOnOnePage = res.Get("FitSheetOnOnePage");
            FitAllColumsOnOnePage = res.Get("FitAllColumsOnOnePage");
            FitAllRowsOnOnePage = res.Get("FitAllRowsOnOnePage");
            FontScale = res.Get("FontScale");

            res.Root("Export,Misc");
            PageBreaks = res.Get("PageBreaks");
            Seamless = res.Get("Seamless");
            PrintOptimized = res.Get("PrintOptimized");
            SplitPages = res.Get("SplitPages");
            Options = res.Get("Options");

            res.Root("Export,Text");
            DataOnly = res.Get("DataOnly");
        }
    }
}
