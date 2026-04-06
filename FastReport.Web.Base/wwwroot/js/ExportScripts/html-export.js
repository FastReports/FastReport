//HTMLEXPORT//
import { AddAction } from './export-utils.js';
var HTMLButtons;
var HTMLPictures = new Boolean(false);
var HTMLSubFolder = new Boolean(false);
var HTMLNavigator = new Boolean(false);
var HTMLSinglePage = new Boolean(false);
var HTMLLayers = new Boolean(false);
var HTMLShowPageBorder = new Boolean(false);
var HTMLCenterAndWrapPages = new Boolean(false)
var HTMLEmbeddingPictures = new Boolean(false);
var HTMLWysiwyg = new Boolean(false);

function HTMLExport() {
    if (document.getElementById('HTMLWysiwyg').classList.contains('activeButton')) {
        HTMLWysiwyg = new Boolean(true);
    }
    else { HTMLWysiwyg = new Boolean(false); };

    if (document.getElementById('HTMLPictures').classList.contains('activeButton')) {
        HTMLPictures = new Boolean(true);
    }
    else { HTMLPictures = new Boolean(false); };

    if (document.getElementById('HTMLSubFolder').classList.contains('activeButton')) {
        HTMLSubFolder = new Boolean(true);
    }
    else { HTMLSubFolder = new Boolean(false); };

    if (document.getElementById('HTMLShowPageBorder').classList.contains('activeButton')) {
        HTMLShowPageBorder = new Boolean(true);
    }
    else { HTMLShowPageBorder = new Boolean(false); };

    if (document.getElementById('HTMLCenterAndWrapPages').classList.contains('activeButton')) {
        HTMLCenterAndWrapPages = new Boolean(true);
    }
    else { HTMLCenterAndWrapPages = new Boolean(false); };

    if (document.getElementById('HTMLNavigator').classList.contains('activeButton')) {
        HTMLNavigator = new Boolean(true);
    }
    else { HTMLNavigator = new Boolean(false); };

    if (document.getElementById('HTMLSinglePage').classList.contains('activeButton')) {
        HTMLSinglePage = new Boolean(true);
    }
    else { HTMLSinglePage = new Boolean(false); };

    if (document.getElementById('HTMLLayers').classList.contains('activeButton')) {
        HTMLLayers = new Boolean(true);
    }
    else { HTMLLayers = new Boolean(false); };

    if (document.getElementById('HTMLEmbeddingPictures').classList.contains('activeButton')) {
        HTMLEmbeddingPictures = new Boolean(true);
    }
    else { HTMLEmbeddingPictures = new Boolean(false); };
    HTMLButtons = ('&Navigator=' + HTMLNavigator + '&Wysiwyg=' + HTMLWysiwyg + '&Pictures=' + HTMLPictures + '&SinglePage=' + HTMLSinglePage + '&Layers=' + HTMLLayers + '&SubFolder=' + HTMLSubFolder + '&EmbedPictures=' + HTMLEmbeddingPictures
        + '&ShowPageBorders=' + HTMLShowPageBorder + '&CenterAndWrapPages=' + HTMLCenterAndWrapPages);

    window.location.href = HtmlExport.href + HTMLButtons + window.frActions.PageSelector;
};

AddAction('HTMLExport', HTMLExport);
