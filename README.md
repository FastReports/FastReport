[![Gitter](https://badges.gitter.im/FastReports/FastReport.svg)](https://gitter.im/FastReports/FastReport?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge) [![Average time to resolve an issue](http://isitmaintained.com/badge/resolution/FastReports/FastReport.svg)](http://isitmaintained.com/project/FastReports/FastReport "Average time to resolve an issue") [![Percentage of issues still open](http://isitmaintained.com/badge/open/FastReports/FastReport.svg)](http://isitmaintained.com/project/FastReports/FastReport "Percentage of issues still open")

[![FastReport Open Source](https://fastreports.github.io/FastReport.Documentation/images/fros-youtube-title.jpg)](https://youtu.be/Js78gl_xAOU)

## What is FastReport?

FastReport provides free open source report generator for .NET Core/.NET Framework. You can use the FastReport Open Source in MVC, Web API applications.

[![Image of FastReport](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot2-small.png)](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot2.png)

## Features

FastReport is written in C# and it is compatible with .NET Standard 2.0 and higher. Extendable FastReport architecture allows creating your own objects, export filters, wizards and DB engines.

[![Image of FastReport](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot1-small.png)](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot1.png)

### Report Objects

- FastReport is a band-oriented report generator. There are 13 types of bands available: Report Title, Report Summary, Page Header, Page Footer, Column Header, Column Footer, Data Header, Data, Data Footer, Group Header, Group Footer, Child and Overlay. In addition, sub-reports are fully supported. 

- A wide range of band types allows creating any kind of report: list, master-detail, group, multi-column, master-detail-detail and many more.

- Wide range of available report objects : text, picture, line, shape, barcode, matrix, table, checkbox.

- Reports can consist of several design pages, which allows reports to contain a cover, the data and a back cover, all in one file.

- The Table object allows building a tabular report with variable number of rows and/or columns, just like in MS Excel. Aggregate functions are also available.

- Powerful, fully configurable Matrix object that can be used to print pivot tables.

- Report inheritance. For creating many reports with common elements such as titles, logos or footers you can place all the common elements in a base report and inherit all other reports from this base.

### Data Sources

- You can get data from XML, CSV, Json, MS SQL, MySql, Oracle, Postgres, MongoDB, Couchbase, RavenDB, SQLite.

- FastReport has ability to get data from business objects of IEnumerable type. 

- Report can contain data sources (tables, queries, DB connections). 

- Thus you can not only use application-defined datasets but also connect to any database and use tables and queries directly within the report.

### Internal Scripting

FastReport has a built-in script engine that supports two .NET languages, C# and VB.NET. You can use all of the .NET power in your reports to perform complex data handling and much more.

## Working with report templates

You can make a report template in several ways:

- Creating report from code.

- Developing report template as XML file.

- Using the FastReport Online Designer.

- Using the FastReport Designer Community Edition (freeware). It can be downloaded from [FastReport releases page](https://github.com/FastReports/FastReport/releases).

[![Image of FastReport](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot3-small.png)](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot3.png)

## Exporting

FastReport Open Source can save documents in HTML, BMP, PNG, JPEG, GIF, TIFF, EMF. PDF Export is available as a plugin.

## Installation

FastReport can be compiled from sources or installed from [NuGet packages](https://www.nuget.org/profiles/FastReports).

### Compilation

1. Install latest .NET Core SDK for your OS from https://www.microsoft.com/net/download
2. Follow the commands

```sh
# for windows users
git clone https://github.com/FastReports/FastReport.git
cd FastReport
Tools\pack.bat
```

```sh
# for linux users
git clone https://github.com/FastReports/FastReport.git
cd FastReport
chmod 777 Tools/pack.sh && ./Tools/pack.sh
```

The package is located at `fr_nuget` directory.

### NuGet

You can add FastReport to your current project via NuGet package manager:
```
Install-Package FastReport.OpenSource
Install-Package FastReport.OpenSource.Web
```

## Examples

In the [Demos](https://github.com/FastReports/FastReport/tree/master/Demos) folder you can see examples of using FastReport.

## Bug Report

See the [Issues](https://github.com/FastReports/FastReport/issues) section of website.

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## License

Licensed under the MIT license. See [LICENSE.md](LICENSE.md) for details.

## Resources

[FastReport Open Source Home](https://github.com/FastReports/FastReport "Click for visiting the FastReport Open Source GitHub")

[FastReport Open Source Documentation](https://fastreports.github.io/FastReport.Documentation/)

[FastReport Open Source Blog with Articles and How-Tos](https://opensource.fast-report.com/)

[The Feature Comparison Table for FastReport Open Source, FastReport Core, FastReport .NET](https://opensource.fast-report.com/p/the-feature-comparison-table-for.html "FastReport Open Source vs FastReport Core vs FastReport .NET")

[FastReport Core Online Demo](https://www.fast-report.com:2018 "Click to view FastReport Online Demo")

[FastReport Online Designer](https://www.fast-report.com/en/product/fast-report-online-designer/ "Click to view FastReport Online Designer Home Page")

[Fast Reports Home Page](https://www.fast-report.com "Click for visiting the Fast Reports Home Page")

