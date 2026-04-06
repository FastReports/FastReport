//SVGEXPORT
import { AddAction } from './export-utils.js';
var SVGImageFormat = '&ImageFormat=Jpeg';
var SVGEmbedPictures = false;
var SVGHasMultiplyFiles = false;
var SVGButtons;

function SVGImageFormatFunc(select) {
    const ImageFormatOption = select.querySelector(`option[value='${select.value}']`)
    if (ImageFormatOption.value == 'None') { ImageFormatOption.value = ''; }
    else { SVGImageFormat = '&ImageFormat=' + ImageFormatOption.value; }
}
function SVGExport() {
    if (document.getElementById('SVGEmbedPictures').classList.contains('activeButton')) {
        SVGEmbedPictures = new Boolean(true);
    }
    else { SVGEmbedPictures = false; };

    if (document.getElementById('SVGOnHasMultipleFilesClick').classList.contains('activeButton')) {
        SVGHasMultiplyFiles = new Boolean(true);
    }
    else { SVGHasMultiplyFiles = false; };

    SVGButtons = ('&EmbedPictures=' + SVGEmbedPictures + '&HasMultipleFiles=' + SVGHasMultiplyFiles);

    window.location.href = SvgExport.href + SVGImageFormat + SVGButtons + frActions.PageSelector;
}

AddAction('SVGImageFormatFunc', SVGImageFormatFunc);
AddAction('SVGExport', SVGExport);
