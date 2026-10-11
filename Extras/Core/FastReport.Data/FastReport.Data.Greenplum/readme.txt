How to use it:

- execute the following code once at the application start:

FastReport.Utils.RegisteredObjects.AddConnection(typeof(GreenplumDataConnection));

- now you should be able to create a new Greenplum (PostgreSQL) data source from Designer (.NET4) or from code:

Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
GreenplumDataConnection conn = new GreenplumDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);