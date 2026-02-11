using System.Text.Json.Serialization;
using System;

namespace FastReport.Data
{
    public class OAuthToken : Google.Apis.Http.IConfigurableHttpClientInitializer
    {
        [JsonPropertyName("create_time")]
        public DateTime CreateTime { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }

        internal void Assign(OAuthToken token)
        {
            if (token == null)
                return;
            AccessToken = token.AccessToken;
            TokenType = token.TokenType;
            ExpiresIn = token.ExpiresIn;
            Scope = token.Scope;
        }

        public OAuthToken()
        {
            CreateTime = DateTime.Now;
        }

        // implementation of the IConfigurableHttpClientInitializer interface
        public void Initialize(Google.Apis.Http.ConfigurableHttpClient httpClient)
        {
            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", this.AccessToken);
        }
    }
}