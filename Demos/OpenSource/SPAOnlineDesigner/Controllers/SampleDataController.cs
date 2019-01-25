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
            WebReport.Report.Load("App_Data/Master-Detail.frx"); //Загружаем отчет в объект WebReport
            System.Data.DataSet dataSet = new System.Data.DataSet(); //Создаем источник данных
            dataSet.ReadXml("App_Data/nwind.xml");  //Открываем базу данных xml
            WebReport.Report.RegisterData(dataSet, "NorthWind"); //Регистрируем источник данных в отчете
            WebReport.Mode = WebReportMode.Designer; //Устанавливаем режим объекта веб отчет - отображение дизайнера
            WebReport.DesignerLocale = "en";
            WebReport.DesignerPath = @"WebReportDesigner/index.html"; //Задаем URL онлайн дизайнера
            WebReport.DesignerSaveCallBack = @"SampleData/SaveDesignedReport"; //Задаем URL представления для метода сохранения отчета
            ViewBag.WebReport = WebReport; //передаем отчет во View
            return View();
        }


        [HttpPost]
        // call-back for save the designed report 
        public ActionResult SaveDesignedReport(string reportID, string reportUUID)
        {
            ViewBag.Message = String.Format("Confirmed {0} {1}", reportID, reportUUID); //Задаем сообщение для представления

            Stream reportForSave = Request.Body; //Записываем в поток результат Post запроса
            string pathToSave = @"App_Data/TestReport.frx"; //получаем путь для сохранения файла
            using (FileStream file = new FileStream(pathToSave, FileMode.Create)) //Создаем файловый поток
            {
                reportForSave.CopyTo(file); //Сохраняем результат запроса в файл
            }
            return View();
        }


    }
}
