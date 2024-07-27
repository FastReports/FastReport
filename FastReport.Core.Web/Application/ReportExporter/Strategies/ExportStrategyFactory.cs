using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.Image;

namespace FastReport.Web
{
    internal static class ExportStrategyFactory
    {
        public static IExportStrategy GetExportStrategy(ExportsHelper.ExportInfo exportInfo, ExportBase export)
        {
            IExportStrategy strategy;

            if (exportInfo.Export == Exports.Prepared)
                strategy = new PreparedExportStrategy();
            else if (export is HTMLExport { EmbedPictures: false } or ImageExport { SeparateFiles: true })
                strategy = new ArchiveExportStrategy();
            else if (export != null)
                strategy = new DefaultExportStrategy();
            else
                throw new UnsupportedExportException();

            return strategy;
        }
    }

}