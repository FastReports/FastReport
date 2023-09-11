using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace FastReport.Web.Services
{
    internal sealed class TextEditService : ITextEditService
    {

        public string GetTemplateTextEditForm(string click, WebReport webReport)
        {
            if (!click.IsNullOrWhiteSpace())
            {
                var @params = click.Split(',');
                if (@params.Length == 4)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        string result = null;

                        webReport.Report.FindClickedObject<TextObject>(@params[0], pageN, left, top,
                            (textObject, reportPage, _pageN) =>
                            {
                                webReport.Res.Root("Buttons");
                                string okText = webReport.Res.Get("Ok");
                                string cancelText = webReport.Res.Get("Cancel");
                                result = Template_textedit_form(textObject.Text, okText, cancelText);
                            });

                        return result;
                    }
                }
            }

            return null;
        }

        private static string Template_textedit_form(string text, string okText, string cancelText) => $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Text edit</title>
    <style>
body {{
    margin: 8px;
    height: calc(100vh - 16px);
}}
textarea {{
    width: 100%;
    height: calc(100% - 42px);
    box-sizing : border-box;
}}
button {{
    float: right;
    margin-left: 8px;
    margin-top: 8px;
    height: 30px;
}}
    </style>
</head>
<body>
    <textarea autofocus>{HttpUtility.HtmlEncode(text)}</textarea>
    <br>
    <button onclick=""window.close();"">{HttpUtility.HtmlEncode(cancelText)}</button>
    <button onclick=""window.postMessage('submit', '*');"">{HttpUtility.HtmlEncode(okText)}</button>
</body>
</html>
";
    }
}
