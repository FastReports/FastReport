using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data;
using Google.Apis.Sheets.v4.Data;

namespace FastReport.Data
{
	public partial class GoogleSheetsDataConnection : DataConnectionBase
	{
		#region Properties
		/// <summary>
		/// Gets or sets id Google Sheets.
		/// </summary>
		[Category("Data")]
		public string Sheets
		{
			get
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				return builder.Sheets;
			}
			set
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				builder.Sheets = value;
				ConnectionString = builder.ToString();
			}
		}

		/// <summary>
		/// Gets or sets table name.
		/// </summary>
		[Category("Data")]
		public string TableName
		{
			get
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				return builder.TableName;
			}
			set
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				builder.TableName = value;
				ConnectionString = builder.ToString();
			}
		}

		/// <summary>
		/// Gets or sets first column in the Google Sheets.
		/// </summary>
		[Category("Data")]
		public string StartColumn
		{
			get
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				return builder.StartColumn;
			}
			set
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				builder.StartColumn = value;
				ConnectionString = builder.ToString();
			}
		}

		/// <summary>
		/// Gets or sets last column in the Google Sheets.
		/// </summary>
		public string EndColumn
		{
			get
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				return builder.EndColumn;
			}
			set
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				builder.EndColumn = value;
				ConnectionString = builder.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the value indicating that field names should be loaded from the first string of the file.
		/// </summary>
		public bool FieldNamesInFirstString
		{
			get
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				return builder.FieldNamesInFirstString;
			}
			set
			{
				GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
				builder.FieldNamesInFirstString = value;
				ConnectionString = builder.ToString();
			}
		}

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleSheetsDataConnection"/> class.
		/// </summary>
		public GoogleSheetsDataConnection()
		{
			IsSqlBased = false;
		}

		#endregion Constructors

		#region Protected Methods

		/// <inheritdoc/>
		protected override DataSet CreateDataSet()
		{
			DataSet dataset = base.CreateDataSet();
			GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
			Spreadsheet spreadsheet = GoogleSheetsUtils.ReadTable(builder);

			for (int i = 0; i < spreadsheet.Sheets.Count; i++)
			{
				builder.TableName = spreadsheet.Sheets[i].Properties.Title;
                DataTable table = GoogleSheetsUtils.CreateDataTable(spreadsheet.SpreadsheetId, builder);
				dataset.Tables.Add(table);
			}

			return dataset;
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
			if (DataSet.Tables.Contains(source.TableName))
			{
				source.Table = DataSet.Tables[source.TableName];
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
