using FastReport.Web.Infrastructure;

using System.Net;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_FR => $"fr{ID}";
        internal string template_ROUTE_BASE_PATH => WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RoutePathBaseRoot, FastReportGlobal.FastReportOptions.RouteBasePath);
        string template_resource_url(string resourceName, string contentType) => $"{template_ROUTE_BASE_PATH}/resources.getResource?resourceName={WebUtility.UrlEncode(resourceName)}&contentType={WebUtility.UrlEncode(contentType)}";
        internal string template_export_url(string exportFormat) => $"{template_ROUTE_BASE_PATH}/preview.exportReport?reportId={ID}&exportFormat={exportFormat}";
        internal string templte_email_export_url => $"{template_ROUTE_BASE_PATH}/preview.sendEmail?reportId={ID}";
        internal string template_print_url(string printMode) => $"{template_ROUTE_BASE_PATH}/preview.printReport?reportId={ID}&printMode={printMode}";
        //string template_TOOLBAR_HEIGHT_FACTOR => 40px * ToolbarHeight;

        string template_render(bool renderBody)
        {
#if !OPENSOURCE
            var needModal = Toolbar.Exports.EnableSettings || Toolbar.Exports.ShowEmailExport;
#else
            var needModal = Toolbar.Exports.EnableSettings;
#endif

            return $@"
<div class=""{template_FR}-container"">
   
    <style>
        {template_style()}
    </style>

    <script>
        {template_script()}
    </script>
    <div class=""{template_FR}-spinner"" {(renderBody ? @"style=""display:none""" : "")}>
        <img src=""{template_resource_url("spinner.svg", "image/svg+xml")}"">
    </div>

    {template_toolbar(renderBody)}

    {template_body(renderBody)}
     
{(needModal ? template_modalcontainer() : "")}
 ";
        }
    }
}