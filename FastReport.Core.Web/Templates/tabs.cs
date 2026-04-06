using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_tabs()
        {
            if (Tabs.Count > 1)
            {
                StringBuilder sb = new StringBuilder(64);

                sb.Append($@"<div class=""fr-tabs"">");

                for (int i = 0; i < Tabs.Count; i++)
                {

                    //sb.Append("<div class=\"tabselector\">");
                    //sb.AppendFormat("<input class=\"td tab {2}\" type=\"button\" name=\"tab1\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                    //    GetTabName(i), GetNavRequest("settab", i.ToString()), i == CurrentTabIndex ? "tabselected" : "", fTabs[i].Name);
                    //if (ReportProperties.ShowTabCloseButton)
                    //{
                    //    sb.AppendFormat("<input class=\"td tabclose {2}\" type=\"button\" name=\"tab1c\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                    //        "X", GetNavRequest("closetab", i.ToString()), i == CurrentTabIndex ? "tabselected" : "", "X");
                    //}
                    //sb.Append("</div>");

                    var tab = Tabs[i];
                    var active = i == CurrentTabIndex ? "active" : "";
                    var settab = i == CurrentTabIndex ? "" : CreateOnClickEvent(ScriptName, "settab", i.ToString());
                    var closetab = CreateOnClickEvent(ScriptName, "closetab", i.ToString());

                    sb.Append($@"<div class=""fr-tab {active}"">");
                    sb.Append($@"<a {settab} class=""fr-tab-title"">{GetTabName(i)}</a>");

                    if (tab.Closeable)
                        sb.Append($@"<a {closetab} class=""fr-tab-close"" title=""Close"">
                                        {GetResource("close.svg")}
                                     </a>");

                    sb.Append("</div>");
                }
                sb.Append("</div>");
                return sb.ToString();
            }
            return string.Empty;
        }
    }
}
