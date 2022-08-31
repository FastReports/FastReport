using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

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
                {
                    if (Regex.IsMatch((string)result, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = (Convert.FromBase64String(result.ToString()));
                        return System.Text.Encoding.UTF8.GetString(base64str);
                    }
                    return (string)result;
                }
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
                {
                    if (Regex.IsMatch((string)result, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = (Convert.FromBase64String(result.ToString()));
                        return System.Text.Encoding.UTF8.GetString(base64str);
                    }
                    return (string)result;
                }
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

        /// <summary>
        /// Set or get headers of the connection string.
        /// 
        /// </summary>
        /// <remarks>
        /// Returns copy of dictionary. If you need to update values, set the dictionary again!
        /// </remarks>
        public Dictionary<string, string> Headers
        {
            get
            {
                var numberformat = CultureInfo.InvariantCulture.NumberFormat;
                Dictionary<string, string> headers = new Dictionary<string, string>();
                object result;
                string header = string.Empty;
                string[] splittedHeader;
                int headerIteration = 0;
                while (TryGetValue("Header" + headerIteration.ToString(numberformat), out result) && result != null)
                {
                    header = (string)result;

                    if (!string.IsNullOrWhiteSpace(header))
                    {
                        splittedHeader = header.Split(':');

                        string headerKey = splittedHeader[0], headerVal = splittedHeader[1];

                        if (Regex.IsMatch(headerKey, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                        {
                            var base64str = Convert.FromBase64String(headerKey);
                            headerKey = System.Text.Encoding.UTF8.GetString(base64str);
                        }

                        if (Regex.IsMatch(headerVal, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                        {
                            var base64str = Convert.FromBase64String(headerVal);
                            headerVal = System.Text.Encoding.UTF8.GetString(base64str);
                        }

                        headers[headerKey] = headerVal;
                    }
                    headerIteration++;
                }
                return headers;
            }
            set
            {
                var numberformat = CultureInfo.InvariantCulture.NumberFormat;
                int headerIteration = 0;

                while (Remove("Header" + headerIteration.ToString(numberformat)))
                {
                    headerIteration++;
                }

                if (value != null)
                {
                    headerIteration = 0;
                    foreach (var header in value)
                    {
                        var headerKey = header.Key;
                        var headerVal = header.Value;
                        if (headerKey.Contains(":"))
                        {
                            headerKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerKey));
                        }

                        if (headerVal.Contains(":"))
                        {
                            headerVal = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerVal));
                        }

                        base["Header" + headerIteration.ToString(numberformat)] = headerKey + ":" + headerVal;
                        headerIteration++;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets simple structure value
        /// </summary>
        public bool SimpleStructure
        {
            get
            {
                object result;
                if (TryGetValue("SimpleStructure", out result) && result != null)
                    return result.ToString().ToLower() == "true";
                return false;
            }
            set
            {
                base["SimpleStructure"] = value ? "true" : "false";
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