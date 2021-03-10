using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client
{
    internal class ClickHouseUriBuilder
    {
        private readonly IDictionary<string, string> queryParameters = new Dictionary<string, string>();

        public ClickHouseUriBuilder(Uri baseUri) => BaseUri = baseUri;

        public Uri BaseUri { get; }

        public string Sql { get; set; }

        public bool UseCompression { get; set; }

        public string Database { get; set; }

        public string SessionId { get; set; }

        public string DefaultFormat => "RowBinaryWithNamesAndTypes";

        public IReadOnlyDictionary<string, object> CustomParameters { get; set; }

        public bool AddQueryParameter(string name, string value) => DictionaryExtensions.TryAdd(queryParameters, name, value);

        public override string ToString()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty); // NameValueCollection but a special one
            parameters.Set("enable_http_compression", UseCompression.ToString(CultureInfo.InvariantCulture).ToLowerInvariant());
            parameters.Set("default_format", DefaultFormat);
            parameters.SetOrRemove("database", Database);
            parameters.SetOrRemove("session_id", SessionId);
            parameters.SetOrRemove("query", Sql);

            foreach (var parameter in queryParameters)
                parameters.Set("param_" + parameter.Key, parameter.Value);

            if (CustomParameters != null)
            {
                foreach (var parameter in CustomParameters)
                    parameters.Set(parameter.Key, Convert.ToString(parameter.Value, CultureInfo.InvariantCulture));
            }
            var uriBuilder = new UriBuilder(BaseUri) { Query = parameters.ToString() };
            return uriBuilder.ToString();
        }
    }
}
