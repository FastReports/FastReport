//ODTEXPORT//
import { AddAction } from './export-utils.js';
var OdtButtons;
var OdtOnCompliance = '&OdfCompliance=None';
var OdtWysiwyg = false;
var OdtPageBreaks = false;

function OdtOnComplianceChangeFunc(select) {
    const OdtOnComplianceChange = select.querySelector(`option[value='${select.value}']`)
    OdtOnCompliance = '&OdfCompliance=' + OdtOnComplianceChange.value;
}
function ODTExport() {
    if (document.getElementById('OdtWysiwyg').classList.contains('activeButton')) {
        OdtWysiwyg = new Boolean(true);
    }
    else { OdtWysiwyg = false; };
    if (document.getElementById('OdtPageBreaks').classList.contains('activeButton')) {
        OdtPageBreaks = new Boolean(true);
    }
    else { OdtPageBreaks = false; };
    OdtButtons = ('&Wysiwyg=' + OdtWysiwyg + '&PageBreaks=' + OdtPageBreaks);

    window.location.href = OdtExport.href + OdtButtons + OdtOnCompliance;
}

AddAction('OdtOnComplianceChangeFunc', OdtOnComplianceChangeFunc);
AddAction('ODTExport', ODTExport);
