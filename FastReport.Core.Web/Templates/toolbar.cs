using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_toolbar(bool renderBody)
        {
            if (!Toolbar.Show || !renderBody
#if !OPENSOURCE
                || (Mode == WebReportMode.Dialog && !Toolbar.ShowOnDialogPage)
#endif
                )
                return "";


            var localization = new ToolbarLocalization(Res);
            var exports = Toolbar.Exports;
            var toolbarExportItem = $@"<div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("save.svg", "image/svg+xml")}"" title=""{localization.saveTxt}"" style=""transform:translateY(1px)"">
        <div class=""{template_FR}-toolbar-dropdown-content"">" +
            (exports.ShowPreparedReport ? $@"<a target=""_blank"" href=""{template_export_url("fpx")}"">{localization.preparedTxt}</a>" : "")
#if !OPENSOURCE
            + (exports.ShowPdfExport ? $@"<a target=""_blank"" href=""{template_export_url("pdf")}"">{localization.pdfTxt}</a>" : "") +
            (exports.ShowExcel2007Export ? $@"<a target=""_blank"" href=""{template_export_url("xlsx")}"">{localization.excel2007Txt}</a>" : "") +
            (exports.ShowWord2007Export ? $@"<a target=""_blank"" href=""{template_export_url("docx")}"">{localization.word2007Txt}</a>" : "") +
            (exports.ShowPowerPoint2007Export ? $@"<a target=""_blank"" href=""{template_export_url("pptx")}"">{localization.powerPoint2007Txt}</a>" : "") +
            (exports.ShowTextExport ? $@"<a target=""_blank"" href=""{template_export_url("txt")}"">{localization.textTxt}</a>" : "") +
            (exports.ShowRtfExport ? $@"<a target=""_blank"" href=""{template_export_url("rtf")}"">{localization.rtfTxt}</a>" : "") +
            (exports.ShowXpsExport ? $@"<a target=""_blank"" href=""{template_export_url("xps")}"">{localization.xpsTxt}</a>" : "") +
            (exports.ShowOdsExport ? $@"<a target=""_blank"" href=""{template_export_url("ods")}"">{localization.odsTxt}</a>" : "") +
            (exports.ShowOdtExport ? $@"<a target=""_blank"" href=""{template_export_url("odt")}"">{localization.odtTxt}</a>" : "") +
            (exports.ShowXmlExcelExport ? $@"<a target=""_blank"" href=""{template_export_url("xml")}"">{localization.xmlTxt}</a>" : "") +
            (exports.ShowDbfExport ? $@"<a target=""_blank"" href=""{template_export_url("dbf")}"">{localization.dbfTxt}</a>": "") +
            (exports.ShowCsvExport ? $@"<a target=""_blank"" href=""{template_export_url("csv")}"">{localization.csvTxt}</a>" : "") +
            (exports.ShowSvgExport ? $@"<a target=""_blank"" href=""{template_export_url("svg")}"">{localization.svgTxt}</a>" : "") +
            (exports.ShowMhtExport ? $@"<a target=""_blank"" href=""{template_export_url("mht")}"">{localization.mhtTxt}</a>" : "") +
            (exports.ShowExcel97Export ? $@"<a target=""_blank"" href=""{ template_export_url("xls")}"">{localization.excel97Txt}</a>" :"") +
            //(exports.ShowEmailExport ? $@"<a target=""_blank"" href=""{ template_export_url("email")}"">{emailTxt}</a>" : "") +
            (exports.ShowHpglExport ? $@"<a target=""_blank"" href=""{ template_export_url("hpgl")}"">{localization.hpglTxt}</a>" : "") +
            (exports.ShowHTMLExport ? $@"<a target=""_blank"" href=""{ template_export_url("html")}"">{localization.htmlTxt}</a>" : "") +
            //(exports.ShowImageExport ? $@"<a target=""_blank"" href=""{ template_export_url("image")}"">{imageTxt}</a>" : "") +
            (exports.ShowJsonExport ? $@"<a target=""_blank"" href=""{ template_export_url("json")}"">{localization.jsonTxt}</a>" : "") +
             (exports.ShowDxfExport ? $@"<a target=""_blank"" href=""{ template_export_url("dxf")}"">{localization.dxfTxt}</a>" : "") +
            (exports.ShowLaTeXExport ? $@"<a target=""_blank"" href=""{ template_export_url("latex")}"">{localization.latexTxt}</a>" : "") +
            (exports.ShowPpmlExport ? $@"<a target=""_blank"" href=""{ template_export_url("ppml")}"">{localization.ppmlTxt}</a>" : "") +
            (exports.ShowPSExport ? $@"<a target=""_blank"" href=""{ template_export_url("ps")}"">{localization.psTxt}</a>" : "") +
            (exports.ShowXamlExport ? $@"<a target=""_blank"" href=""{ template_export_url("xaml")}"">{localization.xamlTxt}</a>" : "") +
            (exports.ShowZplExport ? $@"<a target=""_blank"" href=""{ template_export_url("zpl")}"">{localization.zplTxt}</a>" : "") 
#endif
             +"</div></div>"
            ;
            var toolbarPrintItem = $@" <div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("print.svg", "image/svg+xml")}"" title=""{localization.printTxt}"">
        <div class=""{template_FR}-toolbar-dropdown-content"">
            " +
            (Toolbar.PrintInHtml ? $@"<a target=""_blank"" href=""{template_print_url("html")}"">{localization.printFromBrowserTxt}</a>
            " : "") +
#if !OPENSOURCE
            (Toolbar.PrintInPdf ? $@"<a target=""_blank"" href=""{template_print_url("pdf")}"">{localization.printFromPdf}</a>
        " : "") +
#endif
        $@"</div>
    </div>";
            var currentZoom = Zoom * 100;
            var selectedZoom1 = $@"<div class=""{template_FR}-zoom-selected"">•</div>";
            var selectedZoom2 = $@"<div class=""{template_FR}-zoom-selected""></div>";
            var isFirstPage = CurrentPageIndex == 0;
            var isLastPage = CurrentPageIndex >= TotalPages - 1;

            string templateToolbar = $@"
<div class=""{template_FR}-toolbar"">

{(Toolbar.ShowRefreshButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-pointer"" onclick=""{template_FR}.refresh();"" title=""{localization.reloadTxt}"">
        <img src=""{template_resource_url("reload.svg", "image/svg+xml")}"">
    </div>" : "")}

{(Toolbar.Exports.Show ? $"{toolbarExportItem}" : "")}


{(Toolbar.ShowPrint ? $"{toolbarPrintItem}" : "")}

{(Toolbar.ShowZoomButton ? $@"<div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("magnifier.svg", "image/svg+xml")}"" title=""{localization.zoomTxt}"" style=""transform:translateY(-1px)"">
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
    </div>" : "")}
  

{((SinglePage || TotalPages < 2) ? "" : $@"
 {(Toolbar.ShowFirstButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('first');""")} title=""{localization.firstPageTxt}"">
        <img src=""{template_resource_url("angle-double-left.svg", "image/svg+xml")}"">
    </div>" : "")}
   
{(Toolbar.ShowPrevButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('prev');""")} title=""{localization.previousPageTxt}"">
        <img src=""{template_resource_url("angle-left.svg", "image/svg+xml")}"">
    </div>" : "")}
   

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input class=""{template_FR}-current-page-input"" type=""text"" value=""{((CurrentPageIndex + 1) > TotalPages ? TotalPages : (CurrentPageIndex + 1))}"" onchange=""{template_FR}.goto(document.getElementsByClassName('{template_FR}-current-page-input')[0].value);"" title=""{localization.currentPageTxt}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton {template_FR}-toolbar-slash"">
        <img src=""{template_resource_url("slash.svg", "image/svg+xml")}"">
    </div>


    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input type=""text"" value=""{TotalPages}"" readonly=""readonly"" title=""{localization.totalPagesTxt}"">
    </div>

{(Toolbar.ShowNextButton ? $@" <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('next');""")} title=""{localization.nextPageTxt}"">
        <img src=""{template_resource_url("angle-right.svg", "image/svg+xml")}"">
    </div>" : "")}
   
{(Toolbar.ShowLastButton ? $@"   <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('last');""")} title=""{localization.lastPageTxt}"">
        <img src=""{template_resource_url("angle-double-right.svg", "image/svg+xml")}"">
    </div>" : "")}
 
")}
</div>

{template_tabs()}

";
            return templateToolbar;
        }
    }
}
