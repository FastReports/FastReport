using FastReport;
using FastReport.Export.PdfSimple;
using FastReport.Utils;
using System;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace PdfExport
{
    class Program
    {
        static void Main(string[] args)
        {
            string reportsFolder = FindReportsFolder();

            Report report = new Report();
            report.Load(Path.Combine(reportsFolder, "Simple List.frx"));

            DataSet data = new DataSet();
            data.ReadXml(Path.Combine(reportsFolder, "nwind.xml"));

            report.RegisterData(data, "NorthWind");

            report.Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();

            pdfExport.Export(report, "Simple List.pdf");

            
        }


        public static string FindReportsFolder()
        {
            string FReportsFolder = "";
            string thisFolder = Config.ApplicationFolder;

            for (int i = 0; i < 6; i++)
            {
                string dir = Path.Combine(thisFolder, "Reports");
                if (Directory.Exists(dir))
                {
                    string rep_dir = Path.GetFullPath(dir);
                    if (System.IO.File.Exists(Path.Combine(rep_dir, "reports.xml")))
                    {
                        FReportsFolder = rep_dir;
                        break;
                    }
                }
                thisFolder = Path.Combine(thisFolder, @"..");
            }
            return FReportsFolder;
        }
    }
}
