//ODSEXPORT//
import { AddAction } from './export-utils.js';
var OdsButtons;
var OdsOnCompliance = '&OdfCompliance=None';
var OdsWysiwyg = false;
var OdsPageBreaks = false;

function OdsOnComplianceChangeFunc(select) {
    const OdsOnComplianceChange = select.querySelector(`option[value='${select.value}']`)
    OdsOnCompliance = '&OdfCompliance=' + OdsOnComplianceChange.value;
}
function ODSExport() {
    if (document.getElementById('OdsWysiwyg').classList.contains('activeButton')) {
        OdsWysiwyg = new Boolean(true);
    }
    else { OdsWysiwyg = false; };
    if (document.getElementById('OdsPageBreaks').classList.contains('activeButton')) {
        OdsPageBreaks = new Boolean(true);
    }
    else { OdsPageBreaks = false; };
    OdsButtons = ('&Wysiwyg=' + OdsWysiwyg + '&PageBreaks=' + OdsPageBreaks);

    window.location.href = OdsExport.href + OdsButtons + OdsOnCompliance + frActions.PageSelector;
}

AddAction('OdsOnComplianceChangeFunc', OdsOnComplianceChangeFunc);
AddAction('ODSExport', ODSExport);
