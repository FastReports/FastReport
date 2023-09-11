#if !WASM
using System;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Routing;
using static FastReport.Web.Infrastructure.ControllerBuilder;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace FastReport.Web.Infrastructure
{
    internal sealed class ControllerExecutor
    {
        private readonly EndpointInfo[] _endpoints;

        public ControllerExecutor(EndpointInfo[] endpoints)
        {
            _endpoints = endpoints;
        }

        public async ValueTask<bool> ExecuteAsync(HttpContext httpContext)
        {
            bool success = false;
            var endpoint = FindEndpoint(httpContext.Request);

            if (endpoint.Method != null)    // Found
            {
                await ExecuteEndpoint(endpoint, httpContext);
                success = true;
            }

            return success;
        }

        internal static async Task ExecuteEndpoint(EndpointInfo endpoint, HttpContext httpContext)
        {
            // TODO: create scope ?
            //using var scope = httpContext.RequestServices.CreateScope();

            object[] arguments = ResolveDependencies(endpoint, httpContext);

            var returnValue = endpoint.Method.Invoke(null, arguments);

            await endpoint.Handler(httpContext, returnValue);
        }

        private EndpointInfo FindEndpoint(HttpRequest httpRequest)
        {
            var path = httpRequest.Path;

            var routeValues = new RouteValueDictionary();
            var endpoints = _endpoints.Where(endpoint =>
                endpoint.HttpMethod == httpRequest.Method &&
                endpoint.Route.TryMatch(path, routeValues)
            );

            return endpoints.FirstOrDefault();
        }

        private static object[] ResolveDependencies(EndpointInfo endpoint, HttpContext httpContext)
        {
            object[] arguments = new object[endpoint.Parameters.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                var param = endpoint.Parameters[i];
                arguments[i] = param.Invoke(httpContext);
            }
            return arguments;
        }
    }

}
#endif
