using System.Data.Common;

namespace FastReport.Data
{
	/// <summary>
	/// Represents the GoogleDataConnection connection string builder.
	/// </summary>
	/// <remarks>
	/// Use this class to parse connection string returned by the <b>GoogleDataConnection</b> class.
	/// </remarks>
	public class GoogleSheetsConnectionStringBuilder : DbConnectionStringBuilder
	{
		#region Properties

		/// <summary>
		/// Gets or sets id Google Sheets.
		/// </summary>
		public string Sheets
		{
			get
			{
				object gSheets;
				if (TryGetValue("GoogleSheets", out gSheets))
				{
					return (string)gSheets;
				}
				return "";
			}
				
			set { base["GoogleSheets"] = value; }
		}

		/// <summary>
		/// Gets or sets table name.
		/// </summary>
		public string TableName
		{
			get
			{
				object tableName;
				if (TryGetValue("TableName", out tableName))
				{
					return (string)tableName;
				}
				return "";
			}

			set { base["TableName"] = value; }
		}

		/// <summary>
		/// Gets or sets first column in the Google Sheets.
		/// </summary>
		public string StartColumn
		{
			get
			{
				object startColumn;
				if (TryGetValue("StartColumn", out startColumn))
				{
					return (string)startColumn;
				}
				return "";
			}

			set { base["StartColumn"] = value; }
		}

		/// <summary>
		/// Gets or sets last column in the Google Sheets.
		/// </summary>
		public string EndColumn
		{
			get
			{
				object endColumn;
				if (TryGetValue("EndColumn", out endColumn))
				{
					return (string)endColumn;
				}
				return "";
			}

			set { base["EndColumn"] = value; }
		}

		/// <summary>
		/// Gets or sets the value indicating that field names should be loaded from the first string of the file.
		/// </summary>
		public bool FieldNamesInFirstString
		{
			get
			{
				object fieldNamesInFirstString;
				if (TryGetValue("FieldNamesInFirstString", out fieldNamesInFirstString))
				{
					return fieldNamesInFirstString.ToString().ToLower() == "true";
				}
				return false;
			}
			set { base["FieldNamesInFirstString"] = value.ToString().ToLower(); }
		}

		#endregion Properties

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleSheetsConnectionStringBuilder"/> class.
		/// </summary>
		public GoogleSheetsConnectionStringBuilder()
		{
			ConnectionString = "";
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleSheetsConnectionStringBuilder"/> class with a specified connection string.
		/// </summary>
		/// <param name="connectionString">The connection string.</param>
		public GoogleSheetsConnectionStringBuilder(string connectionString) : base()
		{
			ConnectionString = connectionString;
		}

		#endregion Constructors
	}
}
