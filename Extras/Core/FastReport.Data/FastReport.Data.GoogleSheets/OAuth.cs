using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    public static class OAuth
    {
        public static int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public static bool IsNeedRefresh(this OAuthToken OAtoken)
        {
            return DateTime.Now > OAtoken.CreateTime.AddSeconds(OAtoken.ExpiresIn);
        }

        public static async Task Refresh(this OAuthToken token, HttpClient client, string clientId, string clientSecret, string getTokenUrl, CancellationToken cancelToken = default)
        {
            Log("Refreshing token...");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "refresh_token", token.RefreshToken }
            });

            HttpResponseMessage tokenResponse = await client.PostAsync(getTokenUrl, content, cancelToken);
            await HandleResponse(tokenResponse);

            // reads response body
            string responseText = await tokenResponse.Content.ReadAsStringAsync();

            var newToken = JsonSerializer.Deserialize<OAuthToken>(responseText);
            token.Assign(newToken);
            token.CreateTime = DateTime.Now;
        }

        internal static async Task<OAuthToken> DoOAuthAsync(HttpClient client, string clientId, string clientSecret, string scope, string getCodeUrl, string getTokenUrl, int listenerPort = 0, CancellationToken cancelToken = default)
        {
            // Generates state and PKCE values.
            string state = GenerateRandomDataBase64url(32);
            string codeVerifier = GenerateRandomDataBase64url(32);
            string codeChallenge = Base64UrlEncodeNoPadding(Sha256Ascii(codeVerifier));
            const string codeChallengeMethod = "S256";

            if (listenerPort == 0)
                listenerPort = GetRandomUnusedPort();
            // Creates a redirect URI using an available port on the loopback address.
            string redirectUri = $"http://{IPAddress.Loopback}:{listenerPort}/meter/";
            Log("redirect URI: " + redirectUri);

            HttpListenerContext context = null;
            // Creates an HttpListener to listen for requests on that redirect URI.
            using (var http = new HttpListener())
            {
                http.Prefixes.Add(redirectUri);
                Log("Listening..");
                http.Start();

                // Creates the OAuth 2.0 authorization request.
                string authorizationRequest = string.Format("{0}?response_type=code&scope={6}&redirect_uri={1}&client_id={2}&state={3}&code_challenge={4}&code_challenge_method={5}",
                    getCodeUrl,
                    Uri.EscapeDataString(redirectUri),
                    clientId,
                    state,
                    codeChallenge,
                    codeChallengeMethod,
                    scope);

                // Opens request in the browser.
                Process.Start(new ProcessStartInfo(authorizationRequest) { UseShellExecute = true });

                // Canceling authorization using CancellationToken.
                var getContextTask = http.GetContextAsync();

                var cancellationTask = Task.Delay(Timeout.Infinite, cancelToken);

                var completedTask = await Task.WhenAny(getContextTask, cancellationTask);

                if (completedTask == cancellationTask)
                {
                    http.Stop();
                    http.Close();
                    throw new OperationCanceledException(cancelToken);
                }

                // Waits for the OAuth authorization response.
                context = await getContextTask;
                // Sends an HTTP response to the browser.
                var response = context?.Response;
                string responseString = "<html><script type=\"text/javascript\">window.open('''', ''_self'', '''').close();</script><body>Close the window.</body></html>";
                byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                response.ContentLength64 = buffer.Length;
                var responseOutput = response.OutputStream;
                await responseOutput.WriteAsync(buffer, 0, buffer.Length);
                responseOutput.Close();
            }
            Log("HTTP server stopped.");

            // Checks for errors.
            var error = context?.Request.QueryString.Get("error");
            // Extracts the code.
            var code = context?.Request.QueryString.Get("code");
            var incomingState = context?.Request.QueryString.Get("state");
            if (!string.IsNullOrEmpty(error))
            {
                Log($"OAuth authorization error: {error}.");
                return null;
            }
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(incomingState))
            {
                Log($"Invalid authorization response message. {context?.Request.QueryString}");
                return null;
            }

            // Compares the receieved state to the expected value, to ensure that
            // this app made the request which resulted in authorization.
            if (incomingState != state)
            {
                Log($"Received request with invalid state ({incomingState})");
                return null;
            }
            Log("Authorization code: " + code);
            // Starts the code exchange at the Token Endpoint.
            return await ExchangeCodeForTokensAsync(client, code, codeVerifier, redirectUri, clientId, clientSecret, getTokenUrl, cancelToken);
        }

        async static Task<OAuthToken> ExchangeCodeForTokensAsync(HttpClient client, string code, string codeVerifier, string redirectUri, string clientId, string clientSecret, string getTokenUrl, CancellationToken cancelToken = default)
        {
            Log("Exchanging code for tokens...");

            // builds the  request
            string body = $"grant_type=authorization_code&client_id={clientId}&client_secret={clientSecret}" +
                $"&redirect_uri={Uri.EscapeDataString(redirectUri)}&code_verifier={codeVerifier}&code={code}";

            var content = new ByteArrayContent(Encoding.ASCII.GetBytes(body));
            content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

            // sends the request
            HttpResponseMessage tokenResponse = await client.PostAsync(getTokenUrl, content, cancelToken);
            await HandleResponse(tokenResponse);

            // converts to dictionary
            return JsonSerializer.Deserialize<OAuthToken>(await tokenResponse.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Appends the given string to the on-screen log, and the debug console.
        /// </summary>
        /// <param name="output">String to be logged</param>
        private static void Log(string output)
        {
            Debug.WriteLine(output);
        }

        /// <summary>
        /// Returns URI-safe data with a given input length.
        /// </summary>
        /// <param name="length">Input length (nb. output will be longer)</param>
        /// <returns></returns>
        private static string GenerateRandomDataBase64url(uint length)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[length];
                rng.GetBytes(bytes);
                return Base64UrlEncodeNoPadding(bytes);
            }
        }

        /// <summary>
        /// Returns the SHA256 hash of the input string, which is assumed to be ASCII.
        /// </summary>
        private static byte[] Sha256Ascii(string text)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(text);
            using (var sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(bytes);
            }
        }

        /// <summary>
        /// Base64url no-padding encodes the given input buffer.
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        private static string Base64UrlEncodeNoPadding(byte[] buffer)
        {
            string base64 = Convert.ToBase64String(buffer);
            // Converts base64 to base64url.
            base64 = base64.Replace('+', '-');
            base64 = base64.Replace('/', '_');
            // Strips padding.
            base64 = base64.Replace("=", "");

            return base64;
        }

        internal static async Task HandleResponse(HttpResponseMessage httpResponseMessage)
        {
            if (httpResponseMessage != null)
            {
                try
                {
                    httpResponseMessage.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                    ex.Data.Add("ResponseMessage", await httpResponseMessage.Content.ReadAsStringAsync());
                    throw;
                }
            }
        }
    }
}
