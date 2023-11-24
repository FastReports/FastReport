#if !OPENSOURCE
using FastReport.Web.Application.Localizations;
using System.IO;

namespace FastReport.Web
{
    partial class WebReport
    {
        internal string template_EmailExportSettings()
        {
            var localizationEmail = new EmailExportSettingsLocalization(Res);
            var localizationPageSelector = new PageSelectorLocalization(Res);
            var localization = new ToolbarLocalization(Res);
            var exports = Toolbar.Exports;

            return $@"
<div class=""modalcontainer modalcontainer--3"" data-target=""email"">
    <div class=""fr-webreport-popup-content-export-parameters"">
        <div class=""fr-webreport-popup-content-title"">
            {localizationEmail.Title}
        </div>
    </div>        

<div class=""fr-email-export-form"">
    <div class=""fr-webreport-popup-content-export-parameters"">
        <label style=""margin-bottom: 15px;"">{localizationEmail.Email}</label>

        <div class=""fr-email-export-field"">
            <label class=""fr-email-export-label"">{localizationEmail.Address}<span class=""fr-required-star"">*</span></label>
            <input id=""Email"" class=""fr-email-export-input"" type=""text"" placeholder=""1 E-mail; 2 E-mail; 3 E-mail..."" required />
        </div>

        <div class=""fr-email-export-field"">
            <label class=""fr-email-export-label"">{localizationEmail.Subject}</label>
            <input id=""Subject"" class=""fr-email-export-input"" />
        </div>

        <div class=""fr-email-export-field"">
            <label class=""fr-email-export-label"">{localizationEmail.Message}</label>
            <textarea id=""Message"" class=""fr-email-export-textarea""></textarea>
        </div>

        <div class=""fr-email-export-field"">
            <label class=""fr-email-export-label"">{localizationEmail.Attachment}</label>
            <select id=""ExportFormat"" class=""fr-email-export-select"">
                {(exports.ShowPreparedReport ? @"<option value=""fpx"">FPX</option>" : "")}
                {(exports.ShowPdfExport ? @"<option value=""pdf"">PDF</option>" : "")}
                {(exports.ShowExcel2007Export ? @"<option value=""xlsx"">Excel 2007</option>" : "")}
                {(exports.ShowWord2007Export ? @"<option value=""docx"">Microsoft Word 2007</option>" : "")}
                {(exports.ShowPowerPoint2007Export ? @"<option value=""pptx"">PowerPoint 2007</option>" : "")}
                {(exports.ShowTextExport ? @"<option value=""txt"">Text/Matrix Printer</option>" : "")}
                {(exports.ShowRtfExport ? @"<option value=""rtf"">Rich Text</option>" : "")}
                {(exports.ShowXpsExport ? @"<option value=""xps"">XPS</option>" : "")}
                {(exports.ShowOdsExport ? @"<option value=""ods"">Open Office Calc</option>" : "")}
                {(exports.ShowOdtExport ? @"<option value=""odt"">Open Office Writer</option>" : "")}
                {(exports.ShowXmlExcelExport ? @"<option value=""xml"">XML</option>" : "")}
                {(exports.ShowDbfExport ? @"<option value=""dbf"">DBF</option>" : "")}
                {(exports.ShowCsvExport ? @"<option value=""csv"">CSV</option>" : "")}
                {(exports.ShowSvgExport ? @"<option value=""svg"">SVG</option>" : "")}
                {(exports.ShowMhtExport ? @"<option value=""mht"">MHT (web-archive)</option>" : "")}
                {(exports.ShowExcel97Export ? @"<option value=""xls"">Excel 97</option>" : "")}
                {(exports.ShowHpglExport ? @"<option value=""hpgl"">Hpgl (plt)</option>" : "")}
                {(exports.ShowHTMLExport ? @"<option value=""html"">HTML</option>" : "")}
                {(exports.ShowJsonExport ? @"<option value=""json"">Json</option>" : "")}
                {(exports.ShowDxfExport ? @"<option value=""dxf"">DXF</option>" : "")}
                {(exports.ShowLaTeXExport ? @"<option value=""latex"">LaTeX</option>" : "")}
                {(exports.ShowPpmlExport ? @"<option value=""ppml"">PPML</option>" : "")}
                {(exports.ShowPSExport ? @"<option value=""ps"">PostScript</option>" : "")}
                {(exports.ShowXamlExport ? @"<option value=""xaml"">XAML</option>" : "")}
                {(exports.ShowZplExport ? @"<option value=""zpl"">ZPL</option>" : "")}
            </select>
        </div>
        
        <div class=""fr-email-export-field"">
            <label class=""fr-email-export-label"">{localizationEmail.NameAttachmentFile}</label>
            <input id=""NameAttachmentFile"" class=""fr-email-export-input"" placeholder=""{Path.GetFileNameWithoutExtension(Report.FileName)}""/>
        </div>
    </div>
</div>

    <div class=""fr-webreport-popup-content-buttons"">
        <button class=""fr-webreport-popup-content-btn-submit fr-webreport-popup-content-btn-cancel"">{localizationPageSelector.LocalizedCancel}</button>
        <button class=""fr-webreport-popup-content-btn-submit"" onclick=""EMAILExport()"" id=""okButton"">OK</button>
    </div>
</div>

<script>
{template_modalcontainerscript}
//EMAILEXPORT//
var EmailExportInputs;
var RecieverAddress;
var Subject;
var Message;
var SenderAddress;
var Host;

function OnPageSelectorClickEmail() {{
    {template_pscustom}
}}

function EMAILExport() {{
    var url = `/_fr/preview.sendEmail?reportId={ID}`;

    var formData = new FormData();
    formData.append(""Address"", document.getElementById(""Email"").value);
    formData.append(""Subject"", document.getElementById(""Subject"").value);
    formData.append(""MessageBody"", document.getElementById(""Message"").value);
    formData.append(""ExportFormat"", document.getElementById(""ExportFormat"").value);
    formData.append(""NameAttachmentFile"", document.getElementById(""NameAttachmentFile"").value);

    fetch(url, {{
        method: 'POST',
        body: formData
    }})
    .then(response => {{
        if (response.ok) {{
            {template_FR}.showPopup(`{localizationEmail.SuccessMessage}`, true);
        }} else {{
            {template_FR}.showPopup(`{localizationEmail.FailureMessage}`, false);
        }}
    }})
    .catch(error => {{
        {template_FR}.showPopup(`{localizationEmail.FailureMessage}`, false);
    }});
}}

</script>";

        }

    }

}
#endif