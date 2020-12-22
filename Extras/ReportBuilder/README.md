# FastReport.ReportBuilder
Building a FastReport without designer easy way!
Author: [Emrah KONDUR](https://github.com/ekondur)
```csharp
var builder = new ReportBuilder();

var report = builder.Report(list)
.ReportTitle(title => title
    .Text("Person Report - [MonthName(Month([Date]))]")
    .HorzAlign(HorzAlign.Center)
 )
.GroupHeader(header => header
    .Condition(con => con.LastName)
    .SortOrder(SortOrder.Descending)
    .Expression("Substring({0},0,1)")
 )
.DataHeader(header => header
    .TextColor(Color.Red)
    .Font("Helvetica")
 )
.Data(data =>
{
    data.Column(col => col.FirstName).Width(20);
    data.Column(col => col.LastName).Expression("UpperCase({0})");
    data.Column(col => col.BirthDate).Format("MM/dd/yyyy");
    data.Column(col => col.IsActive).Title("Active").Width(10);
    data.Column(col => col.Level).HorzAlign(HorzAlign.Center);
})
.Prepare();
```

### Where can I get it?
Install [FastReport.ReportBuilder](https://www.nuget.org/packages/FastReport.ReportBuilder/) from the package manager console:

```
PM> Install-Package FastReport.ReportBuilder
```