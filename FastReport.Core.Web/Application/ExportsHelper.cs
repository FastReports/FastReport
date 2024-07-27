using System;
using System.Collections.Generic;
using System.Linq;
using FastReport.Export;
using FastReport.Export.Html;
using FastReport.Export.Image;
using System.Reflection;
using System.Diagnostics.CodeAnalysis;
#if !OPENSOURCE
using FastReport.Export.Csv;
using FastReport.Export.OoXML;
using FastReport.Export.Pdf;
using FastReport.Export.BIFF8;
using FastReport.Export.RichText;
using FastReport.Export.LaTeX;
using FastReport.Export.Zpl;
using FastReport.Export.Xml;
using FastReport.Export.Mht;
using FastReport.Export.Hpgl;
using FastReport.Export.Odf;
using FastReport.Export.Dbf;
using FastReport.Export.Text;
using FastReport.Export.XAML;
using FastReport.Export.Svg;
using FastReport.Export.Ppml;
using FastReport.Export.PS;
using FastReport.Export.Json;
using FastReport.Export.Dxf;
using FastReport.Export.Email;
#endif

namespace FastReport.Web
{
    internal static class ExportsHelper
    {
        static readonly ExportInfo[] ExportsCollection;

        static ExportsHelper()
        {
            ExportsCollection = new ExportInfo[]
            {
#if !OPENSOURCE
                new ExportInfo("latex", Exports.LaTeX, typeof(LaTeXExport), false),
                new ExportInfo("xls", Exports.Excel97, typeof(Excel2003Document), false),
                new ExportInfo("zpl", Exports.Zpl, typeof(ZplExport), false),
                new ExportInfo("hpgl", Exports.Hpgl, typeof(HpglExport), false),
                new ExportInfo("pdf", Exports.Pdf, typeof(PDFExport), true),
                new ExportInfo("rtf", Exports.Rtf, typeof(RTFExport), true),
                new ExportInfo("mht", Exports.Mht, typeof(MHTExport), false),
                new ExportInfo("xml", Exports.XmlExcel, typeof(XMLExport), true),
                new ExportInfo("docx", Exports.Word2007, typeof(Word2007Export), true),
                new ExportInfo("xlsx", Exports.Excel2007, typeof(Excel2007Export), true),
                new ExportInfo("pptx", Exports.PowerPoint2007, typeof(PowerPoint2007Export), true),
                new ExportInfo("ods", Exports.Ods, typeof(ODSExport), true),
                new ExportInfo("odt", Exports.Odt, typeof(ODTExport), true),
                new ExportInfo("xps", Exports.Xps, typeof(XPSExport), false),
                new ExportInfo("csv", Exports.Csv, typeof(CSVExport), false),
                new ExportInfo("dbf", Exports.Dbf, typeof(DBFExport), false),
                new ExportInfo("txt", Exports.Text, typeof(TextExport), false),
                new ExportInfo("xaml", Exports.Xaml, typeof(XAMLExport), false),
                new ExportInfo("svg", Exports.Svg, typeof(SVGExport), true),
                new ExportInfo("ppml", Exports.Ppml, typeof(PPMLExport), false),
                new ExportInfo("ps", Exports.PS, typeof(PSExport), false),
                new ExportInfo("json", Exports.Json, typeof(JsonExport), false),
                new ExportInfo("dxf", Exports.Dxf, typeof(DxfExport), false),
                new ExportInfo("email", Exports.Email, typeof(EmailExport), true),
                new ExportInfo("image", Exports.Image, typeof(ImageExport), true),
#endif
                new ExportInfo("html", Exports.HTML, typeof(HTMLExport), true),
                new ExportInfo("fpx", Exports.Prepared, null, false),
            };
        }

        internal static Exports GetExportFromExt(string extension)
        {
            var exportProperty = GetInfoFromExt(extension);
            return exportProperty.Export;
        }

        internal static string GetExtFromExportType(Exports type)
        {
            var exportProperty = GetInfoFromExport(type);
            return exportProperty.Extension;
        }

        internal static ExportBase GetExport(Exports type)
        {
            var exportProperty = GetInfoFromExport(type);

            return (ExportBase)Activator.CreateInstance(exportProperty.ExportType);
        }

        internal static ExportBase GetExport(string extension)
        {
            var exportProperty = GetInfoFromExt(extension);

            return (ExportBase)Activator.CreateInstance(exportProperty.ExportType);
        }


        internal static string GetLocalization(ToolbarLocalization localization, Exports type)
        {
            throw new NotImplementedException();
        }

#if BLAZOR && !WASM
        internal static string GetHref(WebReport webReport)
        {
            string href = webReport.template_export_url(GetExtFromExportType(webReport.ExportType));
            return href;
        }
#endif

        internal static ExportInfo GetInfoFromExt(string ext)
        {
            var exportProperty = ExportsCollection
                .FirstOrDefault(export => export.Extension == ext);
            if (exportProperty.Extension == null)
                throw new Exception("Unknown export extension");

            return exportProperty;
        }

        internal static ExportInfo GetInfoFromExport(Exports type)
        {
            var exportProperty = ExportsCollection
                .FirstOrDefault(export => export.Export == type);
            if (exportProperty.Extension == null)
                throw new Exception("Unknown export type");

            return exportProperty;
        }


        internal readonly struct ExportInfo
        {
            public readonly string Extension;
            public readonly Exports Export;
            [DynamicallyAccessedMembers(LinkerFlags.ExportTypeMembers)]
            public readonly Type ExportType;
            public readonly bool HaveSettings;

            public PropertyInfo[] Properties
                => ExportType?.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(property => property.CanWrite)
                            .ToArray();

            public ExportBase CreateExport()
            {
                if(Export != Exports.Prepared)
                    return (ExportBase)Activator.CreateInstance(ExportType);
                else
                    return null;
            }

            public ExportInfo(string extension, Exports export, [DynamicallyAccessedMembers(LinkerFlags.ExportTypeMembers)] Type exportType, bool haveSettings)
            {
                Extension = extension;
                ExportType = exportType;
                Export = export;
                HaveSettings = haveSettings;
            }
        }
    }
}