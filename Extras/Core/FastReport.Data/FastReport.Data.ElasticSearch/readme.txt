How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(ESDataSourceConnection));
- now you should be able to create a new ElasticSearch data source from Designer (.Net 4) or from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
ESDataSourceConnection conn = new ESDataSourceConnection();
conn.ConnectionString = "your connection string";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);