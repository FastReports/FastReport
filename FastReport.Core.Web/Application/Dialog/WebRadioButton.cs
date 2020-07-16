using FastReport.Dialog;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {

        private void RadioButtonClick(RadioButtonControl rb, string data)
        {
            rb.Checked = data == "true";
            rb.FilterData();
            rb.OnClick(null);
        }

        private string GetRadioButtonHtml(RadioButtonControl control)
        {
            string id = GetControlID(control);
            return string.Format("<span style=\"{0}\"><input style=\"vertical-align:middle;width:{1}px;border:none;padding:0;margin:0 5px 0 0;\" type=\"radio\" name=\"{2}\" value=\"{3}\" onclick=\"{4}\" id=\"{5}\" {6}/><label style=\"{9}\" for=\"{7}\">{8}</label></span>",
                // style
                GetRadioButtonStyle(control),
                // width
                Zoom(10),
                // name
                control.Name,
                // value
                control.Text,
                // onclick
                GetEvent(ONCLICK, control, SILENT_RELOAD, $"document.getElementById('{id}').checked"),
                // title
                id,
                control.Checked ? "checked" : "",
                id,
                control.Text,
                GetControlFont(control.Font)
                );
        }

        private string GetRadioButtonStyle(RadioButtonControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)}";
        }

    }
}
