using FastReport;
using FastReport.Export.Image;
using System;
using System.Data;
using System.IO;

namespace DataFromDataSet
{
    class Program
    {
        private static DataSet dataSet;
        private static string outFolder = @"..\..\..\out\";
        private static string inFolder = @"..\..\..\in\";

        static Program()
        {
            inFolder = Utils.FindDirectory("in");
            outFolder = Directory.GetParent(inFolder).FullName + "\\out";
        }

        private static void CreateDataSet()
        {
            // create simple dataset with one table
            dataSet = new DataSet();

            DataTable table = new DataTable();
            table.TableName = "Employees";
            dataSet.Tables.Add(table);

            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));

            table.Rows.Add(1, "Andrew Fuller");
            table.Rows.Add(2, "Nancy Davolio");
            table.Rows.Add(3, "Margaret Peacock");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! Press any key to procced...");
            Console.ReadKey();
            CreateDataSet();

            // create report instance
            Report report = new Report();

            // load the existing report
            report.Load($@"{inFolder}\report.frx");

            // register the dataset
            report.RegisterData(dataSet);

            // prepare the report
            report.Prepare();

            // save prepared report
            if (!Directory.Exists(outFolder))
                Directory.CreateDirectory(outFolder);
            report.SavePrepared($@"{outFolder}\Prepared Report.fpx");

            // export to image
            ImageExport image = new ImageExport();
            image.ImageFormat = ImageExportFormat.Jpeg;
            report.Export(image, $@"{outFolder}\report.jpg");

            // free resources used by report
            report.Dispose();

            Console.WriteLine("\nPrepared report and report exported as image have been saved into the 'out' folder.");
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
