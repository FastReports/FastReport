using FastReport.Web.Infrastructure;
using FastReport.Web.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        internal sealed class GetResourceParams
        {
            public string ResourceName { get; init; }

            public string ContentType { get; set; }
        }

        [HttpGet("/resources.getResource")]
        public static async Task<IResult> GetResource([FromQuery] GetResourceParams query,
            IResourceLoader resourceLoader,
            CancellationToken cancellationToken)
        {
            var resource = await resourceLoader.GetBytesAsync(query.ResourceName, cancellationToken);
            if (resource == null)
                return Results.NotFound();

            if (query.ContentType.IsNullOrWhiteSpace())
                query.ContentType = "application/octet-stream";

            return Results.File(resource, query.ContentType);
        }

    }
}
