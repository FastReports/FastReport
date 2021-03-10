namespace FastReport.Web
{
    partial class WebReport
    {
        string template_style() => $@"
/**********
    MAIN
***********/

.{template_FR}-container {{
    {(Width.IsNullOrWhiteSpace() ? "" : $"width: {Width};")};
    {(Height.IsNullOrWhiteSpace() ? "" : $"height: {Height};")};
    background-color: white;
    display: {(Inline ? "inline-" : "")}flex;
    flex-direction: column;
    position: relative;
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
}}

.{template_FR}-report {{
    overflow: auto;
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
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
    flex-shrink: 0;
    font-family: Verdana,Arial,sans-serif;
    background-color: {ToolbarColor.Name};
    {(Tabs.Count > 1 ? "" : "box-shadow: 0px 3px 4px -2px rgba(0, 0, 0, 0.2);")}
    display: flex;
    /* flex-wrap: wrap; */
    width: 100%;
    position: relative;
    z-index: 2;
    /*min-width: intrinsic;
    min-width: -moz-max-content;
    min-width: -webkit-max-content;
    min-width: max-content;*/
}}

.{template_FR}-toolbar-item {{
    height: {ToolbarHeight}px;
    position: relative;
}}

.{template_FR}-toolbar-item:hover {{
    background-color: #d1d1d1;
}}

.{template_FR}-toolbar-item > img {{
    height: calc({ToolbarHeight}px * 0.7);
    padding-top: calc({ToolbarHeight}px * 0.15);
    padding-bottom: calc({ToolbarHeight}px * 0.15);
    padding-left: calc({ToolbarHeight}px * 0.25);
    padding-right: calc({ToolbarHeight}px * 0.25);
    opacity: 0.5;
    display: block;
}}

.{template_FR}-toolbar-item:hover > img {{
    opacity: 1;
}}

.{template_FR}-toolbar-notbutton:hover {{
    background-color: transparent;
}}

.{template_FR}-toolbar-notbutton:hover > img {{
    opacity: 0.5;
}}

/**********************
    TOOLBAR DROPDOWN
***********************/

.{template_FR}-toolbar-dropdown-content {{
    display: none;
    box-shadow: 0px 8px 16px 0px rgba(0,0,0,0.2);
    background-color: #f9f9f9;
    min-width: 120px;
    z-index: 2;
    position: absolute;
    top: {ToolbarHeight}px;
    left: 0;
    white-space: nowrap;
}}

.{template_FR}-toolbar-item:hover > .{template_FR}-toolbar-dropdown-content {{
    display: block;
}}

.{template_FR}-toolbar-dropdown-content > a {{
    float: none;
    color: black;
    padding: 6px 12px 6px 8px;
    text-decoration: none;
    display: block;
    text-align: left;
    height: auto;
    font-size: 14px;
    user-select: none;
}}

.{template_FR}-toolbar-dropdown-content > a:hover {{
    background-color: #ddd;
    cursor: pointer;
}}

.{template_FR}-zoom-selected {{
    width: 14px;
    opacity: 0.6;
    display: inline-block;
    font-size: 14px;
}}

/************************
    TOOLBAR NAVIGATION
*************************/

.{template_FR}-toolbar-narrow > img {{
    padding-left: 0px;
    padding-right: 0px;
}}

.{template_FR}-toolbar-slash > img {{
    height: calc({ToolbarHeight}px * 0.44);
    padding-top: calc({ToolbarHeight}px * 0.3);
    padding-bottom: calc({ToolbarHeight}px * 0.26);
    padding-left: 0;
    padding-right: 0;
}}

.{template_FR}-toolbar-item > input {{
    font-family: Arial,sans-serif;
    font-size: calc({ToolbarHeight}px * 0.35);
    text-align: center;
    border: 0;
    background: #fbfbfb;
    border-radius: 4px;

    height: calc({ToolbarHeight}px * 0.68);
    width: 2.5em;
    margin-top: calc({ToolbarHeight}px * 0.17);
    margin-bottom: calc({ToolbarHeight}px * 0.15);
    margin-left: calc({ToolbarHeight}px * 0.1);
    margin-right: calc({ToolbarHeight}px * 0.1);
    padding: 0;
    display: block;
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

.{template_FR}-spinner img {{
    width: 90px;
    height: 90px; 
    left: calc(50% - 50px);
    top: calc(50% - 50px);
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
    width: 100%;
    box-shadow: 0px 3px 4px -2px rgba(0, 0, 0, 0.2);
    position: relative;
    z-index: 1;
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

.{template_FR}-tabs .{template_FR}-tab a img {{
    height: 13px;
    opacity: 0;
}}

.{template_FR}-tabs .{template_FR}-tab.active a img {{
    opacity: 0.5;
}}

.{template_FR}-tabs .{template_FR}-tab:hover a img {{
    opacity: 0.5;
}}

.{template_FR}-tabs .{template_FR}-tab a img:hover {{
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

";
    }
}