//RTFEXPORT//
import { AddAction } from './export-utils.js';
var RtfOnRichObject = '&Pictures=false&EmbeddedRTF=false';
var RtfOnPictures = '&ImageFormat=Jpeg';
var RtfEmbedded = false;
var RtfPicture = false;

function RtfOnPicturesChangeFunc(select) {
    const RtfOnPicturesChange = select.querySelector(`option[value='${select.value}']`)
        if (RtfOnPicturesChange.value != 'None') {
        RtfOnPictures = '&ImageFormat=' + RtfOnPicturesChange.value;
    }
        else{RtfOnPictures = ''}
}
function RtfOnRichObjectChangeFunc(select) {

    const RtfOnRichObjectChange = select.querySelector(`option[value='${select.value}']`)

    if (RtfOnRichObjectChange.value == 'EmbeddedRTF') {
        RtfPicture = false;
        RtfEmbedded = new Boolean(true);
    }
    if (RtfOnRichObjectChange.value == 'Picture') {
        RtfPicture = new Boolean(true);
        RtfEmbedded = false;
    }

    RtfOnRichObject = '&Pictures=' + RtfPicture + '&EmbeddedRTF=' + RtfEmbedded;
}
function RTFExport() {
    window.location.href = RtfExport.href + RtfOnPictures + RtfOnRichObject + frActions.PageSelector;
}
AddAction('RtfOnPicturesChangeFunc', RtfOnPicturesChangeFunc);
AddAction('RtfOnRichObjectChangeFunc', RtfOnRichObjectChangeFunc);
AddAction('RTFExport', RTFExport);

