How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(CassandraDataConnection));
- now you should be able to create a new Excel data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
ExcelDataConnection conn = new CassandraDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);