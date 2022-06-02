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
                            <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name=""PageSelectorInput""  onchange=""OnInputClickXLSX()""type=""text""class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2, 5-132""value="""" >
                       </div>
                    </div>
                    <div class=""fr-webreport-popup-content-export-parameters"">
                        <label>{localizationXlsx.Options}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""XlsxWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                    Wysiwyg
                                </button>
                                <button id=""XlsxPageBreaks"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationXlsx.PageBreaks}
                                </button>
                            </div>
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""XlsxDataOnly"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationXlsx.DataOnly}
                                </button>
                                <button id=""XlsxSeamless"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationXlsx.Seamless}
                                </button>
                            </div>
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""XlsxPrintOptimized"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                    {localizationXlsx.PrintOptimized}
                                </button>
                                <button id=""XlsxSplitPages"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationXlsx.SplitPages}
                                </button>
                            </div>
                        </div>
                    </div>
                    <div class=""fr-webreport-popup-content-export-parameters"">
                    <label>{localizationXlsx.PrintScaling}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-col"">   
                            <select class=""custom-select"" onchange=""XlsxOnPrintFitChangeFunc(this)"">
                                <option value=""NoScaling"" selected>{localizationXlsx.NoScaling}</option>
                                <option value=""FitSheetOnOnePage"">{localizationXlsx.FitSheetOnOnePage}</option>
                                <option value=""FitAllColumsOnOnePage"">{localizationXlsx.FitAllColumsOnOnePage}</option>
                                <option value=""FitAllRowsOnOnePage"">{localizationXlsx.FitAllRowsOnOnePage}</option>
                            </select>
                            <div style=""display:flex;"" class=""fr-webreport-popup-content-export-parameters-slider"">
                                <input type=""range"" min=""0.1"" step=""0.01"" max=""5.0"" value=""1.00"" name = ""SliderRange"" onchange = ""Slider()"">
                                <p>{localizationXlsx.FontScale} <span name=""SliderValue"">1</span></p>
                            </div>   
                        </div>
                    </div>
                    <div class=""fr-webreport-popup-content-buttons"">
                        <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""XLSXExport()"">OK</button>
                    </div>
                </div>
<script> 
       {template_modalcontainerscript}
    //SLIDER//
    var SliderOutputExcel = '1';
    function Slider() {{
        var SliderRange = document.getElementsByName('SliderRange');
        var SliderValue = document.getElementsByName('SliderValue');
        for (var i = 0; i < SliderRange.length; i++) {{
            SliderValue[i].innerHTML = SliderRange[i].value

        }}
        SliderOutputExcel = SliderRange[0].value;
    }}
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

    function XlsxOnPrintFitChangeFunc(select) {{
        const XlsxOnPrintFitChange = select.querySelector(`option[value='${{select.value}}']`)
        XlsxOnPrintFit = '&PrintFit=' + XlsxOnPrintFitChange.value;
     }}

    function OnInputClickXLSX() {{
        {template_pscustom}
    }}

    function XLSXExport() {{

        XlsxFontScaling = '&FontScale=' + SliderOutputExcel.replace('.', ',');

        if (document.getElementById('XlsxWysiwyg').classList.contains('active')) {{
            XlsxWysiwyg = new Boolean(true);
        }}
        else {{ XlsxWysiwyg = false; }};

        if (document.getElementById('XlsxPageBreaks').classList.contains('active')) {{
            XlsxPageBreaks = new Boolean(true);
        }}
        else {{ XlsxPageBreaks = false; }};
        if (document.getElementById('XlsxDataOnly').classList.contains('active')) {{
            XlsxDataOnly = new Boolean(true);
        }}
        else {{ XlsxSeamless = false; }};
        if (document.getElementById('XlsxSeamless').classList.contains('active')) {{
            XlsxSeamless = new Boolean(true);
        }}
        else {{ XlsxPrintOptimized = false; }};
        if (document.getElementById('XlsxPrintOptimized').classList.contains('active')) {{
            XlsxPrintOptimized = new Boolean(true);
        }}
        else {{ XlsxPrintOptimized = false; }};
        if (document.getElementById('XlsxSplitPages').classList.contains('active')) {{
            XlsxSplitPages = new Boolean(true);
        }}
        else {{ XlsxSplitPages = false; }};

        XlsxButtons = (XlsxOnPrintFit + '&Wysiwyg=' + XlsxWysiwyg + '&PrintOptimized=' + XlsxPrintOptimized + '&DataOnly=' + XlsxDataOnly + '&Seamless=' + XlsxSeamless + '&SplitPages=' + XlsxSplitPages + '&PageBreaks=' + XlsxPageBreaks);
        window.location.href = XlsxExport.href + XlsxButtons + PageSelector + XlsxFontScaling;
    }}
</script>

            "; 

        }
       
    }

}
