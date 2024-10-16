using FastReport.Data;
using FastReport.Utils;
using FastReport.Web.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using FastReport.Web.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace FastReport.Web.Controllers
{
    static partial class Controllers
    {
        public sealed class ConnectionsParams
        {
            public string ConnectionType { get; set; }

            public string ConnectionString { get; set; }
        }

        public sealed class ConnectionTablesRequestModel
        {
            public ConnectionsParams ConnectionsParams { get; set; }

            public List<CustomViewModel> CustomViews { get; set; }
        }

        [HttpGet("/designer.getConnectionTypes")]
        public static IResult GetConnectionTypes([FromQuery] string needSqlSupportInfo, IConnectionsService connectionsService)
        {
            var isNeedSqlSupport = bool.TryParse(needSqlSupportInfo, out var parsedBool) && parsedBool;

            var response = connectionsService.GetConnectionTypes(isNeedSqlSupport);

            return Results.Json(response);
        }

        [Obsolete]
        [HttpGet("/designer.getConnectionTables")]
        public static IResult GetConnectionTables([FromQuery] ConnectionsParams query,
            IConnectionsService connectionsService)
        {
            var request = new ConnectionTablesRequestModel
            {
                ConnectionsParams = query,
                CustomViews = new()
            };

            return GetConnectionTables(request, connectionsService);
        }

        [HttpPost("/designer.getConnectionTables")]
        public static IResult GetConnectionTables([FromBody] ConnectionTablesRequestModel request, IConnectionsService connectionsService)
        {
            try
            {
                var response = connectionsService.GetConnectionTables(request.ConnectionsParams.ConnectionType, request.ConnectionsParams.ConnectionString, request.CustomViews);

                return Results.Content(response, "application/xml");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpPost("/designer.makeConnectionString")]
        public static IResult MakeConnectionString(string connectionType,
            IConnectionsService connectionsService,
            HttpRequest request)
        {
            var form = request.Form;
            var response = connectionsService.CreateConnectionStringJSON(connectionType, form, out var isError);

            return isError
                ? Results.Content(response, "text/plain")
                : Results.Content(response, "application/xml");
        }


        [HttpPost("/designer.updateConnectionTable")]
        public static IResult UpdateConnectionTable([FromQuery] string reportId,
            [FromQuery] string connectionType,
            [FromBody] UpdateTableParams parameters,
            IReportService reportService,
            IConnectionsService connectionsService,
            HttpRequest request)
        {
            if (!IsAuthorized(request))
                return Results.Unauthorized();

            try
            {
                string response;

                if (parameters.ConnectionString.IsNullOrWhiteSpace())
                {
                    if (!reportService.TryFindWebReport(reportId, out var webReport))
                        return Results.NotFound();

                    response = connectionsService.GetUpdatedTableByReportId(webReport, parameters);
                }
                else
                {
                    response = connectionsService.GetUpdatedTableByConnectionString(parameters.ConnectionString,
                        connectionType, parameters);
                }

                return Results.Content(response, "application/xml");
            }
            catch (Exception ex)
            {
                return Results.BadRequest(ex.Message);
            }
        }

        [HttpGet("/designer.getParameterTypes")]
        public static IResult GetParameterTypes([FromQuery] string connectionType,
            IConnectionsService connectionsService)
        {
            var response = connectionsService.GetParameterTypes(connectionType, out string error);

            return string.IsNullOrEmpty(error) ?
                Results.Json(response)
                : Results.BadRequest(error);
        }

        [HttpGet("/designer.getConnectionStringProperties")]
        public static IResult GetConnectionStringProperties([FromQuery] ConnectionsParams query,
            IConnectionsService connectionsService)
        {
            var response = connectionsService.GetConnectionStringPropertiesJSON(query.ConnectionType, query.ConnectionString, out var isError);

            return isError
                ? Results.Content(response, "text/plain")
                : Results.Content(response, "application/xml");
        }
    }
}