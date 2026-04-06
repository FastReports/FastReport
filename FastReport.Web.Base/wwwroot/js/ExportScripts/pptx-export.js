//PPTXEXPORT//
import { AddAction } from './export-utils.js';
var PptxImageFormat = '&ImageFormat=Jpeg';

function PptxImageFormatFunc(select) {
    const PptxImageFormatChage = select.querySelector(`option[value='${select.value}']`)
    PptxImageFormat = '&ImageFormat=' + PptxImageFormatChage.value;
}

function PPTXExport() {
    window.location.href = PptxExport.href + PptxImageFormat //+PageSelector;
}

AddAction('PptxImageFormatFunc', PptxImageFormatFunc);
AddAction('PPTXExport', PPTXExport);
