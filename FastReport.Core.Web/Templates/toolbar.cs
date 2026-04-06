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
            var target = $"target=\"{(!exports.ExportInNewTab ? "_self" : "_blank")}\"";
            var toolbarExportItem = $@"<div {disableExport} class=""fr-toolbar-item fr-toolbar-item"" title=""{localization.saveTxt}"">
        {GetResource("save.svg")}
        <div class=""fr-toolbar-dropdown-content fr-toolbar-dropdown-content"">"
          + (exports.ShowPreparedReport ? $@"<a {target} href=""{template_export_url("fpx")}"">{localization.preparedTxt}</a>" : "")
#if !OPENSOURCE
          + (exports.ShowPdfExport ? $@"<a id=""PdfExport"" {target} href=""{template_export_url("pdf")}"">{localization.pdfTxt}</a>":"")
          + (exports.EnableSettings  && exports.ShowPdfExport ? $@"<button class=""fr-settings-btn"" data-path=""pdf"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowExcel2007Export ? $@"<a id=""XlsxExport"" {target} href=""{template_export_url("xlsx")}"">{localization.excel2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowExcel2007Export ? $@"<button class=""fr-settings-btn"" data-path=""xlsx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowWord2007Export ? $@"<a id=""DocxExport"" {target} href=""{template_export_url("docx")}"">{localization.word2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowWord2007Export ? $@"<button class=""fr-settings-btn"" data-path=""docx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowPowerPoint2007Export ? $@"<a id=""PptxExport"" {target} href=""{template_export_url("pptx")}"">{localization.powerPoint2007Txt}</a>" : "")
          + (exports.EnableSettings && exports.ShowPowerPoint2007Export ? $@"<button class=""fr-settings-btn"" data-path=""pptx"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowTextExport ? $@"<a {target} href=""{template_export_url("txt")}"">{localization.textTxt}</a>" : "")
          + (exports.ShowRtfExport ? $@"<a id=""RtfExport"" {target} href=""{template_export_url("rtf")}"">{localization.rtfTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowRtfExport ? $@"<button class=""fr-settings-btn"" data-path=""rtf"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowXpsExport ? $@"<a {target} href=""{template_export_url("xps")}"">{localization.xpsTxt}</a>" : "")
          + (exports.ShowOdsExport ? $@"<a id=""OdsExport"" {target} href=""{template_export_url("ods")}"">{localization.odsTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowOdsExport ? $@"<button class=""fr-settings-btn"" data-path=""ods"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowOdtExport ? $@"<a id=""OdtExport"" {target} href=""{template_export_url("odt")}"">{localization.odtTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowOdtExport ? $@"<button class=""fr-settings-btn"" data-path=""odt"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowXmlExcelExport ? $@"<a id=""XmlExport"" {target} href=""{template_export_url("xml")}"">{localization.xmlTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowXmlExcelExport ? $@"<button class=""fr-settings-btn"" data-path=""xml"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowDbfExport ? $@"<a {target} href=""{template_export_url("dbf")}"">{localization.dbfTxt}</a>" : "")
          + (exports.ShowCsvExport ? $@"<a {target} href=""{template_export_url("csv")}"">{localization.csvTxt}</a>" : "")
          + (exports.ShowSvgExport ? $@"<a id=""SvgExport"" {target} href=""{template_export_url("svg")}"">{localization.svgTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowSvgExport? $@"<button class=""fr-settings-btn"" data-path=""svg"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowMhtExport ? $@"<a {target} href=""{template_export_url("mht")}"">{localization.mhtTxt}</a>" : "")
          + (exports.ShowExcel97Export ? $@"<a {target} href=""{ template_export_url("xls")}"">{localization.excel97Txt}</a>" : "")
          + (exports.ShowEmailExport ? $@"<a id=""EmailExport"" {target} {CreateOnClickEvent(ScriptName, "showEmailExportModal")}>{localization.emailTxt}</a>" : "")
          + (exports.ShowHpglExport ? $@"<a {target} href=""{ template_export_url("hpgl")}"">{localization.hpglTxt}</a>" : "")
          + (exports.ShowHTMLExport ? $@"<a id=""HtmlExport"" {target} href=""{template_export_url("html")}"">{localization.htmlTxt}</a>" : "")
          + (exports.EnableSettings && exports.ShowHTMLExport ? $@"<button class=""fr-settings-btn"" data-path=""html"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowImageExport? $@"<a id=""ImageExport"" {target} href=""{template_export_url("image")}"">{localization.imageTxt}</a>" : "") + (exports.EnableSettings && exports.ShowImageExport ? $@"<button class=""fr-settings-btn"" data-path=""image"">{GetResource("settings.svg")}</button>" : "")
          + (exports.ShowJsonExport ? $@"<a {target} href=""{ template_export_url("json")}"">{localization.jsonTxt}</a>" : "")
          + (exports.ShowDxfExport ? $@"<a {target} href=""{ template_export_url("dxf")}"">{localization.dxfTxt}</a>" : "")
          + (exports.ShowLaTeXExport ? $@"<a {target} href=""{ template_export_url("latex")}"">{localization.latexTxt}</a>" : "")
          + (exports.ShowPpmlExport ? $@"<a {target} href=""{ template_export_url("ppml")}"">{localization.ppmlTxt}</a>" : "")
          + (exports.ShowPSExport ? $@"<a {target} href=""{ template_export_url("ps")}"">{localization.psTxt}</a>" : "") 
          + (exports.ShowXamlExport ? $@"<a {target} href=""{ template_export_url("xaml")}"">{localization.xamlTxt}</a>" : "") 
          + (exports.ShowZplExport ? $@"<a {target} href=""{ template_export_url("zpl")}"">{localization.zplTxt}</a>" : "")
#endif
             + " </div></div>"
            ;
            var toolbarPrintItem = $@" <div {disableExport} class=""fr-toolbar-item fr-toolbar-item"" title=""{localization.printTxt}"">
        {GetResource("print.svg")}
        <div class=""fr-toolbar-dropdown-content fr-toolbar-dropdown-content"">
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
            var selectedZoom1 = $@"<div class=""fr-toolbar-zoom-selected fr-zoom-selected"">";
            var selectedZoom2 = $@"<div>";
            var isFirstPage = CurrentPageIndex == 0;
            var isLastPage = CurrentPageIndex >= TotalPages - 1;
            var isSinglePage = SinglePage || TotalPages < 2;
            var customButtons = string.Join("", Toolbar.Elements.Select(x => x.Render(ScriptName)));

            string templateToolbar = $@"
<div class=""fr-toolbar fr-toolbar"">
     
{(showRefreshButton ? $@"<div class=""fr-toolbar-item fr-toolbar-pointer fr-toolbar-item fr-pointer"" {CreateOnClickEvent(ScriptName, "refresh")} title=""{localization.reloadTxt}"">
        {GetResource("reload.svg")}
    </div>" : "")}

{(exports.Show ? $"{toolbarExportItem}" : "")}


{(Toolbar.ShowPrint ? $"{toolbarPrintItem}" : "")}
{(Toolbar.ShowSearchButton ? $@"<div class=""fr-toolbar-item fr-toolbar-pointer fr-toolbar-item fr-pointer"" title=""{localization.searchTxt}"" {CreateOnClickEvent($"{ScriptName}.Searcher", "toggleSearchForm")}>
            {GetResource("magnifier-search.svg")}
        </div>
        <div class=""fr-toolbar-search-form"" id=""fr-toolbar-search-form"">
            <div id=""close-search-form-button"" {CreateOnClickEvent($"{ScriptName}.Searcher", "toggleSearchForm")}><img src=""data:image/svg+xml;base64,{GerResourceBase64("close.svg")}"" /></div>
            <div class=""fr-toolbar-dropdown-content-searchbox"" >
                <input type=""text"" id=""fr-search-text"" placeholder=""{localization.searchPlaceholder}"" name=""SearchText"" {CreateEvent(JSEvents.INPUT, $"{ScriptName}.Searcher", "onEnterSearchText")}"" />
                <input hidden {CreateOnClickEvent($"{ScriptName}.Searcher", "clearSearchText")} id=""clear-searchbox"" type=""image"" src=""data:image/svg+xml;base64,{GerResourceBase64("clear_searchbox.svg")}"" />
            </div>
            <div class=""search-navigation-info-block"">
                <p id=""fr-searchform-text-info"">
                    
                </p>
                <div style=""margin-left:auto;"">
                    <button disabled title=""{localization.searchPrev}"" {CreateOnClickEvent($"{ScriptName}.Searcher", "search", "true", $"'{localization.searchNotFound}'")} id=""fr-search-prev"">
                        {GetResource("angle-left.svg")}
                    </button>
                    <button disabled title=""{localization.searchNext}"" {CreateOnClickEvent($"{ScriptName}.Searcher", "search", "false", $"'{localization.searchNotFound}'")} id=""fr-search-next"">
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
{(Toolbar.ShowZoomButton ? $@"<div class=""fr-toolbar-item fr-toolbar-item"" title=""{localization.zoomTxt}"">
        {GetResource("magnifier.svg")}
        <div class=""fr-toolbar-item-dropdown-content fr-toolbar-dropdown-content"">
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "300")}>{(currentZoom == 300 ? selectedZoom1 : selectedZoom2)}300%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "200")}>{(currentZoom == 200 ? selectedZoom1 : selectedZoom2)}200%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "150")}>{(currentZoom == 150 ? selectedZoom1 : selectedZoom2)}150%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "100")}>{(currentZoom == 100 ? selectedZoom1 : selectedZoom2)}100%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "90")}>{(currentZoom == 90 ? selectedZoom1 : selectedZoom2)}90%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "75")}>{(currentZoom == 75 ? selectedZoom1 : selectedZoom2)}75%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "50")}>{(currentZoom == 50 ? selectedZoom1 : selectedZoom2)}50%</div></a>
            <a {CreateOnClickEvent($"{ScriptName}", "zoom", "25")}>{(currentZoom == 25 ? selectedZoom1 : selectedZoom2)}25%</div></a>
        </div>
    </div>" : "")}" + $@"
 {(Toolbar.ShowFirstButton ? $@"<div class=""fr-toolbar-item fr-toolbar-narrow v-toolbar-item fr-toolbar-narrow {(isFirstPage ? $"fr-toolbar-notbutton fr-disabled" : $"fr-pointer")}"" {(isFirstPage ? "" : CreateOnClickEvent(ScriptName, "goto", "first"))} title=""{localization.firstPageTxt}"">
        {GetResource("angle-double-left.svg")}
    </div>" : "")}
   
{(Toolbar.ShowPrevButton ? $@"<div class=""fr-toolbar-item fr-toolbar-narrow fr-toolbar-item fr-toolbar-narrow {(isFirstPage ? $"fr-toolbar-notbutton fr-disabled" : $"fr-pointer")}"" {(isFirstPage ? "" : CreateOnClickEvent(ScriptName, "goto", "prev"))} title=""{localization.previousPageTxt}"">
        {GetResource("angle-left.svg")}
    </div>" : "")}
   
    <div class=""fr-toolbar-item fr-toolbar-notbutton fr-toolbar-item fr-toolbar-notbutton"">
        <input id=""CurrentPage"" style=""{(Toolbar.Position == Positions.Top && Toolbar.Position == Positions.Bottom ? "margin-left: 0px;" : "")} {(isSinglePage ? $@"opacity: 0.5" : "")}"" class=""fr-current-page-input"" {(isSinglePage ? $@"readonly=""readonly""" : "")} type=""text"" value=""{((CurrentPageIndex + 1) > TotalPages ? TotalPages : (CurrentPageIndex + 1))}"" {CreateEvent(JSEvents.CHANGE, ScriptName, "goto", "this.value")} title=""{localization.currentPageTxt}"">
    </div>

    <div class=""fr-toolbar-item fr-toolbar-notbutton fr-toolbar-slash fr-toolbar-item fr-toolbar-notbutton fr-toolbar-slash"" style=""{(isSinglePage ? $@"opacity: 0.5" : "")}"">
        {GetResource("slash.svg")}
    </div>

    <div class=""fr-toolbar-item fr-toolbar-notbutton fr-toolbar-item fr-toolbar-notbutton"">
        <input id=""AllPages"" style=""{(Toolbar.Position == Positions.Top && Toolbar.Position == Positions.Bottom ? "margin-left: 0px;" : "")} {(isSinglePage ? $@"opacity: 0.5" : "")}"" type=""text"" value=""{TotalPages}"" readonly=""readonly"" title=""{localization.totalPagesTxt}"">
    </div>

{(Toolbar.ShowNextButton ? $@" <div class=""fr-toolbar-item fr-toolbar-narrow fr-toolbar-item fr-toolbar-narrow {(isLastPage ? $"fr-toolbar-notbutton fr-disabled" : $"fr-pointer")}"" {(isLastPage ? "" : CreateOnClickEvent(ScriptName, "goto", "next"))} title=""{localization.nextPageTxt}"">
        {GetResource("angle-right.svg")}
    </div>" : "")}
   
{(Toolbar.ShowLastButton ? $@" <div class=""fr-toolbar-item fr-toolbar-narrow fr-toolbar-item fr-toolbar-narrow {(isLastPage ? $"fr-toolbar-notbutton fr-disabled" : $"fr-pointer")}"" {(isLastPage ? "" : CreateOnClickEvent(ScriptName, "goto", "last"))} title=""{localization.lastPageTxt}"">
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
