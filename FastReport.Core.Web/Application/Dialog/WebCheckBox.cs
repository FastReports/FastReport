using FastReport.Dialog;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private void CheckBoxClick(CheckBoxControl cb, string data)
        {
            cb.Checked = data == "true";
            cb.FilterData();
            cb.OnClick(null);
        }

        private string GetCheckBoxHtml(CheckBoxControl control)
        {
            string id = GetControlID(control);
            return string.Format("<span style=\"{0}\"><input style=\"vertical-align:middle;padding:0;margin:0 5px 0 0;\" type=\"checkbox\" name=\"{1}\" value=\"{2}\" onclick=\"{3}\" id=\"{4}\" {5}/><label style=\"{8}\" for=\"{6}\">{7}</label></span>",
                // style
                GetCheckBoxStyle(control),
                // name
                control.Name,
                // value
                control.Text,
                // onclick
                GetEvent(ONCLICK, control, DIALOG, $"document.getElementById('{id}').checked"),
                // title
                id,
                control.Checked ? "checked" : "",
                id,
                control.Text,
                GetControlFont(control.Font)
                );
        }

        private string GetCheckBoxStyle(CheckBoxControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)}";
        }
    }
}
