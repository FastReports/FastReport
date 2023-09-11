using FastReport.Web.Infrastructure;

using Microsoft.AspNetCore.Http;

namespace FastReport.Web.Controllers
{
    internal static partial class Controllers
    {
        private static bool IsAuthorized(HttpRequest request) => FastReportGlobal.FastReportOptions.CheckAuthorization(request);

    }
}
