#if !FRCORE
using System;
using System.Windows.Forms;
namespace FastReport.Data
{
    /// <summary>
    /// Manages the user interface for Google Sheets authentication configuration.
    /// </summary>
    public class GoogleSheetsLoginUIManager : IGoogleSheetsLoginUIManager
    {
        /// <inheritdoc/>
        public GoogleSheetsCredentials ShowLoginDialogIfNeeded(GoogleSheetsCredentials credentials)
        {
            if (!String.IsNullOrEmpty(credentials.ClientId) && !String.IsNullOrEmpty(credentials.ClientSecret)
                || !String.IsNullOrEmpty(credentials.PathToJson)
                || !String.IsNullOrEmpty(credentials.ApiKey))
            {
                return credentials;
            }
            else
            {
                using (GoogleAuthConfigurationDialog authDialog = new GoogleAuthConfigurationDialog())
                {
                    var result = authDialog.ShowDialog();

                    if (result != DialogResult.OK)
                    {
                        return null;
                    }
                }
                // after the user closes the dialog, reloads the newly saved
                // credentials from the configuration and returns them
                return new GoogleSheetsConfigLoader().Load();
            }
        }
    }
}
#endif