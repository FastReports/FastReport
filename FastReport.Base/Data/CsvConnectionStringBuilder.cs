using System.Text;
using System.Data.Common;

namespace FastReport.Data
{
    /// <summary>
    /// Represents the CsvDataConnection connection string builder.
    /// </summary>
    /// <remarks>
    /// Use this class to parse connection string returned by the <b>CsvDataConnection</b> class.
    /// </remarks>
    public class CsvConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to .csv file.
        /// </summary>
        public string CsvFile
        {
            get
            {
                object csvFile;
                if (TryGetValue("CsvFile", out csvFile))
                {
                    return (string)csvFile;
                }
                return "";
            }
            set { base["CsvFile"] = value; }
        }

        /// <summary>
        /// Gets or sets the codepage of .csv file.
        /// </summary>
        public string Codepage
        {
            get
            {
                object codepage;
                if (TryGetValue("Codepage", out codepage))
                {
                    return (string)codepage;
                }
                return Encoding.Default.CodePage.ToString();
            }
            set { base["Codepage"] = value; }
        }

        /// <summary>
        /// Gets or sets the separator.
        /// </summary>
        public string Separator
        {
            get
            {
                object separator;
                if (TryGetValue("Separator", out separator))
                {
                    return (string)separator;
                }
                return ";";
            }
            set { base["Separator"] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that field names should be loaded from the first string of the file.
        /// </summary>
        public string FieldNamesInFirstString
        {
            get
            {
                object fieldNamesInFirstString;
                if (TryGetValue("FieldNamesInFirstString", out fieldNamesInFirstString))
                {
                    return fieldNamesInFirstString.ToString().ToLower();
                }
                return "false";
            }
            set { base["FieldNamesInFirstString"] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that quotation marks should be removed.
        /// </summary>
        public string RemoveQuotationMarks
        {
            get
            {
                object removeQuotationMarks;
                if (TryGetValue("RemoveQuotationMarks", out removeQuotationMarks))
                {
                    return removeQuotationMarks.ToString().ToLower();
                }
                return "true";
            }
            set { base["RemoveQuotationMarks"] = value; }
        }

        /// <summary>
        /// Gets or sets the value indicating that field types should be converted.
        /// </summary>
        public string ConvertFieldTypes
        {
            get
            {
                object convertFieldTypes;
                if (TryGetValue("ConvertFieldTypes", out convertFieldTypes))
                {
                    return convertFieldTypes.ToString().ToLower();
                }
                return "true";
            }
            set { base["ConvertFieldTypes"] = value; }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConnectionStringBuilder"/> class.
        /// </summary>
        public CsvConnectionStringBuilder()
        {
            ConnectionString = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvConnectionStringBuilder"/> class with a specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public CsvConnectionStringBuilder(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }

        #endregion Constructors
    }
}
