using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using FastReport.Export.Html;
using FastReport.Utils;
using FastReport.Utils.Json;
using FastReport.Web.Infrastructure;
using FastReport.Web.Services.Helpers;

namespace FastReport.Web.Services
{
    internal sealed class DesignerUtilsService : IDesignerUtilsService
    {
        private const string IsCustomSqlAllowedKey = "custom-sql-allowed";
        private const string EnableIntellisenseKey = "intellisense-enabled";

        private static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        private readonly DesignerOptions _designerOptions;
        private readonly IntelliSenseHelper _intelliSenseHelper;

        public DesignerUtilsService(DesignerOptions designerOptions)
        {
            _designerOptions = designerOptions;
            _intelliSenseHelper = new IntelliSenseHelper(designerOptions.IntelliSenseAssemblies);
        }

        public string GetMSChartTemplateXML(string templateName)
        {
            var stream = ResourceLoader.GetStream("MSChart." + templateName + ".xml");

            try
            {
                return new StreamReader(stream).ReadToEnd();
            }
            catch
            {
                return null;
            }
        }

        public string GetFunctions(Report report)
        {
            using (var xml = new XmlDocument())
            {
                xml.AutoIndent = true;
                var list = new List<FunctionInfo>();
                RegisteredObjects.Functions.EnumItems(list);
                FunctionInfo rootFunctions = null;

                foreach (var item in list)
                    if (item.Name == "Functions")
                    {
                        rootFunctions = item;
                        break;
                    }

                xml.Root.Name = "ReportFunctions";
                if (rootFunctions != null)
                    RegisteredObjects.CreateFunctionsTree(report, rootFunctions, xml.Root);

                using (var stream = new MemoryStream())
                {
                    xml.Save(stream);
                    stream.Position = 0;
                    var buff = new byte[stream.Length];
                    stream.Read(buff, 0, buff.Length);

                    return Encoding.UTF8.GetString(buff);
                }
            }
        }

        public string GetPropertiesJSON(string componentName)
        {
            if (ComponentInformationCache.ComponentPropertiesCache.TryGetValue(componentName,
                    out var componentPropertiesJson))
                return componentPropertiesJson;

            var prefixes = new[]
            {
                "", "SVG.", "MSChart.", "Dialog.", "AdvMatrix.", "Table.", "Barcode.", "Map.", "CrossView.", "Matrix.",
                "Gauge.Simple.", "Gauge.Radial.", "Gauge.Linear.", "Gauge.Simple.Progress."
            };
            Type type = null;

            foreach (var prefix in prefixes)
            {
                type = ComponentInformationCache.Assembly.GetType("FastReport." + prefix + componentName);
                if (type != null)
                    break;
            }

            if (type is null || !type.IsPublic)
                return null;

            var jsonObject = new JsonObject();

            // Doesn't return collections because they don't have a setter (Example - TextObject.Highlight)
            foreach (var property in type.GetProperties())
            {
                if (!property.CanWrite) continue;

                var isVisible = true;
                var defaultValue = "null";
                var category = "Misc";

                foreach (var attribute in property.GetCustomAttributes())
                    switch (attribute)
                    {
                        case CategoryAttribute categoryAttribute:
                            category = categoryAttribute.Category;
                            break;
                        case DefaultValueAttribute defaultValueAttribute:
                            defaultValue = defaultValueAttribute.Value.ToString();
                            break;
                        case BrowsableAttribute browsableAttribute:
                            isVisible = browsableAttribute.Browsable;
                            break;
                    }

                if (isVisible)
                    jsonObject[$@"{category}:{property.Name}"] = defaultValue;
            }

            componentPropertiesJson = jsonObject.ToString();

            ComponentInformationCache.ComponentPropertiesCache.TryAdd(componentName, componentPropertiesJson);

            return componentPropertiesJson;
        }

        public string DesignerObjectPreview(WebReport webReport, string reportObj)
        {
            var sb = new StringBuilder();
            using (var report = new Report())
            {
                var obj = report.Xml(reportObj);
                //obj.SetReport(report);

                using (var html = new HTMLExport(true))
                {
                    html.StylePrefix = obj.Name.Trim();
                    html.SetReport(report);
                    html.ExportReportObject(obj);
                    sb.Append("<div>");
                    //sb.Append("<div ")
                    //    .Append(" style=\"position:relative;")
                    //    .Append(" width:").Append(Px(maxWidth * Zoom + 3))
                    //    .Append(" height:").Append(Px(maxHeight * Zoom))
                    //    .Append("\">");
                    webReport.DoHtmlPage(sb, html, 0);
                }
            }

            return sb.ToString();
        }

        public string GetConfig(WebReport webReport)
        {
            JsonBase config;

            try
            {
                config = JsonBase.FromString(webReport.Designer.Config);
            }
            catch
            {
                config = new JsonObject();
            }

            config[IsCustomSqlAllowedKey] = _designerOptions.AllowCustomSqlQueries;
            config[EnableIntellisenseKey] = _designerOptions.EnableIntelliSense;

            return config.ToString();
        }

        public string GetNamespacesInfoJson(IReadOnlyCollection<string> namespaces)
        {
            var result = _intelliSenseHelper.GetNamespacesInfo(namespaces);

            return JsonSerializer.Serialize(result, JsonSerializerOptions);
        }

        public string GetClassDetailsJson(string className)
        {
            var classInfo = _intelliSenseHelper.GetClassDetails(className);

            return JsonSerializer.Serialize(classInfo, JsonSerializerOptions);
        }
    }

    internal static class ComponentInformationCache
    {
        internal static readonly Dictionary<string, string> ComponentPropertiesCache = new();
        internal static readonly Assembly Assembly = typeof(Report).Assembly;
    }
}