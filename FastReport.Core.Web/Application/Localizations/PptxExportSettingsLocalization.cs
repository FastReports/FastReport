
namespace FastReport.Web.Application.Localizations
{
    internal class PptxExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Pictures;

        public PptxExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Pptx");
            Title = res.Get("");

            res.Root("Export,Misc");
            Pictures = res.Get("Pictures");
        }
    }
}
