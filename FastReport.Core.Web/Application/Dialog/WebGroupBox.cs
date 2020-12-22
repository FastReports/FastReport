using FastReport.Dialog;
using System.Text;

namespace FastReport.Web
{
    public partial class Dialog
    {
        private string GetGroupBoxHtml(GroupBoxControl groupBox)
        {
            StringBuilder sb = new StringBuilder();

            string s = $"<div style=\"{GetControlPosition(groupBox)}\">";
            sb.Append(s);

            sb.Append($"<div id=\"{groupBox.Name}\" style=\"position:relative;\">");

            GetComponentHtml(sb, groupBox.Controls);
            sb.Append("</div></div>");
            return sb.ToString();
        }

    }
}
