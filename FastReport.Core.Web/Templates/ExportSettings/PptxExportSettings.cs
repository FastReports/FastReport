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

        internal string template_PptxExportSettings()
        {
            var localizationPptx = new PptxExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--s"" data-target=""pptx"">
	 <div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationPptx.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickPPTX()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" > 
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-export-parameters-col"">
            <label style=""font-size:12px;"">{localizationPptx.Pictures}</label>
            <div class=""fr-webreport-popup-content-export-parameters-col"">
            <select class=""custom-select"" onchange=""PptxImageFormatFunc(this)"">
                <option value=""Png"">PNG</option>
                <option value=""Jpeg"" selected>JPEG</option>
            </select>
            </div>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""PPTXExport()"" id=""okButton"">OK</button>
    </div>
  </div
</div>
<script>
   {template_modalcontainerscript}
//PPTXEXPORT//
var PptxImageFormat = '&ImageFormat=Jpeg';

function OnInputClickPPTX() {{
   {template_pscustom}
}}
function PptxImageFormatFunc(select) {{
    const PptxImageFormatChage = select.querySelector(`option[value='${{select.value}}']`)
    PptxImageFormat = '&ImageFormat=' + PptxImageFormatChage.value;
}}

function PPTXExport() {{
    {validation}
    window.location.href = PptxExport.href + PptxImageFormat //+PageSelector;
}}
</script>"; 

        }
       
    }

}
