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

        internal string template_SvgExportSettings()
        {
            var localizationSvg = new SvgExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--11"" data-target=""svg"">
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationSvg.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickSVG()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" > 
</div>" : "")}
    </div>

    <div class=""fr-webreport-popup-content-export-parameters"">
        <label for=""picturesSelect"" class=""mb-1"">{localizationSvg.Pictures}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <select class=""custom-select"" onchange=""SVGImageFormatFunc(this)"" id=""picturesSelect"">
                <option value=""None"">None</option>
                <option value=""Png"">Png</option>
                <option value=""Jpeg"" selected>Jpeg</option>
            </select>
        <div class=""fr-webreport-popup-content-export-parameters-col"">
            <button id=""SVGEmbedPictures"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"">
                {localizationSvg.EmbPic}
            </button>
            <button id=""SVGOnHasMultipleFilesClick"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                {localizationSvg.ToMultipleFiles}
            </button>
        </div>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""SVGExport()"" id=""okButton"">OK</button>
    </div>
</div>
<script>
   {template_modalcontainerscript}
//SVGEXPORT
var SVGImageFormat = '&ImageFormat=Jpeg';
var SVGEmbedPictures = false;
var SVGHasMultiplyFiles = false;
var SVGButtons;

function OnInputClickSVG() {{
  {template_pscustom}
}}
function SVGImageFormatFunc(select) {{
    const ImageFormatOption = select.querySelector(`option[value='${{select.value}}']`)
    if (ImageFormatOption.value == 'None') {{ ImageFormatOption.value = ''; }}
    else {{ SVGImageFormat = '&ImageFormat=' + ImageFormatOption.value; }}
}}
function SVGExport() {{
    {validation}    

    if (document.getElementById('SVGEmbedPictures').classList.contains('activeButton')) {{
        SVGEmbedPictures = new Boolean(true);
    }}
    else {{ SVGEmbedPictures = false; }};

    if (document.getElementById('SVGOnHasMultipleFilesClick').classList.contains('activeButton')) {{
        SVGHasMultiplyFiles = new Boolean(true);
    }}
    else {{ SVGHasMultiplyFiles = false; }};

    SVGButtons = ('&EmbedPictures=' + SVGEmbedPictures + '&HasMultipleFiles=' + SVGHasMultiplyFiles);

    window.location.href = SvgExport.href + SVGImageFormat + SVGButtons + PageSelector;
}}
</script>"; 

        }
       
    }

}
