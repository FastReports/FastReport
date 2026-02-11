namespace FastReport.Data
{
    /// <summary>
    /// Defines a manager responsible for handling the UI aspects of the Google Sheets login process.
    /// </summary>
    public interface IGoogleSheetsLoginUIManager
    {
        /// <summary>
        /// Checks if the provided credentials are sufficient for authentication and, if not, displays a configuration dialog to the user.
        /// </summary>
        /// <param name="credentials">The current Google Sheets credentials to check.</param>
        /// <returns>The loaded credentials from the configuration after potentially showing the dialog.</returns>
        GoogleSheetsCredentials ShowLoginDialogIfNeeded(GoogleSheetsCredentials credentials);
    }
}
