//XLSXEXPORT//
import { AddAction } from './export-utils.js';
var XlsxButtons;
var XlsxFontScaling = '&FontScale=1';
var XlsxOnPrintFit = '&PrintFit=NoScaling';
var XlsxPageBreaks = false;
var XlsxDataOnly = false;
var XlsxWysiwyg = false;
var XlsxSeamless = false;
var XlsxPrintOptimized = false;
var XlsxSplitPages = false;

function XlsxFontScalingFunc(select){
    const XlsxFontScalingChange = select.querySelector(`option[value='${select.value}']`)
    XlsxFontScaling = '&FontScale=' + XlsxFontScalingChange.value.replace('.', ',');
}

function XlsxOnPrintFitChangeFunc(select) {
    const XlsxOnPrintFitChange = select.querySelector(`option[value='${select.value}']`)
    XlsxOnPrintFit = '&PrintFit=' + XlsxOnPrintFitChange.value;
 }

function XLSXExport() {
    if (document.getElementById('XlsxWysiwyg').classList.contains('activeButton')) {
        XlsxWysiwyg = new Boolean(true);
    }
    else { XlsxWysiwyg = false; };

    if (document.getElementById('XlsxPageBreaks').classList.contains('activeButton')) {
        XlsxPageBreaks = new Boolean(true);
    }
    else { XlsxPageBreaks = false; };
    if (document.getElementById('XlsxDataOnly').classList.contains('activeButton')) {
        XlsxDataOnly = new Boolean(true);
    }
    else { XlsxSeamless = false; };
    if (document.getElementById('XlsxSeamless').classList.contains('activeButton')) {
        XlsxSeamless = new Boolean(true);
    }
    else { XlsxPrintOptimized = false; };
    if (document.getElementById('XlsxPrintOptimized').classList.contains('activeButton')) {
        XlsxPrintOptimized = new Boolean(true);
    }
    else { XlsxPrintOptimized = false; };
    if (document.getElementById('XlsxSplitPages').classList.contains('activeButton')) {
        XlsxSplitPages = new Boolean(true);
    }
    else { XlsxSplitPages = false; };

    XlsxButtons = (XlsxOnPrintFit + '&Wysiwyg=' + XlsxWysiwyg + '&PrintOptimized=' + XlsxPrintOptimized + '&DataOnly=' + XlsxDataOnly + '&Seamless=' + XlsxSeamless + '&SplitPages=' + XlsxSplitPages + '&PageBreaks=' + XlsxPageBreaks);
    window.location.href = XlsxExport.href + XlsxButtons + frActions.PageSelector + XlsxFontScaling;
}

AddAction('XlsxFontScalingFunc', XlsxFontScalingFunc);
AddAction('XlsxOnPrintFitChangeFunc', XlsxOnPrintFitChangeFunc);
AddAction('XLSXExport', XLSXExport);
