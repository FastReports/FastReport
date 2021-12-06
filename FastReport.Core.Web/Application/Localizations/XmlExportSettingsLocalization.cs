
namespace FastReport.Web.Application.Localizations
{
    internal class XmlExportSettingsLocalization
    {
        internal readonly string Title;

        internal readonly string Options;

        internal readonly string PageBreaks;

        internal readonly string DataOnly;


        public XmlExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Xml");
            Title = res.Get(""); 

            res.Root("Export,Misc");
            PageBreaks = res.Get("PageBreaks");
            Options = res.Get("Options");

            res.Root("Export,Text");
            DataOnly = res.Get("DataOnly");
        }
    }
}
