//DOCXEXPORT//
import { AddAction } from './export-utils.js';
var DocxButtons;
var DocxRowHeights = '&RowHeight=Exactly';
var DocxOnRenderMode = '&PrintFit=layers';
var DocxWysiwyg = false;
var DocxPrintOptimized = false;
var DocxDoNotExpandShiftReturn = false;

function DocxRowHeightsFunc(select) {
    const DocxRowHeightsChange = select.querySelector(`option[value='${select.value}']`)
    DocxRowHeights = '&RowHeight=' + DocxRowHeightsChange.value;
}

function DocxOnRenderModeFunc(select) {
    const DocxOnRenderModeChange = select.querySelector(`option[value='${select.value}']`);
    const matrixBasedValue = DocxOnRenderModeChange.value === "table" ? "true" : "false";
    DocxOnRenderMode = '&PrintFit=' + DocxOnRenderModeChange.value + '&MatrixBased=' + matrixBasedValue;
}

export function DOCXExport() {
    if (document.getElementById('DocxPrintOptimized').classList.contains('activeButton')) {
        DocxPrintOptimized = new Boolean(true);
    }
    else { DocxPrintOptimized = false; };
    if (document.getElementById('DocxDoNotExpandShiftReturn').classList.contains('activeButton')) {
        DocxDoNotExpandShiftReturn = new Boolean(true);
    }
    else { DocxDoNotExpandShiftReturn = false; };
    if (document.getElementById('DocxWysiwyg').classList.contains('activeButton')) {
        DocxWysiwyg = new Boolean(true);
    }
    else { DocxWysiwyg = false; };
    DocxButtons = ('&PrintOptimized=' + DocxPrintOptimized + '&DoNotExpandShiftReturn=' + DocxDoNotExpandShiftReturn + '&Wysiwyg=' + DocxWysiwyg + DocxRowHeights + DocxOnRenderMode );

    window.location.href = DocxExport.href + DocxButtons + frActions.PageSelector;
}

AddAction('DocxRowHeightsFunc', DocxRowHeightsFunc);
AddAction('DocxOnRenderModeFunc', DocxOnRenderModeFunc);
AddAction('DOCXExport', DOCXExport);
