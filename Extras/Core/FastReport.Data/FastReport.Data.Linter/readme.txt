How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(LinterDataConnection));
- now you should be able to create a new Linter data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
LinterDataConnection conn = new LinterDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);