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
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationImage.Title}
        </div>
        {(Report.PreparedPages.Count != 1 ? $@"<label class=""fr-webreport-popup-content-export-parameters-page-range-title"">{localizationPageSelector.PageRange}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"" name=""OnAllClick"" onclick=""OnAllClick()"">
                {localizationPageSelector.All}
            </button>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                {localizationPageSelector.First}
            </button>
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickIMAGE()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >   
</div>" : "")}
   </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-export-parameters-col"">
            <label for=""formatSelect"" class=""mb-1"">{localizationImage.Format}</label>
            <select class=""custom-select"" onchange=""ImageOnImageFormatFunc(this)"" id=""formatSelect"">
                <option value=""Bmp"">{localizationImage.Bmp}</option>
                <option value=""Png"">{localizationImage.Png}</option>
                <option value=""Jpeg"" selected>{localizationImage.Jpeg}</option>
                <option value=""Gif"">{localizationImage.Gif}</option>
                <option value=""Tiff"">{localizationImage.Tiff}</option>
                <option value=""Metafile"">{localizationImage.Metafile}</option>
            </select>
            <label>{localizationImage.Resolution}</label>
                    <p style=""display:block;margin-left:0.1rem;margin:0;padding:0;"" name =""ImageEnableOrNot""><input value=""90"" class=""fr-webreport-popup-content-export-parameters-input"" type=""number"" min=""1"" max=""100"" step=""1"" id=""ImageResolutionX"">X</p>
                    <p style=""display:none;margin-left:0.1rem;margin:0;padding:0;"" name =""ImageEnableOrNot""><input  value=""90"" class=""fr-webreport-popup-content-export-parameters-input"" type=""number"" min=""1"" max=""100"" step=""1"" id=""ImageResolutionY"">Y</p>
                    <div style=""display:flex;""name =""ImageEnableOrNot"" class=""fr-webreport-popup-content-export-parameters-slider"">
                    <p>{localizationImage.JpegQuality}</p>
                    <select class=""custom-select"" onchange=""JpegQualityFunc(this)"" id=""formatSelect"">
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
    <div class=""fr-webreport-popup-content-export-parameters-col"">
        <button style=""display:none;"" name =""ImageEnableOrNot"" id=""ImageOnMultiFrameTiffClick"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
            {localizationImage.MultiFrame}
        </button>
        <button style=""display:none;"" name =""ImageEnableOrNot"" id=""ImageOnMonochromeTiffClick"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
            {localizationImage.MonochromeTIFF}
        </button>
        <button style=""display:block;"" name =""ImageEnableOrNot"" id=""ImageOnSeparateFilesClick"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
            {localizationImage.Separate}
        </button>
    </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""IMAGEExport()"" id=""okButton"">OK</button>
    </div>
</div>
<script>
{template_modalcontainerscript}
//SLIDER//
var SliderOutputImage = '90';
function Slider() {{
    var SliderRange = document.getElementsByName('SliderRange');
    var SliderValue = document.getElementsByName('SliderValue');
    for (var i = 0; i < SliderRange.length; i++) {{
        SliderValue[i].innerHTML = SliderRange[i].value

    }}
    SliderOutputImage = SliderRange[0].value;
}}

function JpegQualityFunc(select){{
    const JpegQuality = select.querySelector(`option[value='${{select.value}}']`)
    SliderOutputImage = JpegQuality.value
}}

//IMAGE//
var ImageButtons;
var ImageResolutionX = '&ResolutionX=90';
var ImageResolutionY = '&ResolutionY=90';
var ImageQuality = '&JpegQuality=90';
var ImageOptionSettings = document.getElementsByName('ImageEnableOrNot');
var ImageOnImageFormat = '&ImageFormat=Jpeg';
var ImageOnMultiFrameTiffClick = false;
var ImageOnMonochromeTiffClick = false;
var ImageOnSeparateFilesClick = false;

function OnInputClickIMAGE() {{
    {template_pscustom}
}}
function ImageOnImageFormatFunc(select) {{
    const ImageOnImageFormatChange = select.querySelector(`option[value='${{select.value}}']`)

    if (ImageOnImageFormatChange.value == 'Gif' || ImageOnImageFormatChange.value == 'Png' || ImageOnImageFormatChange.value == 'Bmp' || ImageOnImageFormatChange.value == 'Metafile') {{

        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'none';
        ImageOptionSettings[2].style.display = 'none';
        ImageOptionSettings[3].style.display = 'none';
        ImageOptionSettings[5].style.display = 'block';
    }}
    else if (ImageOnImageFormatChange.value == 'Jpeg') {{

        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'none';
        ImageOptionSettings[2].style.display = 'flex';
        ImageOptionSettings[5].style.display = 'block';


    }}
    else if (ImageOnImageFormatChange.value == 'Tiff') {{
        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'block';
        ImageOptionSettings[2].style.display = 'none';
        ImageOptionSettings[3].style.display = 'block';
        ImageOptionSettings[4].style.display = 'block';
        ImageOptionSettings[5].style.display = 'block';
    }}
    ImageOnImageFormat = '&ImageFormat=' + ImageOnImageFormatChange.value;
}}
function IMAGEExport() {{
    {validation}

    ImageResolutionX = '&ResolutionX=' + document.getElementById('ImageResolutionX').value;
    ImageResolutionY = '&ResolutionY=' + document.getElementById('ImageResolutionY').value;
    ImageQuality = '&JpegQuality=' + SliderOutputImage;

    if (document.getElementById('ImageOnMultiFrameTiffClick').classList.contains('activeButton')) {{
        ImageOnMultiFrameTiffClick = new Boolean(true);
    }}
    else {{ ImageOnMultiFrameTiffClick = false; }};

    if (document.getElementById('ImageOnMonochromeTiffClick').classList.contains('activeButton')) {{
        ImageOnMonochromeTiffClick = new Boolean(true);
    }}
    else {{ ImageOnMonochromeTiffClick = false; }};

    if (document.getElementById('ImageOnSeparateFilesClick').classList.contains('activeButton')) {{
        ImageOnSeparateFilesClick = new Boolean(true);
    }}
    else {{ ImageOnSeparateFilesClick = false; }};

    ImageButtons = ('&MultiFrameTiff=' + ImageOnMultiFrameTiffClick + '&MonochromeTiff=' + ImageOnMonochromeTiffClick + '&SeparateFiles=' + ImageOnSeparateFilesClick);
    window.location.href = ImageExport.href + ImageOnImageFormat + ImageButtons + PageSelector + ImageQuality + ImageResolutionX + ImageResolutionY;
}}
</script>";

        }

    }

}