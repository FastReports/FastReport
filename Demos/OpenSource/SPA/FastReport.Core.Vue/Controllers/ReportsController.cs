using System.Data;
using System.Drawing;
using FastReport;
using FastReport.Export.Html;
using FastReport.Export.Image;
using FastReport.Web;
using Microsoft.AspNetCore.Mvc;
using MVC.Service;

namespace Demo.SPA.Vue.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportsController : Controller
    {
        private readonly DataSetService _dataSetService;

        public ReportsController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        [HttpGet("[action]")]
        public IActionResult ShowReport(string name)
        {
            var webReport = CreateWebReport(name);

            ViewBag.WebReport = webReport;

            return View();
        }
        [HttpGet("[action]")]
        public IActionResult Download([FromQuery] ReportQuery query)
        {
            string mime = "application/" + query.Format;

            using (MemoryStream stream = new MemoryStream())
            {
                try
                {
                    var report = CreateReport(query.Name);
                    report.Prepare();

                    switch (query.Format)
                    {

                        case "png":
                            ImageExport png = new ImageExport()
                            {
                                ImageFormat = ImageExportFormat.Png,
                                SeparateFiles = false
                            };
                            report.Export(png, stream);
                            break;
                        case "html":
                            HTMLExport html = new HTMLExport()
                            {
                                SinglePage = true,
                                Navigator = false,
                                EmbedPictures = true
                            };
                            report.Export(html, stream);

                            mime = "text/html";

                            break;
                    }

                    var saveName = string.Concat(query.Name, ".", query.Format);
                    return File(stream.ToArray(), mime, saveName); ;
                }
                finally
                {
                    stream.Dispose();
                }
            }
        }

        private WebReport CreateWebReport(string name)
        {
            WebReport webReport = new WebReport()
            {
                Width = "1000",
                Height = "1000",
                Report = CreateReport(name),
                Toolbar = new ToolbarSettings()
                {
                    Position = Positions.Left,
                    Exports = new ExportMenuSettings() { ExportTypes = Exports.Prepared },
                    Color = name == "Master-Detail" ? Color.DarkSlateGray : Color.Orange,
                    IconColor = name == "Master-Detail" ? IconColors.White : IconColors.Black,
                    ShowPrint = false
                },

            };
            return webReport;
        }

        private Report CreateReport(string name)
        {
            Report report = new Report();
            report.Load((Path.Combine(_dataSetService.ReportsPath, name + ".frx")));
            report.RegisterData(_dataSetService.DataSet, "NorthWind");

            return report;
        }
    }
}
