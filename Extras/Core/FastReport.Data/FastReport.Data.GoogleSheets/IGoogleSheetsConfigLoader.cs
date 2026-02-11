namespace FastReport.Data
{
    /// <summary>
    /// Defines a contract for loading and clearing Google Sheets credentials from the configuration.
    /// </summary>
    public interface IGoogleSheetsConfigLoader
    {
        /// <summary>
        /// Loads and decrypts the Google Sheets credentials from the configuration.
        /// </summary>
        /// <returns>A <see cref="GoogleSheetsCredentials"/> object with the loaded credentials.</returns>
        GoogleSheetsCredentials Load();

        /// <summary>
        /// Clears all stored Google Sheets credentials from the configuration.
        /// </summary>
        void Clear();
    }
}