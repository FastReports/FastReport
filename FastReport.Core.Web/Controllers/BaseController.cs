using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FastReport.Web.Controllers
{
    abstract class BaseController
    {
        public readonly struct Handler
        {
            //public HttpMethod Method;
            public readonly TemplateMatcher RouteTemplate;
            public readonly Func<IActionResult> ActionSync;
            public readonly Func<Task<IActionResult>> ActionAsync;

            public Handler(TemplateMatcher routeTemplate,
                            Func<IActionResult> actionSync,
                            Func<Task<IActionResult>> actionAsync)
            {
                RouteTemplate = routeTemplate;
                ActionSync = actionSync;
                ActionAsync = actionAsync;
            }
        }

        //public readonly string RouteBasePath;

        protected HttpContext Context { get; private set; }
        protected HttpRequest Request { get; private set; }
        protected HttpResponse Response { get; private set; }
        //protected RouteValueDictionary RouteValues { get; private set; }

        private readonly List<Handler> handlers = new List<Handler>();

        public BaseController(/*string routeBasePath*/)
        {
            //RouteBasePath = routeBasePath;
        }

        protected void RegisterHandler(/*HttpMethod method, */string routeTemplate, Func<IActionResult> action)
        {
            RegisterHandler(/*method, */routeTemplate, new RouteValueDictionary(), action);
        }

        protected void RegisterHandler(/*HttpMethod method, */string routeTemplate, RouteValueDictionary routeDefaults, Func<IActionResult> action)
        {
            handlers.Add(new Handler(
                //Method = method,
                routeTemplate: new TemplateMatcher(TemplateParser.Parse(WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, /*RouteBasePath, */routeTemplate).TrimStart('/')), routeDefaults),
                actionSync: action,
                actionAsync: null
            ));
        }

        protected void RegisterHandler(/*HttpMethod method, */string routeTemplate, Func<Task<IActionResult>> action)
        {
            RegisterHandler(/*method, */routeTemplate, new RouteValueDictionary(), action);
        }

        protected void RegisterHandler(/*HttpMethod method, */string routeTemplate, RouteValueDictionary routeDefaults, Func<Task<IActionResult>> action)
        {
            handlers.Add(new Handler(
                //Method = method,
                routeTemplate: new TemplateMatcher(TemplateParser.Parse(WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, /*RouteBasePath, */routeTemplate).TrimStart('/')), routeDefaults),
                actionAsync: action,
                actionSync: null
            ));
        }

        public async Task<bool> OnRequest(HttpContext httpContext)
        {
            //if (!httpContext.Request.Path.StartsWithSegments(WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, RouteBasePath)))
            //    return false;

            foreach (var handler in handlers)
            {
                //if (httpContext.Request.Method != handler.Method.Method)
                //    continue;

                var routeValues = new RouteValueDictionary();

                if (!handler.RouteTemplate.TryMatch(httpContext.Request.Path, routeValues))
                    continue;

                // setup values for action
                Context = httpContext;
                Request = httpContext.Request;
                Response = httpContext.Response;
                //RouteValues = routeValues;

                IActionResult actionResult = null;

                //try
                //{

                if (handler.ActionAsync != null)
                    actionResult = await handler.ActionAsync.Invoke();
                if (actionResult == null && handler.ActionSync != null)
                    actionResult = await Task.Run(() => handler.ActionSync.Invoke());

                //}
                //catch (Exception e)
                //{
                //    actionResult = new ContentResult()
                //    {
                //        StatusCode = (int)HttpStatusCode.InternalServerError,
                //        ContentType = "text/html",
                //        Content = e.ToString(),
                //    };
                //}

                await actionResult.ExecuteResultAsync(new ActionContext(httpContext, new RouteData(), new ActionDescriptor()));

                // clear values after action
                Context = null;
                Request = null;
                Response = null;
                //RouteValues = null;

                return true;
            }

            return false;
        }
    }
}
