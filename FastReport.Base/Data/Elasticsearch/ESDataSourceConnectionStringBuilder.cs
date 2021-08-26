using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FastReport.Data.ElasticSearch
{
    public class ESDataSourceConnectionStringBuilder : DbConnectionStringBuilder
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets EndPoint ElasticSearch DB
        /// </summary>
        public string EndPoint
        {
            get
            {
                object result;
                if (TryGetValue("EndPoint", out result) && result != null)
                {
                    if (Regex.IsMatch((string)result, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = (Convert.FromBase64String(result.ToString()));
                        return Encoding.UTF8.GetString(base64str);
                    }
                    string resultStr = result.ToString();
                    if (resultStr[resultStr.Length - 1] == '/')
                        return resultStr.Remove(resultStr.Length - 1);
                    return (string)result;
                }
                return "";
            }
            set
            {
                base["EndPoint"] = value;
            }
        }
        public Dictionary<string, string> Headers { get; set; }

        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ESDataSourceConnectionStringBuilder"/> class with default settings.
        /// </summary>
        public ESDataSourceConnectionStringBuilder()
        {
            ConnectionString = "";
            Headers = new Dictionary<string, string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ESDataSourceConnectionStringBuilder"/> class with
        /// specified connection string.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public ESDataSourceConnectionStringBuilder(string connectionString)
            : base()
        {
            ConnectionString = connectionString;
            Headers = new Dictionary<string, string>();

            object result;
            string header = string.Empty;
            string[] splittedHeader;
            int headerIteration = 0;
            while (TryGetValue("Header" + headerIteration.ToString(CultureInfo.InvariantCulture.NumberFormat), out result) && result != null)
            {
                header = (string)result;

                if (!string.IsNullOrWhiteSpace(header))
                {
                    splittedHeader = header.Split(':');

                    string headerKey = splittedHeader[0], headerVal = splittedHeader[1];

                    if (Regex.IsMatch(headerKey, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = Convert.FromBase64String(headerKey);
                        headerKey = Encoding.UTF8.GetString(base64str);
                    }

                    if (Regex.IsMatch(headerVal, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = Convert.FromBase64String(headerVal);
                        headerVal = Encoding.UTF8.GetString(base64str);
                    }

                    Headers.Add(headerKey, headerVal);
                }

                headerIteration++;
            }
        }

        #endregion Public Constructors

        #region Public Methods
        // escape / ; " :
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("EndPoint=").Append(Convert.ToBase64String(Encoding.UTF8.GetBytes(EndPoint)));

            int headerIteration = 0;
            foreach (var header in Headers)
            {
                var headerKey = header.Key;
                var headerVal = header.Value;
                if (headerKey.Contains(";") || headerKey.Contains(":") || headerKey.Contains("\"") || headerKey.Contains("\'"))
                {
                    headerKey = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerKey));
                }

                if (headerVal.Contains(";") || headerVal.Contains(":") || headerVal.Contains("\"") || headerVal.Contains("\'"))
                {
                    headerVal = Convert.ToBase64String(Encoding.UTF8.GetBytes(headerVal));
                }

                builder.Append(";Header").Append(headerIteration.ToString(CultureInfo.InvariantCulture.NumberFormat)).Append("=").Append(headerKey).Append(":").Append(headerVal);
            }
            return builder.ToString();
        }
        #endregion
    }
}
