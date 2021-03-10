using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;

namespace FastReport.Web.Controllers
{
    class DesignerController : BaseController
    {
        #region Routes

        public DesignerController() : base()
        {
            RegisterHandler("/designer.getReport", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                return webReport.DesignerGetReport();
            });

            RegisterHandler("/designer.saveReport", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                if (webReport.DesignerSaveMethod == null)
                {
                    // old saving way by self-request
                    return webReport.DesignerSaveReport(Context);
                }
                else
                {
                    // save by using a Func

                    string report = webReport.GetPOSTReport(Context);
                    string msg = string.Empty;
                    int code = 200;
                    try
                    {
                        msg = webReport.DesignerSaveMethod(webReport.ID, webReport.ReportFileName, report);
                    }
                    catch(Exception ex)
                    {
                        code = 500;
                        msg = ex.Message;
                    }

                    var result = new ContentResult()
                    {
                        StatusCode = code,
                        ContentType = "text/html",
                        Content = msg
                    };
                    return result;
                }
            });

            RegisterHandler("/designer.previewReport", async () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                return await webReport.DesignerMakePreview(Context);
            });

            RegisterHandler("/designer.getConfig", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = webReport.DesignerConfig.IsNullOrWhiteSpace() ? "{}" : webReport.DesignerConfig,
                };
            });

            RegisterHandler("/designer.getFunctions", () =>
            {
                if (!FindWebReport(out WebReport webReport))
                    return new NotFoundResult();

                return GetFunctions(webReport.Report);
            });

            RegisterHandler("/designer.getConnectionTypes", () =>
            {
                return GetConnectionTypes();
            });

            RegisterHandler("/designer.getConnectionTables", () =>
            {
                var connectionType = Request.Query["connectionType"].ToString();
                var connectionString = Request.Query["connectionString"].ToString();

                return GetConnectionTables(connectionType, connectionString);
            });

            RegisterHandler("/designer.getConnectionStringProperties", () =>
            {
                return GetConnectionStringProperties();
            });

            RegisterHandler("/designer.makeConnectionString", () =>
            {
                return MakeConnectionString();
            });
        }

        #endregion

        #region Private Methods

        bool FindWebReport(out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(Request.Query["reportId"].ToString());
            return webReport != null;
        }

        IActionResult GetConnectionTypes()
        {
            var names = new List<string>();
            var objects = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(objects);

            foreach (var info in objects)
                if (info.Object != null && info.Object.IsSubclassOf(typeof(DataConnectionBase)))
                    names.Add($@"""{info.Object.FullName}"":""{Res.TryGetBuiltin(info.Text)}""");

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "{" + String.Join(",", names.ToArray()) + "}",
            };
        }

        IActionResult GetConnectionTables(string connectionType, string connectionString)
        {
            var objects = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
                    info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                    info.Object.FullName == connectionType)
                {
                    connType = info.Object;
                    break;
                }

            if (connType != null)
            {
                try
                {
                    using (DataConnectionBase conn = (DataConnectionBase)Activator.CreateInstance(connType))
                    using (var writer = new FRWriter())
                    {
                        conn.ConnectionString = connectionString;
                        conn.CreateAllTables(true);

                        foreach (TableDataSource c in conn.Tables)
                            c.Enabled = true;

                        writer.SaveChildren = true;
                        writer.WriteHeader = false;
                        writer.Write(conn);

                        using (var ms = new MemoryStream())
                        {
                            writer.Save(ms);
                            ms.Position = 0;

                            return new ContentResult()
                            {
                                StatusCode = (int)HttpStatusCode.OK,
                                ContentType = "application/xml",
                                Content = Encoding.UTF8.GetString(ms.ToArray()),
                            };
                        }
                    }
                }
                catch (Exception ex)
                {
                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.InternalServerError,
                        ContentType = "text/plain",
                        Content = ex.ToString(),
                    };
                }
            }
            else
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = "Connection type not found",
                };
            }
        }

        IActionResult GetFunctions(Report report)
        {
            using (var xml = new XmlDocument())
            {
                xml.AutoIndent = true;
                var list = new List<ObjectInfo>();
                RegisteredObjects.Objects.EnumItems(list);
                ObjectInfo rootFunctions = null;

                foreach (ObjectInfo item in list)
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

        IActionResult GetConnectionStringProperties()
        {
            var connectionType = Request.Query["connectionType"].ToString();
            var connectionString = Request.Query["connectionString"].ToString();

            var objects = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
                    info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                    info.Object.FullName == connectionType)
                {
                    connType = info.Object;
                    break;
                }

            if (connType == null)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = "Connection type not found",
                };
            }

            var data = new StringBuilder();

            // this piece of code mimics functionality of PropertyGrid: finds available properties
            try
            {
                using (var conn = (DataConnectionBase)Activator.CreateInstance(connType))
                {
                    conn.ConnectionString = connectionString;
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(conn);

                    foreach (PropertyDescriptor pd in props)
                    {
                        if (!pd.IsBrowsable || pd.IsReadOnly)
                            continue;

                        if (pd.Name == "Name" ||
                            pd.Name == "ConnectionString" ||
                            pd.Name == "ConnectionStringExpression" ||
                            pd.Name == "LoginPrompt" ||
                            pd.Name == "CommandTimeout" ||
                            pd.Name == "Alias" ||
                            pd.Name == "Description" ||
                            pd.Name == "Restrictions")
                            continue;

                        object value = null;

                        try
                        {
                            object owner = conn;
                            if (conn is ICustomTypeDescriptor)
                                owner = ((ICustomTypeDescriptor)conn).GetPropertyOwner(pd);
                            value = pd.GetValue(owner);
                        }
                        catch { }

                        data.Append("{");
                        data.Append("\"name\":\"" + JavaScriptEncoder.Default.Encode(pd.Name) + "\",");
                        data.Append("\"displayName\":\"" + JavaScriptEncoder.Default.Encode(pd.DisplayName) + "\",");
                        data.Append("\"description\":\"" + JavaScriptEncoder.Default.Encode(pd.Description) + "\",");
                        data.Append("\"value\":\"" + JavaScriptEncoder.Default.Encode(value == null ? "" : value.ToString()) + "\",");
                        data.Append("\"propertyType\":\"" + JavaScriptEncoder.Default.Encode(pd.PropertyType.FullName) + "\"");
                        data.Append("},");
                    }
                }
            }
            catch (Exception ex)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = ex.ToString(),
                };
            }

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = $@"{{""properties"":[{data.ToString().TrimEnd(',')}]}}",
            };
        }

        IActionResult MakeConnectionString()
        {
            var connectionType = Request.Query["connectionType"].ToString();

            var objects = new List<ObjectInfo>();
            RegisteredObjects.Objects.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
                    info.Object.IsSubclassOf(typeof(DataConnectionBase)) &&
                    info.Object.FullName == connectionType)
                {
                    connType = info.Object;
                    break;
                }

            if (connType == null)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = "Connection type not found",
                };
            }

            try
            {
                using (var conn = (DataConnectionBase)Activator.CreateInstance(connType))
                {
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(conn);

                    foreach (PropertyDescriptor pd in props)
                    {
                        if (!pd.IsBrowsable || pd.IsReadOnly)
                            continue;

                        if (pd.Name == "Name" ||
                            pd.Name == "ConnectionString" ||
                            pd.Name == "ConnectionStringExpression" ||
                            pd.Name == "LoginPrompt" ||
                            pd.Name == "CommandTimeout" ||
                            pd.Name == "Alias" ||
                            pd.Name == "Description" ||
                            pd.Name == "Restrictions")
                            continue;

                        try
                        {
                            string propertyValue = Request.Form[pd.Name].ToString();
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(pd.PropertyType);
                            object value = typeConverter.ConvertFromString(propertyValue);

                            object owner = conn;
                            if (conn is ICustomTypeDescriptor)
                                owner = ((ICustomTypeDescriptor)conn).GetPropertyOwner(pd);
                            pd.SetValue(owner, value);
                        }
                        catch (Exception ex)
                        {
                            return new ContentResult()
                            {
                                StatusCode = (int)HttpStatusCode.InternalServerError,
                                ContentType = "text/plain",
                                Content = ex.ToString(),
                            };
                        }
                    }

                    return new ContentResult()
                    {
                        StatusCode = (int)HttpStatusCode.OK,
                        ContentType = "application/json",
                        Content = $@"{{""connectionString"":""{JavaScriptEncoder.Default.Encode(conn.ConnectionString)}""}}",
                    };
                }
            }
            catch (Exception ex)
            {
                return new ContentResult()
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    ContentType = "text/plain",
                    Content = ex.ToString(),
                };
            }
        }

        #endregion
    }
}
