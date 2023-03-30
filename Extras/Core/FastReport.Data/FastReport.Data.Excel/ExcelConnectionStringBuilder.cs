using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Data
{
    class ExcelConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Properties

        /// <summary>
        /// Gets or sets the path to .xslx file.
        /// </summary>
        public string ExcelFile
        {
            get
            {
                object excelFile;
                if (TryGetValue("ExcelFile", out excelFile))
                {
                    return (string)excelFile;
                }
                return "";
            }
            set { base["ExcelFile"] = value; }
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
        /// Initializes a new instance of the <see cref="ExcelConnectionStringBuilder"/> class.
        /// </summary>
        public ExcelConnectionStringBuilder()
        {
            ConnectionString = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExcelConnectionStringBuilder"/> class with a specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ExcelConnectionStringBuilder(string connectionString) : base()
        {
            ConnectionString = connectionString;
        }

        #endregion Constructors
    }
}
