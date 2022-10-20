using FastReport.Data.JsonConnection;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;

namespace FastReport.Data.ElasticSearch
{
    public partial class ESDataSourceConnection : DataConnectionBase, IJsonProviderSourceConnection
    {
        #region Private Fields
        private List<JsonDataSourceConnection> connectionCollection;
        private ESDataSourceConnectionStringBuilder sourceConnectionStringBuilder;
        private string[] tableNames;
        #endregion Private Fields

        #region Public Methods

        public ESDataSourceConnection()
        {
            IsSqlBased = false;
        }

        public override void CreateAllTables(bool initSchema)
        {
            if (connectionCollection == null)
                InitConnection();

            bool found = false;
            foreach (Base b in Tables)
            {
                if (b is JsonTableDataSource)
                {
                    (b as JsonTableDataSource).UpdateSchema = true;
                    (b as JsonTableDataSource).InitSchema();
                    found = true;
                    break;
                }
            }

            for (int i = 0; i < connectionCollection.Count; i++)
            {

                if (!found)
                {
                    JsonTableDataSource jsonDataSource = new JsonTableDataSource();

                    string fixedTableName = tableNames[i].Replace(".", "_").Replace("[", "").Replace("]", "").Replace("\"", "");
                    jsonDataSource.TableName = fixedTableName;

                    if (Report != null)
                    {
                        jsonDataSource.Name = Report.Dictionary.CreateUniqueName(fixedTableName);
                        jsonDataSource.Alias = Report.Dictionary.CreateUniqueAlias(jsonDataSource.Alias);
                    }
                    else
                        jsonDataSource.Name = fixedTableName;

                    jsonDataSource.Parent = connectionCollection[i];
                    jsonDataSource.InitSchema();
                    jsonDataSource.Enabled = false;
                    Tables.Add(jsonDataSource);
                }
            }

            // init table schema
            if (initSchema)
            {
                foreach (TableDataSource table in Tables)
                {
                    table.InitSchema();
                }
            }
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        ///// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {

        }
        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
        }

        public override string[] GetTableNames()
        {
            string respoce;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create($"{sourceConnectionStringBuilder.EndPoint}/_alias");
            req.Method = "GET";
            foreach (var header in sourceConnectionStringBuilder.Headers)
            {
                req.Headers.Add(header.Key, header.Value);
            }

            using (var response = req.GetResponse() as HttpWebResponse)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                    respoce = reader.ReadToEnd();
            }
            List<string> Names = new List<string>();

            foreach (string name in JsonBase.FromString(respoce).Keys)
                Names.Add(name);

            return Names.ToArray();
        }

        #endregion

        #region Private Methods
        private int GetRecCount(string teableName)
        {
            string responce;
            HttpWebRequest req;
            req = (HttpWebRequest)WebRequest.Create($"{sourceConnectionStringBuilder.EndPoint}/{teableName}/_count");
            req.Method = "GET";
            foreach (var header in sourceConnectionStringBuilder.Headers)
            {
                req.Headers.Add(header.Key, header.Value);
            }

            using (var response = req.GetResponse() as HttpWebResponse)
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                    responce = reader.ReadToEnd();
            }

            int countRec = 0;
            int.TryParse(JsonBase.FromString(responce)["count"].ToString(), out countRec);
            return countRec;
        }

        private void InitConnection()
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            connectionCollection = new List<JsonDataSourceConnection>();
            JsonDataSourceConnection jsonData;
            int countRec;
            sourceConnectionStringBuilder = new ESDataSourceConnectionStringBuilder(ConnectionString);
            tableNames = GetTableNames();

            foreach (var tableName in tableNames)
            {
                countRec = GetRecCount(tableName);
                JsonDataSourceConnectionStringBuilder connectionStringBuilder = new JsonDataSourceConnectionStringBuilder();
                connectionStringBuilder.ConnectionString = 
                    $"Json={sourceConnectionStringBuilder.EndPoint}/{tableName}/_search?size={countRec};JsonShema=;Encoding=utf-8;";
                connectionStringBuilder.Headers = sourceConnectionStringBuilder.Headers;
                jsonData = new JsonDataSourceConnection()
                {
                    ConnectionString = connectionStringBuilder.ToString()
                };
                connectionCollection.Add(jsonData);
            }
        }

        public JsonBase GetJson(TableDataSource tableDataSource)
        {
            // I was move code to here
            // Here it’s completely fail,
            // I commented out the code with ElasticSearch,
            // when you bring ElasticSearch into a separate plugin,
            // then this will need to be corrected
            sourceConnectionStringBuilder = new ESDataSourceConnectionStringBuilder(ConnectionString);

            if (tableNames == null)
                tableNames = GetTableNames();

            if (tableNames == null || !tableNames.Contains(tableDataSource.Name))
                return null;

            int countRec = GetRecCount(tableDataSource.Name);
            JsonDataSourceConnectionStringBuilder connectionStringBuilder = new JsonDataSourceConnectionStringBuilder();

            connectionStringBuilder.ConnectionString =
                $"Json={sourceConnectionStringBuilder.EndPoint}/{tableDataSource.Name}/_search?size={countRec};JsonShema=;Encoding=utf-8;";

            connectionStringBuilder.Headers = sourceConnectionStringBuilder.Headers;

            return (new JsonDataSourceConnection() { ConnectionString = connectionStringBuilder.ToString() }).GetJson(tableDataSource);
        }
        #endregion
    }
}
