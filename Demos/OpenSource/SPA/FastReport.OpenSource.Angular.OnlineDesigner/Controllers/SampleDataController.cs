using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MVC.Service;

namespace Demo.SPA.Angular.OnlineDesigner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleDataController : Controller
    {
        private readonly DataSetService _dataSetService;


        public SampleDataController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        [HttpGet]
        [Route("design")]
        public IActionResult Design()
        {
            WebReport WebReport = new WebReport();
            WebReport.Width = "1000";
            WebReport.Height = "1000";
            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Loading a report into a WebReport object
            
            WebReport.Report.RegisterData(_dataSetService.DataSet, "NorthWind"); //Registering a data source in a report
            WebReport.Mode = WebReportMode.Designer; //Set the mode of the web report object - display designer
            WebReport.Designer.Locale = "en";
            WebReport.Designer.Path = @"WebReportDesigner/index.html"; //Set the URL of the online designer
            WebReport.Designer.SaveCallBack = @"SampleData/SaveDesignedReport"; //Set the view URL for the report save method
            ViewBag.WebReport = WebReport; //pass report to View
            return View();
        }


        [HttpPost]
        // call-back for save the designed report 
        public ActionResult SaveDesignedReport(string reportID, string reportUUID)
        {
            ViewBag.Message = String.Format("Confirmed {0} {1}", reportID, reportUUID); //Set the message for the view

            Stream reportForSave = Request.Body; //We write the result of the Post request to the stream
            string pathToSave = @"App_Data/TestReport.frx"; //Get the path to save the file
            using (FileStream file = new FileStream(pathToSave, FileMode.Create)) //Create a file stream
            {
                reportForSave.CopyTo(file); //Saving the query result to a file
            }
            return View();
        }
    }
}
