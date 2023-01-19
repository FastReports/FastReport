using FastReport.Utils;
using FastReport.Utils.Json;
using FastReport.Web.Cache;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace FastReport.Web.Controllers.Designer
{
    public sealed class UtilsController : ApiControllerBase
    {
        public UtilsController()
        {

        }

        #region Routes
        [HttpGet]
        [Route("/_fr/designer.objects/mschart/template")]
        public IActionResult GetMSChartTemplate(string name)
        {
            string result;
            var stream = ResourceLoader.GetStream("MSChart." + name + ".xml");

            try
            {
                result = new StreamReader(stream).ReadToEnd();
            }
            catch (Exception ex)
            {
                return new NotFoundResult();
            };

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = result
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getComponentProperties")]
        public IActionResult GetComponentProperties(string name)
        {
            string responseJson = GetProperties(name);

            if (responseJson.IsNullOrEmpty())
                return new NotFoundResult();
            else
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = responseJson
                };
        }

        [HttpGet]
        [Route("/_fr/designer.getConfig")]
        public IActionResult GetConfig(string reportId)
        {
            if (!TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = webReport.Designer.Config.IsNullOrWhiteSpace() ? "{}" : webReport.Designer.Config,
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getFunctions")]
        public IActionResult GetFunctions(string reportId)
        {
            if (!TryFindWebReport(reportId, out WebReport webReport))
                return new NotFoundResult();

            return GetFunctions(webReport.Report);
        }
        #endregion

        #region PrivateMethods
        private IActionResult GetFunctions(Report report)
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

                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "application/xml",
                        Content = Encoding.UTF8.GetString(buff),
                    };
                }
            }
        }

        private string GetProperties(string componentName)
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

        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }
        #endregion
    }
}
