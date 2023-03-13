using FastReport.Web.Cache;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FastReport.Web.Services
{
    internal sealed class ReportService : IReportService
    {
        [Obsolete]
        internal static ReportService Instance { get; } = new ReportService();

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

        public async Task<string> GetReportAsync(WebReport webReport, GetReportServiceParams @params, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(GetReport(webReport, @params));
        }

        public bool TryFindWebReport(string reportId, out WebReport webReport)
        {
            webReport = WebReportCache.Instance.Find(reportId);
            return webReport != null;
        }

        public void Touch(string reportId)
        {
            WebReportCache.Instance.Touch(reportId);
        }
    }
}
