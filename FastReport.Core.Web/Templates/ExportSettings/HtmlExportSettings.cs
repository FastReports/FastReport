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

        internal string template_HtmlExportSettings()
        {
            var localizationHtml = new HtmlExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--9"" data-target=""html"">
    <div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationHtml.Title}
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
            <input name=""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" {CreateEvent(JSEvents.CHANGE, "frActions", "OnPageSelectorChange")} type=""text"" class=""fr-popup-content-export-parameters-input"" pattern=""\d+(\s*-\s*\d+)?"" placeholder=""2 or 10-20"" value="""" >
        </div>" : "")}
    </div>

    <div class=""fr-popup-content-export-parameters"">
        <label>{localizationHtml.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""HTMLWysiwyg"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    Wysiwyg
                </button>
                <button id=""HTMLPictures"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationHtml.Pictures}
                </button>
                <button id=""HTMLLayers"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationHtml.Layers}
                </button>
                <button id=""HTMLSinglePage"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationHtml.SinglePage}
                </button>
                <button id=""HTMLSubFolder"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationHtml.SubFolder}
                </button>
            </div>
            <div class=""fr-popup-content-export-parameters-col"">
                <button id=""HTMLNavigator"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationHtml.Navigator}
                </button>
                <button id=""HTMLEmbeddingPictures"" type=""button"" class=""fr-popup-content-export-parameters-button activeButton"">
                    {localizationHtml.EmbPic}
                </button>
                <button id=""HTMLShowPageBorder"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationHtml.ShowPageBorders}
                </button>
                <button id=""HTMLCenterAndWrapPages"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                   {localizationHtml.CenterAndWrapPages}
                </button>
            </div>
        </div>
    </div>
    {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/html-export.js""></script>" : "")}
    <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "HTMLExport")} id=""okButton"">OK</button>
    </div>
</div>"; 
        }     
    }
}
