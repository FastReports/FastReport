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

        internal string template_XmlExportSettings()
        {
            var localizationXml = new XmlExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
 <div class=""modalcontainer modalcontainer--8"" data-target=""xml"">
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationXml.Title}
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
        <label>{localizationXml.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""XmlWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    Wysiwyg
                </button>
                <button id=""XmlDataOnly"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXml.DataOnly}
                </button>
            </div>
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""XmlPageBreaks"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationXml.PageBreaks}
                </button>
            </div>
        </div>
    </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/xml-export.js""></script>" : "")}
    <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "XMLExport")} id=""okButton"">OK</button>
    </div>
</div>";
        }
    }
}
