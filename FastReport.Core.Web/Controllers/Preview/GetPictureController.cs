using FastReport.Web.Cache;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    [Route("/_fr/preview.getPicture")]
    public sealed class GetPictureController : ApiControllerBase
    {
        private readonly IReportService _reportService;

        public GetPictureController(IReportService reportService)
        {
            _reportService = reportService;
        }

        public sealed class GetPictureParams
        {
            public string ReportId { get; set; }

            public string PictureId { get; set; }

        }

        [HttpGet]
        public IActionResult GetPicture([FromQuery] GetPictureParams query)
        {
            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var pictureId = query.PictureId.TrimStart('=');
            if (webReport.PictureCache.TryGetValue(pictureId, out byte[] value))
            {
                string imgType = WebUtils.IsPng(value) ? "image/png" : "image/svg+xml";
                return new FileContentResult(value, imgType);
            }

            return new NotFoundResult();
        }
    }
}
