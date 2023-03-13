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
    
        internal string template_PdfExportSettings()
        {
            var localizationPdf = new PdfExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);


            return $@"
<div class=""modalcontainer modalcontainer--1"" data-target=""pdf"">
	<div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationPdf.Title}
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
            <input name =""PageSelectorInput"" style=""margin-top: 2px;"" id=""PageSelector"" onchange=""OnInputClickPDF()""type=""text"" class=""fr-webreport-popup-content-export-parameters-input""pattern=""[0-9,-\s]""placeholder=""2 or 10-20""value="""" >
</div>" : "")}
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label> {localizationPdf.Options}</label>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <select style=""align-self:flex-start;"" class=""custom-select"" onchange = ""PDFCompilanceFunc(this)"">
                <option value=""None"" selected>PDF 1.5</option>
                <option value=""PdfA_1a"">PDF/A-1a</option>
                <option value=""PdfA_2a"">PDF/A-2a</option>
                <option value=""PdfA_2b"">PDF/A-2b</option>
                <option value=""PdfA_3a"">PDF/A-3a</option>
                <option value=""PdfA_3b"">PDF/A-3b</option>
                <option value=""PdfX_3"">PDF/X-3</option>
                <option value=""PdfX_4"">PDF/X-4</option>
            </select>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
        <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button id=""PDFOnEmbeddedFonts"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationPdf.EmbeddedFonts}
                </button>
                <button id=""PDFOnInteractive"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationPdf.InteractiveForms}
                </button>
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-col"">
                <button  id=""PDFOnTextInCurves"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationPdf.TextInCurves}
                </button>
                <button id=""PDFOnBackground"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                    {localizationPdf.Background}
                </button>
            </div>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label> {localizationPdf.Images}</label>
        <div class=""fr-webreport-popup-content-export-parameters-col"">
                <div style=""display:flex; font-weight: normal;"" class=""fr-webreport-popup-content-export-parameters-slider"">
                    <p>{localizationPdf.JpegQuality}</p>
                    <select class=""custom-select"" style=""margin-left: 10px;"" onchange = ""PDFJpegQualityFunc(this)"">
                        <option value = ""10"">10</option>
                        <option value = ""20"">20</option>
                        <option value = ""30"">30</option>
                        <option value = ""40"">40</option>
                        <option value = ""50"">50</option>
                        <option value = ""60"">60</option>
                        <option value = ""70"">70</option>
                        <option value = ""80"">80</option>
                        <option value = ""90"" selected>90</option>
                        <option value = ""100"">100</option>
                    </select>
                </div>   
        </div>
        <div class=""fr-webreport-popup-content-export-parameters-row"">
            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button activeButton"" style=""margin-top: 5px;"" id = ""RGB"" name=""OnRgbClick"" onclick=""OnRgbClick()"">
                RGB
            </button>
            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" style=""margin-top: 5px;"" id = ""CMYK"" name=""OnCmykClick"" onclick=""OnCmykClick()"">
                CMYK
            </button>
            <select class=""custom-select"" onchange = ""PDFImageOptionFunc(this)"">
                <option value=""None"" selected>None</option>
                <option value=""PrintOptimized"">{localizationPdf.PrintOptimized}</option>
                <option value=""ImagesOriginalResolution"">{localizationPdf.OriginalResolution}</option>
            </select>
        </div>
    </div>
    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""PDFExport()"" id=""okButton"">OK</button>
   </div>
</div>
<script>
   {template_modalcontainerscript}
//PDFEXPORT//
var PDFCompilance = '&PdfCompliance=None';
var PDFButtons = '&EmbeddingFonts=false&TextInCurves=false&InteractiveForms=false&Background=false';
var PDFColorSpace = '&PDFColorSpace=RGB';
var PDFJpegQuality = '&PDFJpegQuality=90';
var PDFOnImageOption = '&PDFPrintOptimized=false&PDFImagesOriginalResolution=false';
var PDFPrintOptimized = false;
var PDFImagesOriginalResolution = false;
var PDFOnEmbeddedFonts = false;
var PDFOnTextInCurves = false;
var PDFOnInteractive = false;
var PDFOnBackground = false;

function OnRgbClick(){{
    let rgb = document.getElementById('RGB');
    let cmyk = document.getElementById('CMYK');

    if(!document.getElementById('RGB').classList.contains('activeButton')){{
        cmyk.classList.remove('activeButton'); 
        rgb.classList.add('activeButton');
    }}
        
}}

function OnCmykClick(){{
    let rgb = document.getElementById('RGB');
    let cmyk = document.getElementById('CMYK');

    if(!document.getElementById('CMYK').classList.contains('activeButton')){{
        rgb.classList.remove('activeButton');
        cmyk.classList.add('activeButton');
    }}
        
}}

function OnInputClickPDF() {{
   {template_pscustom}
}}
function PDFCompilanceFunc(select) {{
    const PDFCompilanceOption = select.querySelector(`option[value='${{select.value}}']`)
    PDFCompilance = '&PdfCompliance=' + PDFCompilanceOption.value;
}}

function PDFJpegQualityFunc(select){{
    const JpegQuality = select.querySelector(`option[value='${{select.value}}']`)
    PDFJpegQuality = '&PDFJpegQuality=' + JpegQuality.value;
}}

function PDFImageOptionFunc(select) {{
    const ImageOption = select.querySelector(`option[value='${{select.value}}']`)
    if (ImageOption.value == 'None') {{
        PDFPrintOptimized = false;
        PDFImagesOriginalResolution = false;
    }}
    if (ImageOption.value == 'PrintOptimized') {{
        PDFPrintOptimized = new Boolean(true);
        PDFImagesOriginalResolution = false;
    }}
    if (ImageOption.value == 'ImagesOriginalResolution') {{
        PDFPrintOptimized = false;
        PDFImagesOriginalResolution = new Boolean(true);
    }}
    PDFOnImageOption = '&PDFPrintOptimized=' + PDFPrintOptimized + '&PDFImagesOriginalResolution=' + PDFImagesOriginalResolution;
}}

function PDFExport() {{
    {validation}    

    if (document.getElementById('PDFOnEmbeddedFonts').classList.contains('activeButton')) {{
        PDFOnEmbeddedFonts = new Boolean(true);
    }}
    else {{ PDFOnEmbeddedFonts = false; }};

    if (document.getElementById('RGB').classList.contains('activeButton')){{
        PDFColorSpace = '&PDFColorSpace=' + 'RGB';
    }}
    else {{ PDFColorSpace = '&PDFColorSpace=' + 'CMYK'; }}

    if (document.getElementById('PDFOnTextInCurves').classList.contains('activeButton')) {{
        PDFOnTextInCurves = new Boolean(true);
    }}
    else {{ PDFOnTextInCurves = false; }};

    if (document.getElementById('PDFOnInteractive').classList.contains('activeButton')) {{
        PDFOnInteractive = new Boolean(true);
    }}
    else {{ PDFOnInteractive = false; }};

    if (document.getElementById('PDFOnBackground').classList.contains('activeButton')) {{
        PDFOnBackground = new Boolean(true);
    }}
    else {{ PDFOnBackground = false; }};
    PDFButtons = ('&EmbeddingFonts=' + PDFOnEmbeddedFonts + '&TextInCurves=' + PDFOnTextInCurves + '&InteractiveForms=' + PDFOnInteractive + '&Background=' + PDFOnBackground);

    window.location.href = PdfExport.href + PDFCompilance + PDFColorSpace + PDFJpegQuality + PageSelector + PDFOnImageOption + PDFButtons;

    }}
</script>"; 

        }
    }

}
