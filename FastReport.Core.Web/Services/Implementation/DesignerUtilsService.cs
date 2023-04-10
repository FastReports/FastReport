using FastReport.Utils;
using FastReport.Utils.Json;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Xml.Linq;

namespace FastReport.Web.Services
{
    internal sealed class DesignerUtilsService : IDesignerUtilsService
    {
        [Obsolete]
        internal static DesignerUtilsService Instance { get; } = new DesignerUtilsService();

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

                foreach (FunctionInfo item in list)
                {
                    if (item.Name == "Functions")
                    {
                        rootFunctions = item;
                        break;
                    }
                }

                xml.Root.Name = "ReportFunctions";
                if (rootFunctions != null)
                    RegisteredObjects.CreateFunctionsTree(report, rootFunctions, xml.Root);

                using (var stream = new MemoryStream())
                {
                    xml.Save(stream);
                    stream.Position = 0;
                    byte[] buff = new byte[stream.Length];
                    stream.Read(buff, 0, buff.Length);

                    return Encoding.UTF8.GetString(buff);
                }
            }
        }

        public string GetPropertiesJSON(string componentName)
        {
            if (ComponentInformationCache.ComponentPropertiesCache.TryGetValue(componentName, out string componentPropertiesJson))
                return componentPropertiesJson;

            var prefixes = new string[]
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

            ComponentInformationCache.ComponentPropertiesCache.Add(componentName, componentPropertiesJson);

            return componentPropertiesJson;
        }

        public string DesignerObjectPreview(WebReport webReport, string reportObj)
        {
            var sb = new StringBuilder();
            using (var report = new Report())
            {
                var obj = report.Xml(reportObj);
                //obj.SetReport(report);

                using (var html = new Export.Html.HTMLExport(true))
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
    }

    static class ComponentInformationCache
    {
        internal static readonly Dictionary<string, string> ComponentPropertiesCache = new Dictionary<string, string>();
        internal static readonly Assembly Assembly = typeof(Report).Assembly;
    }
}
