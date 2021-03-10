using FastReport.Dialog;
using System.Text;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private void ComboBoxChange(ComboBoxControl cb, int index)
        {
            cb.SelectedIndex = index;
            ControlFilterRefresh(cb);
            cb.OnSelectedIndexChanged(null);
        }

        private string GetComboBoxHtml(ComboBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            else
            {
                if (control.SelectedIndex == -1)
                    control.SelectedIndex = 0;
                control.SelectedItem = control.Items[control.SelectedIndex];
                control.Text = control.SelectedItem.ToString();
            }

            string id = GetControlID(control);
            string html = string.Format("<select style=\"{0}\" name=\"{1}\" onchange=\"{2}\" id=\"{3}\">{4}</select>",
                // style
                GetComboBoxStyle(control),
                // name
                control.Name,
                // onclick
                GetEvent(ONCHANGE, control, SILENT_RELOAD, $"document.getElementById('{id}').selectedIndex"),
                // title
                id,
                GetComboBoxItems(control)//control.Text
                );
            control.FilterData();
            return html;
        }

        private string GetComboBoxItems(ComboBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                sb.Append(string.Format("<option {0} value=\"{1}\">{2}</option>",
                    i == control.SelectedIndex ? "selected" : "",
                    control.Items[i],
                    control.Items[i]));
            }
            return sb.ToString();
        }

        private string GetComboBoxStyle(ComboBoxControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)}";
        }        

    }
}
