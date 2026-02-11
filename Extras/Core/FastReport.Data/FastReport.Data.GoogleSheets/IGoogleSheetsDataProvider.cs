using Google.Apis.Sheets.v4;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Defines a provider responsible for fetching data from Google Sheets and constructing a <see cref="DataSet"/>.
    /// </summary>
    public interface IGoogleSheetsDataProvider
    {
        /// <summary>
        /// Asynchronously fetches the schema of all relevant sheets from a Google Sheets spreadsheet and returns them as a <see cref="DataSet"/>.
        /// The returned <see cref="DataTable"/>s will have columns defined but will contain no data rows.
        /// </summary>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="connectionStringBuilder">The connection string builder containing all connection options.</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        /// <returns>A <see cref="DataSet"/> containing a <see cref="DataTable"/> for each accessible sheet, with schema only.</returns>
        Task<DataSet> GetDataSetAsync(SheetsService service, GoogleSheetsConnectionStringBuilder connectionStringBuilder, CancellationToken token);

        /// <summary>
        /// Asynchronously fills the provided <see cref="DataTable"/> with data from the corresponding sheet.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/> to fill. Its name corresponds to the sheet title.</param>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="spreadsheetId">The ID of the spreadsheet.</param>
        /// <param name="fieldNamesInFirstRow">Indicates if the first row in the sheet is a header and should be skipped.</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        Task FillTableDataAsync(DataTable table, SheetsService service, string spreadsheetId, bool fieldNamesInFirstRow, CancellationToken token);

        /// <summary>
        /// Asynchronously creates a <see cref="DataTable"/> schema based on the structure of a specified sheet.
        /// This method fetches only the necessary data (typically the first row) to define the columns and their names/types.
        /// </summary>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="spreadSheetsId">The ID of the spreadsheet containing the target sheet.</param>
        /// <param name="sheetTitle">The title of the sheet for which to create the schema.</param>
        /// <param name="fieldNamesInFirstRow">Indicates if the first row in the sheet defines column headers.</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        /// <returns>A <see cref="DataTable"/> representing the schema of the specified sheet.</returns>
        Task<DataTable> CreateTableSchemaAsync(SheetsService service, string spreadSheetsId, string sheetTitle, bool fieldNamesInFirstRow, CancellationToken token);

        /// <summary>
        /// Asynchronously retrieves the names of all sheets within a specified Google Spreadsheet.
        /// Optionally includes or excludes sheets that are marked as hidden.
        /// </summary>
        /// <param name="service">The authenticated <see cref="SheetsService"/>.</param>
        /// <param name="spreadsheetId">The ID of the spreadsheet to query.</param>
        /// <param name="includeHiddenSheets">A flag indicating whether hidden sheets should be included in the result.</param>
        /// <param name="token">A cancellation token to cancel the request.</param>
        /// <returns>An array of strings, where each string is the name of a sheet.</returns>
        Task<string[]> GetSheetNamesOnlyAsync(SheetsService service, string spreadsheetId, bool includeHiddenSheets, CancellationToken token);

        /// <summary>
        /// Extracts the unique identifier (Spreadsheet ID) from a Google Sheets URL or validates an already existing ID string.
        /// </summary>
        /// <param name="input">The input string, which can be either a full Google Sheets URL or the raw Spreadsheet ID.</param>
        /// <returns>The extracted or validated Spreadsheet ID.</returns>
        string ExtractSpreadsheetId(string input);
    }
}
