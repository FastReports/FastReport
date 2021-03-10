using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.Formats;
using ClickHouse.Client.Utility;

namespace ClickHouse.Client.ADO
{
    public class ClickHouseConnection : DbConnection, IClickHouseConnection, ICloneable
    {
        private const string CustomSettingPrefix = "set_";

        private readonly HttpClient httpClient;
        private readonly ConcurrentDictionary<string, object> customSettings = new ConcurrentDictionary<string, object>();
        private ConnectionState state = ConnectionState.Closed; // Not an autoproperty because of interface implementation
        private Version serverVersion;
        private string database = "default";
        private string username;
        private string password;
        private bool useCompression;
        private string session;
        private TimeSpan timeout;
        private Uri serverUri;

        public ClickHouseConnection()
            : this(string.Empty)
        {
        }

        public ClickHouseConnection(string connectionString)
        {
            ConnectionString = connectionString;
            var httpClientHandler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate };
            httpClient = new HttpClient(httpClientHandler, true)
            {
                Timeout = timeout,
            };
            // Connection string has to be initialized after HttpClient, as it may set HttpClient.Timeout
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClickHouseConnection"/> class using provided HttpClient.
        /// Note that HttpClient must have AutomaticDecompression enabled if compression is not disabled in connection string
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="httpClient">instance of HttpClient</param>
        public ClickHouseConnection(string connectionString, HttpClient httpClient)
        {
            ConnectionString = connectionString;
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Gets or sets string defining connection settings for ClickHouse server
        /// Example: Host=localhost;Port=8123;Username=default;Password=123;Compression=true
        /// </summary>
        public sealed override string ConnectionString
        {
            get
            {
                var builder = new ClickHouseConnectionStringBuilder
                {
                    Database = database,
                    Username = username,
                    Password = password,
                    Host = serverUri?.Host,
                    Port = (ushort)serverUri?.Port,
                    Compression = useCompression,
                    UseSession = session != null,
                    Timeout = timeout,
                };

                foreach (var kvp in CustomSettings)
                    builder[CustomSettingPrefix + kvp.Key] = kvp.Value;

                return builder.ToString();
            }

            set
            {
                var builder = new ClickHouseConnectionStringBuilder() { ConnectionString = value };
                database = builder.Database;
                username = builder.Username;
                password = builder.Password;
                serverUri = new UriBuilder(builder.Protocol, builder.Host, builder.Port).Uri;
                useCompression = builder.Compression;
                session = builder.UseSession ? builder.SessionId ?? Guid.NewGuid().ToString() : null;
                timeout = builder.Timeout;

                foreach (var key in builder.Keys.Cast<string>().Where(k => k.StartsWith(CustomSettingPrefix)))
                {
                    CustomSettings.Set(key.Replace(CustomSettingPrefix, string.Empty), builder[key]);
                }
            }
        }

        public IDictionary<string, object> CustomSettings => customSettings;

        public override ConnectionState State => state;

        public override string Database => database;

        public override string DataSource { get; }

        public override string ServerVersion => serverVersion?.ToString();

        public override DataTable GetSchema() => GetSchema(null, null);

        public override DataTable GetSchema(string type) => GetSchema(type, null);

        public override DataTable GetSchema(string type, string[] restrictions) => SchemaDescriber.DescribeSchema(this, type, restrictions);

        internal async Task<HttpResponseMessage> PostSqlQueryAsync(string sqlQuery, CancellationToken token, ClickHouseParameterCollection parameters = null)
        {
            var uriBuilder = CreateUriBuilder();
            if (parameters != null)
            {
                var httpParametersSupported = await SupportsHttpParameters();

                if (httpParametersSupported)
                {
                    foreach (ClickHouseDbParameter parameter in parameters)
                        uriBuilder.AddQueryParameter(parameter.ParameterName, HttpParameterFormatter.Format(parameter));
                }
                else
                {
                    var formattedParameters = new Dictionary<string, string>(parameters.Count);
                    foreach (ClickHouseDbParameter parameter in parameters)
                        formattedParameters.TryAdd(parameter.ParameterName, InlineParameterFormatter.Format(parameter));
                    sqlQuery = SubstituteParameters(sqlQuery, formattedParameters);
                }
            }
            string uri = uriBuilder.ToString();

            using var postMessage = new HttpRequestMessage(HttpMethod.Post, uri);

            AddDefaultHttpHeaders(postMessage.Headers);
            HttpContent content = new StringContent(sqlQuery);
            content.Headers.ContentType = new MediaTypeHeaderValue("text/sql");
            if (useCompression)
            {
                content = new CompressedContent(content, DecompressionMethods.GZip);
            }

            postMessage.Content = content;

            var response = await httpClient.SendAsync(postMessage, HttpCompletionOption.ResponseHeadersRead, token).ConfigureAwait(false);
            return await HandleError(response, sqlQuery).ConfigureAwait(false);
        }

        internal async Task PostStreamAsync(string sql, Stream data, bool isCompressed, CancellationToken token)
        {
            var builder = CreateUriBuilder(sql);
            using var postMessage = new HttpRequestMessage(HttpMethod.Post, builder.ToString());
            AddDefaultHttpHeaders(postMessage.Headers);

            postMessage.Content = new StreamContent(data);
            postMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            if (isCompressed)
            {
                postMessage.Content.Headers.Add("Content-Encoding", "gzip");
            }

            using var response = await httpClient.SendAsync(postMessage, HttpCompletionOption.ResponseContentRead, token).ConfigureAwait(false);
            await HandleError(response, sql).ConfigureAwait(false);
        }

        private static async Task<HttpResponseMessage> HandleError(HttpResponseMessage response, string query)
        {
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw ClickHouseServerException.FromServerResponse(error, query);
            }
            return response;
        }

        private static string SubstituteParameters(string query, IDictionary<string, string> parameters)
        {
            var builder = new StringBuilder(query.Length);

            var paramStartPos = query.IndexOf('{');
            var paramEndPos = -1;

            while (paramStartPos != -1)
            {
                builder.Append(query.Substring(paramEndPos + 1, paramStartPos - paramEndPos - 1));

                paramStartPos += 1;
                paramEndPos = query.IndexOf('}', paramStartPos);
                var param = query.Substring(paramStartPos, paramEndPos - paramStartPos);
                var delimiterPos = param.LastIndexOf(':');
                if (delimiterPos == -1)
                    throw new NotSupportedException($"param {param} doesn`t have data type");
                var name = param.Substring(0, delimiterPos);

                if (!parameters.TryGetValue(name, out var value))
                    throw new ArgumentOutOfRangeException($"Parameter {name} not found in parameters list");

                builder.Append(value);

                paramStartPos = query.IndexOf('{', paramEndPos);
            }

            builder.Append(query.Substring(paramEndPos + 1, query.Length - paramEndPos - 1));

            return builder.ToString();
        }

        public override void ChangeDatabase(string databaseName) => database = databaseName;

        public object Clone() => new ClickHouseConnection(ConnectionString);

        public override void Close() => state = ConnectionState.Closed;

        public override void Open() => OpenAsync().ConfigureAwait(false).GetAwaiter().GetResult();

        public override async Task OpenAsync(CancellationToken token)
        {
            if (State == ConnectionState.Open)
                return;
            const string versionQuery = "SELECT version() FORMAT TSV";
            try
            {
                var response = await PostSqlQueryAsync(versionQuery, token).ConfigureAwait(false);
                response = await HandleError(response, versionQuery);
                var data = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);

                if (data.Length > 2 && data[0] == 0x1F && data[1] == 0x8B) // Check if response starts with GZip marker
                    throw new InvalidOperationException("ClickHouse server returned compressed result but HttpClient did not decompress it. Check HttpClient settings");

                if (data.Length == 0)
                    throw new InvalidOperationException("ClickHouse server did not return version, check if the server is functional");

                serverVersion = Version.Parse(Encoding.UTF8.GetString(data).Trim());
                state = ConnectionState.Open;
            }
            catch
            {
                state = ConnectionState.Broken;
                throw;
            }
        }

        public new ClickHouseCommand CreateCommand() => new ClickHouseCommand(this);

        /// <summary>
        /// Detects whether server supports parameters through URI
        ///   ClickHouse Release 19.11.3.11, 2019-07-18: New Feature: Added support for prepared statements. #5331 (Alexander) #5630 (alexey-milovidov)
        /// </summary>
        /// <returns>whether parameters are supported</returns>
        internal virtual async Task<bool> SupportsHttpParameters()
        {
            if (State != ConnectionState.Open)
                await OpenAsync();
            if (serverVersion == null)
                throw new InvalidOperationException("Connection does not define server version");
            return serverVersion >= new Version(19, 11, 3, 11);
        }

        /// <summary>
        /// Detects whether server supports putting query into POST body along with binary data
        /// Added somewhere in ClickHouse 20.5
        /// </summary>
        /// <returns>whether parameters are supported</returns>
        internal virtual async Task<bool> SupportsInlineQuery()
        {
            if (State != ConnectionState.Open)
                await OpenAsync();
            if (serverVersion == null)
                throw new InvalidOperationException("Connection does not define server version");
            return serverVersion >= new Version(20, 5);
        }

        /// <summary>
        ///  20.1.2.4 Add DateTime64 datatype with configurable sub-second precision. #7170 (Vasily Nemkov)
        /// </summary>
        /// <returns>whether DateTime64 is supported</returns>
        internal virtual async Task<bool> SupportsDateTime64()
        {
            if (State != ConnectionState.Open)
                await OpenAsync();
            if (serverVersion == null)
                throw new InvalidOperationException("Connection does not define server version");
            return serverVersion >= new Version(20, 1, 2, 4);
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();

        protected override DbCommand CreateDbCommand() => CreateCommand();

        private void AddDefaultHttpHeaders(HttpRequestHeaders headers)
        {
            headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}")));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/csv"));
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/octet-stream"));
            if (useCompression)
            {
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            }
        }

        private ClickHouseUriBuilder CreateUriBuilder(string sql = null) => new ClickHouseUriBuilder(serverUri)
        {
            Database = database,
            SessionId = session,
            UseCompression = useCompression,
            CustomParameters = customSettings,
            Sql = sql,
        };
    }
}
