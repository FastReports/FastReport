using System;
using System.Windows.Forms;
using FastReport.Forms;
using FastReport.Utils;
using Google.Apis.Auth.OAuth2;

namespace FastReport.Data
{
	internal partial class SignInGoogle : BaseDialogForm
	{
		#region Fields

		private string apiKey;
		private string clientId;
		private string clientSecret;
		private string pathToJson;
		private bool isUserAuthorized;

		#endregion Fields

		#region Properties

		/// <summary>
		/// Gets or sets user's api key.
		/// </summary>
		public string ApiKey
		{
			get { return apiKey; }
			set { apiKey = value; }
		}

		/// <summary>
		/// Gets or sets client identifier from the OAuth2 specification.
		/// </summary>
		public string ClientId
		{
			get { return clientId; }
			set { clientId = value; }
		}

		/// <summary>
		/// Gets or sets client secret from the OAuth2 specification.
		/// </summary>
		public string ClientSecret
		{
			get { return clientSecret; }
			set { clientSecret = value; }
		}

		/// <summary>
		/// Gets or sets client file .json from the OAuth2 specification.
		/// </summary>
		public string PathToJson
		{
			get { return pathToJson; }
			set { pathToJson = value; }
		}

		/// <summary>
		/// Gets or sets the information is user authorized or not.
		/// </summary>
		public bool IsUserAuthorized
		{
			get { return isUserAuthorized; }
			set { isUserAuthorized = value; }
		}

		#endregion Properties

		#region Constructors
		public SignInGoogle()
		{
			this.clientId = "";
			this.clientSecret = "";
			this.pathToJson = "";
			this.apiKey = "";
			this.isUserAuthorized = false;

			InitializeComponent();
			Localize();
			InitSignInList();
            UIUtils.CheckRTL(this);
			UpdateDpiDependencies();
		}

		#endregion Constructors

		#region Events Handlers

		private void tbPathToJsonFile_ButtonClick(object sender, EventArgs e)
		{
			using (OpenFileDialog dialog = new OpenFileDialog())
			{
				dialog.Filter = Res.Get("FileFilters,JsonFile");
				if (dialog.ShowDialog() == DialogResult.OK)
					tbPathToJsonFile.Text = dialog.FileName;
			}
		}

		private void cbxPathToJsonFile_CheckedChanged(object sender, EventArgs e)
		{
			if (cbxPathToJsonFile.Checked)
			{
				lblPath.Visible = true;
				lblClientId.Visible = false;
				lblClientSecret.Visible = false;

				tbPathToJsonFile.Visible = true;
				tbClientId.Visible = false;
				tbClientSecret.Visible = false;
			}
			else
			{
				lblPath.Visible = false;
				lblClientId.Visible = true;
				lblClientSecret.Visible = true;

				tbPathToJsonFile.Visible = false;
				tbClientId.Visible = true;
				tbClientSecret.Visible = true;
			}
		}

		private void btnSignIn_Click(object sender, EventArgs e)
		{
			if (!String.IsNullOrEmpty(tbClientId.Text) && !String.IsNullOrEmpty(tbClientSecret.Text))
			{
				clientId = tbClientId.Text;
				clientSecret = tbClientSecret.Text;
				var token = GoogleAuthService.GetAccessToken(ClientId, ClientSecret);
				IsUserAuthorized = true;
				XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
				xi.SetProp("ClientId", ClientId);
				xi.SetProp("ClientSecret", ClientSecret);
                xi.SetProp("PathToJson", "");
                xi.SetProp("ApiKey", "");
                xi.SetProp("IsUserAuthorized", IsUserAuthorized.ToString());
				xi.SetProp("AccessToken", token.ToString());
				DialogResult = DialogResult.OK;
                this.Close();
			}	
			else if (!String.IsNullOrEmpty(tbPathToJsonFile.Text) && cbxPathToJsonFile.Checked)
			{
				pathToJson = tbPathToJsonFile.Text;
				var token = GoogleAuthService.GetAccessToken(PathToJson);
				IsUserAuthorized = true;
				XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
                xi.SetProp("ClientId", "");
                xi.SetProp("ClientSecret", "");
                xi.SetProp("PathToJson", PathToJson);
                xi.SetProp("ApiKey", "");
                xi.SetProp("IsUserAuthorized", IsUserAuthorized.ToString());
				xi.SetProp("AccessToken", token.ToString());
				DialogResult = DialogResult.OK;
                this.Close();
			}
			else if (!String.IsNullOrEmpty(tbApiKey.Text))
			{
				apiKey = tbApiKey.Text;
				IsUserAuthorized = true;
				XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
                xi.SetProp("ClientId", "");
                xi.SetProp("ClientSecret", "");
                xi.SetProp("PathToJson", "");
                xi.SetProp("ApiKey", ApiKey);
				xi.SetProp("IsUserAuthorized", IsUserAuthorized.ToString());
				DialogResult = DialogResult.OK;
                this.Close();
			}

			if (IsUserAuthorized == true)
			{
                MessageBox.Show(Res.Get("Forms,SignInGoogle,OnConnection"));
            }
			else
			{
				MessageBox.Show(Res.Get("Forms,SignInGoogle,OffConnection"));
			}
		}

		private void cbxChangeSignIn_SelectedIndexChanged(object sender, EventArgs e)
		{
            if (cbxChangeSignIn.SelectedIndex == 0)
            {
                gbxSignInGoogleAPI.Visible = true;
                gbxSignInGoogleAPI.Enabled = true;

                gbxGoogleApiKey.Visible = false;
                gbxGoogleApiKey.Enabled = false;
            }
            else
            {
                gbxSignInGoogleAPI.Visible = false;
                gbxSignInGoogleAPI.Enabled = false;

                gbxGoogleApiKey.Visible = true;
                gbxGoogleApiKey.Enabled = true;
            }
        }

        #endregion Events Handlers

        #region Private Methods

        private void InitSignInList()
        {
            cbxChangeSignIn.Items.Add("OAuth 2.0");
            cbxChangeSignIn.Items.Add("API key");
            cbxChangeSignIn.SelectedIndex = 0;
        }

        #endregion Private Methods

        #region Public Methods

        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("Forms,SignInGoogle");

            lblCaption.Text = res.Get("Caption");
            gbxSignInGoogleAPI.Text = res.Get("SignInGoogleApi");
            gbxGoogleApiKey.Text = res.Get("SignInApiKey");
            lblClientId.Text = res.Get("ClientId");
            lblClientSecret.Text = res.Get("ClientSecret");
            lblPath.Text = res.Get("PathToFile");
            lblApiKey.Text = res.Get("ApiKey");
            cbxPathToJsonFile.Text = res.Get("CheckPathToJson");
            btnSignIn.Text = res.Get("SignIn");
        }

        public override void UpdateDpiDependencies()
		{
			base.UpdateDpiDependencies();
			tbPathToJsonFile.Image = this.GetImage(1);
		}

		#endregion Public Methods
	}
}