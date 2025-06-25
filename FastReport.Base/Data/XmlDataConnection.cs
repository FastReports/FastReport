using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.ComponentModel;
using FastReport.Utils;
using System.Data.Common;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to xml file-based database.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report1;
    /// XmlDataConnection conn = new XmlDataConnection();
    /// conn.XmlFile = @"c:\data.xml";
    /// report1.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public partial class XmlDataConnection : DataConnectionBase
    {
        #region Properties
        /// <summary>
        /// Gets or sets the path to .xsd file.
        /// </summary>
        [Category("Data")]
        public string XsdFile
        {
            get
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                return builder.XsdFile;
            }
            set
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                builder.XsdFile = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the path to .xml file.
        /// </summary>
        [Category("Data")]
        public string XmlFile
        {
            get
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                return builder.XmlFile;
            }
            set
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                builder.XmlFile = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the codepage of the .xml file.
        /// </summary>
        [Category("Data")]
        public int Codepage
        {
            get
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                return builder.Codepage;
            }
            set
            {
                XmlConnectionStringBuilder builder = new XmlConnectionStringBuilder(ConnectionString);
                builder.Codepage = value;
                ConnectionString = builder.ToString();
            }
        }
        #endregion

        #region Protected Methods
        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            DataSet dataset = base.CreateDataSet();
            ReadXmlSchema(dataset);
            ReadXml(dataset);
            return dataset;
        }

        protected override async Task<DataSet> CreateDataSetAsync(CancellationToken cancellationToken)
        {
            var dataset = await base.CreateDataSetAsync(cancellationToken);
            await ReadXmlSchemaAsync(dataset);
            await ReadXmlAsync(dataset);
            return dataset;
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

        public override Task FillTableSchemaAsync(DataTable table, string selectCommand,
            CommandParameterCollection parameters,
            CancellationToken cancellationToken = default)
        {
            // do nothing
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // do nothing
        }

        public override Task FillTableDataAsync(DataTable table, string selectCommand, CommandParameterCollection parameters,
            CancellationToken cancellationToken = default)
        {
            // do nothing
            return Task.CompletedTask;
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

        public override Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            CreateTable(source);
            return Task.CompletedTask;
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
        #endregion

        #region private methods
        private void ReadXml(DataSet dataset)
        {
            try
            {
                // fix for datafile in current folder
                if (File.Exists(XmlFile))
                    XmlFile = Path.GetFullPath(XmlFile);

                Uri uri = new Uri(XmlFile);

                if (uri.IsFile)
                {
                    if (Config.ForbidLocalData)
                        throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));

                    if (Codepage != 1251)
                    {
                        //Adding the ability to decode a file from a computer in the selected encoding.
                        using (var reader = new StreamReader(XmlFile, Encoding.GetEncoding(Codepage))) 
                        {
                            dataset.ReadXml(reader);
                        }
                    }
                    else
                    {
                        dataset.ReadXml(XmlFile);
                    }  
                }
                else if (uri.OriginalString.StartsWith("http") || uri.OriginalString.StartsWith("ftp"))
                {
                    LoadXmlFromUrl(dataset);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private async Task ReadXmlAsync(DataSet dataset)
        {
            try
            {
                // fix for datafile in current folder
                if (File.Exists(XmlFile))
                    XmlFile = Path.GetFullPath(XmlFile);

                Uri uri = new Uri(XmlFile);

                if (uri.IsFile)
                {
                    if (Config.ForbidLocalData)
                        throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));

                    if (Codepage != 1251)
                    {
                        //Adding the ability to decode a file from a computer in the selected encoding.
                        using (var reader = new StreamReader(XmlFile, Encoding.GetEncoding(Codepage))) 
                        {
                            dataset.ReadXml(reader);
                        }
                    }
                    else
                    {
                        dataset.ReadXml(XmlFile);
                    }  
                }
                else if (uri.OriginalString.StartsWith("http") || uri.OriginalString.StartsWith("ftp"))
                {
                    await LoadXmlFromUrlAsync(dataset);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        private void LoadXmlFromUrl(DataSet dataset)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            HttpWebRequest req = WebRequest.CreateHttp(XmlFile);
            using (var response = req.GetResponse() as HttpWebResponse)
            {
                LoadXmlFromUrlShared(dataset, response);
            }
        }
        
        private async Task LoadXmlFromUrlAsync(DataSet dataset)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            HttpWebRequest req = WebRequest.CreateHttp(XmlFile);
            using (var response = await req.GetResponseAsync() as HttpWebResponse)
            {
                LoadXmlFromUrlShared(dataset, response);
            }
        }

        private void LoadXmlFromUrlShared(DataSet dataset, HttpWebResponse response)
        {
            string charset = null;
            Encoding encoding;
            if (Codepage != 1251)
            {
                encoding = Encoding.GetEncoding(Codepage);
            }
            else
            {
                // Extracting the charset from the Content-Type header, if specified.
                var contentType = response.ContentType;
                if (!string.IsNullOrEmpty(contentType))
                {
                    var match = Regex.Match(contentType, @"charset=([\w-]+)", RegexOptions.IgnoreCase);
                    if (match.Success && match.Groups.Count > 1)
                        charset = match.Groups[1].Value;
                }

                // Defining the encoding. If omitted or there is an error, we use UTF�8 by default.
                try
                {
                    encoding = string.IsNullOrWhiteSpace(charset) ? Encoding.UTF8 : Encoding.GetEncoding(charset);
                }
                catch
                {
                    encoding = Encoding.UTF8;
                }
            }

            using (var responseStream = response.GetResponseStream())
            using (var reader = new System.IO.StreamReader(responseStream, encoding))
                dataset.ReadXml(reader, XmlReadMode.Auto);
        }

        private void ReadXmlSchema(DataSet dataset)
        {
            if (String.IsNullOrEmpty(XsdFile))
                return;

            try
            {
                Uri uri = new Uri(XsdFile);

                if (uri.IsFile)
                {
                    if (Config.ForbidLocalData)
                        throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));
                    dataset.ReadXmlSchema(XsdFile);
                }
                else if (uri.OriginalString.StartsWith("http") || uri.OriginalString.StartsWith("ftp"))
                {
                    LoadXmlSchemaFromUrl(dataset);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        
        private async Task ReadXmlSchemaAsync(DataSet dataset)
        {
            if (String.IsNullOrEmpty(XsdFile))
                return;

            try
            {
                Uri uri = new Uri(XsdFile);

                if (uri.IsFile)
                {
                    if (Config.ForbidLocalData)
                        throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));
                    dataset.ReadXmlSchema(XsdFile);
                }
                else if (uri.OriginalString.StartsWith("http") || uri.OriginalString.StartsWith("ftp"))
                {
                    await LoadXmlSchemaFromUrlAsync(dataset);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void LoadXmlSchemaFromUrl(DataSet dataset)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(XsdFile);
            using (var response = req.GetResponse() as HttpWebResponse)
            {
                var encoding = response.CharacterSet.Equals(String.Empty) ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                    dataset.ReadXmlSchema(reader);
            }
        }
        
        private async Task LoadXmlSchemaFromUrlAsync(DataSet dataset)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)(0xc0 | 0x300 | 0xc00);
            HttpWebRequest req = WebRequest.CreateHttp(XsdFile);
            using (var response = await req.GetResponseAsync() as HttpWebResponse)
            {
                var encoding = response.CharacterSet.Equals(String.Empty) ? Encoding.UTF8 : Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new System.IO.StreamReader(responseStream, encoding))
                    dataset.ReadXmlSchema(reader);
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataConnection"/> class with default settings.
        /// </summary>
        public XmlDataConnection()
        {
            IsSqlBased = false;
        }
    }
}
