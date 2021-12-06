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
                        <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name =""PageSelectorInput""  onchange=""OnInputClickXML()""type=""text""class=""fr-webreport-popup-content-export-parameters-input"" pattern=""[0-9,-\s]"" placeholder=""2, 5-132""value="""" >
                       </div>
                    </div>
                    <div class=""fr-webreport-popup-content-export-parameters"">  
                        <label>{localizationXml.Options}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button id=""XmlWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                Wysiwyg
                            </button>
                            <button id=""XmlPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                {localizationXml.PageBreaks}
                            </button>
                            <button id=""XmlDataOnly"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                {localizationXml.DataOnly}
                            </button>
                        </div>
                    </div>
                    <div class=""fr-webreport-popup-content-buttons"">
                        <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""XMLExport()"">OK</button>
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
        if (document.getElementById('XmlWysiwyg').classList.contains('active')) {{
            XmlWysiwyg = new Boolean(true);
        }}
        else {{ XmlWysiwyg = false; }};

        if (document.getElementById('XmlPageBreaks').classList.contains('active')) {{
            XmlPageBreaks = new Boolean(true);
        }}
        else {{ XmlPageBreaks = false; }};

        if (document.getElementById('XmlDataOnly').classList.contains('active')) {{
            XmlDataOnly = new Boolean(true);
        }}
        else {{ XmlDataOnly = false; }};
        XmlButtons = ('&Wysiwyg=' + XmlWysiwyg + '&PageBreaks=' + XmlPageBreaks + '&DataOnly=' + XmlDataOnly);
        window.location.href = XmlExport.href + XmlButtons + PageSelector;
    }}
    </script>
            "; 

        }
       
    }

}
