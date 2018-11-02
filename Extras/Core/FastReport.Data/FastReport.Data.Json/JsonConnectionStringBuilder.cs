using System.Data.Common;

namespace FastReport.Data
{
    /// <summary>
    /// Represents the JsonDataConnection connection string builder.
    /// </summary>
    /// <remarks>
    /// Use this class to parse connection string returned by the <b>JsonDataConnection</b> class.
    /// </remarks>
    public class JsonConnectionStringBuilder : DbConnectionStringBuilder
    {
        /// <summary>
        /// Gets or sets the path to json. It can be a path to local file or URL.
        /// </summary>
        public string Json
        {
            get
            {
                object json;
                if (TryGetValue("Json", out json))
                    return (string)json;
                return "";
            }
            set
            {
                base["Json"] = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConnectionStringBuilder"/> class with default settings.
        /// </summary>
        public JsonConnectionStringBuilder()
        {
            ConnectionString = "";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConnectionStringBuilder"/> class with 
        /// specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public JsonConnectionStringBuilder(string connectionString)
            : base()
        {
            ConnectionString = connectionString;
        }
    }
}
