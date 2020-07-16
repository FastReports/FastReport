using FastReport.Dialog;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private void TextBoxChange(TextBoxControl tb, string data)
        {
            tb.Text = data;
            tb.FilterData();
            tb.OnTextChanged(null);
        }

        private string GetValHook()
        {
            return
                "<script>$.valHooks.textarea = {" +
                "get: function(elem) {" +
                "return elem.value.replace(/\\r?\\n/g, \'\\r\\n\');" +
                "}};</script>";
        }

        private string GetTextBoxHtml(TextBoxControl control)
        {
            string id = GetControlID(control);
            if (control.Multiline)
            {
                return
                    //GetValHook() +
                    string.Format("<textarea style=\"{0}\" type=\"text\" name=\"{1}\" onchange=\"{3}\" id=\"{4}\">{2}</textarea>",
                    // style
                    GetTextBoxStyle(control), //0
                    // name
                    control.Name, //1
                    // value
                    control.Text, //2
                    // onclick
                    GetEvent(ONCHANGE, control, DIALOG, $"document.getElementById('{id}').value.replace(/\\r?\\n/g, \'\\r\\n\')"), //3
                    // title
                    id //4
                    );
            }
            else
            {
                return string.Format("<input style=\"{0}\" type=\"text\" name=\"{1}\" value=\"{2}\" onchange=\"{3}\" id=\"{4}\"/>",
                    // style
                    GetTextBoxStyle(control),
                    // name
                    control.Name,
                    // value
                    control.Text,
                    // onclick
                    GetEvent(ONCHANGE, control, DIALOG, $"document.getElementById('{id}').value"),
                    // title
                    id
                    );
            }
        }

        private string GetTextBoxStyle(TextBoxControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)} {GetControlAlign(control)}";
        }
    }
}
