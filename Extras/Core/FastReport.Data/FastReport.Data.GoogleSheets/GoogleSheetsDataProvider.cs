using FastReport.Utils;
using Google.Apis.Sheets.v4;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Data
{
    /// <summary>
    /// Provides methods for fetching data and schema from Google Sheets using a <see cref="SheetsService"/>.
    /// </summary>
    public class GoogleSheetsDataProvider : IGoogleSheetsDataProvider
    {
        private readonly IGoogleSheetsClient _sheetsClient;

        /// <summary>
        /// Initializes a new instance of the GoogleSheetsDataProvider with the specified client.
        /// </summary>
        /// <param name="sheetsClient">The client to use for API calls.</param>
        public GoogleSheetsDataProvider(IGoogleSheetsClient sheetsClient)
        {
            _sheetsClient = sheetsClient;
        }

        /// <inheritdoc/>
        public async Task<DataSet> GetDataSetAsync(SheetsService service, GoogleSheetsConnectionStringBuilder connectionStringBuilder, CancellationToken token)
        {
            var spreadsheetId = ExtractSpreadsheetId(connectionStringBuilder.SpreadsheetId);
            try
            {
                var spreadsheet = await _sheetsClient.ReadSpreadSheetAsync(service, spreadsheetId, token).ConfigureAwait(false);
                var dataSet = new DataSet();

                foreach (var sheet in spreadsheet.Sheets)
                {
                    if (connectionStringBuilder.IncludeHiddenSheets == false && sheet.Properties.Hidden == true)
                        continue;

                    var table = await CreateTableSchemaAsync(service, spreadsheet.SpreadsheetId, sheet.Properties.Title, connectionStringBuilder.FieldNamesInFirstRow, token).ConfigureAwait(false);
                    if (table != null)
                    {
                        dataSet.Tables.Add(table);
                    }
                }
                return dataSet;
            }
            catch (Exception)
            {
                return new DataSet();
            }
        }

        /// <inheritdoc/>
        public async Task FillTableDataAsync(DataTable table, SheetsService service, string spreadsheetId, bool fieldNamesInFirstRow, CancellationToken token)
        {
            // loading all the lines of the sheet
            var rawLines = await _sheetsClient.ReadDataAsync(service, spreadsheetId, table.TableName, token).ConfigureAwait(false);
            if (rawLines == null || rawLines.Count == 0)
                return;

            if (fieldNamesInFirstRow)
            {
                rawLines.RemoveAt(0);
            }

            foreach (var row in rawLines)
            {
                var dataRow = table.NewRow();
                for (int j = 0; j < Math.Min(row.Count, table.Columns.Count); j++)
                {
                    dataRow[j] = row[j] ?? DBNull.Value;
                }
                table.Rows.Add(dataRow);
            }
        }

        /// <inheritdoc/>
        public async Task<DataTable> CreateTableSchemaAsync(SheetsService service, string spreadSheetsId, string sheetTitle, bool fieldNamesInFirstRow, CancellationToken token)
        {
            var table = new DataTable(sheetTitle);

            // reads only the first row ('1:1') of the specified sheet to get column names
            var firstRowData = await _sheetsClient.ReadDataAsync(service, spreadSheetsId, $"'{sheetTitle}'!1:1", token).ConfigureAwait(false);
            if (firstRowData == null || firstRowData.Count == 0)
                return null;

            var firstRow = firstRowData[0];

            var columnNames = new HashSet<string>();
            int emptyNameCounter = 1;

            for (int i = 0; i < firstRow.Count; i++)
            {
                string baseName = fieldNamesInFirstRow ? firstRow[i]?.ToString() : null;
                if (string.IsNullOrWhiteSpace(baseName))
                {
                    baseName = "Column" + emptyNameCounter++;
                }

                string finalName = baseName;
                int suffix = 1;
                while (columnNames.Contains(finalName))
                {
                    finalName = $"{baseName}_{suffix++}";
                }
                columnNames.Add(finalName);
                table.Columns.Add(finalName, typeof(string));
            }

            return table;
        }

        /// <inheritdoc/>
        public async Task<string[]> GetSheetNamesOnlyAsync(SheetsService service, string spreadsheetId, bool includeHiddenSheets, CancellationToken token)
        {
            if (string.IsNullOrEmpty(spreadsheetId))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,SpreadsheetIdRequiredError"), nameof(spreadsheetId));

            var request = service.Spreadsheets.Get(spreadsheetId);
            // specifies that only the title and hidden status of sheets are needed
            request.Fields = "sheets.properties.title,sheets.properties.hidden";

            var response = await request.ExecuteAsync(token).ConfigureAwait(false);

            if (response?.Sheets == null)
                return Array.Empty<string>();

            return response.Sheets
                .Where(sheet => sheet.Properties?.Title != null)
                .Where(sheet => includeHiddenSheets || sheet.Properties.Hidden != true)
                .Select(sheet => sheet.Properties.Title)
                .ToArray();
        }

        /// <inheritdoc/>
        public string ExtractSpreadsheetId(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,SpreadsheetUrlOrIdCannotBeEmptyError"), nameof(input));

            var regex = new Regex(@"/spreadsheets/d/([a-zA-Z0-9_-]+)", RegexOptions.IgnoreCase);
            var match = regex.Match(input);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            if (!Regex.IsMatch(input, @"^[a-zA-Z0-9_-]+$"))
            {
                throw new ArgumentException(Res.Get("ConnectionEditors,GoogleSheets,Errors,InvalidSpreadsheetUrlOrIdError"), nameof(input));
            }

            return input;
        }
    }
}
