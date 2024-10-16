using System;
using System.Threading;
using System.Threading.Tasks;
using FastReport.Preview;

namespace FastReport.Engine
{
    public partial class ReportEngine
    {
        #region Private Methods

        private Task RenderSubreportAsync(SubreportObject subreport, CancellationToken cancellationToken)
        {
            if (subreport.ReportPage != null)
                return RunBandsAsync(subreport.ReportPage.Bands, cancellationToken);
            return Task.CompletedTask;
        }

        private async Task RenderOuterSubreportsAsync(BandBase parentBand, CancellationToken cancellationToken)
        {
            float saveCurY = CurY;
            float saveOriginX = originX;
            int saveCurPage = CurPage;

            float maxY = 0;
            int maxPage = CurPage;
            bool hasSubreports = false;

            try
            {
                for (int i = 0; i < parentBand.Objects.Count; i++)
                {
                    SubreportObject subreport = parentBand.Objects[i] as SubreportObject;

                    // Apply visible expression if needed.
                    if (subreport != null && !String.IsNullOrEmpty(subreport.VisibleExpression))
                    {
                        subreport.Visible = subreport.CalcVisibleExpression(subreport.VisibleExpression);
                    }

                    if (subreport != null && subreport.Visible && !subreport.PrintOnParent)
                    {
                        hasSubreports = true;
                        // restore start position
                        CurPage = saveCurPage;
                        CurY = saveCurY - subreport.Height;
                        originX = saveOriginX + subreport.Left;
                        // do not upload generated pages to the file cache
                        PreparedPages.CanUploadToCache = false;

                        await RenderSubreportAsync(subreport, cancellationToken);

                        // find maxY. We will continue from maxY when all subreports finished.
                        if (CurPage == maxPage)
                        {
                            if (CurY > maxY)
                                maxY = CurY;
                        }
                        else if (CurPage > maxPage)
                        {
                            maxPage = CurPage;
                            maxY = CurY;
                        }
                    }
                }
            }
            finally
            {
                if (hasSubreports)
                {
                    CurPage = maxPage;
                    CurY = maxY;
                }
                originX = saveOriginX;
                PreparedPages.CanUploadToCache = true;
            }
        }

        #endregion Private Methods
    }
}
