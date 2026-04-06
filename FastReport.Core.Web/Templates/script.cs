using FastReport.Web.Infrastructure;
using System.Text.Json;

namespace FastReport.Web
{
    partial class WebReport
    {
        private class ScriptProps
        {
            public string ID { get; set; }
            public string TemplateFR { get; set; }
            public string SearchNotFoundText { get; set; }
            public string RouteBasePath { get; set; }
            public bool Outline { get; set; }
            public int SearchScroolOffset { get; set; }

            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }

        private class ScriptEventData
        {
            public string TargetObj { get; set; }
            public string Event { get; set; }
            public string Func { get; set; }
            public string[] Params { get; set; }

            public override string ToString()
            {
                return JsonSerializer.Serialize(this);
            }
        }

        /// <summary>
        /// Prepare the data for creating the event in the js module.
        /// </summary>
        /// <param name="ev">The event to which the handler will be added.</param>
        /// <param name="targetObj">The object that will call the function.</param>
        /// <param name="func">The function that is called when the event is triggered</param>
        /// <param name="par">Parameters of function</param>
        /// <returns></returns>
        internal static string CreateEvent(string ev, string targetObj, string func, params string[] par)
        {
            var e = new ScriptEventData()
            {
                Event = ev,
                Func = func,
                Params = par,
                TargetObj = targetObj,
            };
            return $"data-event='{e}'";
        }

        /// <summary>
        /// Prepare the data for creating the onclick event in the js module.
        /// </summary>
        /// <param name="targetObj">The object that will call the function.</param>
        /// <param name="func">The function that is called when the event is triggered</param>
        /// <param name="par">Parameters of function</param>
        /// <returns></returns>
        internal static string CreateOnClickEvent(string targetObj, string func, params string[] par)
        {
            return CreateEvent(JSEvents.CLICK, targetObj, func, par);
        }

        /// <summary>
        /// Entry point for calling js scripts.
        /// </summary>
        public string ScriptName { get => $"window.Webreports.get('{ID}')"; }

        private string GetScriptProps()
        {
            var scriptProps = new ScriptProps()
            {
                ID = this.ID,
                SearchScroolOffset = Toolbar.SearchScrollOffsetTop,
                TemplateFR = "fr",
                SearchNotFoundText = new ToolbarLocalization(Res).searchNotFound,
                RouteBasePath = WebUtils.ToUrl(FastReportGlobal.FastReportOptions.RoutePathBaseRoot, FastReportGlobal.FastReportOptions.RouteBasePath),
                Outline = Outline,
            };

            return scriptProps.ToString();
        }
    }
}