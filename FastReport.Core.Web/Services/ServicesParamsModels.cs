using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace FastReport.Web.Services
{
    public class SaveReportResponseModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public bool IsError { get; set; }
    }

    public class SaveReportServiceParams
    {
        public Stream RequestBody { get; set; }
        public string Scheme { get; set; }
        public string Host { get; set; }
        public int? Port { get; set; }
        public IRequestCookieCollection Cookies { get; set; }

        public static SaveReportServiceParams ParseRequest(HttpRequest request)
        {
            var @params = new SaveReportServiceParams()
            {
                Scheme = request.Scheme,
                Host = request.Host.Host,
                Port = request.Host.Port,
                RequestBody = request.Body,
                Cookies = request.Cookies,
            };

            return @params;
        }
    }

    public sealed class CustomViewModel
    {
        public string TableName { get; set; }
        public string SqlQuery { get; set; }
        public List<ParameterModel> Parameters { get; set; }
    }

    public sealed class CreateConnectionParams 
    {
        public string Connection { get; set; }
        public string ConnectionName {  get; set; } 
    }

    public class UpdateTableParams
    {
        public string ConnectionString { get; set; }
        public string SqlQuery { get; set; }
        public List<ParameterModel> Parameters { get; set; }
        public string TableName { get; set; }
    }

    public sealed class ParameterModel
    {
        public string Name { get; set; }
        public int DataType { get; set; }
        public string Value { get; set; }
        public int Size { get; set; }
        public string Expression { get; set; }
    }

    #region GetReportServiceParams
    public class GetReportServiceParams
    {
        public ClickParams ClickParams { get; set; }
        public DialogParams DialogParams { get; set; }
        public ReportTabParams ReportTabParams { get; set; }
        public ReportPageParams ReportPageParams { get; set; }
        public ReportSearchParams ReportSearchParams { get; set; }
        public string RenderBody { get; set; }
        public string SkipPrepare { get; set; }
        public string ForceRefresh { get; set; }
        public string Zoom { get; set; }

        public static GetReportServiceParams ParseRequest(HttpRequest request)
        {
            var reportServiceParams = new GetReportServiceParams
            {
                SkipPrepare = request.Query["skipPrepare"].ToString(),
                ForceRefresh = request.Query["forceRefresh"].ToString(),
                RenderBody = request.Query["renderBody"].ToString(),
            };

            reportServiceParams.Zoom = request.Query["zoom"].ToString();

            reportServiceParams.ReportSearchParams = new ReportSearchParams();
            reportServiceParams.DialogParams = new DialogParams();

            reportServiceParams.ReportPageParams = new ReportPageParams
            {
                GoTo = request.Query["goto"].ToString(),
                Bookmark = request.Query["bookmark"].ToString(),
                DetailedReport = request.Query["detailed_report"].ToString(),
                DetailedPage = request.Query["detailed_page"].ToString()
            };

            reportServiceParams.ReportTabParams = new ReportTabParams
            {
                SetTab = request.Query["settab"].ToString(),
                CloseTab = request.Query["closetab"].ToString()
            };

            reportServiceParams.ClickParams = new ClickParams
            {
                Click = request.Query["click"].ToString(),
                CheckBoxClick = request.Query["checkbox_click"].ToString(),
                TextEdit = request.Query["text_edit"].ToString(),
                AdvMatrixClick = request.Query["advmatrix_click"].ToString()
            };

            if (!reportServiceParams.ClickParams.TextEdit.IsNullOrEmpty())
                reportServiceParams.ClickParams.Text = request.Form["text"].ToString();

            reportServiceParams.DialogParams.ParseRequest(request);

            reportServiceParams.ReportSearchParams.ParseRequest(request);

            return reportServiceParams;
        }
    }

    public class ClickParams
    {
        public string Click { get; set; }
        public string CheckBoxClick { get; set; }
        public string TextEdit { get; set; }
        public string Text { get; set; }
        public string AdvMatrixClick { get; set; }
    }

    public class DialogParams
    {
        public string DialogN { get; set; }
        public string DialogControlName { get; set; }
        public string DialogEventName { get; set; }
        public string DialogData { get; set; }

        public void ParseRequest(HttpRequest request)
        {
            DialogN = request.Query["dialog"].ToString();
            DialogControlName = request.Query["control"].ToString();
            DialogEventName = request.Query["event"].ToString();
            DialogData = request.Query["data"].ToString();
        }
    }

    public class ReportTabParams
    {
        public string SetTab { get; set; }
        public string CloseTab { get; set; }
    }

    public class ReportPageParams
    {
        public string GoTo { get; set; }
        public string Bookmark { get; set; }
        public string DetailedReport { get; set; }
        public string DetailedPage { get; set; }
    }

    public class ReportSearchParams
    {
        public string SearchText { get; set; }
        public string Backward { get; set; }
        public string MatchCase { get; set; }
        public string WholeWord { get; set; }

        public void ParseRequest(HttpRequest request)
        {
            SearchText = request.Query["searchText"].ToString();
            Backward = request.Query["backward"].ToString();
            MatchCase = request.Query["matchCase"].ToString();
            WholeWord = request.Query["wholeWord"].ToString();
        }
    }
    #endregion
}
