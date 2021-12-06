using FastReport.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;
using FastReport.Web.Application.Localizations;
using FastReport.Web.Controllers;

namespace FastReport.Web
{
     partial class WebReport
     {

        internal string template_HtmlExportSettings()
        {
            var localizationHtml = new HtmlExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);

            return $@"
              <div class=""modalcontainer modalcontainer--9"" data-target=""html"">
                <div class=""fr-webreport-popup-content-export-parameters"">
                        <div class=""fr-webreport-popup-content-title"">
                            {localizationHtml.Title}
                        </div>
                        <label>{localizationPageSelector.PageRange}</label>
                        <div class=""fr-webreport-popup-content-export-parameters-row"">
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"" name=""OnAllClick"" onclick=""OnAllClick()"">
                                {localizationPageSelector.All}
                            </button>
                            <button type=""button"" class=""fr-webreport-popup-content-export-parameters-button"" name=""OnFirstClick"" onclick=""OnFirstClick()"">
                                {localizationPageSelector.First}
                            </button>
                            <input name =""PageSelectorInput""  onchange=""OnInputClickHTML()""type=""text""class=""fr-webreport-popup-content-export-parameters-input"" pattern=""[0-9,-\s]"" placeholder=""2, 5-132""value="""" >
                       </div>
                </div>

                <div class=""fr-webreport-popup-content-export-parameters"">
                    <label>{localizationHtml.Options}</label>
                    <div class=""fr-webreport-popup-content-export-parameters-row"">
                        <div class=""fr-webreport-popup-content-export-parameters-col"">
                            <button id=""HTMLWysiwyg"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                Wysiwyg
                            </button>
                            <button id=""HTMLPictures"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                {localizationHtml.Pictures}
                            </button>
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-col"">
                            <button id=""HTMLSubFolder"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button"">
                                {localizationHtml.SubFolder}
                            </button>
                            <button id=""HTMLNavigator"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                {localizationHtml.Navigator}
                            </button>
                        <button id=""HTMLEmbeddingPictures"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                            {localizationHtml.EmbPic}
                        </button>
                        </div>
                        <div class=""fr-webreport-popup-content-export-parameters-col"">
                            <button id=""HTMLSinglePage"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                {localizationHtml.SinglePage}
                            </button>
                            <button id=""HTMLLayers"" type=""button"" class=""fr-webreport-popup-content-export-parameters-button active"">
                                {localizationHtml.Layers}
                            </button>
                        </div>
                    </div>
                </div>
                <div class=""fr-webreport-popup-content-buttons"">
                    <button class=""fr-webreport-popup-content-btn-submit"">{localizationPageSelector.LocalizedCancel}</button>
                    <button class=""fr-webreport-popup-content-btn-submit"" onclick=""HTMLExport()"">OK</button>
                </div>
            </div>
<script>
    {template_modalcontainerscript}
    //HTMLEXPORT//
    var HTMLButtons;
    var HTMLPictures = false;
    var HTMLSubFolder = false;
    var HTMLNavigator = false;
    var HTMLSinglePage = false;
    var HTMLLayers = false;
    var HTMLEmbeddingPictures = false;
    var HTMLWysiwyg = false;

    function OnInputClickHTML() {{
       {template_pscustom}
    }}

    function HTMLExport() {{
        if (document.getElementById('HTMLWysiwyg').classList.contains('active')) {{
            HTMLWysiwyg = new Boolean(true);
        }}
        else {{ HTMLWysiwyg = false; }};

        if (document.getElementById('HTMLPictures').classList.contains('active')) {{
            HTMLPictures = new Boolean(true);
        }}
        else {{ HTMLPictures = false; }};

        if (document.getElementById('HTMLSubFolder').classList.contains('active')) {{
            HTMLSubFolder = new Boolean(true);
        }}
        else {{ HTMLSubFolder = false; }};

        if (document.getElementById('HTMLNavigator').classList.contains('active')) {{
            HTMLNavigator = new Boolean(true);
        }}
        else {{ HTMLNavigator = false; }};

        if (document.getElementById('HTMLSinglePage').classList.contains('active')) {{
            HTMLSinglePage = new Boolean(true);
        }}
        else {{ HTMLSinglePage = false; }};

        if (document.getElementById('HTMLLayers').classList.contains('active')) {{
            HTMLLayers = new Boolean(true);
        }}
        else {{ HTMLLayers = false; }};

        if (document.getElementById('HTMLEmbeddingPictures').classList.contains('active')) {{
            HTMLEmbeddingPictures = new Boolean(true);
        }}
        else {{ HTMLEmbeddingPictures = false; }};
        HTMLButtons = ('&Navigator=' + HTMLNavigator + '&Wysiwyg=' + HTMLWysiwyg + '&Pictures=' + HTMLPictures + '&SinglePage=' + HTMLSinglePage + '&Layers=' + HTMLLayers + PageSelector + '&SubFolder=' + HTMLSubFolder + '&EmbedPictures=' + HTMLEmbeddingPictures);

        window.location.href = HtmlExport.href + HTMLButtons + PageSelector;
    }}
</script>

            "; 

        }
       
    }

}
