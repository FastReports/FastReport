How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(GoogleSheetsDataConnection));
- you should now be able to create a new Google Sheets data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
GoogleSheetsDataConnection conn = new GoogleSheetsDataConnection();
conn.ConnectionString = "";
conn.Sheets = "https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit#gid=0";
conn.CreateAllTables(); 
report.Dictionary.Connections.Add(conn);