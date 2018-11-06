How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(OracleDataConnection));
- now you should be able to create a new Oracle data source:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
OracleDataConnection conn = new OracleDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);
- or reassign report connection string:
report.Dictionary.Connections[0].ConnectionString = "your connection string";