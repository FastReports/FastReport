How to use it:
- execute the following code once at the application start:
FastReport.Utils.RegisteredObjects.AddConnection(typeof(ExcelDataConnection));
- now you should be able to create a new Excel data connection from code:
Report report = new Report(); 
report.Load(@"YourReport.frx");
//... 
ExcelDataConnection conn = new ExcelDataConnection();
conn.ConnectionString = "path to Excel file";
conn.CreateAllTables();
report.Dictionary.Connections.Add(conn);