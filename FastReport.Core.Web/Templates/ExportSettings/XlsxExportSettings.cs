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

        internal string template_XlsxExportSettings()
        {
            var localizationXlsx = new XlsxExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--2"" data-target=""xlsx"" >
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationXlsx.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickXLSX()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label>{localizationXlsx.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""XlsxWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"">
                    Wysiwyg
                </button>
                <button id=""XlsxPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXlsx.PageBreaks}
                </button>
                <button id=""XlsxDataOnly"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXlsx.DataOnly}
                </button>
            </div>
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""XlsxSeamless"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXlsx.Seamless}
                </button>
                <button id=""XlsxPrintOptimized"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"">
                    {localizationXlsx.PrintOptimized}
                </button>
                <button id=""XlsxSplitPages"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationXlsx.SplitPages}
                </button>
            </div>
          </div>
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"" style=""padding-bottom: 0px;"">
    <label>{localizationXlsx.PrintScaling}</label>
        <div class=""fr-webreport-popup-content-export-parameters-col"">   
            <select class=""custom-select"" onchange=""XlsxOnPrintFitChangeFunc(this)"">
                <option value=""NoScaling"" selected>{localizationXlsx.NoScaling}</option>
                <option value=""FitSheetOnOnePage"">{localizationXlsx.FitSheetOnOnePage}</option>
                <option value=""FitAllColumsOnOnePage"">{localizationXlsx.FitAllColumsOnOnePage}</option>
                <option value=""FitAllRowsOnOnePage"">{localizationXlsx.FitAllRowsOnOnePage}</option>
            </select>
            <div class=""fr-webreport-popup-content-export-parameters-slider""  style=""display:flex; padding-left: 0.4rem;"">
                <p style=""font-weight: normal;"">{localizationXlsx.FontScale}</p>
                    <select class=""custom-select"" onchange = ""XlsxFontScalingFunc(this)"">
                        <option value = ""0.1"">0.1</option>
                        <option value = ""0.5"">0.5</option>
                        <option value = ""1"" selected>1</option>
                        <option value = ""1.5"">1.5</option>
                        <option value = ""2"">2</option>
                        <option value = ""2.5"">2.5</option>
                        <option value = ""3"">3</option>
                        <option value = ""3.5"">3.5</option>
                        <option value = ""4"">4</option>
                        <option value = ""4.5"">4.5</option>
                        <option value = ""5"">5</option>
                    </select>
            </div>   
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"" style = ""margin-top: 8px;"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""XLSXExport()"" id=""okButton"">OK</button>
    </div>
</div>
<script> 
   {template_modalcontainerscript}
//XLSXEXPORT//
var XlsxButtons;
var XlsxFontScaling = '&FontScale=1';
var XlsxOnPrintFit = '&PrintFit=NoScaling';
var XlsxPageBreaks = false;
var XlsxDataOnly = false;
var XlsxWysiwyg = false;
var XlsxSeamless = false;
var XlsxPrintOptimized = false;
var XlsxSplitPages = false;

function XlsxFontScalingFunc(select){{
    const XlsxFontScalingChange = select.querySelector(`option[value='${{select.value}}']`)
    XlsxFontScaling = '&FontScale=' + XlsxFontScalingChange.value.replace('.', ',');
}}

function XlsxOnPrintFitChangeFunc(select) {{
    const XlsxOnPrintFitChange = select.querySelector(`option[value='${{select.value}}']`)
    XlsxOnPrintFit = '&PrintFit=' + XlsxOnPrintFitChange.value;
 }}

function OnInputClickXLSX() {{
    {template_pscustom}
}}

function XLSXExport() {{
    {validation}

    if (document.getElementById('XlsxWysiwyg').classList.contains('activeButton')) {{
        XlsxWysiwyg = new Boolean(true);
    }}
    else {{ XlsxWysiwyg = false; }};

    if (document.getElementById('XlsxPageBreaks').classList.contains('activeButton')) {{
        XlsxPageBreaks = new Boolean(true);
    }}
    else {{ XlsxPageBreaks = false; }};
    if (document.getElementById('XlsxDataOnly').classList.contains('activeButton')) {{
        XlsxDataOnly = new Boolean(true);
    }}
    else {{ XlsxSeamless = false; }};
    if (document.getElementById('XlsxSeamless').classList.contains('activeButton')) {{
        XlsxSeamless = new Boolean(true);
    }}
    else {{ XlsxPrintOptimized = false; }};
    if (document.getElementById('XlsxPrintOptimized').classList.contains('activeButton')) {{
        XlsxPrintOptimized = new Boolean(true);
    }}
    else {{ XlsxPrintOptimized = false; }};
    if (document.getElementById('XlsxSplitPages').classList.contains('activeButton')) {{
        XlsxSplitPages = new Boolean(true);
    }}
    else {{ XlsxSplitPages = false; }};

    XlsxButtons = (XlsxOnPrintFit + '&Wysiwyg=' + XlsxWysiwyg + '&PrintOptimized=' + XlsxPrintOptimized + '&DataOnly=' + XlsxDataOnly + '&Seamless=' + XlsxSeamless + '&SplitPages=' + XlsxSplitPages + '&PageBreaks=' + XlsxPageBreaks);
    window.location.href = XlsxExport.href + XlsxButtons + PageSelector + XlsxFontScaling;
}}
</script>"; 

        }
       
    }

}
