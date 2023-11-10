using FastReport.Utils;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace FastReport.Data
{
	internal static class GoogleSheetsUtils
	{
		/// <summary>
		/// The default field name.
		/// </summary>
		private const string DEFAULT_FIELD_NAME = "Field";

        /// <summary>
		/// Maximum speaker range.
		/// </summary>
        private const string ALL_RANGE = "!A:ZZZ";

        internal static Spreadsheet ReadTable(GoogleSheetsConnectionStringBuilder builder)
        {
            XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
            string id = xi.GetProp("ClientId");
            string secret = xi.GetProp("ClientSecret");
            string pathToJson = xi.GetProp("PathToJson");
            string apiKey = xi.GetProp("ApiKey");

            // checking for empty account data
            if (!String.IsNullOrEmpty(apiKey))
                GoogleSheets.InitService(apiKey);
            else if (!String.IsNullOrEmpty(pathToJson))
                GoogleSheets.InitService(GoogleAuthService.GetAccessToken(pathToJson));
            else if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(secret))
                GoogleSheets.InitService(GoogleAuthService.GetAccessToken(id, secret));
            else
                throw new Exception(Res.Get("ConnectionEditors,GoogleSheets,FailedToLogin"));

            // checking for an empty URL string
            if (String.IsNullOrEmpty(builder.Sheets))
                throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));

            string gId = builder.Sheets;

            if (builder.Sheets.StartsWith("https://docs.google.com/spreadsheets/d/"))
            {
                string idGoogle = builder.Sheets.Remove(0, 39);
                int startIndex = idGoogle.IndexOf("/");

                if (startIndex == -1)
                    throw new Exception(Res.Get("ConnectionEditors,Common,OnlyUrlException"));

                gId = idGoogle.Substring(0, startIndex);
            }

            var objects = GoogleSheets.ReadSpreadSheet(gId);

            return objects;
        }

        internal static DataTable CreateDataTable(string spreadSheetsId, GoogleSheetsConnectionStringBuilder builder)
		{
            //Receiving a sheet
            var rawLines = GoogleSheets.ReadData(spreadSheetsId, builder.TableName + ALL_RANGE);

            if (rawLines == null)
				return null;

            //maxCount is needed to determine the maximum number of cells in the row width
            int maxCount = rawLines.Max(l => l.Count);
            DataTable table = new DataTable(builder.TableName);

            //Adding empty columns to align the first row
            for (int i = 0; i < maxCount; i++)
                if (rawLines[0].Count < maxCount)
                    rawLines[0].Add("");

            if (builder.FieldNamesInFirstString == true)
			{
                foreach (var column in rawLines[0])
                {
                    table.Columns.Add(column.ToString());
                }
				for (int i = 1; i < rawLines.Count; i++)
                {
                    var row = rawLines[i];
                    var dataRow = table.NewRow();

                    for (int j = 0; j < row.Count; j++)
                    {
                        dataRow[j] = row[j].ToString();
                    }

                    table.Rows.Add(dataRow);
                }
			}
			else
			{
				int count = 0;

				foreach (var column in rawLines[0])
				{
					table.Columns.Add(DEFAULT_FIELD_NAME + count.ToString());
                    count++;
                }

                for (int i = 1; i < rawLines.Count; i++)
                {
                    var row = rawLines[i];
                    var dataRow = table.NewRow();

                    for (int j = 0; j < row.Count; j++)
                    {
                        dataRow[j] = row[j].ToString();
                    }

                    table.Rows.Add(dataRow);
                }
            }

			return table;
		}
    }
}
