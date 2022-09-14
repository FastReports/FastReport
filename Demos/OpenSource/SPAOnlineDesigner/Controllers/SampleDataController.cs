using System;
using Microsoft.AspNetCore.Mvc;
using FastReport.Web;
using System.IO;

namespace SPAOnlineDesigner.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {

        [HttpGet("[action]")]
        public IActionResult Design()
        {
            WebReport WebReport = new WebReport();
            WebReport.Width = "1000";
            WebReport.Height = "1000";
            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Loading a report into a WebReport object
            System.Data.DataSet dataSet = new System.Data.DataSet(); //Create a data source
            dataSet.ReadXml("App_Data/nwind.xml");  //open xml database
            WebReport.Report.RegisterData(dataSet, "NorthWind"); //Registering a data source in a report
            WebReport.Mode = WebReportMode.Designer; //Set the mode of the web report object - display designer
            WebReport.DesignerLocale = "en";
            WebReport.DesignerPath = @"WebReportDesigner/index.html"; //Set the URL of the online designer
            WebReport.DesignerSaveCallBack = @"SampleData/SaveDesignedReport"; //Set the view URL for the report save method
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
