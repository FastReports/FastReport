using System;
using FastReport.Data.ConnectionEditors;
using FastReport.Utils;

namespace FastReport.Data
{
    internal partial class GoogleSheetsConnectionEditor : ConnectionEditorBase
	{
		#region Fields

		private bool updating;

		#endregion Fields

		#region Constructors
		public GoogleSheetsConnectionEditor()
		{
			updating = true;
			InitializeComponent();
			CheckSignInGoogleAPI();
			Localize();
			updating = false;
		}

		#endregion Constructors

		#region Events Handlers

		private void btSignInGoogle_Click(object sender, EventArgs e)
		{
			using (SignInGoogle signInGoogle = new SignInGoogle())
			{
				signInGoogle.ShowDialog();
			}
		}

		#endregion Events Handlers

		#region Protected Methods

		protected override string GetConnectionString()
		{
			GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder();
			builder.Sheets = tbGoogleId.Text;
			builder.FieldNamesInFirstString = cbxFieldNames.Checked;
			return builder.ToString();
		}

		protected override void SetConnectionString(string value)
		{
			GoogleSheetsConnectionStringBuilder builder = new GoogleSheetsConnectionStringBuilder(value);
			tbGoogleId.Text = builder.Sheets;
			cbxFieldNames.Checked = builder.FieldNamesInFirstString;
		}

		#endregion Protected Methods

		#region Private Methods

		private void Localize()
		{
			MyRes res = new MyRes("ConnectionEditors,GoogleSheets");
			gbSelect.Text = res.Get("ConfigureDatabase");
			lblSelectGId.Text = res.Get("SelectSheetsId");
			cbxFieldNames.Text = res.Get("FieldNames");
			btSignInGoogle.Text = res.Get("SignIn");
		}

		private void CheckSignInGoogleAPI ()
		{
			XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
			string id = xi.GetProp("ClientId");
			string secret = xi.GetProp("ClientSecret");
			string pathToJson = xi.GetProp("PathToJson");
			string apiKey = xi.GetProp("ApiKey");
			if (String.IsNullOrEmpty(id) && String.IsNullOrEmpty(secret) && String.IsNullOrEmpty(pathToJson) && String.IsNullOrEmpty(apiKey))
			{
				using (SignInGoogle signInGoogle = new SignInGoogle())
					signInGoogle.ShowDialog();
			}
		}

		#endregion Private Methods

		#region Public Methods

		public override void UpdateDpiDependencies()
		{
			base.UpdateDpiDependencies();
		}

		#endregion Public Methods
	}
}