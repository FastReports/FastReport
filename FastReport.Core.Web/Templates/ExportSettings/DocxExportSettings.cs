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

                        <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name =""PageSelectorInput""  onchange=""OnInputClickDOCX()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2, 5-132""value="""" >
                        </div>
                    </div>

                    <div class=""fr-webreport-popup-content-export-parameters"">
                        <label>{localizationDocx.Options}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""DocxWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                    Wysiwyg
                                </button>
                            </div>
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""DocxPrintOptimized"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                    {localizationDocx.PrintOptimized}
                                </button>
                            </div>
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-col"">
                            <button id=""DocxDoNotExpandShiftReturn"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationDocx.DoNotExpandShiftReturn}
                            </button>
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-col"">
                            <div class=""fr-webreport-popup-content-export-parameters-row"">
                                <span style=""margin-left: 0.5rem;align-self: center;font-size: 12px;margin-bottom: 0.5rem;"">{localizationDocx.RowHeightIs}</span>
                                <select class=""custom-select"" onchange=""DocxRowHeightsFunc(this)"">
                                    <option value=""Exactly"" selected>{localizationDocx.Exactly}</option>
                                    <option value=""Minimum"">{localizationDocx.Minimum}</option>
                                </select>
                            </div>
                            <div class=""fr-webreport-popup-content-export-parameters-row"">
                                <span style=""margin-left: 0.5rem;align-self: center;font-size: 12px;margin-bottom: 0.5rem;"">{localizationDocx.Options}</span>
                                <select class=""custom-select""  onchange=""DocxOnRenderModeFunc(this)"">
                                    <option value=""table"">{localizationDocx.Table}</option>
                                    <option value=""layers"" selected>{localizationDocx.Layers}</option>
                                    <option value=""paragraphs"">{localizationDocx.Paragraphs}</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class=""fr-webreport-popup-content-buttons"">
                        <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""DOCXExport()"">OK</button>
                    </div>
                </div>

<script>
    {template_modalcontainerscript}
    //DOCXEXPORT//
    var DocxButtons;
    var DocxRowHeights = '&RowHeightIs=Exactly&MatrixBased=false';
    var DocxOnRenderMode = '&PrintFit=layers';
    var DocxWysiwyg = false;
    var DocxPrintOptimized = false;
    var DocxDoNotExpandShiftReturn = false;

    function OnInputClickDOCX() {{
       {template_pscustom}
   
    }}
    function DocxRowHeightsFunc(select) {{
        const DocxRowHeightsChange = select.querySelector(`option[value='${{select.value}}']`)
        DocxRowHeights = '&RowHeightIs=' + DocxRowHeightsChange.value + '&MatrixBased=false';
    }}
    function DocxOnRenderModeFunc(select) {{
        const DocxOnRenderModeChange = select.querySelector(`option[value='${{select.value}}']`)
        DocxOnRenderMode = '&PrintFit=' + DocxOnRenderModeChange.value;
    }}
    function DOCXExport() {{
        if (document.getElementById('DocxPrintOptimized').classList.contains('active')) {{
            DocxPrintOptimized = new Boolean(true);
        }}
        else {{ DocxPrintOptimized = false; }};
        if (document.getElementById('DocxDoNotExpandShiftReturn').classList.contains('active')) {{
            DocxDoNotExpandShiftReturn = new Boolean(true);
        }}
        else {{ DocxDoNotExpandShiftReturn = false; }};
        if (document.getElementById('DocxWysiwyg').classList.contains('active')) {{
            DocxWysiwyg = new Boolean(true);
        }}
        else {{ DocxWysiwyg = false; }};
        DocxButtons = ('&PrintOptimized=' + DocxPrintOptimized + '&DoNotExpandShiftReturn=' + DocxDoNotExpandShiftReturn + '&Wysiwyg=' + DocxWysiwyg);

        window.location.href = DocxExport.href + DocxButtons + PageSelector;
    }}
</script>

            "; 

        }
       
    }

}
