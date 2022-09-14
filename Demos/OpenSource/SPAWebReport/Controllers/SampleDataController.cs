using System;
using Microsoft.AspNetCore.Mvc;
using FastReport.Web;
using System.IO;

namespace SPAWebReport.Controllers
{
    [Route("api/[controller]")]
    public class SampleDataController : Controller
    {

        [HttpGet("[action]")]
        public IActionResult ShowReport()
        {
            WebReport WebReport = new WebReport();
            WebReport.Width = "1000";
            WebReport.Height = "1000";
            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Loading a report into a WebReport object
            System.Data.DataSet dataSet = new System.Data.DataSet(); //Create a data source
            dataSet.ReadXml("App_Data/nwind.xml");  //open xml database
            WebReport.Report.RegisterData(dataSet, "NorthWind"); //Registering a data source in a report
            ViewBag.WebReport = WebReport; //pass report to View
            return View();
        }

    }
}
