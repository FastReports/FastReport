using FastReport.Web.Cache;

using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace FastReport.Web.Controllers
{
    public sealed class ServiceController : ApiControllerBase
    {

        public ServiceController()
        {
        }

        public sealed class PrintReportParams
        {
            public string ReportId { get; set; }

            public string Click { get; set; }
        }

        [HttpGet]
        [Route("/_fr/preview.textEditForm")]
        public IActionResult TextEditForm([FromQuery] PrintReportParams query)
        {
            if (!TryFindWebReport(query.ReportId, out WebReport webReport))
                return new NotFoundResult();

            var click = query.Click;
            if (!click.IsNullOrWhiteSpace())
            {
                var @params = click.Split(',');
                if (@params.Length == 4)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        string result = null;

                        webReport.Report.FindClickedObject<TextObject>(@params[0], pageN, left, top,
                            (textObject, reportPage, _pageN) =>
                            {
                                webReport.Res.Root("Buttons");
                                string okText = webReport.Res.Get("Ok");
                                string cancelText = webReport.Res.Get("Cancel");
                                result = ReportController.Template_textedit_form(textObject.Text, okText, cancelText);
                            });

                        if (result != null)
                        {
                            return new ContentResult()
                            {
                                StatusCode = (int)HttpStatusCode.OK,
                                ContentType = "text/html",
                                Content = result,
                            };
                        }
                    }
                }
            }

            return new NotFoundResult();
        }

        bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

    }
}
