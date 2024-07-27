using FastReport.Export;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FastReport.Web
{
    internal sealed class ReportExporter
    {
        private readonly IExportStrategy _exportStrategy;

        public ReportExporter(IExportStrategy exportStrategy)
        {
            _exportStrategy = exportStrategy;
        }

        public void ExportReport(Stream stream, Report report, ExportBase export)
        {
            _exportStrategy.Export(stream, report, export);
        }

        public static ExportBase CreateExport(ExportsHelper.ExportInfo exportInfo,
            IEnumerable<KeyValuePair<string, string>> exportParameters)
        {
            var export = exportInfo.CreateExport();
            var exportParametersList = exportParameters.ToList();

            if (!exportInfo.HaveSettings || !exportParametersList.Any())
                return export;

            var availableProperties = exportInfo.Properties;
            if (availableProperties == null)
                return export;

            SetExportProperties(export, availableProperties, exportParametersList);

            return export;
        }

        private static void SetExportProperties(ExportBase export,
            IEnumerable<PropertyInfo> availableProperties,
            IEnumerable<KeyValuePair<string, string>> exportParameters)
        {
            foreach (var (key, propValue) in exportParameters)
            {
                var existProp = availableProperties.FirstOrDefault(prop => prop.Name == key);

                if (existProp == null) continue;

                var propValueConverted = ConvertPropertyValue(existProp.PropertyType, propValue);

                System.Diagnostics.Debug.WriteLine($"Export setting: {existProp.Name}: {propValueConverted}");

                existProp.SetMethod?.Invoke(export, new[] { propValueConverted });
            }
        }

        private static object ConvertPropertyValue(Type propertyType, string propValue)
        {
            return propertyType.IsEnum
                ? Enum.Parse(propertyType, propValue)
                : Convert.ChangeType(propValue, propertyType, CultureInfo.InvariantCulture);
        }
    }
}