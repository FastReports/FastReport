using FastReport.Export;
using System.IO;

namespace FastReport.Web
{
    internal sealed class DefaultExportStrategy : IExportStrategy
    {
        public void Export(Stream stream, Report report, ExportBase export)
        {
            report.Export(export, stream);
        }
    }
}