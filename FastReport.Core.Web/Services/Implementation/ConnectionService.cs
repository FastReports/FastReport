using FastReport.Data;
using FastReport.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace FastReport.Web.Services
{
    internal sealed class ConnectionService : IConnectionsService
    {

        public string GetConnectionStringPropertiesJSON(string connectionType, string connectionString, out bool isError)
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
                isError = true;
                return "Connection type not found";
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
                isError = true;
                return ex.ToString();
            }

            isError = false;
            return $@"{{""properties"":[{data.ToString().TrimEnd(',')}]}}";
        }

        public string CreateConnectionStringJSON(string connectionType, IFormCollection form, out bool isError)
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
                isError = true;
                return "Connection type not found";
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
                            string propertyValue = form[pd.Name].ToString();
                            TypeConverter typeConverter = TypeDescriptor.GetConverter(pd.PropertyType);
                            object value = typeConverter.ConvertFromString(propertyValue);

                            object owner = conn;
                            if (conn is ICustomTypeDescriptor)
                                owner = ((ICustomTypeDescriptor)conn).GetPropertyOwner(pd);
                            pd.SetValue(owner, value);
                        }
                        catch (Exception ex)
                        {
                            isError = true;
                            return ex.ToString();
                        }
                    }

                    isError = false;
                    return $@"{{""connectionString"":""{JavaScriptEncoder.Default.Encode(conn.ConnectionString)}""}}";
                }
            }
            catch (Exception ex)
            {
                isError = true;
                return ex.ToString();
            }
        }

        public string GetConnectionTables(string connectionType, string connectionString)
        {
            if (!IsConnectionStringValid(connectionString, out var errorMsg))
            {
                throw new Exception(errorMsg);
            }

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

                            return Encoding.UTF8.GetString(ms.ToArray());
                        }
                    }
                }
                catch 
                {
                    throw new Exception("Error in creating tables. Please verify your connection string.");
                }
            }

            throw new Exception("Connection type not found");

        }

        private static bool IsConnectionStringValid(string connectionString, out string errorMsg)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                errorMsg = "Connection string is null or empty";
                return false;
            }

            errorMsg = string.Empty;
            return true;
        }

        public List<string> GetConnectionTypes()
        {
            var names = new List<string>();
            var objects = new List<DataConnectionInfo>();
            RegisteredObjects.DataConnections.EnumItems(objects);

            foreach (var info in objects)
                if (info.Object != null)
                    names.Add($@"""{info.Object.FullName}"":""{Res.TryGetBuiltin(info.Text)}""");

            return names;
        }
    }
}
