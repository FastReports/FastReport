using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Defines a client for interacting with the Google Sheets API.
    /// </summary>
    public interface IGoogleSheetsClient
    {
        /// <summary>
        /// Initializes a new <see cref="SheetsService"/> using OAuth 2.0 user credentials.
        /// </summary>
        /// <param name="credential">The OAuth 2.0 user credential.</param>
        /// <returns>An initialized <see cref="SheetsService"/>.</returns>
        SheetsService InitService(UserCredential credential);

        /// <summary>
        /// Initializes a new <see cref="SheetsService"/> using a simple API key.
        /// </summary>
        /// <param name="apiKey">The API key for authentication.</param>
        /// <returns>An initialized <see cref="SheetsService"/>.</returns>
        SheetsService InitService(string apiKey);

        /// <summary>
        /// Asynchronously reads cell data from a specified spreadsheet and range.
        /// </summary>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="spreadsheetsId">The ID of the spreadsheet.</param>
        /// <param name="range">The A1 notation range to read data from (e.g., "Sheet1!A1:B10").</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        /// <returns>A list of rows, where each row is a list of cell values.</returns>
        Task<IList<IList<object>>> ReadDataAsync(SheetsService service, string spreadsheetsId, string range, CancellationToken token);

        /// <summary>
        /// Asynchronously reads the metadata for an entire spreadsheet.
        /// </summary>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="spreadsheetsId">The ID of the spreadsheet.</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        /// <returns>A <see cref="Spreadsheet"/> object containing its metadata (properties, sheets, etc.).</returns>
        Task<Spreadsheet> ReadSpreadSheetAsync(SheetsService service, string spreadsheetsId, CancellationToken token);
    }
}