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

        internal string template_RtfExportSettings()
        {
            var localizationRtf = new RtfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--5"" data-target=""rtf"">
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationRtf.Title}
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
        <label>{localizationRtf.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <button id=""RtfWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton""  style=""padding-right: 40px"">
                Wysiwyg
            </button>
            <button id=""RtfPageBreaks"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                {localizationRtf.PageBreaks}
            </button>
        </div>
        <div class=""fr-popup-content-export-parameters-row"" >
            <div class=""fr-popup-content-export-parameters-col"">
                <span style=""margin-left: 0.4rem;font-size: 12px; font-weight: normal; margin-top: 11px;"">{localizationRtf.Pictures}</span>
                <span style=""margin-left: 0.4rem;font-size: 12px; font-weight: normal; margin-top: 19px;"">{localizationRtf.RTFObjectAs}</span>
            </div>
            <div class=""fr-popup-content-export-parameters-col"" style=""margin-left: 18px;"">
                <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "RtfOnPicturesChangeFunc", "this")}>
                    <option value=""None"">{localizationRtf.None}</option>
                    <option value=""Png"">Png</option>
                    <option value=""Jpeg"" selected>Jpeg</option>
                    <option value=""Metafile"">{localizationRtf.Metafile}</option>
                </select>
                <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "RtfOnRichObjectChangeFunc", "this")}>
                    <option value=""Picture"" selected>{localizationRtf.AsPicture}</option>
                    <option value=""EmbeddedRTF"">{localizationRtf.EmbeddedRTF}</option>
                </select>
            </div>
        </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/rtf-export.js""></script>" : "")}
            <div class=""fr-popup-content-buttons"" style=""margin-bottom: -3.4rem; margin-top: 8px;"">
                <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
                <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "RTFExport")} id=""okButton"">OK</button>
            </div>
    </div>";
        }
    }
}
