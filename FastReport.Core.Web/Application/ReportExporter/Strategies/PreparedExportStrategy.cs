using FastReport.Export;
using System.IO;

namespace FastReport.Web
{
    internal sealed class PreparedExportStrategy : IExportStrategy
    {
        public void Export(Stream stream, Report report, ExportBase export)
        {
            report.SavePrepared(stream);
        }
    }
}