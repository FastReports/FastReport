using FastReport.Export;
using System.IO;

namespace FastReport.Web
{
    internal interface IExportStrategy
    {
        void Export(Stream stream, Report report, ExportBase export);
    }
}