How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(JsonDataConnection));
- now you should be able to create a new Json data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
JsonDataConnection conn = new JsonDataConnection();
conn.ConnectionString = "path to JSON file";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);