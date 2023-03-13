using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using MVC.Service;

namespace Demo.SPA.React.Controllers
{

    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly DataSetService _dataSetService;
        public HomeController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string reportToLoad)
        {
            WebReport webReport = new WebReport();
            if (reportToLoad == "-- Select report name --")
            {
                reportToLoad = "Groups";
            }

            webReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            webReport.Report.RegisterData(_dataSetService.DataSet, "NorthWind");

            //webReport.Mode = WebReportMode.Designer;
            //webReport.Width = "1000px";
            //webReport.Height = "1000px";

            ViewBag.WebReport = webReport;
            return View();
        }
    }
}
