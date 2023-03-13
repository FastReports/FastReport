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

namespace FastReport.Web.Controllers
{
    public sealed class ConnectionsController : ApiControllerBase
    {
        private readonly IConnectionsService _connectionsService;

        public ConnectionsController(IConnectionsService connectionsService)
        {
            _connectionsService = connectionsService;
        }

        public sealed class ConnectionsParams
        {
            public string ConnectionType { get; set; }
            public string ConnectionString { get; set; }
        }

        [HttpGet]
        [Route("/_fr/designer.getConnectionTypes")]
        public IActionResult GetConnectionTypes()
        {
            var response = _connectionsService.GetConnectionTypes();

            return new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "{" + String.Join(",", response.ToArray()) + "}",
            };
        }

        [HttpGet]
        [Route("/_fr/designer.getConnectionTables")]
        public IActionResult GetConnectionTables([FromQuery] ConnectionsParams query)
        {
            var response = _connectionsService.GetConnectionTables(query.ConnectionType, query.ConnectionString, out bool isError);

            return isError ? new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "text/plain",
                Content = response,
            }
            : new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = response
            };
        }

        [HttpPost]
        [Route("/_fr/designer.makeConnectionString")]
        public IActionResult MakeConnectionString(string connectionType)
        {
            var form = Request.Form;
            var response = _connectionsService.CreateConnectionStringJSON(connectionType, form, out bool isError);

            return isError ? new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "text/plain",
                Content = response,
            }
            : new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = response
            };  
        }

        [HttpGet]
        [Route("/_fr/designer.getConnectionStringProperties")]
        public IActionResult GetConnectionStringProperties([FromQuery] ConnectionsParams query)
        {
            var response = _connectionsService.GetConnectionStringPropertiesJSON(query.ConnectionType, query.ConnectionString, out bool isError);

            return isError ? new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.InternalServerError,
                ContentType = "text/plain",
                Content = response,
            }
            : new ContentResult()
            {
                StatusCode = (int)HttpStatusCode.OK,
                ContentType = "application/xml",
                Content = response
            };
        }
    }
}
