using FastReport.Utils;
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
<div class=""fr-outline"">
    <div class=""fr-outline-inner"">
        {outline}
    </div>
</div>

<script src=""/_content/FastReport.Web/js/split.min.js""></script>
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

                sb.Append($@"<div class=""fr-outline-node"">");

                string caret;
                if (hasChildren)
                {
                    caret = $@"<img class=""fr-outline-caret fr-js-outline-open-node"" src=""data:image/svg+xml;base64,{GerResourceBase64("caret-right.svg")}"" data-fr-outline-node-id=""{nodeId}"" {CreateOnClickEvent(ScriptName, "outlineOpenNode", "this")} {styleShow}>";
                    caret += $@"<img class=""fr-outline-caret fr-js-outline-close-node"" src=""data:image/svg+xml;base64,{GerResourceBase64("caret-down.svg")}"" data-fr-outline-node-id=""{nodeId}"" {CreateOnClickEvent(ScriptName, "outlineCloseNode", "this")} {styleHide}>";
                }
                else
                {
                    caret = $@"<div class=""fr-outline-caret-blank""></div>";
                }

                sb.Append($@"<div class=""fr-outline-text"">");
                sb.Append(caret);
                sb.Append($@"<img class=""fr-outline-file"" src=""data:image/svg+xml;base64,{GerResourceBase64("file.svg")}"">");
                sb.Append($@"<a  {CreateOnClickEvent(ScriptName, "outlineGoto", (page + 1).ToString(), offset.ToString().Replace(',', '.'), SinglePage.ToString().ToLower())}>{HttpUtility.HtmlEncode(text)}</a>");
                sb.Append($@"</div>");

                if (hasChildren)
                {
                    sb.Append($@"<div style=""width:100%""></div>"); // line break
                    sb.Append($@"<div class=""fr-outline-children"" {styleHide}>");
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
