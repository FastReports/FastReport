using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using FastReport.Web.Application.Localizations;

namespace FastReport.Web
{
    partial class WebReport
    {

        internal string template_XlsxExportSettings()
        {
            var localizationXlsx = new XlsxExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--2"" data-target=""xlsx"" >
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationXlsx.Title}
        </div>
        {(Report.PreparedPages.Count != 1 ? $@"<label class=""fr-popup-content-export-parameters-page-range-title"">{localizationPageSelector.PageRange}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-popup-content-export-parameters-button activeButton"" name=""OnAllClick"" {CreateOnClickEvent("frActions", "OnAllClick")}>
                {localizationPageSelector.All}
            </button>
        </div>
        <div class=""fr-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-popup-content-export-parameters-button"" name=""OnFirstClick"" {CreateOnClickEvent("frActions", "OnFirstClick")}>
                {localizationPageSelector.First}
            </button>
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" {CreateEvent(JSEvents.CHANGE, "frActions", "OnPageSelectorChange")} type=""text"" class=""fr-popup-content-export-parameters-input"" pattern=""\d+(\s*-\s*\d+)?"" placeholder=""2 or 10-20""value="""" >
</div>" : "")}
    </div>
    <div class=""fr-popup-content-export-parameters"">
        <label>{localizationXlsx.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""XlsxWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    Wysiwyg
                </button>
                <button id=""XlsxPageBreaks"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXlsx.PageBreaks}
                </button>
                <button id=""XlsxDataOnly"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXlsx.DataOnly}
                </button>
            </div>
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""XlsxSeamless"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXlsx.Seamless}
                </button>
                <button id=""XlsxPrintOptimized"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationXlsx.PrintOptimized}
                </button>
                <button id=""XlsxSplitPages"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXlsx.SplitPages}
                </button>
            </div>
          </div>
    </div>
    <div class=""fr-popup-content-export-parameters"" style=""padding-bottom: 0px;"">
    <label>{localizationXlsx.PrintScaling}</label>
        <div class=""fr-popup-content-export-parameters-col"">   
            <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "XlsxOnPrintFitChangeFunc", "this")}>
                <option value=""NoScaling"" selected>{localizationXlsx.NoScaling}</option>
                <option value=""FitSheetOnOnePage"">{localizationXlsx.FitSheetOnOnePage}</option>
                <option value=""FitAllColumsOnOnePage"">{localizationXlsx.FitAllColumsOnOnePage}</option>
                <option value=""FitAllRowsOnOnePage"">{localizationXlsx.FitAllRowsOnOnePage}</option>
            </select>
            <div class=""fr-popup-content-export-parameters-slider""  style=""display:flex; padding-left: 0.4rem;"">
                <p style=""font-weight: normal;"">{localizationXlsx.FontScale}</p>
                    <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "XlsxFontScalingFunc", "this")}>
                        <option value = ""0.1"">0.1</option>
                        <option value = ""0.5"">0.5</option>
                        <option value = ""1"" selected>1</option>
                        <option value = ""1.5"">1.5</option>
                        <option value = ""2"">2</option>
                        <option value = ""2.5"">2.5</option>
                        <option value = ""3"">3</option>
                        <option value = ""3.5"">3.5</option>
                        <option value = ""4"">4</option>
                        <option value = ""4.5"">4.5</option>
                        <option value = ""5"">5</option>
                    </select>
            </div>   
        </div>
    </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/xlsx-export.js""></script>" : "")}
    <div class=""fr-popup-content-buttons"" style = ""margin-top: 8px;"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "XLSXExport")} id=""okButton"">OK</button>
    </div>
</div>";
        }
    }
}
