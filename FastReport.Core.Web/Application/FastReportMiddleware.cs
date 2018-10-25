using FastReport.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using System;
using System.Threading.Tasks;

namespace FastReport.Web
{
    class FastReportMiddleware
    {
        private readonly RequestDelegate next;

        public FastReportMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!(await RequestFastReportController(httpContext)))
                await next(httpContext);
        }

        private async Task<bool> RequestFastReportController(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.StartsWithSegments(WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath)))
                return false;

            if (!(FastReportGlobal.FastReportOptions.Authorize?.Invoke(httpContext.Request) ?? true))
            {
                await new UnauthorizedResult().ExecuteResultAsync(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));
                return true;
            }

            // create new controllers on each request
            var controllers = new BaseController[]
            {
                new ResourceController(),
                new ReportController(),
                new DesignerController(),
            };

            foreach (var controller in controllers)
            {
                if (await controller.OnRequest(httpContext))
                    return true;
            }

            return false;
        }
    }
}
