namespace FastReport.Data
{
    /// <summary>
    /// Represents credentials for connecting to Google Sheets.
    /// <para/>Supported authentication methods are:
    /// <list type="bullet">
    /// <item>API Key (read-only)</item>
    /// <item>Client ID and Secret</item>
    /// <item>JSON file with credentials</item>
    /// </list>
    /// </summary>
    public class GoogleSheetsCredentials
    {
        /// <summary>
        /// Gets the Client ID for OAuth 2.0 authentication.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets the Client Secret for OAuth 2.0 authentication.
        /// </summary>
        public string ClientSecret { get; }

        /// <summary>
        /// Gets the path to the JSON file containing credentials for OAuth 2.0 authentication.
        /// </summary>
        public string PathToJson { get; }

        /// <summary>
        /// Gets the API Key for API key authentication.
        /// </summary>
        public string ApiKey { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="GoogleSheetsCredentials"/> class.
        /// </summary>
        /// <param name="clientId">The Client ID for OAuth 2.0 authentication.</param>
        /// <param name="clientSecret">The Client Secret for OAuth 2.0 authentication.</param>
        /// <param name="pathToJson">The path to the JSON credentials file for OAuth 2.0 authentication.</param>
        /// <param name="apiKey">The API Key for API key authentication.</param>
        public GoogleSheetsCredentials(string clientId, string clientSecret, string pathToJson, string apiKey)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            PathToJson = pathToJson;
            ApiKey = apiKey;
        }
    }
}