using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using ConsoleAppsUtils;
using FastReport;
using FastReport.Export.Image;
using FastReport.Utils;

namespace UserFunctions
{
    class Program
    {
        private static readonly string outFolder = @"..\..\..\out\";

        static Program()
        {
            outFolder = Utils.FindDirectory("UserFunctions") + "\\out";
        }

        private static void RegisterOwnFunctions()
        {
            RegisteredObjects.AddFunctionCategory("MyFuncs", "My Functions");

            // obtain MethodInfo for our functions
            Type myType = typeof(MyFunctions);
            MethodInfo myUpperCaseFunc = myType.GetMethod("MyUpperCase");
            MethodInfo myMaximumIntFunc = myType.GetMethod("MyMaximum", new Type[] { typeof(int), typeof(int) });
            MethodInfo myMaximumLongFunc = myType.GetMethod("MyMaximum", new Type[] { typeof(long), typeof(long) });

            // register simple function
            RegisteredObjects.AddFunction(myUpperCaseFunc, "MyFuncs");

            // register overridden functions
            RegisteredObjects.AddFunction(myMaximumIntFunc, "MyFuncs,MyMaximum");
            RegisteredObjects.AddFunction(myMaximumLongFunc, "MyFuncs,MyMaximum");
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome!\nThis demo show how to\n -create custom functions\n -use them in your report \nPress any key to procced...");
            Console.ReadKey();

            RegisterOwnFunctions();

            // create report instance
            Report report = new Report();
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
            titleText1.HorzAlign = HorzAlign.Center;

            // !!! use our function
            titleText1.Text = "[MyUpperCase(\"products\")]";
            // !!!

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
