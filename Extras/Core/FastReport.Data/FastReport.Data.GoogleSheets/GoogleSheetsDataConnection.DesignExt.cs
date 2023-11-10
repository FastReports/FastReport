using FastReport.Data.ConnectionEditors;
using System.Data;
using System;

namespace FastReport.Data
{
	public partial class GoogleSheetsDataConnection
	{
		#region Public Methods

		/// <inheritdoc/>
		public override void TestConnection()
		{
			using (DataSet dataset = CreateDataSet())
			{

			}
		}

		/// <inheritdoc/>
		public override ConnectionEditorBase GetEditor()
		{
			return new GoogleSheetsConnectionEditor();
		}

		/// <inheritdoc/>
		public override string GetConnectionId()
		{
			return "GoogleSheets: " + Sheets;
		}

		#endregion Public Methods
	}
}
