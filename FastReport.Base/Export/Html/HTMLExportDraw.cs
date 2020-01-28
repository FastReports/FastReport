using System;
using System.Drawing;
using System.IO;
using FastReport.Utils;
using System.Windows.Forms;

namespace FastReport.Export.Html
{
    public partial class HTMLExport : ExportBase
    {
        private void HTMLFontStyle(FastString FFontDesc, Font font, float LineHeight)
        {
            FFontDesc.Append((((font.Style & FontStyle.Bold) > 0) ? "font-weight:bold;" : String.Empty) +
                (((font.Style & FontStyle.Italic) > 0) ? "font-style:italic;" : "font-style:normal;"));
            if ((font.Style & FontStyle.Underline) > 0 && (font.Style & FontStyle.Strikeout) > 0)
                FFontDesc.Append("text-decoration:underline|line-through;");
            else if ((font.Style & FontStyle.Underline) > 0)
                FFontDesc.Append("text-decoration:underline;");
            else if ((font.Style & FontStyle.Strikeout) > 0)
                FFontDesc.Append("text-decoration:line-through;");
            FFontDesc.Append("font-family:").Append(font.Name).Append(";");
            FFontDesc.Append("font-size:").Append(Px(Math.Round(font.Size * 96 / 72)));
            if (LineHeight > 0)
                FFontDesc.Append("line-height:").Append(Px(LineHeight)).Append(";");
            else
                FFontDesc.Append("line-height: 1.2;");
        }

        private void HTMLPadding(FastString PaddingDesc, Padding padding, float ParagraphOffset)
        {
            PaddingDesc.Append("text-indent:").Append(Px(ParagraphOffset));
            PaddingDesc.Append("padding-left:").Append(Px(padding.Left));
            PaddingDesc.Append("padding-right:").Append(Px(padding.Right));
            PaddingDesc.Append("padding-top:").Append(Px(padding.Top));
            PaddingDesc.Append("padding-bottom:").Append(Px(padding.Bottom));
        }

        private string HTMLBorderStyle(BorderLine line)
        {
            switch (line.Style)
            {
                case LineStyle.Dash:
                case LineStyle.DashDot:
                case LineStyle.DashDotDot:
                    return "dashed";
                case LineStyle.Dot:
                    return "dotted";
                case LineStyle.Double:
                    return "double";
                default:
                    return "solid";
            }
        }

        private float HTMLBorderWidth(BorderLine line)
        {
            if (line.Style == LineStyle.Double)
                return (line.Width * 3 * Zoom);
            else
                return line.Width * Zoom;
        }

        private string HTMLBorderWidthPx(BorderLine line)
        {
            if (line.Style != LineStyle.Double && line.Width == 1 && Zoom == 1)
                return "1px;";
            float width;
            if (line.Style == LineStyle.Double)
                width = line.Width * 3 * Zoom;
            else
                width = line.Width * Zoom;
            return Convert.ToString(Math.Round(width, 2), numberFormat) + "px;";
        }

        private void HTMLBorder(FastString BorderDesc, Border border)
        {
            if (!layers)
                BorderDesc.Append("border-collapse: separate;");
            if (border.Lines > 0)
            {
                // bottom
                if ((border.Lines & BorderLines.Bottom) > 0)
                    BorderDesc.Append("border-bottom-width:").
                        Append(HTMLBorderWidthPx(border.BottomLine)).
                        Append("border-bottom-color:").
                        Append(ExportUtils.HTMLColor(border.BottomLine.Color)).Append(";border-bottom-style:").
                        Append(HTMLBorderStyle(border.BottomLine)).Append(";");
                else
                    BorderDesc.Append("border-bottom:none;");
                // top
                if ((border.Lines & BorderLines.Top) > 0)
                    BorderDesc.Append("border-top-width:").
                        Append(HTMLBorderWidthPx(border.TopLine)).
                        Append("border-top-color:").
                        Append(ExportUtils.HTMLColor(border.TopLine.Color)).Append(";border-top-style:").
                        Append(HTMLBorderStyle(border.TopLine)).Append(";");
                else
                    BorderDesc.Append("border-top:none;");
                // left
                if ((border.Lines & BorderLines.Left) > 0)
                    BorderDesc.Append("border-left-width:").
                        Append(HTMLBorderWidthPx(border.LeftLine)).
                        Append("border-left-color:").
                        Append(ExportUtils.HTMLColor(border.LeftLine.Color)).Append(";border-left-style:").
                        Append(HTMLBorderStyle(border.LeftLine)).Append(";");
                else
                    BorderDesc.Append("border-left:none;");
                // right
                if ((border.Lines & BorderLines.Right) > 0)
                    BorderDesc.Append("border-right-width:").
                        Append(HTMLBorderWidthPx(border.RightLine)).
                        Append("border-right-color:").
                        Append(ExportUtils.HTMLColor(border.RightLine.Color)).Append(";border-right-style:").
                        Append(HTMLBorderStyle(border.RightLine)).Append(";");
                else
                    BorderDesc.Append("border-right:none;");
            }
            else
                BorderDesc.Append("border:none;");
        }

        private bool HTMLBorderWidthValues(ReportComponentBase obj, out float left, out float top, out float right, out float bottom)
        {
            Border border = obj.Border;
            left = 0;
            top = 0;
            right = 0;
            bottom = 0;

            if (border.Lines > 0)
            {
                if ((border.Lines & BorderLines.Left) > 0)
                    left += HTMLBorderWidth(border.LeftLine);

                if ((border.Lines & BorderLines.Right) > 0)
                    right += HTMLBorderWidth(border.RightLine);

                if ((border.Lines & BorderLines.Top) > 0)
                    top += HTMLBorderWidth(border.TopLine);

                if ((border.Lines & BorderLines.Bottom) > 0)
                    bottom += HTMLBorderWidth(border.BottomLine);

                return true;
            }

            return false;
        }

        private void HTMLAlign(FastString sb, HorzAlign horzAlign, VertAlign vertAlign, bool wordWrap)
        {
            sb.Append("text-align:");
            if (horzAlign == HorzAlign.Left)
                sb.Append("Left");
            else if (horzAlign == HorzAlign.Right)
                sb.Append("Right");
            else if (horzAlign == HorzAlign.Center)
                sb.Append("Center");
            else if (horzAlign == HorzAlign.Justify)
                sb.Append("Justify");
            sb.Append(";vertical-align:");
            if (vertAlign == VertAlign.Top)
                sb.Append("Top");
            else if (vertAlign == VertAlign.Bottom)
                sb.Append("Bottom");
            else if (vertAlign == VertAlign.Center)
                sb.Append("Middle");
            if (wordWrap)
                sb.Append(";word-wrap:break-word");
            sb.Append(";overflow:hidden;");
        }

        private void HTMLRtl(FastString sb, bool rtl)
        {
            if (rtl)
                sb.Append("direction:rtl;");
        }

        private string HTMLGetStylesHeader()
        {
            FastString header = new FastString();
            if (singlePage && pageBreaks)
            {
                header.AppendLine("<style type=\"text/css\" media=\"print\"><!--");
                header.Append("div." + pageStyleName + 
                    " { page-break-after: always; page-break-inside: avoid; ");
                if (d.page.Landscape)
                {
                    header.Append("width:").Append(Px(maxHeight * Zoom).Replace(";", " !important;"))
                          .Append("transform: rotate(90deg); -webkit-transform: rotate(90deg)");
                }

                header.AppendLine("}")
                      .AppendLine("--></style>");
            }
            header.AppendLine("<style type=\"text/css\"><!-- ");
            return header.ToString();
        }

        private string HTMLGetStyleHeader(long index, long subindex)
        {
            FastString header = new FastString();
            return header.Append(".").
                Append(stylePrefix).
                Append("s").
                Append(index.ToString()).
                Append((singlePage || layers)? String.Empty : String.Concat("-", subindex.ToString())).
                Append(" { ").ToString();
        }

        private void HTMLGetStyle(FastString style, Font Font, Color TextColor, Color FillColor, HorzAlign HAlign, VertAlign VAlign,
            Border Border, Padding Padding, bool RTL, bool wordWrap, float LineHeight, float ParagraphOffset)
        {
            HTMLFontStyle(style, Font, LineHeight);
            style.Append("color:").Append(ExportUtils.HTMLColor(TextColor)).Append(";");
            style.Append("background-color:");
            style.Append(FillColor.A == 0 ? "transparent" : ExportUtils.HTMLColor(FillColor)).Append(";");
            HTMLAlign(style, HAlign, VAlign, wordWrap);
            HTMLBorder(style, Border);
            HTMLPadding(style, Padding, ParagraphOffset);
            HTMLRtl(style, RTL);
            style.AppendLine("}");
        }

        private string HTMLGetStylesFooter()
        {
            return "--></style>";
        }

        private string HTMLGetAncor(string ancorName)
        {
            FastString ancor = new FastString();
            return ancor.Append("<a name=\"PageN").Append(ancorName).Append("\" style=\"padding:0;margin:0;font-size:1px;\"></a>").ToString();
        }

        private string HTMLGetImageTag(string file)
        {
            return "<img src=\"" + file + "\" alt=\"\"/>";
        }

        private string HTMLGetImage(int PageNumber, int CurrentPage, int ImageNumber, string hash, bool Base,
            System.Drawing.Image Metafile, MemoryStream PictureStream, bool isSvg)
        {
            if (pictures)
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Bmp;
                if (imageFormat == ImageFormat.Png)
                    format = System.Drawing.Imaging.ImageFormat.Png;
                else if (imageFormat == ImageFormat.Jpeg)
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (imageFormat == ImageFormat.Gif)
                    format = System.Drawing.Imaging.ImageFormat.Gif;
                string formatNm = isSvg ? "svg" : format.ToString().ToLower();

                string embedImgType = isSvg ? "svg+xml" : format.ToString();
                string embedPreffix = "data:image/" + embedImgType + ";base64,";

                FastString ImageFileNameBuilder = new FastString(48);
                string ImageFileName;
                if (!saveStreams)
                    ImageFileNameBuilder.Append(Path.GetFileName(targetFileName)).Append(".");
                ImageFileNameBuilder.Append(hash).
                    Append(".").Append(formatNm);

                ImageFileName = ImageFileNameBuilder.ToString();

                if (!webMode && !(preview || print))
                {
                    if (Base)
                    {
                        if (Metafile != null && !EmbedPictures)
                        {
                            if (saveStreams)
                            {
                                MemoryStream ImageFileStream = new MemoryStream();
                                Metafile.Save(ImageFileStream, format);
                                GeneratedUpdate(targetPath + ImageFileName, ImageFileStream);
                            }
                            else
                            {
                                using (FileStream ImageFileStream =
                                    new FileStream(targetPath + ImageFileName, FileMode.Create))
                                    Metafile.Save(ImageFileStream, format);
                            }
                        }
                        else if (PictureStream != null && !EmbedPictures)
                        {
                            if (this.format == HTMLExportFormat.HTML)
                            {
                                string fileName = targetPath + ImageFileName;
                                FileInfo info = new FileInfo(fileName);
                                if (!(info.Exists && info.Length == PictureStream.Length))
                                {
                                    if (saveStreams)
                                    {
                                        GeneratedUpdate(targetPath + ImageFileName, PictureStream);
                                    }
                                    else
                                    {
                                        using (FileStream ImageFileStream =
                                        new FileStream(fileName, FileMode.Create))
                                            PictureStream.WriteTo(ImageFileStream);
                                    }
                                }
                            }
                            else
                            {
                                PicsArchiveItem item;
                                item.FileName = ImageFileName;
                                item.Stream = PictureStream;
                                bool founded = false;
                                for (int i = 0; i < picsArchive.Count; i++)
                                    if (item.FileName == picsArchive[i].FileName)
                                    {
                                        founded = true;
                                        break;
                                    }
                                if (!founded)
                                    picsArchive.Add(item);
                            }
                        }
                        if (!saveStreams)
                            GeneratedFiles.Add(targetPath + ImageFileName);
                    }
                    if (EmbedPictures && PictureStream != null)
                    {
                        return embedPreffix + GetBase64Image(PictureStream, hash);
                    }
                    else if (subFolder && singlePage && !navigator)
                        return ExportUtils.HtmlURL(subFolderPath + ImageFileName);
                    else
                        return ExportUtils.HtmlURL(ImageFileName);
                }
                else
                {
                    if (EmbedPictures)
                    {
                        return embedPreffix + GetBase64Image(PictureStream, hash); 
                    }
                    else
                    {
                        if (print || preview)
                        {
                            printPageData.Pictures.Add(PictureStream);
                            printPageData.Guids.Add(hash);
                        }
                        else if (Base)
                        {
                            pages[CurrentPage].Pictures.Add(PictureStream);
                            pages[CurrentPage].Guids.Add(hash);
                        }
                        return webImagePrefix + "=" + hash + webImageSuffix;
                    }
                }
            }
            else
                return String.Empty;
        }

        private string GetBase64Image(MemoryStream PictureStream, string hash)
        {
            string base64Image = String.Empty;
            if (!EmbeddedImages.TryGetValue(hash, out base64Image))
            {
                base64Image = Convert.ToBase64String(PictureStream.ToArray());
                EmbeddedImages.Add(hash, base64Image);
            }
            return base64Image;
        }
    }
}
