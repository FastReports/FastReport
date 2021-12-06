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
                            <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name =""PageSelectorInput"" pattern=""[0-9,-\s]"" placeholder=""2, 5-132"" type=""text"" value="""" class=""fr-webreport-popup-content-export-parameters-input"" onchange=""OnInputClickPDF()"">
                       </div>
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
                            <div class=""fr-webreport-popup-content-export-parameters-col"">
                                <button id=""PDFOnEmbeddedFonts"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationPdf.EmbeddedFonts}
                                </button>
                                <button  id=""PDFOnTextInCurves"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationPdf.TextInCurves}
                                </button>
                                <button id=""PDFOnInteractive"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                    {localizationPdf.InteractiveForms}
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
                                <div style=""display:flex;"" class=""fr-webreport-popup-content-export-parameters-slider"">
                                    <input type=""range"" min=""1"" max=""100"" value=""90"" name = ""SliderRange"" onchange = ""Slider()"">
                                    <p>{localizationPdf.JpegQuality} <span name=""SliderValue"">90</span></p>
                                </div>   
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <select class=""custom-select""  onchange=""PDFColorSpaceFunc(this)"">
                                <option value=""RGB"" selected>RGB</option>
                                <option value=""CMYK"">CMYK</option>
                            </select>
                            <select class=""custom-select"" onchange = ""PDFImageOptionFunc(this)"">
                                <option value=""None"" selected>None</option>
                                <option value=""PrintOptimized"">{localizationPdf.PrintOptimized}</option>
                                <option value=""ImagesOriginalResolution"">{localizationPdf.OriginalResolution}</option>
                            </select>
                        </div>
                    </div>
                    <div class=""fr-webreport-popup-content-buttons"">
                        <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""PDFExport()"">OK</button>
                   </div>
                </div>
<script>
       {template_modalcontainerscript}
     //SLIDER//
    var SliderOutputPDF = '90';
    function Slider() {{
        var SliderRange = document.getElementsByName('SliderRange');
        var SliderValue = document.getElementsByName('SliderValue');
        for (var i = 0; i < SliderRange.length; i++) {{
            SliderValue[i].innerHTML = SliderRange[i].value

        }}
        SliderOutputPDF = SliderRange[0].value;
    }}
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

    function OnInputClickPDF() {{
       {template_pscustom}
    }}
    function PDFCompilanceFunc(select) {{
        const PDFCompilanceOption = select.querySelector(`option[value='${{select.value}}']`)
        PDFCompilance = '&PdfCompliance=' + PDFCompilanceOption.value;
    }}

    function PDFColorSpaceFunc(select) {{
        const ColorSpaceOption = select.querySelector(`option[value='${{select.value}}']`)
        PDFColorSpace = '&PDFColorSpace=' + ColorSpaceOption.value;
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

        PDFJpegQuality = '&PDFJpegQuality=' + SliderOutputPDF;

        if (document.getElementById('PDFOnEmbeddedFonts').classList.contains('active')) {{
            PDFOnEmbeddedFonts = new Boolean(true);
        }}
        else {{ PDFOnEmbeddedFonts = false; }};

        if (document.getElementById('PDFOnTextInCurves').classList.contains('active')) {{
            PDFOnTextInCurves = new Boolean(true);
        }}
        else {{ PDFOnTextInCurves = false; }};

        if (document.getElementById('PDFOnInteractive').classList.contains('active')) {{
            PDFOnInteractive = new Boolean(true);
        }}
        else {{ PDFOnInteractive = false; }};

        if (document.getElementById('PDFOnBackground').classList.contains('active')) {{
            PDFOnBackground = new Boolean(true);
        }}
        else {{ PDFOnBackground = false; }};
        PDFButtons = ('&EmbeddingFonts=' + PDFOnEmbeddedFonts + '&TextInCurves=' + PDFOnTextInCurves + '&InteractiveForms=' + PDFOnInteractive + '&Background=' + PDFOnBackground);

        window.location.href = PdfExport.href + PDFCompilance + PDFColorSpace + PDFJpegQuality + PageSelector + PDFOnImageOption + PDFButtons;

    }}
</script>
            "; 

        }
    }

}
