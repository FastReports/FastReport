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
            StringBuilder sb = new StringBuilder();
            if (Tabs.Count > 1)
            {
                sb.Append($@"<div class=""{template_FR}-tabs"">");

                for (int i = 0; i < Tabs.Count; i++)
                {
                    //sb.Append("<div class=\"tabselector\">");
                    //sb.Append(string.Format("<input class=\"td tab {2}\" type=\"button\" name=\"tab1\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                    //    GetTabName(i), GetNavRequest("settab", i.ToString()), i == CurrentTabIndex ? "tabselected" : "", fTabs[i].Name));
                    //if (ReportProperties.ShowTabCloseButton)
                    //{
                    //    sb.Append(string.Format("<input class=\"td tabclose {2}\" type=\"button\" name=\"tab1c\" value=\"{0}\" title=\"{3}\" onclick=\"{1}\"/>",
                    //        "X", GetNavRequest("closetab", i.ToString()), i == CurrentTabIndex ? "tabselected" : "", "X"));
                    //}
                    //sb.Append("</div>");

                    var tab = Tabs[i];
                    var active = i == CurrentTabIndex ? "active" : "";
                    var settab = i == CurrentTabIndex ? "" : $@"onclick=""{template_FR}.settab('{i}');""";
                    var closetab = $@"onclick=""{template_FR}.closetab('{i}');""";

                    sb.Append($@"<div class=""{template_FR}-tab {active}"">");
                    sb.Append($@"<a {settab} class=""{template_FR}-tab-title"">{GetTabName(i)}</a>");

                    if (tab.Closeable)
                        sb.Append($@"<a {closetab} class=""{template_FR}-tab-close"" title=""Close"">
                                         <img src=""{template_resource_url("close.svg", "image/svg+xml")}"">
                                     </a>");

                    sb.Append($@"</div>");
                }

                sb.Append($@"</div>");
            }
            return sb.ToString();
        }

        internal string GetCurrentTabName()
        {
            return GetTabName(CurrentTabIndex);
        }

        internal string GetTabName(int i)
        {
            if (String.IsNullOrEmpty(Tabs[i].Name))
            {
                string s = Tabs[i].Report.ReportInfo.Name;
                if (String.IsNullOrEmpty(s))
                    s = Path.GetFileNameWithoutExtension(Tabs[i].Report.FileName);
                if (String.IsNullOrEmpty(s))
                    s = (i + 1).ToString();
                return s;
            }
            else
                return Tabs[i].Name;
        }
    }
}
