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

        public Dictionary<string, string> Headers { get; set; }



        #endregion Public Properties

        #region Public Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonDataSourceConnectionStringBuilder"/> class with default settings.
        /// </summary>
        public JsonDataSourceConnectionStringBuilder()
        {
            ConnectionString = "";
            Headers = new Dictionary<string, string>();
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
            Headers = new Dictionary<string, string>();
            //while (ConnectionString.Contains("Header="))
            //{
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
                        headerKey = System.Text.Encoding.UTF8.GetString(base64str);
                    }

                    if (Regex.IsMatch(headerVal, @"^([A-Za-z0-9+\/]{4})*([A-Za-z0-9+\/]{3}=|[A-Za-z0-9+\/]{2}==)?$"))
                    {
                        var base64str = Convert.FromBase64String(headerVal);
                        headerVal = System.Text.Encoding.UTF8.GetString(base64str);
                    }

                    Headers.Add(headerKey, headerVal);
                    //ConnectionString = ConnectionString.Replace(header, string.Empty);
                }

                headerIteration++;
            }
            //}
        }

        #endregion Public Constructors

        // escape / ; " :
        public override string ToString()
        {
            //TODO: do via stringbuilder
            //string connString = $"Json={Json};JsonSchema={JsonSchema};Encoding={Encoding}";
            StringBuilder builder = new StringBuilder();
            builder.Append("Json=").Append(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Json)))
                .Append(";JsonSchema=").Append(Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSchema)))
                .Append(";Encoding=").Append(Encoding);
            //string connString = "Json=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(Json))
            //    + ";JsonSchema=" + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(JsonSchema)) + ";Encoding=" + Encoding;
            int headerIteration = 0;
            foreach (var header in Headers)
            {
                var headerKey = header.Key;
                var headerVal = header.Value;
                if (headerKey.Contains(";") || headerKey.Contains(":") || headerKey.Contains("\"") || headerKey.Contains("\'"))
                {
                    headerKey = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerKey));
                }

                if (headerVal.Contains(";") || headerVal.Contains(":") || headerVal.Contains("\"") || headerVal.Contains("\'"))
                {
                    headerVal = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(headerVal));
                }

                builder.Append(";Header").Append(headerIteration.ToString(CultureInfo.InvariantCulture.NumberFormat)).Append("=").Append(headerKey).Append(":").Append(headerVal);
                //connString += ";Header" + headerIteration + "=" + headerKey + ":" + headerVal;
            }
            return builder.ToString();
        }
    }
}