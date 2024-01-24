using System;
using System.Drawing;
using FastReport.Table;
using FastReport.Utils;

namespace FastReport.Export.Html
{
    public partial class HTMLExport : ExportBase
    {

        // TODO:
        private bool InlineStyles { get; set; }


        private string GetStyleFromObject(ReportComponentBase obj)
        {
            string style;
            if (obj is TextObject)
            {
                TextObject textObj = obj as TextObject;
                style = GetStyle(textObj.Font, textObj.TextColor, textObj.FillColor,
                    textObj.RightToLeft, textObj.HorzAlign, textObj.Border, textObj.WordWrap, textObj.LineHeight,
                    textObj.Width, textObj.Height, textObj.Clip);
            }
            else if (obj is HtmlObject)
            {
                HtmlObject htmlObj = obj as HtmlObject;
                style = GetStyle(DrawUtils.DefaultTextObjectFont, Color.Black, htmlObj.FillColor,
                    false, HorzAlign.Left, htmlObj.Border, true, 0, htmlObj.Width, htmlObj.Height, false);
            }
            else
                style = GetStyle(null, Color.White, obj.FillColor, false, HorzAlign.Center, obj.Border, false, 0, obj.Width, obj.Height, false);
            return style;
        }

        private string GetStyle(Font Font, Color TextColor, Color FillColor,
            bool RTL, HorzAlign HAlign, Border Border, bool WordWrap, float LineHeight, float Width, float Height, bool Clip)
        {
            FastString style = new FastString(256);

            if (Font != null)
            {
                if (Zoom != 1)
                {
                    using (Font newFont = new Font(Font.FontFamily, Font.Size * Zoom, Font.Style, Font.Unit, Font.GdiCharSet, Font.GdiVerticalFont))
                        HTMLFontStyle(style, newFont, LineHeight);
                }
                else
                    HTMLFontStyle(style, Font, LineHeight);
            }
            style.Append("text-align:");
            if (HAlign == HorzAlign.Left)
                style.Append(RTL ? "right" : "left");
            else if (HAlign == HorzAlign.Right)
                style.Append(RTL ? "left" : "right");
            else if (HAlign == HorzAlign.Center)
                style.Append("center");
            else
                style.Append("justify");
            style.Append(";");

            if (WordWrap)
                style.Append("word-wrap:break-word;");

            if (Clip)
                style.Append("overflow:hidden;");

            style.Append("position:absolute;color:").
                Append(ExportUtils.HTMLColor(TextColor)).
                Append(";background-color:").
                Append(FillColor.A == 0 ? "transparent" : ExportUtils.HTMLColor(FillColor)).
                Append(";").Append(RTL ? "direction:rtl;" : String.Empty);

            Border newBorder = Border;
            HTMLBorder(style, newBorder);
            style.Append("width:").Append(Px(Math.Abs(Width) * Zoom)).Append("height:").Append(Px(Math.Abs(Height) * Zoom));
            return style.ToString();
        }

        private int UpdateCSSTable(ReportComponentBase obj)
        {
            var style = GetStyleFromObject(obj);
            return UpdateCSSTable(style);
        }

        private int UpdateCSSTable(string style)
        {
            int i = cssStyles.IndexOf(style);
            if (i == -1)
            {
                i = cssStyles.Count;
                cssStyles.Add(style);
            }
            return i;
        }


        private string GetStyle(string style)
        {
            if (InlineStyles)
            {
                return InlineStyle(style);
            }
            else
            {
                int index = UpdateCSSTable(style);
                return GetStyleTag(index);
            }
        }

        private string GetStyle(ReportComponentBase obj)
        {
            if (InlineStyles)
            {
                var style = GetStyleFromObject(obj);
                return InlineStyle(style);
            }
            else
            {
                int index = UpdateCSSTable(obj);
                return GetStyleTag(index);
            }
        }

        private string GetStyle(ReportComponentBase obj, string additionalStyle)
        {
            if (InlineStyles)
            {
                var style = GetStyleFromObject(obj);
                var resultStyle = string.Concat(style, " ", additionalStyle);
                return InlineStyle(resultStyle);
            }
            else
            {
                int index1 = UpdateCSSTable(obj);
                int index2 = UpdateCSSTable(additionalStyle);
                return GetStyleTag(index1, index2);
            }
        }

        private static string InlineStyle(string style)
        {
            return $"style=\"{style}\"";
        }

        private string GetStyleTag(int index)
        {
            return String.Format("class=\"{0}s{1}\"",
                stylePrefix,
                index.ToString()
            );
        }

        private string GetStyleTag(int index1, int index2)
        {
            return String.Format("class=\"{0}s{1} {0}s{2}\"",
                stylePrefix,
                index1.ToString(),
                index2.ToString()
            );
        }

    }
}
