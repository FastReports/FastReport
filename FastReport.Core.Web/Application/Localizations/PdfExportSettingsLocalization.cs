
namespace FastReport.Web.Application.Localizations
{
    internal class PdfExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Options;
        internal readonly string EmbeddedFonts;
        internal readonly string TextInCurves;
        internal readonly string InteractiveForms;
        internal readonly string Background;
        internal readonly string Images;
        internal readonly string JpegQuality;
        internal readonly string PrintOptimized;
        internal readonly string OriginalResolution;

        public PdfExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Pdf");
            Title = res.Get("");
            EmbeddedFonts = res.Get("EmbeddedFonts");
            TextInCurves = res.Get("TextInCurves");
            InteractiveForms = res.Get("InteractiveForms");
            Background = res.Get("Background");
            Images = res.Get("Images");
            JpegQuality = res.Get("JpegQuality");
            OriginalResolution = res.Get("OriginalResolution");

            res.Root("Export,Misc");
            PrintOptimized = res.Get("PrintOptimized");
            Options = res.Get("Options");
        }
    }
}
