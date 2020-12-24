using Microsoft.VisualStudio.TestTools.UnitTesting;
using FastReport.Export.PdfSimple;
using System.Drawing;
using System.Collections.Generic;

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
            pdfExport.Export(report, "Employees.pdf");
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
            pdfExport.Export(report, "Employees.pdf");
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
            pdfExport.Export(report, "Employees.pdf");
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
            pdfExport.Export(report, "Employees.pdf");
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
            pdfExport.Export(report, "Employees.pdf");
        }
    }
}
