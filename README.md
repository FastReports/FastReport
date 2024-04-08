[![Visits Badge](https://badges.pufler.dev/visits/FastReports/FastReport)](https://github.com/FastReports/FastReport) [![Created Badge](https://badges.pufler.dev/created/FastReports/FastReport)](https://github.com/FastReports/FastReport) [![Twitter Follow](https://img.shields.io/twitter/follow/fastreports?style=social)](https://twitter.com/FastReports)  [![Channel on Telegram](https://img.shields.io/badge/Channel%20on-Telegram-brightgreen.svg)](https://t.me/fastreport_open_source) [![Chat on Telegram](https://img.shields.io/badge/Chat%20on-Telegram-brightgreen.svg)](https://t.me/joinchat/hs87tfi79Rg3OGQy)

[![FastReport Open Source](https://fastreports.github.io/FastReport.Documentation/images/fros-youtube-title.jpg)](https://youtu.be/Js78gl_xAOU)

## What is FastReport?

FastReport provides free open source report generator for .NET 6/.NET Core/.NET Framework. You can use the FastReport Open Source in MVC, Web API, console applications.

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

- Using the [FastReport Online Designer](https://fast-report.com/en/product/fast-report-online-designer/).

- Using the FastReport Designer Community Edition (freeware). It can be downloaded from [FastReport releases page](https://github.com/FastReports/FastReport/releases).

[![Image of FastReport](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot3-small.png)](https://raw.githubusercontent.com/FastReports/FastReport.Documentation/master/images/FastReport-screenshot3.png)

## Exporting

FastReport Open Source can save documents in HTML, BMP, PNG, JPEG, GIF, TIFF, EMF. 

**PDF** export is available as a [plugin](https://github.com/FastReports/FastReport/tree/master/Extras/OpenSource/FastReport.OpenSource.Export.PdfSimple). You can see an example of its use [here](https://github.com/FastReports/FastReport/tree/master/Demos/OpenSource/Console%20apps/PdfExport).  If this export is not enough for you and you need a full-featured PDF export with encryption, digital signing and fonts embedding - take a look at [FastReport .NET Core](https://fast-report.com/en/product/fast-report-net/).

## Report Designer Community Edition

To edit reports, we made a special report designer build - [FastReport Designer Community Edition](https://github.com/FastReports/FastReport/releases/latest). The program is intended for use in the Windows operating system and contains all the limitations of the Open Source version. We do not supply the source code of the editor because it is part of the commercial product [FastReport .NET](https://www.fast-report.com/en/product/fast-report-net/). Publishing this program is our good will and our wish. The MIT license does not cover its source code.

## Installation

FastReport can be compiled from sources or installed from [NuGet packages](https://www.nuget.org/profiles/FastReports).

### Compilation

1. Install .NET 5 SDK for your OS from https://www.microsoft.com/net/download
2. Follow the commands

```sh
# for windows users
git clone https://github.com/FastReports/FastReport.git
cd FastReport
pack.bat
```

```sh
# for linux users
git clone https://github.com/FastReports/FastReport.git
cd FastReport
chmod 777 pack.sh && ./pack.sh
```

The package is located at `fr_packages` directory.

### NuGet

You can add FastReport to your current project via NuGet package manager:
```
Install-Package FastReport.OpenSource
Install-Package FastReport.OpenSource.Web
```

## Extras

The Extras folder contains additional modules that extend FastReport functionality:

- [Core/FastReport.Data](https://github.com/FastReports/FastReport/tree/master/Extras/Core/FastReport.Data) - connectors to various databases;
- [OpenSource/FastReport.OpenSource.Export.PdfSimple](https://github.com/FastReports/FastReport/tree/master/Extras/OpenSource/FastReport.OpenSource.Export.PdfSimple)  - simple export in PDF format;
- [ReportBuilder](https://github.com/FastReports/FastReport/tree/master/Extras/ReportBuilder) - a simple report builder from code without using templates.

## Examples

In the [Demos](https://github.com/FastReports/FastReport/tree/master/Demos) folder you can see examples of using FastReport.

## Bug Reports

See the [Issues](https://github.com/FastReports/FastReport/issues) section of website. When describing the issue, please attach screenshots or examples to help reproduce the problem.

## Contributors

This project exists because of all the people who have contributed and continue to work on the project:

[@ATZ-FR](https://github.com/ATZ-FR), [@Detrav](https://github.com/Detrav), [@fediachov](https://github.com/fediachov), [@8VAid8](https://github.com/8VAid8), 
 [@KirillKornienko](https://github.com/KirillKornienko), [@mandrookin](https://github.com/mandrookin), [@ekondur](https://github.com/ekondur), [@Gromozekaster](https://github.com/Gromozekaster), 
[@daviddesmet](https://github.com/daviddesmet), [@mjftechnology](https://github.com/mjftechnology), [@jonny-xhl](https://github.com/jonny-xhl), [@radiodeer](https://github.com/radiodeer), [@Des1re7](https://github.com/Des1re7), [@araujofrancisco](https://github.com/araujofrancisco), [@conqu1stador](https://github.com/conqu1stador), [@pietro29](https://github.com/pietro29).

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Documentation

You can read the [FastReport Open Source Documentation](https://fastreports.github.io/FastReport.Documentation/) on the github site or you can read the [documentation for the commercial product](https://www.fast-report.com/public_download/docs/FRNet/online/en/index.html), amending the [functionality limitations](https://opensource.fast-report.com/p/the-feature-comparison-table-for.html).

## License

Licensed under the MIT license. See [LICENSE.md](LICENSE.md) for details. The MIT license does not cover the FastReport Designer Community Edition.

## Resources

- [FastReport Open Source Blog with Articles and How-Tos](https://opensource.fast-report.com/)
- [The Feature Comparison Table for FastReport Open Source, FastReport Core, FastReport .NET](https://opensource.fast-report.com/p/the-feature-comparison-table-for.html "FastReport Open Source vs FastReport Core vs FastReport .NET")
- [FastReport Core Online Demo](https://www.fast-report.com:2018 "Click to view FastReport Online Demo")
- [FastReport Online Designer](https://www.fast-report.com/en/product/fast-report-online-designer/ "Click to view FastReport Online Designer Home Page")
- [Fast Reports Home Page](https://www.fast-report.com "Click for visiting the Fast Reports Home Page")


