How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(OdbcDataConnection));
- now you should be able to create a new Odbc data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
OdbcDataConnection conn = new OdbcDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);