using System.Drawing;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_style() => $@"
/**********
    MAIN
***********/
.{template_FR}-container {{
    {(Width.IsNullOrWhiteSpace() ? "" : $"width: {Width};")}
    {(Height.IsNullOrWhiteSpace() ? "" : $"height: {Height};")}
    background-color: white;
    display: {InlineStyle};
    flex-direction: {Toolbar.Vertical};
    position: relative;
    align-items: {Toolbar.Content};
}}

.{template_FR}-container * {{
    box-sizing: content-box;
    -moz-box-sizing: content-box;
}}

.{template_FR}-body {{
    display: flex;
    overflow: hidden;
    width: 100%;
    height: 100%;
    margin-top: 20px;
}}

.{template_FR}-report {{
    overflow: auto;
    width: 100%;
    display: flex;
    flex-direction: {Toolbar.RowOrColumn};
    align-items: flex-start;
}}

.{template_FR}-spinner[style*=""display:none""] ~ .{template_FR}-toolbar ~ .{template_FR}-body {{
  box-shadow: 0px 2px 4px rgba(0, 0, 0, 0.25);
}}

.fr-form-header {{
    padding: 5px;
    padding-left: 12px;
    font: 14px Verdana,Arial sans-serif Regular;
    min-height: 18px;
    text-overflow: ellipsis;
    overflow: hidden;
    border-bottom: solid 1px lightgray;
    vertical-align: middle;
    text-align: -webkit-match-parent;
    height: 30px;
    align-items: center;
    display: flex;
    font-weight: bold;
}}

.{template_FR}-body:has(.fr-dialog-form) {{
    width: fit-content;
    border-radius: 12px;
    background: #FFF;
    box-shadow: 0px 4px 4px 0px rgba(0, 0, 0, 0.25);
}}

/***********
    SPLIT
************/

.{template_FR}-gutter {{
    background-color: #f1f1f1;
    background-repeat: no-repeat;
    background-position: 50%;
}}

.{template_FR}-gutter.{template_FR}-gutter-horizontal {{
    background-image: url('data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAUAAAAeCAYAAADkftS9AAAAIklEQVQoU2M4c+bMfxAGAgYYmwGrIIiDjrELjpo5aiZeMwF+yNnOs5KSvgAAAABJRU5ErkJggg==');
    cursor: ew-resize;
}}

/*************
    TOOLBAR
**************/

.{template_FR}-toolbar {{
    flex-shrink: 1;
    font:{Toolbar.UserFontSettings};
    background-color: {ColorTranslator.ToHtml(Toolbar.Color)};
    /* {(Tabs.Count > 1 ? "" : "box-shadow: 0px 3px 4px -2px rgba(0, 0, 0, 0.2);")} */
    display: flex;
    flex-direction: {Toolbar.RowOrColumn};
    /* flex-wrap: wrap; */
    width: fit-content;
    height: {Toolbar.VerticalToolbarHeight};
    order:{Toolbar.TopOrBottom} ;
    position: relative;
    align-items: center;
    justify-content:{Toolbar.Content};
    z-index: 2;
    border-radius: {Toolbar.ToolbarRoundness}px;
    /*min-width: intrinsic;
    min-width: -moz-max-content;
    min-width: -webkit-max-content;
    min-width: max-content;*/
    -webkit-user-select: none; /* Safari */
    -ms-user-select: none; /* IE 10 and IE 11 */
    user-select: none; /* Standard syntax */
    {Toolbar.StickyToolbarTags}
}}

.{template_FR}-toolbar-item {{
    height: {Toolbar.Height}px;
    border: none;
    border-radius:{Toolbar.ToolbarRoundness}px;
    background-color: #00000000;
    position: relative;
    align-items: center;
    display: flex;
}}

.{template_FR}-toolbar-item:hover:not([hidden]) {{
    background-color: {ColorTranslator.ToHtml(Toolbar.Color)};
}}

.{template_FR}-toolbar-item[hidden] > svg {{
    filter: invert(99%) sepia(5%) saturate(91%) hue-rotate(66deg) brightness(114%) contrast(86%);
}}

.{template_FR}-toolbar-item > svg {{
    height: calc({Toolbar.Height}px * 0.5);
    padding-top: calc({Toolbar.Height}px * 0.15);
    padding-bottom: calc({Toolbar.Height}px * 0.15);
    padding-left: calc({Toolbar.Height}px * 0.25);
    padding-right: calc({Toolbar.Height}px * 0.25);
    opacity: {Toolbar.TransparencyIcon};
    display: block;
    filter:invert({Toolbar.ColorIcon});
    margin-left: 10px;
    margin-right: 10px;
}}

.{template_FR}-toolbar-item:hover:not([hidden]) > svg {{
    opacity: 0.5;
}}

.{template_FR}-toolbar-notbutton:hover {{
    background-color: transparent;
}}

.{template_FR}-toolbar-notbutton:hover > svg {{
    opacity: 1;
}}

.{template_FR}-toolbar-image{{
    width: calc({Toolbar.Height}px * 0.5);
}}

/****************
    SEARCH FORM
*****************/
.{template_FR}-toolbar-dropdown-content-searchbox {{
    float: none;
    background-color: white;
    text-decoration: none;
    text-align: left;
    height: auto;
    user-select: none;
    border: 1px solid #000;
    border-radius: 6px; 
}}

.{template_FR}-toolbar-search-form > div > label > input {{
    margin-bottom: 12px;
    border-radius: 3px;
}}

.{template_FR}-toolbar-search-form > div > label:has(:disabled) {{
    color: #CACACA;
}}

.{template_FR}-toolbar-search-form {{
    font-size: 14px;
    display: none;
    position: fixed;
    width: 390px;
    z-index: 4;
    left: calc(50% - 195);
    top: 100px;
    padding: 0px 10px 0px 10px;
    border-radius: 12px;
    box-shadow: 0px 4px 4px 0px rgba(0, 0, 0, 0.2);
    background-color: #EFEFEF;
}}

.search-navigation-info-block {{
    display: flex;
    align-items: center;
    justify-content: center;
    text-align: center;
    margin: 12px 0px 6px 0px;
}}

#fr-search-next > svg, #fr-search-prev > svg {{
    vertical-align: middle;
    height: 12px;
}}

#fr-search-next, #fr-search-prev {{
    background-color: white;
    border: none;
}}

#fr-search-next:disabled > svg, #fr-search-prev:disabled > svg {{
    filter: invert(99%) sepia(5%) saturate(91%) hue-rotate(66deg) brightness(114%) contrast(86%);
}}

#fr-search-next:hover:enabled, #fr-search-prev:hover:enabled {{
    /*   background-color: #d9d9d9; */
}}

#fr-search-next:active:enabled, #fr-search-prev:active:enabled {{
    background-color: #4A4A4A;
}}

#fr-search-next:active:enabled > svg, #fr-search-prev:active:enabled > svg {{
    filter: invert(100%) sepia(100%) saturate(0%) hue-rotate(241deg) brightness(103%) contrast(103%);
}}

#fr-search-prev{{
    margin-right: -5px;
    border-radius: 3px 0px 0px 3px;
}}

#fr-search-next {{
    border-radius: 0px 3px 3px 0px;
}}

#fr-search-text {{
    min-height: 25px;
    width: 90%;
    border: none;
    outline: none;
    vertical-align: middle;
    -webkit-appearance: none;
    border-radius: 6px;
}}

#clear-searchbox {{
    width: 20px; 
    height: 20px; 
    border: none;
    vertical-align: middle;
}}

#close-search-form-button {{
    font-size: 10px;
    text-align: right;
    padding: 6px 0px 0px 0px;
}}

#fr-WebRepot-text-info {{
    width: 90%;
}}

#close-search-form-button > img {{
    height: 20px;
    width: 20px;
    filter: invert(36%) sepia(0%) saturate(0%) hue-rotate(85deg) brightness(100%) contrast(91%);
}}

#{template_FR}-toolbar-search-form.open {{
    display: block;
}}

.search-highlight{{
    background-color: {ColorTranslator.ToHtml(Toolbar.SearchHighlight)};
}}

/**********************
    TOOLBAR DROPDOWN
***********************/

.{template_FR}-toolbar-dropdown-content {{
    display: none;
    box-shadow: 0px 8px 16px 0px rgba(0,0,0,0.2);
    background-color: {ColorTranslator.ToHtml(Toolbar.DropDownMenuColor)};
    min-width: 50px;
    z-index: 2;
    position: absolute;
    {Toolbar.DropDownMenuPosition}
    white-space: nowrap;
    border-radius: {Toolbar.DropDownListBorder};
}}

.{template_FR}-toolbar-item:hover:not([hidden]) > .{template_FR}-toolbar-dropdown-content {{
    display: block;
}}

.{template_FR}-toolbar-dropdown-content > a {{
    float: none;
    color: {ColorTranslator.ToHtml(Toolbar.DropDownMenuTextColor)};
    padding: 6px 12px 6px 8px;
    text-decoration: none;
    display: block;
    text-align: left;
    height: auto;
    font-size: 14px;
    user-select: none;
}}

.{template_FR}-toolbar-dropdown-content > a:hover {{
    background-color: {ColorTranslator.ToHtml(Toolbar.DropDownMenuColor)};
    opacity: 0.5;
    cursor: pointer;
    border-radius: 0px 0px 10px 10px;
}}

.{template_FR}-zoom-selected {{
    font-weight: bold;
}}
.modalcontainer-overlay {{
    position: absolute;
    left: 0;
    top: 0;
    right: 0;
    bottom: 0;
    background-color: rgba(0, 0, 0, 0.7);
    display: flex;
    align-items: flex-start;
    justify-content: flex-start;
    opacity: 0;
    z-index: 2;
    visibility: hidden;
    transition: all 0.3s ease-in-out;
    align-content: flex-start;
}}

.modalcontainer {{
    background-color: #fff;
	width: 300px;
	height: 300px;
	display: flex;
	align-items: center;
	justify-content: center;
	display: none;
}}

.modalcontainer-overlay--visible {{
    opacity: 1;
	visibility: visible;
	transition: all 0.3s ease-in-out;
    {Toolbar.Exports.FixedContainerPosition}
}}

.modalcontainer--visible {{
    display: flex;
    width: fit-content;
    height: fit-content;
    witdh: auto;
    min-width: 348px;
    height: auto;
    margin-top: 4rem;
    margin-left: 4rem;
    padding: 0px 9px 50px 9px;
    z-index: 4;
    border-radius: 30px 30px 12px 12px;
    background-color: #EFEFEF;
    flex-wrap: nowrap;
    align-content: center;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    font-family: Arial,Verdana sans-serif;
    {Toolbar.Exports.FixedContainerTags}
    {Toolbar.ModalContainerPosition}
}}
////////////////////
       POPUP
///////////////////
.fr-webreport-popup {{
    min-width: 100%;
    position: absolute;
    min-height: 100%;
    z-index: 3;
    padding-bottom: 1rem;
    background-color: #0000005c;
    display: flex;
    align-content: flex-start;
    flex-direction: row;
    flex-wrap: nowrap;
    justify-content: flex-start;
    align-items: flex-start;
}}

.fr-webreport-popup-content {{
    display: flex;
    width: fit-content;
    height: fit-content;
    margin-top: 2.5rem;
    margin-left: 2.6rem;
    padding: 0px 10px 50px 10px;
    z-index: 4;
    border-radius: 10px;
    background-color: white;
    flex-wrap: nowrap;
    align-content: center;
    justify-content: center;
    align-items: center;
    flex-direction: column;
    font-family: Arial,Verdana sans-serif;
}}

.fr-webreport-popup-content-title {{
    display: flex;
    width: 100%;
    background-color: #35363A;
    color: white;
    font-weight: bold;
    box-shadow: 0rem -0.1rem 0rem 0.6rem #35363A;
    margin-top: 10px; 
    margin-bottom: 10px;
    align-items: center;
    flex-direction: column;
    flex-wrap: nowrap;
    justify-content: flex-start;
    align-content: center;
    font: {Toolbar.Exports.UserFontSettingsStyle} 16px {Toolbar.Exports.UserFontSettingsFamily};
    border-radius: 12px 12px 0px 0px;
}}

.fr-webreport-popup-content-export-parameters-page-range-title{{
    margin-top: 25px;
}}

.fr-webreport-popup-content-title input {{
    background-color: white;
    border: 3px solid  {ColorTranslator.ToHtml(Toolbar.Exports.Color)};
    color: black;
    max-height: 9.8px;
    border-radius: 3px;
}}

button{{
    text-align: left;
}}

.fr-webreport-popup-content-export-parameters {{
    display: flex;
    width: 100%;
    -ms-flex-wrap: wrap;
    flex-direction: column;
    border-radius: 3px;
    padding-bottom: 1rem;
    align-content: flex-start;
    flex-wrap: wrap;
    align-items: flex-start;
    justify-content: flex-start;
    font: {Toolbar.Exports.UserFontSettingsStyle} 14px {Toolbar.Exports.UserFontSettingsFamily};
    font-weight: bold;
}}

.fr-webreport-popup-content-export-parameters-col {{
    display: flex;
    align-content: center;
    flex-wrap: nowrap;
    flex-direction: column;
    font: {Toolbar.Exports.UserFontSettingsStyle} 11px {Toolbar.Exports.UserFontSettingsFamily};
}}
.custom-select{{
    width:auto;
    outline: none;
    max-width: 170px;
    min-width: 20px;
    border: none;
    margin: 5px 5px 5px 5px;
    border-radius: 3px;
    height: 25px;
    font: {Toolbar.Exports.UserFontSettingsStyle} 12px {Toolbar.Exports.UserFontSettingsFamily};
    font-size: 11px;
    overflow: hidden;
    background: #ffffff url('data:image/svg+xml;base64,{GerResourceBase64("select-arrow.svg")}') no-repeat;
    background-position: calc(100% - 10px) center;
    -moz-appearance:none; /* Firefox */
    -webkit-appearance:none; /* Safari and Chrome */
    appearance:none;
    padding:0 30px 0 10px !important;
    -webkit-padding-end: 30px !important;
    -webkit-padding-start: 10px !important;
}}
.fr-webreport-popup-content-export-parameters-input {{
    margin-left: 0.3rem;
    margin-bottom: 0.3rem;
    padding: 6px;
    max-width: 70px;
    height: 8px;
    outline: none;
    border: none;
    margin-left: 5px;
    margin-right: 5px;
    min-width: inherit;
    border-radius: 4px;
    font: {Toolbar.Exports.UserFontSettingsStyle} 10px {Toolbar.Exports.UserFontSettingsFamily};
}}

.input-error
{{
    animation: shake 0.2s ease-in-out 0s 2;
    box-shadow: 0 0 0.5em red;
}}

@keyframes shake {{
  0% {{ margin-left: 0rem; }}
  25% {{ margin-left: 0.5rem; }}
  75% {{ margin-left: -0.5rem; }}
  100% {{ margin-left: 0rem; }}
}}

.fr-webreport-popup-content-export-parameters-row {{
    display: flex;
    padding-top: 5px;
    flex-direction: row;
    align-items: flex-start;
    justify-content: flex-start;
}}
.fr-webreport-popup-content-export-parameters-slider {{
    display: flex;
    margin: 0rem 0rem 0rem 0.35rem;
    background-color: transparent;
    border-radius: 10px;
    justify-content: flex-start;
    align-items: center;
    align-content: center;
    flex-direction: row;
}}

.fr-webreport-popup-content-export-parameters-slider span {{
    color: black;
    min-width: 128px;
    font: {Toolbar.Exports.UserFontSettingsStyle} 11px {Toolbar.Exports.UserFontSettingsFamily};
    font-weight: normal;
    white-space: nowrap;
}}
input[type=range] {{
    height: 1.7rem;
    overflow: hidden;
    -webkit-appearance: none;
    margin-left: 0;
    outline: none;
    background-color: #424242;
}}

input[type=range]::-webkit-slider-runnable-track {{
    width: 100%;
    height: 100%;
    cursor: pointer;
    animate: 0.2s;
    background: #424242;
    border-radius: 0px;
}}
input[type=range]::-webkit-slider-thumb {{
    height: 100%;
    width: 5%;
    border-radius: 0px;
    background: linear-gradient(gray,5%, {ColorTranslator.ToHtml(Toolbar.Exports.Color)});
    cursor: pointer;
    -webkit-appearance: none;
    box-shadow: -100vw 0vw 0vw 100vw {ColorTranslator.ToHtml(Toolbar.Exports.Color)};
    margin-top: 0px;
}}

label{{
     margin: 5px 0px;
}}
.fr-webreport-popup-content-export-parameters-col input[type=text] {{
    background-color: white;
    color: #000000;
    border: 2px solid  {ColorTranslator.ToHtml(Toolbar.Exports.Color)};
    max-height: 11px;
}}
.fr-webreport-popup-content-export-parameters-button {{
    padding: 5px;
    outline: none; 
    border: none;
    margin-left: 5px;
    margin-right: 5px;
    min-width: inherit; 
    font: {Toolbar.Exports.UserFontSettingsStyle} 11px {Toolbar.Exports.UserFontSettingsFamily};
    color: black;
    background: url(/_fr/resources.getResource?resourceName=button.svg&contentType=image%2Fsvg%2Bxml) no-repeat;
    background-position: 0px center;
    padding-left: 20px;
    vertical-align: middle;
    cursor: pointer;
}}

.fr-webreport-popup-content-buttons {{
    display: flex;
    margin-bottom: -2.3rem;
    width: 100%;
    flex-wrap: nowrap;
    align-content: center;
    justify-content: space-between;
    align-items: center;
    flex-direction: row;
}}

.fr-webreport-popup-content-btn-submit {{
    outline: none;
    border: none;
    background-color: #DD4433;
    border-radius: 3px;
    padding: 3px;
    font: {Toolbar.Exports.UserFontSettingsStyle} 14px {Toolbar.Exports.UserFontSettingsFamily};
    color: {ColorTranslator.ToHtml(Toolbar.Exports.FontColor)};
    min-width: 70px;
    width: fit-content;
    height: 30px;
    cursor: pointer;
    text-align: center;
}}

.fr-webreport-popup-content-btn-cancel{{
    padding: 3px;
    background-color: #D9D9D9;
    font: {Toolbar.Exports.UserFontSettingsStyle} 14px {Toolbar.Exports.UserFontSettingsFamily};
    min-width: 70px;
    width: fit-content;
    height: 30px;
    border-radius: 3px;
    border: none;
    outline: none;
    color: black;
    text-align: center;
}}

.fr-webreport-popup-content-btn-submit:hover {{
    transform: scale(1.015);
}}

.{template_FR}-container .activeButton {{
    background: url('data:image/svg+xml;base64,{GerResourceBase64("button-active.svg")}') no-repeat;
    background-position: 0px center;
    vertical-align: middle;
}}


.fr-webreport-settings-btn {{
    background-color: transparent;
    color: transparent;
    padding-left: 1rem;
    margin-left: 230px;
    display: flex;
    float: right;
    margin-top: -1.6rem;
    outline: none;
    padding-right: 1rem;
    border: none;
    position: relative;
    z-index: 4;
    flex-direction: row;
    align-content: space-around;
    justify-content: space-between;
    filter:alpha(opacity=50);
    opacity: 1;
}}
.fr-webreport-settings-btn:hover{{
    background-color: transparent;
    color: transparent;
    padding-left: 1rem;
    margin-left: 230px;
    display: flex;
    float: right;
    margin-top: -1.6rem;
    outline: none;
    padding-right: 1rem;
    border: none;
    position: relative;
    z-index: 4;
    flex-direction: row;
    align-content: space-around;
    justify-content: space-between;
    transform: scale(1.1);
    filter:alpha(opacity=0);
    opacity: 0.5;
    cursor: pointer;
}}
.fr-webreport-settings-btn::-moz-focus-inner {{
    background-color: transparent;
    color: transparent;
    padding-left: 1rem;
    margin-left: 230px;
    display: flex;
    float: right;
    margin-top: -1.6rem;
    outline: none;
    padding-right: 1rem;
    border: none;
    position: relative;
    z-index: 4;
    flex-direction: row;
    align-content: space-around;
    justify-content: space-between;
}}

.fr-webreport-popup-disabled-button{{
    background-color: #D9D9D9;
    cursor: default;
}}

/************************
    TOOLBAR NAVIGATION
*************************/

.{template_FR}-toolbar-narrow > svg {{
    transform: rotate({Toolbar.ToolbarNarrow}deg);
    padding-left: 0px;
    padding-right: 0px;
    height: calc({Toolbar.Height}px * 0.35);
    padding-top: 9px;
}}

.{template_FR}-toolbar-slash{{
 }}

.{template_FR}-toolbar-slash > svg {{
    margin-left: 0px;
    margin-right: 0px;
    height: calc({Toolbar.Height}px * 0.35);
    padding-top: 9px;
    margin: 5px 0px 0px;
    padding-right: 5px;
    padding-left: 5px;
}}

.{template_FR}-toolbar-item > input {{
    font: {Toolbar.Exports.UserFontSettingsStyle} 12px {Toolbar.Exports.UserFontSettingsFamily};
    font-size: calc({Toolbar.Height}px * 0.35);
    text-align: center;
    border: 0;
    background: #fbfbfb;
    border-radius:{Toolbar.ToolbarRoundness}px;
    height: calc({Toolbar.Height}px * 0.68);
    width: 3.5em;
    margin-top: calc({Toolbar.Height}px * 0.17);
    margin-bottom: calc({Toolbar.Height}px * 0.15);
    margin-left: calc({Toolbar.Height}px * 0.1);
    margin-right: calc({Toolbar.Height}px * 0.1);
    padding: 0;
    display: block;
    border-radius: 5px;
}}

.{template_FR}-toolbar-item > input:hover:not([readonly]) {{
    background: #fff;
}}

.{template_FR}-toolbar-item > input[readonly] {{
    cursor: default;
}}

/**************
    SPINNER
**************/

.{template_FR}-spinner {{
    height: 100%;
    width: 100%;
    position: absolute;
    background-color: rgba(255, 255, 255, 0.7);
    z-index: 10;
}}

.{template_FR}-spinner svg {{
    width: 90px;
    height: 90px; 
    left: calc(50%-50px);
    top: calc(50%-50px);
    position: absolute;
    animation: {template_FR}-spin 1s infinite steps(8);
    opacity: 0.5;
}}

@keyframes {template_FR}-spin {{
	from {{ -webkit-transform: rotate(0deg); }}
	to {{ -webkit-transform: rotate(360deg); }}
}}

/************
    ERROR
************/

.{template_FR}-error-container {{
    width: 100%;
    height: 100%;
    display: flex;
    flex-direction: column;
    overflow: auto;
}}

.{template_FR}-error-text {{
    color: red;
    font-family: Consolas,monospace;
    font-size: 16px;
    margin: 20px;
    text-align: center;
}}

.{template_FR}-error-response {{
    height: 100%;
    position: relative;
}}

/***********
    TABS
***********/

.{template_FR}-tabs {{
    flex-shrink: 0;
    font-family: Verdana,Arial,sans-serif;
    background-color: #f1f1f1;
    display: table;
    width: {Toolbar.TabsPositionSettings};
    max-width: {ReportMaxWidth}px;
    box-shadow: 0px 3px 4px -2px rgba(0, 0, 0, 0.2);
    position: relative;
    border-radius: 9px;
    z-index: 1;
    {Toolbar.StickyToolbarTabsTags}
    {Toolbar.TabsPositionSettings}
}}


.{template_FR}-tabs .{template_FR}-tab {{
    float: left;
    display: block;
    color: #3b3b3b;
    text-align: center;
    text-decoration: none;
    font-size: 12px;
    background-color: #f1f1f1;
    margin-top: 2px;
    margin-right: 2px;
    height: 24px;
    border-radius: 9px;
}}

.{template_FR}-tabs .{template_FR}-tab-title {{
    display: block;
    float: left;
    padding: 4.5px 10px;
}}

.{template_FR}-tabs .{template_FR}-tab-close {{
    width: 13px;
    height: 13px;
    display: block;
    float: left;
    margin-top: 6px;
    margin-right: 6px;
}}

.{template_FR}-tabs .{template_FR}-tab-close svg{{
    border-radius: 10px;
}}

.{template_FR}-tabs .{template_FR}-tab:hover {{
    background-color: lightgray;
    color: black;
    cursor: pointer;
}}

.{template_FR}-tabs .{template_FR}-tab.active {{
    background-color: lightgray;
    color: black;
    cursor: default;
}}

.{template_FR}-tabs .{template_FR}-tab a svg {{
    height: 13px;
    opacity: 0;
}}

.{template_FR}-tabs .{template_FR}-tab.active a svg {{
    opacity: 0.5;
}}

.{template_FR}-tabs .{template_FR}-tab:hover a svg {{
    opacity: 0.5;
}}

.{template_FR}-tabs .{template_FR}-tab a svg:hover {{
    opacity: 1;
    background-color: #f1f1f1;
    cursor: pointer;
}}

/***********
    MISC
***********/

.{template_FR}-pointer:hover {{
    cursor: pointer;
}}

.{template_FR}-disabled {{
    opacity: 0.5;
}}
.datetimepicker > input[type=""date""]{{
    background: url('data:image/svg+xml;base64,{GerResourceBase64("date-picker.svg")}') no-repeat;
    background-size: 10px;
    background-position: right 5px center;
    background-origin: content-box, content-box;
}}
.datetimepicker > input[type=""date""]:disabled {{
    background: url("""") no-repeat;
}}
input[type=date]::-webkit-calendar-picker-indicator {{
    opacity: 0;
}}
/**************
    OUTLINE
**************/

.{template_FR}-outline {{
    overflow: auto;
    height: auto;
    font-family: Verdana,Arial,sans-serif;
    font-size: 11px;
}}

.{template_FR}-outline img {{
    opacity: 0.5;
}}

.{template_FR}-outline-inner {{
    padding: 5px;
}}

.{template_FR}-outline-node {{
    display: flex;
    flex-wrap: wrap;
}}

.{template_FR}-outline-caret {{
    height: 14px;
    width: 14px;
    margin-top: 1.5px;
    margin-right: 4px;
}}

.{template_FR}-outline-caret:hover {{
    cursor: pointer;
}}

.{template_FR}-outline-caret-blank {{
    width: 18px;
}}

.{template_FR}-outline-file {{
    height: 14px;
    margin-top: 1.5px;
}}

.{template_FR}-outline-text {{
    white-space: nowrap;
    display: flex;
    align-items: center;
}}

.{template_FR}-outline-text > a {{
    margin: 2px;
    padding: 2px;
}}

.{template_FR}-outline-text > a:hover {{
    text-decoration: underline;
    cursor: pointer;
}}

.{template_FR}-outline-children {{
    padding-left: 20px;
}}

/*******************

    EMAIL EXPORT

*******************/
.fr-notification {{
    position: fixed;
    bottom: 20px;
    right: 20px;
    display: flex; 
    align-items: center;
    padding: 10px 20px;
    border-radius: 4px;
    font-size: 14px;
    color: white;
    opacity: 1;
    transition: opacity 0.5s;
    z-index: 9999;
    font-family: Arial, sans-serif;
}}

.fr-notification-content {{
    display: flex; 
    align-items: center;
}}

.fr-notification-content img {{
    margin-right: 10px;
}}

.fr-notification.positive {{
    background-color: #44cc44;
}}

.fr-required-star {{
    color: red;
}}


.fr-notification.negative {{
    background-color: #cc4444;
}}


.fr-email-export-form {{
    display: flex;
    flex-direction: column;
    padding: 5px;
    box-sizing: border-box;
    width: 100%;
}}

.fr-email-export-field {{
    display: flex;
    margin-bottom: 5px;
    justify-content: space-between;
    width: 100%;
    font-size: 12px;
}}

.fr-email-export-input{{
    width: 244px;
    padding: 8px;
    border: none;
    border-radius: 4px;
    background: #FFF;
    display: flex;
    margin-left: 30px;
    font-size: 12px;
}}

.fr-email-export-textarea{{
    margin - left: 10px;
    width: 244px;
    height: 146px;
    padding: 8px;
    border: none;
    border-radius: 5px;
    resize: none;
    font-size: 12px;
}}

.fr-email-export-select{{
    overflow: hidden;
    -moz-appearance:none; /* Firefox */
    -webkit-appearance:none; /* Safari and Chrome */
    background: #ffffff url('data:image/svg+xml;base64,{GerResourceBase64("select-arrow.svg")}') no-repeat;
    background-position: calc(100% - 10px) center;
    margin - left: 10px;
    width: 244px;
    padding: 8px;
    border-radius: 5px;
    border: none;
    font-size: 12px;
}}

.fr-email-export-label{{
    font-weight: normal;
    font-size: 12px;
}}
";
    }
}