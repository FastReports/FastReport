How to use it:

- execute the following code once at the application start:

FastReport.Utils.RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

- now you should be able to create a new PostgreSQL data source from Designer (.NET4) or from code:

Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
PostgresDataConnection conn = new PostgresDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);