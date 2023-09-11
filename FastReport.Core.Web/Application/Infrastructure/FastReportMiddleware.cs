#if !WASM
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FastReport.Web.Infrastructure
{
    sealed class FastReportMiddleware
    {
        private static readonly ValueTask<bool> _falseResult = new ValueTask<bool>(false);

        private readonly RequestDelegate next;

        Func<HttpContext, ValueTask<bool>> ExecuteFunc;

        public FastReportMiddleware(RequestDelegate next)
        {
            this.next = next;
            ExecuteFunc = RequestFastReportControllerStart;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (!(await ExecuteFunc(httpContext)))
                await next(httpContext);
        }

        private ValueTask<bool> RequestFastReportControllerStart(HttpContext httpContext)
        {
            FastReportGlobal.FastReportOptions.RoutePathBaseRoot = httpContext.Request.PathBase;
            ExecuteFunc = RequestFastReportController;
            return RequestFastReportController(httpContext);
        }

        private ValueTask<bool> RequestFastReportController(HttpContext httpContext)
        {
            if (!httpContext.Request.Path.StartsWithSegments(FastReportGlobal.FastReportOptions.RouteBasePath))
                return _falseResult;

            return ControllerBuilder.Executor.ExecuteAsync(httpContext);
        }
    }
}
#endif