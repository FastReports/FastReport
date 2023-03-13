using System;
using System.Data;
using System.Drawing;
using System.IO;
using ConsoleAppsUtils;
using FastReport;
using FastReport.Data;
using FastReport.Export.Image;
using FastReport.Format;
using FastReport.Table;
using FastReport.Utils;

namespace DataFromDataSet
{
    class Program
    {
        private static readonly string outFolder;
        private static readonly string inFolder;
        private static readonly DataSet dataSet = new DataSet();
        static Program()
        {
            inFolder = Utils.FindDirectory("in");
            outFolder = Path.Combine(Directory.GetParent(inFolder).FullName, "out");

            // load nwind database
            dataSet.ReadXml(Path.Combine(Utils.FindDirectory("Reports"), "nwind.xml"));
        }

        #region Reports creating

        static Report GetSimpleListReport()
        {
            Report report = new Report();

            // register all data tables and relations
            report.RegisterData(dataSet);

            // enable the "Employees" table to use it in the report
            report.GetDataSource("Employees").Enabled = true;

            // add report page
            ReportPage page = new ReportPage();
            report.Pages.Add(page);
            // always give names to objects you create. You can use CreateUniqueName method to do this;
            // call it after the object is added to a report.
            page.CreateUniqueName();

            // create title band
            page.ReportTitle = new ReportTitleBand();
            // native FastReport unit is screen pixel, use conversion 
            page.ReportTitle.Height = Units.Centimeters * 1;
            page.ReportTitle.CreateUniqueName();

            // create title text
            TextObject titleText = new TextObject();
            titleText.Parent = page.ReportTitle;
            titleText.CreateUniqueName();
            titleText.Bounds = new RectangleF(Units.Centimeters * 5, 0, Units.Centimeters * 10, Units.Centimeters * 1);
            titleText.Font = new Font("Arial", 14, FontStyle.Bold);
            titleText.Text = "Employees";
            titleText.HorzAlign = HorzAlign.Center;

            // create data band
            DataBand dataBand = new DataBand();
            page.Bands.Add(dataBand);
            dataBand.CreateUniqueName();
            dataBand.DataSource = report.GetDataSource("Employees");
            dataBand.Height = Units.Centimeters * 0.5f;

            // create two text objects with employee's name and birth date
            TextObject empNameText = new TextObject();
            empNameText.Parent = dataBand;
            empNameText.CreateUniqueName();
            empNameText.Bounds = new RectangleF(0, 0, Units.Centimeters * 5, Units.Centimeters * 0.5f);
            empNameText.Text = "[Employees.FirstName] [Employees.LastName]";

            TextObject empBirthDateText = new TextObject();
            empBirthDateText.Parent = dataBand;
            empBirthDateText.CreateUniqueName();
            empBirthDateText.Bounds = new RectangleF(Units.Centimeters * 5.5f, 0, Units.Centimeters * 3, Units.Centimeters * 0.5f);
            empBirthDateText.Text = "[Employees.BirthDate]";
            // format value as date
            DateFormat format = new DateFormat();
            format.Format = "MM/dd/yyyy";
            empBirthDateText.Format = format;

            return report;
        }

        static Report GetMasterDetailReport()
        {
            Report report = new Report();

            // register all data tables and relations
            report.RegisterData(dataSet);

            // enable the "Categories" and "Products" tables to use it in the report
            report.GetDataSource("Categories").Enabled = true;
            report.GetDataSource("Products").Enabled = true;
            // enable relation between two tables
            report.Dictionary.UpdateRelations();

            // add report page
            ReportPage page = new ReportPage();
            report.Pages.Add(page);
            // always give names to objects you create. You can use CreateUniqueName method to do this;
            // call it after the object is added to a report.
            page.CreateUniqueName();

            // create master data band
            DataBand masterDataBand = new DataBand();
            page.Bands.Add(masterDataBand);
            masterDataBand.CreateUniqueName();
            masterDataBand.DataSource = report.GetDataSource("Categories");
            masterDataBand.Height = Units.Centimeters * 0.5f;

            // create category name text
            TextObject categoryText = new TextObject();
            categoryText.Parent = masterDataBand;
            categoryText.CreateUniqueName();
            categoryText.Bounds = new RectangleF(0, 0, Units.Centimeters * 5, Units.Centimeters * 0.5f);
            categoryText.Font = new Font("Arial", 10, FontStyle.Bold);
            categoryText.Text = "[Categories.CategoryName]";

            // create detail data band
            DataBand detailDataBand = new DataBand();
            masterDataBand.Bands.Add(detailDataBand);
            detailDataBand.CreateUniqueName();
            detailDataBand.DataSource = report.GetDataSource("Products");
            detailDataBand.Height = Units.Centimeters * 0.5f;
            // set sort by product name
            detailDataBand.Sort.Add(new Sort("[Products.ProductName]"));

            // create product name text
            TextObject productText = new TextObject();
            productText.Parent = detailDataBand;
            productText.CreateUniqueName();
            productText.Bounds = new RectangleF(Units.Centimeters * 1, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
            productText.Text = "[Products.ProductName]";

            return report;
        }

        static Report GetGroupReport()
        {
            Report report = new Report();

            // register all data tables and relations
            report.RegisterData(dataSet);

            // enable the "Products" table to use it in the report
            report.GetDataSource("Products").Enabled = true;

            // add report page
            ReportPage page = new ReportPage();
            report.Pages.Add(page);
            // always give names to objects you create. You can use CreateUniqueName method to do this;
            // call it after the object is added to a report.
            page.CreateUniqueName();

            // create group header
            GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
            page.Bands.Add(groupHeaderBand);
            groupHeaderBand.Height = Units.Centimeters * 1;
            groupHeaderBand.Condition = "[Products.ProductName].Substring(0,1)";
            groupHeaderBand.SortOrder = FastReport.SortOrder.Ascending;

            // create group text
            TextObject groupText = new TextObject();
            groupText.Parent = groupHeaderBand;
            groupText.CreateUniqueName();
            groupText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 1);
            groupText.Font = new Font("Arial", 14, FontStyle.Bold);
            groupText.Text = "[[Products.ProductName].Substring(0,1)]";
            groupText.VertAlign = VertAlign.Center;
            groupText.Fill = new LinearGradientFill(Color.OldLace, Color.Moccasin, 90, 0.5f, 1);

            // create data band
            DataBand dataBand = new DataBand();
            groupHeaderBand.Data = dataBand;
            dataBand.CreateUniqueName();
            dataBand.DataSource = report.GetDataSource("Products");
            dataBand.Height = Units.Centimeters * 0.5f;

            // create product name text
            TextObject productText = new TextObject();
            productText.Parent = dataBand;
            productText.CreateUniqueName();
            productText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
            productText.Text = "[Products.ProductName]";

            // create group footer
            groupHeaderBand.GroupFooter = new GroupFooterBand();
            groupHeaderBand.GroupFooter.CreateUniqueName();
            groupHeaderBand.GroupFooter.Height = Units.Centimeters * 1;

            // create total
            Total groupTotal = new Total();
            groupTotal.Name = "TotalRows";
            groupTotal.TotalType = TotalType.Count;
            groupTotal.Evaluator = dataBand;
            groupTotal.PrintOn = groupHeaderBand.GroupFooter;
            report.Dictionary.Totals.Add(groupTotal);

            // show total in the group footer
            TextObject totalText = new TextObject();
            totalText.Parent = groupHeaderBand.GroupFooter;
            totalText.CreateUniqueName();
            totalText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
            totalText.Text = "Rows: [TotalRows]";
            totalText.HorzAlign = HorzAlign.Right;
            totalText.Border.Lines = BorderLines.Top;

            return report;
        }

        static Report GetNestedGroupsReport()
        {
            Report report = new Report();

            // register all data tables and relations
            report.RegisterData(dataSet);

            // enable the "Products" table to use it in the report
            report.GetDataSource("Products").Enabled = true;

            // add report page
            ReportPage page = new ReportPage();
            report.Pages.Add(page);
            // always give names to objects you create. You can use CreateUniqueName method to do this;
            // call it after the object is added to a report.
            page.CreateUniqueName();

            // create group header
            GroupHeaderBand groupHeaderBand = new GroupHeaderBand();
            page.Bands.Add(groupHeaderBand);
            groupHeaderBand.Height = Units.Centimeters * 1;
            groupHeaderBand.Condition = "[Products.ProductName].Substring(0,1)";

            // create group text
            TextObject groupText = new TextObject();
            groupText.Parent = groupHeaderBand;
            groupText.CreateUniqueName();
            groupText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 1);
            groupText.Font = new Font("Arial", 14, FontStyle.Bold);
            groupText.Text = "[[Products.ProductName].Substring(0,1)]";
            groupText.VertAlign = VertAlign.Center;
            groupText.Fill = new LinearGradientFill(Color.OldLace, Color.Moccasin, 90, 0.5f, 1);

            // create nested group header
            GroupHeaderBand nestedGroupBand = new GroupHeaderBand();
            groupHeaderBand.NestedGroup = nestedGroupBand;
            nestedGroupBand.Height = Units.Centimeters * 0.5f;
            nestedGroupBand.Condition = "[Products.ProductName].Substring(0,2)";

            // create nested group text
            TextObject nestedText = new TextObject();
            nestedText.Parent = nestedGroupBand;
            nestedText.CreateUniqueName();
            nestedText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
            nestedText.Font = new Font("Arial", 10, FontStyle.Bold);
            nestedText.Text = "[[Products.ProductName].Substring(0,2)]";

            // create data band
            DataBand dataBand = new DataBand();
            // connect it to inner group
            nestedGroupBand.Data = dataBand;
            dataBand.CreateUniqueName();
            dataBand.DataSource = report.GetDataSource("Products");
            dataBand.Height = Units.Centimeters * 0.5f;
            // set sort by product name
            dataBand.Sort.Add(new Sort("[Products.ProductName]"));

            // create product name text
            TextObject productText = new TextObject();
            productText.Parent = dataBand;
            productText.CreateUniqueName();
            productText.Bounds = new RectangleF(Units.Centimeters * 0.5f, 0, Units.Centimeters * 9.5f, Units.Centimeters * 0.5f);
            productText.Text = "[Products.ProductName]";

            // create group footer for outer group
            groupHeaderBand.GroupFooter = new GroupFooterBand();
            groupHeaderBand.GroupFooter.CreateUniqueName();
            groupHeaderBand.GroupFooter.Height = Units.Centimeters * 1;

            // create total
            Total groupTotal = new Total();
            groupTotal.Name = "TotalRows";
            groupTotal.TotalType = TotalType.Count;
            groupTotal.Evaluator = dataBand;
            groupTotal.PrintOn = groupHeaderBand.GroupFooter;
            report.Dictionary.Totals.Add(groupTotal);

            // show total in the group footer
            TextObject totalText = new TextObject();
            totalText.Parent = groupHeaderBand.GroupFooter;
            totalText.CreateUniqueName();
            totalText.Bounds = new RectangleF(0, 0, Units.Centimeters * 10, Units.Centimeters * 0.5f);
            totalText.Text = "Rows: [TotalRows]";
            totalText.HorzAlign = HorzAlign.Right;
            totalText.Border.Lines = BorderLines.Top;

            return report;
        }

        static Report GetSubreportReport()
        {
            Report report = new Report();

            // register all data tables and relations
            report.RegisterData(dataSet);

            // enable the "Products" and "Suppliers" tables to use it in the report
            report.GetDataSource("Products").Enabled = true;
            report.GetDataSource("Suppliers").Enabled = true;

            // add report page
            ReportPage page = new ReportPage();
            report.Pages.Add(page);
            // always give names to objects you create. You can use CreateUniqueName method to do this;
            // call it after the object is added to a report.
            page.CreateUniqueName();

            // create title band
            page.ReportTitle = new ReportTitleBand();
            // native FastReport unit is screen pixel, use conversion 
            page.ReportTitle.Height = Units.Centimeters * 1;
            page.ReportTitle.CreateUniqueName();

            // create two title text objects
            TextObject titleText1 = new TextObject();
            titleText1.Parent = page.ReportTitle;
            titleText1.CreateUniqueName();
            titleText1.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 1);
            titleText1.Font = new Font("Arial", 14, FontStyle.Bold);
            titleText1.Text = "Products";
            titleText1.HorzAlign = HorzAlign.Center;

            TextObject titleText2 = new TextObject();
            titleText2.Parent = page.ReportTitle;
            titleText2.CreateUniqueName();
            titleText2.Bounds = new RectangleF(Units.Centimeters * 9, 0, Units.Centimeters * 8, Units.Centimeters * 1);
            titleText2.Font = new Font("Arial", 14, FontStyle.Bold);
            titleText2.Text = "Suppliers";
            titleText2.HorzAlign = HorzAlign.Center;

            // create report title's child band that will contain subreports
            ChildBand childBand = new ChildBand();
            page.ReportTitle.Child = childBand;
            childBand.CreateUniqueName();
            childBand.Height = Units.Centimeters * 0.5f;

            // create the first subreport
            SubreportObject subreport1 = new SubreportObject();
            subreport1.Parent = childBand;
            subreport1.CreateUniqueName();
            subreport1.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);

            // create subreport's page
            ReportPage subreportPage1 = new ReportPage();
            report.Pages.Add(subreportPage1);
            // connect subreport to page
            subreport1.ReportPage = subreportPage1;

            // create report on the subreport's page
            DataBand dataBand = new DataBand();
            subreportPage1.Bands.Add(dataBand);
            dataBand.CreateUniqueName();
            dataBand.DataSource = report.GetDataSource("Products");
            dataBand.Height = Units.Centimeters * 0.5f;

            TextObject productText = new TextObject();
            productText.Parent = dataBand;
            productText.CreateUniqueName();
            productText.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);
            productText.Text = "[Products.ProductName]";


            // create the second subreport
            SubreportObject subreport2 = new SubreportObject();
            subreport2.Parent = childBand;
            subreport2.CreateUniqueName();
            subreport2.Bounds = new RectangleF(Units.Centimeters * 9, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);

            // create subreport's page
            ReportPage subreportPage2 = new ReportPage();
            report.Pages.Add(subreportPage2);
            // connect subreport to page
            subreport2.ReportPage = subreportPage2;

            // create report on the subreport's page
            DataBand dataBand2 = new DataBand();
            subreportPage2.Bands.Add(dataBand2);
            dataBand2.CreateUniqueName();
            dataBand2.DataSource = report.GetDataSource("Suppliers");
            dataBand2.Height = Units.Centimeters * 0.5f;

            // create supplier name text
            TextObject supplierText = new TextObject();
            supplierText.Parent = dataBand2;
            supplierText.CreateUniqueName();
            supplierText.Bounds = new RectangleF(0, 0, Units.Centimeters * 8, Units.Centimeters * 0.5f);
            supplierText.Text = "[Suppliers.CompanyName]";

            return report;
        }

        static Report GetTableReport()
        {
            Report report = new Report();
            ReportPage page = new ReportPage();
            page.Name = "Page1";
            report.Pages.Add(page);
            DataBand dataBand = new DataBand();
            dataBand.Name = "DataBand";
            page.Bands.Add(dataBand);
            TableObject table = new TableObject();
            table.Name = "Table1";
            table.RowCount = 10;
            table.ColumnCount = 10;
            for (int i = 0; i < 10; i++)
                for (int j = 0; j < 10; j++)
                {
                    table[j, i].Text = (10 * i + j + 1).ToString();
                    table[j, i].Border.Lines = BorderLines.All;
                }
            dataBand.Objects.Add(table);
            table.CreateUniqueNames();
            return report;
        }

        #endregion

        static void Main(string[] args)
        {

            Console.WriteLine("Welcome! Choose report type, please:\n" +
                "1 - SimpleList\n" +
                "2 - MasterDetail\n" +
                "3 - Group\n" +
                "4 - NestedGroups\n" +
                "5 - Subreport\n" +
                "6 - Table");
            Report report;
            //while 1,2,3,4 is not pressed
            char key;
            do
            {
                key = Console.ReadKey().KeyChar;
            }
            while ((int)key < 49 || (int)key > 54);

            // create report instance
            switch (key)
            {
                case '1': report = GetSimpleListReport(); break;
                case '2': report = GetMasterDetailReport(); break;
                case '3': report = GetGroupReport(); break;
                case '4': report = GetNestedGroupsReport(); break;
                case '5': report = GetSubreportReport(); break;
                case '6': report = GetTableReport(); break;
                default: report = GetSimpleListReport(); break;
            }

            // prepare the report
            report.Prepare();

            // save prepared report
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);
            report.SavePrepared(Path.Combine(outFolder, "Prepared Report.fpx"));

            // export to image
            ImageExport image = new ImageExport();
            image.ImageFormat = ImageExportFormat.Jpeg;
            report.Export(image, Path.Combine(outFolder, "report.jpg"));

            // free resources used by report
            report.Dispose();

            Console.WriteLine("\nPrepared report and report exported as image have been saved into the 'out' folder.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
