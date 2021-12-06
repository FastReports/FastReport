
namespace FastReport.Web.Application.Localizations
{
    internal class SvgExportSettingsLocalization
    {
        internal readonly string EmbPic;
        internal readonly string Title;
        //internal readonly string EmbeddedRTF;

        internal readonly string ToMultipleFiles;
        internal readonly string Pictures;
        //internal readonly string Metafile;
        //internal readonly string None;


        public SvgExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Svg");
            Title = res.Get("");
            EmbPic = res.Get("EmbPic");

            res.Root("Export,Misc");
            ToMultipleFiles = res.Get("ToMultipleFiles");
            Pictures = res.Get("Pictures");
        }
    }
}
