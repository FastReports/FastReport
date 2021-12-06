
namespace FastReport.Web.Application.Localizations
{
    internal class HtmlExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Options;
        internal readonly string Layers;
        internal readonly string EmbPic;
        internal readonly string SinglePage;
        internal readonly string Navigator;
        internal readonly string SubFolder;
        internal readonly string Pictures;

        public HtmlExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Html");
            Title = res.Get("");
            Layers = res.Get("Layers");
            EmbPic = res.Get("EmbPic");
            SinglePage = res.Get("SinglePage");
            Navigator = res.Get("Navigator");
            SubFolder = res.Get("SubFolder");

            res.Root("Export,Misc");
            Pictures = res.Get("Pictures");
            Options = res.Get("Options");
        }
    }
}
