using FastReport.Export.Html;
using System.IO;
using System.Text;
using System.Collections.Generic;

#if !OPENSOURCE
using FastReport.AdvMatrix;
#endif

namespace FastReport.Web
{
    public partial class WebReport
    {
        #region Internal Methods

        internal void ExportReport(Stream stream, string exportFormat,
            IEnumerable<KeyValuePair<string, string>> exportParameters)
        {
            var exportInfo = ExportsHelper.GetInfoFromExt(exportFormat);
            ExportReport(stream, exportInfo, exportParameters);
        }

        internal void ExportReport(Stream stream, ExportsHelper.ExportInfo exportInfo,
            IEnumerable<KeyValuePair<string, string>> exportParameters)
        {
            using var export = ReportExporter.CreateExport(exportInfo, exportParameters);

            var strategy = ExportStrategyFactory.GetExportStrategy(exportInfo, export);

            var exporter = new ReportExporter(strategy);
            exporter.ExportReport(stream, Report, export);

            stream.Position = 0;
        }

        #endregion

        #region Private Methods

        private void Refresh(int pageN, ReportPage page)
        {
            if (Report.NeedRefresh)
                Report.InteractiveRefresh();
            else
                Report.PreparedPages.ModifyPage(pageN, page);
        }

        private void GoToPageNumber(string @goto)
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
        }

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
                            if (c?.Bookmark == bookmark)
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
        private void DoClickAdvancedMatrixByParamID(string objectName, int pageN, int index)
        {
#if !OPENSOURCE
            if (Report.PreparedPages != null)
            {
                bool found = false;
                while (pageN < Report.PreparedPages.Count && !found)
                {
                    ReportPage page = Report.PreparedPages.GetPage(pageN);
                    if (page != null)
                    {
                        ObjectCollection allObjects = page.AllObjects;
                        foreach (Base obj in allObjects)
                        {
                            if (obj is MatrixCollapseButton collapseButton)
                            {
                                if (collapseButton.Name == objectName
                                    && collapseButton.Index == index)
                                {
                                    collapseButton.MatrixCollapseButtonClick();
                                    Refresh(pageN, page);
                                    found = true;
                                    break;
                                }
                            }
                            else if(obj is MatrixSortButton sortButton)
                            {
                                if(sortButton.Name == objectName
                                    && sortButton.Index == index)
                                {
                                    sortButton.MatrixSortButtonClick();
                                    Refresh(pageN, page);
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
#endif
        }

        private void Click(ReportComponentBase c, ReportPage page, int pageN)
        {
            c.OnClick(null);
            Refresh(pageN, page);
        }

        private void CheckboxClick(CheckBoxObject checkBox, ReportPage page, int pageN)
        {
            checkBox.Checked = !checkBox.Checked;
            Refresh(pageN, page);
        }

        private void DoDetailedReport(ReportComponentBase obj, string value)
        {
            if (obj != null)
            {
                string fileName = obj.Hyperlink.DetailReportName;
                if (File.Exists(fileName))
                {
                    Report tabReport = new Report();
                    tabReport.Load(fileName);
                    string paramName = obj.Hyperlink.ReportParameter;
                    string paramValue = string.IsNullOrEmpty(obj.Hyperlink.Value) ? value : obj.Hyperlink.Value;

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
                        Closeable = true,
                        NeedParent = false
                    });

                    CurrentTabIndex = Tabs.Count - 1;
                }
            }
        }

        internal void DoHtmlPage(StringBuilder sb, HTMLExport html, int pageN)
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

                if (!html.EmbedPictures)
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

        internal float GetReportPageWidthInPixels()
        {
            float _pageWidth = 0;

            if (SinglePage)
            {
                foreach (PageBase page in Report.Pages)
                {
                    // find maxWidth
                    if (page is ReportPage)
                    {
                        var _page = page as ReportPage;
                        if (_page.WidthInPixels > _pageWidth)
                            _pageWidth = _page.WidthInPixels;
                    }
                }
            }
            else
            {
                _pageWidth = Report.PreparedPages.GetPageSize(CurrentPageIndex).Width;
            }

            return _pageWidth;
        }

        internal float GetReportPageHeightInPixels()
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
