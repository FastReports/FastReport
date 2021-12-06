
namespace FastReport.Web.Application.Localizations
{
    internal class ImageExportSettingsLocalization
    {
        internal readonly string Title;
        internal readonly string Format;
        internal readonly string Bmp;
        internal readonly string Png;
        internal readonly string Jpeg;
        internal readonly string Gif;
        internal readonly string Tiff;
        internal readonly string Metafile;
        internal readonly string Resolution;
        internal readonly string JpegQuality;
        internal readonly string MultiFrame;
        internal readonly string MonochromeTIFF;
        internal readonly string Separate;


        public ImageExportSettingsLocalization(IWebRes res)
        {
            res.Root("Export,Image");
            Title = res.Get("");
            Format = res.Get("ImageFormat");
            Bmp = res.Get("Bmp");
            Png = res.Get("Png");
            Jpeg = res.Get("Jpeg");
            Gif = res.Get("Gif");
            Tiff = res.Get("Tiff");
            Metafile = res.Get("Metafile");


            Resolution = res.Get("Resolution");
            JpegQuality = res.Get("Quality");
            MultiFrame = res.Get("MultiFrame");
            MonochromeTIFF = res.Get("Monochrome");
            Separate = res.Get("SeparateFiles");
        }
    }
}
