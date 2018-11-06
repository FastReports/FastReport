How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(MongoDBDataConnection));
- now you should be able to create a new MongoDB data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
MongoDBDataConnection conn = new MongoDBDataConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);