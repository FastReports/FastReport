//SLIDER//
import { AddAction } from './export-utils.js';
var SliderOutputImage = '90';
function Slider() {
    var SliderRange = document.getElementsByName('SliderRange');
    var SliderValue = document.getElementsByName('SliderValue');
    for (var i = 0; i < SliderRange.length; i++) {
        SliderValue[i].innerHTML = SliderRange[i].value

    }
    SliderOutputImage = SliderRange[0].value;
}

function JpegQualityFunc(select){
    const JpegQuality = select.querySelector(`option[value='${select.value}']`)
    SliderOutputImage = JpegQuality.value
}

//IMAGE//
var ImageButtons;
var ImageResolutionX = '&ResolutionX=90';
var ImageResolutionY = '&ResolutionY=90';
var ImageQuality = '&JpegQuality=90';
var ImageOptionSettings = document.getElementsByName('ImageEnableOrNot');
var ImageOnImageFormat = '&ImageFormat=Jpeg';
var ImageOnMultiFrameTiffClick = false;
var ImageOnMonochromeTiffClick = false;
var ImageOnSeparateFilesClick = false;

//function OnInputClickIMAGE() {
//    {template_pscustom}
//}
function ImageOnImageFormatFunc(select) {
    const ImageOnImageFormatChange = select.querySelector(`option[value='${select.value}']`)

    if (ImageOnImageFormatChange.value == 'Gif' || ImageOnImageFormatChange.value == 'Png' || ImageOnImageFormatChange.value == 'Bmp' || ImageOnImageFormatChange.value == 'Metafile') {

        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'none';
        ImageOptionSettings[2].style.display = 'none';
        ImageOptionSettings[3].style.display = 'none';
        ImageOptionSettings[5].style.display = 'block';
    }
    else if (ImageOnImageFormatChange.value == 'Jpeg') {

        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'none';
        ImageOptionSettings[2].style.display = 'flex';
        ImageOptionSettings[5].style.display = 'block';


    }
    else if (ImageOnImageFormatChange.value == 'Tiff') {
        ImageOptionSettings[0].style.display = 'block';
        ImageOptionSettings[1].style.display = 'block';
        ImageOptionSettings[2].style.display = 'none';
        ImageOptionSettings[3].style.display = 'block';
        ImageOptionSettings[4].style.display = 'block';
        ImageOptionSettings[5].style.display = 'block';
    }
    ImageOnImageFormat = '&ImageFormat=' + ImageOnImageFormatChange.value;
}
function IMAGEExport() {
    ImageResolutionX = '&ResolutionX=' + document.getElementById('ImageResolutionX').value;
    ImageResolutionY = '&ResolutionY=' + document.getElementById('ImageResolutionY').value;
    ImageQuality = '&JpegQuality=' + SliderOutputImage;

    if (document.getElementById('ImageOnMultiFrameTiffClick').classList.contains('activeButton')) {
        ImageOnMultiFrameTiffClick = new Boolean(true);
    }
    else { ImageOnMultiFrameTiffClick = false; };

    if (document.getElementById('ImageOnMonochromeTiffClick').classList.contains('activeButton')) {
        ImageOnMonochromeTiffClick = new Boolean(true);
    }
    else { ImageOnMonochromeTiffClick = false; };

    if (document.getElementById('ImageOnSeparateFilesClick').classList.contains('activeButton')) {
        ImageOnSeparateFilesClick = new Boolean(true);
    }
    else { ImageOnSeparateFilesClick = false; };

    ImageButtons = ('&MultiFrameTiff=' + ImageOnMultiFrameTiffClick + '&MonochromeTiff=' + ImageOnMonochromeTiffClick + '&SeparateFiles=' + ImageOnSeparateFilesClick);
    window.location.href = ImageExport.href + ImageOnImageFormat + ImageButtons + frActions.PageSelector + ImageQuality + ImageResolutionX + ImageResolutionY;
}

AddAction('IMAGEExport', IMAGEExport);
AddAction('ImageOnImageFormatFunc', ImageOnImageFormatFunc);
AddAction('Slider', Slider);
AddAction('JpegQualityFunc', JpegQualityFunc);
