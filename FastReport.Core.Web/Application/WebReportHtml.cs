using FastReport.Export.Html;
using FastReport.Preview;
using FastReport.Table;
using FastReport.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using System.Text;
#if  !OPENSOURCE
using FastReport.Export.Pdf;
#endif

namespace FastReport.Web
{
    public partial class WebReport
    {
#region Internal Methods

#if  !OPENSOURCE
        public IActionResult PrintPdf()
        {
            using (var pdfExport = new PDFExport())
            {
                pdfExport.OpenAfterExport = false;
                //pdfExport.EmbeddingFonts = PdfEmbeddingFonts;
                //pdfExport.TextInCurves = PdfTextInCurves;
                //pdfExport.Background = PdfBackground;
                //pdfExport.PrintOptimized = PdfPrintOptimized;
                //pdfExport.Title = PdfTitle;
                //pdfExport.Author = PdfAuthor;
                //pdfExport.Subject = PdfSubject;
                //pdfExport.Keywords = PdfKeywords;
                //pdfExport.Creator = PdfCreator;
                //pdfExport.Producer = PdfProducer;
                //pdfExport.Outline = PdfOutline;
                //pdfExport.DisplayDocTitle = PdfDisplayDocTitle;
                //pdfExport.HideToolbar = PdfHideToolbar;
                //pdfExport.HideMenubar = PdfHideMenubar;
                //pdfExport.HideWindowUI = PdfHideWindowUI;
                //pdfExport.FitWindow = PdfFitWindow;
                //pdfExport.CenterWindow = PdfCenterWindow;
                //pdfExport.PrintScaling = PdfPrintScaling;
                //pdfExport.UserPassword = PdfUserPassword;
                //pdfExport.OwnerPassword = PdfOwnerPassword;
                //pdfExport.AllowPrint = PdfAllowPrint;
                //pdfExport.AllowCopy = PdfAllowCopy;
                //pdfExport.AllowModify = PdfAllowModify;
                //pdfExport.AllowAnnotate = PdfAllowAnnotate;
                //pdfExport.PdfCompliance = PdfA ? PDFExport.PdfStandard.PdfA_2a : PDFExport.PdfStandard.None;
                pdfExport.ShowPrintDialog = true;
                pdfExport.ExportMode = PDFExport.ExportType.WebPrint;

                using (MemoryStream ms = new MemoryStream())
                {
                    pdfExport.Export(Report, ms);
                    return new FileContentResult(ms.ToArray(), "application/pdf");
                }
            }
        }
#endif

        public IActionResult PrintHtml()
        {
            PictureCache.Clear();

            using (var htmlExport = new HTMLExport())
            {
                htmlExport.OpenAfterExport = false;
                htmlExport.Navigator = false;
                htmlExport.Layers = Layers;
                htmlExport.SinglePage = true;
                htmlExport.Pictures = Pictures;
                htmlExport.Print = true;
                htmlExport.Preview = true;
                htmlExport.SubFolder = false;
                htmlExport.EmbedPictures = EmbedPictures;
                //htmlExport.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, controller.RouteBasePath, ID, "picture") + "/";
                htmlExport.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, $"preview.getPicture?reportId={ID}&pictureId=");
                htmlExport.ExportMode = HTMLExport.ExportType.WebPrint;

                byte[] file = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    htmlExport.Export(Report, ms);
                    file = ms.ToArray();
                }

                if (htmlExport.PrintPageData != null)
                {
                    //WebReportCache cache = new WebReportCache(this.Context);

                    // add all pictures in cache
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        byte[] image = new byte[stream.Length];
                        stream.Position = 0;
                        int n = stream.Read(image, 0, (int)stream.Length);
                        string picGuid = htmlExport.PrintPageData.Guids[i];
                        //cache.PutObject(picGuid, image);
                        PictureCache[picGuid] = image;
                    }

                    // cleanup
                    for (int i = 0; i < htmlExport.PrintPageData.Pictures.Count; i++)
                    {
                        Stream stream = htmlExport.PrintPageData.Pictures[i];
                        stream.Dispose();
                        stream = null;
                    }

                    htmlExport.PrintPageData.Pictures.Clear();
                    htmlExport.PrintPageData.Guids.Clear();
                }

                return new FileContentResult(file, "text/html");
            }
        }

        internal void Dialogs(HttpRequest request)
        {
#if DIALOGS
            string dialogN = request.Query["dialog"];
            string controlName = request.Query["control"];
            string eventName = request.Query["event"];
            string data = request.Query["data"];

            Dialog.SetDialogs(dialogN, controlName, eventName, data);
#endif
        }


        internal StringBuilder ReportBody()
        {
#if DIALOGS
            if (Mode == WebReportMode.Dialog)
            {
                StringBuilder sb = new StringBuilder();

                Dialog.ProcessDialogs(sb);

                return sb;
            }
            else
#endif
                return ReportInHtml();
        }

        internal StringBuilder ReportInHtml()
        {
            PictureCache.Clear();

            var sb = new StringBuilder();

            using (HTMLExport html = new HTMLExport())
            {
                html.ExportMode = HTMLExport.ExportType.WebPreview;
                //html.CustomDraw += this.CustomDraw;
                html.StylePrefix = $"fr{ID}"; //html.StylePrefix = Prop.ControlID.Substring(0, 6);
                html.Init_WebMode();
                html.Pictures = Pictures; //html.Pictures = Prop.Pictures;
                html.EmbedPictures = EmbedPictures; //html.EmbedPictures = EmbedPictures;
                html.OnClickTemplate = "fr{0}.click(this,'{1}','{2}')";
                html.ReportID = ID; //html.ReportID = Prop.ControlID;
                html.EnableMargins = EnableMargins; //html.EnableMargins = Prop.EnableMargins;

                // calc zoom
                //CalcHtmlZoom(html);
                html.Zoom = Zoom;

                html.Layers = Layers; //html.Layers = Layers;
                html.PageNumbers = SinglePage ? "" : (CurrentPageIndex + 1).ToString(); //html.PageNumbers = SinglePage ? "" : (Prop.CurrentPage + 1).ToString();

                //if (Prop.AutoWidth)
                //    html.WidthUnits = HtmlSizeUnits.Percent;
                //if (Prop.AutoHeight)
                //    html.HeightUnits = HtmlSizeUnits.Percent;

                //html.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, controller.RouteBasePath, ID, "picture") + "/"; //html.WebImagePrefix = String.Concat(context.Response.ApplyAppPathModifier(WebUtils.HandlerFileName), "?", WebUtils.PicsPrefix);
                html.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RouteBasePath, $"preview.getPicture?reportId={ID}&pictureId=");
                html.SinglePage = SinglePage; //html.SinglePage = SinglePage;
                html.CurPage = CurrentPageIndex; //html.CurPage = CurrentPage;
                html.Export(Report, (Stream)null);

                //sb.Append("<div class=\"frbody\" style =\"");
                //if (HtmlLayers)
                //    sb.Append("position:relative;z-index:0;");
                //sb.Append("\">");

                // container for html report body
                //int pageWidth = (int)Math.Ceiling(GetReportPageWidthInPixels() * html.Zoom);
                //int pageHeight = (int)Math.Ceiling(GetReportPageHeightInPixels() * html.Zoom);
                //int paddingLeft = (int)Math.Ceiling(Padding.Left * html.Zoom);
                //int paddingRight = (int)Math.Ceiling(Padding.Right * html.Zoom);
                //int paddingTop = (int)Math.Ceiling(Padding.Top * html.Zoom);
                //int paddingBottom = (int)Math.Ceiling(Padding.Bottom * html.Zoom);
                //sb.Append("<div class=\"frcontainer\" style=\"width:" + pageWidth +
                //    "px;height:" + (SinglePage ? pageHeight * html.Count : pageHeight) +
                //    "px;padding-left:" + paddingLeft +
                //    "px;padding-right:" + paddingRight +
                //    "px;padding-top:" + paddingTop +
                //    "px;padding-bottom:" + paddingBottom + "px\">");

                if (html.Count > 0)
                {
                    if (SinglePage)
                    {
                        DoAllHtmlPages(sb, html);
                        CurrentPageIndex = 0; //Prop.CurrentPage = 0;
                    }
                    else
                    {
                        DoHtmlPage(sb, html, 0);
                    }
                }

                //sb.Append("</div>");
                //sb.Append("</div>");

                // important container, it cuts off elements that are outside of the report page bounds
                int pageWidth = (int)Math.Ceiling(GetReportPageWidthInPixels() * html.Zoom);
                int pageHeight = (int)Math.Ceiling(GetReportPageHeightInPixels() * html.Zoom);
                sb.Insert(0, $@"<div style=""width:{pageWidth}px;height:{pageHeight}px;overflow:hidden;display:inline-block;"">");
                sb.Append("</div>");
            }

            return sb;
        }

        internal void ProcessClick(HttpRequest request)
        {
            var click = request.Query["click"].ToString();
            if (!click.IsNullOrWhiteSpace())
            {
                var @params = click.Split(',');
                if (@params.Length == 4)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        DoClickObjectByParamID(@params[0], pageN, left, top);
                    }
                }
                return;
            }

            var checkbox_click = request.Query["checkbox_click"].ToString();
            if (!checkbox_click.IsNullOrWhiteSpace())
            {
                var @params = checkbox_click.Split(',');
                if (@params.Length == 4)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        Report.FindClickedObject<CheckBoxObject>(@params[0], pageN, left, top,
                            (obj, reportPage, _pageN) =>
                            {
                                obj.Checked = !obj.Checked;

                                if (Report.NeedRefresh)
                                    Report.InteractiveRefresh();
                                else
                                    Report.PreparedPages.ModifyPage(_pageN, reportPage);
                            });
                    }
                }
                return;
            }

            var text_edit = request.Query["text_edit"].ToString();
            if (!text_edit.IsNullOrWhiteSpace())
            {
                var text = request.Form["text"].ToString();
                var @params = text_edit.Split(',');
                if (@params.Length == 4 && text != null)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        Report.FindClickedObject<TextObject>(@params[0], pageN, left, top,
                            (obj, reportPage, _pageN) =>
                            {
                                obj.Text = text;

                                if (Report.NeedRefresh)
                                    Report.InteractiveRefresh();
                                else
                                    Report.PreparedPages.ModifyPage(_pageN, reportPage);
                            });
                    }
                }
            }
        }

        internal void SetReportZoom(HttpRequest request)
        {
            if (int.TryParse(request.Query["zoom"].ToString(), out int result))
                if (result / 100f > 0f)
                    Zoom = result / 100f;
        }

        internal void SetReportPage(HttpRequest request)
        {
            string @goto = request.Query["goto"].ToString();
            if (!@goto.IsNullOrWhiteSpace())
            {
                switch (@goto)
                {
                    case "first":
                        FirstPage();
                        break;
                    case "last":
                        LastPage();
                        break;
                    case "prev":
                        PrevPage();
                        break;
                    case "next":
                        NextPage();
                        break;
                    default:
                        if (int.TryParse(@goto, out int value))
                            GotoPage(value - 1);
                        break;
                }
                return;
            }

            string bookmark = request.Query["bookmark"].ToString();
            if (!bookmark.IsNullOrWhiteSpace())
            {
                int i = PageNByBookmark(WebUtility.UrlDecode(bookmark));
                if (i != -1)
                    GotoPage(i);
                return;
            }

            string detailed_report = request.Query["detailed_report"].ToString();
            if (!detailed_report.IsNullOrWhiteSpace())
            {
                string[] detailParams = WebUtility.UrlDecode(detailed_report).Split(',');
                if (detailParams.Length == 3)
                {
                    if (!String.IsNullOrEmpty(detailParams[0]) &&
                        !String.IsNullOrEmpty(detailParams[1]) &&
                        !String.IsNullOrEmpty(detailParams[2])
                        )
                    {
                        DoDetailedReport(detailParams[0], detailParams[1], detailParams[2]);
                    }
                }
                return;
            }

            string detailed_page = request.Query["detailed_page"].ToString();
            if (!detailed_page.IsNullOrWhiteSpace())
            {
                string[] detailParams = WebUtility.UrlDecode(detailed_page).Split(',');
                if (detailParams.Length == 3)
                {
                    if (!String.IsNullOrEmpty(detailParams[0]) &&
                        !String.IsNullOrEmpty(detailParams[1]) &&
                        !String.IsNullOrEmpty(detailParams[2])
                        )
                    {
                        DoDetailedPage(detailParams[0], detailParams[1], detailParams[2]);
                    }
                }
                return;
            }
        }

        internal void SetReportTab(HttpRequest request)
        {
            string settab = request.Query["settab"].ToString();
            if (!settab.IsNullOrWhiteSpace())
            {
                int i = 0;
                if (int.TryParse(settab, out i))
                    //if (i >= 0 && i < Tabs.Count)
                    CurrentTabIndex = i;
            }

            string closetab = request.Query["closetab"].ToString();
            if (!closetab.IsNullOrWhiteSpace())
            {
                int i = 0;
                if (int.TryParse(closetab, out i) && (i >= 0 && i < Tabs.Count))
                {
                    var activeTab = CurrentTab;

                    Tabs[i].Report.Dispose();
                    Tabs.RemoveAt(i);

                    if (activeTab == null)
                    {
                        CurrentTabIndex = 0;
                    }
                    else
                    {
                        for (int j = 0; j < Tabs.Count; j++)
                            if (Tabs[j] == activeTab)
                            {
                                CurrentTabIndex = j;
                                break;
                            }
                    }

                    //if (i < Tabs.Count)
                    //    CurrentTabIndex = i;
                    //else
                    //    CurrentTabIndex = i - 1;
                }
            }
        }

#endregion

#region Private Methods

        private int PageNByBookmark(string bookmark)
        {
            int pageN = -1;
            if (Report.PreparedPages != null)
            {
                for (int i = 0; i < Report.PreparedPages.Count; i++)
                {
                    ReportPage page = Report.PreparedPages.GetPage(i);
                    if (page != null)
                    {
                        ObjectCollection allObjects = page.AllObjects;
                        for (int j = 0; j < allObjects.Count; j++)
                        {
                            ReportComponentBase c = allObjects[j] as ReportComponentBase;
                            if (c.Bookmark == bookmark)
                            {
                                pageN = i;
                                break;
                            }
                        }
                        page.Dispose();
                        if (pageN != -1)
                            break;
                    }
                }
            }
            return pageN;
        }

        private void DoClickObjectByParamID(string objectName, int pageN, float left, float top)
        {
            if (Report.PreparedPages != null)
            {
                bool found = false;
                while (pageN < Report.PreparedPages.Count && !found)
                {
                    ReportPage page = Report.PreparedPages.GetPage(pageN);
                    if (page != null)
                    {
                        ObjectCollection allObjects = page.AllObjects;
                        System.Drawing.PointF point = new System.Drawing.PointF(left + 1, top + 1);
                        foreach (Base obj in allObjects)
                        {
                            if (obj is ReportComponentBase)
                            {
                                ReportComponentBase c = obj as ReportComponentBase;
                                if (c is TableBase)
                                {
                                    TableBase table = c as TableBase;
                                    for (int i = 0; i < table.RowCount; i++)
                                    {
                                        for (int j = 0; j < table.ColumnCount; j++)
                                        {
                                            TableCell textcell = table[j, i];
                                            if (textcell.Name == objectName)
                                            {
                                                System.Drawing.RectangleF rect =
                                                    new System.Drawing.RectangleF(table.Columns[j].AbsLeft,
                                                    table.Rows[i].AbsTop,
                                                    textcell.Width,
                                                    textcell.Height);
                                                if (rect.Contains(point))
                                                {
                                                    Click(textcell, pageN, page);
                                                    found = true;
                                                    break;
                                                }
                                            }
                                        }
                                        if (found)
                                            break;
                                    }
                                }
                                else
                                if (c.Name == objectName &&
                                  //#if FRCORE
                                  c.AbsBounds.Contains(point))
                                //#else
                                //                                  c.PointInObject(point))
                                //#endif
                                {
                                    Click(c, pageN, page);
                                    found = true;
                                    break;
                                }
                            }
                        }
                        page.Dispose();
                        pageN++;
                    }
                }
            }
        }

        private void Click(ReportComponentBase c, int pageN, ReportPage page)
        {
            c.OnClick(null);
            if (Report.NeedRefresh)
                Report.InteractiveRefresh();
            else
                Report.PreparedPages.ModifyPage(pageN, page);
        }

        private void DoDetailedReport(string objectName, string paramName, string paramValue)
        {
            if (!String.IsNullOrEmpty(objectName))
            {
                Report tabReport = new Report();
                ReportComponentBase obj = Report.FindObject(objectName) as ReportComponentBase;
                if (obj != null && obj.Hyperlink.Kind == HyperlinkKind.DetailReport)
                {
                    string fileName = obj.Hyperlink.DetailReportName;
                    if (File.Exists(fileName))
                    {
                        tabReport.Load(fileName);

                        Data.Parameter param = tabReport.Parameters.FindByName(paramName);
                        if (param != null && param.ChildObjects.Count > 0)
                        {
                            string[] paramValues = paramValue.Split(obj.Hyperlink.ValuesSeparator[0]);
                            if (paramValues.Length > 0)
                            {
                                int i = 0;
                                foreach (Data.Parameter childParam in param.ChildObjects)
                                {
                                    childParam.Value = paramValues[i++];
                                    if (i >= paramValues.Length)
                                        break;
                                }
                            }
                        }
                        else
                            tabReport.SetParameterValue(paramName, paramValue);
                        Report.Dictionary.ReRegisterData(tabReport.Dictionary);

                        tabReport.PreparePhase1();
                        tabReport.PreparePhase2();

                        Tabs.Add(new ReportTab()
                        {
                            Name = paramValue,
                            Report = tabReport,
                            ReportPrepared = true,
                            Closeable = true,
                            NeedParent = false
                        });

                        int prevTab = CurrentTabIndex;
                        CurrentTabIndex = Tabs.Count - 1;
                        //Prop.PreviousTab = prevTab;
                    }
                }
            }
        }

        private void DoDetailedPage(string objectName, string paramName, string paramValue)
        {
            if (!String.IsNullOrEmpty(objectName))
            {
                Report currentReport = CurrentTab.NeedParent ? Tabs[0].Report : Report;
                ReportComponentBase obj = currentReport.FindObject(objectName) as ReportComponentBase;
                if (obj != null && obj.Hyperlink.Kind == HyperlinkKind.DetailPage)
                {
                    ReportPage reportPage = currentReport.FindObject(obj.Hyperlink.DetailPageName) as ReportPage;
                    if (reportPage != null)
                    {
                        Data.Parameter param = currentReport.Parameters.FindByName(paramName);
                        if (param != null && param.ChildObjects.Count > 0)
                        {
                            string[] paramValues = paramValue.Split(obj.Hyperlink.ValuesSeparator[0]);
                            if (paramValues.Length > 0)
                            {
                                int i = 0;
                                foreach (Data.Parameter childParam in param.ChildObjects)
                                {
                                    childParam.Value = paramValues[i++];
                                    if (i >= paramValues.Length)
                                        break;
                                }
                            }
                        }
                        else
                            currentReport.SetParameterValue(paramName, paramValue);
                        PreparedPages oldPreparedPages = currentReport.PreparedPages;
                        PreparedPages pages = new PreparedPages(currentReport);
                        currentReport.SetPreparedPages(pages);
                        currentReport.PreparePage(reportPage);
                        Report tabReport = new Report();
                        tabReport.SetPreparedPages(currentReport.PreparedPages);
                        Tabs.Add(new ReportTab()
                        {
                            Name = paramValue,
                            Report = tabReport,
                            ReportPrepared = true,
                            Closeable = true,
                            NeedParent = true
                        });

                        int prevTab = CurrentTabIndex;
                        currentReport.SetPreparedPages(oldPreparedPages);
                        CurrentTabIndex = Tabs.Count - 1;
                        //Prop.PreviousTab = prevTab;
                    }
                }
            }
        }

        void DoHtmlPage(StringBuilder sb, HTMLExport html, int pageN)
        {
            if (html.PreparedPages[pageN].PageText == null)
            {
                html.PageNumbers = (pageN + 1).ToString();
                html.Export(Report, (Stream)null);
            }

            //Prop.CurrentWidth = html.PreparedPages[pageN].Width;
            //Prop.CurrentHeight = html.PreparedPages[pageN].Height;

            if (html.PreparedPages[pageN].CSSText != null &&
                html.PreparedPages[pageN].PageText != null)
            {
                sb.Append(html.PreparedPages[pageN].CSSText);
                sb.Append(html.PreparedPages[pageN].PageText);

                if (!EmbedPictures)
                    CacheHtmlPictures(html, pageN);
            }
        }

        void DoAllHtmlPages(StringBuilder sb, HTMLExport html)
        {
            //Prop.CurrentWidth = 0;
            //Prop.CurrentHeight = 0;

            for (int pageN = 0; pageN < html.PreparedPages.Count; pageN++)
            {
                if (html.PreparedPages[pageN].PageText == null)
                {
                    html.PageNumbers = (pageN + 1).ToString();
                    html.Export(Report, (Stream)null);

                    //if (html.PreparedPages[pageN].Width > Prop.CurrentWidth)
                    //    Prop.CurrentWidth = html.PreparedPages[pageN].Width;
                    //if (html.PreparedPages[pageN].Height > Prop.CurrentHeight)
                    //    Prop.CurrentHeight = html.PreparedPages[pageN].Height;
                }

                if (html.PreparedPages[pageN].CSSText != null &&
                    html.PreparedPages[pageN].PageText != null)
                {
                    sb.Append(html.PreparedPages[pageN].CSSText);
                    sb.Append(html.PreparedPages[pageN].PageText);

                    if (!EmbedPictures)
                        CacheHtmlPictures(html, Layers ? 0 : pageN);
                }
            }
        }

        void CacheHtmlPictures(HTMLExport html, int pageN)
        {
            //WebReportCache cache = new WebReportCache(this.Context);
            for (int i = 0; i < html.PreparedPages[pageN].Pictures.Count; i++)
            {
                try
                {
                    Stream picStream = html.PreparedPages[pageN].Pictures[i];
                    byte[] image = new byte[picStream.Length];
                    picStream.Position = 0;
                    int n = picStream.Read(image, 0, (int)picStream.Length);
                    string guid = html.PreparedPages[pageN].Guids[i];
                    //cache.PutObject(guid, image);
                    PictureCache[guid] = image;
                }
                catch
                {
                    //Log.AppendFormat("Error with picture: {0}\n", i.ToString());
                }
            }
        }

        float GetReportPageWidthInPixels()
        {
            float _pageWidth = 0;

            if (SinglePage)
            {
                foreach (ReportPage page in Report.Pages)
                {
                    // find maxWidth
                    if (page.WidthInPixels > _pageWidth)
                        _pageWidth = page.WidthInPixels;
                }
            }
            else
            {
                _pageWidth = Report.PreparedPages.GetPageSize(CurrentPageIndex).Width;
            }

            return _pageWidth;
        }

        float GetReportPageHeightInPixels()
        {
            float _pageHeight = 0;
            if (SinglePage)
            {
                for (int i = 0; i < Report.PreparedPages.Count; i++)
                {
                    _pageHeight += Report.PreparedPages.GetPageSize(i).Height;
                }
            }
            else
            {
                _pageHeight = Report.PreparedPages.GetPageSize(CurrentPageIndex).Height;
            }

            return _pageHeight;
        }

        #endregion
    }
}
