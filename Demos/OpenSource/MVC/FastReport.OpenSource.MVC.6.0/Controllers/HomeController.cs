using Demo.MVC.Net6.Models;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MVC.Service;

namespace Demo.MVC.Net6.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        readonly IWebHostEnvironment _hostingEnvironment;
        readonly DataSetService _dataSetService;

        public HomeController(IWebHostEnvironment hostingEnvironment, DataSetService dataSetService)
        {
            _hostingEnvironment = hostingEnvironment;
            _dataSetService = dataSetService;
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
                    "Polygon",
                    "Barcode",
                },
            };

            var reportToLoad = model.ReportsList[0];
            if (reportIndex >= 0 && reportIndex < model.ReportsList.Length)
                reportToLoad = model.ReportsList[reportIndex.Value];

            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));

            model.WebReport.Report.RegisterData(_dataSetService.DataSet, "NorthWind");

            //model.WebReport.SinglePage = true;

            model.WebReport.Designer.Path = "/WebReportDesigner/index.html";
            //model.WebReport.Designer.SaveCallBack = "/SaveDesignedReport";
            model.WebReport.Designer.SaveMethod = (string reportID, string filename, string report) =>
            {
                string webRootPath = _hostingEnvironment.WebRootPath;

                string pathToSave = Path.Combine(webRootPath, "DesignedReports", filename);
                if (!Directory.Exists(pathToSave))
                    Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));

                System.IO.File.WriteAllTextAsync(pathToSave, report);

                return "OK";
            };


            //Uncomment to use ToolbarCustomization
            //ToolbarSettings toolbar = new ToolbarSettings()
            //{
            //    Color = Color.Red,
            //    DropDownMenuColor = Color.Red,
            //    DropDownMenuTextColor = Color.White,
            //    IconColor = IconColors.White,
            //    Position = Positions.Right,
            //    FontSettings = new Font("Arial", 14, FontStyle.Bold),
            //    Exports = new ExportMenuSettings()
            //    {
            //        ExportTypes = Exports.Pdf | Exports.Excel97 | Exports.Rtf
            //    }
            //};
            //model.WebReport.Toolbar = toolbar;


            // Uncomment to use Online Designer
            //model.WebReport.Width = "1000";
            //model.WebReport.Height = "1000";
            //model.WebReport.Mode = WebReportMode.Designer;

            return View(model);
        }

        [HttpGet("prepared/{file}")]
        public IActionResult Prepared(string file)
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

            model.WebReport.LoadPrepared(file);

            return View("Index", model);
        }
        
        // Call-back for save the designed report 
        [HttpPost("/SaveDesignedReport")]
        public ActionResult SaveDesignedReport(string reportID, [FromQuery(Name = "reportUUID")] string reportName)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;

            Stream reportForSave = Request.Body;
            string pathToSave = Path.Combine(webRootPath, "DesignedReports", reportName);
            if (!Directory.Exists(pathToSave))
                Directory.CreateDirectory(Path.GetDirectoryName(pathToSave));
            using (FileStream file = new FileStream(pathToSave, FileMode.Create))
            {
                reportForSave.CopyToAsync(file);
            }
            return new OkResult();
        }
    }
}