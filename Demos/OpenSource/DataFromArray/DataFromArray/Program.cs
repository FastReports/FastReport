using FastReport;
using FastReport.Export.Image;
using System;
using System.IO;

namespace DataFromArray
{
    class Program
    {
        private static int[] array;
        private static string outFolder = @"..\..\..\out\";
        private static string inFolder = @"..\..\..\in\";

        static Program()
        {
            inFolder = Utils.FindDirectory("in");
            outFolder = Directory.GetParent(inFolder).FullName + "\\out";
        }

        private static void CreateArray()
        {
            array = new int[10];
            for (int i = 0; i < 10; i++)
            {
                array[i] = i + 1;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! Press any key to procced...");
            Console.ReadKey();
            CreateArray();

            // create report instance
            Report report = new Report();

            // load the existing report
            report.Load($@"{inFolder}\report.frx");

            // register the array
            report.RegisterData(array, "Array");

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
