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

        public string GetReport(WebReport webReport, GetReportServiceParams @params)
        {
#if DIALOGS
            webReport.Dialogs(@params.DialogParams);

            if (webReport.Canceled)
                return null;

            if (webReport.Mode == WebReportMode.Dialog)
            {
                webReport.Report.PreparePhase1();
            }
            else
#endif
            if (!webReport.ReportPrepared && @params.SkipPrepare != "yes" || @params.ForceRefresh == "yes")
            {
                webReport.Report.Prepare();

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

        public Task<string> GetReportAsync(WebReport webReport, GetReportServiceParams @params, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(GetReport(webReport, @params));
        }

        public async Task<string> InvokeCustomElementAction(WebReport webReport, string elementId, string inputValue)
        {
            var element = webReport.Toolbar.Elements.FirstOrDefault(e =>
            {
                switch (e)
                {
                    case ToolbarButton button:
                        return button.ID.ToString() == elementId;
                    case ToolbarInput input:
                        return input.ID.ToString() == elementId;
                    case ToolbarSelect select:
                        return select.Items.Any(i => i.ID.ToString() == elementId);
                    default:
                        return false;
                }
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

        public void Touch(string reportId)
        {
            _cache.Touch(reportId);
        }
    }
}
