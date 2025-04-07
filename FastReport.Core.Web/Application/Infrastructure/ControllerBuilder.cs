#if !WASM
using System;
using Microsoft.AspNetCore.Http;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Globalization;
using System.Diagnostics;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.AspNetCore.Routing;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Text.Json;

namespace FastReport.Web.Infrastructure
{
    internal static partial class ControllerBuilder
    {
        private static ControllerExecutor _executor;

        private static readonly JsonSerializerOptions _serializerOptions = new() { PropertyNameCaseInsensitive = true,  NumberHandling = System.Text.Json.Serialization.JsonNumberHandling.AllowReadingFromString };

        public static MethodInfo[] GetControllerMethods()
        {
            return GetControllerMethods(typeof(Controllers.Controllers));
        }

        internal static MethodInfo[] GetControllerMethods(Type fromType)
        {
            return fromType.GetMethods(BindingFlags.Public | BindingFlags.Static);
        }


        public static Delegate BuildMinimalAPIDelegate(MethodInfo method)
        {
            var methodParams = method.GetParameters().Select(param => param.ParameterType).ToList();
            var returnType = method.ReturnType;

            Type delegateType;
            switch (methodParams.Count + 1)  // plus returnType
            {
                case 1:
                    if (returnType == typeof(void))
                        delegateType = typeof(Action);
                    else
                        delegateType = typeof(Func<>);
                    break;

                case 2:
                    delegateType = typeof(Func<,>);
                    break;

                case 3:
                    delegateType = typeof(Func<,,>);
                    break;

                case 4:
                    delegateType = typeof(Func<,,,>);
                    break;

                case 5:
                    delegateType = typeof(Func<,,,,>);
                    break;

                case 6:
                    delegateType = typeof(Func<,,,,,>);
                    break;

                case 7:
                    delegateType = typeof(Func<,,,,,,>);
                    break;

                case 8:
                    delegateType = typeof(Func<,,,,,,,>);
                    break;

                case 9:
                    delegateType = typeof(Func<,,,,,,,,>);
                    break;

                case 10:
                    delegateType = typeof(Func<,,,,,,,,,>);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (methodParams.Count > 0)
            {
                methodParams.Add(returnType);
                delegateType = delegateType.MakeGenericType(methodParams.ToArray());
            }

            return Delegate.CreateDelegate(delegateType, method);
        }

        public static ControllerExecutor Executor => _executor;

        public static void InitializeControllers()
        {
            var methods = GetControllerMethods();

            InitializeControllers(methods);
        }

        internal static void InitializeControllers(MethodInfo[] methods)
        {
            var endpoints = BuildEndpoints(methods);
            _executor = new ControllerExecutor(endpoints);
        }


        internal static EndpointInfo[] BuildEndpoints(IReadOnlyCollection<MethodInfo> methods)
        {
            //var availableServices = GetFastReportServices();

            List<EndpointInfo> endpoints = new List<EndpointInfo>();
            foreach (var method in methods)
            {
                var endpoint = BuildEndpoint(method);
                endpoints.AddRange(endpoint);
            }

            return endpoints.ToArray();
        }


        internal static IEnumerable<EndpointInfo> BuildEndpoint(MethodInfo method)
        {
            var httpMethodAttribute = method.GetCustomAttribute<HttpMethodAttribute>();
            string routeTemplate = httpMethodAttribute?.Template ?? method.GetCustomAttribute<RouteAttribute>()?.Template;

            if (routeTemplate == null)
                throw new Exception($"Method '{method.Name}' doesn't have a route template. Please, use RouteAttribute or Http[Get|Post|...]Attribute with route template");

            var parameters = new List<Func<HttpContext, object>>();
            var parameterInfos = method.GetParameters();
            for (int i = 0; i < parameterInfos.Length; i++)
            {
                var parameterInfo = parameterInfos[i];

                var paramType = parameterInfo.ParameterType;

                // if FromQuery/FromRoute/FromBody etc.
                if (TryAttributesSearching(parameters, parameterInfo, paramType))
                    continue;

                // if knownTypes:
                if (TryKnownTypesSearching(parameters, parameterInfo.Name, paramType))
                    continue;

                // if exist in serviceProvider
                parameters.Add((httpContext) => httpContext.RequestServices.GetService(paramType));
                continue;
            }

            var resultHandler = BuildResultHandler(method.ReturnType);

            var templateMatcher = new TemplateMatcher(TemplateParser.Parse(WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, routeTemplate).TrimStart('/')), new RouteValueDictionary());

            if(httpMethodAttribute != null)
                foreach(var httpMethod in httpMethodAttribute.HttpMethods)
                {
                    yield return new EndpointInfo(httpMethod,
                        templateMatcher,
                        method,
                        parameters.ToArray(),
                        resultHandler);
                }
            else
            {
                // if HttpMethodAttribute doesn't exists => it's GET
                yield return new EndpointInfo("GET",
                        templateMatcher,
                        method,
                        parameters.ToArray(),
                        resultHandler);
            }
        }

        private static Func<HttpContext, object, Task> BuildResultHandler(Type returnType)
        {
            PropertyInfo resultProperty;
            if(returnType.IsGenericType)
            {
                var generticType = returnType.GetGenericTypeDefinition();
                if (generticType == typeof(Task<>))
                {
                    resultProperty = returnType.GetProperty(nameof(Task<object>.Result));
                    return async (httpContext, methodResult) =>
                    {
                        await (Task)methodResult;
                        var result = resultProperty.GetValue(methodResult);
                        await BuilderTemplates.HandleResult(httpContext, result);
                    };
                }
                else if (generticType == typeof(ValueTask<>))
                {
                    resultProperty = returnType.GetProperty(nameof(ValueTask<object>.Result));
                    return async (httpContext, methodResult) =>
                    {
                        // TODO: NOT WORKING
                        await (ValueTask<object>)methodResult;
                        var result = resultProperty.GetValue(methodResult);
                        await BuilderTemplates.HandleResult(httpContext, result);
                    };
                }
            }
            else if (returnType == typeof(Task))
            {
                return BuilderTemplates.Task_AwaitAndHandleResult;
            }
            else if (returnType == typeof(ValueTask))
            {
                return BuilderTemplates.ValueTask_AwaitAndHandleResult;
            }

            return BuilderTemplates.HandleResult;
        }

        internal static class BuilderTemplates
        {
            private static readonly IResult _SuccessResult = Results.Ok();

            public static async Task Task_AwaitAndHandleResult(HttpContext httpContext, object methodResult)
            {
                await(Task)methodResult;
                await HandleResult(httpContext, null);
            }

            public static async Task ValueTask_AwaitAndHandleResult(HttpContext httpContext, object methodResult)
            {
                await (ValueTask)methodResult;
                await HandleResult(httpContext, null);
            }

            public static Task HandleResult(HttpContext httpContext, object result)
            {
                if (result == null)
                {
                    return _SuccessResult.ExecuteAsync(httpContext);
                }

                if (result is IResult iResult)
                {
                    return iResult.ExecuteAsync(httpContext);
                }
                else
                {
                    Debug.Assert(result.GetType() != typeof(IActionResult));

                    string content = JsonSerializer.Serialize(result);
                    var contentResult = Results.Content(content, "text/json");
                    return contentResult.ExecuteAsync(httpContext);
                }
            }
        }




        private static bool TryKnownTypesSearching(List<Func<HttpContext, object>> parameters, string paramName, Type paramType)
        {
            if (paramType == typeof(HttpContext))
            {
                parameters.Add(static (httpContext) => httpContext);
                return true;
            }
            else if (paramType == typeof(HttpRequest))
            {
                parameters.Add(static (httpContext) => httpContext.Request);
                return true;
            }
            else if (paramType == typeof(HttpResponse))
            {
                parameters.Add(static (httpContext) => httpContext.Response);
                return true;
            }
            else if (paramType == typeof(CancellationToken))
            {
                parameters.Add(static (httpContext) => httpContext.RequestAborted);
                return true;
            }
            else if (paramType == typeof(IServiceProvider))
            {
                parameters.Add(static (httpContext) => httpContext.RequestServices);
                return true;
            }
            // Special case: we check primitive types 
            else if (paramType.IsSimpleType())
            {
                parameters.Add((httpContext) =>
                {
                    var request = httpContext.Request;
                    if (request.Query.TryGetValue(paramName, out var queryValue))
                        return ParseToType(queryValue, paramType);

                    if (request.Headers.TryGetValue(paramName, out var headerValue))
                        return ParseToType(headerValue, paramType);

                    if (request.RouteValues.TryGetValue(paramName, out var routeValue))
                        return ParseToType(routeValue, paramType);

                    if (request.Form.TryGetValue(paramName, out var formValue))
                        return ParseToType(formValue, paramType);

                    return default;
                });
                return true;
            }

            return false;
        }

        private static bool TryAttributesSearching(List<Func<HttpContext, object>> parameters, ParameterInfo parameterInfo, Type paramType)
        {
            var attributes = parameterInfo.GetCustomAttributes(false);
            foreach (var attribute in attributes)
            {
                Func<HttpContext, object> func;
                if (attribute is FromQueryAttribute fromQuery)
                {
                    string name = fromQuery.Name ?? parameterInfo.Name;
                    if (paramType.IsSimpleType())
                        func = (httpContext) =>
                        {
                            var queryValue = httpContext.Request.Query[name].ToString();
                            return ParseToType(queryValue, paramType);
                        };
                    else
                        func = (httpContext) =>
                        {
                            return DeserializeFromCollection(httpContext.Request.Query, paramType);
                        };

                    parameters.Add(func);
                    return true;
                }
                else if (attribute is FromBodyAttribute)
                {
                    func = (httpContext) =>
                    {
                        using var reader = new StreamReader(httpContext.Request.Body);
                        var requestBody = reader.ReadToEndAsync().GetAwaiter().GetResult();

                        return JsonSerializer.Deserialize(requestBody, paramType, _serializerOptions);
                    };

                    parameters.Add(func);
                    return true;
                }
                else if (attribute is FromHeaderAttribute fromHeader)
                {
                    string name = fromHeader.Name ?? parameterInfo.Name;
                    if (paramType.IsSimpleType())
                        func = (httpContext) =>
                        {
                            var queryValue = httpContext.Request.Headers[name].ToString();
                            return ParseToType(queryValue, paramType);
                        };
                    else
                        func = (httpContext) =>
                        {
                            return DeserializeFromCollection(httpContext.Request.Headers, paramType);
                        };

                    parameters.Add(func);
                    return true;
                }
                else if (attribute is FromRouteAttribute fromRoute)
                {
                    string name = fromRoute.Name ?? parameterInfo.Name;
                    if (paramType.IsSimpleType())
                        func = (httpContext) =>
                        {
                            var queryValue = httpContext.Request.RouteValues[name].ToString();
                            return ParseToType(queryValue, paramType);
                        };
                    else
                        func = (httpContext) =>
                        {
                            return DeserializeFromCollection(httpContext.Request.RouteValues, paramType);
                        };

                    parameters.Add(func);
                    return true;
                }
                else if (attribute is FromServicesAttribute)
                {
                    parameters.Add((httpContext) =>
                    {
                        return httpContext.RequestServices.GetService(paramType);
                    });
                    return true;
                }
            }
            return false;
        }

        private static object DeserializeFromCollection(IDictionary<string, object> dictionary, Type paramType)
        {
            var result = Activator.CreateInstance(paramType);
            var properties = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var property in properties)
            {
                if (dictionary.TryGetValue(property.Name, out var value))
                {
                    var parsedValue = ParseToType(value.ToString(), property.PropertyType);
                    property.SetValue(result, parsedValue);
                }
            }

            return result;
        }


        private static object DeserializeFromCollection(IQueryCollection dictionary, Type paramType)
        {
            var result = Activator.CreateInstance(paramType);
            // TODO: test with init-only fields
            var properties = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var property in properties)
            {
                if (dictionary.TryGetValue(property.Name, out var value))
                {
                    var parsedValue = ParseToType(value.ToString(), property.PropertyType);
                    property.SetValue(result, parsedValue);
                }
            }

            return result;
        }

        private static object DeserializeFromCollection(IHeaderDictionary dictionary, Type paramType)
        {
            var result = Activator.CreateInstance(paramType);
            var properties = paramType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
            foreach (var property in properties)
            {
                if (dictionary.TryGetValue(property.Name, out var value))
                {
                    var parsedValue = ParseToType(value.ToString(), property.PropertyType);
                    property.SetValue(result, parsedValue);
                }
            }

            return result;
        }

        private static object ParseToType(string value, Type type)
        {
            object result;
            if (type.IsPrimitive || type == typeof(string))
            {
                result = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else // if (type.IsEnum)
            {
                result = Enum.Parse(type, value);
            }

            return result;
        }

        private static object ParseToType(object value, Type type)
        {
            object result;
            if (type.IsPrimitive || type == typeof(string))
            {
                result = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);
            }
            else // if (type.IsEnum)
            {
                result = Enum.Parse(type, value.ToString());
            }

            return result;
        }

        [DebuggerDisplay("{Method.Name} [{Parameters.Length}], {HttpMethod} {Route.Template}")]
        internal readonly struct EndpointInfo
        {
            public readonly string HttpMethod;

            public readonly TemplateMatcher Route;

            public readonly Func<HttpContext, object>[] Parameters;

            public readonly MethodInfo Method;

            public readonly Func<HttpContext, object, Task> Handler;

            public EndpointInfo(string httpMethod, TemplateMatcher route, MethodInfo method,
                Func<HttpContext, object>[] parameters,
                Func<HttpContext, object, Task> handler)
            {
                HttpMethod = httpMethod;
                Route = route;
                Method = method;
                Parameters = parameters;
                Handler = handler;
            }
        }

        [DebuggerDisplay("{Method.Name} [{Parameters.Length}], {HttpMethod} {Route.Template}")]
        internal readonly struct BuiltEndpointInfo
        {
            public readonly string HttpMethod;

            public readonly TemplateMatcher Route;

            public readonly Func<HttpContext, Task> Method;


            public BuiltEndpointInfo(string httpMethod, TemplateMatcher route, Func<HttpContext, Task> method)
            {
                HttpMethod = httpMethod;
                Route = route;
                Method = method;
            }
        }


        //private static Type[] GetFastReportServices()
        //{
        //    var types = typeof(ControllerBuilder).Assembly.GetTypes();
        //    var services = new List<Type>();
        //    foreach (var type in types)
        //    {
        //        if(type.IsInterface && type.IsPublic)
        //        {
        //            services.Add(type);
        //        }
        //    }
        //    return services.ToArray();
        //}
    }

    internal static class TypeExtensions
    {
        public static bool IsSimpleType(this Type type)
            => type.IsPrimitive || type.IsEnum || type == typeof(string);
    }
}
#endif
