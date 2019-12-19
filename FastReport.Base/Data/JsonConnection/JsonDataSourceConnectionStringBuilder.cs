using System.Data.Common;

namespace FastReport.Data.JsonConnection
{
    /// <summary>
    /// Represents the JsonDataConnection connection string builder.
    /// </summary>
    /// <remarks>
    /// Use this class to parse connection string returned by the <b>JsonDataConnection</b> class.
    /// </remarks>
    public class JsonDataSourceConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets json data
        /// </summary>
        public string Json
        {
            get
            {
                object result;
                if (TryGetValue("Json", out result) && result != null)
                    return (string)result;
                return "";
            }
            set
            {
                base["Json"] = value;
            }
        }

        /// <summary>
        /// Gets or sets json schema
        /// </summary>
        public string JsonSchema
        {
            get
            {
                object result;
                if (TryGetValue("JsonSchema", out result) && result != null)
                    return (string)result;
                return "";
            }
            set
            {
                base["JsonSchema"] = value;
            }
        }

        /// <summary>
        /// Gets or sets json url encoding
        /// </summary>
        public string Encoding
        {
            get
            {
                object result;
                if (TryGetValue("Encoding", out result) && result != null)
                    return (string)result;
                return "";
            }
            set
            {
                base["Encoding"] = value;
            }
        }

    

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataSourceConnectionStringBuilder"/> class with default settings.
        /// </summary>
        public JsonDataSourceConnectionStringBuilder()
        {
            ConnectionString = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataSourceConnectionStringBuilder"/> class with
        /// specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public JsonDataSourceConnectionStringBuilder(string connectionString)
            : base()
        {
            ConnectionString = connectionString;
        }

        #endregion Public Constructors
    }
}