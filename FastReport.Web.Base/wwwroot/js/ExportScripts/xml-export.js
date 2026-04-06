//XMLEXPORT//
import { AddAction } from './export-utils.js';
var XmlButtons;
var XmlWysiwyg = false;
var XmlPageBreaks = false;
var XmlDataOnly = false;

function XMLExport() {
    if (document.getElementById('XmlWysiwyg').classList.contains('activeButton')) {
        XmlWysiwyg = new Boolean(true);
    }
    else { XmlWysiwyg = false; };

    if (document.getElementById('XmlPageBreaks').classList.contains('activeButton')) {
        XmlPageBreaks = new Boolean(true);
    }
    else { XmlPageBreaks = false; };

    if (document.getElementById('XmlDataOnly').classList.contains('activeButton')) {
        XmlDataOnly = new Boolean(true);
    }
    else { XmlDataOnly = false; };
    XmlButtons = ('&Wysiwyg=' + XmlWysiwyg + '&PageBreaks=' + XmlPageBreaks + '&DataOnly=' + XmlDataOnly);
    window.location.href = XmlExport.href + XmlButtons + frActions.PageSelector;
}

AddAction('XMLExport', XMLExport);
