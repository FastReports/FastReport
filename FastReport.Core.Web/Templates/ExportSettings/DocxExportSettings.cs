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

       
        internal string template_DocxExportSettings()
        {
            var localizationDocx = new DocxExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--3"" data-target=""docx"">
    <div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationDocx.Title}
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
            <input name =""PageSelectorInput"" id=""PageSelector"" style=""margin-top: 2px;""  {CreateEvent(JSEvents.CHANGE, "frActions", "OnPageSelectorChange")} type=""text"" class=""fr-popup-content-export-parameters-input"" pattern=""\d+(\s*-\s*\d+)?"" placeholder=""2 or 10-20""value="""" >  
        </div>
        " : "")}
    </div>

    <div class=""fr-popup-content-export-parameters"">
        <label>{localizationDocx.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""DocxWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    Wysiwyg
                </button>
            </div>
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""DocxPrintOptimized"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationDocx.PrintOptimized}
                </button>
            </div>
        </div>
        <div class=""fr-popup-content-export-parameters-col"">
            <button id=""DocxDoNotExpandShiftReturn"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationDocx.DoNotExpandShiftReturn}
            </button>
        </div>
        <div class=""fr-popup-content-export-parameters-row"">
            <div class=""fr-popup-content-export-parameters-col"">
                <span style=""margin-left: 0.5rem;font-size: 12px; font-weight: normal; margin-top: 11px;"">{localizationDocx.RowHeightIs}</span>
                <span style=""margin-left: 0.5rem;font-size: 12px; font-weight: normal; margin-top: 18px;"">{localizationDocx.Options}</span>
            </div>       
            <div class=""fr-popup-content-export-parameters-col"">
                <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "DocxRowHeightsFunc", "this")}>
                    <option value=""Exactly"" selected>{localizationDocx.Exactly}</option>
                    <option value=""Minimum"">{localizationDocx.Minimum}</option>
                </select>
                <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "DocxOnRenderModeFunc", "this")}>
                    <option value=""table"">{localizationDocx.Table}</option>
                    <option value=""layers"" selected>{localizationDocx.Layers}</option>
                    <option value=""paragraphs"">{localizationDocx.Paragraphs}</option>
                </select>
            </div>
        </div>
    </div>
{(false ? "<script type=\"module\" src=\"./_content/FastReport.Web/js/ExportScripts/docx-export.js\"></script>" : "")}
    <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "DOCXExport")} id=""okButton"">OK</button>
    </div>
</div>";
        }
    }
}
