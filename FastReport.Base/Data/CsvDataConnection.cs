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
                ConnectionString = builder.ToString();
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
			if (Report != null)
				if (!String.IsNullOrEmpty(Report.FileName))
					if (!File.Exists(builder.CsvFile) && !Path.GetDirectoryName(Report.FileName).Equals(Path.GetDirectoryName(builder.CsvFile)))
						builder.CsvFile = Path.Combine(Path.GetDirectoryName(Report.FileName), builder.CsvFile);
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

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            // do nothing
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
        #endregion Public Methods
    }
}
