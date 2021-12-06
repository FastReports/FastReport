using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using FastReport.Web.Application.Localizations;
using FastReport.Web.Controllers;

namespace FastReport.Web
{
     partial class WebReport
     {

        internal string template_OdsExportSettings()
        {
            var localizationOdf = new OdfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
               <div class=""modalcontainer modalcontainer--6"" data-target=""ods"">
	                <div class=""fr-webreport-popup-content-export-parameters"">
                        <div class=""fr-webreport-popup-content-title"">
                            {localizationOdf.OdsTitle}
                        </div>
                        <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name =""PageSelectorInput""  onchange=""OnInputClickODS()""type=""text""class=""fr-webreport-popup-content-export-parameters-input"" pattern=""[0-9,-\s]""placeholder=""2, 5-132""value="""" >
                       </div>
                    </div>
                    <div class=""fr-webreport-popup-content-export-parameters"">
                        <label> {localizationOdf.Options}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <label style = ""margin-bottom: 0.7rem;"">{localizationOdf.Compliance}</label>
                            <select class=""custom-select"" onchange=""OdsOnComplianceChangeFunc(this)"">
                                <option value=""None"" selected>ODF 1.0/1.1</option>
                                <option value=""Odf1_2"">ODF 1.2</option>
                            </select>
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button id=""OdsWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                Wysiwyg
                            </button>
                            <button id=""OdsPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                {localizationOdf.PageBreaks}
                            </button>
                        </div>
                    </div>
                        <div class=""fr-webreport-popup-content-buttons"">
                            <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                            <button class=""fr-webreport-popup-content-btn-submit"" onclick=""ODSExport()"">OK</button>
                        </div>
                </div>
<script>
 {template_modalcontainerscript}
    //ODSEXPORT//
    var OdsButtons;
    var OdsOnCompliance = '&OdfCompliance=None';
    var OdsWysiwyg = false;
    var OdsPageBreaks = false;

    function OnInputClickODS() {{
        {template_pscustom}
    }}
    function OdsOnComplianceChangeFunc(select) {{
        const OdsOnComplianceChange = select.querySelector(`option[value='${{select.value}}']`)
        OdsOnCompliance = '&OdfCompliance=' + OdsOnComplianceChange.value;
    }}
    function ODSExport() {{

        if (document.getElementById('OdsWysiwyg').classList.contains('active')) {{
            OdsWysiwyg = new Boolean(true);
        }}
        else {{ OdsWysiwyg = false; }};
        if (document.getElementById('OdsPageBreaks').classList.contains('active')) {{
            OdsPageBreaks = new Boolean(true);
        }}
        else {{ OdsPageBreaks = false; }};
        OdsButtons = ('&Wysiwyg=' + OdsWysiwyg + '&PageBreaks=' + OdsPageBreaks);

        window.location.href = OdsExport.href + OdsButtons + OdsOnCompliance + PageSelector;
    }}
</script>
            "; 

        }
       
    }

}
