# Changelog

## 2020.3.0 - May 20, 2019
- added new type of QR code, Swiss QR Code
- added property MatrixObject.PrintIfEmpty, which allows displaying the matrix even if it is empty
- added property Page.LastPageSource, which allows to configure the printer tray for printing the last page of the report
- added VisibleExpression, PrintableExpression, and ExportableExpression properties (these properties allow to set the value of the Visible, Printable, and Exportable properties, depending on the fulfillment of the specified condition)
- fixed a bug with text object visibility when Highlight.Visible parameter is enabled
- fixed a bug with work of property PrintableExpression
- optimized saving of embedded fonts in PDF-export. File size has decreased significantly.
- fixed a bug with resource loading in WebReport
- fixed a bug with image scaling in WebReport
- fixed a bug with timeout exception while saving a report in the Online Designer
- fixed a bug with calls of WebReport.ExportPdf, WebReport.ExportCsv, WebReport.ExportRtf, etc.
- fixed compilation error on Unix-systems (because of net40 in targetFrameworks)
- fixed some API errors in report code, available in System.Drawing.Primitives

## 2020.1.0 - Dec 2, 2019
- added an ability to change decimal digits for Number, Currency and Percent formats when UseLocale property is true
- added a property "SplitRows" for MatrixObject. By default, its value is False and in this case rows with the same vaues are joined. If True - rows are split (like TableObject)
- added an ability to change export settings. To do this, you need to subscribe to the ExportParameters event in WebReport.Report
- fixed a bug with trying to convert DBNull in empty string when ConvertNulls is disabled
- fixed a bug when PageFooter with PrintOn=LastPage causes to print it on penultimate page
- fixed view of background on BarcodeObject at Html export
- fixed incorrect width and height for reports with mixed page orientation (Landscape & Portrait)
- fixed incorrect view of background in ShapeObject
- fixed lack of non-standard fill (Hatch, LinearGradient, etc.) on ShapeObject
- fixed a bug with SQLite plugin if database includes null-values
- known issue: Save button in Preview does not respond to click.

## 2019.4.0 - Sep 10, 2019
- graphics dependency changed from CoreCompat.System.Drawing to System.Drawing.Common
- added a new Json data connection integrated into the engine
- added RepeatBandNTimes property for bands
- added Bezier curve for polygons
- added a new time format minutes:seconds [mm:ss]
- fixed TypeConverter on the TextObject.ParagraphFormat property
- fixed a bug with breaking ManualBuild table with CanBreak = false
- fixed a bug with changing the GroupHeaderBand hierarchy if it had a child GroupHeaderBand
- fixed a bug when font changed in parent report were not changed in inherited report
- fixed image size calculation when AutoSize is enabled in the preparation stage
- fixed a bug with display on the penultimate page with PageFooter PrintOn = LastPage

## 2019.3.0 - May 15, 2019
- added a CoreCompat.System.Drawing reference to the script, which allows using Color, Font and some other features of System.Drawing
- added ImageAlign property for image alignment inside PictureObject; by default, alignment is disabled
- fixed a bug when in some cases the TypeConverter`s were not loaded correctly
- fixed a bug with infinite loop in AdvancedTextRenderer when WordWrap is true and width of object less than width of one character
- fixed a bug causing memory leak in HtmlTextRenderer
- added links for images to HTML-export

## 2019.2.7 - Mar 28, 2019
- added links for images to HTML-export
- fixed a bug when in some cases the TypeConverter`s were not loaded correctly
- fixed bug with infinite loop in AdvancedTextRenderer when WordWrap is true and width of object less than width of one character
- added a CoreCompat.System.Drawing reference to the script, which allows using Color, Font and some other features of System.Drawing

## 2019.2.0 - Mar 5, 2019
- sync with FastReport .NET 2019.2

## 2019.1.0 - Dec 19, 2018
- sync with FastReport .NET 2019.1

## 2018.4.16 - Nov 30, 2018
- Changed the build method, now the packages are assembled with all the necessary native libraries

## 2018.4.15 - Nov 27, 2018
- fixed bug with Roslyn wrapper, when a warning is raised as error

## 2018.4.14 - Nov 26, 2018
- In this release we have added the main connectors

## 2018.4.9 - Nov 2, 2018
- sync with FastReport 2018.4.9 changes
- disabled "new dialog page" button
- added script for packing on linux systems
- updated license in nuget packages
- added PostgreSQL connector

## 2018.4.7 - Oct 25, 2018
- Initial Release
