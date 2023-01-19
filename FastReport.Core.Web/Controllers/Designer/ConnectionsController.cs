using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace FastReport.Web.Controllers.Designer
{
    public sealed class ConnectionsController : ApiControllerBase
    {
        public ConnectionsController()
        {

        }

        public sealed class ConnectionsParams
        {
            public string ConnectionType { get; set; }
            public string ConnectionString { get; set; }
        }

        #region Routes
        [HttpGet]
        [Route("/_fr/designer.getConnectionTypes")]
        public IActionResult GetConnectionTypes()
        {
            var names = new List<string>();
            var objects = new List<DataConnectionInfo>();
            RegisteredObjects.DataConnections.EnumItems(objects);

            foreach (var info in objects)
                if (info.Object != null)
                    names.Add($@"""{info.Object.FullName}"":""{Res.TryGetBuiltin(info.Text)}""");

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "{" + String.Join(",", names.ToArray()) + "}",
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getConnectionTables")]
        public IActionResult GetConnectionTables([FromQuery] ConnectionsParams query)
        {
            return GetConnectionTables(query.ConnectionType, query.ConnectionString);
        }

        [HttpPost]
        [Route("/_fr/designer.makeConnectionString")]
        public IActionResult MakeConnectionString(string connectionType)
        {
            return CreateConnectionString(connectionType);
        }

        [HttpGet]
        [Route("/_fr/designer.getConnectionStringProperties")]
        public IActionResult GetConnectionStringProperties([FromQuery] ConnectionsParams query)
        {
            return GetConnectionStringProperties(query.ConnectionType, query.ConnectionString);
        }
        #endregion

        #region PrivateMethods
        private IActionResult GetConnectionTables(string connectionType, string connectionString)
        {
            var objects = new List<DataConnectionInfo>();
            RegisteredObjects.DataConnections.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
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

        private IActionResult CreateConnectionString(string connectionType)
        {
            var objects = new List<DataConnectionInfo>();
            RegisteredObjects.DataConnections.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
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

        private IActionResult GetConnectionStringProperties(string connectionType, string connectionString)
        {
            var objects = new List<DataConnectionInfo>();
            RegisteredObjects.DataConnections.EnumItems(objects);
            Type connType = null;

            foreach (var info in objects)
                if (info.Object != null &&
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
        #endregion
    }
}
