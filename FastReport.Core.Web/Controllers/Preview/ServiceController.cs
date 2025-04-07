using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Net.Mime;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {

        internal sealed class TextEditParams
        {
            public string ReportId { get; init; }

            public string Click { get; init; }
        }


        [HttpGet("/preview.textEditForm")]
        public static IResult TextEditForm([FromQuery] TextEditParams query,
            IReportService _reportService,
            ITextEditService _textEditService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            if (!_reportService.TryFindWebReport(query.ReportId, out WebReport webReport))
                return Results.NotFound();

            var result = _textEditService.GetTemplateTextEditForm(query.Click, webReport);

            if (result != null)
            {
                return Results.Content(result, MediaTypeNames.Text.Html);
            }
            else
                return Results.NotFound();
        }
    }
}
