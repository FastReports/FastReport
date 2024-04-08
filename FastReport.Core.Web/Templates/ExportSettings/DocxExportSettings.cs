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

       
        internal string template_DocxExportSettings()
        {
            var localizationDocx = new DocxExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
<div class=""modalcontainer modalcontainer--3"" data-target=""docx"">
    <div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationDocx.Title}
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
            <input name =""PageSelectorInput"" id=""PageSelector"" style=""margin-top: 2px;""  onchange=""OnInputClickDOCX()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >  
        </div>
        " : "")}
    </div>

    <div class=""fr-webreport-popup-content-export-parameters"">
        <label>{localizationDocx.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""DocxWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"">
                    Wysiwyg
                </button>
            </div>
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""DocxPrintOptimized"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"">
                    {localizationDocx.PrintOptimized}
                </button>
            </div>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-col"">
            <button id=""DocxDoNotExpandShiftReturn"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationDocx.DoNotExpandShiftReturn}
            </button>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <span style=""margin-left: 0.5rem;font-size: 12px; font-weight: normal; margin-top: 11px;"">{localizationDocx.RowHeightIs}</span>
                <span style=""margin-left: 0.5rem;font-size: 12px; font-weight: normal; margin-top: 18px;"">{localizationDocx.Options}</span>
            </div>       
            <div class=""fr-webreport-popup-content-export-parameters-col"">
                <select class=""custom-select"" onchange=""DocxRowHeightsFunc(this)"">
                    <option value=""Exactly"" selected>{localizationDocx.Exactly}</option>
                    <option value=""Minimum"">{localizationDocx.Minimum}</option>
                </select>
                <select class=""custom-select""  onchange=""DocxOnRenderModeFunc(this)"">
                    <option value=""table"">{localizationDocx.Table}</option>
                    <option value=""layers"" selected>{localizationDocx.Layers}</option>
                    <option value=""paragraphs"">{localizationDocx.Paragraphs}</option>
                </select>
            </div>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""DOCXExport()"" id=""okButton"">OK</button>
    </div>
</div>

<script>
{template_modalcontainerscript}
//DOCXEXPORT//
var DocxButtons;
var DocxRowHeights = '&RowHeight=Exactly';
var DocxOnRenderMode = '&PrintFit=layers';
var DocxWysiwyg = false;
var DocxPrintOptimized = false;
var DocxDoNotExpandShiftReturn = false;

function OnInputClickDOCX() {{
   {template_pscustom}
}}

function DocxRowHeightsFunc(select) {{
    const DocxRowHeightsChange = select.querySelector(`option[value='${{select.value}}']`)
    DocxRowHeights = '&RowHeight=' + DocxRowHeightsChange.value;
}}

function DocxOnRenderModeFunc(select) {{
    const DocxOnRenderModeChange = select.querySelector(`option[value='${{select.value}}']`);
    const matrixBasedValue = DocxOnRenderModeChange.value === ""table"" ? ""true"" : ""false"";
    DocxOnRenderMode = '&PrintFit=' + DocxOnRenderModeChange.value + '&MatrixBased=' + matrixBasedValue;
}}

function DOCXExport() {{
    {validation}

    if (document.getElementById('DocxPrintOptimized').classList.contains('activeButton')) {{
        DocxPrintOptimized = new Boolean(true);
    }}
    else {{ DocxPrintOptimized = false; }};
    if (document.getElementById('DocxDoNotExpandShiftReturn').classList.contains('activeButton')) {{
        DocxDoNotExpandShiftReturn = new Boolean(true);
    }}
    else {{ DocxDoNotExpandShiftReturn = false; }};
    if (document.getElementById('DocxWysiwyg').classList.contains('activeButton')) {{
        DocxWysiwyg = new Boolean(true);
    }}
    else {{ DocxWysiwyg = false; }};
    DocxButtons = ('&PrintOptimized=' + DocxPrintOptimized + '&DoNotExpandShiftReturn=' + DocxDoNotExpandShiftReturn + '&Wysiwyg=' + DocxWysiwyg + DocxRowHeights + DocxOnRenderMode );

    window.location.href = DocxExport.href + DocxButtons + PageSelector;
}}
</script>"; 

        }
       
    }

}
