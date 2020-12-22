using FastReport.Dialog;
using System.Text;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private void ListBoxChange(ListBoxControl cb, int index)
        {
            cb.SelectedIndex = index;
            ControlFilterRefresh(cb);
            cb.OnSelectedIndexChanged(null);
        }

        private string GetListBoxHtml(ListBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            string id = GetControlID(control);
            string html = string.Format("<select style=\"{0}\" name=\"{1}\" size=\"{2}\" onchange=\"{3}\" id=\"{4}\">{5}</select>",
                // style
                GetListBoxStyle(control),
                // name
                control.Name,
                // size
                control.Items.Count.ToString(),
                // onclick
                GetEvent(ONCHANGE, control, DIALOG, $"document.getElementById('{id}').selectedIndex"),
                // title
                id,
                GetListBoxItems(control)//control.Text
                );
            control.FilterData();
            return html;
        }

        private string GetListBoxItems(ListBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                sb.Append(string.Format("<option {0}>{1}</option>",
                    i == control.SelectedIndex ? "selected" : "",
                    control.Items[i]));
            }
            return sb.ToString();
        }

        private string GetListBoxStyle(ListBoxControl control)
        {
            return $"{GetControlPosition(control)} {GetControlFont(control.Font)}";
        }        

    }
}
