using System;
using System.Data;
using System.IO;
using ConsoleAppsUtils;
using FastReport;
using FastReport.Export.Image;

namespace DataFromDataSet
{
    class Program
    {
        private static DataSet dataSet;
        private static readonly string outFolder;
        private static readonly string inFolder;

        static Program()
        {
            inFolder = Utils.FindDirectory("in");
            outFolder = Path.Combine(Directory.GetParent(inFolder).FullName, "out");
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
            Console.WriteLine("Welcome! \nThis demo shows how to:\n -create a simple data set\n -add it to the report" +
                              "\n -export raw report (.frx) to prepared one (.fpx).\nPress any key to proceed...");
            Console.ReadKey();
            CreateDataSet();

            // create report instance
            Report report = new Report();

            // load the existing report
            report.Load(Path.Combine(inFolder, "report.frx"));

            // register the dataset
            report.RegisterData(dataSet);

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
