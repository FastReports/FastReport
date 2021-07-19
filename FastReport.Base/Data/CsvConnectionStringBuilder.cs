using System.Text;
using System.Data.Common;
using System.Globalization;

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
        public int Codepage
        {
            get
            {
                object codepage;
                if (TryGetValue("Codepage", out codepage))
                {
                    return int.Parse((string)codepage);
                }
                return Encoding.Default.CodePage;
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

        /// <summary>
        /// Gets or sets the value indicating that quotation marks should be removed.
        /// </summary>
        public bool RemoveQuotationMarks
        {
            get
            {
                object removeQuotationMarks;
                if (TryGetValue("RemoveQuotationMarks", out removeQuotationMarks))
                {
                    return removeQuotationMarks.ToString().ToLower() == "true";
                }
                return true;
            }
            set { base["RemoveQuotationMarks"] = value.ToString().ToLower(); }
        }

        /// <summary>
        /// Gets or sets the value indicating that field types should be converted.
        /// </summary>
        public bool ConvertFieldTypes
        {
            get
            {
                object convertFieldTypes;
                if (TryGetValue("ConvertFieldTypes", out convertFieldTypes))
                {
                    return convertFieldTypes.ToString().ToLower() == "true";
                }
                return true;
            }
            set { base["ConvertFieldTypes"] = value.ToString().ToLower(); }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert numeric fields, e.g. "en-US".
        /// </summary>
        public string NumberFormat
        {
            get
            {
                object numberFormat;
                if (TryGetValue("NumberFormat", out numberFormat))
                {
                    return numberFormat.ToString();
                }
                return CultureInfo.CurrentCulture.Name;
            }
            set { base["NumberFormat"] = value; }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert currency fields, e.g. "en-US".
        /// </summary>
        public string CurrencyFormat
        {
            get
            {
                object currencyFormat;
                if (TryGetValue("CurrencyFormat", out currencyFormat))
                {
                    return currencyFormat.ToString();
                }
                return CultureInfo.CurrentCulture.Name;
            }
            set { base["CurrencyFormat"] = value; }
        }

        /// <summary>
        /// Gets or sets locale name used to auto-convert datetime fields, e.g. "en-US".
        /// </summary>
        public string DateTimeFormat
        {
            get
            {
                object dateTimeFormat;
                if (TryGetValue("DateTimeFormat", out dateTimeFormat))
                {
                    return dateTimeFormat.ToString();
                }
                return CultureInfo.CurrentCulture.Name;
            }
            set { base["DateTimeFormat"] = value; }
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
