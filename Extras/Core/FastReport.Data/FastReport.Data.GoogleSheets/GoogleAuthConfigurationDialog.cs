#if !FRCORE
using FastReport.Forms;
using FastReport.Utils;
using System;
using System.Windows.Forms;

namespace FastReport.Data
{
    /// <summary>
    /// A dialog form that allows the user to configure authentication settings
    /// (OAuth 2.0 or API Key) for connecting to Google services.
    /// </summary>
    internal partial class GoogleAuthConfigurationDialog : BaseDialogForm
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the GoogleAuthConfigurationDialog.
        /// </summary>
        public GoogleAuthConfigurationDialog()
        {
            InitializeComponent();
            Localize();
            InitAuthModes(); // sets up the authentication mode selector (OAuth/API Key)
            LoadStoredSettings(); // loads and applies previously saved settings
            UIUtils.CheckRTL(this);
            UpdateDpiDependencies();
        }

        #endregion

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
            bool useJsonFile = cbxPathToJsonFile.Checked;

            lblPathToJsonFile.Visible = useJsonFile;
            tbPathToJsonFile.Visible = useJsonFile;

            lblClientId.Visible = !useJsonFile;
            tbClientId.Visible = !useJsonFile;
            lblClientSecret.Visible = !useJsonFile;
            tbClientSecret.Visible = !useJsonFile;

            ResetData();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbClientId.Text) && string.IsNullOrEmpty(tbClientSecret.Text)
                && string.IsNullOrEmpty(tbPathToJsonFile.Text)
                && string.IsNullOrEmpty(tbApiKey.Text))
            {
                MessageBox.Show(Res.Get("ConnectionEditors,GoogleSheets,SignInGoogle,NullData"));
            }
            else
            {
                XmlItem storageSettings = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");

                storageSettings.SetProp("ClientId", Crypter.EncryptString(tbClientId.Text));
                storageSettings.SetProp("ClientSecret", Crypter.EncryptString(tbClientSecret.Text));
                storageSettings.SetProp("PathToJson", tbPathToJsonFile.Text); // path is not a secret, no need to encrypt
                storageSettings.SetProp("ApiKey", Crypter.EncryptString(tbApiKey.Text));

                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void cbxAuthMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool isOAuth = (cbxAuthMode.SelectedIndex == 0);

            gbxOAuth.Visible = isOAuth;
            gbxOAuth.Enabled = isOAuth;

            gbxApiKey.Visible = !isOAuth;
            gbxApiKey.Enabled = !isOAuth;

            ResetData();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads saved authentication settings from the configuration and updates the UI.
        /// Determines the authentication mode based on the saved data and populates the relevant fields.
        /// </summary>
        private void LoadStoredSettings()
        {
            XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
            if (xi == null)
                return;
         
            string clientId = Crypter.DecryptString(xi.GetProp("ClientId"));
            string clientSecret = Crypter.DecryptString(xi.GetProp("ClientSecret"));
            string pathToJson = xi.GetProp("PathToJson");
            string apiKey = Crypter.DecryptString(xi.GetProp("ApiKey"));

            if (!string.IsNullOrEmpty(apiKey))
            {
                // API Key mode was used
                cbxAuthMode.SelectedIndex = 1;
                tbApiKey.Text = apiKey;
            }
            else
            {
                // OAuth 2.0 mode was used
                cbxAuthMode.SelectedIndex = 0;
                if (!string.IsNullOrEmpty(pathToJson))
                {
                    // JSON file path was used
                    cbxPathToJsonFile.Checked = true;
                    tbPathToJsonFile.Text = pathToJson;
                }
                else
                {
                    // Client ID/ Secret were used
                    cbxPathToJsonFile.Checked = false;
                    tbClientId.Text = clientId;
                    tbClientSecret.Text = clientSecret;
                }
            }
        }

        /// <summary>
        /// Initializes the authentication mode selector with available options (OAuth 2.0, API key).
        /// </summary>
        private void InitAuthModes()
        {
            cbxAuthMode.Items.Add("OAuth 2.0");
            cbxAuthMode.Items.Add("API key");
            cbxAuthMode.SelectedIndex = 0;
        }

        /// <summary>
        /// Clears input fields that are hidden when switching authentication modes.
        /// </summary>
        private void ResetData()
        {
            if (cbxAuthMode.SelectedIndex == 0) // OAuth mode
            {
                tbApiKey.Text = "";

                if (cbxPathToJsonFile.Checked)
                {
                    tbClientId.Text = "";
                    tbClientSecret.Text = "";
                }
                else
                {
                    tbPathToJsonFile.Text = "";
                }
            }
            else // API Key mode
            {
                tbClientId.Text = "";
                tbClientSecret.Text = "";
                tbPathToJsonFile.Text = "";
            }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public override void Localize()
        {
            base.Localize();
            MyRes res = new MyRes("ConnectionEditors,GoogleSheets,SignInGoogle");

            Text = res.Get("");
            lblCaption.Text = res.Get("Caption");
            lblClientId.Text = res.Get("ClientId");
            lblClientSecret.Text = res.Get("ClientSecret");
            lblPathToJsonFile.Text = res.Get("PathToFile");
            lblApiKey.Text = res.Get("ApiKey");
            cbxPathToJsonFile.Text = res.Get("CheckPathToJson");
        }

        /// <inheritdoc/>
        public override void UpdateDpiDependencies()
        {
            base.UpdateDpiDependencies();
            tbPathToJsonFile.Image = this.GetImage(1);
        }

        #endregion
    }
}
#endif