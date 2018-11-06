How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(RavenDBDataConnection));
- now you should be able to create a new RavenDB data source from Designer (.Net 4) or from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
RavenDBDataConnection conn = new RavenDBDataConnection();
conn.Host = "https://a.ravenhost.development.run:80";
conn.DatabaseName = "SampleDatabase";
conn.CertificatePath = @"C:\certificate.pfx";
conn.Password = "password";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);