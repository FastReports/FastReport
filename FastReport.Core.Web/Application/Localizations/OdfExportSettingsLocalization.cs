
namespace FastReport.Web.Application.Localizations
{
    internal class OdfExportSettingsLocalization
    {
        internal readonly string OdtTitle;
        internal readonly string OdsTitle;
        internal readonly string Compliance;
        internal readonly string Options;
        internal readonly string PageBreaks;


        public OdfExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Odt");
            OdtTitle = res.Get("");

            res.Root("Export,Ods");
            OdsTitle = res.Get("");

            res.Root("Export,Odf");
            Compliance = res.Get("Compliance");
           
            res.Root("Export,Misc");
            PageBreaks = res.Get("PageBreaks");
            Options = res.Get("Options");
        }
    }
}
