using FastReport.Utils;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// A client wrapper for simplifying interactions with the Google Sheets API.
    /// </summary>
    public class GoogleSheetsClient : IGoogleSheetsClient
    {
        public const string ApplicationName = "FastReport.Data.GoogleSheets";

        /// <inheritdoc/>
        public SheetsService InitService(UserCredential credential)
        {
            if (credential == null)
                throw new ArgumentNullException(nameof(credential));

            // standard way to initialize a Google API service client
            return new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = ApplicationName,
                HttpClientInitializer = credential,
            });
        }

        /// <inheritdoc/>
        public SheetsService InitService(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,ApiKeyRequiredError"), nameof(apiKey));

            return new SheetsService(new BaseClientService.Initializer()
            {
                ApplicationName = ApplicationName,
                ApiKey = apiKey,
            });
        }

        /// <inheritdoc/>
        public async Task<IList<IList<object>>> ReadDataAsync(SheetsService service, string spreadsheetId, string range, CancellationToken token)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,SpreadsheetIdRequiredError"), nameof(spreadsheetId));
            if (string.IsNullOrEmpty(range))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,RangeRequiredError"), nameof(range));
            
            // this line only builds the request object; it does not execute it
            var request = service.Spreadsheets.Values.Get(spreadsheetId, range);

            request.ValueRenderOption = SpreadsheetsResource.ValuesResource.GetRequest.ValueRenderOptionEnum.FORMATTEDVALUE;

            // .ConfigureAwait(false) is an optimization that prevents potential deadlocks
            // by not forcing the continuation to run on the original context (e.g., the UI thread)
            var response = await request.ExecuteAsync(token).ConfigureAwait(false);
            
            return response.Values ?? new List<IList<object>>();
        }

        /// <inheritdoc/>
        public async Task<Spreadsheet> ReadSpreadSheetAsync(SheetsService service, string spreadsheetId, CancellationToken token)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,SpreadsheetIdRequiredError"), nameof(spreadsheetId));
            
            // creates the request object to fetch spreadsheet metadata
            var request = service.Spreadsheets.Get(spreadsheetId);


            try
            {
                Spreadsheet spreadsheet = await request.ExecuteAsync(token).ConfigureAwait(false);
                return spreadsheet;
            }
            catch (Google.GoogleApiException ex)
            {
                throw new InvalidOperationException($"{ex.HttpStatusCode} - {ex.Message}", ex);
            }
        }
    }
}