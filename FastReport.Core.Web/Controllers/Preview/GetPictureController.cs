using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        internal sealed class GetPictureParams
        {
            public string ReportId { get; init; }

            public string PictureId { get; init; }
        }


        [HttpGet("/preview.getPicture")]
        public static IResult GetPicture([FromQuery] GetPictureParams query,
            IReportService reportService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return Results.NotFound();

            var pictureId = query.PictureId.TrimStart('=');
            if (webReport.PictureCache.TryGetValue(pictureId, out byte[] value))
            {
                string imgType = WebUtils.IsPng(value) ? "image/png" : "image/svg+xml";
                return Results.File(value, imgType);
            }

            return Results.NotFound();
        }
    }
}
