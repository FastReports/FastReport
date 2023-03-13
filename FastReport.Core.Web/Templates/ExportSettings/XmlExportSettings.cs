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
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationXml.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickXML()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >    
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">  
        <label>{localizationXml.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""XmlWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    Wysiwyg
                </button>
                <button id=""XmlDataOnly"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXml.DataOnly}
                </button>
            </div>
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""XmlPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXml.PageBreaks}
                </button>
            </div>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""XMLExport()"" id=""okButton"">OK</button>
    </div>
</div>
<script>
  {template_modalcontainerscript}
//XMLEXPORT//
var XmlButtons;
var XmlWysiwyg = false;
var XmlPageBreaks = false;
var XmlDataOnly = false;

function OnInputClickXML() {{
 {template_pscustom}
}}
function XMLExport() {{
    {validation}

    if (document.getElementById('XmlWysiwyg').classList.contains('activeButton')) {{
        XmlWysiwyg = new Boolean(true);
    }}
    else {{ XmlWysiwyg = false; }};

    if (document.getElementById('XmlPageBreaks').classList.contains('activeButton')) {{
        XmlPageBreaks = new Boolean(true);
    }}
    else {{ XmlPageBreaks = false; }};

    if (document.getElementById('XmlDataOnly').classList.contains('activeButton')) {{
        XmlDataOnly = new Boolean(true);
    }}
    else {{ XmlDataOnly = false; }};
    XmlButtons = ('&Wysiwyg=' + XmlWysiwyg + '&PageBreaks=' + XmlPageBreaks + '&DataOnly=' + XmlDataOnly);
    window.location.href = XmlExport.href + XmlButtons + PageSelector;
}}
</script>";

        }
       
    }

}
