using FastReport.Utils;

namespace FastReport.Data
{
    /// <summary>
    /// Handles loading and clearing of Google Sheets credentials from the application's configuration.
    /// </summary>
    public class GoogleSheetsConfigLoader : IGoogleSheetsConfigLoader
    {
        ///<inheritdoc/>
        public GoogleSheetsCredentials Load()
        {
            XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
            return new GoogleSheetsCredentials(
                clientId: Crypter.DecryptString(xi.GetProp("ClientId")),
                clientSecret: Crypter.DecryptString(xi.GetProp("ClientSecret")),
                pathToJson: xi.GetProp("PathToJson"),
                apiKey: Crypter.DecryptString(xi.GetProp("ApiKey"))
                );
        }

        ///<inheritdoc/>
        public void Clear()
        {
            XmlItem xi = Config.Root.FindItem("GoogleSheets").FindItem("StorageSettings");
            xi.SetProp("ClientId", "");
            xi.SetProp("ClientSecret", "");
            xi.SetProp("PathToJson", "");
            xi.SetProp("ApiKey", "");
        }
    }
}