using FastReport.Preview;
using FastReport.Table;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;
using System.Globalization;
using FastReport.Web.Services;
using FastReport.Export.Html;
using FastReport.Web.Infrastructure;

namespace FastReport.Web
{
    public partial class WebReport
    {

        internal StringBuilder ReportBody()
        {
#if DIALOGS
            if (Mode == WebReportMode.Dialog)
            {
                StringBuilder sb = new StringBuilder(4096);

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

                html.WebImagePrefix = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RoutePathBaseRoot, FastReportGlobal.FastReportOptions.RouteBasePath, $"preview.getPicture?reportId={ID}&pictureId=");
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

                // important container, it cuts off elements that are outside of the report page bounds
                int pageWidth = (int)Math.Ceiling(GetReportPageWidthInPixels() * html.Zoom);
                int pageHeight = (int)Math.Ceiling(GetReportPageHeightInPixels() * html.Zoom);
                ReportMaxWidth = pageWidth;
                sb.Append($@"<div style=""width:{pageWidth}px;height:{pageHeight}px;overflow:hidden;display:inline-block;"">");

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
                sb.Append("</div>");
            }

            return sb;
        }


#if DIALOGS
        internal void Dialogs(DialogParams @params)
        {
            string dialogN = @params.DialogN;
            string controlName = @params.DialogControlName;
            string eventName = @params.DialogEventName;
            string data = @params.DialogData;

            Dialog.SetDialogs(dialogN, controlName, eventName, data);
        }
#endif
        internal void ProcessClick(ClickParams clickParams)
        {
            var click = clickParams.Click;
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

            var checkbox_click = clickParams.CheckBoxClick;
            if (!checkbox_click.IsNullOrWhiteSpace())
            {
                var @params = checkbox_click.Split(',');
                if (@params.Length == 4)
                {
                    if (int.TryParse(@params[1], out var pageN) &&
                        float.TryParse(@params[2], out var left) &&
                        float.TryParse(@params[3], out var top))
                    {
                        Report.FindClickedObject<CheckBoxObject>(@params[0], pageN, left, top, CheckboxClick);
                    }
                }
                return;
            }

            var text_edit = clickParams.TextEdit;
            if (!text_edit.IsNullOrWhiteSpace())
            {
                var text = clickParams.Text;
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

                                Refresh(_pageN, reportPage);
                            });
                    }
                }
                return;
            }

            var advmatrix_button = clickParams.AdvMatrixClick;
            if (!advmatrix_button.IsNullOrWhiteSpace())
            {
                var @params = advmatrix_button.Split(',');
                if (@params.Length == 3)
                {
                    if (
                        int.TryParse(@params[1], out var pageN) &&
                        int.TryParse(@params[2], out var index))
                    {
                        DoClickAdvancedMatrixByParamID(@params[0], pageN, index);
                    }
                }
            }

        }

        internal void SetReportZoom(string zoom)
        {
            if (int.TryParse(zoom, out int result))
                if (result / 100f > 0f)
                    Zoom = result / 100f;
        }

        internal void SetReportPage(ReportPageParams @params)
        {
            string @goto = @params.GoTo;
            if (!@goto.IsNullOrWhiteSpace())
            {
                GoToPageNumber(@goto);
                return;
            }

            string bookmark = @params.Bookmark;
            if (!bookmark.IsNullOrWhiteSpace())
            {
                int i = PageNByBookmark(WebUtility.UrlDecode(bookmark));
                if (i != -1)
                    GotoPage(i);
                return;
            }

            string detailed_report = @params.DetailedReport;
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

            string detailed_page = @params.DetailedPage;
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

        internal void SetReportTab(ReportTabParams @params)
        {
            string settab = @params.SetTab;
            if (!settab.IsNullOrWhiteSpace())
            {
                if (int.TryParse(settab, out int i))
                {
                    //if (i >= 0 && i < Tabs.Count)
                    SetTab(i);
                }
            }

            string closetab = @params.CloseTab;
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


        #region Private Methods


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
                                                    Click(textcell, page, pageN);
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
                                    Click(c, page, pageN);
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

        private void DoDetailedReport(string objectName, string paramName, string paramValue)
        {
            if (!String.IsNullOrEmpty(objectName))
            {
                ReportComponentBase obj = Report.FindObject(objectName) as ReportComponentBase;
                DoDetailedReport(obj, paramValue);
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
                        currentReport.PreparePage(reportPage, true);
                        Report tabReport = new Report();
                        tabReport.SetPreparedPages(currentReport.PreparedPages);
                        Tabs.Add(new ReportTab()
                        {
                            Name = paramValue,
                            Report = tabReport,
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

        #endregion
    }
}
