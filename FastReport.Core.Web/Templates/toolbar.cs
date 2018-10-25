using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_toolbar()
        {
            if (!ShowToolbar)
                return "";

            var currentZoom = Zoom * 100;
            var selectedZoom1 = $@"<div class=""{template_FR}-zoom-selected"">•</div>";
            var selectedZoom2 = $@"<div class=""{template_FR}-zoom-selected""></div>";
            var isFirstPage = CurrentPageIndex == 0;
            var isLastPage = CurrentPageIndex >= TotalPages - 1;

            string templateToolbar = $@"
<div class=""{template_FR}-toolbar"">

    <div class=""{template_FR}-toolbar-item {template_FR}-pointer"" onclick=""{template_FR}.refresh();"" title=""Reload"">
        <img src=""{template_resource_url("reload.svg", "image/svg+xml")}"">
    </div>

    <div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("save.svg", "image/svg+xml")}"" title=""Save"" style=""transform:translateY(1px)"">
        <div class=""{template_FR}-toolbar-dropdown-content"">
            <a target=""_blank"" href=""{template_export_url("fpx")}"">Prepared report</a>
            " +
#if  !OPENSOURCE
            $@"<a target=""_blank"" href=""{template_export_url("pdf")}"">Adobe PDF</a>
            <a target=""_blank"" href=""{template_export_url("xlsx")}"">Microsoft Excel 2007</a>
            <!-- <a target=""_blank"" href=""{template_export_url("docx")}"">Microsoft Word 2007</a> -->
            <!-- <a target=""_blank"" href=""{template_export_url("pptx")}"">Microsoft PowerPoint 2007</a> -->
            <a target=""_blank"" href=""{template_export_url("txt")}"">Text File/Matrix Printer</a>
            <a target=""_blank"" href=""{template_export_url("rtf")}"">Rich Text</a>
            <a target=""_blank"" href=""{template_export_url("xps")}"">Microsoft XPS</a>
            <a target=""_blank"" href=""{template_export_url("ods")}"">OpenOffice Calc</a>
            <a target=""_blank"" href=""{template_export_url("odt")}"">OpenOffice Writer</a>
            <a target=""_blank"" href=""{template_export_url("xml")}"">XML (Excel) table</a>
            <!-- <a target=""_blank"" href=""{template_export_url("dbf")}"">DBF table</a> -->
            <a target=""_blank"" href=""{template_export_url("csv")}"">CSV file</a>
        " +
#endif
        $@"</div>
    </div>

    <div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("print.svg", "image/svg+xml")}"" title=""Print"">
        <div class=""{template_FR}-toolbar-dropdown-content"">
            <a target=""_blank"" href=""{template_print_url("html")}"">Print from browser</a>
            " +
#if  !OPENSOURCE
            $@"<a target=""_blank"" href=""{template_print_url("pdf")}"">Print from PDF viewer</a>
        " +
#endif
        $@"</div>
    </div>

    <div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("magnifier.svg", "image/svg+xml")}"" title=""Zoom"" style=""transform:translateY(-1px)"">
        <div class=""{template_FR}-toolbar-dropdown-content"">
            <a onclick=""{template_FR}.zoom(300);"">{(currentZoom == 300 ? selectedZoom1 : selectedZoom2)}300%</a>
            <a onclick=""{template_FR}.zoom(200);"">{(currentZoom == 200 ? selectedZoom1 : selectedZoom2)}200%</a>
            <a onclick=""{template_FR}.zoom(150);"">{(currentZoom == 150 ? selectedZoom1 : selectedZoom2)}150%</a>
            <a onclick=""{template_FR}.zoom(100);"">{(currentZoom == 100 ? selectedZoom1 : selectedZoom2)}100%</a>
            <a onclick=""{template_FR}.zoom(90);"">{(currentZoom == 90 ? selectedZoom1 : selectedZoom2)}90%</a>
            <a onclick=""{template_FR}.zoom(75);"">{(currentZoom == 75 ? selectedZoom1 : selectedZoom2)}75%</a>
            <a onclick=""{template_FR}.zoom(50);"">{(currentZoom == 50 ? selectedZoom1 : selectedZoom2)}50%</a>
            <a onclick=""{template_FR}.zoom(25);"">{(currentZoom == 25 ? selectedZoom1 : selectedZoom2)}25%</a>
        </div>
    </div>

{((SinglePage || TotalPages < 2) ? "" : $@"

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('first');""")} title=""First page"">
        <img src=""{template_resource_url("angle-double-left.svg", "image/svg+xml")}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('prev');""")} title=""Previous page"">
        <img src=""{template_resource_url("angle-left.svg", "image/svg+xml")}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input class=""{template_FR}-current-page-input"" type=""text"" value=""{((CurrentPageIndex + 1) > TotalPages ? TotalPages : (CurrentPageIndex + 1))}"" onchange=""{template_FR}.goto(document.getElementsByClassName('{template_FR}-current-page-input')[0].value);"" title=""Current page"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton {template_FR}-toolbar-slash"">
        <img src=""{template_resource_url("slash.svg", "image/svg+xml")}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input type=""text"" value=""{TotalPages}"" readonly=""readonly"" title=""Total pages"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('next');""")} title=""Next page"">
        <img src=""{template_resource_url("angle-right.svg", "image/svg+xml")}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('last');""")} title=""Last page"">
        <img src=""{template_resource_url("angle-double-right.svg", "image/svg+xml")}"">
    </div>
")}
</div>

{template_tabs()}

";
            return templateToolbar;
        }
    }
}
