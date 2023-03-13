using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MVC.Service;

namespace Demo.SPA.Angular.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebReportController : Controller
    {
        private readonly DataSetService _dataSetService;


        public WebReportController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }


        [HttpGet]
        [Route("get")]
        public IActionResult Get()
        {
            WebReport WebReport = new WebReport();
            WebReport.Width = "1000";
            WebReport.Height = "1000";

            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Loading a report into a WebReport object

            WebReport.Report.RegisterData(_dataSetService.DataSet, "NorthWind"); //Registering a data source in a report
            WebReport.Report.Prepare();
            ViewBag.WebReport = WebReport;
            return View();
        }
    }
}