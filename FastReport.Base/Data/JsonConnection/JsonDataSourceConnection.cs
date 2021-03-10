using FastReport.Data.JsonConnection.JsonParser;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace FastReport.Data.JsonConnection
{
    /// <summary>
    /// FastReport json connection
    /// </summary>
    public partial class JsonDataSourceConnection : DataConnectionBase
    {
        #region Public Fields

        /// <summary>
        /// Name of json object table
        /// </summary>
        public const string TABLE_NAME = "JSON";

        #endregion Public Fields

        #region Private Fields

        private JsonArray jsonInternal = null;
        private JsonSchema jsonSchema = null;
        private string jsonSchemaString = "";

        #endregion Private Fields

        #region Internal Properties

        internal JsonArray Json
        {
            get
            {
                if (jsonInternal == null)
                    InitConnection();
                return jsonInternal;
            }
        }

        internal JsonSchema JsonSchema
        {
            get
            {
                if (jsonSchema == null)
                    InitConnection();
                return jsonSchema;
            }
        }

        #endregion Internal Properties

        #region Public Constructors

        /// <summary>
        /// Initialize a new instance
        /// </summary>
        public JsonDataSourceConnection()
        {
            IsSqlBased = false;
        }

        #endregion Public Constructors

        #region Public Methods

        /// <inheritdoc/>
        public override void CreateAllTables(bool initSchema)
        {
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

            if (!found)
            {
                JsonTableDataSource jsonDataSource = new JsonTableDataSource();

                string fixedTableName = TABLE_NAME;
                jsonDataSource.TableName = fixedTableName;

                if (Report != null)
                {
                    jsonDataSource.Name = Report.Dictionary.CreateUniqueName(fixedTableName);
                    jsonDataSource.Alias = Report.Dictionary.CreateUniqueAlias(jsonDataSource.Alias);
                }
                else
                    jsonDataSource.Name = fixedTableName;

                jsonDataSource.Parent = this;
                jsonDataSource.InitSchema();
                jsonDataSource.Enabled = true;
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

        /// <inheritdoc/>
        public override void CreateRelations()
        {
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void DeleteTable(TableDataSource source)
        {
            //throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            return new string[] { TABLE_NAME };
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        #endregion Public Methods

        #region Protected Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        protected override void SetConnectionString(string value)
        {
            jsonInternal = null;
            base.SetConnectionString(value);
        }

        #endregion Protected Methods

        #region Private Methods

        private void InitConnection()
        {
            InitConnection(false);
        }

        private void InitConnection(bool rebuildSchema)
        {
            JsonDataSourceConnectionStringBuilder builder = new JsonDataSourceConnectionStringBuilder(ConnectionString);
            JsonBase obj = null;
            string jsonText = builder.Json.Trim();
            if (jsonText.Length > 0)
            {
                if (!(jsonText[0] == '{' || jsonText[0] == '['))
                {
                    //using (WebClient client = new WebClient())
                    //{
                    //    try
                    //    {
                    //        client.Encoding = Encoding.GetEncoding(builder.Encoding);
                    //    }
                    //    catch
                    //    {
                    //        client.Encoding = Encoding.UTF8;
                    //    }
                    //    jsonText = client.DownloadString(jsonText);
                    //}

                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(jsonText);

                    foreach (var header in builder.Headers)
                    {
                        req.Headers.Add(header.Key, header.Value);
                    }

                    using (var response = req.GetResponse() as HttpWebResponse)
                    {
                        var encoding = Encoding.GetEncoding(response.CharacterSet);

                        using (var responseStream = response.GetResponseStream())
                        using (var reader = new System.IO.StreamReader(responseStream, encoding))
                            jsonText = reader.ReadToEnd();
                    }

                }
                obj = JsonBase.FromString(jsonText) as JsonBase;
            }

            string schema = builder.JsonSchema;

            // have to update schema
            if (schema != jsonSchemaString || jsonSchema == null || String.IsNullOrEmpty(jsonSchemaString))
            {
                JsonSchema schemaObj = null;
                if (String.IsNullOrEmpty(schema) || rebuildSchema)
                {
                    if (obj != null)
                    {
                        schemaObj = JsonSchema.FromJson(obj);
                        JsonObject child = new JsonObject();
                        schemaObj.Save(child);
                        jsonSchemaString = child.ToString();
                    }
                }
                else
                {
                    schemaObj = JsonSchema.Load(JsonBase.FromString(schema) as JsonObject);
                    jsonSchemaString = schema;
                }

                if (schemaObj == null)
                {
                    schemaObj = new JsonSchema();
                    schemaObj.Type = "array";
                }

                if (schemaObj.Type != "array")
                {
                    JsonSchema parentSchema = new JsonSchema();
                    parentSchema.Items = schemaObj;
                    parentSchema.Type = "array";
                    schemaObj = parentSchema;
                }

                jsonSchema = schemaObj;
            }

            if (obj is JsonArray)
            {
                jsonInternal = obj as JsonArray;
            }
            else
            {
                JsonArray result = new JsonArray();
                if (obj != null)
                    result.Add(obj);
                jsonInternal = result;
            }
        }

        #endregion Private Methods
    }
}