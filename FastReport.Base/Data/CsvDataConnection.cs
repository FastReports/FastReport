using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net;
using FastReport.Utils;
using System.Globalization;
using System.Collections;
using System.Threading.Tasks;
using System.Threading;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a connection to csv file-based database.
    /// </summary>
    /// <example>This example shows how to add a new connection to the report.
    /// <code>
    /// Report report1;
    /// CsvDataConnection conn = new CsvDataConnection();
    /// conn.CsvFile = @"c:\data.csv";
    /// report1.Dictionary.Connections.Add(conn);
    /// conn.CreateAllTables();
    /// </code>
    /// </example>
    public partial class CsvDataConnection : DataConnectionBase
    {
        #region Fields
        #endregion Fields

        #region Properties
        /// <summary>
        /// Gets or sets the path to .csv file.
        /// </summary>
        [Category("Data")]
        public string CsvFile
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.CsvFile;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.CsvFile = value;
                ConnectionString = CheckForChangeConnection(builder);
            }
        }

        /// <summary>
        /// Gets or sets the codepage of the .csv file.
        /// </summary>
        [Category("Data")]
        public int Codepage
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.Codepage;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.Codepage = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the separator of the .csv file.
        /// </summary>
        [Category("Data")]
        public string Separator
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.Separator;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.Separator = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that field names should be loaded from the first string of the file.
        /// </summary>
        [Category("Data")]
        public bool FieldNamesInFirstString
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.FieldNamesInFirstString;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.FieldNamesInFirstString = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that quotation marks should be removed.
        /// </summary>
        [Category("Data")]
        public bool RemoveQuotationMarks
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.RemoveQuotationMarks;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.RemoveQuotationMarks = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets the value indicating that field types fhould be converted.
        /// </summary>
        [Category("Data")]
        public bool ConvertFieldTypes
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.ConvertFieldTypes;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.ConvertFieldTypes = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert numeric fields, e.g. "en-US".
        /// </summary>
        [Category("Data")]
        public string NumberFormat
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.NumberFormat;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.NumberFormat = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert currency fields, e.g. "en-US".
        /// </summary>
        [Category("Data")]
        public string CurrencyFormat
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.CurrencyFormat;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.CurrencyFormat = value;
                ConnectionString = builder.ToString();
            }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert datetime fields, e.g. "en-US".
        /// </summary>
        [Category("Data")]
        public string DateTimeFormat
        {
            get
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                return builder.DateTimeFormat;
            }
            set
            {
                CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
                builder.DateTimeFormat = value;
                ConnectionString = builder.ToString();
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataConnection"/> class.
        /// </summary>
        public CsvDataConnection()
        {
            IsSqlBased = false;
        }

        #endregion Constructors

        #region Protected Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            DataSet dataset = base.CreateDataSet();
            return CreateDataSetShared(dataset);
        }

        protected override async Task<DataSet> CreateDataSetAsync(CancellationToken cancellationToken)
        {
            DataSet dataset = await base.CreateDataSetAsync(cancellationToken);
            return CreateDataSetShared(dataset);
        }

        private DataSet CreateDataSetShared(DataSet dataset)
        {
            CsvConnectionStringBuilder builder = new CsvConnectionStringBuilder(ConnectionString);
            RelatedPathCheck(builder);
            List<string> rawLines = CsvUtils.ReadLines(builder);
            DataTable table = CsvUtils.CreateDataTable(builder, rawLines);
            if (table != null)
                dataset.Tables.Add(table);
            return dataset;
        }

        /// <summary>
        /// Checking a relative path relative to a file
        /// </summary>
        protected void RelatedPathCheck(CsvConnectionStringBuilder builder)
        {
            // checking for an empty file path string
            if (string.IsNullOrEmpty(builder.CsvFile))
                throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));

            if (Report == null || string.IsNullOrEmpty(Report.FileName))
                return;

            // combining a path will only work if the path is relative
            if (!File.Exists(builder.CsvFile))
            {
                Uri.TryCreate(builder.CsvFile, UriKind.RelativeOrAbsolute, out Uri uri);
                if (!uri.IsAbsoluteUri)
                    if (!Path.GetDirectoryName(Report.FileName).Equals(Path.GetDirectoryName(builder.CsvFile)))
                        builder.CsvFile = Path.Combine(Path.GetDirectoryName(Report.FileName), builder.CsvFile);
            }
        }

        /// <inheritdoc/>
        protected override void SetConnectionString(string value)
        {
            DisposeDataSet();
            base.SetConnectionString(value);
        }

        #endregion Protected Methods

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
            if (DataSet.Tables.Count == 1)
            {
                source.Table = DataSet.Tables[0];
                base.CreateTable(source);
            }
            else
            {
                source.Table = null;
            }
        }

        public override async Task CreateTableAsync(TableDataSource source, CancellationToken cancellationToken = default)
        {
            if (DataSet.Tables.Count == 1)
            {
                source.Table = DataSet.Tables[0];
                await base.CreateTableAsync(source, cancellationToken);
            }
            else
            {
                source.Table = null;
            }
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
        #endregion Public Methods
    }
}
