using FastReport.Data.ConnectionEditors;
using FastReport.Utils;
using System;

namespace FastReport.Data
{
    public partial class GoogleSheetsDataConnection
    {
        #region Public Methods

        /// <inheritdoc/>
        public override void TestConnection()
        {
            using (var dataset = CreateDataSet())
            {
                // checks if the connection was successful, but did not return tables
                // this may indicate a problem with the table ID or permissions
                if (dataset == null || dataset.Tables.Count == 0)
                    throw new InvalidOperationException(Res.Get("ConnectionEditors,GoogleSheets,Errors,TestConnectionNoDataError"));
            }
            // if the code has reached this point, the connection test was successful
            // exceptions from CreateDataSet() will float to the top without additional wrapping
        }

        /// <inheritdoc/>
        public override ConnectionEditorBase GetEditor()
        {
            return new GoogleSheetsConnectionEditor();
        }

        /// <inheritdoc/>
        public override string GetConnectionId()
        {
            return "GoogleSheets: " + SpreadsheetId;
        }

        #endregion
    }
}
