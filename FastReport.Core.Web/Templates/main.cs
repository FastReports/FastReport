using System;
using FastReport.Web.Infrastructure;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_FR => $"fr{ID}";
        internal string template_ROUTE_BASE_PATH => WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RoutePathBaseRoot, FastReportGlobal.FastReportOptions.RouteBasePath);
        internal string template_export_url(string exportFormat) => $"{template_ROUTE_BASE_PATH}/preview.exportReport?reportId={ID}&exportFormat={exportFormat}";
        internal string templte_email_export_url => $"{template_ROUTE_BASE_PATH}/preview.sendEmail?reportId={ID}";
        internal string template_print_url(string printMode) => $"{template_ROUTE_BASE_PATH}/preview.printReport?reportId={ID}&printMode={printMode}";
        //string template_TOOLBAR_HEIGHT_FACTOR => 40px * ToolbarHeight;
        internal string GetResource(string resourceName) => ResourceLoader.GetContent(resourceName);
        internal string GerResourceBase64(string resourceName) => Convert.ToBase64String(ResourceLoader.GetBytes(resourceName));

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
        {GetResource("spinner.svg")}
    </div>

    {template_toolbar(renderBody)}

    {template_body(renderBody)}
     
{(needModal ? template_modalcontainer() : "")}
 ";
        }
    }
}