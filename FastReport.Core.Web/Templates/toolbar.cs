using System;
using System.Collections.Generic;
using System.Linq;
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

            // disable export and print buttons in dialog mode
#if DIALOGS
            var disableExport = Mode == WebReportMode.Dialog ? "hidden" : "";
#else
            var disableExport = "";
#endif

            var showRefreshButton = Toolbar.ShowRefreshButton && !Report.IsLoadPrepared;
            var localization = new ToolbarLocalization(Res);
            var exports = Toolbar.Exports;
            var toolbarExportItem = $@"<div {disableExport} class=""fr-toolbar-item {template_FR}-toolbar-item"" title=""{localization.saveTxt}"">
        {GetResource("save.svg")}
        <div class=""fr-toolbar-dropdown-content {template_FR}-toolbar-dropdown-content"">"
          + (exports.ShowPreparedReport ? $@"<a target=""_blank"" href=""{template_export_url("fpx")}"">{localization.preparedTxt}</a>" : "")
#if !OPENSOURCE
          + (exports.ShowPdfExport ? $@"<a id=""PdfExport"" target=""_blank"" href=""{template_export_url("pdf")}"">{localization.pdfTxt}</a>":"")
          + (exports.EnableSettings  && exports.ShowPdfExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""pdf"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowExcel2007Export ? $@"<a id=""XlsxExport"" target =""_blank"" href=""{template_export_url("xlsx")}"">{localization.excel2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowExcel2007Export ? $@"<button class=""fr-webreport-settings-btn"" data-path=""xlsx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowWord2007Export ? $@"<a id=""DocxExport"" target=""_blank"" href=""{template_export_url("docx")}"">{localization.word2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowWord2007Export ? $@"<button class=""fr-webreport-settings-btn"" data-path=""docx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowPowerPoint2007Export ? $@"<a id=""PptxExport"" target=""_blank"" href=""{template_export_url("pptx")}"">{localization.powerPoint2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowPowerPoint2007Export ? $@"<button class=""fr-webreport-settings-btn"" data-path=""pptx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowTextExport ? $@"<a target=""_blank"" href=""{template_export_url("txt")}"">{localization.textTxt}</a>" : "")
          + (exports.ShowRtfExport ? $@"<a id=""RtfExport"" target=""_blank"" href=""{template_export_url("rtf")}"">{localization.rtfTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowRtfExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""rtf"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowXpsExport ? $@"<a target=""_blank"" href=""{template_export_url("xps")}"">{localization.xpsTxt}</a>" : "")
          + (exports.ShowOdsExport ? $@"<a id=""OdsExport"" target=""_blank"" href=""{template_export_url("ods")}"">{localization.odsTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowOdsExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""ods"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowOdtExport ? $@"<a id=""OdtExport"" target=""_blank"" href=""{template_export_url("odt")}"">{localization.odtTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowOdtExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""odt"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowXmlExcelExport ? $@"<a id=""XmlExport"" target=""_blank"" href=""{template_export_url("xml")}"">{localization.xmlTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowXmlExcelExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""xml"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowDbfExport ? $@"<a target=""_blank"" href=""{template_export_url("dbf")}"">{localization.dbfTxt}</a>" : "")
          + (exports.ShowCsvExport ? $@"<a target=""_blank"" href=""{template_export_url("csv")}"">{localization.csvTxt}</a>" : "")
          + (exports.ShowSvgExport ? $@"<a id=""SvgExport"" target=""_blank"" href=""{template_export_url("svg")}"">{localization.svgTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowSvgExport? $@"<button class=""fr-webreport-settings-btn"" data-path=""svg"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowMhtExport ? $@"<a target=""_blank"" href=""{template_export_url("mht")}"">{localization.mhtTxt}</a>" : "")
          + (exports.ShowExcel97Export ? $@"<a target=""_blank"" href=""{ template_export_url("xls")}"">{localization.excel97Txt}</a>" : "")
          + (exports.ShowEmailExport ? $@"<a id=""EmailExport"" target=""_blank"" onclick=""{template_FR}.showEmailExportModal();"">{localization.emailTxt}</a>" : "")
          + (exports.ShowHpglExport ? $@"<a target=""_blank"" href=""{ template_export_url("hpgl")}"">{localization.hpglTxt}</a>" : "")
          + (exports.ShowHTMLExport ? $@"<a id=""HtmlExport"" target=""_blank"" href=""{template_export_url("html")}"">{localization.htmlTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowHTMLExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""html"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowImageExport? $@"<a id=""ImageExport"" target=""_blank"" href=""{template_export_url("image")}"">{localization.imageTxt}</a>" : "") + (exports.EnableSettings && exports.ShowImageExport ? $@"<button class=""fr-webreport-settings-btn"" data-path=""image"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowJsonExport ? $@"<a target=""_blank"" href=""{ template_export_url("json")}"">{localization.jsonTxt}</a>" : "")
          + (exports.ShowDxfExport ? $@"<a target=""_blank"" href=""{ template_export_url("dxf")}"">{localization.dxfTxt}</a>" : "")
          + (exports.ShowLaTeXExport ? $@"<a target=""_blank"" href=""{ template_export_url("latex")}"">{localization.latexTxt}</a>" : "")
          + (exports.ShowPpmlExport ? $@"<a target=""_blank"" href=""{ template_export_url("ppml")}"">{localization.ppmlTxt}</a>" : "")
          + (exports.ShowPSExport ? $@"<a target=""_blank"" href=""{ template_export_url("ps")}"">{localization.psTxt}</a>" : "") 
          + (exports.ShowXamlExport ? $@"<a target=""_blank"" href=""{ template_export_url("xaml")}"">{localization.xamlTxt}</a>" : "") 
          + (exports.ShowZplExport ? $@"<a target=""_blank"" href=""{ template_export_url("zpl")}"">{localization.zplTxt}</a>" : "")
#endif
             + "</div></div>"
            ;
            var toolbarPrintItem = $@" <div {disableExport} class=""fr-toolbar-item {template_FR}-toolbar-item"" title=""{localization.printTxt}"">
        {GetResource("print.svg")}
        <div class=""fr-toolbar-dropdown-content {template_FR}-toolbar-dropdown-content"">
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
            var selectedZoom1 = $@"<div class=""fr-toolbar-zoom-selected {template_FR}-zoom-selected"">";
            var selectedZoom2 = $@"<div>";
            var isFirstPage = CurrentPageIndex == 0;
            var isLastPage = CurrentPageIndex >= TotalPages - 1;
            var isSinglePage = SinglePage || TotalPages < 2;
            var customButtons = string.Join("", Toolbar.Elements.Select(x => x.Render(template_FR)));

            string templateToolbar = $@"
<div class=""fr-toolbar {template_FR}-toolbar"">
     
{(showRefreshButton ? $@"<div class=""fr-toolbar-item fr-toolbar-pointer {template_FR}-toolbar-item {template_FR}-pointer"" onclick=""{template_FR}.refresh();"" title=""{localization.reloadTxt}"">
        {GetResource("reload.svg")}
    </div>" : "")}

{(exports.Show ? $"{toolbarExportItem}" : "")}


{(Toolbar.ShowPrint ? $"{toolbarPrintItem}" : "")}
{(Toolbar.ShowSearchButton ? $@"<div class=""fr-toolbar-item fr-toolbar-pointer {template_FR}-toolbar-item {template_FR}-pointer"" title=""{localization.searchTxt}"" onclick=""{template_FR}.toggleSearchForm();"">
            {GetResource("magnifier-search.svg")}
        </div>
        <div class=""{template_FR}-toolbar-search-form"" id=""{template_FR}-toolbar-search-form"">
            <div id=""close-search-form-button"" onclick=""{template_FR}.toggleSearchForm();""><img src=""data:image/svg+xml;base64,{GerResourceBase64("close.svg")}"" /></div>
            <div class=""{template_FR}-toolbar-dropdown-content-searchbox"" >
                <input type=""text"" id=""fr-search-text"" placeholder=""{localization.searchPlaceholder}"" name=""SearchText"" oninput=""{template_FR}.onEnterSearchText();"" />
                <input hidden onclick=""{template_FR}.clearSearchText();"" id=""clear-searchbox"" type=""image"" src=""data:image/svg+xml;base64,{GerResourceBase64("clear_searchbox.svg")}"" />
            </div>
            <div class=""search-navigation-info-block"">
                <p id=""fr-WebRepot-text-info"">
                    
                </p>
                <div style=""margin-left:auto;"">
                    <button disabled title=""{localization.searchPrev}"" onclick=""{template_FR}.search(true);"" id=""fr-search-prev"">
                        {GetResource("angle-left.svg")}
                    </button>
                    <button disabled title=""{localization.searchNext}"" onclick=""{template_FR}.search(false);"" id=""fr-search-next"">
                        {GetResource("angle-right.svg")}
                    </button>
                </div>
            </div>
            <div>
                <label>
                    <input disabled type=""checkbox"" id=""fr-match-case"" name=""MatchCase"" />
                    {localization.matchCase}
                </label>
            </div>
            <div>
                <label>
                    <input disabled type=""checkbox"" id=""fr-whole-word"" name=""WholeWord"" />
                    {localization.wholeWord}
                </label>
            </div>
        </div>" : "")}
{(Toolbar.ShowZoomButton ? $@"<div class=""fr-toolbar-item {template_FR}-toolbar-item"" title=""{localization.zoomTxt}"">
        {GetResource("magnifier.svg")}
        <div class=""fr-toolbar-item-dropdown-content {template_FR}-toolbar-dropdown-content"">
            <a onclick=""{template_FR}.zoom(300);"">{(currentZoom == 300 ? selectedZoom1 : selectedZoom2)}300%</div></a>
            <a onclick=""{template_FR}.zoom(200);"">{(currentZoom == 200 ? selectedZoom1 : selectedZoom2)}200%</div></a>
            <a onclick=""{template_FR}.zoom(150);"">{(currentZoom == 150 ? selectedZoom1 : selectedZoom2)}150%</div></a>
            <a onclick=""{template_FR}.zoom(100);"">{(currentZoom == 100 ? selectedZoom1 : selectedZoom2)}100%</div></a>
            <a onclick=""{template_FR}.zoom(90);"">{(currentZoom == 90 ? selectedZoom1 : selectedZoom2)}90%</div></a>
            <a onclick=""{template_FR}.zoom(75);"">{(currentZoom == 75 ? selectedZoom1 : selectedZoom2)}75%</div></a>
            <a onclick=""{template_FR}.zoom(50);"">{(currentZoom == 50 ? selectedZoom1 : selectedZoom2)}50%</div></a>
            <a onclick=""{template_FR}.zoom(25);"">{(currentZoom == 25 ? selectedZoom1 : selectedZoom2)}25%</div></a>
        </div>
    </div>" : "")}" + $@"
 {(Toolbar.ShowFirstButton ? $@"<div class=""fr-toolbar-item fr-toolbar-narrow {template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('first');""")} title=""{localization.firstPageTxt}"">
        {GetResource("angle-double-left.svg")}
    </div>" : "")}
   
{(Toolbar.ShowPrevButton ? $@"<div class=""fr-toolbar-item fr-toolbar-narrow {template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isFirstPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isFirstPage ? "" : $@"onclick=""{template_FR}.goto('prev');""")} title=""{localization.previousPageTxt}"">
        {GetResource("angle-left.svg")}
    </div>" : "")}
   
    <div class=""fr-toolbar-item fr-toolbar-notbutton {template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input id=""CurrentPage"" style=""{(Toolbar.Position == Positions.Top && Toolbar.Position == Positions.Bottom ? "margin-left: 0px;" : "")} {(isSinglePage ? $@"opacity: 0.5" : "")}"" class=""{template_FR}-current-page-input"" {(isSinglePage ? $@"readonly=""readonly""" : "")} type=""text"" value=""{((CurrentPageIndex + 1) > TotalPages ? TotalPages : (CurrentPageIndex + 1))}"" onchange=""{template_FR}.goto(document.getElementsByClassName('{template_FR}-current-page-input')[0].value);"" title=""{localization.currentPageTxt}"">
    </div>

    <div class=""fr-toolbar-item fr-toolbar-notbutton fr-toolbar-slash {template_FR}-toolbar-item {template_FR}-toolbar-notbutton {template_FR}-toolbar-slash"" style=""{(isSinglePage ? $@"opacity: 0.5" : "")}"">
        {GetResource("slash.svg")}
    </div>

    <div class=""fr-toolbar-item fr-toolbar-notbutton {template_FR}-toolbar-item {template_FR}-toolbar-notbutton"">
        <input id=""AllPages"" style=""{(Toolbar.Position == Positions.Top && Toolbar.Position == Positions.Bottom ? "margin-left: 0px;" : "")} {(isSinglePage ? $@"opacity: 0.5" : "")}"" type=""text"" value=""{TotalPages}"" readonly=""readonly"" title=""{localization.totalPagesTxt}"">
    </div>

{(Toolbar.ShowNextButton ? $@" <div class=""fr-toolbar-item fr-toolbar-narrow {template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('next');""")} title=""{localization.nextPageTxt}"">
        {GetResource("angle-right.svg")}
    </div>" : "")}
   
{(Toolbar.ShowLastButton ? $@" <div class=""fr-toolbar-item fr-toolbar-narrow {template_FR}-toolbar-item {template_FR}-toolbar-narrow {(isLastPage ? $"{template_FR}-toolbar-notbutton {template_FR}-disabled" : $"{template_FR}-pointer")}"" {(isLastPage ? "" : $@"onclick=""{template_FR}.goto('last');""")} title=""{localization.lastPageTxt}"">
        {GetResource("angle-double-right.svg")}
    </div>" : "")}
{customButtons}
</div>

{template_tabs()}
";
            return templateToolbar;
        }
    }
}
