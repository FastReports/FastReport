using System;
using System.Data.Common;

namespace FastReport.Data
{
    /// <summary>
    /// Provides a simple way to create and manage the contents of connection strings
    /// used by the Google Sheets data connection.
    /// </summary>
    public class GoogleSheetsConnectionStringBuilder : DbConnectionStringBuilder
    {
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
        /// <param name="connectionString">The connection string to parse.</param>
        public GoogleSheetsConnectionStringBuilder(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the Google Spreadsheet.
        /// </summary>
        public string SpreadsheetId
        {
            get
            {
                if (TryGetValue("SpreadsheetId", out object gSheets))
                {
                    return gSheets as string ?? "";
                }
                return "";
            }

            set { base["SpreadsheetId"] = value ?? ""; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first row of the data range should be treated as column headers.
        /// </summary>
        public bool FieldNamesInFirstRow
        {
            get
            {
                return TryGetValue("FieldNamesInFirstRow", out object value) &&
                       Convert.ToBoolean(value);
            }
            set { base["FieldNamesInFirstRow"] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating whether to include hidden sheets.
        /// </summary>
        public bool IncludeHiddenSheets
        {
            get
            {
                return TryGetValue("IncludeHiddenSheets", out object value) &&
                       Convert.ToBoolean(value);
            }
            set { base["IncludeHiddenSheets"] = value; }
        }

        #endregion
    }
}