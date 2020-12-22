using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Http;

namespace FastReport.Web.Controllers
{
    class ResourceController : BaseController
    {
        public ResourceController() : base()
        {
            RegisterHandler("/resources.getResource", async () =>
            {
                var resourceName = Request.Query["resourceName"].ToString();
                var resource = await Resources.Instance.GetBytes(resourceName);
                if (resource == null)
                    return new NotFoundResult();

                var contentType = Request.Query["contentType"].ToString();
                if (contentType.IsNullOrWhiteSpace())
                    contentType = "application/octet-stream";

                return new FileContentResult(resource, contentType);
            });
        }
    }
}
