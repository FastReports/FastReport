using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.Image;
using System.IO;
using System.IO.Compression;

namespace FastReport.Web
{
    internal sealed class ArchiveExportStrategy : IExportStrategy
    {
        public void Export(Stream stream, Report report, ExportBase export)
        {
            EnableSaveSteams(export);

            export.Export(report, stream);

            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Create, true);
            var reportName = Path.GetFileNameWithoutExtension(report.FileName);

            // in html export the main report file is not saved in the generated streams, so we save it manually
            if (export is HTMLExport)
            {
                stream.Position = 0;
                AddStreamToZipArchive(zipArchive, $"{reportName}.html", stream);
            }

            AddGeneratedFilesToZipArchive(zipArchive, export);
        }

        private static void AddGeneratedFilesToZipArchive(ZipArchive zipArchive, ExportBase export)
        {
            for (var i = 0; i < export.GeneratedStreams.Count; i++)
                AddStreamToZipArchive(zipArchive, export.GeneratedFiles[i], export.GeneratedStreams[i]);
        }

        private static void AddStreamToZipArchive(ZipArchive zipArchive, string entryName, Stream stream)
        {
            stream.Position = 0;
            var entry = zipArchive.CreateEntry(entryName);
            using var entryStream = entry.Open();
            stream.CopyTo(entryStream);
        }

        private static void EnableSaveSteams(ExportBase export)
        {
            switch (export)
            {
                case HTMLExport htmlExport:
                    htmlExport.SaveStreams = true;
                    break;
                case ImageExport imageExport:
                    imageExport.SaveStreams = true;
                    break;
            }
        }
    }
}