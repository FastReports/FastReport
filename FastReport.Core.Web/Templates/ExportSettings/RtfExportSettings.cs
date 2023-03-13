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
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationRtf.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickRTF()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label>{localizationRtf.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <button id=""RtfWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton""  style=""padding-right: 40px"">
                Wysiwyg
            </button>
            <button id=""RtfPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                {localizationRtf.PageBreaks}
            </button>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"" >
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <span style=""margin-left: 0.4rem;font-size: 12px; font-weight: normal; margin-top: 11px;"">{localizationRtf.Pictures}</span>
                <span style=""margin-left: 0.4rem;font-size: 12px; font-weight: normal; margin-top: 19px;"">{localizationRtf.RTFObjectAs}</span>
            </div>
            <div class=""fr-webreport-popup-content-export-parameters-col"" style=""margin-left: 18px;"">
                <select class=""custom-select"" onchange=""RtfOnPicturesChangeFunc(this)"">
                    <option value=""None"">{localizationRtf.None}</option>
                    <option value=""Png"">Png</option>
                    <option value=""Jpeg"" selected>Jpeg</option>
                    <option value=""Metafile"">{localizationRtf.Metafile}</option>
                </select>
                <select class=""custom-select"" onchange=""RtfOnRichObjectChangeFunc(this)"">
                    <option value=""Picture"" selected>{localizationRtf.AsPicture}</option>
                    <option value=""EmbeddedRTF"">{localizationRtf.EmbeddedRTF}</option>
                </select>
            </div>
        </div>
            <div class=""fr-webreport-popup-content-buttons"" style=""margin-bottom: -3.4rem; margin-top: 8px;"">
                <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
                <button class=""fr-webreport-popup-content-btn-submit"" onclick=""RTFExport()"" id=""okButton"">OK</button>
            </div>
    </div>
<script>
  {template_modalcontainerscript}
//RTFEXPORT//
var RtfOnRichObject = '&Pictures=false&EmbeddedRTF=false';
var RtfOnPictures = '&ImageFormat=Jpeg';
var RtfEmbedded = false;
var RtfPicture = false;


function OnInputClickRTF() {{
  {template_pscustom}
}}

function RtfOnPicturesChangeFunc(select) {{
    const RtfOnPicturesChange = select.querySelector(`option[value='${{select.value}}']`)
        if (RtfOnPicturesChange.value != 'None') {{
        RtfOnPictures = '&ImageFormat=' + RtfOnPicturesChange.value;
    }}
        else{{RtfOnPictures = ''}}
}}
function RtfOnRichObjectChangeFunc(select) {{

    const RtfOnRichObjectChange = select.querySelector(`option[value='${{select.value}}']`)

    if (RtfOnRichObjectChange.value == 'EmbeddedRTF') {{
        RtfPicture = false;
        RtfEmbedded = new Boolean(true);
    }}
    if (RtfOnRichObjectChange.value == 'Picture') {{
        RtfPicture = new Boolean(true);
        RtfEmbedded = false;
    }}

    RtfOnRichObject = '&Pictures=' + RtfPicture + '&EmbeddedRTF=' + RtfEmbedded;
}}
function RTFExport() {{
    {validation}
    window.location.href = RtfExport.href + RtfOnPictures + RtfOnRichObject + PageSelector;
}}
</script>"; 

        }
       
    }

}
