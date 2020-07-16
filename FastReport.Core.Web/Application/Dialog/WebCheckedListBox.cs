using FastReport.Dialog;
using System;
using System.Text;
using static FastReport.Web.Constants;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private void CheckedListBoxChange(CheckedListBoxControl cb, string index)
        {
            int i = index.IndexOf("_");
            if (i != -1)
            {
                string item = index.Substring(0, i);
                string state = index.Substring(i + 1);
                int checkedIndex;
                if (Int32.TryParse(item, out checkedIndex))
                {
                    cb.CheckedListBox.SetItemChecked(checkedIndex, state == "true");
                    ControlFilterRefresh(cb);
                    cb.OnSelectedIndexChanged(null);                    
                }
            }
        }

        private string GetCheckedListBoxHtml(CheckedListBoxControl control)
        {
            if (control.Items.Count == 0)
            {
                control.FillData();
                ControlFilterRefresh(control);
            }
            string id = GetControlID(control);
            string html = string.Format("<span style=\"{0}\" name=\"{1}\" size=\"{2}\" id=\"{3}\">{4}</span>",
                // style
                GetCheckedListBoxStyle(control),
                // name
                control.Name,
                // size
                control.Items.Count.ToString(),
                // title
                id,
                GetCheckedListBoxItems(control)
                );
            control.FilterData();
            return html;
        }

        private string GetCheckedListBoxItems(CheckedListBoxControl control)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < control.Items.Count; i++)
            {
                string id = GetControlID(control) + i.ToString();
                sb.Append(string.Format("<input {0} type=\"checkbox\" onchange=\"{1}\" id=\"{2}\" /> {3}<br />",
                    control.CheckedIndices.Contains(i) ? "checked" : "",
                    // onchange
                    GetEvent(ONCHANGE, control, DIALOG, $"{i} + '_' + document.getElementById('{id}').checked"),
                    id,
                    control.Items[i]
                    ));
            }
            return sb.ToString();
        }

        private string GetCheckedListBoxStyle(CheckedListBoxControl control)
        {
            return $"overflow-y:scroll;{GetControlPosition(control)}{GetControlFont(control.Font)}";
        }        

    }
}
