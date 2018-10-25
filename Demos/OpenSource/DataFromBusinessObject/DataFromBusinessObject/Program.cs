using FastReport;
using FastReport.Export.Image;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataFromBusinessObject
{
    class Program
    {
        private static List<Category> businessObjects;
        private static string outFolder = @"..\..\..\out\";
        private static string inFolder = @"..\..\..\in\";

        static Program()
        {
            inFolder = Utils.FindDirectory("in");
            outFolder = Directory.GetParent(inFolder).FullName + "\\out";
        }

        private static  void CreateBusinessObject()
        {
            businessObjects = new List<Category>();

            Category category = new Category("Beverages", "Soft drinks, coffees, teas, beers");
            category.Products.Add(new Product("Chai", 18m));
            category.Products.Add(new Product("Chang", 19m));
            category.Products.Add(new Product("Ipoh coffee", 46m));
            businessObjects.Add(category);

            category = new Category("Confections", "Desserts, candies, and sweet breads");
            category.Products.Add(new Product("Chocolade", 12.75m));
            category.Products.Add(new Product("Scottish Longbreads", 12.5m));
            category.Products.Add(new Product("Tarte au sucre", 49.3m));
            businessObjects.Add(category);

            category = new Category("Seafood", "Seaweed and fish");
            category.Products.Add(new Product("Boston Crab Meat", 18.4m));
            category.Products.Add(new Product("Red caviar", 15m));
            businessObjects.Add(category);
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome! Press any key to procced...");
            Console.ReadKey();

            CreateBusinessObject();

            // create report instance
            Report report = new Report();

            // load the existing report
            report.Load($@"{inFolder}\report.frx");

            // register the array
            report.RegisterData(businessObjects, "Categories");

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
