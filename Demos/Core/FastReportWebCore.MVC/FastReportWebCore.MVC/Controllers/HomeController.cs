using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastReport.Web;
using FastReportWebCore.MVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace FastReportWebCore.MVC.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
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

            model.WebReport.Report.Load($"../../../Reports/{reportToLoad}.frx");

            var dataSet = new DataSet();
            dataSet.ReadXml("../../../Reports/nwind.xml");
            model.WebReport.Report.RegisterData(dataSet, "NorthWind");

            return View(model);
        }
    }
}
