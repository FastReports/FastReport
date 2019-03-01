using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport.Web;
using FastReport.OpenSource.Web.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using FastReport.Utils;

namespace FastReport.OpenSource.Web.MVC.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {

        static string ReportsFolder = FindReportsFolder();

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

        [HttpGet("{reportIndex:int?}")]
        public IActionResult Index(int? reportIndex = 0)
        {
            var model = new HomeModel()
            {
                WebReport = new WebReport(),
                ReportsList = new[]
                {
                    "Simple List",
                    "Labels",
                    "Master-Detail",
                    "Badges",
                    "Interactive Report, 2-in-1",
                    "Hyperlinks, Bookmarks",
                    "Outline",
                    "Complex (Hyperlinks, Outline, TOC)",
                    "Drill-Down Groups",
                    "Mail Merge"
                },
            };

            var reportToLoad = model.ReportsList[0];
            if (reportIndex >= 0 && reportIndex < model.ReportsList.Length)
                reportToLoad = model.ReportsList[reportIndex.Value];

            model.WebReport.Report.Load(Path.Combine(ReportsFolder, $"{reportToLoad}.frx"));

            var dataSet = new DataSet();
            dataSet.ReadXml(Path.Combine(ReportsFolder,"nwind.xml"));
            model.WebReport.Report.RegisterData(dataSet, "NorthWind");

            return View(model);
        }
    }
}
