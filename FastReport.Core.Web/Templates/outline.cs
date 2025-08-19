﻿using FastReport.Utils;
using FastReport.Web.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace FastReport.Web
{
    partial class WebReport
    {
        string template_outline()
        {
            if (!Outline)
                return "";

            if ((Report.PreparedPages?.OutlineXml?.Count ?? 0) == 0)
                return "";

            var outline = new StringBuilder();
            BuildOutline(outline, Report.PreparedPages.OutlineXml, true);

            return $@"
<div class=""{template_FR}-outline"">
    <div class=""{template_FR}-outline-inner"">
        {outline}
    </div>
</div>

<script>
    (function(){{{ResourceLoader.GetContent("split.min.js")}}}).call({template_FR});
    {template_FR}.outline();
</script>
";
        }

        void BuildOutline(StringBuilder sb, XmlItem xml, bool top)
        {
            for (int i = 0; i < xml.Count; i++)
            {
                var opened = top && xml.Count == 1;

                var styleShow = opened ? @"style=""display:none""" : @"style=""display:block""";
                var styleHide = opened ? @"style=""display:block""" : @"style=""display:none""";

                var node = xml[i];
                var hasChildren = node.Count > 0;
                var (text, page, offset) = ReadOutlineNode(node);
                var nodeId = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{ID}{page}{offset}{sb.Length}{GetCurrentTabName()}"));

                sb.Append($@"<div class=""{template_FR}-outline-node"">");
                
                string caret;
                if (hasChildren)
                {
                    caret = $@"<img class=""{template_FR}-outline-caret {template_FR}-js-outline-open-node"" src=""data:image/svg+xml;base64,{GerResourceBase64("caret-right.svg")}"" data-fr-outline-node-id=""{nodeId}"" onclick=""{template_FR}.outlineOpenNode(this);"" {styleShow}>";
                    caret += $@"<img class=""{template_FR}-outline-caret {template_FR}-js-outline-close-node"" src=""data:image/svg+xml;base64,{GerResourceBase64("caret-down.svg")}"" data-fr-outline-node-id=""{nodeId}"" onclick=""{template_FR}.outlineCloseNode(this);"" {styleHide}>";
                }
                else
                {
                    caret = $@"<div class=""{template_FR}-outline-caret-blank""></div>";
                }

                sb.Append($@"<div class=""{template_FR}-outline-text"">");
                sb.Append(caret);
                sb.Append($@"<img class=""{template_FR}-outline-file"" src=""data:image/svg+xml;base64,{GerResourceBase64("file.svg")}"">");
                sb.Append($@"<a onclick=""{template_FR}.outlineGoto({page + 1}, {offset.ToString().Replace(',', '.')});"">{HttpUtility.HtmlEncode(text)}</a>");
                sb.Append($@"</div>");

                if (hasChildren)
                {
                    sb.Append($@"<div style=""width:100%""></div>"); // line break
                    sb.Append($@"<div class=""{template_FR}-outline-children"" {styleHide}>");
                    BuildOutline(sb, node, false);
                    sb.Append("</div>");
                }

                sb.Append("</div>");
            }
        }

        (string text, int page, float offset) ReadOutlineNode(XmlItem node)
        {
            string text = node.GetProp("Text");
            int page = 0;
            float offset = 0f;

            string s = node.GetProp("Page");
            if (!s.IsNullOrWhiteSpace())
            {
                page = int.Parse(s);
                s = node.GetProp("Offset");
                if (!s.IsNullOrWhiteSpace())
                    offset = (float)Converter.FromString(typeof(float), s) * Zoom /* * PDF_DIVIDER*/;
            }

            return (text, page, offset);
        }

    }
}
