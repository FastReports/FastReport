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
    
        internal string template_PdfExportSettings()
        {
            var localizationPdf = new PdfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);


            return $@"
<div class=""modalcontainer modalcontainer--1"" data-target=""pdf"">
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationPdf.Title}
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
        <label> {localizationPdf.Options}</label>
        <div class=""fr-popup-content-export-parameters-row"">
            <select style=""align-self:flex-start;"" class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "PDFCompilanceFunc", "this")}>
                <option value=""None"" selected>PDF 1.5</option>
                <option value=""PdfA_1a"">PDF/A-1a</option>
                <option value=""PdfA_2a"">PDF/A-2a</option>
                <option value=""PdfA_2b"">PDF/A-2b</option>
                <option value=""PdfA_3a"">PDF/A-3a</option>
                <option value=""PdfA_3b"">PDF/A-3b</option>
                <option value=""PdfX_3"">PDF/X-3</option>
                <option value=""PdfX_4"">PDF/X-4</option>
            </select>
        </div>
        <div class=""fr-popup-content-export-parameters-row"">
        <div class=""fr-popup-content-export-parameters-col"">
                <button id=""PDFOnEmbeddedFonts"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationPdf.EmbeddedFonts}
                </button>
                <button id=""PDFOnInteractive"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationPdf.InteractiveForms}
                </button>
        </div>
        <div class=""fr-popup-content-export-parameters-col"">
                <button  id=""PDFOnTextInCurves"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationPdf.TextInCurves}
                </button>
                <button id=""PDFOnBackground"" type=""button"" class=""fr-popup-content-export-parameters-button"">
                    {localizationPdf.Background}
                </button>
            </div>
        </div>
    </div>
    <div class=""fr-popup-content-export-parameters"">
        <label> {localizationPdf.Images}</label>
        <div class=""fr-popup-content-export-parameters-col"">
                <div style=""display:flex; font-weight: normal;"" class=""fr-popup-content-export-parameters-slider"">
                    <p>{localizationPdf.JpegQuality}</p>
                    <select class=""custom-select"" style=""margin-left: 10px;"" {CreateEvent(JSEvents.CHANGE, "frActions", "PDFJpegQualityFunc", "this")}>
                        <option value = ""10"">10</option>
                        <option value = ""20"">20</option>
                        <option value = ""30"">30</option>
                        <option value = ""40"">40</option>
                        <option value = ""50"">50</option>
                        <option value = ""60"">60</option>
                        <option value = ""70"">70</option>
                        <option value = ""80"">80</option>
                        <option value = ""90"" selected>90</option>
                        <option value = ""100"">100</option>
                    </select>
                </div>   
        </div>
        <div class=""fr-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-popup-content-export-parameters-button activeButton"" style=""margin-top: 5px;"" id = ""RGB"" name=""OnRgbClick"" {CreateOnClickEvent("frActions", "OnRgbClick")}>
                RGB
            </button>
            <button type=""button"" class=""fr-popup-content-export-parameters-button"" style=""margin-top: 5px;"" id = ""CMYK"" name=""OnCmykClick"" {CreateOnClickEvent("frActions", "OnCmykClick")}>
                CMYK
            </button>
            <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "PDFImageOptionFunc", "this")}>
                <option value=""None"" selected>None</option>
                <option value=""PrintOptimized"">{localizationPdf.PrintOptimized}</option>
                <option value=""ImagesOriginalResolution"">{localizationPdf.OriginalResolution}</option>
            </select>
        </div>
    </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/pdf-export.js""></script>" : "")}
    <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "PDFExport")} id=""okButton"">OK</button>
   </div>
</div>";
        }
    }
}
