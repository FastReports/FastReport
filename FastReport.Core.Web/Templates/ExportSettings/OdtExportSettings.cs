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

        internal string template_OdtExportSettings()
        {
            var localizationOdf = new OdfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--7"" data-target=""odt"">
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationOdf.OdtTitle}
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
        <label>{localizationOdf.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <button id=""OdtWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"" style=""padding-right: 40px"">
                Wysiwyg
            </button>
            <button id=""OdtPageBreaks"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                {localizationOdf.PageBreaks}
            </button>
        </div>
        <div class=""fr-popup-content-export-parameters-row"">
            <label style = ""margin-top: 10px; font-weight: normal; font-size: 11px;"">{localizationOdf.Compliance}</label>
            <select class=""custom-select"" style=""margin-left: 20px;"" {CreateEvent(JSEvents.CHANGE, "frActions", "OdtOnComplianceChangeFunc", "this")}>
                <option value=""None"" selected>ODF 1.0/1.1</option>
                <option value=""Odf1_2"">ODF 1.2</option>
            </select>
        </div>
    </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/odt-export.js""></script>" : "")}
     <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "PDFCompilanceFunc")} id=""okButton"">OK</button>
    </div>
</div>";
        }
    }
}
