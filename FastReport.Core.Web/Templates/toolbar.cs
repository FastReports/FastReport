using System;
using System.Collections.Generic;
using System.Text;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_toolbar(string saveTxt = "Save", string reloadTxt = "Reload", string preparedTxt = "Prepared report",
            string printTxt = "Print", string printFromBrowserTxt = "Print from browser",
            string printFromPdf = "Print from PDF viewer", string zoomTxt = "Zoom", string firstPageTxt = "First page",
            string previousPageTxt = "Previous page", string currentPageTxt = "Current page",
            string nextPageTxt = "Next page", string lastPageTxt = "Last page", string totalPagesTxt = "Total Pages",
            string pdfTxt = "Adobe PDF", string excel2007Txt = "Microsoft Excel 2007", string word2007Txt = "Microsoft Word 2007",
            string pptxTxt = "Microsoft PowerPoint 2007", string txtTxt = "Text File/Matrix Printer", string rtfTxt = "Rich Text",
            string xpsTxt = "Microsoft XPS", string odsTxt = "OpenOffice Calc", string odtTxt = "OpenOffice Writer",
            string xmlTxt = "XML (Excel) table", string csvTxt = "CSV file", string dbfTxt = "DBF table", string mhtTxt = "MHT file")
        {
            if (!ShowToolbar)
                return "";

            var toolbarExportItem = $@"<div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("save.svg", "image/svg+xml")}"" title=""{saveTxt}"" style=""transform:translateY(1px)"">
        <div class=""{template_FR}-toolbar-dropdown-content"">" +
            (ShowPreparedReport ? $@"<a target=""_blank"" href=""{template_export_url("fpx")}"">{preparedTxt}</a>" : "")
#if !OPENSOURCE
            + (ShowPdfExport ? $@"<a target=""_blank"" href=""{template_export_url("pdf")}"">{pdfTxt}</a>" : "") +
            (ShowExcel2007Export ? $@"<a target=""_blank"" href=""{template_export_url("xlsx")}"">{excel2007Txt}</a>" : "") +
            (ShowWord2007Export ? $@"<a target=""_blank"" href=""{template_export_url("docx")}"">{word2007Txt}</a>" : "") +
            (ShowPowerPoint2007Export ? $@"<a target=""_blank"" href=""{template_export_url("pptx")}"">{pptxTxt}</a>" : "") +
            (ShowTextExport ? $@"<a target=""_blank"" href=""{template_export_url("txt")}"">{txtTxt}</a>" : "") +
            (ShowRtfExport ? $@"<a target=""_blank"" href=""{template_export_url("rtf")}"">{rtfTxt}</a>" : "") +
            (ShowXpsExport ? $@"<a target=""_blank"" href=""{template_export_url("xps")}"">{xpsTxt}</a>" : "") +
            (ShowOdsExport ? $@"<a target=""_blank"" href=""{template_export_url("ods")}"">{odsTxt}</a>" : "") +
            (ShowOdtExport ? $@"<a target=""_blank"" href=""{template_export_url("odt")}"">{odtTxt}</a>" : "") +
            (ShowXmlExcelExport ? $@"<a target=""_blank"" href=""{template_export_url("xml")}"">{xmlTxt}</a>" : "") +
            (ShowDbfExport ? $@"<a target=""_blank"" href=""{template_export_url("dbf")}"">{dbfTxt}</a>": "") +
            (ShowCsvExport ? $@"<a target=""_blank"" href=""{template_export_url("csv")}"">{csvTxt}</a>" : "") +
            (ShowMhtExport ? $@"<a target=""_blank"" href=""{template_export_url("mht")}"">{mhtTxt}</a>" : "")
#endif
            + "</div></div>"
            ;
            var toolbarPrintItem = $@" <div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("print.svg", "image/svg+xml")}"" title=""{printTxt}"">
        <div class=""{template_FR}-toolbar-dropdown-content"">
            " +
            (PrintInHtml ? $@"<a target=""_blank"" href=""{template_print_url("html")}"">{printFromBrowserTxt}</a>
            " : "") +
#if !OPENSOURCE
            (PrintInPdf ? $@"<a target=""_blank"" href=""{template_print_url("pdf")}"">{printFromPdf}</a>
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

{(ShowRefreshButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-pointer"" onclick=""{template_FR}.refresh();"" title=""{reloadTxt}"">
        <img src=""{template_resource_url("reload.svg", "image/svg+xml")}"">
    </div>" : "")}

{(ShowExports ? $"{toolbarExportItem}" : "")}


{(ShowPrint ? $"{toolbarPrintItem}" : "")}

{(ShowZoomButton ? $@"<div class=""{template_FR}-toolbar-item"">
        <img src=""{template_resource_url("magnifier.svg", "image/svg+xml")}"" title=""{zoomTxt}"" style=""transform:translateY(-1px)"">
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
 {(ShowFirstButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('first');""")} title=""{firstPageTxt}"">
        <img src=""{template_resource_url("angle-double-left.svg", "image/svg+xml")}"">
    </div>" : "")}
   
{(ShowPrevButton ? $@"<div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('prev');""")} title=""{previousPageTxt}"">
        <img src=""{template_resource_url("angle-left.svg", "image/svg+xml")}"">
    </div>" : "")}
   

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input class=""{template_FR}-current-page-input"" type=""text"" value=""{((CurrentPageIndex + 1) > TotalPages ? TotalPages : (CurrentPageIndex + 1))}"" onchange=""{template_FR}.goto(document.getElementsByClassName('{template_FR}-current-page-input')[0].value);"" title=""{currentPageTxt}"">
    </div>

    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton {template_FR}-toolbar-slash"">
        <img src=""{template_resource_url("slash.svg", "image/svg+xml")}"">
    </div>


    <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input type=""text"" value=""{TotalPages}"" readonly=""readonly"" title=""{totalPagesTxt}"">
    </div>

{(ShowNextButton ? $@" <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('next');""")} title=""{nextPageTxt}"">
        <img src=""{template_resource_url("angle-right.svg", "image/svg+xml")}"">
    </div>" : "")}
   
{(ShowLastButton ? $@"   <div class=""{template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('last');""")} title=""{lastPageTxt}"">
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
