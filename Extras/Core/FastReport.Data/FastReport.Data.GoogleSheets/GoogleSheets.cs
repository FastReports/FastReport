using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FastReport.Data
{
	/// <summary>
	/// Provides connection, reading and writing data from Google Sheets
	/// </summary>
	public class GoogleSheets
	{
		#region Private Fields

		private static SheetsService service;

		#endregion Private Fields

		#region Public Method

		/// <summary>
		/// Initializing the Sheets service with an Access Token
		/// </summary>
		/// <param name="credential">Contains an access token for the Google Sheets API</param>
		public static void InitService(UserCredential credential)
		{
			service = new SheetsService(new BaseClientService.Initializer()
			{
				ApplicationName = " ",
				HttpClientInitializer = credential,
			});
		}

		/// <summary>
		/// Initializing the Sheets service using an API key
		/// </summary>
		/// <param name="APIkey">Contains an API key to access the Google Sheets API</param>
		/// <returns>SheetsService for performing operations with the Google Sheets API</returns>
		public static SheetsService InitService(string APIkey)
		{
			service = new SheetsService(new BaseClientService.Initializer()
			{
				ApplicationName = " ",
				ApiKey = APIkey,
			});

			return service;
		}

		/// <summary>
		/// Read data from a sheet specifying a range of cells
		/// </summary>
		/// <param name="spreadsheetsId">Google Sheets Table ID</param>
		/// <param name="range">Range of cells to be read</param>
		/// <returns>A list of lists of objects (IList<IList<object>>) that contains the data read</returns>
		public static IList<IList<object>> ReadData(string spreadsheetsId, string range)
		{
			var response = service.Spreadsheets.Values.Get(spreadsheetsId, range).Execute();

            return response.Values;
		}

		/// <summary>
		/// Read data from a sheet specifying a range of cells
		/// </summary>
		/// <param name="spreadsheetsId">Google Sheets Table ID</param>
		/// <param name="startColumn">Range start column to be read</param>
		/// <param name="endColumn">Range end column to be read</param>
		/// <returns>A list of lists of objects (IList<IList<object>>) that contains the data read</returns>
		public static IList<IList<object>> ReadData(string spreadsheetsId, string startColumn, string endColumn)
		{
			var range = startColumn + ":" + endColumn;

			var response = service.Spreadsheets.Values.Get(spreadsheetsId, range).Execute();

			return response.Values;
		}

        /// <summary>
        /// Reading data from a table
        /// </summary>
        /// <param name="spreadsheetsId">Google Sheets Table ID</param>
        /// <returns>Returns a Spreadsheet with information about the table</returns>
        public static Spreadsheet ReadSpreadSheet (string spreadsheetsId)
        {
            var spreadsheet = service.Spreadsheets.Get(spreadsheetsId).Execute();

            return spreadsheet;
        }

        #endregion Public Method
    }
}