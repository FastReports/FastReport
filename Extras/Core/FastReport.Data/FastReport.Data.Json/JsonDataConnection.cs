using System;
using System.Data;
using System.ComponentModel;
using System.Data.Common;
using System.Net;
using FastReport.Json;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using FastReport.JsonClassGenerator;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to json database.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report = new Report();
    /// JsonDataConnection conn = new JsonDataConnection();
    /// conn.Json = @"c:\data.txt";
    /// report.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public partial class JsonDataConnection : DataConnectionBase
    {
        private Encoding dataEncoding;
        private string jsonData;

        #region Properties

        public string JsonData
        {
            get
            {
                if (string.IsNullOrEmpty(jsonData))
                {
                    System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                    using (WebClient webClient = new WebClient())
                    {
                        webClient.Encoding = dataEncoding;
                        jsonData = webClient.DownloadString(Json);
                    }
                }

                return jsonData;
            }
            set { jsonData = value; }
        }

        public async Task<string> GetJsonData(CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(jsonData))
            {
                System.Net.ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
                using (WebClient webClient = new WebClient())
                {
                    webClient.Encoding = dataEncoding;
                    jsonData = await webClient.DownloadStringTaskAsync(Json);
                }
            }
            return jsonData;
        }

        /// <summary>
        /// Gets or sets the path to json. It can be a path to local file or URL.
        /// </summary>
        [Category("Data")]
        public string Json
        {
            get
            {
                JsonConnectionStringBuilder builder = new JsonConnectionStringBuilder(ConnectionString);
                return builder.Json;
            }
            set
            {
                JsonConnectionStringBuilder builder = new JsonConnectionStringBuilder(ConnectionString);
                builder.Json = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the data encoding value.
        /// </summary>
        [Browsable(false)]
        public Encoding DataEncoding
        {
            get { return dataEncoding; }
            set { dataEncoding = value; }
        }

        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            DataSet dataset = base.CreateDataSet();

            CreateDataSetShared(dataset);

            return dataset;
        }

        protected override async Task<DataSet> CreateDataSetAsync(CancellationToken cancellationToken)
        {
            DataSet dataset = await base.CreateDataSetAsync(cancellationToken);

            await GetJsonData(cancellationToken);
            CreateDataSetShared(dataset);

            return dataset;
        }

        private void CreateDataSetShared(DataSet dataset)
        {
            if (string.IsNullOrWhiteSpace(jsonData))
                throw new Exception("Data is empty.");

            if (jsonData.Trim().StartsWith("["))
                jsonData = "{\"Data\":" + jsonData + "}";


            var type = JsonCompiler.Compile(jsonData);
            var properties = type.GetProperties();
            var obj = JsonConvert.DeserializeObject(jsonData, type);

            foreach (var prop in properties)
            {
                IList list = prop.GetValue(obj, null) as IList;
                if (list != null)
                {
                    DataTable dataTable = ToDataTable(list);
                    dataTable.TableName = prop.Name;
                    dataset.Tables.Add(dataTable);
                }
            }
        }

        /// <inheritdoc/>
        protected override void SetConnectionString(string value)
        {
            DisposeDataSet();
            base.SetConnectionString(value);
        }
        #endregion

        #region Public Methods
        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // do nothing
        }

        public override Task FillTableSchemaAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // do nothing
        }

        public override Task FillTableDataAsync(DataTable table, string selectCommand, CommandParameterCollection parameters, CancellationToken cancellationToken = default)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (DataSet.Tables.Contains(source.TableName))
            {
                source.Table = DataSet.Tables[source.TableName];
                base.CreateTable(source);
            }
            else
                source.Table = null;
        }

        public override async Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            if (DataSet.Tables.Contains(source.TableName))
            {
                source.Table = DataSet.Tables[source.TableName];
                await base.CreateTableAsync(source, cancellationToken);
            }
            else
                source.Table = null;
        }

        /// <inheritdoc/>
        public override void DeleteTable(TableDataSource source)
        {
            // do nothing
        }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            string[] result = new string[DataSet.Tables.Count];
            for (int i = 0; i < DataSet.Tables.Count; i++)
            {
                result[i] = DataSet.Tables[i].TableName;
            }
            return result;
        }

        public override Task<string[]> GetTableNamesAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetTableNames());
        }

        /// <inheritdoc/>
        public override Type GetConnectionType()
        {
            return typeof(JsonConnectionType);
        }

        /// <inheritdoc/>
        public override Type GetParameterType()
        {
            return typeof(JsonTypeEnum);
        }

        #endregion

        DataTable ToDataTable(IList data)
        {
            DataTable table = new DataTable();

            if (data == null || data.Count == 0)
                return table;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(data[0]);

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }

            object[] values = new object[props.Count];

            foreach (object item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }

            return table;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataConnection"/> class with default settings.
        /// </summary>
        public JsonDataConnection()
        {
            IsSqlBased = false;
            dataEncoding = Encoding.UTF8;
        }
    }
}
