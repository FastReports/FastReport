using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System.Drawing;
using System.Collections.Generic;
using System.IO;

namespace FastReport.ReportBuilder.UnitTest
{
    [TestClass]
    public class ReportTest
    {
        List<Person> list;
        ReportBuilder builder;
        public ReportTest()
        {
            list = new Person().GetData();
            builder = new ReportBuilder();
            // there are no assertions on test method, test it on created file at \bin\Debug\Empleyee.pdf path
        }

        [TestMethod]
        public void export_basic_report_pdf()
        {
            var report = builder.Report(list)
                .ReportTitle(title => title
                    .Text("Employee List")
                    .HorzAlign(HorzAlign.Center)
                 )
                .Data(data =>
                {
                    data.Column(col => col.FirstName); // this column get title with data annotation
                    data.Column(col => col.LastName).Title("Last Name");
                    data.Column(col => col.BirthDate);
                    data.Column(col => col.IsActive);
                    data.Column(col => col.Level);
                })
                .Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "basic_report.pdf");
        }

        [TestMethod]
        public void export_report_resized_columns_by_percentage_pdf()
        {
            var report = builder.Report(list)
                .ReportTitle(title => title
                    .Text("Employee List")
                    .HorzAlign(HorzAlign.Center)
                 )
                .Data(data =>
                {
                    data.Column(col => col.FirstName).Width(25); // 25%
                    data.Column(col => col.LastName).Width(20); // 20%
                    data.Column(col => col.BirthDate); // other three columns are equal - (55/3)%
                    data.Column(col => col.IsActive);
                    data.Column(col => col.Level);
                })
                .Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "report_resized_columns_by_percentage.pdf");
        }

        [TestMethod]
        public void export_report_using_column_format_pdf()
        {
            var report = builder.Report(list)
                .ReportTitle(title => title
                    .Text("Employee List")
                    .HorzAlign(HorzAlign.Center)
                 )
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                    data.Column(col => col.BirthDate).Format("MM/dd/yyyy");
                    data.Column(col => col.IsActive);
                    data.Column(col => col.Level); // this column formatted with data annotation
                })
                .Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "report_using_column_format.pdf");
        }

        [TestMethod]
        public void export_report_using_expression_pdf()
        {
            var report = builder.Report(list)
                .ReportTitle(title => title
                    .Text("Employee List - [MonthName(Month([Date]))]")
                    .HorzAlign(HorzAlign.Center)
                 )
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName).Expression("UpperCase({0})");
                    data.Column(col => col.BirthDate);
                    data.Column(col => col.IsActive);
                    data.Column(col => col.Level);
                })
                .Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "report_using_expression.pdf");
        }

        [TestMethod]
        public void export_group_report_pdf()
        {
            var report = builder.Report(list)
                .ReportTitle(title => title
                    .Text("Employee List - [MonthName(Month([Date]))]")
                    .HorzAlign(HorzAlign.Center)
                 )
                .GroupHeader(header => header
                    .Condition(con => con.LastName)
                    .SortOrder(SortOrder.Descending)
                    .Expression("Substring({0},0,1)")
                 )
                .DataHeader(header => header
                    .TextColor(Color.Brown)
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

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "group_report.pdf");
        }

        [TestMethod]
        public void prepare_report_with_page_configuration_and_extra_bands()
        {
            var report = builder.Report(list)
                .ReportTitle(t => t
                    .Text("Employee List")
                    .HorzAlign(HorzAlign.Center)
                 )
                .Landscape()
                .PaperSize(210, 297)
                .Margins(15, 20, 15, 20)
                .PageHeader(header => header
                    .Text("Employees")
                    .HorzAlign(HorzAlign.Center)
                    .Height(0.7f))
                .DataHeader(header => header
                    .TextColor(Color.DarkBlue)
                    .FillColor(Color.LightGray))
                .PageFooter(footer => footer
                    .Text("Page [Page#] of [TotalPages#]")
                    .HorzAlign(HorzAlign.Right)
                    .Height(0.7f))
                .ReportSummary(summary => summary
                    .Text("Report completed")
                    .Height(0.7f))
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                })
                .Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();
            pdfExport.Export(report, "report_with_page_configuration_and_extra_bands.pdf");

            var page = (ReportPage)report.Pages[0];
            var headerText = (TextObject)page.PageHeader.Objects[0];
            var footerText = (TextObject)page.PageFooter.Objects[0];
            var summaryText = (TextObject)page.ReportSummary.Objects[0];

            Assert.IsTrue(page.Landscape);
            Assert.AreEqual(210f, page.PaperWidth, 0.001f);
            Assert.AreEqual(297f, page.PaperHeight, 0.001f);
            Assert.AreEqual(15f, page.LeftMargin, 0.001f);
            Assert.AreEqual(20f, page.TopMargin, 0.001f);
            Assert.AreEqual(15f, page.RightMargin, 0.001f);
            Assert.AreEqual(20f, page.BottomMargin, 0.001f);
            Assert.AreEqual("Employees", headerText.Text);
            Assert.AreEqual(HorzAlign.Center, headerText.HorzAlign);
            Assert.AreEqual("Page [Page#] of [TotalPages#]", footerText.Text);
            Assert.AreEqual(HorzAlign.Right, footerText.HorzAlign);
            Assert.AreEqual("Report completed", summaryText.Text);
            Assert.IsTrue(File.Exists("report_with_page_configuration_and_extra_bands.pdf"));
        }

        [TestMethod]
        public void prepare_report_title_horz_align_is_applied()
        {
            var report = builder.Report(list)
                .ReportTitle(t => t
                    .Text("Employee List")
                    .HorzAlign(HorzAlign.Right)
                )
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                })
                .Prepare();

            var page = (ReportPage)report.Pages[0];
            var titleText = (TextObject)page.ReportTitle.Objects[0];

            Assert.AreEqual(HorzAlign.Right, titleText.HorzAlign);
        }

        [TestMethod]
        public void prepare_report_title_height_is_applied()
        {
            var report = builder.Report(list)
                .ReportTitle(t => t
                    .Text("Employee List")
                    .Height(2.0f)
                )
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                })
                .Prepare();

            var page = (ReportPage)report.Pages[0];

            Assert.AreEqual(Units.Centimeters * 2.0f, page.ReportTitle.Height, 0.001f);
        }

        [TestMethod]
        public void prepare_data_header_fill_and_text_color_are_applied()
        {
            var report = builder.Report(list)
                .DataHeader(header => header
                    .FillColor(Color.LightBlue)
                    .TextColor(Color.DarkRed)
                )
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                })
                .Prepare();

            var page = (ReportPage)report.Pages[0];
            var dataBand = (DataBand)page.Bands[0];
            var dataHeader = (DataHeaderBand)dataBand.Header;
            var headerCell = (TextObject)dataHeader.Objects[0];

            Assert.AreEqual(Color.LightBlue, headerCell.FillColor);
            Assert.AreEqual(Color.DarkRed, headerCell.TextColor);
        }

        [TestMethod]
        public void prepare_report_level_horz_align_propagates_to_data_header_cells()
        {
            var report = builder.Report(list)
                .HorzAlign(HorzAlign.Center)
                .Data(data =>
                {
                    data.Column(col => col.FirstName);
                    data.Column(col => col.LastName);
                })
                .Prepare();

            var page = (ReportPage)report.Pages[0];
            var dataBand = (DataBand)page.Bands[0];
            var dataHeader = (DataHeaderBand)dataBand.Header;
            var headerCell = (TextObject)dataHeader.Objects[0];

            Assert.AreEqual(HorzAlign.Center, headerCell.HorzAlign);
        }
    }
}
