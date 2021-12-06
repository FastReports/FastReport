
namespace FastReport.Web.Application.Localizations
{
    internal class RtfExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Options;
        internal readonly string RTFObjectAs;
        internal readonly string AsPicture;
        internal readonly string EmbeddedRTF;

        internal readonly string PageBreaks;
        internal readonly string Pictures;
        internal readonly string Metafile;
        internal readonly string None;


        public RtfExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,RichText");
            Title = res.Get(""); 
            RTFObjectAs = res.Get("RTFObjectAs");
            AsPicture = res.Get("Picture");
            EmbeddedRTF = res.Get("EmbeddedRTF");

            res.Root("Export,Misc");
            PageBreaks = res.Get("PageBreaks");
            Pictures = res.Get("Pictures");
            Metafile = res.Get("Metafile");
            None = res.Get("None");
            Options = res.Get("Options");
        }
    }
}
