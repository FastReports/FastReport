using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FastReport.OpenSource.MVC.Models;

using System.Reflection;
using FastReport;
using FastReport.Export.Html;
using FastReport.Data;
using System.Data;

using System.IO;
using System.Text;
using FastReport.Utils;

namespace FastReport.OpenSource.MVC.Controllers
{
    public class HomeController : Controller
    {

        private readonly FastreportContext _context;

        public static Dictionary<string, Dictionary<string, int>> Reports { get; private set; } = new Dictionary<string, Dictionary<string, int>>();
        public static List<string> ReportFiles { get; private set; } = new List<string>();

        private static DataSet NorthWind;

        static string ReportsFolder = FindReportsFolder();

        static HomeController()
        {

            XmlDocument reports = new XmlDocument();
            reports.Load(Path.Combine(ReportsFolder, "reports.xml"));

            for (int i = 0; i < reports.Root.Count; i++)
            {
                XmlItem folderItem = reports.Root[i];
                if (folderItem.GetProp("WinDemo") == "false")
                    continue;
                if (folderItem.GetProp("WebDemo") == "false")
                    continue;
                if (folderItem.GetProp("Core") == "false")
                    continue;
                string culture = System.Globalization.CultureInfo.CurrentCulture.Name;
                string text = folderItem.GetProp("Name-" + culture);
                if (String.IsNullOrEmpty(text))
                    text = folderItem.GetProp("Name");

                Dictionary<string, int> folderNode = new Dictionary<string, int>();
                Reports[text] = folderNode;

                for (int j = 0; j < folderItem.Count; j++)
                {
                    XmlItem reportItem = folderItem[j];
                    if (reportItem.GetProp("WinDemo") == "false")
                        continue;
                    if (reportItem.GetProp("WebDemo") == "false")
                        continue;
                    if (reportItem.GetProp("Core") == "false")
                        continue;

                    string file = reportItem.GetProp("File");
                    string fileName = reportItem.GetProp("Name-" + culture);
                    if (String.IsNullOrEmpty(fileName))
                        fileName = Path.GetFileNameWithoutExtension(file);


                    folderNode[fileName] = ReportFiles.Count;
                    ReportFiles.Add(Path.Combine(ReportsFolder, file));
                }
            }
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

        public HomeController(FastreportContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? report = null)
        {
            string strReport = ReportFiles[report.GetValueOrDefault()];
            if (strReport != null)
            {
                Report r = new Report();

                if (NorthWind == null)
                    NorthWind = _context.GetDataSet("NorthWind");
                r.RegisterData(NorthWind, "NorthWind");
                r.Load(strReport);

                r.Prepare();
                HTMLExport export = new HTMLExport();
                export.Layers = true;
                using (MemoryStream ms = new MemoryStream())
                {
                    export.EmbedPictures = true;
                    export.Export(r, ms);
                    ms.Flush();
                    ViewData["Report"] = Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            return View();
        }



        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            //ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
