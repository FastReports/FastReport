#if !NET6_0_OR_GREATER
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;

using System;
using System.Net;
using System.Threading.Tasks;

namespace FastReport.Web.Infrastructure
{
    internal interface IResult
    {
        Task ExecuteAsync(HttpContext httpContext);
    }

    internal sealed class ResultHandler : IResult
    {
        private readonly IActionResult _actionResult;

        public static IResult Parse(IActionResult actionResult)
        {
            return new ResultHandler(actionResult);
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            var actionDescriptor = new ActionDescriptor();
            var context = new ActionContext(httpContext, new RouteData() /*httpContext.GetRouteData()*/, actionDescriptor);
            return _actionResult.ExecuteResultAsync(context);
        }

        private ResultHandler(IActionResult actionResult)
        {
            _actionResult = actionResult;
        }
    }

    internal static class Results
    {
        internal static IResult Content(string content, string contentType) => ResultHandler.Parse(new ContentResult()
        {
            Content = content,
            ContentType = contentType,
            StatusCode = StatusCodes.Status200OK
        });

        internal static IResult BadRequest(string content) => ResultHandler.Parse(new BadRequestObjectResult(content));

        internal static IResult File(
            byte[] fileContents,
            string contentType = null,
            string fileDownloadName = null,
            bool enableRangeProcessing = false,
            DateTimeOffset? lastModified = null)
            => ResultHandler.Parse(new FileContentResult(fileContents, contentType)
            {
                FileDownloadName = fileDownloadName,
                EnableRangeProcessing = enableRangeProcessing,
                LastModified = lastModified
            });

        internal static IResult NotFound() => ResultHandler.Parse(new NotFoundResult());

        internal static IResult Ok() => ResultHandler.Parse(new OkResult());

        internal static IResult Ok(object value)
        {
            IActionResult result;
            if (value == null)
                result = new OkResult();
            else
                result = new OkObjectResult(value);
            return ResultHandler.Parse(result);
        }

        internal static IResult StatusCode(int code) => ResultHandler.Parse(new StatusCodeResult(code));

        internal static IResult Unauthorized() => ResultHandler.Parse(new UnauthorizedResult());
    }
}
#endif