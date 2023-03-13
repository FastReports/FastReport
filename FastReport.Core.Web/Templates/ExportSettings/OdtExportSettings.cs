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

        internal string template_OdtExportSettings()
        {
            var localizationOdf = new OdfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--7"" data-target=""odt"">
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationOdf.OdtTitle}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickODT()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >   
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label>{localizationOdf.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <button id=""OdtWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"" style=""padding-right: 40px"">
                Wysiwyg
            </button>
            <button id=""OdtPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                {localizationOdf.PageBreaks}
            </button>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <label style = ""margin-top: 10px; font-weight: normal; font-size: 11px;"">{localizationOdf.Compliance}</label>
            <select class=""custom-select"" style=""margin-left: 20px;"" onchange =""OdtOnComplianceChangeFunc(this)"">
                <option value=""None"" selected>ODF 1.0/1.1</option>
                <option value=""Odf1_2"">ODF 1.2</option>
            </select>
        </div>
    </div>
     <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""ODTExport()"" id=""okButton"">OK</button>
    </div>
</div>	
<script>
{template_modalcontainerscript}
//ODTEXPORT//
var OdtButtons;
var OdtOnCompliance = '&OdfCompliance=None';
var OdtWysiwyg = false;
var OdtPageBreaks = false;

function OnInputClickODT() {{
   {template_pscustom}
}}
function OdtOnComplianceChangeFunc(select) {{
    const OdtOnComplianceChange = select.querySelector(`option[value='${{select.value}}']`)
    OdtOnCompliance = '&OdfCompliance=' + OdtOnComplianceChange.value;
}}
function ODTExport() {{
    {validation}

    if (document.getElementById('OdtWysiwyg').classList.contains('activeButton')) {{
        OdtWysiwyg = new Boolean(true);
    }}
    else {{ OdtWysiwyg = false; }};
    if (document.getElementById('OdtPageBreaks').classList.contains('activeButton')) {{
        OdtPageBreaks = new Boolean(true);
    }}
    else {{ OdtPageBreaks = false; }};
    OdtButtons = ('&Wysiwyg=' + OdtWysiwyg + '&PageBreaks=' + OdtPageBreaks);

    window.location.href = OdtExport.href + OdtButtons + OdtOnCompliance;
}}
</script>"; 

        }
       
    }

}
