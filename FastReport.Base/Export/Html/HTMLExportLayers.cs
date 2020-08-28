using System;
using System.Drawing;
using System.IO;
using FastReport.Table;
using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Export.Html
{
    public partial class HTMLExport : ExportBase
    {
        private bool doPageBreak;

        private string GetStyle()
        {
            return "position:absolute;";
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

        private void ExportPageStylesLayers(FastString styles, int PageNumber)
        {
            if (prevStyleListIndex < cssStyles.Count)
            {
                styles.Append(HTMLGetStylesHeader());
                for (int i = prevStyleListIndex; i < cssStyles.Count; i++)
                    styles.Append(HTMLGetStyleHeader(i, PageNumber)).Append(cssStyles[i]).AppendLine("}");
                styles.AppendLine(HTMLGetStylesFooter());
            }
        }

        private string GetStyleTag(int index)
        {
            return String.Format("class=\"{0}s{1}\"",
                stylePrefix,
                index.ToString()
            );
        }

        private void Layer(FastString Page, ReportComponentBase obj,
            float Left, float Top, float Width, float Height, FastString Text, string style, FastString addstyletag)
        {
            if (Page != null && obj != null)
            {
                string onclick = null;
                
                if (!String.IsNullOrEmpty(ReportID))
                {
                    if (!String.IsNullOrEmpty(obj.ClickEvent) || obj.HasClickListeners())
                    {
                        onclick = "click";
                    }

                    CheckBoxObject checkBoxObject = obj as CheckBoxObject;
                    if (checkBoxObject != null && checkBoxObject.Editable)
                    {
                        onclick = "checkbox_click";
                    }

                    TextObject textObject = obj as TextObject;
                    if (textObject != null && textObject.Editable)
                    {
                        onclick = "text_edit";
                    }
                }

                // we need to adjust left, top, width and height values because borders take up space in html elements
                float borderLeft;
                float borderRight;
                float borderTop;
                float borderBottom;
                HTMLBorderWidthValues(obj, out borderLeft, out borderTop, out borderRight, out borderBottom);

                string href = GetHref(obj);

                if (!string.IsNullOrEmpty(href))
                {
                    Page.Append(href);
                }
                Page.Append("<div ").Append(style).Append(" style=\"").
                    Append(onclick != null || !string.IsNullOrEmpty(href) ? "cursor:pointer;" : "").
                    Append("left:").Append(Px((leftMargin + Left) * Zoom - borderLeft / 2f)).
                    Append("top:").Append(Px((topMargin + Top) * Zoom - borderTop / 2f)).
                    Append("width:").Append(Px(Width * Zoom - borderRight / 2f - borderLeft / 2f)).
                    Append("height:").Append(Px(Height * Zoom - borderBottom / 2f - borderTop / 2f));

                if (addstyletag != null)
                    Page.Append(addstyletag);

                Page.Append("\"");

                if (onclick != null)
                {
                    string eventParam = String.Format("{0},{1},{2},{3}",
                        obj.Name,
                        CurPage,
                        obj.AbsLeft.ToString("#0"),
                        obj.AbsTop.ToString("#0"));

                    Page.Append(" onclick=\"")
                        .AppendFormat(OnClickTemplate, ReportID, onclick, eventParam)
                        .Append("\"");
                }

                Page.Append(">");
                if (Text == null)
                    Page.Append(NBSP);
                else
                    Page.Append(Text);
                Page.AppendLine("</div>");
                if (!string.IsNullOrEmpty(href))
                {
                    Page.Append("</a>");
                }
            }
        }

        private string EncodeURL(string value)
        {
#if NETSTANDARD2_0 || NETSTANDARD2_1
            return System.Net.WebUtility.UrlEncode(value);
#else
            return ExportUtils.HtmlURL(value);
#endif
        }

        private string GetHref(ReportComponentBase obj)
        {
            string href = String.Empty;
            if (!String.IsNullOrEmpty(obj.Hyperlink.Value))
            {
                string hrefStyle = String.Empty;
                if (obj is TextObject)
                {
                    TextObject textObject = obj as TextObject;
                    hrefStyle = String.Format("style=\"color:{0}{1}\"",
                        ExportUtils.HTMLColor(textObject.TextColor),
                        !textObject.Font.Underline ? ";text-decoration:none" : String.Empty
                        );
                }
                string url = EncodeURL(obj.Hyperlink.Value);
                if (obj.Hyperlink.Kind == HyperlinkKind.URL)
                    href = String.Format("<a {0} href=\"{1}\"" + (obj.Hyperlink.OpenLinkInNewTab ? "target=\"_blank\"" : "") + ">", hrefStyle, obj.Hyperlink.Value);
                else if (obj.Hyperlink.Kind == HyperlinkKind.DetailReport)
                {
                    url = String.Format("{0},{1},{2}",
                        EncodeURL(obj.Name), // object name for security reasons
                        EncodeURL(obj.Hyperlink.ReportParameter),
                        EncodeURL(obj.Hyperlink.Value));
                    string onClick = String.Format(OnClickTemplate, ReportID, "detailed_report", url);
                    href = String.Format("<a {0} href=\"#\" onclick=\"{1}\">", hrefStyle, onClick);
                }
                else if (obj.Hyperlink.Kind == HyperlinkKind.DetailPage)
                {
                    url = String.Format("{0},{1},{2}",
                        EncodeURL(obj.Name),
                        EncodeURL(obj.Hyperlink.ReportParameter),
                        EncodeURL(obj.Hyperlink.Value));
                    string onClick = String.Format(OnClickTemplate, ReportID, "detailed_page", url);
                    href = String.Format("<a {0} href=\"#\" onclick=\"{1}\">", hrefStyle, onClick);
                }
                else if (SinglePage)
                {
                    if (obj.Hyperlink.Kind == HyperlinkKind.Bookmark)
                        href = String.Format("<a {0} href=\"#{1}\">", hrefStyle, url);
                    else if (obj.Hyperlink.Kind == HyperlinkKind.PageNumber)
                        href = String.Format("<a {0} href=\"#PageN{1}\">", hrefStyle, url);
                }
                else
                {
                    string onClick = String.Empty;
                    if (obj.Hyperlink.Kind == HyperlinkKind.Bookmark)
                        onClick = String.Format(OnClickTemplate, ReportID, "bookmark", url);
                    else if (obj.Hyperlink.Kind == HyperlinkKind.PageNumber)
                        onClick = String.Format(OnClickTemplate, ReportID, "goto", url);

                    if (onClick != String.Empty)
                        href = String.Format("<a {0} href=\"#\" onclick=\"{1}\">", hrefStyle, onClick);
                }
            }
            return href;
        }

        private FastString GetSpanText(TextObjectBase obj, FastString text,
            float top, float width,
            float ParagraphOffset)
        {
            FastString style = new FastString();
            style.Append("display:block;border:0;width:").Append(Px(width * Zoom));
            if (ParagraphOffset != 0)
                style.Append("text-indent:").Append(Px(ParagraphOffset * Zoom));
            if (obj.Padding.Left != 0)
                style.Append("padding-left:").Append(Px((obj.Padding.Left) * Zoom));
            if (obj.Padding.Right != 0)
                style.Append("padding-right:").Append(Px(obj.Padding.Right * Zoom));
            if (top != 0)
                style.Append("margin-top:").Append(Px(top * Zoom));

            // we need to apply border width in order to position our div perfectly
            float borderLeft;
            float borderRight;
            float borderTop;
            float borderBottom;
            if (HTMLBorderWidthValues(obj, out borderLeft, out borderTop, out borderRight, out borderBottom))
            {
                style.Append("position:absolute;")
                    .Append("left:").Append(Px(-1 * borderLeft / 2f))
                    .Append("top:").Append(Px(-1 * borderTop / 2f));
            }

            FastString result = new FastString(128);
            result.Append("<div ").
                Append(GetStyleTag(UpdateCSSTable(style.ToString()))).Append(">").
                Append(text).Append("</div>");

            return result;
        }

        private void LayerText(FastString Page, TextObject obj)
        {
            float top = 0;
            switch (obj.TextRenderType)
            {
                case TextRenderType.HtmlParagraph:

                    using (HtmlTextRenderer htmlTextRenderer = obj.GetHtmlTextRenderer(Zoom, Zoom))
                    {
                        if (obj.VertAlign == VertAlign.Center)
                        {
                            top = (obj.Height - htmlTextRenderer.CalcHeight()) / 2;
                        }
                        else if (obj.VertAlign == VertAlign.Bottom)
                        {
                            top = obj.Height - htmlTextRenderer.CalcHeight();
                        }
                        FastString sb = GetHtmlParagraph(htmlTextRenderer);

                        LayerBack(Page, obj,
                        GetSpanText(obj, sb,
                        top + obj.Padding.Top,
                        obj.Width - obj.Padding.Horizontal,
                        obj.ParagraphOffset));
                    }
                    break;
                default:
                    if (obj.VertAlign != VertAlign.Top)
                    {
                        Graphics g = htmlMeasureGraphics;
                        using (Font f = new Font(obj.Font.Name, obj.Font.Size, obj.Font.Style))
                        {
                            RectangleF textRect = new RectangleF(obj.AbsLeft, obj.AbsTop, obj.Width, obj.Height);
                            StringFormat format = obj.GetStringFormat(Report.GraphicCache, 0);
                            Brush textBrush = Report.GraphicCache.GetBrush(obj.TextColor);
                            AdvancedTextRenderer renderer = new AdvancedTextRenderer(obj.Text, g, f, textBrush, null,
                                textRect, format, obj.HorzAlign, obj.VertAlign, obj.LineHeight, obj.Angle, obj.FontWidthRatio,
                                obj.ForceJustify, obj.Wysiwyg, obj.HasHtmlTags, false, Zoom, Zoom, obj.InlineImageCache);
                            if (renderer.Paragraphs.Count > 0)
                                if (renderer.Paragraphs[0].Lines.Count > 0)
                                {
                                    float height = renderer.Paragraphs[0].Lines[0].CalcHeight();
                                    if (height > obj.Height)
                                        top = -(height - obj.Height) / 2;
                                    else
                                        top = renderer.Paragraphs[0].Lines[0].Top - obj.AbsTop;
                                }
                        }
                    }

                    LayerBack(Page, obj,
                        GetSpanText(obj, ExportUtils.HtmlString(obj.Text, obj.TextRenderType),
                        top + obj.Padding.Top,
                        obj.Width - obj.Padding.Horizontal,
                        obj.ParagraphOffset));
                    break;
            }

        }

        private FastString GetHtmlParagraph(HtmlTextRenderer renderer)
        {
            FastString sb = new FastString();
            foreach (HtmlTextRenderer.Paragraph paragraph in renderer.Paragraphs)
                foreach (HtmlTextRenderer.Line line in paragraph.Lines)
                {
                    if (sb == null) sb = new FastString();
                    sb.Append("<span style=\"");
                    sb.Append("display:block;");
                    if (line.Top + line.Height > renderer.DisplayRect.Bottom)
                        sb.Append("height:").Append(Math.Max(renderer.DisplayRect.Bottom - line.Top, 0).ToString(HtmlTextRenderer.CultureInfo)).Append("px;");
                    else
                    {
                        //sb.Append("height:").Append(line.Height.ToString(HtmlTextRenderer.CultureInfo)).Append("px;");
                        if (line.LineSpacing > 0)
                        {
                            sb.Append("margin-bottom:").Append(line.LineSpacing.ToString(HtmlTextRenderer.CultureInfo)).Append("px;");
                        }
                    }
                    sb.Append("overflow:hidden;");
                    sb.Append("line-height:").Append(line.Height.ToString(HtmlTextRenderer.CultureInfo)).Append("px;");
                    if (line.HorzAlign == HorzAlign.Justify)
                        sb.Append("text-align-last:justify;");
                    else sb.Append("white-space:pre;");
                    sb.Append("\">");
                    HtmlTextRenderer.StyleDescriptor styleDesc = null;
                    foreach (HtmlTextRenderer.Word word in line.Words)
                    {
                        foreach (HtmlTextRenderer.Run run in word.Runs)
                        {
                            if (!run.Style.FullEquals(styleDesc))
                            {
                                if (styleDesc != null)
                                    styleDesc.ToHtml(sb, true);
                                styleDesc = run.Style;
                                styleDesc.ToHtml(sb, false);
                            }

                            if (run is HtmlTextRenderer.RunText)
                            {
                                HtmlTextRenderer.RunText runText = run as HtmlTextRenderer.RunText;

                                foreach (char ch in runText.Text)
                                {
                                    switch (ch)
                                    {
                                        case '"':
                                            sb.Append("&quot;");
                                            break;
                                        case '&':
                                            sb.Append("&amp;");
                                            break;
                                        case '<':
                                            sb.Append("&lt;");
                                            break;
                                        case '>':
                                            sb.Append("&gt;");
                                            break;
                                        case '\t':
                                            sb.Append("&Tab;");
                                            break;
                                        default:
                                            sb.Append(ch);
                                            break;
                                    }
                                }

                            }
                            else if (run is HtmlTextRenderer.RunImage)
                            {
                                HtmlTextRenderer.RunImage runImage = run as HtmlTextRenderer.RunImage;

                                using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
                                {
                                    try
                                    {
                                        float w, h;
                                        using (Bitmap bmp = runImage.GetBitmap(out w, out h))
                                        {

                                            bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                                        }
                                        ms.Flush();
                                        sb.Append("<img src=\"data:image/png;base64,").Append(Convert.ToBase64String(ms.ToArray()))
                                            .Append("\" width=\"").Append(w.ToString(HtmlTextRenderer.CultureInfo)).Append("\" height=\"").Append(h.ToString(HtmlTextRenderer.CultureInfo)).Append("\"/>");
                                    }
                                    catch (Exception /*e*/)
                                    {

                                    }
                                }

                            }
                            //run.ToHtml(sb, true);
                        }
                    }
                    if (styleDesc != null)
                        styleDesc.ToHtml(sb, true);
                    else sb.Append("<br/>");
                    sb.Append("</span>");
                }
            return sb;
        }

        private void LayerHtml(FastString Page, HtmlObject obj)
        {
            LayerBack(Page, obj,
                GetSpanText(obj, new Utils.FastString(obj.Text),
                obj.Padding.Top,
                obj.Width - obj.Padding.Horizontal,
                0));
        }

        private string GetLayerPicture(ReportComponentBase obj, out float Width, out float Height)
        {
            string result = String.Empty;
            Width = 0;
            Height = 0;

            if (obj != null)
            {
                if (pictures)
                {
                    MemoryStream PictureStream = new MemoryStream();
                    System.Drawing.Imaging.ImageFormat FPictureFormat = System.Drawing.Imaging.ImageFormat.Bmp;
                    if (imageFormat == ImageFormat.Png)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Png;
                    else if (imageFormat == ImageFormat.Jpeg)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Jpeg;
                    else if (imageFormat == ImageFormat.Gif)
                        FPictureFormat = System.Drawing.Imaging.ImageFormat.Gif;

                    Width = obj.Width == 0 ? obj.Border.LeftLine.Width : obj.Width;
                    Height = obj.Height == 0 ? obj.Border.TopLine.Width : obj.Height;

                    if (Math.Abs(Width) * Zoom < 1 && Zoom > 0)
                        Width = 1 / Zoom;

                    if (Math.Abs(Height) * Zoom < 1 && Zoom > 0)
                        Height = 1 / Zoom;

                    using (System.Drawing.Image image =
                        new Bitmap(
                            (int)(Math.Abs(Math.Round(Width * Zoom))),
                            (int)(Math.Abs(Math.Round(Height * Zoom)))
                            )
                          )
                    {
                        using (Graphics g = Graphics.FromImage(image))
                        {
                            if (obj is TextObjectBase)
                                g.Clear(Color.White);

                            float Left = Width > 0 ? obj.AbsLeft : obj.AbsLeft + Width;
                            float Top = Height > 0 ? obj.AbsTop : obj.AbsTop + Height;

                            float dx = 0;
                            float dy = 0;
                            g.TranslateTransform((-Left - dx) * Zoom, (-Top - dy) * Zoom);

                            BorderLines oldLines = obj.Border.Lines;
                            obj.Border.Lines = BorderLines.None;
                            obj.Draw(new FRPaintEventArgs(g, Zoom, Zoom, Report.GraphicCache));
                            obj.Border.Lines = oldLines;
                     
                        }

                        if (FPictureFormat == System.Drawing.Imaging.ImageFormat.Jpeg)
                            ExportUtils.SaveJpeg(image, PictureStream, 95);
                        else
                            image.Save(PictureStream, FPictureFormat);
                    }
                    PictureStream.Position = 0;

                    string hash = String.Empty;
                    if (obj is PictureObject)
                    {
                        PictureObject pic = (obj as PictureObject);
                        if (pic.Image != null)
                        {
#if MONO
                            using (MemoryStream picStr = new MemoryStream())
                            {
                                
                                ImageHelper.Save(pic.Image, picStr);
                                using(StreamWriter picWriter = new StreamWriter(picStr))
                                {
                                    picWriter.Write(pic.Width);
                                    picWriter.Write(pic.Height);
                                    picWriter.Write(pic.Angle);
                                    picWriter.Write(pic.Transparency);
                                    picWriter.Write(pic.TransparentColor.ToArgb());
                                    picWriter.Write(pic.CanShrink);
                                    picWriter.Write(pic.CanGrow);
                                    hash = Crypter.ComputeHash(picStr);
                                }        
                            }   
#else
                            hash = Crypter.ComputeHash(PictureStream);
                            PictureStream.Position = 0;
#endif
                        }
                    }
                    else
                        hash = Crypter.ComputeHash(PictureStream);
                    result = HTMLGetImage(0, 0, 0, hash, true, null, PictureStream, false);
                }
            }
            return result;
        }

        private void LayerPicture(FastString Page, ReportComponentBase obj, FastString text)
        {
            if (pictures)
            {
                int styleindex = UpdateCSSTable(obj);
                string old_text = String.Empty;

                if (IsMemo(obj))
                {
                    old_text = (obj as TextObject).Text;
                    (obj as TextObject).Text = String.Empty;
                }

                float Width, Height;
                string pic = GetLayerPicture(obj, out Width, out Height);

                if (IsMemo(obj))
                    (obj as TextObject).Text = old_text;

                FastString picStyleBuilder = new FastString("background: url('")
                    .Append(pic).Append("') no-repeat !important;-webkit-print-color-adjust:exact;");

                int picStyleIndex = UpdateCSSTable(picStyleBuilder.ToString());


                string style = String.Format("class=\"{0}s{1} {0}s{2}\"",
                stylePrefix,
                styleindex.ToString(), picStyleIndex.ToString());

                //FastString addstyle = new FastString(128);
                //addstyle.Append(" background: url('").Append(pic).Append("') no-repeat !important;-webkit-print-color-adjust:exact;");

                //if (String.IsNullOrEmpty(text))
                //    text = NBSP;

                float x = Width > 0 ? obj.AbsLeft : (obj.AbsLeft + Width);
                float y = Height > 0 ? hPos + obj.AbsTop : (hPos + obj.AbsTop + Height);

                Layer(Page, obj, x, y, Width, Height, text, style, null);
            }
        }

        private void LayerShape(FastString Page, ShapeObject obj, FastString text)
        {
            float Width, Height;
            FastString addstyle = new FastString(64);

            addstyle.Append(GetStyle());

            addstyle.Append("background: url('" + GetLayerPicture(obj, out Width, out Height) + "');no-repeat !important;-webkit-print-color-adjust:exact;");

            float x = obj.Width > 0 ? obj.AbsLeft : (obj.AbsLeft + obj.Width);
            float y = obj.Height > 0 ? hPos + obj.AbsTop : (hPos + obj.AbsTop + obj.Height);
            Layer(Page, obj, x, y, obj.Width, obj.Height, text, null, addstyle);
        }

        private void LayerBack(FastString Page, ReportComponentBase obj, FastString text)
        {
            if (obj.Border.Shadow)
            {
                using (TextObject shadow = new TextObject())
                {
                    shadow.Left = obj.AbsLeft + obj.Border.ShadowWidth + obj.Border.LeftLine.Width;
                    shadow.Top = obj.AbsTop + obj.Height + obj.Border.BottomLine.Width;
                    shadow.Width = obj.Width + obj.Border.RightLine.Width;
                    shadow.Height = obj.Border.ShadowWidth + obj.Border.BottomLine.Width;
                    shadow.FillColor = obj.Border.ShadowColor;
                    shadow.Border.Lines = BorderLines.None;
                    LayerBack(Page, shadow, null);

                    shadow.Left = obj.AbsLeft + obj.Width + obj.Border.RightLine.Width;
                    shadow.Top = obj.AbsTop + obj.Border.ShadowWidth + obj.Border.TopLine.Width;
                    shadow.Width = obj.Border.ShadowWidth + obj.Border.RightLine.Width;
                    shadow.Height = obj.Height;
                    LayerBack(Page, shadow, null);
                }
            }

            if (!(obj is PolyLineObject))
            {
                if (obj.Fill is SolidFill)
                    Layer(Page, obj, obj.AbsLeft, hPos + obj.AbsTop, obj.Width, obj.Height, text, GetStyleTag(UpdateCSSTable(obj)), null);
                else
                    LayerPicture(Page, obj, text);
            }
        }

        private void LayerTable(FastString Page, FastString CSS, TableBase table)
        {
            float y = 0;
            for (int i = 0; i < table.RowCount; i++)
            {
                float x = 0;
                for (int j = 0; j < table.ColumnCount; j++)
                {
                    if (!table.IsInsideSpan(table[j, i]))
                    {
                        TableCell textcell = table[j, i];

                        textcell.Left = x;
                        textcell.Top = y;

                        // custom draw
                        CustomDrawEventArgs e = new CustomDrawEventArgs();
                        e.report = Report;
                        e.reportObject = textcell;
                        e.layers = Layers;
                        e.zoom = Zoom;
                        e.left = textcell.AbsLeft;
                        e.top = hPos + textcell.AbsTop;
                        e.width = textcell.Width;
                        e.height = textcell.Height;
                        OnCustomDraw(e);
                        if (e.done)
                        {
                            CSS.Append(e.css);
                            Page.Append(e.html);
                        }
                        else
                        {
                            if (textcell is TextObject && !(textcell as TextObject).TextOutline.Enabled && IsMemo(textcell))
                                LayerText(Page, textcell as TextObject);
                            else
                            {
                                LayerBack(Page, textcell as ReportComponentBase, null);
                                LayerPicture(Page, textcell as ReportComponentBase, null);
                            }
                        }
                    }
                    x += (table.Columns[j]).Width;
                }
                y += (table.Rows[i]).Height;
            }
        }

        private bool IsMemo(ReportComponentBase Obj)
        {
            if(Obj is TextObject)
            {
                TextObject aObj = Obj as TextObject;
                return (aObj.Angle == 0) && (aObj.FontWidthRatio == 1) && (!aObj.TextOutline.Enabled) && (!aObj.Underlines);
            }
            return false;
            
        }

        private void Watermark(FastString Page, ReportPage page, bool drawText)
        {
            using (PictureObject pictureWatermark = new PictureObject())
            {
                pictureWatermark.Left = 0;
                pictureWatermark.Top = 0;

                pictureWatermark.Width = (ExportUtils.GetPageWidth(page) - page.LeftMargin - page.RightMargin) * Units.Millimeters;
                pictureWatermark.Height = (ExportUtils.GetPageHeight(page) - page.TopMargin - page.BottomMargin) * Units.Millimeters;

                pictureWatermark.SizeMode = PictureBoxSizeMode.Normal;
                pictureWatermark.Image = new Bitmap((int)pictureWatermark.Width, (int)pictureWatermark.Height);

                using (Graphics g = Graphics.FromImage(pictureWatermark.Image))
                {
                    g.Clear(Color.Transparent);
                    if (drawText)
                        page.Watermark.DrawText(new FRPaintEventArgs(g, 1f, 1f, Report.GraphicCache),
                            new RectangleF(0, 0, pictureWatermark.Width, pictureWatermark.Height), Report, true);
                    else
                        page.Watermark.DrawImage(new FRPaintEventArgs(g, 1f, 1f, Report.GraphicCache),
                            new RectangleF(0, 0, pictureWatermark.Width, pictureWatermark.Height), Report, true);
                    pictureWatermark.Transparency = page.Watermark.ImageTransparency;
                    LayerBack(Page, pictureWatermark, null);
                    LayerPicture(Page, pictureWatermark, null);
                }
            }
        }

        private void ExportHTMLPageLayeredBegin(HTMLData d)
        {
            if (!singlePage && !WebMode)
                cssStyles.Clear();

            css = new FastString();
            htmlPage = new FastString();

            ReportPage reportPage = d.page;

            if (reportPage != null)
            {
                maxWidth = ExportUtils.GetPageWidth(reportPage) * Units.Millimeters;
                maxHeight = ExportUtils.GetPageHeight(reportPage) * Units.Millimeters;


                if (enableMargins)
                {
                    leftMargin = reportPage.LeftMargin * Units.Millimeters;
                    topMargin = reportPage.TopMargin * Units.Millimeters;
                }
                else
                {
                    maxWidth -= (reportPage.LeftMargin + reportPage.RightMargin) * Units.Millimeters;
                    maxHeight -= (reportPage.TopMargin + reportPage.BottomMargin) * Units.Millimeters;
                    leftMargin = 0;
                    topMargin = 0;
                }

                currentPage = d.PageNumber - 1;

                ExportHTMLPageStart(htmlPage, d.PageNumber, d.CurrentPage);

                doPageBreak = (singlePage && pageBreaks);

                htmlPage.Append(HTMLGetAncor((d.PageNumber).ToString()));

                pageStyleName = "frpage" + currentPage;
                htmlPage.Append("<div ").Append(doPageBreak ? "class=\"" + pageStyleName + "\"" : String.Empty)
                    .Append(" style=\"position:relative;")
                    .Append(" width:").Append(Px(maxWidth * Zoom + 3))
                    .Append(" height:").Append(Px(maxHeight * Zoom));

                if (reportPage.Fill is SolidFill)
                {
                    SolidFill fill = reportPage.Fill as SolidFill;
                    htmlPage.Append(" background-color:").
                        Append(fill.IsTransparent ? "transparent" : ExportUtils.HTMLColor(fill.Color));
                }
                else
                {
                    // to-do for picture background
                }
                htmlPage.Append("\">");

                if (reportPage.Watermark.Enabled && !reportPage.Watermark.ShowImageOnTop)
                    Watermark(htmlPage, reportPage, false);

                if (reportPage.Watermark.Enabled && !reportPage.Watermark.ShowTextOnTop)
                    Watermark(htmlPage, reportPage, true);
            }
        }

        private void ExportHTMLPageLayeredEnd(HTMLData d)
        {
            // to do
            if (d.page.Watermark.Enabled && d.page.Watermark.ShowImageOnTop)
                Watermark(htmlPage, d.page, false);

            if (d.page.Watermark.Enabled && d.page.Watermark.ShowTextOnTop)
                Watermark(htmlPage, d.page, true);

            ExportPageStylesLayers(css, d.PageNumber);

            if (singlePage)
            {
                hPos = 0;
                prevStyleListIndex = cssStyles.Count;
            }
            htmlPage.Append("</div>");

            ExportHTMLPageFinal(css, htmlPage, d, maxWidth, maxHeight);
        }

        private void ExportBandLayers(Base band)
        {
            LayerBack(htmlPage, band as ReportComponentBase, null);
            foreach (Base c in band.ForEachAllConvectedObjects(this))
            {
                if (c is ReportComponentBase && (c as ReportComponentBase).Exportable)
                {
                    ReportComponentBase obj = c as ReportComponentBase;

                    // custom draw
                    CustomDrawEventArgs e = new CustomDrawEventArgs();
                    e.report = Report;
                    e.reportObject = obj;
                    e.layers = Layers;
                    e.zoom = Zoom;
                    e.left = obj.AbsLeft;
                    e.top = hPos + obj.AbsTop;
                    e.width = obj.Width;
                    e.height = obj.Height;

                    OnCustomDraw(e);
                    if (e.done)
                    {
                        css.Append(e.css);
                        htmlPage.Append(e.html);
                    }
                    else
                    {
                        if (!String.IsNullOrEmpty(obj.Bookmark))
                            htmlPage.Append("<a name=\"").Append(obj.Bookmark).Append("\"></a>");

                        if (obj is CellularTextObject)
                            obj = (obj as CellularTextObject).GetTable();
                        if (obj is TableCell)
                            continue;
                        else if (obj is TableBase)
                        {
                            TableBase table = obj as TableBase;
                            if (table.ColumnCount > 0 && table.RowCount > 0)
                            {
                                using (TextObject tableback = new TextObject())
                                {
                                    tableback.Border = table.Border;
                                    tableback.Fill = table.Fill;
                                    tableback.FillColor = table.FillColor;
                                    tableback.Left = table.AbsLeft;
                                    tableback.Top = table.AbsTop;
                                    float tableWidth = 0;
                                    float tableHeight = 0;
                                    for (int i = 0; i < table.ColumnCount; i++)
                                        tableWidth += table[i, 0].Width;
                                    for (int i = 0; i < table.RowCount; i++)
                                        tableHeight += table.Rows[i].Height;
                                    tableback.Width = (tableWidth < table.Width) ? tableWidth : table.Width;
                                    tableback.Height = tableHeight;
                                    LayerText(htmlPage, tableback);
                                }
                                LayerTable(htmlPage, css, table);
                            }
                        }
                        else if (IsMemo(obj))
                        {
                            LayerText(htmlPage, obj as TextObject);
                        }
                        else if (obj is HtmlObject)
                        {
                            LayerHtml(htmlPage, obj as HtmlObject);
                        }
                        else if (obj is BandBase)
                        {
                            LayerBack(htmlPage, obj, null);
                        }
                        else if (obj is LineObject)
                        {
                            LayerPicture(htmlPage, obj, null);
                        }
                        else if (obj is ShapeObject)
                        {
                            LayerShape(htmlPage, obj as ShapeObject, null);
                        }
                        else if (HasExtendedExport(obj))
                        {
                            ExtendExport(htmlPage, obj, null);
                        }
                        else
                        {
                            LayerBack(htmlPage, obj, null);
                            LayerPicture(htmlPage, obj, null);
                        }
                    }
                }
            }
        }
    }
}
