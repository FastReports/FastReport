//PDFEXPORT//
import { AddAction } from './export-utils.js';
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

function OnRgbClick() {
    let rgb = document.getElementById('RGB');
    let cmyk = document.getElementById('CMYK');

    if (!document.getElementById('RGB').classList.contains('activeButton')) {
        cmyk.classList.remove('activeButton');
        rgb.classList.add('activeButton');
    }

}

function OnCmykClick() {
    let rgb = document.getElementById('RGB');
    let cmyk = document.getElementById('CMYK');

    if (!document.getElementById('CMYK').classList.contains('activeButton')) {
        rgb.classList.remove('activeButton');
        cmyk.classList.add('activeButton');
    }

}

function PDFCompilanceFunc(select) {
    const PDFCompilanceOption = select.querySelector(`option[value='${select.value}']`)
    PDFCompilance = '&PdfCompliance=' + PDFCompilanceOption.value;
}

function PDFJpegQualityFunc(select) {
    const JpegQuality = select.querySelector(`option[value='${select.value}']`)
    PDFJpegQuality = '&PDFJpegQuality=' + JpegQuality.value;
}

function PDFImageOptionFunc(select) {
    const ImageOption = select.querySelector(`option[value='${select.value}']`)
    if (ImageOption.value == 'None') {
        PDFPrintOptimized = false;
        PDFImagesOriginalResolution = false;
    }
    if (ImageOption.value == 'PrintOptimized') {
        PDFPrintOptimized = new Boolean(true);
        PDFImagesOriginalResolution = false;
    }
    if (ImageOption.value == 'ImagesOriginalResolution') {
        PDFPrintOptimized = false;
        PDFImagesOriginalResolution = new Boolean(true);
    }
    PDFOnImageOption = '&PDFPrintOptimized=' + PDFPrintOptimized + '&PDFImagesOriginalResolution=' + PDFImagesOriginalResolution;
}

function PDFExport() {
    if (document.getElementById('PDFOnEmbeddedFonts').classList.contains('activeButton')) {
        PDFOnEmbeddedFonts = new Boolean(true);
    }
    else { PDFOnEmbeddedFonts = false; };

    if (document.getElementById('RGB').classList.contains('activeButton')) {
        PDFColorSpace = '&PDFColorSpace=' + 'RGB';
    }
    else { PDFColorSpace = '&PDFColorSpace=' + 'CMYK'; }

    if (document.getElementById('PDFOnTextInCurves').classList.contains('activeButton')) {
        PDFOnTextInCurves = new Boolean(true);
    }
    else { PDFOnTextInCurves = false; };

    if (document.getElementById('PDFOnInteractive').classList.contains('activeButton')) {
        PDFOnInteractive = new Boolean(true);
    }
    else { PDFOnInteractive = false; };

    if (document.getElementById('PDFOnBackground').classList.contains('activeButton')) {
        PDFOnBackground = new Boolean(true);
    }
    else { PDFOnBackground = false; };
    PDFButtons = ('&EmbeddingFonts=' + PDFOnEmbeddedFonts + '&TextInCurves=' + PDFOnTextInCurves + '&InteractiveForms=' + PDFOnInteractive + '&Background=' + PDFOnBackground);

    window.location.href = PdfExport.href + PDFCompilance + PDFColorSpace + PDFJpegQuality + frActions.PageSelector + PDFOnImageOption + PDFButtons;

}

AddAction('OnRgbClick', OnRgbClick);
AddAction('OnCmykClick', OnCmykClick);
AddAction('PDFCompilanceFunc', PDFCompilanceFunc);
AddAction('PDFJpegQualityFunc', PDFJpegQualityFunc);
AddAction('PDFImageOptionFunc', PDFImageOptionFunc);
AddAction('PDFExport', PDFExport);

