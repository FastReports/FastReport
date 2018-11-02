How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(CouchbaseDataConnection));
- now you should be able to create a new Couchbase data source from Designer (.Net 4) or from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
CouchbaseDataConnection con = new CouchbaseDataConnection();
con.Host = "http://127.0.0.1:8091";
con.DatabaseName = "beer-sample";
con.Username = "username";
con.Password = "userPass";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);