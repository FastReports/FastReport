using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers
{
    [Route("/_fr/resources.getResource")]
    public sealed class ResourcesController : ApiControllerBase
    {
        private readonly IResourceLoader _resourceLoader;

        public ResourcesController(IResourceLoader resourceLoader)
        {
            _resourceLoader = resourceLoader;
        }

        public sealed class GetResourceParams
        {
            public string resourceName { get; set; }

            public string contentType { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetResource([FromQuery] GetResourceParams query)
        {
            var resource = await _resourceLoader.GetBytesAsync(query.resourceName);
            if (resource == null)
                return new NotFoundResult();

            if (query.contentType.IsNullOrWhiteSpace())
                query.contentType = "application/octet-stream";

            return new FileContentResult(resource, query.contentType);
        }

    }
}
