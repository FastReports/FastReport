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
            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Загружаем отчет в объект WebReport
            System.Data.DataSet dataSet = new System.Data.DataSet(); //Создаем источник данных
            dataSet.ReadXml("App_Data/nwind.xml");  //Открываем базу данных xml
            WebReport.Report.RegisterData(dataSet, "NorthWind"); //Регистрируем источник данных в отчете
            ViewBag.WebReport = WebReport; //передаем отчет во View
            return View();
        }

    }
}
