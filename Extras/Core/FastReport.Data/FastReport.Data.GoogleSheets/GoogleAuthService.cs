using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using System;
using System.IO;
using System.Threading;

namespace FastReport.Data
{
	public class GoogleAuthService
	{
		#region Public Methods

		/// <summary>
		/// Getting a token by connecting to a .json file
		/// </summary>
		/// <param name="path">Path to the location of the .json file containing the Google client secret data</param>
		/// <returns>UserCredential with access token</returns>
		public static UserCredential GetAccessToken(string path)
		{
			string[] scopes = new string[] { SheetsService.Scope.Spreadsheets };

			using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
					GoogleClientSecrets.FromStream(stream).Secrets,
					scopes,
					"userName",
					CancellationToken.None).Result;

				return credential;
			}
		}

		/// <summary>
		/// Getting a token over an OAuth 2.0 connection
		/// </summary>
		/// <param name="clientId">Google Client ID</param>
		/// <param name="clientSecret">Google Client Secret</param>
		/// <returns>UserCredential with access token</returns>
		public static UserCredential GetAccessToken(string clientId, string clientSecret)
		{
			string[] scopes = new string[] { SheetsService.Scope.Spreadsheets };

			var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
			new ClientSecrets
			{
				ClientId = clientId,
				ClientSecret = clientSecret
			},
			scopes,
			Environment.UserName,
			CancellationToken.None).Result;

			return credential;
		}

		#endregion Public Methods
	}
}