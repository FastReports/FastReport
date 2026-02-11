using FastReport.Utils;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Represents a data connection to Google Sheets.
    /// Handles authentication (API Key, OAuth 2.0 with Client ID/Secret or JSON file) and data retrieval.
    /// </summary>
    public partial class GoogleSheetsDataConnection : DataConnectionBase
    {
        private const string TokenFileNamePattern = "gsheet_token_{0}.json";
        private const string AppDataFolderName = "FastReport"; // name of the main folder in AppData
        private const string SubFolderName = "GoogleSheetsConnector"; // name of the subfolder

        private readonly IGoogleSheetsDataProvider _dataProvider;
        // cached OAuth token for the current session
        private OAuthToken _token;
        // hash of the credentials used to obtain the current token, for cache invalidation
        private string _tokenCredentialHash;

#if !FRCORE
        private readonly IGoogleSheetsConfigLoader _configLoader;
        // cached credentials loaded from configuration
        private GoogleSheetsCredentials _credentials;
#endif

        /// <summary>
        /// Initializes a new instance of the GoogleSheetsDataConnection.
        /// </summary>
        public GoogleSheetsDataConnection()
        {
            IsSqlBased = false;
            _dataProvider = new GoogleSheetsDataProvider(new GoogleSheetsClient());
#if !FRCORE
            _configLoader = new GoogleSheetsConfigLoader();
#endif
        }

        #region Properties

        /// <summary>
        /// Gets or sets the credentials required for connecting to Google Sheets.
        /// This property is mandatory when running in Core mode where UI-based credential prompts are not available.
        /// </summary>
        public GoogleSheetsCredentials Credentials { get; set; }

        /// <summary>
        /// Gets the credentials to use for the connection.
        /// In Framework mode, loads them from config if not already cached.
        /// In Core mode, uses the value from the Credentials property.
        /// </summary>
        private GoogleSheetsCredentials CachedCredentials
        {
            get
            {
#if !FRCORE
                if (_credentials != null)
                {
                    return _credentials;
                }
                _credentials = _configLoader.Load();
                return _credentials;
#else
                if (Credentials != null)
                    return Credentials;
                throw new InvalidOperationException(Res.Get("ConnectionEditors,GoogleSheets,Errors,CredentialsNotProvidedError"));
#endif
            }
        }

        /// <summary>
        /// Gets or sets the unique identifier of the Google Spreadsheet.
        /// </summary>
        [Category("Data")]
        public string SpreadsheetId
        {
            get
            {
                GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
                return builder.SpreadsheetId;
            }
            set
            {
                GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
                builder.SpreadsheetId = value;
                ConnectionString = builder.ToString();
            }
        }

        #endregion

        #region Auth Helpers

        /// <summary>
        /// Reads Client ID and Client Secret from a Google OAuth JSON credentials file.
        /// </summary>
        /// <param name="jsonFilePath">Path to the JSON file.</param>
        /// <returns>A tuple containing ClientId and ClientSecret.</returns>
        private static (string ClientId, string ClientSecret) ReadOAuthCredentialsFromJson(string jsonFilePath)
        {
            if (!File.Exists(jsonFilePath))
                throw new FileNotFoundException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,OAuthJsonFileNotFound"), jsonFilePath), jsonFilePath);

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                using (var doc = JsonDocument.Parse(json))
                {
                    var installed = doc.RootElement.GetProperty("installed");
                    string clientId = installed.GetProperty("client_id").GetString();
                    string clientSecret = installed.GetProperty("client_secret").GetString();

                    if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                        throw new InvalidOperationException(Res.Get("ConnectionEditors,GoogleSheets,Errors,OAuthJsonMissingKeys"));

                    return (clientId, clientSecret);
                }
            }
            catch (Exception ex) when (ex is JsonException || ex is KeyNotFoundException)
            {
                throw new InvalidOperationException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,OAuthJsonInvalidFormat"), jsonFilePath), ex);
            }
        }

        /// <summary>
        /// Gets an authenticated Google Sheets Service instance.
        /// Handles API Key and OAuth 2.0 authentication flows, including token caching and refresh.
        /// </summary>
        /// <param name="token">A cancellation token.</param>
        /// <param name="creds">The credentials to use.</param>
        /// <returns>An authenticated SheetsService instance.</returns>
        private async Task<SheetsService> GetSheetsServiceAsync(CancellationToken token, GoogleSheetsCredentials creds)
        {
            token.ThrowIfCancellationRequested();
            // API Key authentication
            if (!string.IsNullOrEmpty(creds.ApiKey))
            {
                // creates service using API key
                return new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
                {
                    ApiKey = creds.ApiKey,
                    ApplicationName = GoogleSheetsClient.ApplicationName
                });
            }

            // handle JSON file path for OAuth credentials
            string effectiveClientId = creds.ClientId;
            string effectiveClientSecret = creds.ClientSecret;

            if (string.IsNullOrEmpty(creds.ClientId) &&
                string.IsNullOrEmpty(creds.ClientSecret) &&
                !string.IsNullOrEmpty(creds.PathToJson))
            {
                (effectiveClientId, effectiveClientSecret) = ReadOAuthCredentialsFromJson(creds.PathToJson);
            }

            // calculate hash of current credentials for token cache invalidation
            string currentCredHash = GetTokenKey(creds);

            // check if credentials have changed since the token was loaded
            if (_token != null && _tokenCredentialHash != currentCredHash)
            {
                // credentials changed - invalidate the old token
                _token = null;
                _tokenCredentialHash = null;
            }

            // OAuth 2.0 authentication
            if (_token == null)
            {
                _token = LoadTokenFromFile();
                if (_token != null)
                {
                    // after loading token from file, remember which credentials it was for
                    _tokenCredentialHash = currentCredHash;
                }
            }

            // if token exists but is expired, try to refresh it
            if (_token != null && _token.IsNeedRefresh())
            {
                try
                {
                    await _token.Refresh(new HttpClient(), effectiveClientId, effectiveClientSecret, "https://oauth2.googleapis.com/token", token).ConfigureAwait(false);
                    SaveTokenToFile(_token);
                }
                catch
                {
                    // if refresh fails, reset the token for a full re-authentication
                    _token = null;
                    _tokenCredentialHash = null;
                }
            }

            // if no valid token exists after all attempts, initiate full authorization
            if (_token == null)
            {
                // checks if interactive login is required but not allowed
                if (string.IsNullOrEmpty(effectiveClientId) && !this.LoginPrompt)
                {
                    throw new InvalidOperationException(Res.Get("ConnectionEditors,GoogleSheets,Errors,OAuthNotConfigured"));
                }

                try
                {
                    _token = await OAuth.DoOAuthAsync(new HttpClient(), effectiveClientId, effectiveClientSecret,
                        SheetsService.Scope.SpreadsheetsReadonly,
                        "https://accounts.google.com/o/oauth2/v2/auth",
                        "https://oauth2.googleapis.com/token", 0, token).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    if (ex is OperationCanceledException || ex is ObjectDisposedException)
                    {
                        throw;
                    }
                    new GoogleSheetsConfigLoader().Clear();
                    throw new InvalidOperationException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,OAuthFlowError"), ex.Message), ex);
                }

                if (_token == null)
                {
                    throw new OperationCanceledException(Res.Get("ConnectionEditors,GoogleSheets,Errors,AuthFailedOrCancelled"));
                }

                SaveTokenToFile(_token);
                _tokenCredentialHash = currentCredHash;
            }

            // creates service using the authenticated token
            return new SheetsService(new Google.Apis.Services.BaseClientService.Initializer()
            {
                HttpClientInitializer = _token,
                ApplicationName = GoogleSheetsClient.ApplicationName
            });
        }

        /// <summary>
        /// Determines if an interactive login is needed based on current credentials and cached token state.
        /// </summary>
        /// <param name="creds">The credentials to check.</param>
        /// <returns><c>true</c> if interactive login is needed, <c>false</c> otherwise.</returns>
        private bool IsInteractiveLoginNeeded(GoogleSheetsCredentials creds)
        {
            // API key does not need interactive login
            if (!string.IsNullOrEmpty(creds.ApiKey))
                return false;

            // if no OAuth creds are configured, need to show the config dialog first
            if (string.IsNullOrEmpty(creds.ClientId) && string.IsNullOrEmpty(creds.PathToJson))
                return true;

            if (_token == null)
            {
                _token = LoadTokenFromFile();
            }

            // if token exists and is fresh, no interactive login needed
            if (_token != null && !_token.IsNeedRefresh())
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Attempts to authenticate using the API key.
        /// </summary>
        /// <returns>A <see cref="DataSet"/> if successful, or null if API key is not available.</returns> 
        private DataSet TryHandleWithApiKey(GoogleSheetsCredentials credentials)
        {
            if (string.IsNullOrEmpty(credentials.ApiKey))
                return null;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
            {
                try
                {
                    return Task.Run(() => RunAuthenticatedTaskAsync(cts.Token, credentials)).GetAwaiter().GetResult();

                }
                catch (Exception)
                {
                    if (LoginPrompt)
                        return new DataSet();
                    else
                        throw;
                }
            }
        }

        /// <summary>
        /// Attempts to execute a "silent" token refresh.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>A <see cref="DataSet"/> if successful, or null if silent refresh failed.</returns>
        private DataSet TryHandleWithSilentOAuth(GoogleSheetsCredentials credentials)
        {
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15)))
                {
                    return Task.Run(() => RunAuthenticatedTaskAsync(cts.Token, credentials)).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                // if silent refresh fails for any reason (e.g., token was revoked, network error),
                // switching to a full interactive login
                return null;
            }
        }

        /// <summary>
        /// Performs the full interactive authentication process.
        /// </summary>
        /// <param name="credentials">The credentials to use.</param>
        /// <returns>A <see cref="DataSet"/> if successful.</returns>
#if !FRCORE
        private DataSet HandleWithInteractiveOAuth(GoogleSheetsCredentials credentials)
        {
            try
            {
                // creates a progress indicator and runs the authenticated task within it
                var indicator = ProgressIndicatorFactory.Create();

                return indicator.ShowAndRun((token) => RunAuthenticatedTaskAsync(token, credentials).GetAwaiter().GetResult()) ?? new DataSet();
            }
            catch (Exception)
            {
                if (LoginPrompt)
                    return new DataSet();
                else
                    throw;
            }
        }
#endif

        /// <summary>
        /// Executes the data retrieval task within an authenticated context.
        /// </summary>
        /// <param name="token">A cancellation token.</param>
        /// <param name="creds">The credentials to use.</param>
        /// <returns>A <see cref="DataSet"/> containing the retrieved data.</returns>
        private async Task<DataSet> RunAuthenticatedTaskAsync(CancellationToken token, GoogleSheetsCredentials creds)
        {
            var service = await GetSheetsServiceAsync(token, creds).ConfigureAwait(false);
            var connectionStringBuilder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
            return await _dataProvider.GetDataSetAsync(service, connectionStringBuilder, token).ConfigureAwait(false);
        }

        #endregion

        #region Token Caching

        /// <summary>
        /// Gets the file path for caching the OAuth token.
        /// The path is based on the hash of the current credentials.
        /// </summary>
        /// <returns>The file path for the token cache.</returns>
        private string GetTokenCachePath()
        {
            string key = GetTokenKey(this.CachedCredentials);
            string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataFolderName, SubFolderName);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, string.Format(TokenFileNamePattern, key));
        }

        /// <summary>
        /// Saves the OAuth token to a file.
        /// </summary>
        /// <param name="token">The token to save.</param>
        private void SaveTokenToFile(OAuthToken token)
        {
            if (token == null) return;
            string filePath = GetTokenCachePath();
            var jsonString = JsonSerializer.Serialize(token, typeof(OAuthToken));
            File.WriteAllText(filePath, jsonString);
        }

        /// <summary>
        /// Loads the OAuth token from a file.
        /// </summary>
        /// <returns>The loaded <see cref="OAuthToken"/>, or null if loading fails.</returns>
        private OAuthToken LoadTokenFromFile()
        {
            string filePath = GetTokenCachePath();
            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                var token = JsonSerializer.Deserialize<OAuthToken>(json);

                return token;
            }
            return null;
        }

        /// <summary>
        /// Generates a token key based on the provided Google Sheets credentials.
        /// The key is derived from the credential data itself (PathToJson, ClientId+ClientSecret) using a hash function.
        /// If credentials are incomplete, a placeholder key "incomplete_credentials" is returned.
        /// </summary>
        /// <param name="credentials">The Google Sheets credentials to generate the key for.</param>
        /// <returns>A unique token key based on the credentials, or a placeholder if incomplete.</returns>
        public static string GetTokenKey(GoogleSheetsCredentials credentials)
        {
            if (credentials == null)
                throw new ArgumentNullException(nameof(credentials));

            if (!string.IsNullOrEmpty(credentials.PathToJson))
            {
                return Crypter.ComputeHash(credentials.PathToJson);
            }
            else if (!string.IsNullOrEmpty(credentials.ClientId) && !string.IsNullOrEmpty(credentials.ClientSecret))
            {
                string credentialIdentifier = credentials.ClientId + credentials.ClientSecret;
                return Crypter.ComputeHash(credentialIdentifier);
            }
            else
            {
                // if credentials are incomplete, a unique key cannot be created
                // this situation occurs when the user has not entered any credentials yet
                return "incomplete_credentials";
            }
        }

        #endregion

        #region Overridden Methods

        /// <inheritdoc/>
        protected override DataSet CreateDataSet()
        {
            // gets the credentials
            // in FRCORE mode, they are taken from the Credentials property
            // in UI mode, they might be updated via ShowLoginDialogIfNeeded
            var creds = this.CachedCredentials;

            var result = TryHandleWithApiKey(creds);
            if (result != null) return result;

#if !FRCORE
            creds = new GoogleSheetsLoginUIManager().ShowLoginDialogIfNeeded(creds);

            // if the user canceled authentication, an empty DataSet is returned
            if (creds == null)
            {
                return new DataSet();
            }

            _credentials = creds; // updates the cache in case it was changed in the dialog
#endif
            if (!IsInteractiveLoginNeeded(creds))
            {
                result = TryHandleWithSilentOAuth(creds);
                if (result != null) return result;
            }
#if !FRCORE
            // if nothing worked, use interactive method
            return HandleWithInteractiveOAuth(creds);
#else
            // UI cannot be shown in Core mode
            // if the "silent" authentication fails, the OAuth flow is initiated directly
            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromMinutes(3)))
                {
                    return Task.Run(() => RunAuthenticatedTaskAsync(cts.Token, creds), cts.Token).GetAwaiter().GetResult();
                }
            }
            catch (Exception)
            {
                throw;
            }
#endif
        }

        /// <inheritdoc/>
        public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
            {
                try
                {
                    Task.Run(async () =>
                    {
                        var service = await GetSheetsServiceAsync(cts.Token, this.CachedCredentials).ConfigureAwait(false);
                        var connectionStringBuilder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
                        var schemaTable = await _dataProvider.CreateTableSchemaAsync(service, connectionStringBuilder.SpreadsheetId, table.TableName, connectionStringBuilder.FieldNamesInFirstRow, cts.Token).ConfigureAwait(false);

                        if (schemaTable != null)
                        {
                            table.Columns.Clear();
                            foreach (DataColumn column in schemaTable.Columns)
                            {
                                if (!table.Columns.Contains(column.ColumnName))
                                {
                                    table.Columns.Add(column.ColumnName, column.DataType);
                                }
                            }
                        }
                    }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,LoadSchemaError"), table.TableName, ex.Message), ex);
                }
            }
        }

        /// <inheritdoc/>
        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60)))
            {
                try
                {
                    Task.Run(async () =>
                    {
                        var service = await GetSheetsServiceAsync(cts.Token, this.CachedCredentials).ConfigureAwait(false);
                        var connectionStringBuilder = new GoogleSheetsConnectionStringBuilder(ConnectionString);
                        var spreadsheetId = _dataProvider.ExtractSpreadsheetId(connectionStringBuilder.SpreadsheetId);
                        await _dataProvider.FillTableDataAsync(table, service, spreadsheetId, connectionStringBuilder.FieldNamesInFirstRow, cts.Token).ConfigureAwait(false);
                    }).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,LoadDataError"), table.TableName, ex.Message), ex);
                }
            }
        }

        /// <inheritdoc/>
        protected override void SetConnectionString(string value)
        {
#if !FRCORE
            _credentials = null;
            _token = null;
#endif
            DisposeDataSet();
            Tables?.Clear();
            base.SetConnectionString(value);
        }

        /// <inheritdoc/>
        public override string[] GetTableNames()
        {
            var creds = this.CachedCredentials;
            DataSet dataSet = null;

            // !FRCORE + LoginPrompt = true - calls CreateDataSet() (with UI and full authentication)
            // !FRCORE + LoginPrompt = false - performs try/catch (silent authentication via token/API key)
            // FRCORE - the entire #if ... #else #endif block is ignored, and only try/catch is performed
#if !FRCORE
            if (LoginPrompt)
            {
                dataSet = CreateDataSet();
            }
            else
            { 
#endif
                try
                {
                    using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)))
                    {
                        dataSet = Task.Run(() => RunAuthenticatedTaskAsync(cts.Token, creds)).GetAwaiter().GetResult();
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(string.Format(Res.Get("ConnectionEditors,GoogleSheets,Errors,GetTableNamesError"), ex.Message), ex);
                }
#if !FRCORE
            }
#endif
            if (dataSet == null)
            {
                dataSet = new DataSet();
            }

            string[] tableNames = new string[dataSet.Tables.Count];
            for (int i = 0; i < dataSet.Tables.Count; i++)
            {
                tableNames[i] = dataSet.Tables[i].TableName;
            }
            return tableNames;
        }

        /// <inheritdoc/>
        public override void CreateTable(TableDataSource source)
        {
            if (DataSet.Tables.Contains(source.TableName))
            {
                source.Table = DataSet.Tables[source.TableName];
                base.CreateTable(source);
            }
            else
            {
                source.Table = null;
            }
        }

        /// <inheritdoc/>
        public override void DeleteTable(TableDataSource source) { }

        /// <inheritdoc/>
        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return value;
        }

#endregion
    }
}