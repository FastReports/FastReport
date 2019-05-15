How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(FirebirdDataConnection));
- now you should be able to create a new Firebird data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
FirebirdDataConnection conn = new FirebirdDataConnection ();
conn.ConnectionString = "connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);