using Demo.MVC.DataBase.Models;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;

namespace Demo.MVC.DataBase.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public HomeController(FastreportContext context)
        {
            _context = context;
        }

        private readonly FastreportContext _context;

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

            var reportsDirectory = FindReportsFolder(Environment.CurrentDirectory);

            model.WebReport.Report.Load(Path.Combine(reportsDirectory, reportToLoad + ".frx"));

            var dataSet = _context.GetDataSet("NorthWind");

            model.WebReport.Report.RegisterData(dataSet, "NorthWind");

            //Uncomment to use ToolbarCustomization
            //ToolbarSettings toolbar = new ToolbarSettings()
            //{
            //    Color = Color.Red,
            //    DropDownMenuColor = Color.Red,
            //    DropDownMenuTextColor = Color.White,
            //    IconColor = IconColors.White,
            //    Position = Positions.Top,
            //    FontSettings = new Font("Arial", 14, FontStyle.Bold),
            //    Height = 80,
            //    Exports = new ExportMenuSettings()
            //    {
            //        ExportTypes = Exports.Pdf | Exports.Excel97 | Exports.Rtf
            //    }
            //};
            //model.WebReport.Toolbar = toolbar;

            return View(model);
        }

        private string FindReportsFolder(string startDir)
        {
            string directory = Path.Combine(startDir, "Reports");
            if (Directory.Exists(directory))
                return directory;

            for (int i = 0; i < 6; i++)
            {
                startDir = Path.Combine(startDir, "..");
                directory = Path.Combine(startDir, "Reports");
                if (Directory.Exists(directory))
                    return directory;
            }

            throw new Exception("Demos/Reports directory is not found");
        }
    }

}