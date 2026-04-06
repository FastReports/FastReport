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

        internal string template_ImageExportSettings()
        {
            var localizationImage = new ImageExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--10"" data-target=""image"">
	<div class=""fr-popup-content-export-parameters"">
        <div class=""fr-popup-content-title"">
            {localizationImage.Title}
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
        <div class=""fr-popup-content-export-parameters-col"">
            <label for=""formatSelect"" class=""mb-1"">{localizationImage.Format}</label>
            <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "ImageOnImageFormatFunc", "this")} id=""formatSelect"">
                <option value=""Bmp"">{localizationImage.Bmp}</option>
                <option value=""Png"">{localizationImage.Png}</option>
                <option value=""Jpeg"" selected>{localizationImage.Jpeg}</option>
                <option value=""Gif"">{localizationImage.Gif}</option>
                <option value=""Tiff"">{localizationImage.Tiff}</option>
                <option value=""Metafile"">{localizationImage.Metafile}</option>
            </select>
            <label>{localizationImage.Resolution}</label>
                    <p style=""display:block;margin-left:0.1rem;margin:0;padding:0;"" name =""ImageEnableOrNot""><input value=""90"" class=""fr-popup-content-export-parameters-input"" type=""number"" min=""1"" max=""100"" step=""1"" id=""ImageResolutionX"">X</p>
                    <p style=""display:none;margin-left:0.1rem;margin:0;padding:0;"" name =""ImageEnableOrNot""><input  value=""90"" class=""fr-popup-content-export-parameters-input"" type=""number"" min=""1"" max=""100"" step=""1"" id=""ImageResolutionY"">Y</p>
                    <div style=""display:flex;""name =""ImageEnableOrNot"" class=""fr-popup-content-export-parameters-slider"">
                    <p>{localizationImage.JpegQuality}</p>
                    <select class=""custom-select"" {CreateEvent(JSEvents.CHANGE, "frActions", "JpegQualityFunc", "this")} id=""formatSelect"">
                        <option value=""10"">10</option>
                        <option value=""20"">20</option>
                        <option value=""30"">30</option>
                        <option value=""40"">40</option>
                        <option value=""50"">50</option>
                        <option value=""60"">60</option>
                        <option value=""70"">70</option>
                        <option value=""80"">80</option>
                        <option value=""90"" selected>90</option>
                        <option value=""100"">100</option>
                    </select>
         </div>   
    </div>
    <div class=""fr-popup-content-export-parameters-col"">
        <button style=""display:none;"" name =""ImageEnableOrNot"" id=""ImageOnMultiFrameTiffClick"" type=""button"" class=""fr-popup-content-export-parameters-button"">
            {localizationImage.MultiFrame}
        </button>
        <button style=""display:none;"" name =""ImageEnableOrNot"" id=""ImageOnMonochromeTiffClick"" type=""button"" class=""fr-popup-content-export-parameters-button"">
            {localizationImage.MonochromeTIFF}
        </button>
        <button style=""display:block;"" name =""ImageEnableOrNot"" id=""ImageOnSeparateFilesClick"" type=""button"" class=""fr-popup-content-export-parameters-button"">
            {localizationImage.Separate}
        </button>
    </div>
    </div>
 {(false ? @"<script type=""module"" src=""./_content/FastReport.Web/js/ExportScripts/image-export.js""></script>" : "")}
    <div class=""fr-popup-content-buttons"">
        <button class=""fr-popup-content-btn-submit fr-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-popup-content-btn-submit"" {CreateOnClickEvent("frActions", "IMAGEExport")} id=""okButton"">OK</button>
    </div>
</div>";
        }
    }
}