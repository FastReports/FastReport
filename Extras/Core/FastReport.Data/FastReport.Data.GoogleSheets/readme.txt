How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(GoogleSheetsDataConnection));
- you should now be able to create a new Google Sheets data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
// EXAMPLE 1: OAuth2 authentication with ClientId and ClientSecret
string clientId = "your_client_id";
string clientSecret = "your_client_secret";
string spreadsheetId = "https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit#gid=0";
// or
// string spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";

var creds1 = new GoogleSheetsCredentials(clientId, clientSecret, null, null);
var connection1 = new GoogleSheetsDataConnection
{
    Credentials = creds1,
    SpreadsheetId = spreadsheetId
};

// EXAMPLE 2: Authentication with JSON credentials file
string jsonPath = @"path\to\your\credentials.json";

var creds2 = new GoogleSheetsCredentials(null, null, jsonPath, null);
var connection2 = new GoogleSheetsDataConnection
{
    Credentials = creds2,
    SpreadsheetId = spreadsheetId
};

// EXAMPLE 3: API Key authentication
string apiKey = "your_api_key";

var creds3 = new GoogleSheetsCredentials(null, null, null, apiKey);
var connection3 = new GoogleSheetsDataConnection
{
    Credentials = creds3,
    SpreadsheetId = spreadsheetId
};

// Example usage of one of the options (choose the one you need)
connection1.CreateAllTables();
report.Dictionary.Connections.Add(connection1); // Select the required authentication method