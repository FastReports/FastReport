using FastReport.Web.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using FastReport.Web.Toolbar;

namespace FastReport.Web.Services
{
    internal sealed class ReportService : IReportService
    {

        private readonly IWebReportCache _cache;

        public ReportService(IWebReportCache webReportCache)
        {
            _cache = webReportCache;
        }


        public async Task<string> GetReportAsync(WebReport webReport, GetReportServiceParams @params, CancellationToken cancellationToken = default)
        {
#if DIALOGS
            webReport.Dialogs(@params.DialogParams);

            if (webReport.Canceled)
                return null;

            if (webReport.Mode == WebReportMode.Dialog)
            {
                await webReport.Report.PreparePhase1Async(cancellationToken);
            }
            else
#endif
            if (!webReport.ReportPrepared && @params.SkipPrepare != "yes" || @params.ForceRefresh == "yes")
            {
                // don't reset the data state if we run the dialog page or refresh a report.
                // This is necessary to keep data filtering settings alive
                var resetDataState = @params.ForceRefresh != "yes" && string.IsNullOrEmpty(@params.DialogParams.DialogN);
                await webReport.Report.PrepareAsync(false, resetDataState, cancellationToken);

                webReport.SplitReportPagesByTabs();
            }

            webReport.SetReportTab(@params.ReportTabParams);
            webReport.SetReportPage(@params.ReportPageParams);
            webReport.SetReportZoom(@params.Zoom);
            webReport.ProcessClick(@params.ClickParams);


            //bool renderBodyBool = query.RenderBody ?? default;
            bool renderBodyBool = @params.RenderBody == "yes";

            return webReport.Render(renderBodyBool).ToString();
        }

        public async Task<string> InvokeCustomElementAction(WebReport webReport, string elementId, string inputValue)
        {
            var element = webReport.Toolbar.Elements.FirstOrDefault(e =>
            {
                return e switch
                {
                    ToolbarButton button => button.ID.ToString() == elementId,
                    ToolbarInput input => input.ID.ToString() == elementId,
                    ToolbarSelect select => select.Items.Any(i => i.ID.ToString() == elementId),
                    _ => false,
                };
            });

            switch (element)
            {
                case ToolbarSelect toolbarSelect:
                    {
                        var item = toolbarSelect.Items.FirstOrDefault(i => i.ID.ToString() == elementId);

                        if (item?.OnClickAction is ElementClickAction elementAction)
                            await elementAction.OnClickAction(webReport);
                        break;
                    }
                case ToolbarButton button:
                    {
                        if (button.OnClickAction is ElementClickAction elementAction && elementAction.OnClickAction != null)
                            await elementAction.OnClickAction(webReport);
                        break;
                    }
                case ToolbarInput input when input.OnChangeAction is ElementChangeAction elementAction:
                    await elementAction.OnChangeAction(webReport, inputValue);
                    break;
            }

            return webReport.Render(true).ToString();
        }


        public bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = _cache.Find(reportId);
            return webReport != null;
        }

        public bool Touch(string reportId)
        {
            return _cache.Touch(reportId);
        }

#if !OPENSOURCE
        public bool SearchText(WebReport webReport, ReportSearchParams @params)
        {
           return webReport.ReportSearch(@params.SearchText, @params.Backward == "true", @params.MatchCase == "true", @params.WholeWord == "true");
        }
#endif
    }
}
